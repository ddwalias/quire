"""Byte-level IL mutation of compared methods: every mutant must make a comparison against Mathcad fail its test."""
from concurrent.futures import ThreadPoolExecutor
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
from validate import NS, ROOT, digest, files_under

ACCEPTED = ROOT / 'docs/mirror/accepted-mutants.txt'


def key(method, mutant):
    return f"{method['type']}::{method['signature']}@IL_{mutant['il_offset']:04x} {mutant['description']}"


def accepted(path=ACCEPTED):
    """Reviewed equivalent mutants: `<key> # reason` per line. Maintainer-owned (protected)."""
    if not Path(path).is_file():
        return set()
    return {line.split(' # ')[0].strip() for line in Path(path).read_text().splitlines()
            if line.strip() and not line.startswith('#')}


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


def execute(command, env, timeout):
    """Exit code, or None on timeout after killing the whole process tree (dotnet test spawns testhost)."""
    process = subprocess.Popen(command, env=env, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL,
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


def mismatches(scratch, mvid):
    """Recorded (test, mvid:token) mismatches on methods of the mutated assembly whose test also failed."""
    kills = scratch / 'kills.txt'
    recorded = [line.split('|', 1) for line in kills.read_text().splitlines() if '|' in line] if kills.is_file() else []
    failed = failed_tests(scratch / 'run.trx')
    return {identity for test, identity in recorded if test in failed and identity.startswith(f'{mvid}:')}


def run(coverage, types, test_dll, project, dotnet='dotnet', jobs=1, timeout=600, accepted_keys=None):
    """Patch a private copy of the test output per worker and run only the tests that directly compare the method; a
    mutant is killed when one of their comparisons against Mathcad mismatches and fails its test. Other test
    failures, and mismatches a test swallows, cannot kill it."""
    selected = select(coverage, types)
    accepted_keys = accepted() if accepted_keys is None else accepted_keys
    test_dll = Path(test_dll)
    copies = [Path(tempfile.mkdtemp(prefix='quire-mutant-')) for _ in range(max(1, jobs))]
    for copy in copies:
        shutil.copytree(test_dll.parent, copy, dirs_exist_ok=True)
    pristine = digest(files_under(test_dll.parent), test_dll.parent)
    env = os.environ.copy()
    env['QUIRE_REQUIRE_MATHCAD'] = '1'
    env.pop('QUIRE_METHOD_CASES', None)
    env.pop('QUIRE_VALIDATION_RUN', None)
    env.pop('QUIRE_MUTATION_KILLS', None)

    def test_run(copy, tests, limit):
        """Exit code (None on timeout) and a fresh scratch directory, outside the copy the tests can see, holding
        the mismatch record and TRX report."""
        scratch = Path(tempfile.mkdtemp(prefix='quire-mutant-run-'))
        command = [dotnet, 'test', str(copy / test_dll.name), '--filter', test_filter(tests),
                   '--logger', 'trx;LogFileName=run.trx', '--results-directory', str(scratch)]
        kills = scratch / f'kills-{uuid.uuid4().hex}.txt'
        code = execute(command, dict(env, QUIRE_MUTATION_KILLS=str(kills)), limit)
        if kills.is_file():
            kills.replace(scratch / 'kills.txt')
        return code, scratch

    def unchanged(copy):
        """Code under test runs from the copy; it must not have rewritten it for later runs."""
        if digest(files_under(copy), copy) != pristine:
            raise ValueError(f'Tests changed their own build output in {copy}; mutation results would be meaningless')

    def control(copy, tests, limit):
        """The unmutated copy must pass these tests without recording any mismatch, or no failure under a mutant
        would mean anything; a recorded mismatch in a passing run means a test swallowed it."""
        code, scratch = test_run(copy, tests, limit)
        swallowed = (scratch / 'kills.txt').is_file()
        shutil.rmtree(scratch, ignore_errors=True)
        unchanged(copy)
        if code != 0 or swallowed:
            reason = f'failed (exit {code})' if code != 0 else 'recorded a mismatch that a test swallowed'
            raise ValueError(f'Unmutated control run of {sorted(tests)} {reason}; mutation results would be meaningless')

    mutant_timeout = timeout
    if selected:
        started = time.monotonic()
        try:
            control(copies[0], {case['test'] for method, _ in selected for case in method['cases']}, timeout)
        except ValueError:
            for copy in copies:
                shutil.rmtree(copy, ignore_errors=True)
            raise
        # A mutant that loops forever is cut off relative to how long all its tests take unmutated.
        mutant_timeout = min(timeout, max(30, 5 * (time.monotonic() - started)))
    # Each mutant reruns only its method's tests in a fresh process, so that subset also needs its own control:
    # a test that passes after its siblings but mismatches alone would otherwise kill every mutant.
    controlled, control_lock, failures = {}, threading.Lock(), []

    def attempt(copy, method, mutant):
        tests = frozenset(case['test'] for case in method['cases'])
        with control_lock:
            first = tests not in controlled
            done_control = controlled.setdefault(tests, threading.Event())
        if first:
            try:
                control(copy, tests, timeout)
            except BaseException as error:
                failures.append(error)
                raise
            finally:
                done_control.set()
        else:
            done_control.wait()
        if failures:
            raise failures[0]
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
            code, scratch = test_run(copy, tests, mutant_timeout)
        finally:
            target.write_bytes(image)
        unchanged(copy)
        # Killed only when a comparison against Mathcad mismatched in a test that failed. The first mismatch ends its
        # test, so it may be recorded for a caller compared earlier; failures with no such mismatch prove nothing.
        found = mismatches(scratch, method['quire_mvid']) if code else set()
        shutil.rmtree(scratch, ignore_errors=True)
        if code is None:
            outcome = 'killed (timeout)'
        elif f"{method['quire_mvid']}:{method['token']}" in found:
            outcome = 'killed'
        elif found:
            outcome = 'killed (via another method compared first)'
        elif code:
            outcome = 'survived (tests failed without a mismatch)'
        else:
            outcome = 'survived'
        return {'key': key(method, mutant), 'outcome': outcome, 'accepted': key(method, mutant) in accepted_keys}

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
    survivors = [r['key'] for r in results if r['outcome'].startswith('survived') and not r['accepted']]
    return {'types': sorted(types), 'total': len(results), 'mutant_timeout_seconds': round(mutant_timeout),
            'killed': sum(r['outcome'].startswith('killed') for r in results),
            'accepted_survivors': sum(r['outcome'].startswith('survived') and r['accepted'] for r in results),
            'survivors': survivors, 'complete': not survivors, 'mutants': results}
