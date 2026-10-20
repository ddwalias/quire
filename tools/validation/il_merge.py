"""Check or land a committed branch using only the Release IL acceptance rule.

Run from the main checkout: python tools/validation/merge_gate.py BRANCH [--apply]
The candidate is built in a temporary clone. --apply holds the merge lock, checks
the current base, validates that exact committed tip, then fast-forwards main.
"""
import argparse
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import uuid

from merge_lock import held
import release_il

ROOT = Path(__file__).resolve().parents[2]


def git(*args, cwd=None):
    return release_il.git(*args, cwd=cwd or ROOT)


def gate(args):
    if git('status', '--porcelain'):
        raise RuntimeError('commit or stash changes in the main checkout first')
    base = git('rev-parse', '--verify', f'refs/heads/{args.base}^{{commit}}')
    tip = git('rev-parse', '--verify', f'refs/heads/{args.branch}^{{commit}}')
    if git('merge-base', base, tip) != base:
        raise RuntimeError(f'rebase {args.branch} onto {args.base} first')
    changed = git('diff', '--name-only', base, tip).splitlines()
    if not changed:
        raise RuntimeError('branch has no changes')
    if args.apply and git('branch', '--show-current') != args.base:
        raise RuntimeError(f'--apply must run in the {args.base} checkout')
    results = args.results or ROOT / '.mirror-cache' / f'gate-il-{uuid.uuid4().hex[:12]}'
    results.mkdir(parents=True, exist_ok=False)
    record = {'base': base, 'tip': tip, 'branch': args.branch, 'changed': changed,
              'validation_mode': 'release-il', 'passed': False, 'applied': False}
    try:
        with tempfile.TemporaryDirectory(prefix='quire-il-gate-') as temp:
            tree = Path(temp) / 'tree'
            git('clone', '--quiet', '--shared', '--no-checkout', ROOT, tree)
            git('-c', 'core.hooksPath=', 'checkout', '--quiet', '--detach', tip, cwd=tree)
            projects = release_il.affected_projects(base, tree)
            # An explicit project adds scope; it never removes an affected project.
            projects = sorted(set(projects) | set(args.project))
            unknown = set(projects) - release_il.production_projects(tree)
            if unknown:
                raise RuntimeError(f'unknown or removed production projects: {sorted(unknown)}')
            if projects:
                report = release_il.check(base, projects, results / 'run', args.type, args.method,
                                          args.dotnet, args.timeout, root=tree)
            elif args.type or args.method:
                raise RuntimeError('member selection requires --project')
            else:
                report = {'passed': True, 'checked': [], 'reason': 'no production project changed'}
            record['validation'] = report
            record['passed'] = report['passed']
            if not report['passed']:
                for failure in report.get('failures', [])[:20]:
                    print(f"  {failure['method']}: {failure['reason']}", flush=True)
                if len(report.get('failures', [])) > 20:
                    print(f"  {len(report['failures']) - 20} more mismatches in the report")
                if report.get('error'):
                    print(report['error'], file=sys.stderr)
                return 1
            if args.apply:
                if (git('rev-parse', f'refs/heads/{args.base}') != base
                        or git('rev-parse', f'refs/heads/{args.branch}') != tip):
                    raise RuntimeError('base or candidate moved during validation; retry')
                if git('status', '--porcelain'):
                    raise RuntimeError('main checkout changed during validation; retry')
                git('-c', 'core.hooksPath=', 'merge', '--ff-only', tip)
                record['applied'] = True
            return 0
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
        record['passed'] = False
        record['error'] = str(error)
        raise
    finally:
        (results / 'gate.json').write_text(json.dumps(record, indent=2) + '\n', encoding='utf-8')
        print(f"Release IL gate: {'PASS' if record['passed'] else 'FAIL'}; {results / 'gate.json'}", flush=True)


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('branch')
    parser.add_argument('--base', default='main')
    parser.add_argument('--apply', action='store_true')
    parser.add_argument('--project', action='append', default=[])
    parser.add_argument('--type', action='append', default=[])
    parser.add_argument('--method', action='append', default=[])
    parser.add_argument('--results', type=Path)
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--timeout', type=int, default=1800)
    args = parser.parse_args(argv)
    try:
        if args.apply:
            with held():
                return gate(args)
        return gate(args)
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
        print(f'BLOCKED: {error}', file=sys.stderr)
        return 1


if __name__ == '__main__':
    sys.exit(main())
