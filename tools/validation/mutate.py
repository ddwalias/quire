"""Byte-level IL mutation of compared methods: every mutant must make a comparison against Mathcad fail its test."""
from concurrent.futures import ThreadPoolExecutor
import hashlib
import json
import os
import queue
import re
import signal
from pathlib import Path
import shutil
import subprocess
import tempfile
import threading
import time
import uuid
import xml.etree.ElementTree as ET
from validate import NS, ROOT, digest, files_under, sdk_directory

ACCEPTED = ROOT / 'docs/mirror/accepted-mutants.txt'
# A mutant's timeout: its tests' unmutated duration times MARGIN, at least FLOOR s. A false timeout counts as a kill and
# could hide a weak test, so the floor stays generous; only mutants that loop pay it.
MUTANT_MARGIN, MUTANT_FLOOR = 4, 30
# Outcomes of earlier runs of identical mutants (see cached_outcome). Bump the version when what an outcome means changes.
CACHE_VERSION = 1
CACHEABLE = {'killed', 'killed (via another method compared first)', 'killed (crash)', 'survived',
             'survived (tests failed without a mismatch)'}


def cache_directory():
    """Outside any repository; QUIRE_MUTANT_CACHE overrides the default and is withheld from code under test."""
    directory = Path(os.environ.get('QUIRE_MUTANT_CACHE') or Path.home() / '.cache/quire-mutants').resolve()
    if directory.is_relative_to(ROOT.resolve()):
        raise ValueError(f'mutant cache {directory} lies inside the repository under validation')
    return directory


def cache_key(*parts):
    return hashlib.sha256(json.dumps([CACHE_VERSION, *parts], sort_keys=True).encode()).hexdigest()


def cached_outcome(directory, digest_key):
    """The recorded outcome for this key, or None (absent or unreadable)."""
    try:
        outcome = json.loads((directory / digest_key[:2] / f'{digest_key}.json').read_text())['outcome']
    except (OSError, ValueError, KeyError, TypeError):
        return None
    return outcome if outcome in CACHEABLE else None


def store_outcome(directory, digest_key, outcome):
    """Atomically, so concurrent runs (gates in other lanes, local runs) see a whole entry or none. Best effort: an
    unwritable cache only costs later runs time."""
    if outcome not in CACHEABLE:
        return
    folder = directory / digest_key[:2]
    try:
        folder.mkdir(parents=True, exist_ok=True)
        handle, temporary = tempfile.mkstemp(dir=folder, prefix='.entry-')
        try:
            with os.fdopen(handle, 'w') as entry:
                json.dump({'outcome': outcome}, entry)
            os.replace(temporary, folder / f'{digest_key}.json')
        finally:
            Path(temporary).unlink(missing_ok=True)
    except OSError:
        pass


def key(method, mutant):
    return f"{method['type']}::{method['signature']}@IL_{mutant['il_offset']:04x} {mutant['description']}"


def reasons(path):
    """Mutant key -> reason of a `<key> # reason` per line file; empty when it does not exist."""
    if not Path(path).is_file():
        return {}
    entries = (line.partition(' # ') for line in Path(path).read_text().splitlines()
               if line.strip() and not line.startswith('#'))
    return {key.strip(): reason.strip() for key, _, reason in entries}


def accepted(path=ACCEPTED):
    """Reviewed equivalent mutants: `<key> # reason` per line. Maintainer-owned (protected)."""
    return set(reasons(path))


def select(coverage, types):
    """Mutants of compared methods in the selected types, and of the compiler-generated methods (lambdas, local
    functions, iterator and async bodies) those methods own, run against the owner's tests."""
    owners = {}
    for m in coverage['methods']:
        if m['validation'] == 'compared':
            owners.setdefault((m['type'], re.split(r'[`(]', m['signature'])[0]), []).append(m)
    chosen = []
    for m in coverage['methods']:
        if m['validation'] == 'compared' and m['type'] in types:
            chosen.append(m)
        elif m['validation'] == 'excluded' and m.get('owner') and m['owner']['type'] in types:
            compared = owners.get((m['owner']['type'], m['owner']['name']), [])
            if compared:
                chosen.append(dict(m, cases=[c for o in compared for c in o['cases']]))
    return [(m, mutant) for m in chosen for mutant in m.get('mutants', [])]


