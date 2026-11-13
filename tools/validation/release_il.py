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
is native are compared by their machine code (native_bodies); every other method follows the usual rules.
"""
import argparse
import contextlib
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import struct
import subprocess
import sys
import tarfile
import tempfile
import time
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


def native_body(row):
    """A mixed-mode member whose RVA holds machine code, which MethodAudit reports as having no IL to compare."""
    return bool(row.get('mixed_mode') and (row.get('unsupported') or '').startswith('native code'))


def native_pair(row):
    """A mixed-mode native body whose Mathcad counterpart is native too: no IL on either side (D10)."""
    return bool(native_body(row) and (row.get('original') or {}).get('native_code') is True
                and row.get('shape_matches') is True)


def stub(row):
    """Generated by MirrorGen: a MirrorStub throw, or a member carrying [MirrorStub]."""
    return bool(row.get('stub_body') or row.get('mirror_stub'))


def compare(before, after, types=(), methods=()):
    """Changed bodies/declarations and every previously exact method must match.

    Unmapped changed methods are blockers, including generated helpers. Unchanged
    unfinished methods stay out of the batch. D10 engine-native methods are identified
    by MethodAudit from their original artifact and do not require a managed IL match.
    A generated stub replaced by another generated stub is unchanged, as is a new generated
    stub declared as its Mathcad counterpart, and removing a never-exact generated stub
    (hand-written code now provides the member) does not fail;
    implemented members and previously exact methods are judged as before.
    Removing a compiler-generated member with no Mathcad counterpart (a renumbered
    closure or state machine) does not fail the gate. Nor does changing or removing an
    existing product-only member (D21); a new member without a counterpart still fails.
    A native pair has no IL. Where its machine code was compared (native_bodies), a new or
    changed body, and one that matched before, must match the Mathcad body.
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
                                         ('body_sha256', 'shape_matches', 'declaration_only', 'native_sha256'))
        # A generated stub is unfinished wherever it sits and however MirrorGen spells it (the file a
        # MirrorStub throw records, a constructor stub's chaining); replacing one stub with another is not a change.
        # [MirrorStub] counts: MirrorGen re-spells a constructor stub's chaining, and only generated code carries it.
        if changed and previous is not None and stub(previous) and stub(row) \
                and previous.get('shape_matches') == row.get('shape_matches'):
            changed = False
        # A stub regeneration adds (a member that became mappable) is unfinished too, once declared as Mathcad's.
        if changed and previous is None and stub(row) and row.get('original') is not None \
                and row.get('shape_matches') is True:
            changed = False
        if not (changed or row.get('unsupported') or selected(row, types, methods)
                or previous and previous.get('exact_match') is True):
            continue
        checked.append(key)
        # A native pair's machine code follows the rule for IL: an unchanged body that never matched stays out.
        if native_pair(row) and row.get('native_exact') is False and (
                changed or selected(row, types, methods) or previous.get('native_exact') is True):
            failures.append({'method': key, 'reason': 'native body differs: ' + row.get('native_reason', 'not compared')})
            continue
        # A member that was exact, or that lost its counterpart, keeps its obligations.
        # A native pair that was exact in a mixed-mode base keeps the obligation only if it stopped being exact: its
        # exact_match covers the import record, which is what a native pair has in place of an IL body.
        if (toolset_support(row) or native_pair(row)) and not (
                previous and (previous.get('exact_match') is True
                              and not (native_pair(previous) and row.get('exact_match') is True)
                              or previous.get('original') is not None and row.get('original') is None)):
            continue
        if row.get('unsupported'):
            # An unpaired native body: native_bodies says how it compares with Mathcad's member of the same name.
            reason = (row.get('native_reason') or row['unsupported'] if not native_pair(row)
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
        removed = old[key]
        if any(row.get('project') != removed.get('project')
               and row.get('type') == removed.get('type')
               and row.get('signature') == removed.get('signature')
               and row.get('shape_matches') is True
               and (row.get('exact_match') is True or row.get('declaration_only') is True)
               for row in new.values()):
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
            'native_exact': sum(native_pair(m) and m.get('native_exact') is True for m in new.values()),
            'unfinished': sum(m.get('exact_match') is not True and not m.get('declaration_only')
                              and not m.get('engine_native') and not toolset_support(m) and not native_pair(m)
                              for m in new.values())}


