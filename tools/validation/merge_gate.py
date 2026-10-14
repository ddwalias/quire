"""Merge one work branch into main only when it passes validation with main's own harness.

Run from the checkout that has the base branch checked out:
    python3 tools/validation/merge_gate.py mirror/Quire.Mcd.NumVal            # check only
    python3 tools/validation/merge_gate.py mirror/Quire.Mcd.NumVal --apply    # check, then fast-forward main
"""
import argparse
import fnmatch
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import ratchet

ROOT = Path(__file__).resolve().parents[2]


def git(*args, cwd=ROOT, check=True):
    result = subprocess.run(['git', *args], cwd=cwd, text=True, capture_output=True)
    if check and result.returncode:
        raise RuntimeError(f"git {' '.join(args)}: {result.stderr.strip()}")
    return result.stdout.strip()


def patterns(text):
    return [line.strip() for line in text.splitlines() if line.strip() and not line.startswith('#')]


def is_protected(path, rules):
    """Last matching rule wins; `!pattern` re-allows. Case-insensitive, so `NuGet.Config` cannot slip through."""
    result = False
    for rule in rules:
        negate = rule.startswith('!')
        if fnmatch.fnmatchcase(path.lower(), rule.lstrip('!').lower()):
            result = not negate
    return result


def protected(paths, rules):
    return sorted(p for p in paths if is_protected(p, rules))


def case_collisions(paths):
    """Paths, or their directories, that differ only in case."""
    seen = {}
    for path in paths:
        parts = path.split('/')
        for depth in range(1, len(parts) + 1):
            prefix = '/'.join(parts[:depth])
            seen.setdefault(prefix.lower(), set()).add(prefix)
    return sorted(' / '.join(sorted(names)) for names in seen.values() if len(names) > 1)


def unvalidated_paths(paths):
    """Source and test paths the focused profile neither builds nor runs."""
    focused = ('src/quire.mpl/', 'tests/quire.mpl.tests/', 'tests/quire.differential.tests/',
               'tests/quire.harness.tests/', 'tests/quire.focused.differential.tests/')
    return sorted(p for p in paths if p.lower().startswith(('src/', 'tests/')) and not p.lower().startswith(focused))


def changed_paths(base, tip):
    """Every added, changed, deleted or renamed path, both sides of a rename, unquoted."""
    return sorted({p for p in git('diff', '-z', '--no-renames', '--name-only', f'{base}...{tip}').split('\0') if p})


def on_branch(commit, repo=ROOT):
    """Whether a local branch contains `commit`: the validation clone (`--no-local`) copies only those."""
    return bool(git('for-each-ref', '--contains', commit, '--format=%(refname)', 'refs/heads', cwd=repo))


def baseline_regressions(base, baseline, repo=ROOT):
    """Reasons the validator's new baseline (its bytes, read once) may not become main's. It is written after the last
    test run, but a process code under test left running could still rewrite it, so it must parse, keep the schema
    and hold the ratchet against the base's committed baseline."""
    try:
        old = json.loads(git('show', f'{base}:docs/mirror/validation-baseline.json', cwd=repo))
        new = json.loads(baseline)
        if new.get('schema') != ratchet.SCHEMA:
            return [f"new baseline schema {new.get('schema')!r} is not {ratchet.SCHEMA}"]
        return [f'new baseline: {r}' for r in ratchet.compare(old, new)]
    except (OSError, ValueError, RuntimeError, KeyError, TypeError, AttributeError) as error:
        return [f'new baseline unusable: {error!r}']


def clone(tip, tree, repo=ROOT):
    """A separate clone at `tip`, not a worktree: code under test never sees this repository's .git, hooks or config,
    nor, without the origin remote and the reflogs (`clone: from <path>`), its path."""
    git('clone', '-q', '--no-local', '--no-checkout', str(repo), str(tree), cwd=repo)
    git('remote', 'remove', 'origin', cwd=tree)
    shutil.rmtree(Path(tree) / '.git/logs')
    git('-c', 'advice.detachedHead=false', 'checkout', '-q', tip, cwd=tree)


