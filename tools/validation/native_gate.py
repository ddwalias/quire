"""Build native projects in Release and require changed functions to match the original DLL's bytes (D10).

    python tools/validation/native_gate.py --base main --project Quire.Native.Utl

The native analog of release_il.py. A native project is a directory src/<P>/ holding <P>.vcxproj and no <P>.csproj; its
original DLL is the one docs/mirror/assembly-map.json maps <P> to, and a project the map does not list fails closed.
The base is built from a temporary checkout and the candidate is the current working tree, each with the pinned MSVC
(Release|x64, outputs in a temporary directory). native_bytes.py compares every object
of each build with the original under that tree's own manifest of internal targets, strictly: a function passes
only when its bytes and every call/address target verify.

A function that passed in the base must still pass. A new function, or one whose bytes, relocations or targets changed,
must pass. An unchanged function that does not pass yet is unfinished and does not block. A project without a base
build is new: all of its functions are new. Reports hold names and verdicts, never original bytes.

A project whose vcxproj sets WholeProgramOptimization is built with /GL, so its objects hold no machine code. It is
compared in linked mode (D30, native_linked.py): the gate links it with a /MAP and compares each export of the DLL
with the original's. Link-time code generation sees the whole program, so adding code can change an export nobody
touched: in linked mode such a regression is listed under "regressions" and blocks like any other failure, so no export
that matched stops matching silently. A new or changed export must still pass, and removing a passing one fails.
"""
import argparse
import functools
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import uuid

import native_bytes
import native_linked
from native_bytes import anonymous
from release_il import RAW_BYTES, ROOT, git, mathcad_install, msbuild, msvc_mismatch, run

ASSEMBLY_MAP = Path('docs') / 'mirror' / 'assembly-map.json'
MANIFEST = Path('docs') / 'mirror' / 'native' / 'internal-targets.json'


def native_projects(root=ROOT):
    """src/<P>/ directories with <P>.vcxproj and no <P>.csproj."""
    return {d.name for d in (Path(root) / 'src').iterdir()
            if (d / f'{d.name}.vcxproj').is_file() and not (d / f'{d.name}.csproj').is_file()}


def original_dll(root, project):
    """The original DLL file name the assembly map gives `project`; fails closed when the map does not give one."""
    units = json.loads((Path(root) / ASSEMBLY_MAP).read_text(encoding='utf-8')).get('units', [])
    files = [(u.get('original') or {}).get('file') for u in units if u.get('name') == project]
    if len(files) != 1 or not files[0]:
        raise RuntimeError(f'{project}: {ASSEMBLY_MAP.as_posix()} maps it to no single original DLL')
    return files[0]


def linked(root, project):
    """Whether `project` builds with /GL (WholeProgramOptimization), so the gate compares its linked DLL (D30)."""
    text = (Path(root) / 'src' / project / f'{project}.vcxproj').read_text(encoding='utf-8', errors='replace')
    return '<WholeProgramOptimization>true</WholeProgramOptimization>' in text


def build(root, project, work, log, timeout, link_map=False, whole_program=None):
    """The .obj files of `project` built from `root` with the pinned MSVC, Release|x64, outputs under `work`; with
    `link_map`, (DLL, linker map) instead. `whole_program` overrides the vcxproj's WholeProgramOptimization."""
    out, obj = work / 'bin', work / 'obj'
    env = {**os.environ, '_LINK_': f'/MAP:{out / "candidate.map"}'} if link_map else None
    override = [] if whole_program is None else [f'-p:WholeProgramOptimization={str(whole_program).lower()}']
    run([msbuild(), root / 'src' / project / f'{project}.vcxproj', '-t:Rebuild', '-p:Configuration=Release',
         '-p:Platform=x64', f'-p:OutDir={out}{os.sep}', f'-p:IntDir={obj}{os.sep}', '-nologo', '-v:m', *override],
        root, log, timeout, env)
    dlls = sorted(out.glob('*.dll'))
    for dll in dlls:
        if reason := msvc_mismatch(dll):
            raise RuntimeError(f'{dll.name}: {reason}')
    if link_map:
        if len(dlls) != 1 or not (out / 'candidate.map').is_file():
            raise RuntimeError(f'{project}: the build produced {len(dlls)} DLLs and no single linker map')
        return dlls[0], out / 'candidate.map'
    objects = sorted(obj.glob('*.obj'))
    if not objects:
        raise RuntimeError(f'{project}: the build produced no .obj files')
    return objects


