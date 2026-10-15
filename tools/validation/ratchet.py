"""Validation may only improve: compare a run's per-method states, per-type totals and integrity findings with the
committed baseline."""
from collections import Counter
import json
from pathlib import Path
from validate import ROOT, test_class

BASELINE = ROOT / 'docs/mirror/validation-baseline.json'
SCHEMA = 3
# Baseline schemas compare() reads and the merge gate accepts from a validator. Schema 4 adds only `bodies` and
# `writes` (which narrow mutation selection, not the ratchet); accepting it first lets the branch that starts writing
# it pass the gate run by this code.
READABLE = (3, 4)
GOOD = ('compared', 'total')  # total (hand-written): deleting methods, even stubs, must not shrink 'missing'
# Compiler-generated methods (excluded) may come and go with idiomatic code: mutation covers them through their owner.
# A method's stage (method_coverage.audit) says how far it has got; fixing one gap exposes the next, so a method may
# move forward between unfinished stages but never back. Tests that compare without checking instance state rank
# above having none, so deleting them is never progress. Per type, the unfinished methods may not grow in number, and
# a new method may not arrive with a gap that existing methods' progress would otherwise pay for.
STAGES = ('no_counterpart', 'missing', 'structure_mismatch', 'untested', 'unchecked_state', 'uncovered', 'compared')
UNFINISHED = ('missing', 'structure_mismatch', 'uncovered')
NEW_GAPS = ('structure_mismatch', 'unchecked_state', 'uncovered')


def snapshot(coverage, integrity, profile):
    """Identities, not just counts, so fixing one finding cannot pay for a new one."""
    return {'schema': SCHEMA, 'profile': profile, 'types': coverage['by_type'],
            'methods': {f"{m['type']}::{m['signature']}": stage(m) for m in coverage['methods']},
            'tests': {f"{m['type']}::{m['signature']}": sorted({test_class(c['test']) for c in m['cases']})
                      for m in coverage['methods'] if m.get('cases')},
            'lint': sorted(f"{f['file']} | {f['rule']} | {f['text']}" for f in integrity['lint'])}


def stage(method):
    if not method.get('original') and method['validation'] != 'excluded':
        return 'no_counterpart'
    return method.get('stage', method['validation'])


def forward(old, new):
    return old in STAGES and new in STAGES and STAGES.index(new) > STAGES.index(old)


def compare(baseline, current, scoped=False):
    """Regressions of `current` against `baseline`. A scoped run only answers for its own types."""
    if baseline.get('schema') not in READABLE:
        return [f"baseline schema {baseline.get('schema')!r} is not one of {READABLE}; a maintainer must regenerate it"]
    if baseline.get('profile') != current['profile']:
        return [f"baseline is for profile {baseline.get('profile')!r}, this run is {current['profile']!r}"]
    regressions = []
    old_types, new_types = baseline.get('types', {}), current['types']
    old_methods, new_methods = baseline.get('methods', {}), current['methods']
    if not scoped:
        for name in sorted(set(old_types) - set(new_types)):
            regressions.append(f'{name}: type disappeared from the inventory')
        for name in sorted(set(old_methods) - set(new_methods)):
            if old_methods[name] != 'excluded':
                regressions.append(f'{name}: method disappeared from the inventory')
    for name, state in sorted(new_methods.items()):
        old = old_methods.get(name)
        if old is None and state == 'no_counterpart':
            regressions.append(f'{name}: new method has no Mathcad counterpart')
        elif old is None and state in NEW_GAPS and name.split('::')[0] in old_types:
            regressions.append(f'{name}: new method is {state}')
        elif old is not None and state != old and state != 'compared' and not forward(old, state):
            regressions.append(f'{name}: {old} became {state}')
    for name, counts in sorted(new_types.items()):
        old = old_types.get(name)
        if old is None:
            continue  # A new type starts its own baseline; its stubs show as missing.
        for key in GOOD:
            # Hand-written methods only: lambdas may disappear when code is rewritten.
            now, before = (counts[key] - counts.get('excluded', 0), old.get(key, 0) - old.get('excluded', 0)) \
                if key == 'total' else (counts[key], old.get(key, 0))
            if now < before:
                regressions.append(f'{name}: {key} fell from {before} to {now}')
        now, before = sum(counts[k] for k in UNFINISHED), sum(old.get(k, 0) for k in UNFINISHED)
        if now > before:
            regressions.append(f'{name}: unfinished methods rose from {before} to {now}')
        if counts['verified_unsupported'] > old.get('verified_unsupported', 0):
            regressions.append(f"{name}: verified_unsupported rose from {old.get('verified_unsupported', 0)} to {counts['verified_unsupported']}")
    if not scoped:
        for finding in sorted((Counter(current['lint']) - Counter(baseline.get('lint', []))).elements()):
            regressions.append(f'new lint: {finding}')
    return regressions


def check(coverage, integrity, scoped, profile, path=BASELINE, baseline=False):
    """`baseline`: the parsed baseline (None when absent), when the caller read it before code under test ran."""
    current = snapshot(coverage, integrity, profile)
    if baseline is False:
        baseline = json.loads(Path(path).read_text()) if Path(path).is_file() else None
    if baseline is None:
        return {'ok': False, 'regressions': [f'No baseline at {path}; a maintainer must create it'], 'snapshot': current}
    regressions = compare(baseline, current, scoped)
    return {'ok': not regressions, 'regressions': regressions, 'snapshot': current}
