"""Build production projects in Release and require changed methods to match Mathcad IL.

    python tools/validation/release_il.py --base main --project Quire.Mpl

No test host, test execution, coverage, mutation, status gate or stored baseline is used.
The base is built from a temporary checkout; the candidate is the current working tree.
Reports contain identities and verdicts, never original IL. --method selects additional
Type::signature keys; --type selects all methods declared on a type (including stubs).

    python tools/validation/release_il.py --base main --project Quire.Engine.Utl --candidate-vcxproj PATH

builds the candidate from a Windows C++/CLI .vcxproj (MSBuild, Release|x64) instead of
src/<project>/<project>.csproj; the base is still the C# Release build. In a mixed-mode candidate
(D19), compiler-support methods without a Mathcad counterpart and native bodies whose counterpart
is native are reported but do not fail; every other method follows the usual rules. Builds never
see the Mathcad install, so no original IL can reach a candidate.
"""
import argparse
import contextlib
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import subprocess
import sys
import tempfile
import uuid
from xml.sax.saxutils import quoteattr

ROOT = Path(__file__).resolve().parents[2]
DEFAULT_INSTALL = (r'C:\Program Files\PTC\Mathcad Prime 12.0.1.0' if os.name == 'nt'
                   else '/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0')


def mathcad_install(environ=os.environ):
    return Path(environ.get('MATHCAD_DIR', DEFAULT_INSTALL))


def git(*args, cwd=ROOT):
    # Without renames, --name-only lists both sides of a move: a file moved out of a project changes that project too.
    return subprocess.run(['git', '-c', 'diff.renames=false', *map(str, args)], cwd=cwd, capture_output=True,
                          text=True, check=True).stdout.strip()


def production_projects(root=ROOT):
    return {p.stem for p in (root / 'src').glob('*/*.csproj')}


# MethodAudit pairs every production type through the type map, so a change to it can change any verdict.
TYPE_MAP = 'docs/mirror/type-map.csv'


def affected_projects(base, root=ROOT):
    """Production projects whose Release IL or verdicts the diff from base can change: those with a changed file under
    src/<P>/; every project whose ProjectReference closure (build_closure, through .csproj and imported .props) reaches
    a changed src directory, since a const, enum or default value is compiled into its users; and all of them for root
    build configuration or the type map. MethodAudit/MirrorGen changes select nothing: check builds the judge from the
    base commit, so they cannot change a verdict of their own branch, and both scans of later runs use the new judge."""
    paths = git('diff', '--name-only', base, cwd=root).splitlines()
    paths += git('ls-files', '--others', '--exclude-standard', cwd=root).splitlines()
    present = production_projects(root)
    projects = present | {Path(p).stem for p in git('ls-tree', '-r', '--name-only', base, '--', 'src', cwd=root).splitlines()
                          if p.endswith('.csproj')}
    # Shared compiler inputs can change every output, so they require the full IL scan.
    if any('/' not in p and p.endswith(('.props', '.targets', '.json', '.slnx')) or p == TYPE_MAP for p in paths):
        return sorted(projects)
    changed = {parts[1] for p in paths if len(parts := p.split('/')) >= 3 and parts[0] == 'src'}
    references = {name: project_references(root, root / 'src' / name) for name in present}
    return sorted((changed & projects) | {p for p in present if changed & closure_names(references, p)})


def closure_names(references, project):
    """project plus every src directory its references reach, transitively (cycles allowed)."""
    seen, todo = set(), [project]
    while todo:
        if (name := todo.pop()) not in seen:
            seen.add(name)
            todo += references.get(name, [])
    return seen


def selected(row, types, methods):
    key = f"{row['type']}::{row['signature']}"
    aliases = {key, row['id']}
    if declared := row.get('declared_signature'):
        aliases.add(f"{row['type']}::{declared}")
        if project := row.get('project'):
            aliases.add(f"{project}::{row['type']}::{declared}")
    return row['type'] in types or any(alias in methods for alias in aliases)


