"""Merge one work branch into main only when it passes validation with main's own harness.

Run from the checkout that has the base branch checked out:
    python3 tools/validation/merge_gate.py mirror/Quire.Mcd.NumVal            # check only
    python3 tools/validation/merge_gate.py mirror/Quire.Mcd.NumVal --apply    # check, then fast-forward main
"""
import argparse
import fnmatch
import json
import os
import re
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import ratchet
from validate import mutation_jobs
from validate import SCRATCH_PREFIX, SCRATCH_RECORD
from validate import is_wsl, windows_dotnet, windows_temp

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


STUB = b'[global::Quire.Mirror.MirrorStub]'
# Quire.Mpl only: the focused profile validates nothing else, and other projects' stubs come in shapes (attributes
# above the stub, trailing comments) that line matching cannot judge.
GENERATED = 'src/quire.mpl/generated/mirror/*.g.cs'
TYPE = re.compile(rb'\b(class|struct|interface|enum|record|delegate|event)\b')
# A constant has nothing to implement, and the shape omits its value.
CONST = re.compile(rb'\bconst\b')


def stub_deletions(base, tip, path, repo=ROOT):
    """The member lines `tip` deleted from a generated mirror file, when that is its only change: whole stub members,
    each a `[MirrorStub]` line and the one-line member under it, compared byte for byte apart from a line's trailing
    CR. Implementing a member means deleting its stub; validation then requires the member back with the same shape
    (the manifest check). None for anything else: headers, `MirrorOf`, type declarations, constants, stubs under
    another attribute, edits, additions, deleting the file, or a change that deletes nothing."""
    if not fnmatch.fnmatchcase(path.lower(), GENERATED):
        return None
    blobs = []
    for commit in (base, tip):
        result = subprocess.run(['git', 'show', f'{commit}:{path}'], cwd=repo, capture_output=True)
        if result.returncode:
            return None  # added or deleted
        # Only \n ends a line here: \f, \v or U+2028 do not end a C# comment, so they must not look like line breaks.
        blobs.append([line.removesuffix(b'\r') for line in result.stdout.split(b'\n')])
    old, new = stub_units(blobs[0]), stub_units(blobs[1])
    kept, deleted = 0, []
    for unit in old:
        if kept < len(new) and unit == new[kept]:
            kept += 1
        elif len(unit) == 2:
            deleted.append(unit[1].strip().decode('utf-8', 'replace'))
        else:
            return None  # only whole stub members may go
    return deleted if kept == len(new) and deleted else None


def stub_units(lines):
    """Lines, with each deletable stub member (its `[MirrorStub]` line and the member under it) as one unit.
    Identical units are interchangeable, and other lines cannot equal a stub unit, so matching in order is exact."""
    units, i = [], 0
    while i < len(lines):
        member = lines[i + 1].strip() if i + 1 < len(lines) else b''
        code = re.split(rb'[="]', member, maxsplit=1)[0]
        if (lines[i].strip() == STUB and (i == 0 or not lines[i - 1].strip().startswith(b'['))
                and member and not member.startswith((b'[', b'{', b'}', b'//', b'#')) and b'//' not in member
                and b'MirrorOf' not in member and not TYPE.search(code) and not CONST.search(code)):
            units.append((lines[i], lines[i + 1]))
            i += 2
        else:
            units.append((lines[i],))
            i += 1
    return units


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


PROSE_PATHS = frozenset({
    'README.md', 'docs/mirror/WORK.md', 'docs/mirror/STATE.md', 'docs/mirror/history.md',
    'docs/mirror/work/lane-a.md', 'docs/mirror/work/lane-b.md', 'docs/mirror/work/validation-flow.md',
})


def prose_only(base, tip, paths):
    """Only edits to existing, ordinary UTF-8 documents on the explicit allowlist can reuse validation."""
    if not paths or not set(paths) <= PROSE_PATHS:
        return False
    for revision in (base, tip):
        for path in paths:
            if git('ls-tree', revision, '--', path).split()[:2] != ['100644', 'blob']:
                return False
            try:
                blob = subprocess.run(['git', 'show', f'{revision}:{path}'], cwd=ROOT,
                                      capture_output=True, check=True).stdout
                if b'\0' in blob:
                    return False
                blob.decode('utf-8')
            except (UnicodeError, OSError, subprocess.CalledProcessError):
                return False
    return True


def on_branch(commit, repo=ROOT):
    """Whether a local branch contains `commit`: the validation clone (`--no-local`) copies only those."""
    return bool(git('for-each-ref', '--contains', commit, '--format=%(refname)', 'refs/heads', cwd=repo))


