"""Rebase, check exact Release IL, fast-forward main and push it.

    python tools/validation/lane_merge.py mirror/<lane>-<unit> [--main DIR]

Run in the branch's clean worktree. The branch is rebased onto a fresh origin/main and validated (il_merge.gate
without --apply) outside the merge lock, recording base B and tip T. Then, under the lock: fetch and fast-forward main;
if main is still B, T lands as validated. If main moved, the branch is rebased onto it; when that rebase is clean, the
rebased branch affects no more projects than were validated, and B..main touches none of the candidate's build inputs
(inputs_untouched), the compared IL and its base are what was validated, so it lands without building again.
Otherwise it is revalidated under the lock as before. The receipt .mirror-cache/gate-il-<id>/gate.json records the
path taken under "landing". A failed rebase is aborted and its conflicts reported.
"""
import argparse
import json
import os
from pathlib import Path
import subprocess
import sys
import uuid

from merge_lock import held
import il_merge
import release_il


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
    under src/ and at the repository root (but *.md), the type map, the judge (MethodAudit, MirrorGen) and gate rules
    (tools/validation), and for affected native projects their directories and docs/mirror/native/."""
    dirs = {d.name for d in release_il.build_closure(root, projects)} | set(natives)
    prefixes = ['tools/Quire.Tools.MethodAudit/', 'tools/Quire.Tools.MirrorGen/', 'tools/validation/',
                *(f'src/{d}/' for d in dirs),
                # native_gate reads the manifest and finds each project's original through the assembly map.
                *(['docs/mirror/native/', 'docs/mirror/assembly-map.json'] if natives else [])]
    for path in paths:
        parts = path.split('/')
        if len(parts) == 1 and not path.endswith('.md') or parts[0] == 'src' and len(parts) < 3 \
                or path == release_il.TYPE_MAP \
                or path.startswith(tuple(prefixes)):
            return False
    return True


def land(main_dir, tip):
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
    landing = None
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
        args.dotnet, args.timeout = 'dotnet', 1800
        # Optimistic validation outside the lock, against a fresh origin/main.
        git('fetch', '-q', 'origin')
        rebase(args.branch, 'origin/main')
        receipt = il_merge.ROOT / '.mirror-cache' / f'gate-il-{uuid.uuid4().hex[:12]}'
        args.base, args.apply, args.results = 'refs/remotes/origin/main', False, receipt
        code = il_merge.gate(args)
        if code:
            return code
        record = json.loads((receipt / 'gate.json').read_text(encoding='utf-8'))
        base, tip = record['base'], record['tip']
        with held():
            git('fetch', '-q', 'origin', cwd=main_dir)
            git('merge', '-q', '--ff-only', 'origin/main', cwd=main_dir)
            current = git('rev-parse', 'refs/heads/main', cwd=main_dir)
            if git('rev-parse', f'refs/heads/{args.branch}') != tip:
                raise RuntimeError('candidate moved during validation; retry')
            landing = {'main': current, 'validated_base': base, 'validated_tip': tip}
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
                    copy_inventory(lane, tip, rebased, base, record['projects'])
                else:
                    args.base, args.apply = 'main', True
                    args.results = il_merge.ROOT / '.mirror-cache' / f'gate-il-{uuid.uuid4().hex[:12]}'
                    landing.update(path='revalidated', tip=rebased, receipt=str(args.results / 'gate.json'))
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
    except (OSError, RuntimeError, subprocess.SubprocessError) as error:
        detail = getattr(error, 'stderr', None) or str(error)
        if landing is not None:
            landing['error'] = detail
        print(f'BLOCKED: {detail}', file=sys.stderr)
        return 1
    finally:
        if landing is not None:
            record['landing'] = landing
            (receipt / 'gate.json').write_text(json.dumps(record, indent=2) + '\n', encoding='utf-8')


def copy_inventory(root, tip, rebased, judge, projects):
    """The rebased tip's compared IL is the validated tip's: let the next lane's base scan hit the cache. Best effort."""
    try:
        install = release_il.mathcad_install()
        data = release_il.cached_inventory(release_il.CACHE, release_il.inventory_key(root, tip, judge, projects, install))
        if data and projects:
            release_il.store_inventory(release_il.CACHE, release_il.inventory_key(root, rebased, judge, projects, install), data)
    except (OSError, ValueError, subprocess.SubprocessError):
        pass


if __name__ == '__main__':
    sys.exit(main())
