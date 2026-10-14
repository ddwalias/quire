"""Fail unless each inventoried method has an artifact-bound, direct, passing comparison and the original's IL structure."""
import argparse
import hashlib
import json
from pathlib import Path
import re
import xml.etree.ElementTree as ET
from branch_coverage import gaps
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


def accepted_deviations(repo=ROOT):
    """Accepted deviation id -> the `Quire.Type::Member` names its ledger index row scopes it to. Any other
    [MirrorDeviation] id, or an accepted id on a member outside its scope, is ignored."""
    path = Path(repo) / 'docs/mirror/deviations.md'
    if not path.is_file():
        return {}
    accepted = {}
    for line in path.read_text().splitlines():
        cells = [c.strip() for c in line.strip().strip('|').split('|')]
        if (len(cells) > 1 and re.fullmatch(r'DEV-\d{4}', cells[0])
                and any(c in ('accepted-temporary', 'accepted-permanent') for c in cells)):
            accepted[cells[0]] = set(re.findall(r'[\w.+]+::[\w.<>]+', cells[1].replace('`', '')))
    return accepted


def member(method):
    """`Quire.Type::Name` of a method; a deviation covers every overload of the name it scopes."""
    return f"{method['type']}::{re.split(r'[`(]', method['signature'])[0]}"


# The one 'missing' reason reached only after the structure check passed: such a method is further along than one
# whose structure still differs.
UNTESTED = 'No direct comparison from a passing test in this run'


def audit(inventory, records, tests, run, repo=ROOT, types=(), lines=None, deviations=None):
    """`lines` is merged cobertura data from the differential run; without it the coverage gate cannot pass."""
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
    contracts = {}
    deviations = accepted_deviations(repo) if deviations is None else deviations
    for record in records:
        if record.get('schema') != 1 or record.get('run') != run:
            raise ValueError('Stale or invalid direct-comparison record')
        if record['test'] not in tests:
            raise ValueError(f"Comparison came from a test without a passing result: {record['test']}")
        if not record.get('case_id') or 'inputs' not in record or 'arguments_after' not in record or not isinstance(record.get('outcome'), str):
            raise ValueError('Comparison must record a case, inputs and an outcome')
        if record['contract'] not in contracts:
            path = (evidence_dir / (record['contract'] + '.md')).resolve()
            if not path.is_relative_to(evidence_dir):
                raise ValueError('Contract escapes the evidence directory')
            contents = path.read_bytes()
            contracts[record['contract']] = path, hashlib.sha256(contents).hexdigest(), contents.decode('utf-8').lower()
        contract, digest, text = contracts[record['contract']]
        if digest != record['contract_sha256']:
            raise ValueError(f"Contract changed since comparison: {contract}")
        original = record['original']
        if original['sha256'].lower() not in text or original['mvid'].lower() not in text:
            raise ValueError(f"Contract is not bound to the original artifact: {contract}")
        if not re.search(r'0x0*' + format(original['token'], 'x') + r'\b', text):
            raise ValueError(f"Original method token is absent from contract: {contract}")
        key = identity(record['quire']), identity(original)
        observed.setdefault(key, []).append({'test': record['test'], 'case': record['case_id'], 'contract': record['contract'], 'state_checked': record.get('state_checked', False)})
    counts = {'compared': 0, 'declaration_only': 0, 'excluded': 0, 'missing': 0, 'structure_mismatch': 0, 'uncovered': 0}
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
            elif method['structure_matches'] is not True and not (method['status'] == 'Deviation' and member(method) in deviations.get(method.get('evidence'), ())):
                state, reason = 'structure_mismatch', 'IL structure differs from the original: ' + '; '.join(
                    method['structure_differences'] or ['not compared'])
            elif cases and lines is None:
                state, reason = 'uncovered', 'Branch coverage was not collected'
            elif cases and (gap := gaps(method.get('source'), lines)) is None:
                state, reason = 'uncovered', 'No source lines in the PDB; coverage cannot be attributed'
            elif cases and (gap['uncovered_lines'] or gap['partial_branches']):
                state, reason = 'uncovered', (f"Differential run leaves lines {gap['uncovered_lines']} unexecuted and "
                                              f"branches {gap['partial_branches']} partly covered")
            elif cases:
                state, reason = 'compared', 'Direct comparison passed for the listed cases only; every line and branch executed'
            else:
                state, reason = 'missing', UNTESTED
        counts[state] += 1
        unsupported = method['status'] == 'Verified' and state not in ('compared', 'declaration_only')
        rows.append(dict(method, validation=state, reason=reason, cases=cases, status_supported=not unsupported))
    counts['verified_unsupported'] = sum(not r['status_supported'] for r in rows)
    return {'schema': 1, 'run': run, 'complete': all(counts[k] == 0 for k in ('missing', 'structure_mismatch', 'uncovered', 'verified_unsupported')),
            'policy': 'Direct comparison and matching IL structure for every non-compiler method with a body, regardless of accessibility; accepted deviations may differ structurally',
            'scope': list(types) if types else 'all inventoried project methods',
            'limitations': 'Only recorded cases are compared. Test and renderer quality still need review; no status is upgraded automatically.',
            'counts': counts, 'by_access': {access: {state: sum(m['access'] == access and m['validation'] == state for m in rows)
                for state in counts if state != 'verified_unsupported'} for access in sorted({m['access'] for m in rows})},
            'by_type': by_type(rows), 'methods': rows}


def by_type(rows):
    """Per-type counts the ratchet compares between runs."""
    result = {}
    for row in rows:
        counts = result.setdefault(row['type'], dict.fromkeys(
            ('total', 'compared', 'declaration_only', 'excluded', 'missing', 'structure_mismatch', 'uncovered', 'verified_unsupported'), 0))
        counts['total'] += 1
        counts[row['validation']] += 1
        counts['verified_unsupported'] += not row['status_supported']
    return result


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