def baseline_regressions(base, baseline, repo=ROOT, path='docs/mirror/validation-baseline.json'):
    """Reasons the validator's new baseline (its bytes, read once) may not become main's. It is written after the last
    test run, but a process code under test left running could still rewrite it, so it must parse, keep the schema
    and hold the ratchet against the base's committed baseline at `path` (PHASE_BASELINE for a phase run)."""
    try:
        old = json.loads(git('show', f'{base}:{path}', cwd=repo))
        new = json.loads(baseline)
        if new.get('schema') not in ratchet.READABLE:
            return [f"new baseline schema {new.get('schema')!r} is not one of {ratchet.READABLE}"]
        return [f'new baseline: {r}' for r in ratchet.compare(old, new)]
    except (OSError, ValueError, RuntimeError, KeyError, TypeError, AttributeError) as error:
        return [f'new baseline unusable: {error!r}']


BASELINE = 'docs/mirror/validation-baseline.json'
# The phase profile's baseline (ratchet.PHASE_BASELINE), written and restored like BASELINE when the phase runs.
PHASE_BASELINE = 'docs/mirror/validation-baseline-phase.json'


def restored_paths(base_files, rules, stubs, allow_protected):
    """Protected paths the validation clone takes from the base: all of them (main's harness, whatever the branch
    contains) unless a maintainer allows protected changes, and the ratchet baseline always: the gate writes it, a
    branch never legitimately edits it, and it decides what the ratchet and mutation selection compare against."""
    restore = [] if allow_protected else [p for p in protected(base_files, rules) if p not in stubs]
    for baseline in (BASELINE, PHASE_BASELINE):
        if baseline in base_files and baseline not in restore:
            restore.append(baseline)
    return restore


def validation_command(tree, results, base, baseline, timeout, types_wide=False, everything=False, jobs=None):
    """The clone's validator in gate mode: only accepted-mutants.txt excuses a survivor and only unreachable-lines.txt
    exempts a line (no --proposed-mutants or --proposed-unreachable), and the local mutant cache, which code under
    test could forge, is neither read nor written. `types_wide`: mutate whole changed types; `everything`: every compared
    method (maintainer audits)."""
    return [sys.executable, '-I', '-B', str(tree / 'tools/validation/validate.py'), '--results', str(results),
            '--mutate-since', base, '--write-baseline', str(baseline), '--timeout', str(timeout), '--no-mutant-cache',
            '--jobs', str(mutation_jobs(jobs)),
            *(['--mutate-types-wide'] if types_wide else []), *(['--mutate-all'] if everything else [])]


def phase_dotnet(environ=os.environ):
    """The CLI that runs the phase profile here: `dotnet` on Windows, Windows' dotnet.exe from WSL
    (validate.windows_dotnet); None elsewhere, or when it is not installed."""
    if os.name == 'nt':
        return 'dotnet'
    found = windows_dotnet(environ=environ) if is_wsl(environ) else None
    return found if found and Path(found).is_file() else None


def phase_command(tree, results, baseline, timeout, dotnet):
    """The clone's validator with the phase profile: every solution test project, every manifest project's methods,
    the phase baseline restored from the base, and no proposals. The phase profile runs no mutation."""
    return [sys.executable, '-I', '-B', str(tree / 'tools/validation/validate.py'), '--profile', 'phase',
            '--results', str(results), '--baseline', str(tree / PHASE_BASELINE), '--write-baseline', str(baseline),
            '--timeout', str(timeout), '--dotnet', dotnet]


def phase_temp():
    """The directory phase runs work in: the system's temporary one on Windows; from WSL validate.windows_temp, which
    refuses one off a Windows drive (ValueError)."""
    return Path(tempfile.gettempdir() if os.name == 'nt' else windows_temp())


# Phase run directories (a clone's results, some 300 MB each) kept in the phase temporary directory, newest first; the
# gate removes older ones before each phase run, and gate.json names the directory of its own.
PHASE_RUNS_KEPT = 3


def remove_tree(directory):
    """Remove `directory`, reporting what could not be removed (on Windows, files a lingering process holds open)."""
    failed = []
    shutil.rmtree(directory, onexc=lambda function, path, error: failed.append(f'{path}: {error}'))
    for failure in failed[:10]:
        print(f'could not remove {failure}', flush=True)
    return not failed


