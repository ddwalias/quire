"""Per-function byte parity for native Mathcad DLLs (D10 native lane; the IL gate does not cover native code).

A Quire candidate is a COFF object compiled with the same toolset and flags as the original. Each exported function
of the original is located through its .pdata entry (or, for a leaf function without one, the next function start),
and its code bytes are compared with the candidate's function section. Relocation sites are masked on both sides: the
object lists exactly which bytes depend on link-time addresses, and the same instruction stream has its displacement
at the same offset in the linked original.

Masking hides the displacement, so each call or address relocation is checked separately: when its target is an export
of the original, the original's target address must equal that export's RVA. A target the original does not export
(an internal function, a vtable, a constant) is checked through the reviewed manifest of internal targets
(docs/mirror/native/internal-targets.json, written by native_targets.py): the original's address must equal the
entry's RVA, and the candidate's definition of the target, from any --candidate object, must match the original's
bytes there under the same masking, with its own relocations checked the same way. Targets without an entry, and
entries whose definition the candidates lack or that reference unverified targets themselves, are reported as
unverified; --strict treats them as failures.

The manifest binds to the original's file name and SHA-256. An entry for another build, a function entry that does not
start a .pdata function, or a data entry outside the original's sections fails the run before any comparison. A
section's bytes past its raw size, up to its virtual size, read as zeros, as the loader fills them.

An import by ordinal is named after the export with that ordinal in the system DLL of the same name (System32), so a
candidate's `__imp_<name>` can match it; it keeps the name `#<ordinal>` when that DLL or ordinal is absent.

Export parity compares the export names of a built DLL with the original's.

Reads the original DLL only. Keep the original and any decompiled output in the gitignored .mirror-cache, never in
build inputs (D19).
"""
import argparse
import functools
import hashlib
import json
import os
import struct
import sys
from dataclasses import dataclass, field
from pathlib import Path

IMAGE_FILE_MACHINE_AMD64 = 0x8664
IMAGE_REL_AMD64_ADDR64 = 0x0001
IMAGE_REL_AMD64_ADDR32NB = 0x0003
IMAGE_REL_AMD64_REL32 = 0x0004
IMAGE_REL_AMD64_REL32_5 = 0x0009
IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080
# AMD64 COFF relocation types whose field width is 8 bytes (ADDR64) or 4 bytes (everything else).
RELOC_WIDTH = {IMAGE_REL_AMD64_ADDR64: 8, **{k: 4 for k in range(IMAGE_REL_AMD64_ADDR32NB, IMAGE_REL_AMD64_REL32_5 + 1)}}
DEFAULT_TARGETS = Path(__file__).resolve().parents[2] / "docs" / "mirror" / "native" / "internal-targets.json"


@dataclass
class Reloc:
    offset: int
    kind: int
    symbol: int = -1


@dataclass
class Section:
    name: str
    data: bytes
    relocs: list = field(default_factory=list)


@dataclass
class Symbol:
    name: str
    value: int
    section: int
    storage: int
    type: int


@dataclass
class Function:
    name: str
    code: bytes
    relocs: list
    targets: dict  # relocation offset (within the function) -> external symbol name, or None when not external
    statics: dict = field(default_factory=dict)  # relocation offset -> name of a static (non-section) target symbol
    data: bool = False  # the symbol is not a function: its span is data, such as a vtable or a constant


@dataclass
class Entry:
    """A reviewed internal target: its RVA in the original, and its size when it is data (a function's is .pdata's)."""
    rva: int
    size: int = None


class ManifestError(ValueError):
    pass


@dataclass
class Result:
    name: str
    ok: bool
    reason: str
    unverified: int = 0
    verified: int = 0  # targets verified through the manifest of internal targets
    entries: dict = field(default_factory=dict)  # manifest name -> (status, reason) for entries that did not verify


def _cstr(data, offset):
    return data[offset:data.index(b"\0", offset)].decode("ascii", "replace")


def _rva_offset(sections, rva):
    for va, size, raw, raw_size in sections:
        if va <= rva < va + raw_size:
            return raw + (rva - va)
    raise ValueError(f"RVA {rva:#x} is not in any section's file bytes")


@functools.lru_cache(maxsize=None)
def system_ordinal_names(dll):
    """Ordinal -> export name of the x64 system DLL `dll`, read from System32; empty when it cannot be read."""
    path = Path(os.environ.get("SystemRoot", r"C:\Windows")) / "System32" / dll
    try:
        return PeImage(path, ordinal_names=False).ordinals
    except (OSError, ValueError, struct.error):
        return {}