def ordinalless(text):
    """Compiler-generated member ordinals replaced by '#', as MethodAudit hashes bodies (<>c__DisplayClass12_0,
    <M>b__12_3, <>9__12_3, <M>g__Local|12_0, <M>d__12)."""
    return re.sub(r'(c__DisplayClass|>b__|>9__|\|)\d+(?=_\d)|(>d__)\d+',
                  lambda m: (m.group(1) or m.group(2)) + '#', text)


# D19: what MSVC emits for C++/CLI on its own (standard-library and CRT instantiations,
# exception/startup helpers, C++ attribute and modifier types). Only these may lack a counterpart.
TOOLSET_TYPES = ('std.', '<CrtImplementationDetails>', 'vc.cppcli.')
TOOLSET_GLOBALS = re.compile(r'(std\.|__std_|_CRT|__security_|___Cxx|__eh|__Exception|_atexit_|'
                             r'(_onexit|_getFiberPtrId|PtrToStringChars|new|delete|delete\[\])`)')


def toolset_support(row):
    """A mixed-mode compiler-support member that Mathcad's build did not need: unpaired, and no Mathcad member
    of the same owner, name and arity exists (that would be a pairing miss)."""
    if not row.get('mixed_mode') or row.get('original') is not None or row.get('original_name_taken') is not False:
        return False
    if row['type'] == '<Module>':
        return bool(TOOLSET_GLOBALS.match(row['signature']))
    return row['type'].startswith(TOOLSET_TYPES)


def native_pair(row):
    """A mixed-mode native body whose Mathcad counterpart is native too: no IL on either side (D10)."""
    return bool(row.get('mixed_mode') and (row.get('unsupported') or '').startswith('native code')
                and (row.get('original') or {}).get('native_code') is True and row.get('shape_matches') is True)


def stub(row):
    """Generated by MirrorGen: a MirrorStub throw, or a member carrying [MirrorStub]."""
    return bool(row.get('stub_body') or row.get('mirror_stub'))


