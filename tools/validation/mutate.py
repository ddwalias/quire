"""Byte-level IL mutation of compared methods: every mutant must make a comparison against Mathcad fail its test."""
from concurrent.futures import ThreadPoolExecutor
import hashlib
import json
import os
import queue
import re
import signal
import stat
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
# could hide a weak test, so the floor stays generous; only mutants that loop pay it. Every timeout of the parallel pass
# is rerun alone, against a fresh control, before it counts.
MUTANT_MARGIN, MUTANT_FLOOR = 4, 30
# Mutant copies and run directories older than this, left in the system temporary directory by killed runs, are removed.
STALE_SECONDS = 24 * 3600
# Local runs only: outcomes of earlier runs of identical mutants (see cached_outcome). Bump the version when what an
# outcome means changes. Timeouts and crashes are not cached: load, memory pressure or a flaky crash would make them
# permanent.
CACHE_VERSION = 1
CACHEABLE = {'killed', 'killed (via another method compared first)', 'survived',
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
    return outcome if isinstance(outcome, str) and outcome in CACHEABLE else None


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


BODY = re.compile(r'(.*\S) body=([0-9a-f]{64})')
# Any `body=` field, well-formed or not, so a malformed one cannot pass as part of the key.
ANY_BODY = re.compile(r'(.*\S) body=\S*')
# What `reasons` records instead of a hash for an entry that may excuse nothing (see stale_entries).
MALFORMED, DUPLICATE = 'malformed body hash', 'duplicate entry'


def split_entry(head):
    """(mutant key, body hash, or MALFORMED when a `body=` field is not 64 lowercase hex digits, or None)."""
    match = BODY.fullmatch(head.strip())
    if match:
        return match[1], match[2]
    match = ANY_BODY.fullmatch(head.strip())
    return (match[1], MALFORMED) if match else (head.strip(), None)


def reasons(path):
    """Mutant key -> (body hash or None, reason) of a `<key> [body=<body_sha256>] # reason` per line file; empty when
    it does not exist. The hash is MethodAudit's `body_sha256` of the mutant's method when the entry was reviewed. A
    malformed hash, or a key listed twice, is recorded as MALFORMED or DUPLICATE: such an entry excuses nothing."""
    if not Path(path).is_file():
        return {}
    entries = {}
    for line in Path(path).read_text().splitlines():
        if not line.strip() or line.startswith('#'):
            continue
        head, _, reason = line.partition(' # ')
        mutant_key, body = split_entry(head)
        entries[mutant_key] = (DUPLICATE if mutant_key in entries else body, reason.strip())
    return entries


def accepted(path=ACCEPTED):
    """Reviewed equivalent mutants: `<key> body=<body_sha256> # reason` per line. Maintainer-owned (protected). An
    entry excuses a survivor only while its method's body hash is the one reviewed (see stale_entries)."""
    return reasons(path)


def method_key(mutant_key):
    """`Type::signature` of a mutant key `Type::signature@IL_<offset> <description>`."""
    return mutant_key.rpartition('@IL_')[0]


def stale_entries(methods, entries, origin='accepted'):
    """Entries (`reasons`) that excuse nothing: without a body hash (accepted entries only; a proposal may omit it),
    naming no method or several (one per assembly), whose method's body hash differs from the one reviewed (its IL
    changed, so the proof may no longer hold), or naming no mutant of that method."""
    by_key = {}
    for method in methods:
        by_key.setdefault(f"{method['type']}::{method['signature']}", []).append(method)
    stale = []
    for mutant_key, (body, reason) in sorted(entries.items()):
        candidates = by_key.get(method_key(mutant_key), [])
        if body in (MALFORMED, DUPLICATE):
            problem = body
        elif body is None and origin == 'accepted':
            problem = 'no body hash'
        elif not candidates:
            problem = 'no such method'
        elif len(candidates) > 1:
            problem = 'ambiguous method key'
        elif body is not None and body != candidates[0].get('body_sha256'):
            problem = 'method body changed'
        elif mutant_key not in {key(candidates[0], m) for m in candidates[0].get('mutants') or []}:
            problem = 'no such mutant'
        else:
            continue
        stale.append(dict(key=mutant_key, reason=reason, source=origin, problem=problem))
    return stale


def listed(entries, method, mutant, origin):
    """Whether an entry covers this mutant of this method body: an accepted entry needs the method's current body
    hash; a proposal without one matches by key."""
    entry = entries.get(key(method, mutant))
    if entry is None or entry[0] in (MALFORMED, DUPLICATE) or entry[0] is None and origin == 'accepted':
        return False
    return entry[0] is None or entry[0] == method.get('body_sha256')


def select(coverage, types, methods=()):
    """Mutants of compared methods in the selected types or among the selected `Type::signature` methods, and of the
    compiler-generated methods (lambdas, local functions, iterator and async bodies) that are selected themselves or
    owned by a selected method (they run against the owner's tests, so a change to those or to the owner reaches
    them), run against the owner's tests."""
    owners = {}
    for m in coverage['methods']:
        if m['validation'] == 'compared':
            owners.setdefault((m['type'], re.split(r'[`(]', m['signature'])[0]), []).append(m)
    chosen = []
    name = lambda m: f"{m['type']}::{m['signature']}"
    for m in coverage['methods']:
        if m['validation'] == 'compared' and (m['type'] in types or name(m) in methods):
            chosen.append(m)
        elif m['validation'] == 'excluded' and m.get('owner'):
            compared = owners.get((m['owner']['type'], m['owner']['name']), [])
            if compared and (m['owner']['type'] in types or name(m) in methods
                             or any(name(o) in methods for o in compared)):
                chosen.append(dict(m, cases=[c for o in compared for c in o['cases']]))
    return [(m, mutant) for m in chosen for mutant in m.get('mutants', [])]


# Where run() sweeps (None: the system temporary directory); tests point it elsewhere.
SWEEP_DIRECTORY = None
# Entries examined per directory before it counts as in use: a tree this large is left alone.
SWEEP_ENTRIES = 100_000


def newest_change(path, limit=None):
    """The latest modification time of `path` or anything beneath it (links not followed); None past `limit`
    entries. A directory's own time changes only when entries are added or removed, not when a file is rewritten."""
    newest, seen, limit = path.lstat().st_mtime, 0, SWEEP_ENTRIES if limit is None else limit
    for directory, names, files in os.walk(path, followlinks=False):
        for name in names + files:
            seen += 1
            if seen > limit:
                return None
            try:
                newest = max(newest, os.lstat(os.path.join(directory, name)).st_mtime)
            except OSError:
                pass
    return newest


def sweep_stale(directory=None, age=STALE_SECONDS):
    """Remove `quire-mutant-*` directories (not files or links) that this user owns in `directory` (the system
    temporary directory) and in which nothing changed for `age` seconds: copies of test output and scratch
    directories left by runs that were killed before cleaning up. Returns the removed paths."""
    if not hasattr(os, 'getuid'):
        return []
    cutoff, removed = time.time() - age, []
    for path in Path(directory or tempfile.gettempdir()).glob('quire-mutant-*'):
        try:
            info = path.lstat()
            if not stat.S_ISDIR(info.st_mode) or info.st_uid != os.getuid():
                continue
            newest = newest_change(path)
        except OSError:
            continue
        if newest is not None and newest < cutoff:
            shutil.rmtree(path, ignore_errors=True)
            removed.append(path)
    return removed


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
        global_json=b'{}\n', proposed=None, use_cache=False, mathcad=None, methods=None, scratch=None):
    """Patch a private copy of the test output per worker and run only the tests that directly compare the method; a
    mutant is killed when one of their comparisons against Mathcad mismatches and fails its test, or when it crashes
    the test host that ran them cleanly in the control run. Other test failures, and mismatches a test swallows,
    cannot kill it. `accepted_keys`: `reasons` of the accepted file (default: the repository's); an entry excuses a
    survivor only with its method's current body hash. `global_json`: the SDK choice each run gets (see
    validate.sdk_directory). `proposed`: `reasons` of equivalent mutants proposed for review; a local run reports
    their survivors apart and does not fail on them (the merge gate never passes any). `use_cache`: reuse and record
    outcomes in the local cache; never in the gate. `mathcad`: the Mathcad install the tests compare against.
    `methods`: `Type::signature` -> reasons, methods selected individually (validate.changed_methods). `scratch`: the
    directory that holds the copies and per-run directories (validate gives each run its own); default: the system's
    temporary one.
    A mutant that times out in the parallel pass is rerun once all workers have finished, alone, with a limit from a
    fresh control run of its tests; it counts as killed only if it times out again."""
    sweep_stale(SWEEP_DIRECTORY)
    if scratch is not None:
        Path(scratch).mkdir(parents=True, exist_ok=True)
    proposed = proposed or {}
    methods = methods or {}
    selected = select(coverage, types, methods)
    accepted_keys = accepted() if accepted_keys is None else accepted_keys
    test_dll = Path(test_dll)
    pristine = digest(files_under(test_dll.parent), test_dll.parent)
    cache = build = None
    if use_cache:
        cache = cache_directory()
        # A mutant matches only on byte-identical test output (code under test and tests), the same harness, SDK
        # choice, installed runtimes and Mathcad install (its DLLs by name, size and modification time).
        with sdk_directory(global_json) as cwd:
            toolchain = [subprocess.run([dotnet, flag], cwd=cwd, capture_output=True, text=True).stdout
                         for flag in ('--version', '--list-runtimes')]
        harness = [hashlib.sha256(Path(__file__).with_name(f).read_bytes()).hexdigest() for f in ('mutate.py', 'validate.py')]
        install = Path(mathcad) if mathcad else None
        dlls = sorted((p.relative_to(install).as_posix(), p.stat().st_size, p.stat().st_mtime_ns)
                      for p in install.rglob('*.dll') if p.is_file()) if install and install.is_dir() else []
        build = cache_key(sorted(pristine.items()), global_json.hex(), toolchain, project, harness,
                          str(install), dlls)
    copies = [Path(tempfile.mkdtemp(prefix='quire-mutant-', dir=scratch)) for _ in range(max(1, jobs))]
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
        directory = Path(tempfile.mkdtemp(prefix='quire-mutant-run-', dir=scratch))
        command = [dotnet, 'test', str(copy / test_dll.name), '--filter', test_filter(tests),
                   '--logger', 'trx;LogFileName=run.trx', '--results-directory', str(directory)]
        kills = directory / f'kills-{uuid.uuid4().hex}.txt'
        with sdk_directory(global_json) as cwd:
            code = execute(command, dict(env, QUIRE_MUTATION_KILLS=str(kills)), limit, cwd)
        if kills.is_file():
            kills.replace(directory / 'kills.txt')
        return code, directory

    def unchanged(copy):
        """Code under test runs from the copy; it must not have rewritten it for later runs."""
        if digest(files_under(copy), copy) != pristine:
            raise ValueError(f'Tests changed their own build output in {copy}; mutation results would be meaningless')

    def control(copy, tests, limit):
        """The unmutated copy must pass these tests without recording any mismatch, or no failure under a mutant
        would mean anything; a recorded mismatch in a passing run means a test swallowed it. Returns its duration."""
        started = time.monotonic()
        code, directory = test_run(copy, tests, limit)
        elapsed = time.monotonic() - started
        swallowed = (directory / 'kills.txt').is_file()
        shutil.rmtree(directory, ignore_errors=True)
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

    def limit_for(copy, tests):
        return min(timeout, max(MUTANT_FLOOR, MUTANT_MARGIN * control(copy, tests, timeout)))

    def tests_of(method):
        return frozenset(case['test'] for case in method['cases'])

    def mutant_cache_key(method, mutant, tests):
        # The timeout, which varies with load, is not in the key. Only outcomes reached within a timeout are cached, so
        # a fresh run could differ only by timing out, a kill; the cached outcome is then a kill too or a survivor,
        # which fails the run anyway. Neither direction lets a weak test pass.
        return cache_key(build, key(method, mutant), mutant['file_offset'], mutant['original'], mutant['replacement'],
                         sorted(tests))

    def attempt(copy, method, mutant):
        tests = tests_of(method)
        with control_lock:
            first = tests not in controlled
            done_control = controlled.setdefault(tests, threading.Event())
        if first:
            try:
                limits[tests] = limit_for(copy, tests)
            except BaseException as error:
                failures.append(error)
                raise
            finally:
                done_control.set()
        else:
            done_control.wait()
        if failures:
            raise failures[0]
        outcome = cached_outcome(cache, mutant_cache_key(method, mutant, tests)) if cache else None
        if outcome is not None:
            return result(method, mutant, outcome, limits[tests], cached=True)
        outcome = outcome_of(copy, method, mutant, tests, limits[tests])
        if cache:
            store_outcome(cache, mutant_cache_key(method, mutant, tests), outcome)
        return result(method, mutant, outcome, limits[tests], cached=False)

    def outcome_of(copy, method, mutant, tests, seconds):
        """Patch the copy, run the method's tests within `seconds`, restore it and classify what happened."""
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
            code, directory = test_run(copy, tests, seconds)
        finally:
            target.write_bytes(image)
        unchanged(copy)
        # Killed only when a comparison against Mathcad mismatched in a test that failed. The first mismatch ends its
        # test, so it may be recorded for a caller compared earlier; failures with no such mismatch prove nothing.
        # A crash of the test host (stack overflow, access violation) under tests whose control passed is a kill too.
        found = mismatches(directory, method['quire_mvid']) if code else set()
        crash = bool(code) and crashed(directory / 'run.trx')
        shutil.rmtree(directory, ignore_errors=True)
        if code is None:
            return 'killed (timeout)'
        if f"{method['quire_mvid']}:{method['token']}" in found:
            return 'killed'
        if found:
            return 'killed (via another method compared first)'
        if crash:
            return 'killed (crash)'
        return 'survived (tests failed without a mismatch)' if code else 'survived'

    def result(method, mutant, outcome, seconds, cached):
        return {'key': key(method, mutant), 'outcome': outcome,
                'accepted': listed(accepted_keys, method, mutant, 'accepted'),
                'proposed': listed(proposed, method, mutant, 'proposed'), 'timeout_seconds': round(seconds),
                'cached': cached}

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

    retried, retry_limits = [], []
    try:
        with ThreadPoolExecutor(len(copies)) as pool:
            for future in [pool.submit(worker, copy) for copy in copies]:
                future.result()
        # Load (the other workers, other gates) can stretch a run past a limit set under different load, and a timeout
        # counts as a kill. So each one reruns now, alone in this process, against a fresh control of its tests: it
        # stays a kill only if it times out again; otherwise its real outcome stands.
        retried = [i for i, r in enumerate(results) if r['outcome'] == 'killed (timeout)']
        if retried:
            print(f'  rerunning {len(retried)} timed-out mutants one at a time', flush=True)
        # One fresh control per test set, now that nothing else runs.
        fresh = {}
        for index in retried:
            tests = tests_of(selected[index][0])
            if tests not in fresh:
                fresh[tests] = limit_for(copies[0], tests)
        for index in retried:
            method, mutant = selected[index]
            tests = tests_of(method)
            seconds = fresh[tests]
            outcome = outcome_of(copies[0], method, mutant, tests, seconds)
            if cache:
                store_outcome(cache, mutant_cache_key(method, mutant, tests), outcome)
            results[index] = dict(result(method, mutant, outcome, seconds, cached=False),
                                  parallel_outcome='killed (timeout)', parallel_timeout_seconds=results[index]['timeout_seconds'])
            retry_limits.append(seconds)
    finally:
        for copy in copies:
            shutil.rmtree(copy, ignore_errors=True)
    surviving = [r for r in results if r['outcome'].startswith('survived') and not r['accepted']]
    survivors = [r['key'] for r in surviving if not r['proposed']]
    mutated = sorted({f"{m['type']}::{m['signature']}" for m, _ in selected})
    return {'types': sorted(types), 'methods': dict(sorted(methods.items())), 'mutated_methods': mutated,
            'total': len(results), 'max_mutant_timeout_seconds': round(max([*limits.values(), *retry_limits], default=timeout)),
            'timeouts': sum(r['outcome'] == 'killed (timeout)' for r in results),
            'timed_out': [r['key'] for r in results if r['outcome'] == 'killed (timeout)'],
            'timeout_retries': len(retried),
            'crashes': sum(r['outcome'] == 'killed (crash)' for r in results),
            'cached': sum(r['cached'] for r in results),
            'killed': sum(r['outcome'].startswith('killed') for r in results),
            'accepted_survivors': sum(r['outcome'].startswith('survived') and r['accepted'] for r in results),
            'proposed_survivors': [{'key': r['key'], 'reason': proposed[r['key']][1]} for r in surviving if r['proposed']],
            # killed, accepted, not run, or for another body
            'stale_proposed': sorted(set(proposed) - {r['key'] for r in surviving if r['proposed']}),
            'survivors': survivors, 'complete': not survivors and not any(r['proposed'] for r in surviving),
            'mutants': results}


