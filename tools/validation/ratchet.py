"""Validation may only improve: compare a run's per-method states, per-type totals and integrity findings with the
committed baseline."""
from collections import Counter
import json
from pathlib import Path
from validate import ROOT, test_class

BASELINE = ROOT / 'docs/mirror/validation-baseline.json'
# The phase profile's own baseline: its inventory spans every manifest project, and a baseline holds one profile.
PHASE_BASELINE = ROOT / 'docs/mirror/validation-baseline-phase.json'
SCHEMA = 4
# Baseline schemas compare() reads and the merge gate accepts from a validator. Schema 4 adds `bodies` and `writes`
# (validate.changed_methods), which narrow mutation selection, not the ratchet; from a schema 3 baseline the gate
# mutates whole changed types as before. `exact` (the methods whose IL matches Mathcad's instruction for instruction,
# MethodAudit's exact_match) is optional: a baseline without it holds no exact matches, and a validator that does not
# know it ignores it, so adding it needs no new schema.
READABLE = (3, 4)
# Snapshot fields compare() never reads; only mutation selection does (validate.changed_types, changed_methods).
MUTATION_ONLY = ('tests', 'bodies', 'writes')
# States in which a method's IL is not compared with Mathcad's at all.
UNCOMPARED = ('no_counterpart', 'excluded', 'declaration_only')
GOOD = ('compared', 'total')  # total (hand-written): deleting methods, even stubs, must not shrink 'missing'
# Compiler-generated methods (excluded), and types holding only those, may come and go with idiomatic code: mutation
# covers them through their owner.
# A method's stage (method_coverage.audit) says how far it has got; fixing one gap exposes the next, so a method may
# move forward between unfinished stages but never back. Tests that compare without checking instance state rank
# above having none, so deleting them is never progress. Per type, the unfinished methods may not grow in number, and
# a new method may not arrive with a gap that existing methods' progress would otherwise pay for.
STAGES = ('no_counterpart', 'missing', 'structure_mismatch', 'untested', 'unchecked_state', 'uncovered', 'compared')
UNFINISHED = ('missing', 'structure_mismatch', 'uncovered')
NEW_GAPS = ('structure_mismatch', 'unchecked_state', 'uncovered')


def owner(method, profile):
    """The key of a method's type: `Type`; in the phase profile `Project|Type`, since its inventory spans every manifest
    project and one type name can be compiled into several (shared source, shared native headers, compiler helpers)."""
    return f"{method['project']}|{method['type']}" if profile == 'phase' else method['type']


def snapshot(coverage, integrity, profile):
    """Identities, not just counts, so fixing one finding cannot pay for a new one. Methods are keyed
    `Type::signature`, or `Project|Type::signature` in the phase profile (see owner). The phase profile never mutates,
    so its snapshot leaves out MUTATION_ONLY."""
    from method_coverage import by_type
    methods = [dict(m, type=owner(m, profile)) for m in coverage['methods']]
    snap = {'schema': SCHEMA, 'profile': profile,
            'types': by_type(methods) if profile == 'phase' else coverage['by_type'],
            'methods': {f"{m['type']}::{m['signature']}": stage(m) for m in methods},
            'tests': {f"{m['type']}::{m['signature']}": sorted({test_class(c['test']) for c in m['cases']})
                      for m in methods if m.get('cases')},
            'lint': sorted(f"{f['file']} | {f['rule']} | {f['text']}" for f in integrity['lint']),
            'bodies': {f"{m['type']}::{m['signature']}": m['body_sha256'] for m in methods if m.get('body_sha256')},
            # Fields written, or read while holding a mutable reference (validate.changed_methods counts both).
            'writes': {f"{m['type']}::{m['signature']}": sorted({*m.get('field_writes', []), *m.get('reference_reads', [])})
                       for m in methods if m.get('field_writes') or m.get('reference_reads')},
            'exact': sorted(f"{m['type']}::{m['signature']}" for m in methods if m.get('exact_match'))}
    return {k: v for k, v in snap.items() if k not in MUTATION_ONLY} if profile == 'phase' else snap


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
            if old_types[name].get('excluded', 0) != old_types[name].get('total'):
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
    # A method whose IL matched Mathcad's exactly must keep matching; one that disappeared is reported above.
    exact = set(current.get('exact', ()))
    for name in sorted(set(baseline.get('exact', ())) - exact):
        if new_methods.get(name) in UNCOMPARED:
            regressions.append(f"{name}: matched Mathcad's IL exactly but is now {new_methods[name]}, so its IL is not compared")
        elif name in new_methods:
            regressions.append(f"{name}: IL no longer matches Mathcad's exactly")
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
