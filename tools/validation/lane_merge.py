"""Rebase, check exact Release IL, fast-forward main and push it.

    python tools/validation/lane_merge.py mirror/<lane>-<unit> [--main DIR]

Run in the branch's clean worktree. The branch is rebased onto a fresh origin/main and validated (il_merge.gate
without --apply) outside the merge lock, recording base B and tip T. Then, under the lock: fetch and fast-forward main;
if main is still B, T lands as validated. If main moved, the branch is rebased onto it; when that rebase is clean, the
rebased branch affects no more projects than were validated, and B..main touches none of the candidate's build inputs
(inputs_untouched), the compared IL and its base are what was validated, so it lands without building again.
Otherwise the lock is released and the branch is validated again against the new origin/main (path "retry"); only
the last of ROUNDS rounds revalidates under the lock, so a lane still lands while main keeps moving, and the lock is
otherwise held just to land. A branch that changes tools/validation must also pass that folder's unit tests
(tool_tests), which run before each validation of a tree whose rules were not tested yet. Each round's receipt
.mirror-cache/gate-il-<id>/gate.json records the path taken, the round and the seconds spent waiting for and holding the lock under "landing". The round that lands is also copied to
.mirror-cache/receipts/gate-il-<id>.json: the gate-il folders hold a run's inventories and logs and are disposable
(cache_prune.py removes old ones), the receipts folder is the durable record. A failed rebase is aborted and its
conflicts reported.
"""
import argparse
import json
import os
from pathlib import Path
import subprocess
import sys
import time
import uuid

from merge_lock import held
import il_merge
import release_il
from layout import project_dir, project_of

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'lint'))
import lint


REEXEC = 'QUIRE_LANE_MERGE_FROM_MAIN'


def git(*args, cwd=None):
    # Without renames, --name-only lists both sides of a move (see release_il.git).
    return subprocess.run(['git', '-c', 'diff.renames=false', *args], cwd=cwd, capture_output=True, text=True, check=True).stdout.strip()


def main_worktree(cwd=None):
    directory = None
    for line in git('worktree', 'list', '--porcelain', cwd=cwd).splitlines():
        if line.startswith('worktree '):
            directory = line[len('worktree '):]
        elif line == 'branch refs/heads/main':
            return directory
    return None


def rebase(branch, onto):
    """Rebase the current worktree's branch; on a conflict abort it, leaving the branch as it was, and say where."""
    result = subprocess.run(['git', 'rebase', onto, branch], capture_output=True, text=True)
    if result.returncode:
        conflicts = subprocess.run(['git', 'diff', '--name-only', '--diff-filter=U'], capture_output=True,
                                   text=True).stdout.split()
        subprocess.run(['git', 'rebase', '--abort'], capture_output=True)
        raise RuntimeError(f'rebase of {branch} onto {onto} failed and was aborted; resolve it and retry. '
                           f'Conflicts: {", ".join(conflicts) or "none listed"}\n{(result.stdout + result.stderr)[-1500:]}')


def inputs_untouched(paths, root, projects, natives):
    """Whether none of paths (B..main) can change the compared Release IL, its base or its verdict: conservative.
    Inputs are each compared project and its forward ProjectReference closure, everything outside a project directory
    under src/ and at the repository root (but *.md), the type map, the registry of generated assemblies (which rule
    judges a project), the judge (MethodAudit, MirrorGen) and gate rules
    (tools/validation), and for affected native projects their directories and docs/mirror/native/."""
    closure = release_il.build_closure(root, projects)
    input_projects = {d.name for d in closure} | set(natives)
    dirs = {d.relative_to(root / 'src').as_posix() for d in closure}
    dirs |= {project_dir(root, n).relative_to(root / 'src').as_posix() for n in natives}
    prefixes = ['tools/Quire.Tools.MethodAudit/', 'tools/Quire.Tools.MirrorGen/', 'tools/validation/',
                *(f'src/{d}/' for d in dirs),
                # native_gate reads the manifest and finds each project's original through the assembly map.
                *(['docs/mirror/native/', 'docs/mirror/assembly-map.json'] if natives else [])]
    for path in paths:
        parts = path.split('/')
        owner = project_of(path, root) if parts[0] == 'src' else None
        if owner is not None:
            if owner in input_projects:
                return False
            continue
        if len(parts) == 1 and not path.endswith('.md') or parts[0] == 'src' and len(parts) < 3 \
                or path in (release_il.TYPE_MAP, release_il.SOURCE_LAYOUT, release_il.GENERATED_ASSEMBLIES) \
                or path.startswith(tuple(prefixes)):
            return False
    return True