# Native bodies of a mixed-mode assembly (D19). MethodAudit pairs them and compares their declarations and import
# records. Their machine code is compared here, as native_linked.py compares the exports of a linked native DLL (D30).
NATIVE_RULES = [Path(__file__).with_name(name) for name in ('native_bytes.py', 'native_linked.py')]


def method_defs(image):
    """([(RVA, holds native code, name)] of a managed image's MethodDef rows, how many of the first rows are <Module>'s).
    Row n is token 0x06000001 + n. `image` is a native_bytes.PeImage; only the tables up to MethodDef are sized
    (ECMA-335 II.22 and II.24)."""
    from native_bytes import _rva_offset
    d = image.data
    u16, u32 = (lambda at: struct.unpack_from('<H', d, at)[0]), (lambda at: struct.unpack_from('<I', d, at)[0])
    cli = u32(u32(0x3C) + 24 + 112 + 14 * 8)
    if not cli:
        raise ValueError(f'{image.name}: not a managed image')
    root = _rva_offset(image.sections, u32(_rva_offset(image.sections, cli) + 8))
    header = root + 16 + u32(root + 12)
    streams, entry = {}, header + 4
    for _ in range(u16(header + 2)):
        end = d.index(b'\0', entry + 8)
        streams[d[entry + 8:end]] = root + u32(entry)
        entry += 8 + ((end - entry - 8 + 4) & ~3)  # the name is padded to four bytes
    if b'#~' not in streams or b'#Strings' not in streams:
        raise ValueError(f'{image.name}: no optimized metadata tables')
    tables, strings = streams[b'#~'], streams[b'#Strings']
    rows, cursor = {}, tables + 24
    for table in range(64):
        if struct.unpack_from('<Q', d, tables + 8)[0] >> table & 1:
            rows[table] = u32(cursor)
            cursor += 4
    # An index is four bytes wide once a table it can name has too many rows for two (less the tag bits of a coded one).
    wide = lambda tag_bits, *named: 4 if max(rows.get(t, 0) for t in named) >= 1 << (16 - tag_bits) else 2
    string, guid, blob = (4 if d[tables + 6] & bit else 2 for bit in (1, 2, 4))
    # Module, TypeRef, TypeDef, FieldPtr, Field, MethodPtr: the tables before MethodDef.
    sizes = [2 + string + 3 * guid, wide(2, 0, 0x1A, 0x23, 1) + 2 * string,
             4 + 2 * string + wide(2, 2, 1, 0x1B) + wide(0, 4) + wide(0, 6), wide(0, 4), 2 + string + blob, wide(0, 6)]
    starts = [cursor + sum(size * rows.get(table, 0) for table, size in enumerate(sizes[:n])) for n in range(7)]
    index = lambda at, width: u32(at) if width == 4 else u16(at)
    methods = []
    for row in range(rows.get(6, 0)):
        at = starts[6] + row * (8 + string + blob + wide(0, 8))
        rva, implementation = u32(at), u16(at + 4)
        name = strings + index(at + 8, string)
        # Code type other than IL, or unmanaged (MethodImplAttributes), as MethodAudit decides NativeCode.
        methods.append((rva, bool(rva and implementation & 7), d[name:d.index(b'\0', name)].decode('utf-8', 'replace')))
    # <Module> is the first TypeDef; its methods end where the second type's MethodList begins.
    second = starts[2] + 2 * sizes[2] - wide(0, 6)
    return methods, index(second, wide(0, 6)) - 1 if rows.get(2, 0) > 1 else len(methods)


def symbol_free(dll, directory):
    """A copy of a candidate DLL with its debug directory entry cleared. dumpbin loads the PDB that entry names and then
    prints symbols where the comparison reads addresses. Only a candidate is copied, never an original (D15)."""
    data = bytearray(Path(dll).read_bytes())
    entry = struct.unpack_from('<I', data, 0x3C)[0] + 24 + 112 + 6 * 8
    data[entry:entry + 8] = bytes(8)
    directory.mkdir(parents=True, exist_ok=True)
    copy = directory / Path(dll).name
    copy.write_bytes(data)
    return copy


