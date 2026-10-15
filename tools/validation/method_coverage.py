"""Fail unless each inventoried method has an artifact-bound, direct, passing comparison and the original's IL structure."""
import argparse
import hashlib
import json
from pathlib import Path
import re
import sys
import xml.etree.ElementTree as ET
from branch_coverage import gaps
from validate import NS, ROOT, read_trx

UNREACHABLE = ROOT / 'docs/mirror/unreachable-lines.txt'


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


def unreachable_lines(path=UNREACHABLE, repo=ROOT):
    """`Type::signature` -> {(path, line): {'sha256', 'reason'}} of a
    `<method key> <path>:<line> sha256=<hash> # reason` per line file (see span_sha256); empty when it does not exist.
    The path is relative to the repository, under src/, and must exist. The accepted file is maintainer-owned."""
    entries = {}
    if not Path(path).is_file():
        return entries
    for text in Path(path).read_text().splitlines():
        if not text.strip() or text.lstrip().startswith('#'):
            continue
        key, location, value = parse_entry(text, path, repo)
        if location in entries.setdefault(key, {}):
            raise ValueError(f"{path}: duplicate entry {key} {location[0]}:{location[1]}")
        entries[key][location] = value
    return entries


def parse_entry(text, where, repo=ROOT):
    """(method key, (path, line), {'sha256', 'reason'}) of one entry line."""
    head, _, reason = text.partition(' # ')
    fields = head.split()
    file, _, number = fields[1].rpartition(':') if len(fields) == 3 else ('', '', '')
    if (len(fields) != 3 or not number.isdigit() or not re.fullmatch(r'sha256=[0-9a-f]{64}', fields[2])
            or not reason.strip()):
        raise ValueError(f"{where}: expected `<method key> <path>:<line> sha256=<hash> # reason`, got {text!r}")
    if not file.startswith('src/') or '..' in file.split('/') or not (Path(repo) / file).is_file():
        raise ValueError(f"{where}: {file} is not an existing repository path under src/")
    return fields[0], (file, int(number)), {'sha256': fields[2][len('sha256='):], 'reason': reason.strip()}


def read_sources(*entry_sets, repo=ROOT):
    """Repository path -> its bytes, for every path the entries name; read before the build and any code under test."""
    return {file: (Path(repo) / file).read_bytes()
            for entries in entry_sets for locations in entries.values() for file, _ in locations}


def relative(file, repo=ROOT):
    """A PDB document path relative to the repository (posix), or None outside it; case-insensitive for
    Windows-style paths."""
    file, root = file.replace('\\', '/'), str(repo).replace('\\', '/')
    root = (root if re.match(r'[A-Za-z]:/', root) else Path(repo).resolve().as_posix()).rstrip('/') + '/'
    windows = re.match(r'[A-Za-z]:/', file) is not None
    inside = file.lower().startswith(root.lower()) if windows else file.startswith(root)
    return file[len(root):] if inside else None


def matches(file, path, repo=ROOT):
    """Whether a PDB document is exactly the repository file `path`."""
    rel = relative(file, repo)
    return rel is not None and (rel == path or (re.match(r'[A-Za-z]:', file) is not None and rel.lower() == path.lower()))


def look_alike(file, path, repo=ROOT):
    """A PDB document that ends in `path` without being it (e.g. src/X/Sub/src/X/T.cs, or one outside the repository)."""
    file = file.replace('\\', '/')
    windows = re.match(r'[A-Za-z]:', file) is not None
    ends = (file.lower().endswith('/' + path.lower()) if windows else file.endswith('/' + path))
    return ends and not matches(file, path, repo)


def listed(file, number, entries, repo=ROOT):
    """The (path, line) entry naming this PDB source line."""
    return next((e for e in entries if e[1] == number and matches(file, e[0], repo)), None)


def exempt(gap, accepted, proposed, repo=ROOT):
    """A method's gap without the uncovered lines listed as unreachable for it, which it names apart. Only lines are
    exempt: a partial branch whose missing arm is a listed line stays partial."""
    result = dict(gap, uncovered_lines=[], exempt_lines=[], proposed_exempt_lines=[])
    for item in gap['uncovered_lines']:
        file, _, number = item.rpartition(':')
        bucket = ('exempt_lines' if listed(file, int(number), accepted, repo) else
                  'proposed_exempt_lines' if listed(file, int(number), proposed, repo) else 'uncovered_lines')
        result[bucket].append(item)
    return result


