"""Per-function byte parity for native Mathcad DLLs (D10 native lane; the IL gate does not cover native code).

A Quire candidate is a COFF object compiled with the same toolset and flags as the original. Each exported function
of the original is located through its .pdata entry (or, for a leaf function without one, the next function start),
and its code bytes are compared with the candidate's function section. Relocation sites are masked on both sides: the
object lists exactly which bytes depend on link-time addresses, and the same instruction stream has its displacement
at the same offset in the linked original.

Masking hides the displacement, so each call or address relocation is checked separately: when its target is an export
of the original, the original's target address must equal that export's RVA. Other targets (internal functions, data)
cannot be resolved without the original's symbols, so they are reported as unverified; --strict treats them as failures.

Export parity compares the export names of a built DLL with the original's.

Reads the original DLL only. Keep the original and any decompiled output in the gitignored .mirror-cache, never in
build inputs (D19).
"""
import argparse
import struct
import sys
from dataclasses import dataclass, field
from pathlib import Path

IMAGE_FILE_MACHINE_AMD64 = 0x8664
IMAGE_REL_AMD64_ADDR64 = 0x0001
IMAGE_REL_AMD64_ADDR32NB = 0x0003
IMAGE_REL_AMD64_REL32 = 0x0004
IMAGE_REL_AMD64_REL32_5 = 0x0009
# AMD64 COFF relocation types whose field width is 8 bytes (ADDR64) or 4 bytes (everything else).
RELOC_WIDTH = {IMAGE_REL_AMD64_ADDR64: 8, **{k: 4 for k in range(IMAGE_REL_AMD64_ADDR32NB, IMAGE_REL_AMD64_REL32_5 + 1)}}


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


@dataclass
class Result:
    name: str
    ok: bool
    reason: str
    unverified: int = 0


def _cstr(data, offset):
    return data[offset:data.index(b"\0", offset)].decode("ascii", "replace")


def _rva_offset(sections, rva):
    for va, size, raw, raw_size in sections:
        if va <= rva < va + max(size, raw_size):
            return raw + (rva - va)
    raise ValueError(f"RVA {rva:#x} is not in any section")


class PeImage:
    """Exports, function extents and load base of a PE32+ image, read from the file alone."""

    def __init__(self, path):
        self.data = Path(path).read_bytes()
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
        self.exports = self._read_exports(dirs)
        self.functions = self._read_pdata(dirs)
        self.imports = self._read_imports(dirs)

    def _read_exports(self, dirs):
        d = self.data
        exp_rva, exp_size = struct.unpack_from("<II", d, dirs)
        if not exp_rva:
            return {}
        base = _rva_offset(self.sections, exp_rva)
        _, _, _, _, _, _, _, nnames, aof, aon, aoo = struct.unpack_from("<IIHHIIIIIII", d, base)
        funcs = _rva_offset(self.sections, aof)
        names = _rva_offset(self.sections, aon)
        ords = _rva_offset(self.sections, aoo)
        table = {}
        for i in range(nnames):
            name = _cstr(d, _rva_offset(self.sections, struct.unpack_from("<I", d, names + 4 * i)[0]))
            index = struct.unpack_from("<H", d, ords + 2 * i)[0]
            rva = struct.unpack_from("<I", d, funcs + 4 * index)[0]
            table[name] = rva
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
        """Import address table slot RVA -> imported function name (ordinal imports are named by their number)."""
        d = self.data
        rva, _ = struct.unpack_from("<II", d, dirs + 8)
        if not rva:
            return {}
        table = {}
        desc = _rva_offset(self.sections, rva)
        while struct.unpack_from("<I", d, desc + 16)[0]:  # FirstThunk is non-zero for every real descriptor
            lookup, _, _, _, first = struct.unpack_from("<IIIII", d, desc)
            thunks = _rva_offset(self.sections, lookup or first)
            for i in range(1 << 20):
                entry = struct.unpack_from("<Q", d, thunks + 8 * i)[0]
                if not entry:
                    break
                if entry >> 63:
                    name = f"#{entry & 0xFFFF}"
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
        raw_name, _, _, raw_size, raw_ptr, reloc_ptr, _, nrel, _, _ = struct.unpack_from("<8sIIIIIIHHI", d, base)
        relocs = []
        for j in range(nrel):
            vaddr, sym, kind = struct.unpack_from("<IIH", d, reloc_ptr + 10 * j)
            relocs.append(Reloc(vaddr, kind, sym))
        sections.append(Section(name_of(raw_name), d[raw_ptr:raw_ptr + raw_size], relocs))

    symbols = [None] * nsyms
    i = 0
    while i < nsyms:
        raw = d[symptr + 18 * i:symptr + 18 * i + 18]
        value, sec, typ, storage, aux = struct.unpack_from("<IhHBB", raw, 8)
        symbols[i] = Symbol(name_of(raw[:8]), value, sec, storage, typ)
        i += 1 + aux
    return sections, symbols