GATE_RULES = 'tools/validation/'
# Trees of GATE_RULES whose tests passed in this run: a later round with the same rules does not repeat them.
TESTED = set()


def tool_tests(lane, base, timeout):
    """Run the unit tests of tools/validation when the branch changes that folder: the gate's rules are not judged by
    the gate (MethodAudit compares IL, not Python), so their own tests are what stands between a broken rule and main.
    They are the branch's tests of the branch's rules, run in its worktree."""
    if not any(path.startswith(GATE_RULES) for path in git('diff', '--name-only', base, 'HEAD', cwd=lane).splitlines()):
        return
    folder = Path(lane) / 'tools' / 'validation'
    tree = git('rev-parse', f'HEAD:{GATE_RULES.rstrip("/")}', cwd=lane)
    if tree in TESTED or not any(folder.glob('test_*.py')):
        return
    print(f'{GATE_RULES} changed: running its unit tests', flush=True)
    try:
        result = subprocess.run([sys.executable, '-m', 'unittest', 'discover', '-s', '.', '-p', 'test_*.py'], cwd=folder,
                                capture_output=True, text=True, timeout=timeout)
    except subprocess.TimeoutExpired:
        raise RuntimeError(f'the unit tests of {GATE_RULES} did not finish in {timeout}s') from None
    if result.returncode:
        raise RuntimeError(f"the unit tests of {GATE_RULES} fail on this branch; fix them and retry "
                           f"(python -m unittest discover, in that folder)\n{(result.stdout + result.stderr)[-3000:]}")
    TESTED.add(tree)


def style_lint(lane, base):
    """Fail before the long IL gate when the branch adds an ILSpy-style local name or an unexplained goto/label
    (tools/lint/lint.py, a per-file ratchet against the base: existing debt never blocks)."""
    lint.ROOT = Path(lane)
    items = lint.regressions(base)
    if items:
        lint.report(items)
        raise RuntimeError('style lint failed (see LINT lines above): rename the locals / comment the gotos and retry')


def forget_stale_cherry_pick(main_dir):
    """A cherry-pick of a commit that is already in main's history (someone's retry) leaves CHERRY_PICK_HEAD behind, and the
    next integration then dies with 'You have not concluded your cherry-pick'. With a clean tree there is nothing to
    conclude, so forget the stale state."""
    if not git('status', '--porcelain', cwd=main_dir) and (Path(main_dir) / '.git' / 'CHERRY_PICK_HEAD').exists():
        git('cherry-pick', '--quit', cwd=main_dir)


def land(main_dir, tip):
    forget_stale_cherry_pick(main_dir)
    if git('status', '--porcelain', cwd=main_dir):
        raise RuntimeError('main checkout changed; retry')
    git('-c', 'core.hooksPath=', 'merge', '-q', '--ff-only', tip, cwd=main_dir)


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
        # The verdict logic is main's, never the candidate's: a lane running its own copy re-runs main's.
        own = Path(main_dir) / 'tools' / 'validation' / 'lane_merge.py'
        if own.is_file() and own.resolve() != Path(__file__).resolve() and not os.environ.get(REEXEC):
            return subprocess.run([sys.executable, own, *(sys.argv[1:] if argv is None else argv)],
                                  env={**os.environ, REEXEC: '1'}).returncode
        lane = Path(git('rev-parse', '--show-toplevel'))
        il_merge.ROOT = Path(main_dir)
        # One cache for every lane: the next lane's base is the tip this one lands.
        release_il.CACHE = Path(main_dir) / '.mirror-cache' / 'inventory-cache'
        args.dotnet, args.timeout = 'dotnet', int(os.environ.get('QUIRE_LANE_TIMEOUT', '3600'))
        for number in range(1, ROUNDS + 1):
            code = attempt(args, main_dir, lane, number)
            if code is not None:
                return code
        raise AssertionError('the last round always lands or fails')
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
        print(f'BLOCKED: {getattr(error, "stderr", None) or error}', file=sys.stderr)
        return 1
    finally:
        subprocess.run(['git', 'update-ref', '-d', pin(args.branch)], capture_output=True)


# Rounds of validation outside the lock. A round whose base main has since moved in a way that touches the candidate's
# inputs releases the lock and validates again; only the last round revalidates under the lock, so a lane lands even
# while main keeps moving, and otherwise the lock is held just to land.
ROUNDS = 3


def pin(branch):
    """The ref holding the base the branch's current round validates against."""
    return f'refs/lane-merge/{branch}'