def compare(before, after, types=(), methods=()):
    """Changed bodies/declarations and every previously exact method must match.

    Unmapped changed methods are blockers, including generated helpers. Unchanged
    unfinished methods stay out of the batch. D10 engine-native methods are identified
    by MethodAudit from their original artifact and do not require a managed IL match.
    A generated stub replaced by another generated stub is unchanged, and removing a
    never-exact generated stub (hand-written code now provides the member) does not fail;
    implemented members and previously exact methods are judged as before.
    Removing a compiler-generated member with no Mathcad counterpart (a renumbered
    closure or state machine) does not fail the gate. Nor does changing or removing an
    existing product-only member (D21); a new member without a counterpart still fails.
    """
    old = {m['id']: m for m in before['methods']}
    new = {m['id']: m for m in after['methods']}
    # A compiler-generated member renumbered by an unrelated declaration change is the same member.
    renumbered = {}
    for m in before['methods']:
        if m.get('original') is None and '<' in m['id']:
            renumbered.setdefault(ordinalless(m['id']), []).append(m)
    # A type moved to another project (D2 routing) is the same member there; it keeps its obligations.
    # Signatures name the Quire assembly of each type ([Quire.Engine.Utl]...), which a move changes.
    place = lambda m: (m['type'], re.sub(r'\[Quire(\.[\w.]+)?\]', '[Quire]', m['signature']))
    moved = {place(m): m for k, m in old.items() if k not in new}
    mirrored = {m['type'].split('+')[0] for m in (*old.values(), *new.values()) if m.get('original') is not None}
    failures, checked = [], []
    for key, row in new.items():
        previous = old.get(key)
        origin = moved.get(place(row))
        if previous is None and origin and origin.get('project') != row.get('project'):
            previous = moved.pop(place(row))
        if previous is None and row.get('original') is None and '<' in key:
            same = [m for m in renumbered.get(ordinalless(key), []) if m.get('body_sha256') == row.get('body_sha256')]
            previous = same[0] if same else None  # identical bodies: any of them is the same member
        changed = previous is None or any(previous.get(k) != row.get(k) for k in
                                         ('body_sha256', 'shape_matches', 'declaration_only'))
        # A generated stub is unfinished wherever it sits and however MirrorGen spells it (the file a
        # MirrorStub throw records, a constructor stub's chaining); replacing one stub with another is not a change.
        # [MirrorStub] counts: MirrorGen re-spells a constructor stub's chaining, and only generated code carries it.
        if changed and previous is not None and stub(previous) and stub(row) \
                and previous.get('shape_matches') == row.get('shape_matches'):
            changed = False
        if not (changed or row.get('unsupported') or selected(row, types, methods)
                or previous and previous.get('exact_match') is True):
            continue
        checked.append(key)
        # A member that was exact, or that lost its counterpart, keeps its obligations.
        # A native pair that was exact in a mixed-mode base keeps the obligation only if it stopped being exact: its
        # exact_match covers the import record, which is what a native pair has in place of an IL body.
        if (toolset_support(row) or native_pair(row)) and not (
                previous and (previous.get('exact_match') is True
                              and not (native_pair(previous) and row.get('exact_match') is True)
                              or previous.get('original') is not None and row.get('original') is None)):
            continue
        if row.get('unsupported'):
            reason = (row['unsupported'] if not native_pair(row)
                      else '; '.join(row.get('structure_differences', [])[:2]) or 'native import record differs')
            failures.append({'method': key, 'reason': reason})
            continue
        # D10 exempts an engine-native body, not a member that was exact, nor a new one without a counterpart.
        if row.get('engine_native') is True and not (previous and previous.get('exact_match') is True) \
                and not (previous is None and row.get('original') is None):
            continue
        # D21: an existing product-only member (no Mathcad counterpart before or after) may change; new ones still fail.
        # Not a compiler-generated helper of a mirrored type (a closure whose ordinal missed), nor a selected member.
        if row.get('original') is None and previous is not None and previous.get('original') is None \
                and not ('<' in row['type'] + row['signature'] and row['type'].split('+')[0] in mirrored) \
                and not selected(row, types, methods):
            continue
        if row.get('shape_matches') is not True or not (row.get('exact_match') is True or row.get('declaration_only') is True):
            # MethodAudit records why it could not pair or read a row (an ambiguous modifier tie) in structure_differences.
            reason = (('; '.join(row.get('structure_differences', [])[:2]) or 'no Mathcad counterpart')
                      if row.get('original') is None else
                      'declaration differs' if row.get('shape_matches') is not True else
                      '; '.join(row.get('structure_differences', [])[:2]) or 'Release IL differs')
            failures.append({'method': key, 'reason': reason})
    for key in old.keys() - new.keys():
        # Moved to another project (popped above): its new row was checked there.
        if place(old[key]) not in moved:
            continue
        # Renumbered compiler-generated members Mathcad never had; their replacements are new rows checked above.
        if old[key].get('original') is None and '<' in old[key]['type'] + old[key]['signature']:
            continue
        # A never-exact generated stub MirrorGen no longer writes (hand-written code now provides the member).
        if stub(old[key]) and old[key].get('exact_match') is not True:
            continue
        # D19/D21: a product-only member (no Mathcad counterpart, so never exact) may be removed.
        if old[key].get('original') is None and old[key].get('exact_match') is not True:
            continue
        failures.append({'method': key, 'reason': 'method removed'})
    for name in types:
        if not any(m['type'] == name for m in new.values()):
            failures.append({'method': name, 'reason': 'selected type not found'})
    for name in methods:
        if not any(selected(m, (), (name,)) for m in new.values()):
            failures.append({'method': name, 'reason': 'selected method not found'})
    return {'passed': not failures, 'checked': checked, 'failures': failures,
            'exact': sum(m.get('exact_match') is True for m in new.values()),
            'engine_native': sum(m.get('engine_native') is True for m in new.values()),
            'toolset_support': sum(toolset_support(m) for m in new.values()),
            'native_pairs': sum(native_pair(m) for m in new.values()),
            'unfinished': sum(m.get('exact_match') is not True and not m.get('declaration_only')
                              and not m.get('engine_native') and not toolset_support(m) and not native_pair(m)
                              for m in new.values())}


