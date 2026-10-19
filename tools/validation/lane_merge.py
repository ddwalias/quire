"""Land a lane branch through the merge gate, on Windows or Linux (validation-flow.md, "Merge gate throughput"):

    python tools/validation/lane_merge.py <branch> [--main DIR] [--jobs N]

Run it in the worktree that has <branch> checked out. It fast-forwards main to origin/main briefly under the merge
lock (merge_lock.py), rebases the branch onto it here, validates without the lock (--validate-only), then under the
lock pulls again, applies the gate's record (--apply-validated) and pushes main. A stale record (exit 4) is
validated again; after two stale results in a row, the one-shot --apply holds the lock for the whole validation.
Exits with the gate's code: 0 merged and pushed, 1 blocked (or a failed rebase), otherwise the failing step's.
The gate runs in --main, by default the worktree that has main checked out. The push follows only a merge."""
import argparse
import fnmatch
import subprocess
import sys
from pathlib import Path
from merge_lock import held


def git(*args, cwd=None):
    return subprocess.run(['git', *args], cwd=cwd, capture_output=True, text=True)


def main_worktree(cwd=None):
    """The worktree that has `main` checked out, or None."""
    worktree = None
    for line in git('worktree', 'list', '--porcelain', cwd=cwd).stdout.splitlines():
        if line.startswith('worktree '):
            worktree = line[len('worktree '):]
        elif line == 'branch refs/heads/main':
            return worktree
    return None


def pull(main):
    """Fast-forward main to origin/main; the git exit code."""
    return (subprocess.run(['git', 'fetch', '-q', 'origin'], cwd=main).returncode
            or subprocess.run(['git', 'merge', '-q', '--ff-only', 'origin/main'], cwd=main).returncode)


def push(main):
    return subprocess.run(['git', 'push', '-q', 'origin', 'main'], cwd=main).returncode


def protected_rules(main):
    result = git('show', 'main:tools/validation/protected.txt', cwd=main)
    if result.returncode:
        raise RuntimeError(f"cannot read main's protected-path rules: {result.stderr.strip()}")
    return [line.strip() for line in result.stdout.splitlines()
            if line.strip() and not line.lstrip().startswith('#')]


def is_protected(path, rules):
    protected = False
    for rule in rules:
        excluded = rule.startswith('!')
        pattern = rule[1:] if excluded else rule
        if fnmatch.fnmatchcase(path.casefold(), pattern.casefold()):
            protected = not excluded
    return protected


def allow_only_unreachable_protected_change(paths, rules):
    owned = {path for path in paths if is_protected(path, rules)}
    return owned == {'docs/mirror/unreachable-lines.txt'}


def allow_unreachable_update(branch, main):
    """Allow one lane-owned ledger update; merge_gate still validates its hashes, freshness and coverage."""
    changed = git('diff', '--name-only', f'main...{branch}', cwd=main)
    if changed.returncode:
        raise RuntimeError(f"cannot inspect {branch}'s changes: {changed.stderr.strip()}")
    return allow_only_unreachable_protected_change(changed.stdout.splitlines(), protected_rules(main))


def rebase(branch, cwd=None):
    result = git('rebase', '-q', 'main', branch, cwd=cwd)
    if result.returncode:
        print(f'git rebase main {branch} failed: {(result.stdout + result.stderr).strip()}', file=sys.stderr)
        return 1
    return 0


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('branch')
    parser.add_argument('--main', help='the worktree that has main checked out, where the gate runs (default: found)')
    parser.add_argument('--jobs', type=int, default=14, help='validation workers (default 14)')
    args = parser.parse_args(argv)
    main_dir = args.main or main_worktree()
    if not main_dir:
        print('no worktree has main checked out; pass --main', file=sys.stderr)
        return 1
    gate = [sys.executable, str(Path(main_dir) / 'tools/validation/merge_gate.py'), args.branch]
    def run(*extra):
        return subprocess.run([*gate, *extra], cwd=main_dir).returncode
    jobs = ['--jobs', str(args.jobs)]
    for attempt in range(2):
        with held():
            code = pull(main_dir)                                       # fresh main, briefly under the lock
        if code or rebase(args.branch):
            return code or 1
        gate_options = ['--allow-protected'] if allow_unreachable_update(args.branch, main_dir) else []
        if gate_options:
            print('Allowing only docs/mirror/unreachable-lines.txt through maintainer protection; validating entries.')
        code = run('--validate-only', *gate_options, *jobs)             # BLOCKED: fix the branch
        if code:
            return code
        with held():
            code = pull(main_dir) or run('--apply-validated') or push(main_dir)
        if code != 4:
            return code                                                 # merged and pushed, blocked, or push failed
    # Two stale results in a row: the one-shot gate, holding the lock for the whole validation.
    with held():
        code = pull(main_dir) or rebase(args.branch)
        if code:
            return code
        gate_options = ['--allow-protected'] if allow_unreachable_update(args.branch, main_dir) else []
        if gate_options:
            print('Allowing only docs/mirror/unreachable-lines.txt through maintainer protection; validating entries.')
        return run('--apply', *gate_options, *jobs) or push(main_dir)


if __name__ == '__main__':
    sys.exit(main())
