"""Build production projects in Release and require changed methods to match Mathcad IL.

    python tools/validation/release_il.py --base main --project Quire.Mpl

No test host, test execution, coverage, mutation, status gate or stored baseline is used.
The base is built from a temporary checkout; the candidate is the current working tree.
Reports contain identities and verdicts, never original IL. --method selects additional
Type::signature keys; --type selects all methods declared on a type (including stubs).
"""
import argparse
import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import uuid

ROOT = Path(__file__).resolve().parents[2]
DEFAULT_INSTALL = (r'C:\Program Files\PTC\Mathcad Prime 12.0.1.0' if os.name == 'nt'
                   else '/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0')


def mathcad_install(environ=os.environ):
    return Path(environ.get('MATHCAD_DIR', DEFAULT_INSTALL))


def git(*args, cwd=ROOT):
    return subprocess.run(['git', *map(str, args)], cwd=cwd, capture_output=True,
                          text=True, check=True).stdout.strip()


def production_projects(root=ROOT):
    return {p.stem for p in (root / 'src').glob('*/*.csproj')}


def affected_projects(base, root=ROOT):
    paths = git('diff', '--name-only', base, cwd=root).splitlines()
    paths += git('ls-files', '--others', '--exclude-standard', cwd=root).splitlines()
    projects = production_projects(root)
    projects |= {Path(p).stem for p in git('ls-tree', '-r', '--name-only', base, '--', 'src', cwd=root).splitlines()
                 if p.endswith('.csproj')}
    # Shared compiler inputs can change every output, so they require the full IL scan.
    if any('/' not in p and p.endswith(('.props', '.targets', '.json', '.slnx')) for p in paths):
        return sorted(projects)
    return sorted({parts[1] for p in paths if len(parts := p.split('/')) >= 3
                   and parts[0] == 'src' and parts[1] in projects})


def selected(row, types, methods):
    key = f"{row['type']}::{row['signature']}"
    aliases = {key, row['id']}
    if declared := row.get('declared_signature'):
        aliases.add(f"{row['type']}::{declared}")
        if project := row.get('project'):
            aliases.add(f"{project}::{row['type']}::{declared}")
    return row['type'] in types or any(alias in methods for alias in aliases)


def compare(before, after, types=(), methods=()):
    """Changed bodies/declarations and every previously exact method must match.

    Unmapped changed methods are blockers, including generated helpers. Unchanged
    unfinished methods stay out of the batch. D10 engine-native methods are identified
    by MethodAudit from their original artifact and do not require a managed IL match.
    Removing a compiler-generated member with no Mathcad counterpart (a renumbered
    closure or state machine) does not fail the gate.
    """
    old = {m['id']: m for m in before['methods']}
    new = {m['id']: m for m in after['methods']}
    failures, checked = [], []
    for key, row in new.items():
        previous = old.get(key)
        changed = previous is None or any(previous.get(k) != row.get(k) for k in
                                         ('body_sha256', 'shape_matches', 'declaration_only'))
        if not (changed or selected(row, types, methods) or previous and previous.get('exact_match') is True):
            continue
        checked.append(key)
        if row.get('engine_native') is True:
            continue
        if row.get('shape_matches') is not True or not (row.get('exact_match') is True or row.get('declaration_only') is True):
            reason = ('no Mathcad counterpart' if row.get('original') is None else
                      'declaration differs' if row.get('shape_matches') is not True else
                      '; '.join(row.get('structure_differences', [])[:2]) or 'Release IL differs')
            failures.append({'method': key, 'reason': reason})
    for key in old.keys() - new.keys():
        # Renumbered compiler-generated members Mathcad never had; their replacements are new rows checked above.
        if old[key].get('original') is None and '<' in old[key]['type'] + old[key]['signature']:
            continue
        failures.append({'method': key, 'reason': 'method removed'})
    for name in types:
        if not any(m['type'] == name for m in new.values()):
            failures.append({'method': name, 'reason': 'selected type not found'})
    for name in methods:
        if not any(selected(m, (), (name,)) for m in new.values()):
            failures.append({'method': name, 'reason': 'selected method not found'})
    return {'passed': not failures, 'checked': checked, 'failures': failures,
            'exact': sum(m.get('exact_match') is True for m in new.values()),
            'engine_native': sum(m.get('engine_native') is True for m in new.values()),
            'unfinished': sum(m.get('exact_match') is not True and not m.get('declaration_only')
                              and not m.get('engine_native') for m in new.values())}


def run(command, cwd, log, timeout):
    with log.open('a', encoding='utf-8') as out:
        out.write('$ ' + subprocess.list2cmdline(list(map(str, command))) + '\n')
        out.flush()
        result = subprocess.run(list(map(str, command)), cwd=cwd, stdout=out,
                                stderr=subprocess.STDOUT, timeout=timeout)
    if result.returncode:
        detail = log.read_text(encoding='utf-8', errors='replace')[-2000:]
        raise RuntimeError(f'command failed ({result.returncode}); see {log}\n{detail}')