def run(command, cwd, log, timeout, env=None):
    with log.open('a', encoding='utf-8') as out:
        out.write('$ ' + subprocess.list2cmdline(list(map(str, command))) + '\n')
        out.flush()
        result = subprocess.run(list(map(str, command)), cwd=cwd, stdout=out,
                                stderr=subprocess.STDOUT, timeout=timeout, env=env)
    if result.returncode:
        detail = log.read_text(encoding='utf-8', errors='replace')[-2000:]
        raise RuntimeError(f'command failed ({result.returncode}); see {log}\n{detail}')


# Mathcad's utl-mgd.dll was compiled and linked by MSVC 19.51.36246 (Visual Studio 18.6.2), per its Rich header.
MSVC_BUILD = 36246
LINKER_PRODUCT = 258  # Rich-header product id of the MSVC 14.x linker


def msvc_instance(environ=os.environ):
    """The Visual Studio instance with C++/CLI support whose x64 compiler is build MSVC_BUILD; fails closed."""
    vswhere = Path(environ.get('ProgramFiles(x86)', r'C:\Program Files (x86)'), 'Microsoft Visual Studio', 'Installer', 'vswhere.exe')
    if not vswhere.is_file():
        raise RuntimeError(f'a C++/CLI candidate needs Visual Studio on Windows; {vswhere} not found')
    paths = subprocess.run([vswhere, '-all', '-products', '*', '-requires', 'Microsoft.VisualStudio.Component.VC.CLI.Support',
                            '-property', 'installationPath'], capture_output=True, text=True, check=True).stdout.splitlines()
    for path in map(Path, filter(None, paths)):
        for cl in path.glob('VC/Tools/MSVC/*/bin/Hostx64/x64/cl.exe'):
            banner = subprocess.run([cl], capture_output=True, text=True).stderr
            if f'Version 19.51.{MSVC_BUILD} ' in banner:
                return path
    raise RuntimeError(f'no Visual Studio with C++/CLI support has MSVC 19.51.{MSVC_BUILD} (install Build Tools 18.6.2)')


def msbuild(environ=os.environ):
    """MSBuild of the pinned MSVC instance."""
    return msvc_instance(environ) / 'MSBuild' / 'Current' / 'Bin' / 'amd64' / 'MSBuild.exe'


def rich_builds(dll):
    """(product id, build) pairs of a PE's Rich header; empty for images without one (Roslyn output)."""
    data = Path(dll).read_bytes()
    pe = int.from_bytes(data[0x3c:0x40], 'little')
    end = data.find(b'Rich', 0x80, pe)
    if end < 0:
        return []
    key = int.from_bytes(data[end + 4:end + 8], 'little')
    start = data.rfind((0x536E6144 ^ key).to_bytes(4, 'little'), 0, end)  # 'DanS'
    entries = (int.from_bytes(data[o:o + 4], 'little') ^ key for o in range(start + 16, end, 8))
    return [(comp >> 16, comp & 0xFFFF) for comp in entries]


def msvc_mismatch(dll):
    """Why a mixed-mode DLL was not linked by the pinned MSVC (None when it was, or for IL-only images)."""
    builds = rich_builds(dll)
    if not builds:
        return None
    linkers = {build for product, build in builds if product == LINKER_PRODUCT}
    newer = sorted({build for _, build in builds if build > MSVC_BUILD})
    if linkers != {MSVC_BUILD} or newer:
        return f'linker builds {sorted(linkers)}, newer tool builds {newer}; MSVC 19.51.{MSVC_BUILD} is required'
    return None


