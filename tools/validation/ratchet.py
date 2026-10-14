"""Validation may only improve: compare a run's per-method states, per-type totals and integrity findings with the
committed baseline."""
import json
from pathlib import Path
from validate import ROOT

BASELINE = ROOT / 'docs/mirror/validation-baseline.json'
SCHEMA = 2
GOOD = ('compared', 'total')  # total: deleting methods (even stubs) must not shrink 'missing'
BAD = ('missing', 'structure_mismatch', 'uncovered', 'verified_unsupported', 'excluded')


def snapshot(coverage, integrity, profile):
    """Identities, not just counts, so fixing one finding cannot pay for a new one."""
    return {'schema': SCHEMA, 'profile': profile, 'types': coverage['by_type'],
            'methods': {f"{m['type']}::{m['signature']}": m['validation'] if m.get('original') else 'no_counterpart'
                        for m in coverage['methods']},
            'lint': sorted({f"{f['file']} | {f['rule']} | {f['text']}" for f in integrity['lint']}),
            'listing_errors': sorted(set(integrity['listings']['errors']))}


def compare(baseline, current, scoped=False):
    """Regressions of `current` against `baseline`. A scoped run only answers for its own types."""
    if baseline.get('schema') != SCHEMA:
        return [f"baseline schema {baseline.get('schema')!r} is not {SCHEMA}; a maintainer must regenerate it"]
    if baseline.get('profile') != current['profile']:
        return [f"baseline is for profile {baseline.get('profile')!r}, this run is {current['profile']!r}"]
    regressions = []
    old_types, new_types = baseline.get('types', {}), current['types']
    old_methods, new_methods = baseline.get('methods', {}), current['methods']
    if not scoped:
        for name in sorted(set(old_types) - set(new_types)):
            regressions.append(f'{name}: type disappeared from the inventory')
        for name in sorted(set(old_methods) - set(new_methods)):
            regressions.append(f'{name}: method disappeared from the inventory')
    for name, state in sorted(new_methods.items()):
        old = old_methods.get(name)
        if old is None and state == 'no_counterpart':
            regressions.append(f'{name}: new method has no Mathcad counterpart')
        elif old is not None and state != old and state != 'compared':
            regressions.append(f'{name}: {old} became {state}')
    for name, counts in sorted(new_types.items()):
        old = old_types.get(name)
        if old is None:
            continue  # A new type starts its own baseline; its stubs show as missing.
        for key in GOOD:
            if counts[key] < old.get(key, 0):
                regressions.append(f'{name}: {key} fell from {old.get(key, 0)} to {counts[key]}')
        for key in BAD:
            if counts[key] > old.get(key, 0):
                regressions.append(f'{name}: {key} rose from {old.get(key, 0)} to {counts[key]}')
    if not scoped:
        for key in ('lint', 'listing_errors'):
            for finding in sorted(set(current[key]) - set(baseline.get(key, []))):
                regressions.append(f'new {key.replace("_", " ")}: {finding}')
    return regressions


def check(coverage, integrity, scoped, profile, path=BASELINE):
    current = snapshot(coverage, integrity, profile)
    if not Path(path).is_file():
        return {'ok': False, 'regressions': [f'No baseline at {path}; a maintainer must create it'], 'snapshot': current}
    regressions = compare(json.loads(Path(path).read_text()), current, scoped)
    return {'ok': not regressions, 'regressions': regressions, 'snapshot': current}