def execute(command, env, timeout, cwd=None):
    """Exit code, or None on timeout after killing the whole process tree (dotnet test spawns testhost)."""
    process = subprocess.Popen(command, cwd=cwd, env=env, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL,
                               start_new_session=os.name != 'nt')
    try:
        return process.wait(timeout=timeout)
    except subprocess.TimeoutExpired:
        if os.name == 'nt':
            subprocess.run(['taskkill', '/T', '/F', '/PID', str(process.pid)], capture_output=True)
        else:
            os.killpg(process.pid, signal.SIGKILL)
        process.wait()
        return None


def test_filter(tests):
    return '|'.join(f'FullyQualifiedName={t}' for t in sorted(tests))


def failed_tests(trx):
    """`Namespace.Class.Method` of every failed test in a TRX file; none when the run left no report."""
    if not Path(trx).is_file():
        return set()
    root = ET.parse(trx).getroot()
    names = {}
    for test in root.findall('t:TestDefinitions/t:UnitTest', NS):
        method = test.find('t:TestMethod', NS)
        if method is not None:
            names[test.get('id')] = method.get('className', '').split(',')[0] + '.' + method.get('name', '')
    return {names[r.get('testId')] for r in root.findall('t:Results/t:UnitTestResult', NS)
            if r.get('outcome') == 'Failed' and r.get('testId') in names}


CRASHED = 'The active test run was aborted. Reason: Test host process crashed'


def crashed(trx):
    """Whether VSTest reported in a TRX file that the test host died (fatal CLR error such as a stack overflow,
    access violation or FailFast) mid-run. A run that left no report proves nothing: it may never have launched."""
    if not Path(trx).is_file():
        return False
    root = ET.parse(trx).getroot()
    summary = root.find('t:ResultSummary', NS)
    return summary is not None and summary.get('outcome') == 'Failed' and any(
        (text.text or '').startswith(CRASHED) for text in summary.findall('t:RunInfos/t:RunInfo/t:Text', NS))


def mismatches(scratch, mvid):
    """Recorded (test, mvid:token) mismatches on methods of the mutated assembly whose test also failed."""
    kills = scratch / 'kills.txt'
    recorded = [line.split('|', 1) for line in kills.read_text().splitlines() if '|' in line] if kills.is_file() else []
    failed = failed_tests(scratch / 'run.trx')
    return {identity for test, identity in recorded if test in failed and identity.startswith(f'{mvid}:')}