def without_install(work, environ=os.environ):
    """D19: builds see no Mathcad install, so original IL cannot be copied into a candidate."""
    env = {k: v for k, v in environ.items() if k.upper() != 'MATHCAD_DIR'}
    return env, f'-p:MathcadInstallDir={work / "no-mathcad-install"}{os.sep}'


INSTALL_REFERENCE = re.compile(r'MathcadInstallDir|MATHCAD_DIR|Mathcad Prime|\\PTC\\', re.IGNORECASE)


BUILD_INPUTS = ('.csproj', '.vcxproj', '.props', '.targets', '.rsp', '.cpp', '.h', '.hpp')


def build_files(directory):
    """Files of a project directory that a build can read, outside bin and obj."""
    files = []
    for path, dirs, names in os.walk(directory):
        dirs[:] = [d for d in dirs if d not in ('bin', 'obj')]  # pruned, not listed: obj trees are large
        files += [Path(path, name) for name in names]
    return files


def project_references(root, directory):
    """Names of the src projects a project directory's .csproj and .props files reference (ProjectReference)."""
    names = []
    for p in build_files(directory):
        if p.suffix.lower() in ('.csproj', '.props'):
            for include in PROJECT_REFERENCE.findall(p.read_text(encoding='utf-8', errors='replace')):
                # MSBuild resolves an imported .props' relative paths against the importing project.
                target = (directory / include.replace('\\', os.sep)).resolve().parent
                if target.parent == (root / 'src').resolve():
                    names.append(target.name)
    return names


def build_closure(root, projects):
    """The src projects the compared projects build: each one, plus the src projects its .csproj or .props
    references, transitively. Utl's dependents build its mixed DLL through their Mirror.props reference (D19)."""
    seen, resolved_seen, todo = [], set(), list(projects)
    while todo:
        directory = root / 'src' / todo.pop()
        resolved = directory.resolve()
        if resolved in resolved_seen or not resolved.is_dir():
            continue
        resolved_seen.add(resolved)
        seen.append(directory)
        todo += project_references(root, directory)
    return seen


def reads_install(root, projects, vcxproj=None):
    """D19: build inputs of every project the compared projects build (build_closure), and of the vcxproj's folder,
    that name the Mathcad install, plus the Directory.Build files MSBuild imports for them. Best effort; the .ilnew
    check after the build is the backstop."""
    dirs = build_closure(root, projects) + ([vcxproj.parent] if vcxproj else [])
    files = {p for d in dirs if d.is_dir() for p in build_files(d) if p.suffix.lower() in BUILD_INPUTS}
    for d in dirs:
        for parent in [d, *d.relative_to(root).parents]:
            files |= {f for f in (root / parent).glob('Directory.Build.*') if f.is_file()}
    return sorted(str(p.relative_to(root)) for p in files
                  if INSTALL_REFERENCE.search(p.read_text(encoding='utf-8', errors='replace')))


PROJECT_REFERENCE = re.compile(r"""<ProjectReference\b[^>]*\bInclude=["']([^"']+)["']""", re.IGNORECASE)


def builds_cppcli(root, projects):
    """Whether building the projects builds a .vcxproj: one of them, or a src project they reference through
    their .csproj/.props, holds one. Utl's dependents build its mixed DLL and need the pinned MSVC too."""
    return any(p.suffix.lower() == '.vcxproj' for d in build_closure(root, projects) for p in build_files(d))


def section_names(dll):
    """PE section names; an IL-patching post-build step leaves its own section (D19)."""
    data = Path(dll).read_bytes()
    pe = int.from_bytes(data[0x3c:0x40], 'little')
    count = int.from_bytes(data[pe + 6:pe + 8], 'little')
    table = pe + 24 + int.from_bytes(data[pe + 20:pe + 22], 'little')
    return [data[table + 40 * i:table + 40 * i + 8].rstrip(b'\0').decode('ascii', 'replace') for i in range(count)]