class PeImage:
    """Exports, function extents and load base of a PE32+ image, read from the file alone."""

    def __init__(self, path, ordinal_names=True):
        self.name = Path(path).name
        self.data = Path(path).read_bytes()
        self.sha256 = hashlib.sha256(self.data).hexdigest()
        d = self.data
        if d[:2] != b"MZ":
            raise ValueError(f"{path}: not a PE file")
        pe = struct.unpack_from("<I", d, 0x3C)[0]
        if d[pe:pe + 4] != b"PE\0\0":
            raise ValueError(f"{path}: missing PE signature")
        machine, nsec, _, _, _, opt_size, _ = struct.unpack_from("<HHIIIHH", d, pe + 4)
        if machine != IMAGE_FILE_MACHINE_AMD64:
            raise ValueError(f"{path}: not an x64 image")
        opt = pe + 24
        if struct.unpack_from("<H", d, opt)[0] != 0x20B:
            raise ValueError(f"{path}: not PE32+")
        self.image_base = struct.unpack_from("<Q", d, opt + 24)[0]
        dirs = opt + 112
        self.sections = []
        sh = opt + opt_size
        for i in range(nsec):
            _, vsize, va, raw_size, raw, *_ = struct.unpack_from("<8sIIIIIIHHI", d, sh + 40 * i)
            self.sections.append((va, vsize, raw, raw_size))
        self.ordinals = {}  # export ordinal -> name, for named exports
        self.exports = self._read_exports(dirs)
        self.functions = self._read_pdata(dirs)
        self.imports = self._read_imports(dirs) if ordinal_names else {}

    def _read_exports(self, dirs):
        d = self.data
        exp_rva, exp_size = struct.unpack_from("<II", d, dirs)
        if not exp_rva:
            return {}
        base = _rva_offset(self.sections, exp_rva)
        _, _, _, _, _, ordinal_base, _, nnames, aof, aon, aoo = struct.unpack_from("<IIHHIIIIIII", d, base)
        funcs = _rva_offset(self.sections, aof)
        names = _rva_offset(self.sections, aon)
        ords = _rva_offset(self.sections, aoo)
        table = {}
        for i in range(nnames):
            name = _cstr(d, _rva_offset(self.sections, struct.unpack_from("<I", d, names + 4 * i)[0]))
            index = struct.unpack_from("<H", d, ords + 2 * i)[0]
            rva = struct.unpack_from("<I", d, funcs + 4 * index)[0]
            table[name] = rva
            self.ordinals[ordinal_base + index] = name
        return table

    def _read_pdata(self, dirs):
        d = self.data
        rva, size = struct.unpack_from("<II", d, dirs + 3 * 8)
        if not rva:
            return {}
        base = _rva_offset(self.sections, rva)
        out = {}
        for i in range(size // 12):
            begin, end, _ = struct.unpack_from("<III", d, base + 12 * i)
            out[begin] = end
        return out

    def _read_imports(self, dirs):
        """Import address table slot RVA -> imported function name (see the module docstring for ordinal imports)."""
        d = self.data
        rva, _ = struct.unpack_from("<II", d, dirs + 8)
        if not rva:
            return {}
        table = {}
        desc = _rva_offset(self.sections, rva)
        while struct.unpack_from("<I", d, desc + 16)[0]:  # FirstThunk is non-zero for every real descriptor
            lookup, _, _, dll, first = struct.unpack_from("<IIIII", d, desc)
            dll = _cstr(d, _rva_offset(self.sections, dll))
            thunks = _rva_offset(self.sections, lookup or first)
            for i in range(1 << 20):
                entry = struct.unpack_from("<Q", d, thunks + 8 * i)[0]
                if not entry:
                    break
                if entry >> 63:
                    name = system_ordinal_names(dll.lower()).get(entry & 0xFFFF, f"#{entry & 0xFFFF}")
                else:
                    name = _cstr(d, _rva_offset(self.sections, entry & 0x7FFFFFFF) + 2)
                table[first + 8 * i] = name
            desc += 20
        return table

    def export_extent(self, name):
        """(begin RVA, end RVA) of an exported function, or None if the export is missing."""
        if name not in self.exports:
            return None
        begin = self.exports[name]
        end = self.functions.get(begin)
        if end is not None:
            return begin, end
        starts = sorted({r for r in list(self.exports.values()) + list(self.functions) if r > begin})
        # VirtualSize, not the file-aligned raw size: the tail of a raw block is zero padding, not code.
        section_end = next(va + (size or raw_size) for va, size, _, raw_size in self.sections
                           if va <= begin < va + (size or raw_size))
        end = starts[0] if starts and starts[0] < section_end else section_end
        off = _rva_offset(self.sections, begin)
        # A leaf function's extent is inferred, so trailing int3 padding is removed from it.
        length = len(self.data[off:off + (end - begin)].rstrip(b"\xcc"))
        return begin, begin + length

    def export_code(self, name):
        extent = self.export_extent(name)
        if extent is None:
            return None
        begin, end = extent
        off = _rva_offset(self.sections, begin)
        return self.data[off:off + (end - begin)]

    def read(self, rva, size):
        """`size` bytes at `rva` as loaded: a section's bytes past its raw size are zeros."""
        for va, vsize, raw, raw_size in self.sections:
            if va <= rva < va + max(vsize, raw_size):
                backed = max(0, min(size, raw_size - (rva - va)))
                off = raw + (rva - va)
                return self.data[off:off + backed] + bytes(size - backed)
        raise ValueError(f"RVA {rva:#x} is not in any section")

    def contains(self, rva, size):
        """Whether [rva, rva + size) lies within one section as loaded (its virtual size, or raw size if that is 0)."""
        return any(va <= rva and rva + size <= va + (vsize or raw_size)
                   for va, vsize, _, raw_size in self.sections)

    def read_u32(self, rva):
        return struct.unpack_from("<I", self.data, _rva_offset(self.sections, rva))[0]

    def read_i32(self, rva):
        return struct.unpack_from("<i", self.data, _rva_offset(self.sections, rva))[0]

    def read_u64(self, rva):
        return struct.unpack_from("<Q", self.data, _rva_offset(self.sections, rva))[0]


def read_coff(path):
    """Sections (with relocations) and the symbol table, indexed by COFF symbol index (auxiliary slots are None)."""
    d = Path(path).read_bytes()
    machine, nsec, _, symptr, nsyms, opt_size, _ = struct.unpack_from("<HHIIIHH", d, 0)
    if machine != IMAGE_FILE_MACHINE_AMD64:
        raise ValueError(f"{path}: not an x64 COFF object")
    strtab = symptr + 18 * nsyms

    def name_of(raw):
        if raw[:4] == b"\0\0\0\0":
            return _cstr(d, strtab + struct.unpack_from("<I", raw, 4)[0])
        return raw.rstrip(b"\0").decode("ascii", "replace")

    sections = []
    for i in range(nsec):
        base = 20 + opt_size + 40 * i
        raw_name, _, _, raw_size, raw_ptr, reloc_ptr, _, nrel, _, flags = struct.unpack_from("<8sIIIIIIHHI", d, base)
        relocs = []
        for j in range(nrel):
            vaddr, sym, kind = struct.unpack_from("<IIH", d, reloc_ptr + 10 * j)
            relocs.append(Reloc(vaddr, kind, sym))
        # Uninitialized data (.bss) has a size but no bytes in the file; it is zero when loaded.
        data = bytes(raw_size) if flags & IMAGE_SCN_CNT_UNINITIALIZED_DATA else d[raw_ptr:raw_ptr + raw_size]
        sections.append(Section(name_of(raw_name), data, relocs))

    symbols = [None] * nsyms
    i = 0
    while i < nsyms:
        raw = d[symptr + 18 * i:symptr + 18 * i + 18]
        value, sec, typ, storage, aux = struct.unpack_from("<IhHBB", raw, 8)
        symbols[i] = Symbol(name_of(raw[:8]), value, sec, storage, typ)
        i += 1 + aux
    return sections, symbols


def _spans(path):
    """(symbol, Function) for each external or static symbol defined in a section, other than section symbols: the
    bytes from the symbol to the next symbol of its section, the relocations there and their targets."""
    sections, symbols = read_coff(path)
    by_section = {}
    for s in symbols:
        if s is not None and s.section > 0:
            by_section.setdefault(s.section, []).append(s)
    for s in symbols:
        if s is None or s.section <= 0 or s.storage not in (2, 3) or s.name.startswith("."):
            continue
        sec = sections[s.section - 1]
        later = [t.value for t in by_section[s.section] if t.value > s.value]
        end = min(later) if later else len(sec.data)
        relocs = [Reloc(r.offset - s.value, r.kind, r.symbol) for r in sec.relocs if s.value <= r.offset < end]
        targets, statics = {}, {}
        for r in relocs:
            target = symbols[r.symbol] if 0 <= r.symbol < len(symbols) else None
            external = target is not None and target.storage == 2
            targets[r.offset] = target.name if external else None
            if target is not None and target.storage == 3 and not target.name.startswith("."):
                statics[r.offset] = target.name
        yield s, Function(s.name, sec.data[s.value:end], relocs, targets, statics, (s.type >> 4) != 2)


def functions_in_object(path):
    """Each external function symbol with the bytes of its section span, its relocations and their targets."""
    return [f for s, f in _spans(path) if s.storage == 2 and not f.data]


def definitions_in_object(path):
    """Every function and data symbol the object defines, external or static, by name (the first of duplicates)."""
    out = {}
    for _, f in _spans(path):
        out.setdefault(f.name, f)
    return out


def definitions_in_objects(paths):
    """definitions_in_object over several objects. A name two objects define differently fails: targets are resolved
    by name, so a static helper of one object must not stand in for another object's helper of the same name."""
    out, origin = {}, {}
    for path in paths:
        for name, d in definitions_in_object(path).items():
            if name not in out:
                out[name], origin[name] = d, path
                continue
            o = out[name]
            if (o.code, [(r.offset, r.kind) for r in o.relocs], o.targets, o.statics, o.data) != \
                    (d.code, [(r.offset, r.kind) for r in d.relocs], d.targets, d.statics, d.data):
                raise ValueError(f"{name} is defined differently by {origin[name]} and {path}")
    return out


def masked(code, relocs):
    """Bytes with each relocation site zeroed, and a per-byte flag that is False at masked positions."""
    buf = bytearray(code)
    mask = [True] * len(buf)
    for r in relocs:
        width = RELOC_WIDTH.get(r.kind)
        if width is None:
            raise ValueError(f"unsupported relocation type {r.kind:#x} at offset {r.offset:#x}")
        for k in range(r.offset, min(r.offset + width, len(buf))):
            mask[k] = False
            buf[k] = 0
    return bytes(buf), mask


def compare_code(name, original, original_relocs, candidate, candidate_relocs):
    """Pure comparison of two function bodies. Relocation sites of either side are excluded from the comparison."""
    if len(original) != len(candidate):
        return Result(name, False, f"length {len(candidate)} != original {len(original)}")
    o, om = masked(original, original_relocs)
    c, cm = masked(candidate, candidate_relocs)
    if om != cm:
        return Result(name, False, "relocation sites differ in position or width")
    for i, (x, y) in enumerate(zip(o, c)):
        if om[i] and x != y:
            return Result(name, False, f"first byte difference at +{i:#x}: original {x:02x}, candidate {y:02x}")
    return Result(name, True, f"{len(original)} bytes, {len(candidate_relocs)} relocation sites masked")


def _resolve(image, at, kind):
    """The RVA that the original's relocation field at RVA `at` refers to."""
    if kind == IMAGE_REL_AMD64_ADDR64:
        return image.read_u64(at) - image.image_base
    if kind == IMAGE_REL_AMD64_ADDR32NB:
        return image.read_u32(at)
    if IMAGE_REL_AMD64_REL32 <= kind <= IMAGE_REL_AMD64_REL32_5:
        # The displacement is relative to the end of the field, plus the REL32_n byte count.
        return at + 4 + (kind - IMAGE_REL_AMD64_REL32) + image.read_i32(at)
    raise ValueError(f"unsupported relocation type {kind:#x} at RVA {at:#x}")


def _addend(code, r):
    """The offset from its target symbol that the candidate's relocation field holds in place."""
    fmt = "<q" if r.kind == IMAGE_REL_AMD64_ADDR64 else "<I" if r.kind == IMAGE_REL_AMD64_ADDR32NB else "<i"
    return struct.unpack_from(fmt, code, r.offset)[0]


def load_targets(path, image):
    """The manifest's entries for the original, by decorated name, after checking each against the original.

    The manifest maps DLL file names to {"sha256": ..., "targets": {name: {"rva": "0x...", "size": n}}}; "size" is
    given for data only. A DLL the manifest does not list has no entries. Raises ManifestError when the original's
    entries bind to another build or an entry does not fit the original.
    """
    try:
        doc = json.loads(Path(path).read_text(encoding="utf-8"))
    except (OSError, ValueError) as e:
        raise ManifestError(f"{path}: {e}") from e
    return targets_from(doc, image, path)


def targets_from(doc, image, path="manifest"):
    """load_targets for a manifest already parsed; `path` names it in errors."""
    if not isinstance(doc, dict):
        raise ManifestError(f"{path}: not a JSON object")
    listed = [k for k in doc if k.lower() == image.name.lower()]
    if not listed:
        return {}
    if len(listed) > 1:
        raise ManifestError(f"{path}: {image.name} is listed more than once: {listed}")
    bound = doc[listed[0]]
    if not isinstance(bound, dict) or not isinstance(bound.get("targets", {}), dict):
        raise ManifestError(f"{path}: {listed[0]} must map to {{\"sha256\": ..., \"targets\": {{...}}}}")
    if str(bound.get("sha256", "")).lower() != image.sha256:
        raise ManifestError(f"{path}: {image.name} entries bind to SHA-256 {bound.get('sha256')}, "
                            f"original is {image.sha256}")
    entries = {}
    for name, raw in bound.get("targets", {}).items():
        try:
            entry = Entry(int(raw["rva"], 16), raw.get("size"))
        except (KeyError, TypeError, ValueError) as e:
            raise ManifestError(f"{path}: {name}: malformed entry {raw!r}") from e
        if entry.size is None:
            if entry.rva not in image.functions:
                raise ManifestError(f"{path}: {name}: RVA {entry.rva:#x} does not start a .pdata function")
        elif not (type(entry.size) is int and entry.size > 0 and image.contains(entry.rva, entry.size)):
            raise ManifestError(f"{path}: {name}: {entry.size!r} bytes at RVA {entry.rva:#x} are not in a section")
        entries[name] = entry
    return entries


SEVERITY = {"ok": 0, "unverified": 1, "mismatch": 2}


class InternalTargets:
    """The manifest's entries for one original, checked against the candidates' definitions of the same names."""

    def __init__(self, image, entries, definitions):
        self.image = image
        self.entries = entries
        self.definitions = definitions
        self.verdicts = {}  # name -> (status, reason), settled together with every entry it reaches
        self._local = {}  # name -> (status, reason, manifest names it references)

    def original_bytes(self, name):
        entry = self.entries[name]
        end = self.image.functions[entry.rva] if entry.size is None else entry.rva + entry.size
        return self.image.read(entry.rva, end - entry.rva)

    def local(self, name):
        """The entry's own verdict, from its bytes and its targets other than manifest entries, which it lists."""
        if name not in self._local:
            entry, defn, edges = self.entries[name], self.definitions.get(name), set()
            if defn is None:
                verdict = ("unverified", "not defined by the candidate objects")
            elif defn.data != (entry.size is not None):
                verdict = ("mismatch", f"candidate defines {'data' if defn.data else 'a function'}, "
                                       f"manifest entry is {'data' if entry.size is not None else 'a function'}")
            elif entry.size is not None and len(defn.code) != entry.size:
                verdict = ("mismatch", f"size {len(defn.code)} != manifest {entry.size}")
            else:
                byte_result = compare_code(name, self.original_bytes(name), defn.relocs, defn.code, defn.relocs)
                if not byte_result.ok:
                    verdict = ("mismatch", byte_result.reason)
                else:
                    found = classify_targets(self.image, entry.rva, defn, _Edges(self.entries, edges))
                    if found.mismatches:
                        verdict = ("mismatch", "target differs: " + "; ".join(found.mismatches))
                    elif found.unverified:
                        verdict = ("unverified", f"{found.unverified} unverified call/address targets")
                    else:
                        verdict = ("ok", None)
            self._local[name] = (*verdict, edges)
        return self._local[name]

    def reference(self, name):
        """(status, reason) of an entry: its own verdict, or the worst verdict of an entry it reaches. Cycles verify."""
        if name not in self.verdicts:
            reached, stack = [name], [name]
            while stack:
                for m in sorted(self.local(stack.pop())[2]):
                    if m not in reached and m not in self.verdicts:
                        reached.append(m)
                        stack.append(m)
            verdict = {n: self.local(n)[:2] for n in reached}
            changed = True
            while changed:
                changed = False
                for n in reached:
                    for m in sorted(self.local(n)[2]):
                        status = (self.verdicts.get(m) or verdict[m])[0]
                        if SEVERITY[status] > SEVERITY[verdict[n][0]]:
                            verdict[n] = (status, f"references {m}, which is {status}")
                            changed = True
            self.verdicts.update(verdict)
        return self.verdicts[name]


class _Edges:
    """Stands in for InternalTargets within one entry's own check, recording the manifest names it references."""

    def __init__(self, entries, edges):
        self.entries = entries
        self.edges = edges

    def reference(self, name):
        self.edges.add(name)
        return "ok", None


@dataclass
class Targets:
    mismatches: list
    unverified: int = 0
    verified: int = 0  # through the manifest
    entries: dict = field(default_factory=dict)  # manifest name -> (status, reason) for entries that did not verify


def classify_targets(image, begin, function, internal=None):
    """Check each relocation of `function`, placed at RVA `begin` in the original, against the original.

    An export target must resolve to the export's RVA, and a call through `__imp_<name>` must reach the import slot the
    original imports <name> through. A target listed in `internal` (InternalTargets) must resolve to its entry's RVA
    plus the candidate's addend, and the entry must verify. Any other target is unverified.
    """
    found = Targets([])
    mismatches = found.mismatches
    for r in function.relocs:
        target = function.targets.get(r.offset)
        resolved = _resolve(image, begin + r.offset, r.kind)
        if target is not None and target.startswith("__imp_"):
            imported = target[len("__imp_"):]
            if resolved not in image.imports:
                mismatches.append(f"{imported} at +{r.offset:#x}: original resolves to RVA {resolved:#x}, "
                                  f"which is not an import slot")
            elif image.imports[resolved] != imported:
                mismatches.append(f"{imported} at +{r.offset:#x}: original calls import "
                                  f"{image.imports[resolved]}")
        elif target is not None and target in image.exports:
            if image.exports[target] != resolved:
                mismatches.append(f"{target} at +{r.offset:#x}: original resolves to RVA {resolved:#x}, "
                                  f"export is {image.exports[target]:#x}")
        else:
            name = target or function.statics.get(r.offset)
            if internal is None or name not in internal.entries:
                found.unverified += 1
                continue
            expected = internal.entries[name].rva + _addend(function.code, r)
            if resolved != expected:
                mismatches.append(f"{name} at +{r.offset:#x}: original resolves to RVA {resolved:#x}, "
                                  f"manifest entry gives {expected:#x}")
                found.entries[name] = ("mismatch", f"original resolves to RVA {resolved:#x}, entry gives {expected:#x}")
                continue
            status, reason = internal.reference(name)
            if status == "ok":
                found.verified += 1
                continue
            found.entries[name] = (status, reason)
            if status == "mismatch":
                mismatches.append(f"{name} at +{r.offset:#x}: manifest entry does not match: {reason}")
            else:
                found.unverified += 1
    return found


def check_targets(image, begin, function, internal=None):
    """(mismatch messages, number of unverified relocations) from classify_targets."""
    found = classify_targets(image, begin, function, internal)
    return found.mismatches, found.unverified


def compare_object(original_dll, candidate_obj, symbols=None, targets=None):
    """Compare every external function of the candidate objects (or the named ones) with the original DLL's export.

    `candidate_obj` is one object or a list of them; all of them define internal targets. `targets` is the manifest of
    internal targets, if any. A candidate function the original does not export is compared at its RVA when the
    manifest lists it as a function. Without `symbols`, every other manifest entry the candidates define is checked
    too, so a stale entry fails even when nothing references it.
    """
    image = PeImage(original_dll)
    objects = [candidate_obj] if isinstance(candidate_obj, (str, Path)) else list(candidate_obj)
    entries = load_targets(targets, image) if targets else {}
    definitions = definitions_in_objects(objects)
    funcs = []
    for obj in objects:
        for f in functions_in_object(obj):
            if all(f.name != g.name for g in funcs):
                funcs.append(f)
    internal = InternalTargets(image, entries, definitions)
    if symbols:
        wanted = set(symbols)
        funcs = [f for f in funcs if f.name in wanted]
        missing = wanted - {f.name for f in funcs}
    else:
        missing = set()
    results = [Result(n, False, "named in --symbol but not defined by the candidate") for n in sorted(missing)]
    if not funcs and not missing:
        results.append(Result("(object)", False, "candidate defines no external functions"))
    for f in funcs:
        extent = image.export_extent(f.name)
        if extent is not None:
            begin, code = extent[0], image.export_code(f.name)
        elif f.name in entries and entries[f.name].size is None:
            begin, code = entries[f.name].rva, internal.original_bytes(f.name)
            f = Function(f.name + " (at manifest RVA)", f.code, f.relocs, f.targets, f.statics)
        else:
            results.append(Result(f.name, False, "not an export of the original"))
            continue
        # The original is linked, so it has no relocation records. Its displacements sit at the candidate's relocation
        # offsets because both sides encode the same instructions; the candidate's sites mask both.
        byte_result = compare_code(f.name, code, f.relocs, f.code, f.relocs)
        # Relocation offsets only line up when the lengths agree; otherwise the target check would read the wrong bytes.
        found = classify_targets(image, begin, f, internal) if len(code) == len(f.code) else Targets([])
        reasons = [] if byte_result.ok else [byte_result.reason]
        if found.mismatches:
            reasons.append("call target differs: " + "; ".join(found.mismatches))
        ok = byte_result.ok and not found.mismatches
        results.append(Result(f.name, ok, "; ".join(reasons) if reasons else byte_result.reason, found.unverified,
                              found.verified, found.entries))
    if not symbols:
        compared = {f.name for f in funcs}
        for name in sorted(set(entries) & set(definitions) - compared):
            status, reason = internal.reference(name)
            results.append(Result(f"{name} (manifest entry)", status != "mismatch", reason or "verified at manifest RVA",
                                  int(status == "unverified")))
    return results


def compare_exports(original_dll, built_dll):
    """Export names missing from the built DLL and extra in it, relative to the original."""
    original = set(PeImage(original_dll).exports)
    built = set(PeImage(built_dll).exports)
    return sorted(original - built), sorted(built - original)


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--original", type=Path, required=True, help="original DLL (read only)")
    parser.add_argument("--candidate", type=Path, action="append",
                        help="COFF object compiled with the original's flags (repeatable; every object can define "
                             "internal targets)")
    parser.add_argument("--built-dll", type=Path, help="built Quire DLL: check its export names against the original")
    parser.add_argument("--symbol", action="append", help="limit to this decorated function name (repeatable)")
    parser.add_argument("--strict", action="store_true", help="fail on call targets that cannot be verified")
    parser.add_argument("--targets", type=Path,
                        help="manifest of internal targets (default: docs/mirror/native/internal-targets.json, "
                             "when it exists)")
    args = parser.parse_args(argv)
    if not args.candidate and not args.built_dll:
        parser.error("give --candidate, --built-dll or both")
    failed = False
    if args.built_dll:
        missing, extra = compare_exports(args.original, args.built_dll)
        for name in missing:
            print(f"FAIL export missing from built DLL: {name}")
        for name in extra:
            print(f"FAIL export not in original: {name}")
        print(f"exports: {len(missing)} missing, {len(extra)} extra")
        failed |= bool(missing or extra)
    if args.candidate:
        targets = args.targets or (DEFAULT_TARGETS if DEFAULT_TARGETS.is_file() else None)
        try:
            results = compare_object(args.original, args.candidate, args.symbol, targets)
        except ValueError as e:  # a manifest that does not fit the original, or conflicting candidate definitions
            print(f"FAIL {e}")
            return 1
        unverified_total, verified_total, entries = 0, 0, {}
        for r in results:
            bad = not r.ok or (args.strict and r.unverified)
            failed |= bad
            unverified_total += r.unverified
            verified_total += r.verified
            entries.update(r.entries)
            print(f"{'FAIL' if bad else 'OK  '} {r.name}: {r.reason}"
                  + (f" [{r.unverified} unverified call/address targets]" if r.unverified else "")
                  + (f" [{r.verified} verified by manifest]" if r.verified else ""))
        for name, (status, reason) in sorted(entries.items()):
            print(f"{'FAIL' if status == 'mismatch' or args.strict else 'WARN'} manifest entry {name}: "
                  f"{status}: {reason}")
        matched = sum(r.ok for r in results)
        print(f"{matched}/{len(results)} functions match bytes; {unverified_total} unverified targets; "
              f"{verified_total} verified by manifest" + (" (strict)" if args.strict else ""))
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