def run(coverage, types, test_dll, project, dotnet='dotnet', jobs=1, timeout=600, accepted_keys=None,
        global_json=b'{}\n', proposed=None):
    """Patch a private copy of the test output per worker and run only the tests that directly compare the method; a
    mutant is killed when one of their comparisons against Mathcad mismatches and fails its test, or when it crashes
    the test host that ran them cleanly in the control run. Other test failures, and mismatches a test swallows,
    cannot kill it. `global_json`: the SDK choice each run gets (see
    validate.sdk_directory). `proposed`: key -> reason of equivalent mutants proposed for review; a local run reports
    their survivors apart and does not fail on them (the merge gate never passes any)."""
    proposed = proposed or {}
    selected = select(coverage, types)
    accepted_keys = accepted() if accepted_keys is None else accepted_keys
    test_dll = Path(test_dll)
    cache = cache_directory()
    pristine = digest(files_under(test_dll.parent), test_dll.parent)
    # The cache matches a mutant only on byte-identical test output (code under test, tests, the pinned Mathcad
    # identity they check), SDK choice and installed runtimes, so a run over an unchanged build reuses outcomes.
    with sdk_directory(global_json) as cwd:
        toolchain = [subprocess.run([dotnet, flag], cwd=cwd, capture_output=True, text=True).stdout
                     for flag in ('--version', '--list-runtimes')]
    build = cache_key(sorted(pristine.items()), global_json.hex(), toolchain, project)
    copies = [Path(tempfile.mkdtemp(prefix='quire-mutant-')) for _ in range(max(1, jobs))]
    for copy in copies:
        shutil.copytree(test_dll.parent, copy, dirs_exist_ok=True)
    env = os.environ.copy()
    env['QUIRE_REQUIRE_MATHCAD'] = '1'
    env.pop('QUIRE_METHOD_CASES', None)
    env.pop('QUIRE_VALIDATION_RUN', None)
    env.pop('QUIRE_MUTATION_KILLS', None)
    env.pop('QUIRE_MUTANT_CACHE', None)

    def test_run(copy, tests, limit):
        """Exit code (None on timeout) and a fresh scratch directory, outside the copy the tests can see, holding
        the mismatch record and TRX report."""
        scratch = Path(tempfile.mkdtemp(prefix='quire-mutant-run-'))
        command = [dotnet, 'test', str(copy / test_dll.name), '--filter', test_filter(tests),
                   '--logger', 'trx;LogFileName=run.trx', '--results-directory', str(scratch)]
        kills = scratch / f'kills-{uuid.uuid4().hex}.txt'
        with sdk_directory(global_json) as cwd:
            code = execute(command, dict(env, QUIRE_MUTATION_KILLS=str(kills)), limit, cwd)
        if kills.is_file():
            kills.replace(scratch / 'kills.txt')
        return code, scratch

    def unchanged(copy):
        """Code under test runs from the copy; it must not have rewritten it for later runs."""
        if digest(files_under(copy), copy) != pristine:
            raise ValueError(f'Tests changed their own build output in {copy}; mutation results would be meaningless')

    def control(copy, tests, limit):
        """The unmutated copy must pass these tests without recording any mismatch, or no failure under a mutant
        would mean anything; a recorded mismatch in a passing run means a test swallowed it. Returns its duration."""
        started = time.monotonic()
        code, scratch = test_run(copy, tests, limit)
        elapsed = time.monotonic() - started
        swallowed = (scratch / 'kills.txt').is_file()
        shutil.rmtree(scratch, ignore_errors=True)
        unchanged(copy)
        if code != 0 or swallowed:
            reason = f'failed (exit {code})' if code != 0 else 'recorded a mismatch that a test swallowed'
            raise ValueError(f'Unmutated control run of {sorted(tests)} {reason}; mutation results would be meaningless')
        return elapsed

    if selected:
        try:
            control(copies[0], {case['test'] for method, _ in selected for case in method['cases']}, timeout)
        except ValueError:
            for copy in copies:
                shutil.rmtree(copy, ignore_errors=True)
            raise
    # Each mutant reruns only its method's tests in a fresh process, so that subset also needs its own control:
    # a test that passes after its siblings but mismatches alone would otherwise kill every mutant. A mutant that
    # loops forever is cut off relative to that control, which ran in a worker under the same parallel load; a
    # load-induced timeout would count as a kill, so the margin stays wide.
    controlled, limits, control_lock, failures = {}, {}, threading.Lock(), []

    def attempt(copy, method, mutant):
        tests = frozenset(case['test'] for case in method['cases'])
        with control_lock:
            first = tests not in controlled
            done_control = controlled.setdefault(tests, threading.Event())
        if first:
            try:
                limits[tests] = min(timeout, max(MUTANT_FLOOR, MUTANT_MARGIN * control(copy, tests, timeout)))
            except BaseException as error:
                failures.append(error)
                raise
            finally:
                done_control.set()
        else:
            done_control.wait()
        if failures:
            raise failures[0]
        # The timeout, which varies with load, is not in the key. Only outcomes reached within a timeout are cached, so
        # a fresh run could differ only by timing out, a kill; the cached outcome is then a kill too or a survivor,
        # which fails the run anyway. Neither direction lets a weak test pass.
        mutant_key = cache_key(build, key(method, mutant), mutant['file_offset'], mutant['original'],
                               mutant['replacement'], sorted(tests))
        outcome = cached_outcome(cache, mutant_key)
        if outcome is not None:
            return result(method, mutant, outcome, tests, cached=True)
        target = copy / f'{project}.dll'
        image = bytearray(target.read_bytes())
        original, replacement = bytes.fromhex(mutant['original']), bytes.fromhex(mutant['replacement'])
        offset = mutant['file_offset']
        if image[offset:offset + len(original)] != original:
            raise ValueError(f"{key(method, mutant)}: bytes at offset {offset} differ from the inventory; rebuild and re-audit")
        patched = bytearray(image)
        patched[offset:offset + len(original)] = replacement
        target.write_bytes(patched)
        try:
            code, scratch = test_run(copy, tests, limits[tests])
        finally:
            target.write_bytes(image)
        unchanged(copy)
        # Killed only when a comparison against Mathcad mismatched in a test that failed. The first mismatch ends its
        # test, so it may be recorded for a caller compared earlier; failures with no such mismatch prove nothing.
        # A crash of the test host (stack overflow, access violation) under tests whose control passed is a kill too.
        found = mismatches(scratch, method['quire_mvid']) if code else set()
        crash = bool(code) and crashed(scratch / 'run.trx')
        shutil.rmtree(scratch, ignore_errors=True)
        if code is None:
            outcome = 'killed (timeout)'
        elif f"{method['quire_mvid']}:{method['token']}" in found:
            outcome = 'killed'
        elif found:
            outcome = 'killed (via another method compared first)'
        elif crash:
            outcome = 'killed (crash)'
        elif code:
            outcome = 'survived (tests failed without a mismatch)'
        else:
            outcome = 'survived'
        store_outcome(cache, mutant_key, outcome)  # not timeouts, which depend on the load
        return result(method, mutant, outcome, tests, cached=False)

    def result(method, mutant, outcome, tests, cached):
        return {'key': key(method, mutant), 'outcome': outcome, 'accepted': key(method, mutant) in accepted_keys,
                'proposed': key(method, mutant) in proposed, 'timeout_seconds': round(limits[tests]), 'cached': cached}

    # Each worker owns one copy and takes the next mutant from a shared queue, so a slow mutant idles nobody else.
    pending = queue.Queue()
    for index, item in enumerate(selected):
        pending.put((index, item))
    results = [None] * len(selected)
    done, lock = [0], threading.Lock()

    def worker(copy):
        while not failures:
            try:
                index, (method, mutant) = pending.get_nowait()
            except queue.Empty:
                return
            results[index] = attempt(copy, method, mutant)
            with lock:
                done[0] += 1
                if done[0] % 10 == 0 or done[0] == len(selected):
                    print(f'  mutants: {done[0]}/{len(selected)} run', flush=True)

    try:
        with ThreadPoolExecutor(len(copies)) as pool:
            for future in [pool.submit(worker, copy) for copy in copies]:
                future.result()
    finally:
        for copy in copies:
            shutil.rmtree(copy, ignore_errors=True)
    surviving = [r for r in results if r['outcome'].startswith('survived') and not r['accepted']]
    survivors = [r['key'] for r in surviving if not r['proposed']]
    return {'types': sorted(types), 'total': len(results), 'max_mutant_timeout_seconds': round(max(limits.values(), default=timeout)),
            'timeouts': sum(r['outcome'] == 'killed (timeout)' for r in results),
            'crashes': sum(r['outcome'] == 'killed (crash)' for r in results),
            'cached': sum(r['cached'] for r in results),
            'killed': sum(r['outcome'].startswith('killed') for r in results),
            'accepted_survivors': sum(r['outcome'].startswith('survived') and r['accepted'] for r in results),
            'proposed_survivors': [{'key': r['key'], 'reason': proposed[r['key']]} for r in surviving if r['proposed']],
            'stale_proposed': sorted(set(proposed) - {r['key'] for r in surviving}),  # killed, accepted or not run
            'survivors': survivors, 'complete': not survivors, 'mutants': results}