def build_and_scan(root, projects, tool, work, install, dotnet, log, timeout, vcxproj=None, keep=None):
    """Build the projects in Release and scan them with MethodAudit; keep is where a failed scan's inventory goes."""
    assemblies = work / 'assemblies'
    if found := reads_install(root, projects, vcxproj):
        raise RuntimeError(f'builds must not read the Mathcad install (D19): {", ".join(found)}')
    env, hidden = without_install(work)
    # C++/CLI builds use the pinned MSVC; the production .csproj receives it as QuireMsvcInstallDir.
    if vcxproj or builds_cppcli(root, projects):
        env['QuireMsvcInstallDir'] = f'{msvc_instance()}{os.sep}'
    if vcxproj:
        # OutDir/IntDir keep outputs out of the tree; MethodAudit then requires <project>.dll there (the TargetName).
        run([msbuild(), vcxproj, '-t:Rebuild', '-p:Configuration=Release', '-p:Platform=x64', hidden,
             f'-p:OutDir={assemblies}{os.sep}', f'-p:IntDir={work / "obj"}{os.sep}', '-nologo', '-v:m'],
            root, log, timeout, env)
    targets = [] if vcxproj else [root / 'src' / project / f'{project}.csproj' for project in projects]
    if missing := [t for t in targets if not t.is_file()]:
        raise RuntimeError(f'production project missing from comparison: {missing[0]}')
    if targets:
        # One clean (--no-incremental) build of every project and its references, into one directory: a separate
        # build per project rebuilt the shared reference closure each time, with the same outputs.
        solution = work / 'build.slnx'
        solution.parent.mkdir(parents=True, exist_ok=True)
        solution.write_text('<Solution>\n' + ''.join(f'  <Project Path={quoteattr(str(t.resolve()))} />\n' for t in targets)
                            + '</Solution>\n', encoding='utf-8')
        run([dotnet, 'build', solution, '-c', 'Release', '--no-incremental', '-o', assemblies, hidden,
             '-nologo', '-v:q', '-p:EnableSourceControlManagerQueries=false'], root, log, timeout, env)
    for project in projects:
        if '.ilnew' in section_names(assemblies / f'{project}.dll'):
            raise RuntimeError(f'{project}.dll has an IL-patching .ilnew section (D19)')
        if reason := msvc_mismatch(assemblies / f'{project}.dll'):
            raise RuntimeError(f'{project}.dll: {reason}')
    inventory = work / 'inventory.json'
    try:
        run([dotnet, tool, '--repo', root, '--assemblies', assemblies, '--install', install,
             '--output', inventory, '--release-il',
             *(arg for p in projects for arg in ('--project', p))], root, log, timeout)
        failure = None
    except RuntimeError as error:
        failure = error
    try:
        data = json.loads(inventory.read_text(encoding='utf-8'))
    except (OSError, ValueError):
        data = {}
    errors = data.get('errors') or []
    if failure or errors or 'methods' not in data:
        # The work directory is deleted with the run; keep what MethodAudit wrote for diagnosis.
        if inventory.is_file() and keep:
            shutil.copyfile(inventory, keep)
        detail = '; '.join(errors[:20]) + (f'; {len(errors) - 20} more in {keep}' if len(errors) > 20 else '')
        raise RuntimeError('MethodAudit failed: ' + '\n'.join(map(str, filter(None, (detail, failure))))
                           if detail or failure else 'MethodAudit wrote no inventory')
    return data


CACHE = ROOT / '.mirror-cache' / 'inventory-cache'
CACHE_FORMAT = 1
# The judge's sources: MethodAudit compiles MirrorGen's metadata model.
TOOL_TREES = ('tools/Quire.Tools.MethodAudit', 'tools/Quire.Tools.MirrorGen')


