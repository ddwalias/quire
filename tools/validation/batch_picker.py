"""Rank remaining Tier A members from a MethodAudit inventory or method-coverage report."""
import argparse
import heapq
import json
from collections import Counter, defaultdict
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
TIER_A = ROOT / 'docs/mirror/tiers/tier-a.txt'
STATUSES = ('exact-but-untested', 'structure_mismatch', 'uncovered', 'missing')


def classify(method):
    validation = method.get('validation')
    if validation == 'uncovered':
        return 'uncovered'
    if validation == 'structure_mismatch':
        return 'structure_mismatch'
    if validation == 'missing':
        if method.get('exact_match') is True and method.get('stage') == 'untested':
            return 'exact-but-untested'
        return 'missing'
    if validation == 'compared':
        return None if method.get('exact_match') is True else 'structure_mismatch'
    if validation == 'declaration_only':
        # No implementation body exists to pick for a lane batch.
        return None
    # A MethodAudit inventory has exact_match but no validation stage.
    if method.get('exact_match') is True:
        return 'exact-but-untested'
    if method.get('structure_matches') is False:
        return 'structure_mismatch'
    return 'missing'


def read_tier(path):
    members = []
    for line in Path(path).read_text().splitlines():
        line = line.strip()
        if line and not line.startswith('#'):
            members.append(line)
    return members


def rank_members(members):
    """Stable leaf-first topological order, prioritizing exact untested among currently-ready methods."""
    by_token = {(m.get('project'), m.get('token')): m['key'] for m in members.values()
                if m.get('project') is not None and m.get('token') is not None}
    dependencies = {key: set() for key in members}
    reverse = defaultdict(set)
    for key, member in members.items():
        for token in member.get('calls') or []:
            callee = by_token.get((member.get('project'), token))
            if (callee in members and callee != key
                    and callee.split('::', 1)[0] == key.split('::', 1)[0]):
                dependencies[key].add(callee)
                reverse[callee].add(key)
    remaining = {key: set(value) for key, value in dependencies.items()}

    def priority(key):
        m = members[key]
        return (0 if m['status'] == 'exact-but-untested' else 1,
                m['type'], m['signature'])

    ready = [(priority(key), key) for key, deps in remaining.items() if not deps]
    heapq.heapify(ready)
    result = []
    while ready:
        _, key = heapq.heappop(ready)
        result.append(key)
        for caller in reverse[key]:
            remaining[caller].discard(key)
            if not remaining[caller]:
                heapq.heappush(ready, (priority(caller), caller))
    # Recursive call cycles have no leaf; retain deterministic ordering for the cycle remainder.
    result.extend(sorted((key for key in remaining if key not in result), key=priority))
    return result


def build(coverage, tier, types, type_prefixes=()):
    methods = {}
    for row in coverage.get('methods', []):
        key = f"{row.get('type')}::{row.get('signature')}"
        methods[key] = dict(row, key=key)
    result = {}
    for key in tier:
        type_name, signature = key.split('::', 1)
        if types and type_name not in types:
            continue
        if type_prefixes and not type_name.startswith(tuple(type_prefixes)):
            continue
        row = methods.get(key)
        if row is None:
            row = {'key': key, 'type': type_name, 'signature': signature,
                   'project': type_name.split('.')[0] if '.' in type_name else None,
                   'calls': [], 'status': 'missing'}
        else:
            row['status'] = classify(row)
        if row['status'] is not None:
            result[key] = row
    return result


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('coverage', type=Path, help='focused method-coverage.json or MethodAudit method-inventory.json')
    parser.add_argument('--method-type', action='append', default=[], help='exact type name; repeat to select several')
    parser.add_argument('--type-prefix', action='append', default=[],
                        help='select every type beginning with this namespace prefix; repeat to select several')
    parser.add_argument('--next-size', type=int, default=20, help='suggested batch size (15–30; default 20)')
    parser.add_argument('--tier-a', type=Path, default=TIER_A)
    args = parser.parse_args(argv)
    if not 15 <= args.next_size <= 30:
        parser.error('--next-size must be between 15 and 30')
    report = json.loads(args.coverage.read_text())
    selected = build(report, read_tier(args.tier_a), set(args.method_type), args.type_prefix)
    totals = defaultdict(Counter)
    for member in selected.values():
        totals[member['type']][member['status']] += 1
    ranked = rank_members(selected)
    print('REMAINING TIER A')
    for key in ranked:
        print(f"{selected[key]['status']}\t{key}")
    print('TOTALS BY TYPE')
    for type_name in sorted(totals):
        c = totals[type_name]
        print(f"{type_name}\tremaining={sum(c.values())}\t" +
              '\t'.join(f'{status}={c[status]}' for status in STATUSES))
    print(f'SUGGESTED NEXT BATCH ({min(args.next_size, len(ranked))} members)')
    for key in ranked[:args.next_size]:
        print(f"{selected[key]['status']}\t{key}")
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