def span_sha256(method, path, data, number, repo=ROOT):
    """SHA-256 of the listed line's offset from the method's first statement line in `path`, then the method's
    statement lines there (all its PDB lines), each stripped of trailing whitespace, joined by newlines: an entry stays
    valid only while the method's source, and the line's place in it, are the ones reviewed."""
    numbers = sorted({n for d in method.get('source') or [] if matches(d['file'], path, repo) for n in d['lines']})
    text = data.decode('utf-8-sig').splitlines()
    span = '\n'.join(text[n - 1].rstrip() if n <= len(text) else '' for n in numbers)
    return hashlib.sha256(f"{number - numbers[0] if numbers else 0}\n{span}".encode('utf-8')).hexdigest()


def checksum_matches(document, data):
    """Whether the SHA-256 checksum the compiler recorded for a PDB document is that of `data`, the file as read before
    the build. Weaker algorithms never match."""
    checksum = document.get('checksum') or {}
    return checksum.get('algorithm') == 'sha256' and hashlib.sha256(data).hexdigest() == checksum.get('hash')


def by_method_key(methods):
    """`Type::signature` -> the methods with that key: the phase profile mirrors some types (Quire.ProductInfo
    constants) in several assemblies."""
    keys = {}
    for method in methods:
        keys.setdefault(f"{method['type']}::{method['signature']}", []).append(method)
    return keys


def check_unreachable(methods, lines, entries, origin, sources, repo=ROOT):
    """The entries that may exempt a line, and the stale ones: whose method is not in the inventory; which has a PDB
    document only resembling the listed path; whose key names several methods (one per assembly); which name no statement line of it; a line the differential run
    executed (so reachable); a line another method also has statements on (a lambda on the owner's line); a file whose
    compiled checksum is not that of the file read before the build; or a method whose source changed since the
    entry was hashed."""
    valid, stale = {}, []
    by_key = by_method_key(methods)
    for key, locations in sorted(entries.items()):
        for (file, number), entry in sorted(locations.items()):
            candidates = by_key.get(key, [])
            method = candidates[0] if len(candidates) == 1 else {}
            documents = method.get('source') or []
            exact = [d for d in documents if matches(d['file'], file, repo)]
            shared = {f"{m['type']}::{m['signature']}" for m in methods if m is not method and any(
                number in d['lines'] and matches(d['file'], file, repo) for d in m.get('source') or [])}
            if not candidates:
                problem = 'no such method'
            elif len(candidates) > 1:
                problem = 'ambiguous method key'
            elif any(look_alike(d['file'], file, repo) for d in documents):
                problem = 'document path mismatch'
            elif not any(number in d['lines'] for d in exact):
                problem = 'not a source line of the method'
            elif lines is not None and any(lines.get((Path(d['file']).as_posix(), number), (0,))[0] for d in exact):
                problem = 'executed'
            elif shared:
                problem = f"line shared with {', '.join(sorted(shared))}"
            elif not all('checksum' in d for d in exact):
                problem = 'inventory lacks checksums; rerun'
            elif not all(checksum_matches(d, sources[file]) for d in exact):
                problem = 'document checksum mismatch'
            elif span_sha256(method, file, sources[file], number, repo) != entry['sha256']:
                problem = 'method source changed'
            else:
                valid.setdefault(key, {})[(file, number)] = entry
                continue
            stale.append(dict(key=key, line=f'{file}:{number}', reason=entry['reason'], source=origin, problem=problem))
    return valid, stale


def entry(methods, key, location, reason, repo=ROOT):
    """A correctly hashed `unreachable-lines.txt` entry for a method of a method inventory or coverage report."""
    key, (file, number), value = parse_entry(f"{key} {location} sha256={'0' * 64} # {reason}", 'entry', repo)
    candidates = by_method_key(methods).get(key, [])
    if len(candidates) != 1:
        raise ValueError(f"{key} is {'ambiguous in' if candidates else 'not in'} the report")
    method = candidates[0]
    entries = {key: {(file, number): value}}
    sources = read_sources(entries, repo=repo)
    value['sha256'] = span_sha256(method, file, sources[file], number, repo)
    _, stale = check_unreachable(methods, None, entries, 'accepted', sources, repo)
    if stale:
        raise ValueError(f"{key} {location}: {stale[0]['problem']}")
    return f"{key} {file}:{number} sha256={value['sha256']} # {reason}"