def inventory_key(root, commit, tool_commit, projects, install):
    """Identity of a committed tree's inventory: its sources (the commit: type map, SDK pin and build files included),
    the projects, the judge built from tool_commit's tool trees, and the install MethodAudit reads (path and every
    DLL's size and mtime; hashing the DLLs would cost more than it saves)."""
    tools = [git('rev-parse', f'{tool_commit}:{tree}', cwd=root) for tree in TOOL_TREES]
    install = Path(install)
    dlls = sorted((str(p.relative_to(install)), s.st_size, s.st_mtime_ns) for p in install.rglob('*.dll') for s in [p.stat()])
    payload = json.dumps([CACHE_FORMAT, commit, sorted(projects), tools, str(install), dlls])
    return hashlib.sha256(payload.encode('utf-8')).hexdigest()


def cached_inventory(cache, key):
    """A stored inventory, or None when it is missing or unreadable (the caller then builds)."""
    try:
        data = json.loads((cache / f'{key}.json').read_text(encoding='utf-8'))
    except (OSError, ValueError):
        return None
    return data if isinstance(data, dict) and isinstance(data.get('methods'), list) and not data.get('errors') else None


def store_inventory(cache, key, data):
    """Only inventories of committed trees: never a working tree, which the key cannot identify. Best effort: another
    lane reading or writing the same entry (Windows refuses to replace an open file) only costs a later rebuild."""
    temp = cache / f'{key}.{uuid.uuid4().hex}.tmp'
    try:
        cache.mkdir(parents=True, exist_ok=True)
        temp.write_text(json.dumps(data) + '\n', encoding='utf-8')
        os.replace(temp, cache / f'{key}.json')
    except OSError:
        with contextlib.suppress(OSError):
            temp.unlink(missing_ok=True)


