"""Runs MemberGen on one project and keeps only what passes the merge gate's method checks: members whose Release IL
equals Mathcad's exactly (MethodAudit's exact_match), whose generated direct comparisons pass, and which the scoped
validation (tools/validation/validate.py) reports as fully covered with no surviving mutant. Each round restores the
project's generated files, tests and evidence, emits everything not yet rejected, builds, audits and tests; a rejected
member goes to the skip list and the next round starts over, until a round rejects nothing. On any failure the tree is
restored. See docs/mirror/tooling.md#membergen."""
import argparse
import json
import os
import re
import subprocess
import sys
import time
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DEFAULT_INSTALL = (r'C:\Program Files\PTC\Mathcad Prime 12.0.1.0' if os.name == 'nt'
                   else '/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0')
CACHE = ROOT / '.mirror-cache'
ERROR = re.compile(r'^(?P<path>[^(\n]+)\((?P<line>\d+),\d+\): error (?P<code>CS\d+): (?P<message>.*?)(?: \[[^\]]+\])?$', re.M)
TRX = '{http://microsoft.com/schemas/VisualStudio/TeamTest/2010}'
# The focused host runs mplrun comparisons portably; other projects need the Windows differential project.
HOSTS = {
    'Quire.Mpl': ('Quire.Focused.Differential.Tests', 'net10.0'),
    'Quire.Core': ('Quire.Differential.Tests', 'net10.0-windows'),
    'Quire.Calculation.Core': ('Quire.Differential.Tests', 'net10.0-windows'),
    'Quire.Calculation': ('Quire.Differential.Tests', 'net10.0-windows'),
    'Quire.Controls.Core': ('Quire.Differential.Tests', 'net10.0-windows'),
    'Quire.Controls': ('Quire.Differential.Tests', 'net10.0-windows'),
}


class Failure(Exception):
    """A run that cannot continue; the tree is restored before it is reported."""


def run(command, log, **kwargs):
    """Runs a command, appending its output to `log`; returns (exit code, output)."""
    result = subprocess.run([str(c) for c in command], cwd=ROOT, capture_output=True, text=True, **kwargs)
    with open(log, 'a') as f:
        f.write(f"$ {' '.join(map(str, command))}\n{result.stdout}{result.stderr}\n")
    return result.returncode, result.stdout + result.stderr


def paths(project):
    return [f'src/{project}', 'tests/Quire.Differential.Tests', 'docs/mirror/evidence']


def restore(project):
    """Puts the project's sources, the differential tests and the evidence back as committed (the harness's own
    Mechanical.cs included), deleting files an earlier round added."""
    subprocess.run(['git', 'checkout', '--', *paths(project)], cwd=ROOT, check=True)
    added = subprocess.run(['git', 'ls-files', '--others', '--exclude-standard', '-z', *paths(project)], cwd=ROOT,
                           capture_output=True, check=True).stdout.decode().split('\0')
    for name in filter(None, added):
        os.remove(ROOT / name)


def owners(report):
    """(repo path, line) → member id for every emitted source line (a moved field's lines belong to the member that
    moved it), and test class.method → member id."""
    lines, tests = {}, {}
    for f in report['files']:
        for span in f['members']:
            owner = span['member'].get('owner') or span['member']['id']
            for n in range(span['first'], span['last'] + 1):
                lines[(f['file'], n)] = owner
    for m in report['members']:
        for t in m['tests']:
            tests[f"Quire.Differential.Tests.{m['test_class']}.{t}"] = m['id']
    return lines, tests


def test_owner(path, line, tests, root=ROOT):
    """The member whose generated fact holds `line` of a test file."""
    text = (root / path).read_text().split('\n')
    for n in range(min(line, len(text)) - 1, -1, -1):
        found = re.search(r'public void (\w+)\(', text[n])
        if found:
            name = f"Quire.Differential.Tests.{Path(path).stem}.{found[1]}"
            return tests.get(name)
    return None


def methods(report):
    """`Owner::Key` (MethodAudit's and the validation's method name) → member id, for every emitted method."""
    return {method.split('::', 1)[1]: m['id'] for m in report['members'] for method in m['methods']}


def outcomes(trx, tests):
    """Each generated fact's outcome; fails unless the run reported every one of them."""
    if not trx.exists():
        raise Failure(f'no test results ({trx}); see the log')
    results = {r.get('testName'): r.get('outcome') for r in ET.parse(trx).getroot().iter(f'{TRX}UnitTestResult')}
    missing = sorted(name for name in tests if name not in results)
    if missing:
        raise Failure(f'{len(missing)} of {len(tests)} generated tests did not run, e.g. {missing[0]}')
    return results