def fingerprint(function):
    """A hash of what the candidate controls in a definition: bytes, relocation sites and their targets."""
    key = (function.code.hex(), [(r.offset, r.kind) for r in function.relocs], sorted(function.targets.items(),
           key=str), sorted(function.statics.items()), function.data)
    return hashlib.sha256(anonymous(repr(key)).encode()).hexdigest()


def object_fingerprints(objects):
    """Function name -> fingerprint of its per-object definition, as scan() computes them for a base built per object."""
    return {name: fingerprint(d) for name, d in native_bytes.definitions_in_objects(objects).items()}


def strict(result):
    """Whether a native_bytes or native_linked result passes the gate: it matches and no target is unverified."""
    return result.ok and not result.unverified


def code_of(candidate, name):
    """What the gate fingerprints for the export `name` of a linked candidate (its instructions by name)."""
    rva = candidate.image.exports[name]
    return native_linked.fingerprint_text(candidate, name) if candidate.executable(rva) else name


def scan(original, objects, manifest):
    """Function name -> {'ok', 'reason', 'sha256'} from a strict native_bytes run over `objects`."""
    definitions = native_bytes.definitions_in_objects(objects)
    rows = {}
    for r in native_bytes.compare_object(original, objects, None, manifest):
        name = anonymous(r.name.split(' (')[0])  # "f (at manifest RVA)", "f (manifest entry)" are f
        reason = r.reason if not r.ok else f'{r.unverified} unverified call/address targets' if r.unverified else None
        if name not in rows or rows[name]['ok']:  # a later row of the same name never hides a failure
            rows[name] = {'ok': strict(r), 'reason': reason,
                          'sha256': fingerprint(definitions[name]) if name in definitions else None}
    return rows


def scan_linked(original, dll, link_map, manifest):
    """Export name -> {'ok', 'reason', 'sha256'} from a strict linked comparison (D30) of `dll` with the original's
    Linked (native_linked.Linked.load)."""
    c = native_linked.Linked.load(dll, Path(link_map).read_text(encoding='utf-8', errors='replace'))
    entries = native_bytes.load_targets(manifest, original.image) if manifest else {}
    comparison = native_linked.Comparison(original, c, entries)
    rows = {}
    for name in sorted(c.image.exports):
        r = comparison.export(name)
        reason = r.reason if not r.ok else f'{r.unverified} unverified call/address targets' if r.unverified else None
        rows[anonymous(name)] = {'ok': strict(r), 'reason': reason,
                                 'sha256': hashlib.sha256(code_of(c, name).encode()).hexdigest()}
    return rows


def compare(before, after, linked_mode=False):
    """Functions that passed must still pass; new or changed functions must pass; removing a passing one fails. In
    linked mode (D30) an unchanged export that stops passing is a failure too, and also listed under regressions."""
    failures, checked, regressions = [], [], []
    for name, row in sorted(after.items()):
        previous = before.get(name)
        changed = previous is None or None in (previous['sha256'], row['sha256']) or previous['sha256'] != row['sha256']
        if not (changed or previous['ok']):
            continue
        checked.append(name)
        if not row['ok']:
            if changed:
                failures.append({'method': name, 'reason': row['reason']})
            else:
                failures.append({'method': name, 'reason': f'passed in base: {row["reason"]}'})
                if linked_mode:  # listed apart too, so the report shows which exports link-time codegen moved
                    regressions.append({'method': name, 'reason': f'passed in base: {row["reason"]}'})
    for name in sorted(before.keys() - after.keys()):
        if before[name]['ok']:
            failures.append({'method': name, 'reason': 'function removed'})
    return {'passed': not failures, 'checked': checked, 'failures': failures, 'regressions': regressions,
            'ok': sum(row['ok'] for row in after.values()),
            'unfinished': sum(not row['ok'] for row in after.values())}


def scan_project(original, root, project, work, log, timeout, manifest, linked_original):
    """Rows of `project` built from `root`: per object against the `original` DLL, or from the linked DLL when it builds
    with /GL (D30) against `linked_original()`, the original's Linked."""
    if linked(root, project):
        dll, link_map = build(root, project, work, log, timeout, link_map=True)
        return scan_linked(linked_original(), dll, link_map, manifest)
    return scan(original, build(root, project, work, log, timeout), manifest)


