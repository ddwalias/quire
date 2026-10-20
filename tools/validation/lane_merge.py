"""Rebase, check exact Release IL, fast-forward main and push it.

    python tools/validation/lane_merge.py mirror/<lane>-<unit> [--main DIR]

Run in the branch's clean worktree. One merge lock covers refresh, rebase,
Release IL validation and push, so the validated candidate cannot become stale.
"""
import argparse
from pathlib import Path
import subprocess
import sys

from merge_lock import held
import il_merge


def git(*args, cwd=None):
    return subprocess.run(['git', *args], cwd=cwd, capture_output=True, text=True, check=True).stdout.strip()


def main_worktree(cwd=None):
    directory = None
    for line in git('worktree', 'list', '--porcelain', cwd=cwd).splitlines():
        if line.startswith('worktree '):
            directory = line[len('worktree '):]
        elif line == 'branch refs/heads/main':
            return directory
    return None


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('branch')
    parser.add_argument('--main')
    parser.add_argument('--project', action='append', default=[])
    parser.add_argument('--type', action='append', default=[])
    parser.add_argument('--method', action='append', default=[])
    args = parser.parse_args(argv)
    try:
        main_dir = args.main or main_worktree()
        if not main_dir:
            raise RuntimeError('no main worktree found; pass --main DIR')
        if git('branch', '--show-current') != args.branch:
            raise RuntimeError('run from the candidate branch worktree')
        if git('status', '--porcelain') or git('status', '--porcelain', cwd=main_dir):
            raise RuntimeError('commit or stash changes in both worktrees first')
        if git('branch', '--show-current', cwd=main_dir) != 'main':
            raise RuntimeError('--main must be the main checkout')
        with held():
            git('fetch', '-q', 'origin', cwd=main_dir)
            git('merge', '-q', '--ff-only', 'origin/main', cwd=main_dir)
            git('rebase', 'main', args.branch)
            il_merge.ROOT = Path(main_dir)
            args.base, args.apply, args.results = 'main', True, None
            args.dotnet, args.timeout = 'dotnet', 1800
            code = il_merge.gate(args)
            if code:
                return code
            git('push', 'origin', 'main', cwd=main_dir)
        return 0
    except (OSError, RuntimeError, subprocess.SubprocessError) as error:
        detail = getattr(error, 'stderr', None) or str(error)
        print(f'BLOCKED: {detail}', file=sys.stderr)
        return 1


if __name__ == '__main__':
    sys.exit(main())
