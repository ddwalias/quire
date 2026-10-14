"""Byte-level IL mutation of compared methods: every mutant must fail one of that method's own direct comparisons."""
from concurrent.futures import ThreadPoolExecutor
import os
from pathlib import Path
import shutil
import subprocess
import tempfile
from validate import ROOT

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
    """Mutants of compared methods in the selected types."""
    return [(m, mutant) for m in coverage['methods'] if m['type'] in types and m['validation'] == 'compared'
            for mutant in m.get('mutants', [])]


def run(coverage, types, test_dll, project, dotnet='dotnet', jobs=1, timeout=600, accepted_keys=None):
    """Patch a private copy of the test output per worker; a mutant is killed when one of the tests that directly
    compared its method fails. Unrelated tests cannot kill it."""
    selected = select(coverage, types)
    accepted_keys = accepted() if accepted_keys is None else accepted_keys
    test_dll = Path(test_dll)
    copies = [Path(tempfile.mkdtemp(prefix='quire-mutant-')) for _ in range(max(1, jobs))]
    for copy in copies:
        shutil.copytree(test_dll.parent, copy, dirs_exist_ok=True)
    env = os.environ.copy()
    env['QUIRE_REQUIRE_MATHCAD'] = '1'
    env.pop('QUIRE_METHOD_CASES', None)
    env.pop('QUIRE_VALIDATION_RUN', None)

    def attempt(index_item):
        index, (method, mutant) = index_item
        copy = copies[index % len(copies)]
        target = copy / f'{project}.dll'
        image = bytearray(target.read_bytes())
        original, replacement = bytes.fromhex(mutant['original']), bytes.fromhex(mutant['replacement'])
        offset = mutant['file_offset']
        if image[offset:offset + len(original)] != original:
            raise ValueError(f"{key(method, mutant)}: bytes at offset {offset} differ from the inventory; rebuild and re-audit")
        patched = bytearray(image)
        patched[offset:offset + len(original)] = replacement
        target.write_bytes(patched)
        tests = sorted({case['test'] for case in method['cases']})
        command = [dotnet, 'test', str(copy / test_dll.name), '--filter', '|'.join(f'FullyQualifiedName={t}' for t in tests)]
        try:
            result = subprocess.run(command, env=env, capture_output=True, text=True, timeout=timeout)
            outcome = 'survived' if result.returncode == 0 else 'killed'
        except subprocess.TimeoutExpired:
            outcome = 'killed (timeout)'
        finally:
            target.write_bytes(image)
        return {'key': key(method, mutant), 'outcome': outcome, 'accepted': key(method, mutant) in accepted_keys}

    # Workers own one copy each: item i always runs in copy i % jobs, and items for one copy run in order.
    buckets = [[(i, item) for i, item in enumerate(selected) if i % len(copies) == c] for c in range(len(copies))]
    try:
        with ThreadPoolExecutor(len(copies)) as pool:
            results = [r for bucket in pool.map(lambda b: [attempt(x) for x in b], buckets) for r in bucket]
    finally:
        for copy in copies:
            shutil.rmtree(copy, ignore_errors=True)
    survivors = [r['key'] for r in results if r['outcome'] == 'survived' and not r['accepted']]
    return {'types': sorted(types), 'total': len(results),
            'killed': sum(r['outcome'].startswith('killed') for r in results),
            'accepted_survivors': sum(r['outcome'] == 'survived' and r['accepted'] for r in results),
            'survivors': survivors, 'complete': not survivors, 'mutants': results}