def check(base, projects, results, types=(), methods=(), dotnet='dotnet', timeout=1800,
          root=ROOT, install=None, vcxproj=None, cache=None, store_candidate=False):
    """Compare base and candidate (root's working tree). The judge, MethodAudit, is built from the base commit so a
    candidate cannot change what judges it. Base inventories are cached by inventory_key; store_candidate also caches a
    passing candidate under its HEAD commit, for a root whose tree is exactly that commit (il_merge's clean clone)."""
    install = Path(install or mathcad_install())
    cache = cache or CACHE
    results.mkdir(parents=True, exist_ok=False)
    log = results / 'build.log'
    base = git('rev-parse', '--verify', f'{base}^{{commit}}', cwd=root)
    report = {'base': base, 'tip': git('rev-parse', 'HEAD', cwd=root),
              'working_tree': git('status', '--porcelain', cwd=root), 'projects': projects,
              'types': list(types), 'methods': list(methods), 'passed': False,
              'candidate': str(vcxproj) if vcxproj else 'csproj'}
    try:
        if not projects:
            if types or methods:
                raise RuntimeError('member selection requires a production project')
            report.update(passed=True, checked=[], failures=[], reason='no production project changed')
            return report
        with tempfile.TemporaryDirectory(prefix='quire-release-il-') as temp:
            work = Path(temp)
            original = work / 'base'
            run(['git', 'clone', '--quiet', '--shared', '--no-checkout', root, original], root, log, timeout)
            run(['git', '-c', 'core.hooksPath=', 'checkout', '--quiet', '--detach', base], original, log, timeout)
            # Fail closed: a base without the judge cannot judge.
            audit = original / 'tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj'
            if not audit.is_file():
                raise RuntimeError(f'base {base[:12]} has no MethodAudit to judge with ({audit.relative_to(original)})')
            tool_dir = work / 'tool'
            run([dotnet, 'build', audit, '-c', 'Release', '-o', tool_dir, '-nologo', '-v:q'], original, log, timeout)
            tool = tool_dir / 'Quire.Tools.MethodAudit.dll'
            # A project the candidate adds has no base build; its members start as new (or moved) rows.
            existing = [p for p in projects if (original / 'src' / p / f'{p}.csproj').is_file()]
            key = inventory_key(root, base, base, existing, install) if existing else None
            before = cached_inventory(cache, key) if key else {'methods': []}
            report['base_cached'] = bool(key) and before is not None
            if before is None:
                print('Building base Release assemblies', flush=True)
                before = build_and_scan(original, existing, tool, work / 'before', install, dotnet, log, timeout,
                                        keep=results / 'base-inventory.json')
                store_inventory(cache, key, before)
            else:
                print(f'Base inventory from cache ({key[:12]})', flush=True)
            print('Building candidate Release assemblies', flush=True)
            after = build_and_scan(root, projects, tool, work / 'after', install, dotnet, log, timeout, vcxproj,
                                   keep=results / 'inventory.json')
            # The same reference artifact must have been read by both scans.
            identities = lambda data: {m['original']['assembly']: (m['original']['sha256'], m['original']['mvid'])
                                       for m in data['methods'] if m.get('original')}
            old_id, new_id = identities(before), identities(after)
            # A .vcxproj candidate may pair nothing with the C# base's original; its methods then fail on their own.
            if any((vcxproj is None or name in new_id) and new_id.get(name) != identity for name, identity in old_id.items()):
                raise RuntimeError('Mathcad reference identity changed between scans')
            report.update(compare(before, after, types, methods))
            report['originals'] = new_id
            if store_candidate and report['passed'] and not vcxproj and not report['working_tree']:
                # Judged by the base's tool, so keyed by it: if the candidate changes the tool, its key never hits.
                store_inventory(cache, inventory_key(root, report['tip'], base, projects, install), after)
            # Kept locally for exact signatures, tokens and artifact identities; no original IL.
            (results / 'base-inventory.json').write_text(json.dumps(before, indent=2) + '\n', encoding='utf-8')
            (results / 'inventory.json').write_text(json.dumps(after, indent=2) + '\n', encoding='utf-8')
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
        report['error'] = str(error)
    finally:
        (results / 'release-il.json').write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    return report


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--base', default='main')
    parser.add_argument('--project', action='append', default=[])
    parser.add_argument('--type', action='append', default=[])
    parser.add_argument('--method', action='append', default=[])
    parser.add_argument('--results', type=Path)
    parser.add_argument('--dotnet', default='dotnet')
    parser.add_argument('--timeout', type=int, default=1800)
    parser.add_argument('--candidate-vcxproj', type=Path,
                        help='build the candidate of the single --project from this C++/CLI .vcxproj')
    args = parser.parse_args(argv)
    vcxproj = args.candidate_vcxproj and args.candidate_vcxproj.resolve()
    if vcxproj and (len(args.project) != 1 or vcxproj.suffix != '.vcxproj' or not vcxproj.is_file()
                    or not vcxproj.is_relative_to(ROOT)):
        parser.error('--candidate-vcxproj needs an existing .vcxproj in this repository and exactly one --project')
    projects = sorted(set(args.project) if args.project else set(affected_projects(args.base)))
    unknown = set(projects) - production_projects()
    if unknown:
        parser.error(f'unknown production projects: {sorted(unknown)}')
    results = args.results or ROOT / '.mirror-cache' / ('release-il-' + uuid.uuid4().hex[:12])
    report = check(args.base, projects, results, args.type, args.method, args.dotnet, args.timeout, vcxproj=vcxproj)
    for failure in report.get('failures', [])[:20]:
        print(f"  {failure['method']}: {failure['reason']}")
    if len(report.get('failures', [])) > 20:
        print(f"  {len(report['failures']) - 20} more mismatches in the report")
    if report.get('error'):
        print(report['error'], file=sys.stderr)
    print(f"Release IL: {'PASS' if report['passed'] else 'FAIL'}; {len(report.get('checked', []))} checked; {results / 'release-il.json'}")
    return 0 if report['passed'] else 1


if __name__ == '__main__':
    sys.exit(main())