def annotate(path, methods):
    """Rewrite an accepted-mutants file with each entry's current method body hash (`<key> body=<sha> # reason`),
    from a run's method inventory or coverage report. Entries whose method is missing or ambiguous, or that name no
    mutant of it, are left as they are and returned with the problem; comments and blank lines are kept. Returns
    (problems, keys whose existing hash differed from the current one: reviewed for another body, or malformed)."""
    by_key = {}
    for method in methods:
        by_key.setdefault(f"{method['type']}::{method['signature']}", []).append(method)
    lines, problems, rehashed = [], [], []
    for line in Path(path).read_text().splitlines():
        head, separator, reason = line.partition(' # ')
        if not line.strip() or line.startswith('#'):
            lines.append(line)
            continue
        mutant_key, body = split_entry(head)
        candidates = by_key.get(method_key(mutant_key), [])
        if len(candidates) != 1 or not candidates[0].get('body_sha256'):
            problems.append((mutant_key, 'ambiguous method key' if candidates else 'no such method'))
            lines.append(line)
            continue
        if mutant_key not in {key(candidates[0], m) for m in candidates[0].get('mutants') or []}:
            problems.append((mutant_key, 'no such mutant'))
        if body is not None and body != candidates[0]['body_sha256']:
            rehashed.append(mutant_key)  # its proof was for another body (or the hash was malformed): re-review it
        lines.append(f"{mutant_key} body={candidates[0]['body_sha256']}{separator}{reason}")
    Path(path).write_text('\n'.join(lines) + '\n')
    return problems, rehashed


def main(argv):
    import argparse
    parser = argparse.ArgumentParser(prog='mutate.py annotate-accepted',
                                     description='Add each entry\'s current method body hash to an accepted-mutants file.')
    parser.add_argument('command', choices=['annotate-accepted'])
    parser.add_argument('report', type=Path, help='method-inventory.json or method-coverage.json of a run')
    parser.add_argument('--file', type=Path, default=ACCEPTED, help='accepted-mutants file to rewrite (default: the '
                        'repository\'s docs/mirror/accepted-mutants.txt)')
    args = parser.parse_args(argv)
    problems, rehashed = annotate(args.file, json.loads(args.report.read_text())['methods'])
    for mutant_key in rehashed:
        print(f'rehashed (body changed): {mutant_key}')
    for mutant_key, problem in problems:
        print(f'{problem}: {mutant_key}')
    return 1 if problems else 0


if __name__ == '__main__':
    import sys
    sys.exit(main(sys.argv[1:]))