def coverage_rejections(validation, report):
    """Members whose methods the scoped validation does not report as compared (covered, with a passing exact
    comparison), or whose mutants survive."""
    rejected = {}
    owner = methods(report)
    for row in json.loads((validation / 'method-coverage.json').read_text())['methods']:
        member = owner.get(f"{row['type']}::{row['signature']}")
        if member and row['stage'] != 'compared':
            rejected.setdefault(member, f"coverage: {row['stage']}: {str(row.get('reason'))[:120]}")
    mutation = json.loads((validation / 'summary.json').read_text()).get('mutation') or {}
    for survivor in mutation.get('survivors', []):
        member = owner.get(survivor.split('@')[0])
        if member:
            rejected.setdefault(member, f"mutation: survivor {survivor.split('@', 1)[1]}")
    return rejected


def accept(args, work, log):
    host, framework = HOSTS[args.project]
    flags = ['-nologo', '-v:q', '-p:EnableWindowsTargeting=true', '--disable-build-servers']
    for tool in ('MemberGen', 'MethodAudit'):
        code, _ = run([args.dotnet, 'build', f'tools/Quire.Tools.{tool}', *flags, '-o', work / tool], log)
        if code:
            raise Failure(f'building {tool} failed; see {log}')
    # The stub build MemberGen reads (statuses and PDB lines of the committed generated files).
    code, _ = run([args.dotnet, 'build', f'src/{args.project}', *flags, '-o', work / 'pristine'], log)
    if code:
        raise Failure(f'building {args.project} failed; see {log}')
    skip, reasons = {}, {}
    report = None
    for round in range(1, args.rounds + 1):
        restore(args.project)
        (work / 'skip.txt').write_text(''.join(f'{s}\n' for s in skip))
        code, _ = run([args.dotnet, work / 'MemberGen' / 'Quire.Tools.MemberGen.dll', 'emit', '--repo', ROOT,
                       '--install', args.install, '--project', args.project, '--assemblies', work / 'pristine',
                       '--skip', work / 'skip.txt', '--report', work / 'report.json']
                      + (['--include-tier-a'] if args.include_tier_a else []), log)
        if code:
            raise Failure(f'MemberGen emit failed; see {log}')
        report = json.loads((work / 'report.json').read_text())
        lines, tests = owners(report)
        print(f'round {round}: {len(report["members"])} members emitted', flush=True)
        if not report['members']:
            break
        rejected = {}
        code, out = run([args.dotnet, 'build', f'tests/{host}', *flags], log)
        for e in ERROR.finditer(out):
            path = os.path.relpath(e['path'].strip(), ROOT).replace('\\', '/')
            owner = lines.get((path, int(e['line']))) or (test_owner(path, int(e['line']), tests) if path.startswith('tests/') else None)
            if owner is None:
                raise Failure(f'build error outside the emitted members: {e[0]}')
            rejected.setdefault(owner, f"compile: {e['code']} {e['message'][:120]}")
        if code and not rejected:
            raise Failure(f'build failed without attributable errors; see {log}')
        if not rejected:
            release = work / f'release{round}'
            code, _ = run([args.dotnet, 'build', f'tests/{host}', *flags, '-c', 'Release', '--no-incremental', '-o', release], log)
            if code:
                raise Failure(f'Release build failed; see {log}')
            inventory = work / f'inventory{round}.json'
            code, _ = run([args.dotnet, work / 'MethodAudit' / 'Quire.Tools.MethodAudit.dll', '--repo', ROOT, '--assemblies',
                           ROOT / 'tests' / host / 'bin' / 'Debug' / framework, '--structure-assemblies', release,
                           '--install', args.install, '--output', inventory, '--project', args.project], log)
            if code:
                raise Failure(f'MethodAudit exited {code}; see {log}')
            audited = {m['id']: m for m in json.loads(inventory.read_text())['methods']}
            for m in report['members']:
                for method in m['methods']:
                    row = audited.get(method)
                    if row is None or row['exact_match'] is not True:
                        why = ('absent from the inventory' if row is None
                               else 'no Mathcad counterpart' if row['exact_match'] is None
                               else '; '.join(row['structure_differences'][:1]) or 'opcode sequence differs')
                        rejected.setdefault(m['id'], f'IL: {why}')
            if not args.no_test:
                trx = work / f'tests{round}.trx'
                env = dict(os.environ, QUIRE_REQUIRE_MATHCAD='1', MATHCAD_DIR=args.install)
                run([args.dotnet, 'test', ROOT / 'tests' / host / 'bin' / 'Debug' / framework / 'Quire.Differential.Tests.dll',
                     '--filter', 'FullyQualifiedName~MechanicalDifferentialTests', '--logger', f'trx;LogFileName={trx.name}',
                     '--results-directory', work], log, env=env)
                results = outcomes(trx, tests)
                for name, member in tests.items():
                    if results[name] != 'Passed':
                        rejected.setdefault(member, f'test: {results[name]} {name.rsplit(".", 1)[1]}')
        if not rejected and args.project == 'Quire.Mpl':
            # The gate's own coverage (and mutation) audit of the touched types.
            validation = work / f'validation{round}'
            types = sorted({method.split('::')[1] for m in report['members'] for method in m['methods']})
            code, _ = run([sys.executable, ROOT / 'tools' / 'validation' / 'validate.py', '--profile', 'focused',
                           '--dotnet', args.dotnet, '--results', validation,
                           *(a for t in types for a in ('--method-type', t)), *([] if args.no_mutate else ['--mutate'])], log)
            if not (validation / 'method-coverage.json').exists():
                raise Failure(f'validation exited {code} without a coverage report; see {validation}')
            rejected = coverage_rejections(validation, report)
            # 0: complete; 3: passes with gaps (the stubs left in these types). Anything else nothing here explains.
            if not rejected and code not in (0, 3):
                raise Failure(f'validation exited {code} with no member to reject; see {validation}')
        if not rejected:
            break
        print(f'  rejected {len(rejected)}: ' + ', '.join(sorted({r.split(":")[0] for r in rejected.values()})), flush=True)
        skip.update(rejected)
        reasons.update(rejected)
    else:
        raise Failure(f'no fixed point after {args.rounds} rounds; see {work}')
    return {'project': args.project, 'kept': [m['id'] for m in report['members']],
            'fields_moved': sum(1 for f in report['files'] for s in f['members'] if s['member']['kind'] == 'field'),
            'rejected': reasons, 'rounds': round}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--project', required=True)
    parser.add_argument('--install', default=os.environ.get('MATHCAD_DIR', DEFAULT_INSTALL))
    parser.add_argument('--work', type=Path, help='a new directory for builds, reports and logs '
                        '(default: .mirror-cache/membergen/<project>-<time>)')
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--rounds', type=int, default=8)
    parser.add_argument('--include-tier-a', action='store_true',
                        help='also generate Tier A/A2 members (a lane, for its own members, under normal review)')
    parser.add_argument('--no-mutate', action='store_true', help='skip mutation in the coverage step (faster; the gate '
                        'still mutates, so a survivor then blocks the merge instead of being rejected here)')
    parser.add_argument('--no-test', action='store_true', help='skip running differential tests (e.g. where Release IL match is the acceptance bar)')
    args = parser.parse_args()
    if args.project not in HOSTS:
        sys.exit(f'{args.project}: no portable differential host; only {", ".join(HOSTS)} runs here')
    if subprocess.run(['git', 'status', '--porcelain', '--', *paths(args.project)], cwd=ROOT, capture_output=True,
                      text=True, check=True).stdout.strip():
        sys.exit(f'commit or stash changes under {", ".join(paths(args.project))} first: each round restores them')
    CACHE.mkdir(exist_ok=True)
    lock = CACHE / 'membergen.lock'
    try:
        fd = os.open(lock, os.O_CREAT | os.O_EXCL | os.O_WRONLY)
    except FileExistsError:
        sys.exit(f'{lock} exists: another accept run is using this worktree (delete the file if none is)')
    try:
        os.write(fd, f'{os.getpid()}\n'.encode())
        os.close(fd)
        work = args.work or CACHE / 'membergen' / f"{args.project}-{time.strftime('%Y%m%d-%H%M%S')}"
        work.mkdir(parents=True, exist_ok=False)
        log = work / 'log.txt'
        print(f'MemberGen accept {args.project}: work directory {work}', flush=True)
        try:
            summary = accept(args, work, log)
        except BaseException as error:
            restore(args.project)
            if isinstance(error, Failure):
                sys.exit(f'{error}; the tree is restored')
            raise
    finally:
        os.remove(lock)
    (work / 'summary.json').write_text(json.dumps(summary, indent=2) + '\n')
    print(f"kept {len(summary['kept'])} members (+{summary['fields_moved']} fields), rejected {len(summary['rejected'])}; "
          f"{work / 'summary.json'}")


if __name__ == '__main__':
    main()