def prune_phase_runs(temp, keep=PHASE_RUNS_KEPT):
    """Remove all but the newest `keep - 1` phase run directories under `temp`, making room for one more."""
    runs = sorted((d for d in Path(temp).glob('quire-gate-phase-*') if d.is_dir() and not d.is_symlink()),
                  key=lambda d: d.stat().st_mtime, reverse=True)
    for directory in runs[max(0, keep - 1):]:
        remove_tree(directory)


def phase_validation(tip, base, restore, dotnet, timeout, temp, repo=ROOT):
    """(validator exit, reasons, results, new phase baseline bytes) of the phase profile on `tip`, for a branch that
    changes projects the focused profile does not validate. Like the focused run it validates a separate clone with
    the same restored paths, but in `temp` (phase_temp), where Windows' dotnet.exe can build and test it from WSL.
    Its results stay there (see PHASE_RUNS_KEPT); the clone is removed."""
    prune_phase_runs(temp)
    work = Path(tempfile.mkdtemp(prefix='quire-gate-phase-', dir=temp))
    tree, results, baseline = work / 'tree', work / 'run', work / 'baseline-phase.json'
    try:
        try:
            clone(tip, tree, repo)
            if restore:
                git('checkout', base, '--', *restore, cwd=tree)
        except (RuntimeError, OSError) as error:
            return None, [f'phase: cannot prepare the validation clone: {error}'], results, None
        command = phase_command(tree, results, baseline, timeout, dotnet)
        print('Validating (phase):', ' '.join(command), flush=True)
        code = subprocess.run(command, cwd=tree, env=dict(os.environ, QUIRE_WINDOWS_TEMP=str(temp))).returncode
        summary_path = results / 'summary.json'
        summary = json.loads(summary_path.read_text()) if summary_path.is_file() else {'error': 'no summary written'}
        reasons = verdict(summary)
        if code not in (0, 3):
            reasons.insert(0, f'validator exited {code}')
        data = None
        if not reasons:
            try:
                data = baseline.read_bytes()
            except OSError as error:
                reasons = [f'new baseline unusable: {error!r}']
            else:
                reasons = baseline_regressions(base, data, repo, PHASE_BASELINE)
        return code, [f'phase: {r}' for r in reasons], results, None if reasons else data
    finally:
        if tree.exists():
            remove_tree(tree)