def check(base, projects, results, root=ROOT, install=None, timeout=1800):
    """Build base and candidate of each native project, compare them (compare), and write results/native.json."""
    root, install = Path(root), Path(install or mathcad_install())
    results.mkdir(parents=True, exist_ok=True)
    log = results / 'native-build.log'
    report = {'base': base, 'projects': list(projects), 'passed': False, 'checked': [], 'failures': [],
              'regressions': [], 'originals': {}}
    try:
        report['base'] = base = git('rev-parse', '--verify', f'{base}^{{commit}}', cwd=root)
        if unknown := set(projects) - native_projects(root):
            raise RuntimeError(f'not native projects: {sorted(unknown)}')
        manifest = root / MANIFEST if (root / MANIFEST).is_file() else None
        with tempfile.TemporaryDirectory(prefix='quire-native-', ignore_cleanup_errors=True) as temp:
            work = Path(temp)
            original_tree = work / 'base'
            if projects:
                run(['git', 'clone', '--quiet', '--shared', '--no-checkout', root, original_tree], root, log, timeout)
                # Committed bytes, as il_merge's candidate tree holds them, whatever core.autocrlf says.
                (original_tree / '.git/info').mkdir(parents=True, exist_ok=True)
                (original_tree / '.git/info/attributes').write_text(RAW_BYTES, encoding='utf-8')
                run(['git', '-c', 'core.hooksPath=', 'checkout', '--quiet', '--detach', base], original_tree, log,
                    timeout)
            # The base is scanned under its own manifest: under the candidate's, an entry the candidate removes or
            # alters would fail its callers on both sides, and compare() would skip them as unchanged.
            base_manifest = original_tree / MANIFEST if (original_tree / MANIFEST).is_file() else None
            for project in projects:
                dll = install / original_dll(root, project)
                if not dll.is_file():
                    raise RuntimeError(f'{project}: original {dll} not found')
                image = native_bytes.PeImage(dll)
                report['originals'][project] = image.sha256
                linked_original = functools.cache(functools.partial(native_linked.Linked.load, dll))
                print(f'Building {project} (base and candidate)', flush=True)
                existing = (original_tree / 'src' / project / f'{project}.vcxproj').is_file()
                before = scan_project(dll, original_tree, project, work / project / 'before', log, timeout,
                                      base_manifest, linked_original) if existing else {}
                linked_mode = linked(root, project)
                after = scan_project(dll, root, project, work / project / 'after', log, timeout, manifest,
                                     linked_original)
                if linked_mode and existing and not linked(original_tree, project):
                    # A per-object base's fingerprints cannot match linked ones, so every export would count as changed
                    # and none as a regression (D30). Decide "changed" on the candidate's per-object fingerprints
                    # instead; its linked rows still decide what passes.
                    per_object = object_fingerprints(build(root, project, work / project / 'objects', log, timeout,
                                                           whole_program=False))
                    for name, row in after.items():
                        row['sha256'] = per_object.get(name)
                if linked_mode:  # a linked scan lists exports only; compare the base's exports with them
                    before = {n: r for n, r in before.items() if n in {anonymous(e) for e in image.exports}}
                result = compare(before, after, linked_mode)
                report['checked'] += [f'{project}::{name}' for name in result['checked']]
                report['failures'] += [{**f, 'method': f"{project}::{f['method']}"} for f in result['failures']]
                report['regressions'] += [{**f, 'method': f"{project}::{f['method']}"} for f in result['regressions']]
                report.setdefault('functions', {})[project] = {'ok': result['ok'], 'unfinished': result['unfinished']}
        report['passed'] = not report['failures']
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
        report['error'] = str(error)
    finally:
        (results / 'native.json').write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    return report


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--base', default='main')
    parser.add_argument('--project', action='append', default=[], help='native project (default: all of them)')
    parser.add_argument('--results', type=Path)
    parser.add_argument('--timeout', type=int, default=1800)
    args = parser.parse_args(argv)
    results = args.results or ROOT / '.mirror-cache' / ('native-gate-' + uuid.uuid4().hex[:12])
    report = check(args.base, sorted(args.project or native_projects()), results, timeout=args.timeout)
    for failure in report['failures'][:20]:
        print(f"  {failure['method']}: {failure['reason']}")
    if len(report['failures']) > 20:
        print(f"  {len(report['failures']) - 20} more failures in the report")
    if report.get('error'):
        print(report['error'], file=sys.stderr)
    print(f"Native bytes: {'PASS' if report['passed'] else 'FAIL'}; {len(report['checked'])} checked; "
          f"{results / 'native.json'}")
    return 0 if report['passed'] else 1


if __name__ == '__main__':
    sys.exit(main())