class NativeImage:
    """A mixed-mode image as the native comparison reads it: its linked code and its method table."""

    def __init__(self, dll, map_text=None, tool=None):
        from native_linked import Linked
        self.linked = Linked.load(dll, map_text, tool)
        self.methods, self.globals = method_defs(self.linked.image)
        # A leaf function (one without .pdata) ends where the next known thing starts: a method's native or IL body, or a
        # symbol of the linker map.
        self.starts = sorted({rva for rva, _, _ in self.methods if rva} | set(self.linked.names))

    def rva(self, token):
        if token >> 24 != 6 or not 0 < token & 0xFFFFFF <= len(self.methods):
            raise ValueError(f'{self.linked.image.name} has no method {token:#010x}')
        return self.methods[(token & 0xFFFFFF) - 1][0]

    def extent(self, rva):
        return self.linked.image.function_extent(rva, self.starts)

    def fingerprint(self, rva):
        """SHA-256 of the body at `rva` with each image address replaced by what it names (a place in the body, an
        import, or anything else), so a rebuild that only moves code keeps it."""
        from native_linked import ADDRESS, Comparison
        text = Comparison.imported(self.linked, rva)
        if text is None:
            extent = self.extent(rva)
            body = extent and self.linked.body(*extent)
            if body is None:
                raise ValueError(f'RVA {rva:#x} of {self.linked.image.name} does not start a function')

            def named(match):
                address = self.linked.address(match.group(1))
                if address is None:
                    return match.group(0)
                return f'+{address - extent[0]:#x}' if extent[0] <= address < extent[1] \
                    else Comparison.imported(self.linked, address) or 'X'
            text = '\n'.join(f'{mnemonic} {ADDRESS.sub(named, operands)}' for _, _, mnemonic, operands in body)
        return hashlib.sha256(text.encode('utf-8')).hexdigest()


def compare_native(original, candidate, original_rva, rva):
    """(exact, reason) of a candidate's native body against the original's, under native_linked.py's rules. A body that
    is an import thunk (`jmp [slot]`) matches a thunk of the same imported name. Any other body pairs instruction by
    instruction, and each address is checked by what it refers to. An unverified target is not a match."""
    from native_linked import Comparison
    imported = Comparison.imported(original.linked, original_rva), Comparison.imported(candidate.linked, rva)
    if any(imported):
        same = imported[0] == imported[1]
        return same, f'import {imported[1]}' if same else f'import {imported[1]} != original {imported[0]}'
    extents = original.extent(original_rva), candidate.extent(rva)
    if None in extents:
        return False, f"the {'original' if extents[0] is None else 'candidate'} body does not start a function"
    status, reason, _ = Comparison(original.linked, candidate.linked, {}).instructions(*extents)
    return status == 'ok', reason or 'instructions and targets match'


def install_file(install, assembly):
    """The one DLL of the install named `assembly`, in any case, as MethodAudit finds an original."""
    found = [p for p in Path(install).glob('*.dll') if p.stem.lower() == assembly.lower()]
    if len(found) != 1:
        raise ValueError(f"{'several' if found else 'no'} {assembly}.dll in {install}")
    return found[0]


