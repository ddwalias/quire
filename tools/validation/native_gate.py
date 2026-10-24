"""Build native projects in Release and require changed functions to match the original DLL's bytes (D10).

    python tools/validation/native_gate.py --base main --project Quire.Native.Utl

The native analog of release_il.py. A native project is a directory src/<P>/ holding <P>.vcxproj and no <P>.csproj; its
original DLL is the one docs/mirror/assembly-map.json maps <P> to, and a project the map does not list fails closed.
The base is built from a temporary checkout and the candidate is the current working tree, each with the pinned MSVC
(Release|x64, outputs in a temporary directory). native_bytes.py compares every object
of each build with the original under the candidate tree's manifest of internal targets, strictly: a function passes
only when its bytes and every call/address target verify.

A function that passed in the base must still pass. A new function, or one whose bytes, relocations or targets changed,
must pass. An unchanged function that does not pass yet is unfinished and does not block. A project without a base
build is new: all of its functions are new. Reports hold names and verdicts, never original bytes.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import uuid

import native_bytes
from native_bytes import anonymous
from release_il import ROOT, git, mathcad_install, msbuild, msvc_mismatch, run

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


def build(root, project, work, log, timeout):
    """The .obj files of `project` built from `root` with the pinned MSVC, Release|x64, outputs under `work`."""
    out, obj = work / 'bin', work / 'obj'
    run([msbuild(), root / 'src' / project / f'{project}.vcxproj', '-t:Rebuild', '-p:Configuration=Release',
         '-p:Platform=x64', f'-p:OutDir={out}{os.sep}', f'-p:IntDir={obj}{os.sep}', '-nologo', '-v:m'],
        root, log, timeout)
    for dll in out.glob('*.dll'):
        if reason := msvc_mismatch(dll):
            raise RuntimeError(f'{dll.name}: {reason}')
    objects = sorted(obj.glob('*.obj'))
    if not objects:
        raise RuntimeError(f'{project}: the build produced no .obj files')
    return objects


def fingerprint(function):
    """A hash of what the candidate controls in a definition: bytes, relocation sites and their targets."""
    key = (function.code.hex(), [(r.offset, r.kind) for r in function.relocs], sorted(function.targets.items(),
           key=str), sorted(function.statics.items()), function.data)
    return hashlib.sha256(anonymous(repr(key)).encode()).hexdigest()


def scan(original, objects, manifest):
    """Function name -> {'ok', 'reason', 'sha256'} from a strict native_bytes run over `objects`."""
    definitions = native_bytes.definitions_in_objects(objects)
    rows = {}
    for r in native_bytes.compare_object(original, objects, None, manifest):
        name = r.name.split(' (')[0]  # "f (at manifest RVA)", "f (manifest entry)" are f
        ok = r.ok and not r.unverified
        reason = r.reason if not r.ok else f'{r.unverified} unverified call/address targets' if r.unverified else None
        rows[anonymous(name)] = {'ok': ok, 'reason': reason,
                                 'sha256': fingerprint(definitions[anonymous(name)])
                                 if anonymous(name) in definitions else None}
    return rows


def compare(before, after):
    """Functions that passed must still pass; new or changed functions must pass; removing a passing one fails."""
    failures, checked = [], []
    for name, row in sorted(after.items()):
        previous = before.get(name)
        changed = previous is None or previous['sha256'] != row['sha256']
        if not (changed or previous['ok']):
            continue
        checked.append(name)
        if not row['ok']:
            failures.append({'method': name, 'reason': row['reason'] if changed else f'passed in base: {row["reason"]}'})
    for name in sorted(before.keys() - after.keys()):
        if before[name]['ok']:
            failures.append({'method': name, 'reason': 'function removed'})
    return {'passed': not failures, 'checked': checked, 'failures': failures,
            'ok': sum(row['ok'] for row in after.values()),
            'unfinished': sum(not row['ok'] for row in after.values())}


def check(base, projects, results, root=ROOT, install=None, timeout=1800):
    """Build base and candidate of each native project, compare them (compare), and write results/native.json."""
    root, install = Path(root), Path(install or mathcad_install())
    results.mkdir(parents=True, exist_ok=True)
    log = results / 'native-build.log'
    report = {'base': base, 'projects': list(projects), 'passed': False, 'checked': [], 'failures': [], 'originals': {}}
    try:
        report['base'] = base = git('rev-parse', '--verify', f'{base}^{{commit}}', cwd=root)
        if unknown := set(projects) - native_projects(root):
            raise RuntimeError(f'not native projects: {sorted(unknown)}')
        manifest = root / MANIFEST if (root / MANIFEST).is_file() else None
        with tempfile.TemporaryDirectory(prefix='quire-native-') as temp:
            work = Path(temp)
            original_tree = work / 'base'
            if projects:
                run(['git', 'clone', '--quiet', '--shared', '--no-checkout', root, original_tree], root, log, timeout)
                run(['git', '-c', 'core.hooksPath=', 'checkout', '--quiet', '--detach', base], original_tree, log,
                    timeout)
            for project in projects:
                dll = install / original_dll(root, project)
                if not dll.is_file():
                    raise RuntimeError(f'{project}: original {dll} not found')
                report['originals'][project] = native_bytes.PeImage(dll).sha256
                print(f'Building {project} (base and candidate)', flush=True)
                existing = (original_tree / 'src' / project / f'{project}.vcxproj').is_file()
                before = (scan(dll, build(original_tree, project, work / project / 'before', log, timeout), manifest)
                          if existing else {})
                after = scan(dll, build(root, project, work / project / 'after', log, timeout), manifest)
                result = compare(before, after)
                report['checked'] += [f'{project}::{name}' for name in result['checked']]
                report['failures'] += [{**f, 'method': f"{project}::{f['method']}"} for f in result['failures']]
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