def verdict(summary):
    """Reasons the validation summary blocks a merge; empty when it may merge."""
    reasons = []
    if summary.get('error'):
        reasons.append(f"validation error: {summary['error']}")
    for project in summary.get('projects', []):
        if project.get('exit_code') or project.get('failed') or project.get('skipped') or not project.get('passed'):
            reasons.append(f"{project['project']}: tests did not all pass")
    ratchet = summary.get('ratchet')
    if not ratchet:
        reasons.append('ratchet did not run')
    elif not ratchet['ok']:
        reasons += [f'regression: {r}' for r in ratchet['regressions']]
    if summary.get('mutation', {}).get('survivors'):
        reasons += [f'surviving mutant: {s}' for s in summary['mutation']['survivors']]
    if not reasons and not summary.get('gate_passed'):
        reasons.append('validator did not report gate_passed')
    return reasons


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('branch')
    parser.add_argument('--base', default='main')
    parser.add_argument('--apply', action='store_true', help='fast-forward the base branch and commit the new baseline')
    parser.add_argument('--allow-protected', action='store_true', help='maintainer only: accept protected-path changes')
    parser.add_argument('--allow-unvalidated', action='store_true',
                        help='maintainer only: accept src changes outside Quire.Mpl after a separate phase run')
    parser.add_argument('--results', type=Path, help='new directory for the validation results')
    parser.add_argument('--timeout', type=int, default=1800)
    args = parser.parse_args()
    if args.apply and git('rev-parse', '--abbrev-ref', 'HEAD') != args.base:
        parser.error(f'--apply must run from the checkout that has {args.base} checked out')
    base, tip = git('rev-parse', args.base), git('rev-parse', args.branch)
    if not on_branch(tip):
        print(f'BLOCKED: {args.branch} ({tip[:12]}) is not on a local branch; the validation clone copies only those.')
        return 1
    if git('merge-base', base, tip) != base:
        print(f'BLOCKED: {args.branch} is not based on the current {args.base}; rebase it first.')
        return 1
    changed = changed_paths(base, tip)
    if not changed:
        print(f'BLOCKED: {args.branch} has no changes.')
        return 1
    rules = patterns(git('show', f'{base}:tools/validation/protected.txt'))
    touched = protected(changed, rules)
    if touched and not args.allow_protected:
        print('BLOCKED: maintainer-owned paths changed:\n  ' + '\n  '.join(touched))
        return 1
    collisions = case_collisions(git('ls-tree', '-r', '--name-only', tip).splitlines())
    if collisions:
        print('BLOCKED: paths differ only in case and would merge into one file on Windows:\n  ' + '\n  '.join(collisions))
        return 1
    unvalidated = unvalidated_paths(changed)
    if unvalidated and not args.allow_unvalidated:
        print('BLOCKED: the gate validates only Quire.Mpl and its tests (focused profile); these need the Windows '
              'phase profile and a maintainer merge:\n  ' + '\n  '.join(unvalidated))
        return 1

    if args.results and args.results.exists():
        parser.error('--results must name a new directory')
    tree = Path(tempfile.mkdtemp(prefix='quire-gate-'))
    outputs = (args.results or Path(tempfile.mkdtemp(prefix='quire-gate-results-'))).resolve()
    outputs.mkdir(parents=True, exist_ok=True)
    results = outputs / 'run'
    try:
        try:
            clone(tip, tree, ROOT)
            if not args.allow_protected:
                # Validate with main's harness, whatever the branch contains.
                base_files = git('ls-tree', '-r', '--name-only', base).splitlines()
                restore = protected(base_files, rules)
                if restore:
                    git('checkout', base, '--', *restore, cwd=tree)
        except (RuntimeError, OSError) as error:
            code, reasons = None, [f'cannot prepare the validation clone: {error}']
        else:
            command = [sys.executable, '-I', '-B', str(tree / 'tools/validation/validate.py'), '--results', str(results),
                       '--mutate-since', base, '--write-baseline', str(outputs / 'baseline.json'),
                       '--timeout', str(args.timeout)]
            print('Validating:', ' '.join(command), flush=True)
            code = subprocess.run(command, cwd=tree).returncode
            summary_path = results / 'summary.json'
            summary = json.loads(summary_path.read_text()) if summary_path.is_file() else {'error': 'no summary written'}
            reasons = verdict(summary)
            # 0: complete; 3: gate passed with known gaps. Anything else (crash, kill) blocks whatever the summary says.
            if code not in (0, 3):
                reasons.insert(0, f'validator exited {code}')
            if not reasons:
                # Checked and committed from these bytes: a lingering process may rewrite the file in between.
                try:
                    baseline = (outputs / 'baseline.json').read_bytes()
                except OSError as error:
                    reasons = [f'new baseline unusable: {error!r}']
                else:
                    reasons = baseline_regressions(base, baseline)
        record = {'branch': args.branch, 'base': base, 'tip': tip, 'changed': changed, 'protected_changed': touched,
                  'results': str(results), 'validator_exit': code, 'reasons': reasons}
        (outputs / 'gate.json').write_text(json.dumps(record, indent=2) + '\n')
        if reasons:
            print('BLOCKED:\n  ' + '\n  '.join(reasons) + f'\nResults: {results}')
            return 1
        print(f'PASSED: {args.branch} ({tip[:12]}); results {results}')
        if args.apply:
            merged = baseline_commit(tip, baseline, args.branch, results)
            git('-c', 'core.hooksPath=/dev/null', 'merge', '-q', '--ff-only', merged)
            print(f'MERGED: {args.base} -> {merged[:12]}')
        return 0
    finally:
        shutil.rmtree(tree, ignore_errors=True)


def baseline_commit(tip, baseline, branch, results, repo=ROOT):
    """The tip's committed tree plus the new baseline (the bytes checked), built from objects rather than any working
    tree, so nothing the validated code did to its checkout can reach the merge."""
    with tempfile.TemporaryDirectory() as scratch:
        env = dict(os.environ, GIT_INDEX_FILE=str(Path(scratch) / 'index'))
        def run(*command, data=None):
            return subprocess.run(['git', '-c', 'core.hooksPath=/dev/null', *command], cwd=repo, env=env, check=True,
                                  capture_output=True, input=data).stdout.decode().strip()
        run('read-tree', tip)
        blob = run('hash-object', '-w', '--stdin', data=baseline)
        run('update-index', '--add', '--cacheinfo', f'100644,{blob},docs/mirror/validation-baseline.json')
        tree = run('write-tree')
        if tree == run('rev-parse', f'{tip}^{{tree}}'):
            return tip
        commit = run('commit-tree', tree, '-p', tip, '-m', f'Update validation baseline after merging {branch}',
                     '-m', f'Merge gate results: {results}')
    changed = git('diff', '--name-only', tip, commit, cwd=repo).splitlines()
    if changed != ['docs/mirror/validation-baseline.json']:
        raise RuntimeError(f'baseline commit changed {changed}')
    return commit

if __name__ == '__main__':
    sys.exit(main())