def native_bodies(data, assemblies, install, work, load=None):
    """Compare the machine code of an inventory's native bodies with Mathcad's and record the verdict on each row:
    native_exact and native_reason for a body MethodAudit paired with a native counterpart, and native_sha256
    (NativeImage.fingerprint), which tells a changed body from an unchanged one. A row that cannot be compared is not
    exact, with the reason. A global function MethodAudit left unpaired although Mathcad has one of its name gets only a
    native_reason: how its body compares with that member's, which tells a wrong body from a missing pairing.
    `load` reads a DLL as a NativeImage. A candidate is read from a symbol-free copy under `work`, with the linker map
    beside it (<name>.map), which names the data its bodies refer to; without one such a target is unverified."""
    rows = [m for m in data['methods'] if native_body(m)]
    if not rows:
        return
    if load is None:
        from native_linked import dumpbin
        tool = dumpbin()
        load = lambda dll, map_text: NativeImage(dll, map_text, tool)
    failures = (OSError, ValueError, struct.error, RuntimeError, subprocess.SubprocessError)
    images = {}

    def image(path, candidate=False):
        if path not in images:
            try:
                listing = path.with_suffix('.map')
                images[path] = load(symbol_free(path, Path(work) / 'native'),
                                    listing.read_text(encoding='utf-8', errors='replace') if listing.is_file() else None) \
                    if candidate else load(path, None)
            except failures as error:
                images[path] = ValueError(f'{path.name} unreadable: {error}')
        if isinstance(images[path], Exception):
            raise images[path]
        return images[path]
    # <Module> has no binding of its own: its functions pair with the Mathcad assembly the project's other globals name.
    globals_of = {}
    for m in data['methods']:
        if m['type'] == '<Module>' and m.get('original'):
            globals_of.setdefault(m.get('project'), set()).add(m['original']['assembly'])
    for row in rows:
        try:
            candidate = image(Path(assemblies) / f"{row['project']}.dll", candidate=True)
            rva = candidate.rva(row['token'])
            row['native_sha256'] = candidate.fingerprint(rva)
            counterpart, named = row.get('original'), globals_of.get(row['project'], ())
            if counterpart and counterpart.get('native_code'):
                original = image(install_file(install, counterpart['assembly']))
                row['native_exact'], row['native_reason'] = compare_native(
                    original, candidate, original.rva(counterpart['token']), rva)
            elif counterpart is None and row['type'] == '<Module>' and row.get('original_name_taken') and len(named) == 1:
                assembly, name = next(iter(named)), candidate.methods[(row['token'] & 0xFFFFFF) - 1][2]
                original = image(install_file(install, assembly))
                same_name = [(token, other) for token, (other, native, other_name)
                             in enumerate(original.methods[:original.globals], 0x06000001) if native and other_name == name]
                verdicts = [(token, *compare_native(original, candidate, other, rva)) for token, other in same_name]
                # Overloads share the name: the one the body equals, else the first.
                for token, exact, reason in sorted(verdicts, key=lambda verdict: not verdict[1])[:1]:
                    relation = 'equals' if exact else 'differs from'
                    row['native_reason'] = f'no Mathcad counterpart paired; the body {relation} {assembly} {token:#010x} ({reason})'
        except failures as error:
            row['native_exact'], row['native_reason'] = False, f'not compared: {error}'


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


def build_files(directory):
    """Files of a project directory that a build can read, outside bin and obj."""
    files = []
    for path, dirs, names in os.walk(directory):
        dirs[:] = [d for d in dirs if d not in ('bin', 'obj')]  # pruned, not listed: obj trees are large
        files += [Path(path, name) for name in names]
    return files


def project_references(root, directory):
    """Names of the src projects a project directory's .csproj, .props and .targets files reference (ProjectReference)."""
    names = []
    for p in build_files(directory):
        if p.suffix.lower() in ('.csproj', '.props', '.targets'):
            for include in PROJECT_REFERENCE.findall(p.read_text(encoding='utf-8', errors='replace')):
                # MSBuild resolves an imported .props' relative paths against the importing project.
                target = (directory / include.replace('\\', os.sep)).resolve().parent
                if target.parent == (root / 'src').resolve():
                    names.append(target.name)
    return names


def build_closure(root, projects):
    """The src projects the compared projects build: each one, plus the src projects its .csproj, .props or .targets
    references, transitively. Utl's dependents build it through their Mirror.props reference (D19): its mixed DLL when
    it is a build target, else (a reference built in Debug) its IL-only C# stand-in."""
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


PROJECT_REFERENCE = re.compile(r"""<ProjectReference\b[^>]*\bInclude=["']([^"']+)["']""", re.IGNORECASE)
BUILD_PATH = re.compile(r"""<(\w+)\b[^>]*?\b(?:Include|Project)=["']([^"']+)["']""", re.IGNORECASE)
# Elements whose Include/Project names a file the build reads (MirrorWholeType is compiled by Directory.Build.targets).
BUILD_INPUTS = {'import', 'compile', 'none', 'content', 'embeddedresource', 'page', 'resource', 'applicationdefinition',
                'additionalfiles', 'clcompile', 'clinclude', 'resourcecompile', 'mirrorwholetype'}


def outside_inputs(directory):
    """Whether a project directory's build files read a file outside it other than through ProjectReference (an Import or
    a linked item), which its key would miss. The C++ toolset's own imports ($(VCTargetsPath), the pinned MSVC) are not
    repository files; any other property in such a path could point anywhere, so it counts as outside."""
    for p in build_files(directory):
        if p.suffix.lower() in ('.csproj', '.props', '.targets', '.vcxproj'):
            for element, value in BUILD_PATH.findall(p.read_text(encoding='utf-8', errors='replace')):
                if element.lower() not in BUILD_INPUTS:
                    continue
                for path in filter(None, (part.strip() for part in value.split(';'))):
                    if path.startswith('$(VCTargetsPath)'):
                        continue
                    if '$(' in path or '%(' in path or '@(' in path:
                        return True
                    # Import paths resolve against the importing file, item paths against the project.
                    base = p.parent if element.lower() == 'import' else directory
                    if not (base / path.replace('\\', os.sep)).resolve().is_relative_to(directory.resolve()):
                        return True
    return False


