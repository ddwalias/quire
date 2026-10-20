"""Generate mechanical members and keep only exact Release IL matches with Mathcad.
Each round restores the project, emits remaining candidates, builds Release and rejects
IL mismatches. No test host, tests, coverage or mutation is used. On failure the tree is
restored. See docs/mirror/PLAN.md (D17)."""
import argparse
import json
import os
import re
import subprocess
import sys
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DEFAULT_INSTALL = (r'C:\Program Files\PTC\Mathcad Prime 12.0.1.0' if os.name == 'nt'
                   else '/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0')
CACHE = ROOT / '.mirror-cache'
ERROR = re.compile(r'^(?P<path>[^(\n]+)\((?P<line>\d+),\d+\): error (?P<code>CS\d+): (?P<message>.*?)(?: \[[^\]]+\])?$', re.M)


class Failure(Exception):
    """A run that cannot continue; the tree is restored before it is reported."""


def to_host_path(p, is_windows):
    s = str(p)
    if is_windows and s.startswith('/mnt/'):
        return re.sub(r'^/mnt/([a-zA-Z])/', lambda m: m.group(1).upper() + ':/', s)
    return s


def run(command, log, **kwargs):
    """Runs a command, appending its output to `log`; returns (exit code, output)."""
    is_windows = str(command[0]).lower().endswith('.exe')
    cmd = [str(command[0])] + [to_host_path(c, is_windows) for c in command[1:]]
    result = subprocess.run(cmd, cwd=ROOT, capture_output=True, text=True, **kwargs)
    with open(log, 'a') as f:
        f.write(f"$ {' '.join(map(str, cmd))}\n{result.stdout}{result.stderr}\n")
    return result.returncode, result.stdout + result.stderr


def paths(project):
    return [f'src/{project}']


def restore(project):
    """Restore the project's committed sources and remove files an earlier round added."""
    subprocess.run(['git', 'checkout', '--', *paths(project)], cwd=ROOT, check=True)
    added = subprocess.run(['git', 'ls-files', '--others', '--exclude-standard', '-z', *paths(project)], cwd=ROOT,
                           capture_output=True, check=True).stdout.decode().split('\0')
    for name in filter(None, added):
        os.remove(ROOT / name)


def owners(report):
    """Map every generated source line to the member responsible for it."""
    lines = {}
    for file in report['files']:
        for span in file['members']:
            owner = span['member'].get('owner') or span['member']['id']
            for line in range(span['first'], span['last'] + 1):
                lines[(file['file'], line)] = owner
    return lines


def repo_relpath(p):
    p = p.strip().replace('\\', '/')
    if not (p.startswith('/') or re.match(r'^[a-zA-Z]:', p)):
        return p
    if os.name != 'nt':
        p = re.sub(r'^[a-zA-Z]:/', lambda m: f'/mnt/{m.group(0)[0].lower()}/', p)
    return os.path.relpath(p, ROOT).replace('\\', '/')


def accept(args, work, log):
    flags = ['-c', 'Release', '-nologo', '-v:q', '-p:EnableWindowsTargeting=true', '--disable-build-servers']
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
        lines = owners(report)
        print(f'round {round}: {len(report["members"])} members emitted', flush=True)
        if not report['members']:
            break
        rejected = {}
        release = work / f'release{round}'
        code, out = run([args.dotnet, 'build', f'src/{args.project}', *flags, '--no-incremental', '-o', release], log)
        for e in ERROR.finditer(out):
            path = repo_relpath(e['path'])
            owner = lines.get((path, int(e['line'])))
            if owner is None:
                raise Failure(f'build error outside the emitted members: {e[0]}')
            rejected.setdefault(owner, f"compile: {e['code']} {e['message'][:120]}")
        if code and not rejected:
            raise Failure(f'build failed without attributable errors; see {log}')
        if not rejected:
            inventory = work / f'inventory{round}.json'
            code, _ = run([args.dotnet, work / 'MethodAudit' / 'Quire.Tools.MethodAudit.dll', '--repo', ROOT, '--assemblies',
                           release,
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
                        help='also generate owned Tier A/A2 members')
    args = parser.parse_args()
    if not (ROOT / 'src' / args.project / f'{args.project}.csproj').is_file():
        sys.exit(f'{args.project}: no production project')
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
