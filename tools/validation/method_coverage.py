"""Fail unless each inventoried method has an artifact-bound, direct, passing comparison and the original's IL structure."""
import argparse
import hashlib
import json
from pathlib import Path
import re
import xml.etree.ElementTree as ET
from validate import NS, ROOT, read_trx


def passed_tests(paths):
    passed = set()
    for path in paths:
        read_trx(path)
        root = ET.parse(path).getroot()
        definitions = {}
        for test in root.findall('t:TestDefinitions/t:UnitTest', NS):
            method = test.find('t:TestMethod', NS)
            if method is not None:
                definitions[test.get('id')] = method.get('className', '').split(',')[0] + '.' + method.get('name', '')
        for result in root.findall('t:Results/t:UnitTestResult', NS):
            if result.get('outcome') == 'Passed' and result.get('testId') in definitions:
                passed.add(definitions[result.get('testId')])
    return passed


def identity(value):
    return value['assembly'], value['sha256'], value['mvid'], value['token']


def audit(inventory, records, tests, run, repo=ROOT, types=()):
    if inventory.get('schema') != 2 or inventory.get('errors'):
        raise ValueError(f"Invalid method inventory: {inventory.get('errors')}")
    methods = inventory['methods']
    keys = [(m['project'], m['token']) for m in methods]
    if len(set(keys)) != len(keys):
        raise ValueError('Duplicate methods in inventory')
    if types:
        unknown = set(types) - {m['type'] for m in methods}
        if unknown:
            raise ValueError(f"Types absent from inventory: {sorted(unknown)}")
        methods = [m for m in methods if m['type'] in types]
    if not methods:
        raise ValueError('No methods selected; empty inventories cannot pass')
    observed = {}
    evidence_dir = (Path(repo) / 'docs/mirror/evidence').resolve()
    for record in records:
        if record.get('schema') != 1 or record.get('run') != run:
            raise ValueError('Stale or invalid direct-comparison record')
        if record['test'] not in tests:
            raise ValueError(f"Comparison came from a test without a passing result: {record['test']}")
        if not record.get('case_id') or 'inputs' not in record or 'arguments_after' not in record or not isinstance(record.get('outcome'), str):
            raise ValueError('Comparison must record a case, inputs and an outcome')
        contract = (evidence_dir / (record['contract'] + '.md')).resolve()
        if not contract.is_relative_to(evidence_dir):
            raise ValueError('Contract escapes the evidence directory')
        contents = contract.read_bytes()
        if hashlib.sha256(contents).hexdigest() != record['contract_sha256']:
            raise ValueError(f"Contract changed since comparison: {contract}")
        text = contents.decode('utf-8').lower()
        original = record['original']
        if original['sha256'].lower() not in text or original['mvid'].lower() not in text:
            raise ValueError(f"Contract is not bound to the original artifact: {contract}")
        if not re.search(r'0x0*' + format(original['token'], 'x') + r'\b', text):
            raise ValueError(f"Original method token is absent from contract: {contract}")
        key = identity(record['quire']), identity(original)
        observed.setdefault(key, []).append({'test': record['test'], 'case': record['case_id'], 'contract': record['contract'], 'state_checked': record.get('state_checked', False)})
    counts = {'compared': 0, 'declaration_only': 0, 'excluded': 0, 'missing': 0, 'structure_mismatch': 0}
    rows = []
    for method in methods:
        cases = []
        if method.get('exclusion'):
            state, reason = 'excluded', method['exclusion']
        elif not method.get('original'):
            state, reason = 'missing', 'No exact Mathcad counterpart; needs resolution, never silently exempted'
        elif not method.get('shape_matches'):
            state, reason = 'missing', 'Mathcad method accessibility differs'
        elif method.get('declaration_only'):
            state, reason = 'declaration_only', 'Both artifacts declare a matching method without a body'
        else:
            q = {'assembly': method['project'], 'sha256': method['quire_sha256'], 'mvid': method['quire_mvid'], 'token': method['token']}
            cases = observed.get((identity(q), identity(method['original'])), [])
            if method['status'] in ('Stub', 'Blocked'):
                state, reason = 'missing', 'Method remains a stub or blocked'
            elif cases and not method['is_static'] and not method['is_constructor'] and not all(c['state_checked'] for c in cases):
                state, reason = 'missing', 'Instance method lacks checked before/after target state'
            elif method['structure_matches'] is not True and method['status'] != 'Deviation':
                state, reason = 'structure_mismatch', 'IL structure differs from the original: ' + '; '.join(
                    method['structure_differences'] or ['not compared'])
            elif cases:
                state, reason = 'compared', 'Direct comparison passed for the listed cases only'
            else:
                state, reason = 'missing', 'No direct comparison from a passing test in this run'
        counts[state] += 1
        rows.append(dict(method, validation=state, reason=reason, cases=cases))
    return {'schema': 1, 'run': run, 'complete': counts['missing'] == 0 and counts['structure_mismatch'] == 0,
            'policy': 'Direct comparison and matching IL structure for every non-compiler method with a body, regardless of accessibility; accepted deviations may differ structurally',
            'scope': list(types) if types else 'all inventoried project methods',
            'limitations': 'Only recorded cases are compared. Test and renderer quality still need review; no status is upgraded automatically.',
            'counts': counts, 'by_access': {access: {state: sum(m['access'] == access and m['validation'] == state for m in rows)
                for state in counts} for access in sorted({m['access'] for m in rows})}, 'methods': rows}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--inventory', required=True, type=Path)
    parser.add_argument('--cases', required=True, type=Path)
    parser.add_argument('--trx', required=True, action='append', type=Path)
    parser.add_argument('--run', required=True)
    parser.add_argument('--output', required=True, type=Path)
    parser.add_argument('--type', action='append', default=[], help='exact declaring type; omit for all methods')
    args = parser.parse_args()
    try:
        records = [json.loads(line) for path in args.cases.glob('*.jsonl') for line in path.read_text().splitlines() if line]
        report = audit(json.loads(args.inventory.read_text()), records, passed_tests(args.trx), args.run, types=args.type)
    except (OSError, ValueError, KeyError, TypeError, ET.ParseError) as error:
        report = {'complete': False, 'error': str(error)}
    args.output.write_text(json.dumps(report, indent=2) + '\n')
    print(json.dumps(report.get('counts', report)))
    return 0 if report['complete'] else 1


if __name__ == '__main__':
    raise SystemExit(main())