def audit(inventory, records, tests, run, repo=ROOT, types=(), lines=None, deviations=None, unreachable=None,
          proposed_unreachable=None, sources=None):
    """`lines` is merged cobertura data from the differential run; without it the coverage gate cannot pass.
    `unreachable`: accepted `unreachable_lines` (default: the repository's file); `proposed_unreachable`: entries
    proposed for review in a local run, which exempt lines the same way but leave the run incomplete; `sources`:
    `read_sources` of both, as read before the build (default: read now)."""
    if inventory.get('schema') != 2 or inventory.get('errors'):
        raise ValueError(f"Invalid method inventory: {inventory.get('errors')}")
    methods = inventory['methods']
    unreachable = unreachable_lines(Path(repo) / 'docs/mirror/unreachable-lines.txt', repo) if unreachable is None else unreachable
    proposed_unreachable = proposed_unreachable or {}
    sources = read_sources(unreachable, proposed_unreachable, repo=repo) if sources is None else sources
    unreachable, stale = check_unreachable(methods, lines, unreachable, 'accepted', sources, repo)
    proposed_unreachable, stale_proposed = check_unreachable(methods, lines, proposed_unreachable, 'proposed', sources,
                                                             repo)
    stale += stale_proposed
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
        cases, stage, gap, name = [], None, None, f"{method['type']}::{method['signature']}"
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
                state, stage, reason = 'missing', 'unchecked_state', 'Instance method lacks checked before/after target state'
            elif method['structure_matches'] is not True and not (method['status'] == 'Deviation' and member(method) in deviations.get(method.get('evidence'), ())):
                state, reason = 'structure_mismatch', 'IL structure differs from the original: ' + '; '.join(
                    method['structure_differences'] or ['not compared'])
            elif cases and lines is None:
                state, reason = 'uncovered', 'Branch coverage was not collected'
            elif cases and (gap := gaps(method.get('source'), lines)) is None:
                state, reason = 'uncovered', 'No source lines in the PDB; coverage cannot be attributed'
            elif cases and (gap := exempt(gap, unreachable.get(name, {}), proposed_unreachable.get(name, {}), repo)) and (
                    gap['uncovered_lines'] or gap['partial_branches']):
                state, reason = 'uncovered', (f"Differential run leaves lines {gap['uncovered_lines']} unexecuted and "
                                              f"branches {gap['partial_branches']} partly covered")
            elif cases:
                state, reason = 'compared', 'Direct comparison passed for the listed cases only; every line and branch executed'
                if gap['exempt_lines'] or gap['proposed_exempt_lines']:
                    reason += ' apart from lines listed as unreachable'
            else:
                state, stage, reason = 'missing', 'untested', 'No direct comparison from a passing test in this run'
        counts[state] += 1
        unsupported = method['status'] == 'Verified' and state not in ('compared', 'declaration_only')
        # `missing` covers several gaps; the ratchet orders methods by `stage` (see ratchet.STAGES).
        rows.append(dict(method, validation=state, stage=stage or state, reason=reason, cases=cases,
                         status_supported=not unsupported, exempt_lines=gap and gap.get('exempt_lines') or [],
                         proposed_exempt_lines=gap and gap.get('proposed_exempt_lines') or []))
    counts['verified_unsupported'] = sum(not r['status_supported'] for r in rows)
    # A listed line that executed is reachable, so its entry is wrong; any stale accepted entry needs a maintainer;
    # a proposed exemption is not yet accepted.
    unsettled = [e for e in stale if e['problem'] == 'executed' or e['source'] == 'accepted']
    proposed_used = any(r['validation'] == 'compared' and r['proposed_exempt_lines'] for r in rows)
    return {'schema': 1, 'run': run, 'complete': all(counts[k] == 0 for k in ('missing', 'structure_mismatch', 'uncovered', 'verified_unsupported'))
            and not unsettled and not proposed_used,
            'policy': 'Direct comparison and matching IL structure for every non-compiler method with a body, regardless of accessibility; accepted deviations may differ structurally',
            'scope': list(types) if types else 'all inventoried project methods',
            'limitations': 'Only recorded cases are compared. Test and renderer quality still need review; no status is upgraded automatically.',
            'counts': counts, 'by_access': {access: {state: sum(m['access'] == access and m['validation'] == state for m in rows)
                for state in counts if state != 'verified_unsupported'} for access in sorted({m['access'] for m in rows})},
            'by_type': by_type(rows), 'stale_unreachable': stale, 'methods': rows}


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
    if sys.argv[1:2] == ['unreachable-entry']:
        parser = argparse.ArgumentParser(prog='method_coverage.py unreachable-entry',
                                         description='Print a hashed unreachable-lines.txt entry.')
        parser.add_argument('report', type=Path, help='method-coverage.json or method-inventory.json of a run')
        parser.add_argument('key', help='Quire.Type::signature')
        parser.add_argument('location', help='repository-relative src/ path and line, e.g. src/Quire.Mpl/X.cs:42')
        parser.add_argument('reason', help='why the line cannot execute in the original either')
        args = parser.parse_args(sys.argv[2:])
        try:
            print(entry(json.loads(args.report.read_text())['methods'], args.key, args.location, args.reason))
        except (OSError, ValueError, KeyError) as error:
            parser.error(str(error))
        return 0
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