def build_and_scan(root, projects, tool, work, install, dotnet, log, timeout):
    assemblies = work / 'assemblies'
    for project in projects:
        target = root / 'src' / project / f'{project}.csproj'
        if not target.is_file():
            raise RuntimeError(f'production project missing from comparison: {target}')
        run([dotnet, 'build', target, '-c', 'Release', '--no-incremental', '-o', assemblies,
             '-nologo', '-v:q', '-p:EnableSourceControlManagerQueries=false'], root, log, timeout)
    inventory = work / 'inventory.json'
    run([dotnet, tool, '--repo', root, '--assemblies', assemblies, '--install', install,
         '--output', inventory, '--release-il',
         *(arg for p in projects for arg in ('--project', p))], root, log, timeout)
    data = json.loads(inventory.read_text(encoding='utf-8'))
    if data.get('errors'):
        raise RuntimeError('; '.join(data['errors']))
    return data


def check(base, projects, results, types=(), methods=(), dotnet='dotnet', timeout=1800,
          root=ROOT, install=None):
    install = Path(install or mathcad_install())
    results.mkdir(parents=True, exist_ok=False)
    log = results / 'build.log'
    base = git('rev-parse', '--verify', f'{base}^{{commit}}', cwd=root)
    report = {'base': base, 'tip': git('rev-parse', 'HEAD', cwd=root),
              'working_tree': git('status', '--porcelain', cwd=root), 'projects': projects,
              'types': list(types), 'methods': list(methods), 'passed': False}
    try:
        if not projects:
            if types or methods:
                raise RuntimeError('member selection requires a production project')
            report.update(passed=True, checked=[], failures=[], reason='no production project changed')
            return report
        with tempfile.TemporaryDirectory(prefix='quire-release-il-') as temp:
            work = Path(temp)
            original = work / 'base'
            run(['git', 'clone', '--quiet', '--shared', '--no-checkout', root, original], root, log, timeout)
            run(['git', '-c', 'core.hooksPath=', 'checkout', '--quiet', '--detach', base], original, log, timeout)
            tool_dir = work / 'tool'
            run([dotnet, 'build', root / 'tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj',
                 '-c', 'Release', '-o', tool_dir, '-nologo', '-v:q'], root, log, timeout)
            tool = tool_dir / 'Quire.Tools.MethodAudit.dll'
            print('Building base Release assemblies', flush=True)
            before = build_and_scan(original, projects, tool, work / 'before', install, dotnet, log, timeout)
            print('Building candidate Release assemblies', flush=True)
            after = build_and_scan(root, projects, tool, work / 'after', install, dotnet, log, timeout)
            # The same reference artifact must have been read by both scans.
            identities = lambda data: {m['original']['assembly']: (m['original']['sha256'], m['original']['mvid'])
                                       for m in data['methods'] if m.get('original')}
            old_id, new_id = identities(before), identities(after)
            if any(new_id.get(name) != identity for name, identity in old_id.items()):
                raise RuntimeError('Mathcad reference identity changed between scans')
            report.update(compare(before, after, types, methods))
            report['originals'] = new_id
            # Kept locally for exact signatures, tokens and artifact identities; no original IL.
            (results / 'base-inventory.json').write_text(json.dumps(before, indent=2) + '\n', encoding='utf-8')
            (results / 'inventory.json').write_text(json.dumps(after, indent=2) + '\n', encoding='utf-8')
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
        report['error'] = str(error)
    finally:
        (results / 'release-il.json').write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    return report


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--base', default='main')
    parser.add_argument('--project', action='append', default=[])
    parser.add_argument('--type', action='append', default=[])
    parser.add_argument('--method', action='append', default=[])
    parser.add_argument('--results', type=Path)
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--timeout', type=int, default=1800)
    args = parser.parse_args(argv)
    projects = sorted(set(args.project) if args.project else set(affected_projects(args.base)))
    unknown = set(projects) - production_projects()
    if unknown:
        parser.error(f'unknown production projects: {sorted(unknown)}')
    results = args.results or ROOT / '.mirror-cache' / ('release-il-' + uuid.uuid4().hex[:12])
    report = check(args.base, projects, results, args.type, args.method, args.dotnet, args.timeout)
    for failure in report.get('failures', [])[:20]:
        print(f"  {failure['method']}: {failure['reason']}")
    if len(report.get('failures', [])) > 20:
        print(f"  {len(report['failures']) - 20} more mismatches in the report")
    if report.get('error'):
        print(report['error'], file=sys.stderr)
    print(f"Release IL: {'PASS' if report['passed'] else 'FAIL'}; {len(report.get('checked', []))} checked; {results / 'release-il.json'}")
    return 0 if report['passed'] else 1


if __name__ == '__main__':
    sys.exit(main())