def builds_cppcli(root, projects):
    """Whether building the projects builds a .vcxproj: one of them, or a src project they reference through
    their .csproj/.props/.targets, holds one. Utl's dependents build Utl and need the pinned MSVC too (its mixed DLL
    only when Utl is a build target; as a reference it is built in Debug, its IL-only C# stand-in)."""
    return any(p.suffix.lower() == '.vcxproj' for d in build_closure(root, projects) for p in build_files(d))


@contextlib.contextmanager
def timed(timings, name):
    """Record elapsed wall time, including a step that raises."""
    start = time.perf_counter()
    try:
        yield
    finally:
        timings[name] = time.perf_counter() - start


def build_and_scan(root, projects, tool, work, install, dotnet, log, timeout, vcxproj=None, keep=None,
                   timings=None, prefix='candidate'):
    """Build the projects in Release and scan them with MethodAudit; keep is where a failed scan's inventory goes."""
    timings = timings if timings is not None else {}
    with timed(timings, prefix + '_build'):
        assemblies = work / 'assemblies'
        env = dict(os.environ)
        # C++/CLI builds use the pinned MSVC; the production .csproj receives it as QuireMsvcInstallDir.
        if vcxproj or builds_cppcli(root, projects):
            env['QuireMsvcInstallDir'] = f'{msvc_instance()}{os.sep}'
            # A linker map beside each linked DLL (<name>.map) names the data its native bodies refer to (native_bodies).
            env['_LINK_'] = f"{env.get('_LINK_', '')} /MAP".strip()
        if vcxproj:
            # OutDir/IntDir keep outputs out of the tree; MethodAudit then requires <project>.dll there (the TargetName).
            run([msbuild(), vcxproj, '-t:Rebuild', '-p:Configuration=Release', '-p:Platform=x64',
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
            run([dotnet, 'build', solution, '-c', 'Release', '--no-incremental', '-o', assemblies,
                 '-nologo', '-v:q', '-p:EnableSourceControlManagerQueries=false'], root, log, timeout, env)
        for project in projects:
            if reason := msvc_mismatch(assemblies / f'{project}.dll'):
                raise RuntimeError(f'{project}.dll: {reason}')
    inventory = work / 'inventory.json'
    try:
        with timed(timings, prefix + '_scan'):
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
    native_bodies(data, assemblies, install, work)
    return data


CACHE = ROOT / '.mirror-cache' / 'inventory-cache'
CACHE_FORMAT = 4
# A run's directory is older than this only if its run was killed: every step is bounded by --timeout.
STALE_WORK = 12 * 3600
PURGE = '''import os, shutil, stat, sys
from pathlib import Path
def writable(function, path, error):  # Git marks object files read-only; a file still open stays for a later purge
    try:
        os.chmod(path, stat.S_IWRITE)
        function(path)
    except OSError:
        pass
for path in map(Path, sys.argv[1:]):
    for entry in (list(path.iterdir()) if path.name == '.trash' and path.is_dir() else [path]):
        try:
            shutil.rmtree(entry, onexc=writable)
        except OSError:  # still in use: the next run's purge retries what is in .trash
            pass
'''


def purge(trash, *paths):
    """Delete trash's entries and paths in a detached process that outlives the run. It runs in trash's parent, not the
    caller's directory: Windows cannot remove a directory that is some process's current directory (a lane worktree)."""
    detached = {'creationflags': subprocess.DETACHED_PROCESS | subprocess.CREATE_NEW_PROCESS_GROUP} if os.name == 'nt' \
        else {'start_new_session': True}
    subprocess.Popen([sys.executable, '-I', '-c', PURGE, trash, *paths], cwd=trash.parent, stdin=subprocess.DEVNULL,
                     stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, close_fds=True, **detached)


@contextlib.contextmanager
def work_directory(parent):
    """A run's work directory (base checkout, both builds), discarded when the run ends. Deleting thousands of build
    files would hold up the gate, so the directory is renamed into parent/.trash, which is instant, and purged in the
    background. Directories a killed run left behind go the same way once older than STALE_WORK."""
    parent.mkdir(parents=True, exist_ok=True)
    work = Path(tempfile.mkdtemp(prefix='quire-release-il-', dir=parent))
    try:
        yield work
    finally:
        trash, now, unmoved = parent / '.trash', time.time(), []
        for entry in [work, *(other for other in parent.glob('quire-release-il-*') if other != work)]:
            try:
                if entry != work and now - entry.stat().st_mtime <= STALE_WORK:
                    continue  # another run's
                trash.mkdir(exist_ok=True)
                os.replace(entry, trash / entry.name)
            except FileNotFoundError:  # moved or purged by a concurrent run
                pass
            except OSError:  # a file still open in it: delete this run's where it is; another's waits for a later sweep
                if entry == work:
                    unmoved.append(entry)
        with contextlib.suppress(OSError):
            purge(trash, *unmoved)
# The judge's sources: MethodAudit compiles MirrorGen's metadata model.
TOOL_TREES = ('tools/Quire.Tools.MethodAudit', 'tools/Quire.Tools.MirrorGen')
# A clone's .git/info/attributes that makes checkout and archive write committed bytes (no EOL conversion, filter or
# keyword expansion, whatever core.autocrlf says): base and candidate trees are built from the bytes their keys name.
RAW_BYTES = '* -export-ignore -export-subst -text -ident -filter -working-tree-encoding\n'


def checkout_base(root, base, original, log, timeout):
    """Materialize committed bytes without checkout filters or per-file Git checkout work."""
    run(['git', 'clone', '--quiet', '--shared', '--no-checkout', root, original], root, log, timeout)
    # info/attributes takes precedence over committed export attributes. Archive must include every blob unchanged.
    (original / '.git/info/attributes').write_text(RAW_BYTES, encoding='utf-8')
    archive = original.parent / 'base.tar'
    run(['git', 'archive', '--format=tar', f'--output={archive}', base], original, log, timeout)
    if shutil.which('tar'):
        run(['tar', '-xf', archive, '-C', original], original, log, timeout)
    else:
        with tarfile.open(archive) as source:
            source.extractall(original, filter='data')
    # Builds need the detached commit and its files, not a populated staging index.
    (original / '.git/HEAD').write_text(base + '\n', encoding='ascii')


def judge_key(root, base, sdk):
    # Root build configuration and SDK pin also affect compilation, besides the two source trees.
    inputs = [git('rev-parse', f'{base}:{tree}', cwd=root) for tree in TOOL_TREES]
    inputs.extend(root_files(root, base))
    return hashlib.sha256(json.dumps([1, inputs, sdk]).encode('utf-8')).hexdigest()


def build_judge(original, base, cache, dotnet, log, timeout):
    sdk = subprocess.check_output([dotnet, '--version'], cwd=original, text=True, timeout=timeout).strip()
    key = judge_key(original, base, sdk)
    entry = cache / 'judges' / key
    marker = entry / 'ready.json'
    try:
        output = entry / json.loads(marker.read_text(encoding='utf-8'))['output']
        if (output / 'Quire.Tools.MethodAudit.dll').is_file():
            return output / 'Quire.Tools.MethodAudit.dll'
    except (OSError, ValueError, KeyError, TypeError):
        pass
    output = entry / uuid.uuid4().hex
    output.mkdir(parents=True)
    audit = original / 'tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj'
    run([dotnet, 'build', audit, '-c', 'Release', '-o', output, '-nologo', '-v:q'], original, log, timeout)
    if (output / 'Quire.Tools.MethodAudit.dll').is_file():
        pending = entry / (uuid.uuid4().hex + '.json')
        pending.write_text(json.dumps({'output': output.name}), encoding='utf-8')
        os.replace(pending, marker)
    return output / 'Quire.Tools.MethodAudit.dll'


def root_files(root, commit):
    """The root files a build or the judge can read: every committed root file but prose (*.md)."""
    return [line for line in git('ls-tree', commit, cwd=root).splitlines() if ' blob ' in line and not line.endswith('.md')]


def slice_keys(root, commit, judge, projects, install):
    """Cache key of each project's rows in an inventory of root (a clean tree of commit with committed bytes, RAW_BYTES)
    scanned with exactly these projects by judge (build_judge's key: the judge's sources, root files and SDK).
    MethodAudit derives a project's rows from its assembly and those it references (its ProjectReference closure), the
    type map, the src directory names and the install, so the key is their content, not the commit or the other projects:
    a lane's base hits on rows another lane stored for other projects. One exception is keyed: which mixed-mode (C++/CLI)
    closure members were scanned with it. A reference the build only references is built in Debug (a solution build
    unsets the configuration of projects outside the solution), where a mixed-mode project builds its IL-only C# stand-in
    instead of the mixed DLL, and MethodAudit binds an audited mixed-mode assembly's C++ types by name. This file is
    keyed as the gate's build recipe. A project gets no key (None) when its closure names a directory the commit lacks or
    a closure member's build files read a file outside their directory other than through ProjectReference."""
    src = [line.split(None, 3) for line in git('ls-tree', commit, 'src/', cwd=root).splitlines()]
    trees = {Path(path).name: sha for _, kind, sha, path in src if kind == 'tree'}
    install = Path(install)
    dlls = sorted((str(p.relative_to(install)), s.st_size, s.st_mtime_ns) for p in install.rglob('*.dll') for s in [p.stat()])
    # The native comparison's rules decide the native_* fields of the stored rows.
    recipe = [hashlib.sha256(p.read_bytes()).hexdigest() for p in (Path(__file__), *NATIVE_RULES)]
    common = [CACHE_FORMAT, recipe, judge, root_files(root, commit),
              [path if kind == 'tree' else f'{path} {sha}' for _, kind, sha, path in src],
              git('rev-parse', f'{commit}:{TYPE_MAP}', cwd=root),
              str(install), dlls]  # every DLL's size and mtime: hashing the DLLs would cost more than it saves
    dirs = build_closure(root, projects)
    references = {d.name: project_references(root, d) for d in dirs}
    mixed = {d.name for d in dirs if any(p.suffix.lower() == '.vcxproj' for p in build_files(d))}
    unkeyable = {d.name for d in dirs if outside_inputs(d)}
    keys = {}
    for project in projects:
        closure = closure_names(references, project)
        payload = [common, project, sorted((name, trees[name]) for name in closure & trees.keys()),
                   sorted(closure & mixed & set(projects) - {project})]
        keyable = closure <= trees.keys() and not closure & unkeyable
        keys[project] = hashlib.sha256(json.dumps(payload).encode('utf-8')).hexdigest() if keyable else None
    return keys


def cached_slices(cache, keys):
    """The inventory the projects' stored rows make up, in scan order (as one scan writes them), or None on any miss."""
    methods = []
    for key in keys.values():
        entry = key and cached_inventory(cache, key)
        if not entry:
            return None
        methods += entry['methods']
    return {'projects': list(keys), 'methods': methods}


def store_slices(cache, keys, data):
    """Store each project's rows of a scan of keys' projects."""
    rows = {project: [] for project in keys}
    for row in data['methods']:
        if row.get('project') not in rows:
            return  # a row the keys cannot attribute (an assembly not named as its project): store nothing
        rows[row['project']].append(row)
    for project, methods in rows.items():
        if keys[project]:
            store_inventory(cache, keys[project], {'methods': methods})


def cached_inventory(cache, key):
    """A stored inventory, or None when it is missing or unreadable (the caller then builds). A read refreshes the
    entry's time, so evict only removes entries no run has used for CACHE_TTL."""
    path = cache / f'{key}.json'
    try:
        data = json.loads(path.read_text(encoding='utf-8'))
    except (OSError, ValueError):
        return None
    with contextlib.suppress(OSError):
        os.utime(path)
    return data if isinstance(data, dict) and isinstance(data.get('methods'), list) and not data.get('errors') else None


# Entries are content-keyed and never superseded in place, so unused ones only take space (several MB per project).
CACHE_TTL = 3 * 24 * 3600


def evict(cache):
    """Delete stored inventories (and temporaries a killed run left) unused for CACHE_TTL before the newest entry, so a
    pause with no gate runs does not empty the cache. Best effort: an entry another run has open stays (Windows refuses
    to delete it) and goes next time; one deleted under a reader is a miss."""
    with contextlib.suppress(OSError):
        entries = []
        for entry in os.scandir(cache):
            with contextlib.suppress(OSError):
                if entry.name.endswith(('.json', '.tmp')) and entry.is_file():
                    entries.append((entry.path, entry.stat().st_mtime))
        newest = max((mtime for _, mtime in entries), default=0)
        for path, mtime in entries:
            if newest - mtime > CACHE_TTL:
                with contextlib.suppress(OSError):
                    os.unlink(path)


def store_inventory(cache, key, data):
    """Only inventories of committed trees: never a working tree, which the keys cannot identify. Best effort: another
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
    candidate cannot change what judges it. Base inventories are cached per project by slice_keys; store_candidate also
    caches a passing candidate's, for a root whose tree is exactly its HEAD commit (il_merge's clean clone)."""
    total_start = time.perf_counter()
    timings = dict.fromkeys(('judge_build', 'base_checkout', 'base_build', 'base_scan',
                             'candidate_build', 'candidate_scan', 'compare', 'total'), 0.0)
    install = Path(install or mathcad_install())
    cache = cache or CACHE
    evict(cache)
    results.mkdir(parents=True, exist_ok=False)
    log = results / 'build.log'
    base = git('rev-parse', '--verify', f'{base}^{{commit}}', cwd=root)
    report = {'base': base, 'tip': git('rev-parse', 'HEAD', cwd=root),
              'working_tree': git('status', '--porcelain', cwd=root), 'projects': projects,
              'types': list(types), 'methods': list(methods), 'passed': False,
              'candidate': str(vcxproj) if vcxproj else 'csproj', 'timings': timings}
    try:
        if not projects:
            if types or methods:
                raise RuntimeError('member selection requires a production project')
            report.update(passed=True, checked=[], failures=[], reason='no production project changed')
            return report
        with work_directory(cache.parent / 'gate-work') as work:
            original = work / 'base'
            with timed(timings, 'base_checkout'):
                checkout_base(root, base, original, log, timeout)
            # Fail closed: a base without the judge cannot judge.
            audit = original / 'tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj'
            if not audit.is_file():
                raise RuntimeError(f'base {base[:12]} has no MethodAudit to judge with ({audit.relative_to(original)})')
            with timed(timings, 'judge_build'):
                tool = build_judge(original, base, cache, dotnet, log, timeout)
            # A project the candidate adds has no base build; its members start as new (or moved) rows.
            existing = [p for p in projects if (original / 'src' / p / f'{p}.csproj').is_file()]
            judge = tool.parent.parent.name  # build_judge's key: entries judged by another judge never hit
            keys = slice_keys(original, base, judge, existing, install) if existing else {}
            before = cached_slices(cache, keys) if keys else {'methods': []}
            report['base_cached'] = bool(keys) and before is not None
            if before is None:
                print('Building base Release assemblies', flush=True)
                before = build_and_scan(original, existing, tool, work / 'before', install, dotnet, log, timeout,
                                        keep=results / 'base-inventory.json', timings=timings, prefix='base')
                store_slices(cache, keys, before)
            elif keys:
                print(f'Base inventory from cache ({len(keys)} projects)', flush=True)
            print('Building candidate Release assemblies', flush=True)
            after = build_and_scan(root, projects, tool, work / 'after', install, dotnet, log, timeout, vcxproj,
                                   keep=results / 'inventory.json', timings=timings)
            compare_start = time.perf_counter()
            # The same reference artifact must have been read by both scans.
            identities = lambda data: {m['original']['assembly']: (m['original']['sha256'], m['original']['mvid'])
                                       for m in data['methods'] if m.get('original')}
            old_id, new_id = identities(before), identities(after)
            # A .vcxproj candidate may pair nothing with the C# base's original; its methods then fail on their own.
            if any((vcxproj is None or name in new_id) and new_id.get(name) != identity for name, identity in old_id.items()):
                raise RuntimeError('Mathcad reference identity changed between scans')
            report.update(compare(before, after, types, methods))
            timings['compare'] = time.perf_counter() - compare_start
            report['originals'] = new_id
            # A clean status does not show EOL conversion: only a tree materialized with RAW_BYTES holds what its keys name.
            attributes = root / '.git' / 'info' / 'attributes'
            raw = attributes.is_file() and attributes.read_text(encoding='utf-8') == RAW_BYTES
            if store_candidate and raw and report['passed'] and not vcxproj and not report['working_tree']:
                # Judged by the base's tool, so keyed by it: if the candidate changes the tool, its keys never hit.
                store_slices(cache, slice_keys(root, report['tip'], judge, projects, install), after)
            # Kept locally for exact signatures, tokens and artifact identities; no original IL.
            (results / 'base-inventory.json').write_text(json.dumps(before, indent=2) + '\n', encoding='utf-8')
            (results / 'inventory.json').write_text(json.dumps(after, indent=2) + '\n', encoding='utf-8')
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
        report['error'] = str(error)
    finally:
        timings['total'] = time.perf_counter() - total_start
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
