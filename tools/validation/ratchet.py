"""Counts may only improve: compare a run's per-type totals with the committed baseline."""
import json
from pathlib import Path
from validate import ROOT

BASELINE = ROOT / 'docs/mirror/validation-baseline.json'
GOOD = ('compared',)
BAD = ('missing', 'structure_mismatch', 'uncovered', 'verified_unsupported')
GLOBAL_BAD = ('lint', 'listing_errors')


def snapshot(coverage, integrity):
    return {'schema': 1, 'types': coverage['by_type'],
            'lint': len(integrity['lint']), 'listing_errors': len(integrity['listings']['errors'])}


def compare(baseline, current, scoped=False):
    """Regressions of `current` against `baseline`. A scoped run only answers for its own types."""
    regressions = []
    old_types, new_types = baseline.get('types', {}), current['types']
    if not scoped:
        for name in sorted(set(old_types) - set(new_types)):
            regressions.append(f'{name}: type disappeared from the inventory')
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
        for key in GLOBAL_BAD:
            if current[key] > baseline.get(key, 0):
                regressions.append(f'{key} rose from {baseline.get(key, 0)} to {current[key]}')
    return regressions


def check(coverage, integrity, scoped, path=BASELINE):
    current = snapshot(coverage, integrity)
    if not Path(path).is_file():
        return {'ok': False, 'regressions': [f'No baseline at {path}; a maintainer must create it'], 'snapshot': current}
    regressions = compare(json.loads(Path(path).read_text()), current, scoped)
    return {'ok': not regressions, 'regressions': regressions, 'snapshot': current}