def clone(tip, tree, repo=ROOT):
    """A separate clone at `tip`, not a worktree: code under test never sees this repository's .git, hooks or config,
    nor, without the origin remote and the reflogs (`clone: from <path>`), its path."""
    git('clone', '-q', '--no-local', '--no-checkout', str(repo), str(tree), cwd=repo)
    git('remote', 'remove', 'origin', cwd=tree)
    shutil.rmtree(Path(tree) / '.git/logs', ignore_errors=True)  # absent under core.logAllRefUpdates=false
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
    if summary.get('proposed_mutants'):
        reasons.append('validation used --proposed-mutants; only accepted-mutants.txt may excuse a survivor')
    if summary.get('proposed_unreachable'):
        reasons.append('validation used --proposed-unreachable; only unreachable-lines.txt may exempt a line')
    reasons += [f'listed unreachable line executed: {e}' for e in summary.get('executed_unreachable', [])]
    if summary.get('mutation', {}).get('survivors'):
        reasons += [f'surviving mutant: {s}' for s in summary['mutation']['survivors']]
    if not reasons and not summary.get('gate_passed'):
        reasons.append('validator did not report gate_passed')
    return reasons


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('branch')
    parser.add_argument('--base', default='main')
    parser.add_argument('--apply', action='store_true',
                        help='fast-forward the base branch; update the baseline after runtime validation')
    parser.add_argument('--allow-protected', action='store_true', help='maintainer only: accept protected-path changes')
    parser.add_argument('--allow-unvalidated', action='store_true',
                        help='maintainer only: accept src changes outside Quire.Mpl after a separate phase run')
    parser.add_argument('--results', type=Path, help='new directory for the validation results')
    parser.add_argument('--timeout', type=int, default=1800)
    parser.add_argument('--jobs', type=mutation_jobs, default=mutation_jobs(),
                        help='parallel mutant test runs (default: CPU count, at most 12)')
    parser.add_argument('--mutate-types-wide', action='store_true',
                        help='maintainer audits: mutate every compared method of each changed type, not only changed methods')
    parser.add_argument('--mutate-all', action='store_true', help='maintainer audits: mutate every compared method')
    parser.add_argument('--full-validation', action='store_true',
                        help='run runtime validation even for allowlisted prose edits')
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
    stubs = {}
    for path in (owned := protected(changed, rules)):
        if (members := stub_deletions(base, tip, path)) is not None:
            stubs[path] = members
    touched = [p for p in owned if p not in stubs]
    if touched and not args.allow_protected:
        print('BLOCKED: maintainer-owned paths changed:\n  ' + '\n  '.join(touched))
        return 1
    collisions = case_collisions(git('ls-tree', '-r', '--name-only', tip).splitlines())
    if collisions:
        print('BLOCKED: paths differ only in case and would merge into one file on Windows:\n  ' + '\n  '.join(collisions))
        return 1
    unvalidated = unvalidated_paths(changed)
    # Where Windows' dotnet.exe is at hand, the phase profile validates those paths after the focused run.
    phase = phase_dotnet() if unvalidated and not args.allow_unvalidated else None
    if unvalidated and not args.allow_unvalidated and not phase:
        print('BLOCKED: the gate validates only Quire.Mpl and its tests (focused profile); these need the Windows '
              'phase profile and a maintainer merge:\n  ' + '\n  '.join(unvalidated))
        return 1
    if phase:
        # Checked before any run: without a phase baseline or a usable directory to run in, the phase run could only
        # fail, after the focused run had held the merge lock.
        if PHASE_BASELINE not in git('ls-tree', '--name-only', base, '--', PHASE_BASELINE).splitlines():
            print(f'BLOCKED: {args.branch} changes projects only the phase profile validates, and {args.base} has no '
                  f'{PHASE_BASELINE}; a maintainer must create the phase baseline first.')
            return 1
        try:
            temp = phase_temp()
        except (ValueError, OSError) as error:
            print(f'BLOCKED: phase: {error}')
            return 1

    if args.results and args.results.exists():
        parser.error('--results must name a new directory')
    outputs = (args.results or Path(tempfile.mkdtemp(prefix='quire-gate-results-'))).resolve()
    outputs.mkdir(parents=True, exist_ok=True)
    if (not touched and not (args.full_validation or args.mutate_all or args.mutate_types_wide)
            and prose_only(base, tip, changed)):
        record = {'branch': args.branch, 'base': base, 'tip': tip, 'changed': changed,
                  'protected_changed': touched, 'stub_deletions': stubs, 'validation_mode': 'prose',
                  'baseline_reused_from': base, 'results': None, 'validator_exit': None, 'reasons': []}
        (outputs / 'gate.json').write_text(json.dumps(record, indent=2) + '\n')
        print(f'PASSED: {args.branch} ({tip[:12]}); allowlisted prose edits only; '
              f'runtime validation skipped, baseline unchanged. Results: {outputs}')
        if args.apply:
            git('-c', 'core.hooksPath=/dev/null', 'merge', '-q', '--ff-only', tip)
            print(f'MERGED: {args.base} -> {tip[:12]}')
        return 0
    tree = Path(tempfile.mkdtemp(prefix='quire-gate-'))
    results = outputs / 'run'
    restore = []
    try:
        try:
            clone(tip, tree, ROOT)
            restore = restored_paths(git('ls-tree', '-r', '--name-only', base).splitlines(), rules, stubs,
                                     args.allow_protected)
            if restore:
                git('checkout', base, '--', *restore, cwd=tree)
        except (RuntimeError, OSError) as error:
            code, reasons, mutation = None, [f'cannot prepare the validation clone: {error}'], {}
        else:
            command = validation_command(tree, results, base, outputs / 'baseline.json', args.timeout,
                                         args.mutate_types_wide, args.mutate_all, args.jobs)
            print('Validating:', ' '.join(command), flush=True)
            code = subprocess.run(command, cwd=tree).returncode
            summary_path = results / 'summary.json'
            summary = json.loads(summary_path.read_text()) if summary_path.is_file() else {'error': 'no summary written'}
            mutation = summary.get('mutation') or {}
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
        phase_record = phase_baseline = None
        if phase:
            if reasons:
                phase_record = {'skipped': 'the focused validation blocked'}
            else:
                phase_code, phase_reasons, phase_results, phase_baseline = phase_validation(
                    tip, base, restore, phase, args.timeout, temp, ROOT)
                reasons = phase_reasons
                phase_record = {'dotnet': phase, 'results': str(phase_results), 'validator_exit': phase_code,
                                'reasons': phase_reasons}
        record = {'branch': args.branch, 'base': base, 'tip': tip, 'changed': changed, 'protected_changed': touched,
                  'validation_mode': 'runtime',
                  'mutation_jobs': args.jobs,
                  # Timeouts that held when rerun alone, and crashes: kills without a comparison against Mathcad.
                  'mutation_timeouts': mutation.get('timeouts'), 'mutation_timed_out': mutation.get('timed_out'),
                  'mutation_timeout_retries': mutation.get('timeout_retries'),
                  'mutation_crashes': mutation.get('crashes'),
                  'stub_deletions': stubs,
                  # The phase profile's run, for a branch changing projects the focused profile does not validate.
                  'phase': phase_record,
                  'results': str(results), 'validator_exit': code, 'reasons': reasons}
        (outputs / 'gate.json').write_text(json.dumps(record, indent=2) + '\n')
        if reasons:
            print('BLOCKED:\n  ' + '\n  '.join(reasons) + f'\nResults: {results}')
            return 1
        print(f'PASSED: {args.branch} ({tip[:12]}); results {results}')
        if args.apply:
            merged = baseline_commit(tip, baseline, args.branch, results, ROOT, phase_baseline, base)
            git('-c', 'core.hooksPath=/dev/null', 'merge', '-q', '--ff-only', merged)
            print(f'MERGED: {args.base} -> {merged[:12]}')
        return 0
    finally:
        shutil.rmtree(tree, ignore_errors=True)
        remove_scratch(results)