def attempt(args, main_dir, lane, number):
    """One round: validate against a fresh origin/main outside the lock, then land under it. Returns the exit code, or
    None to validate again (main moved and the candidate's inputs changed; never on the last round)."""
    # Optimistic validation outside the lock, against a fresh origin/main. The base is pinned to a ref of this lane's:
    # a lane landing meanwhile fetches, which moves origin/main under this round.
    git('fetch', '-q', 'origin')
    git('update-ref', pin(args.branch), 'refs/remotes/origin/main')
    rebase(args.branch, pin(args.branch))
    tool_tests(lane, pin(args.branch), args.timeout)
    style_lint(lane, pin(args.branch))
    receipt = il_merge.ROOT / '.mirror-cache' / f'gate-il-{uuid.uuid4().hex[:12]}'
    args.base, args.apply, args.results = pin(args.branch), False, receipt
    code = il_merge.gate(args)
    if code:
        return code
    record = json.loads((receipt / 'gate.json').read_text(encoding='utf-8'))
    base, tip = record['base'], record['tip']
    landing = {'round': number}
    try:
        waiting = time.perf_counter()
        with held():
            landing['lock_wait_s'] = round(time.perf_counter() - waiting, 1)
            held_since = time.perf_counter()
            try:
                code = land_under_lock(args, main_dir, lane, record, base, tip, number, landing)
            finally:
                landing['lock_held_s'] = round(time.perf_counter() - held_since, 1)
        if code is None:
            print(f"main moved under {args.branch}'s inputs; validating again (round {number + 1} of {ROUNDS})",
                  flush=True)
        else:
            print(f"Landing: {landing.get('path')}; {receipt / 'gate.json'}", flush=True)
        return code
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
        landing['error'] = getattr(error, 'stderr', None) or str(error)
        raise
    finally:
        record['landing'] = landing
        (receipt / 'gate.json').write_text(json.dumps(record, indent=2) + '\n', encoding='utf-8')
        if landing.get('pushed'):
            kept = il_merge.keep_receipt(record, receipt.name)
            # The last round's revalidation is a second run with its own record: the landing rests on both.
            revalidation = Path(landing['receipt']) if landing.get('receipt') else None
            if revalidation and revalidation.is_file():
                il_merge.keep_receipt(json.loads(revalidation.read_text(encoding='utf-8')), revalidation.parent.name)
            print(f'Landing receipt: {kept}', flush=True)


def land_under_lock(args, main_dir, lane, record, base, tip, number, landing):
    git('fetch', '-q', 'origin', cwd=main_dir)
    forget_stale_cherry_pick(main_dir)
    git('merge', '-q', '--ff-only', 'origin/main', cwd=main_dir)
    current = git('rev-parse', 'refs/heads/main', cwd=main_dir)
    if git('rev-parse', f'refs/heads/{args.branch}') != tip:
        raise RuntimeError('candidate moved during validation; retry')
    landing.update(main=current, validated_base=base, validated_tip=tip)
    if current == base:
        land(main_dir, tip)
        landing.update(path='validated', tip=tip)
    else:
        rebase(args.branch, 'main')
        rebased = git('rev-parse', f'refs/heads/{args.branch}')
        moved = git('diff', '--name-only', base, current).splitlines()
        changed = git('diff', '--name-only', current, rebased).splitlines()
        natives = il_merge.affected_native(changed, il_merge.native_projects(lane))
        # A src directory added or removed on main changes the judge's set of Quire assemblies.
        same_src = git('ls-tree', '--name-only', base, 'src/') == git('ls-tree', '--name-only', current, 'src/')
        if (changed and same_src and set(release_il.affected_projects(current, lane)) <= set(record['projects'])
                and set(natives) <= set(record['native_projects'])
                and inputs_untouched(moved, lane, record['projects'], record['native_projects'])):
            land(main_dir, rebased)
            landing.update(path='rebased-unaffected', tip=rebased)
        elif number < ROUNDS:
            landing['path'] = 'retry'
            return None
        else:
            args.base, args.apply, args.keep = 'main', True, False  # kept with this round's, once pushed
            args.results = il_merge.ROOT / '.mirror-cache' / f'gate-il-{uuid.uuid4().hex[:12]}'
            landing.update(path='revalidated', tip=rebased, receipt=str(args.results / 'gate.json'))
            tool_tests(lane, current, args.timeout)  # main's rules and the branch's, together for the first time
            code = il_merge.gate(args)
            if code:
                landing['path'] = 'revalidation-failed'
                return code
    try:
        git('push', 'origin', 'main', cwd=main_dir)
    except subprocess.CalledProcessError:
        # Leave main as origin has it, so a retry sees the branch's changes again.
        git('reset', '-q', '--keep', current, cwd=main_dir)
        raise
    landing['pushed'] = True
    return 0


if __name__ == '__main__':
    sys.exit(main())