def functions_in_object(path):
    """Each external function symbol with the bytes of its section span, its relocations and their targets."""
    sections, symbols = read_coff(path)
    by_section = {}
    for s in symbols:
        if s is not None and s.section > 0:
            by_section.setdefault(s.section, []).append(s)
    out = []
    for s in symbols:
        if s is None or s.section <= 0 or s.storage != 2 or (s.type >> 4) != 2:
            continue
        sec = sections[s.section - 1]
        later = [t.value for t in by_section[s.section] if t.value > s.value]
        end = min(later) if later else len(sec.data)
        relocs = [Reloc(r.offset - s.value, r.kind, r.symbol) for r in sec.relocs if s.value <= r.offset < end]
        targets = {}
        for r in relocs:
            target = symbols[r.symbol] if 0 <= r.symbol < len(symbols) else None
            external = target is not None and target.storage == 2
            targets[r.offset] = target.name if external else None
        out.append(Function(s.name, sec.data[s.value:end], relocs, targets))
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


def check_targets(image, begin, function):
    """Verify each relocation whose target is an export of the original: its address there must be the export's RVA.

    Imports are checked the same way: a call through `__imp_<name>` must reach the import slot the original imports
    <name> through. Returns (mismatch messages, number of relocations that could not be verified).
    """
    mismatches, unverified = [], 0
    for r in function.relocs:
        target = function.targets.get(r.offset)
        at = begin + r.offset
        if r.kind == IMAGE_REL_AMD64_ADDR64:
            resolved = image.read_u64(at) - image.image_base
        elif r.kind == IMAGE_REL_AMD64_ADDR32NB:
            resolved = image.read_u32(at)
        elif IMAGE_REL_AMD64_REL32 <= r.kind <= IMAGE_REL_AMD64_REL32_5:
            # The displacement is relative to the end of the field, plus the REL32_n byte count.
            resolved = at + 4 + (r.kind - IMAGE_REL_AMD64_REL32) + image.read_i32(at)
        else:
            raise ValueError(f"unsupported relocation type {r.kind:#x} at offset {r.offset:#x}")
        if target is not None and target.startswith("__imp_"):
            imported = target[len("__imp_"):]
            if resolved not in image.imports:
                mismatches.append(f"{imported} at +{r.offset:#x}: original resolves to RVA {resolved:#x}, "
                                  f"which is not an import slot")
            elif image.imports[resolved] != imported:
                mismatches.append(f"{imported} at +{r.offset:#x}: original calls import "
                                  f"{image.imports[resolved]}")
        elif target is None or target not in image.exports:
            unverified += 1
        elif image.exports[target] != resolved:
            mismatches.append(f"{target} at +{r.offset:#x}: original resolves to RVA {resolved:#x}, "
                              f"export is {image.exports[target]:#x}")
    return mismatches, unverified


def compare_object(original_dll, candidate_obj, symbols=None):
    """Compare every external function of the candidate object (or the named ones) with the original DLL's export."""
    image = PeImage(original_dll)
    funcs = functions_in_object(candidate_obj)
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
        if extent is None:
            results.append(Result(f.name, False, "not an export of the original"))
            continue
        begin, _ = extent
        code = image.export_code(f.name)
        # The original is linked, so it has no relocation records. Its displacements sit at the candidate's relocation
        # offsets because both sides encode the same instructions; the candidate's sites mask both.
        byte_result = compare_code(f.name, code, f.relocs, f.code, f.relocs)
        # Relocation offsets only line up when the lengths agree; otherwise the target check would read the wrong bytes.
        mismatches, unverified = check_targets(image, begin, f) if len(code) == len(f.code) else ([], 0)
        reasons = [] if byte_result.ok else [byte_result.reason]
        if mismatches:
            reasons.append("call target differs: " + "; ".join(mismatches))
        ok = byte_result.ok and not mismatches
        results.append(Result(f.name, ok, "; ".join(reasons) if reasons else byte_result.reason, unverified))
    return results


def compare_exports(original_dll, built_dll):
    """Export names missing from the built DLL and extra in it, relative to the original."""
    original = set(PeImage(original_dll).exports)
    built = set(PeImage(built_dll).exports)
    return sorted(original - built), sorted(built - original)


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--original", type=Path, required=True, help="original DLL (read only)")
    parser.add_argument("--candidate", type=Path, help="COFF object compiled with the original's flags")
    parser.add_argument("--built-dll", type=Path, help="built Quire DLL: check its export names against the original")
    parser.add_argument("--symbol", action="append", help="limit to this decorated function name (repeatable)")
    parser.add_argument("--strict", action="store_true", help="fail on call targets that cannot be verified")
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
        results = compare_object(args.original, args.candidate, args.symbol)
        unverified_total = 0
        for r in results:
            bad = not r.ok or (args.strict and r.unverified)
            failed |= bad
            unverified_total += r.unverified
            print(f"{'FAIL' if bad else 'OK  '} {r.name}: {r.reason}"
                  + (f" [{r.unverified} unverified call/address targets]" if r.unverified else ""))
        matched = sum(r.ok for r in results)
        print(f"{matched}/{len(results)} functions match bytes; {unverified_total} unverified targets"
              + (" (strict)" if args.strict else ""))
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
