"""Inspect exact Release IL differences against Mathcad. Never reads a Debug build.

    python tools/validation/il_diff.py Quire.Mcd.Dim::Div [--project Quire.Mpl]

KEY is a type, Type::Name (all overloads), or Type::signature. Original IL is
printed to the console only. Use release_il.py for an enforcing pass/fail check.
"""
import argparse
from pathlib import Path
import subprocess
import sys
import tempfile

from release_il import ROOT, mathcad_install, run


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('key', nargs='+')
    parser.add_argument('--project', action='append', default=[])
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--timeout', type=int, default=1800)
    args = parser.parse_args(argv)
    projects = args.project or ['Quire.Mpl']
    install = str(mathcad_install())
    try:
        with tempfile.TemporaryDirectory(prefix='quire-il-diff-') as temp:
            work = Path(temp)
            log = work / 'build.log'
            run([args.dotnet, 'build', ROOT / 'tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj',
                 '-c', 'Release', '-o', work / 'tool', '-nologo', '-v:q'], ROOT, log, args.timeout)
            for project in projects:
                run([args.dotnet, 'build', ROOT / 'src' / project / f'{project}.csproj',
                     '-c', 'Release', '--no-incremental', '-o', work / 'assemblies', '-nologo', '-v:q'], ROOT, log, args.timeout)
            return subprocess.run([args.dotnet, str(work / 'tool/Quire.Tools.MethodAudit.dll'),
                '--repo', str(ROOT), '--assemblies', str(work / 'assemblies'), '--install', install, '--release-il',
                *(a for p in projects for a in ('--project', p)),
                *(a for key in args.key for a in ('--diff', key))], cwd=ROOT, timeout=args.timeout).returncode
    except (OSError, RuntimeError, subprocess.SubprocessError) as error:
        print(error, file=sys.stderr)
        return 2


if __name__ == '__main__':
    sys.exit(main())