def remove_scratch(results):
    """Remove the mutation scratch directory (mutant copies of the test output) that the validator named in its
    results, left behind if it was killed mid-run; only a `quire-mutant-scratch-*` directory directly in the temporary
    directory that this user owns, whatever the record says."""
    try:
        named = Path((results / SCRATCH_RECORD).read_text().strip())
        info = named.lstat()
    except (OSError, ValueError):
        return False
    if (named.parent.resolve() != Path(tempfile.gettempdir()).resolve() or not named.name.startswith(SCRATCH_PREFIX)
            or not os.path.isdir(named) or os.path.islink(named)
            or hasattr(os, 'getuid') and info.st_uid != os.getuid()):
        return False
    shutil.rmtree(named, ignore_errors=True)
    return True


def baseline_commit(tip, baseline, branch, results, repo=ROOT, phase_baseline=None, base=None):
    """The tip's committed tree plus the new baseline (the bytes checked) and, when the phase profile ran, the new phase
    baseline; otherwise the phase baseline exactly as `base` has it (absent if it has none), so a branch can change
    neither. Built from objects rather than any working tree, so nothing the validated code did to its checkout can
    reach the merge."""
    baselines = {BASELINE: baseline}
    if phase_baseline is not None:
        baselines[PHASE_BASELINE] = phase_baseline
    elif base is not None:
        # None only when base has no phase baseline; any other failure to read it stops the merge.
        listed = subprocess.run(['git', 'ls-tree', '--name-only', base, '--', PHASE_BASELINE], cwd=repo, check=True,
                                capture_output=True, text=True).stdout.splitlines()
        baselines[PHASE_BASELINE] = (subprocess.run(['git', 'cat-file', 'blob', f'{base}:{PHASE_BASELINE}'], cwd=repo,
                                                    check=True, capture_output=True).stdout
                                     if PHASE_BASELINE in listed else None)
    with tempfile.TemporaryDirectory() as scratch:
        env = dict(os.environ, GIT_INDEX_FILE=str(Path(scratch) / 'index'))
        def run(*command, data=None):
            return subprocess.run(['git', '-c', 'core.hooksPath=/dev/null', *command], cwd=repo, env=env, check=True,
                                  capture_output=True, input=data).stdout.decode().strip()
        run('read-tree', tip)
        for path, data in baselines.items():
            if data is None:
                run('update-index', '--force-remove', path)
                continue
            blob = run('hash-object', '-w', '--stdin', '--path', path, data=data)
            run('update-index', '--add', '--cacheinfo', f'100644,{blob},{path}')
        tree = run('write-tree')
        if tree == run('rev-parse', f'{tip}^{{tree}}'):
            return tip
        commit = run('commit-tree', tree, '-p', tip, '-m', f'Update validation baseline after merging {branch}',
                     '-m', f'Merge gate results: {results}')
    changed = git('diff', '--name-only', tip, commit, cwd=repo).splitlines()
    if not changed or set(changed) - set(baselines):
        raise RuntimeError(f'baseline commit changed {changed}')
    return commit

if __name__ == '__main__':
    sys.exit(main())
