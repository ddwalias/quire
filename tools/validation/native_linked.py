"""Per-export comparison of a linked native candidate DLL with the original (D30).

    python tools/validation/native_linked.py --original "<install>/efistatsci_.dll" \\
        --candidate-dll .mirror-cache/bin/Quire.Native.EfistatsciImpl.dll --map .mirror-cache/bin/candidate.map

The original's objects were compiled with /GL, so the linker generated their machine code from the whole program.
A candidate built the same way holds no machine code in its objects, which native_bytes.py compares, so this module
compares the linked DLLs instead. The pinned toolset's `dumpbin /disasm` decodes both images. Each export's
instructions must pair one to one with the original's: the same lengths, mnemonics and operand text once image
addresses are set aside, and the same bytes when an instruction holds no image address.

Each image address an instruction holds is checked by what it refers to, with the rules native_bytes.py applies to
relocations:

- a place inside the same function: the same offset from the function's start;
- an import slot, or an import thunk (`jmp [slot]`): the same imported name;
- an export of either image: the same export name;
- a manifest entry of the original (docs/mirror/native/internal-targets.json): the candidate's symbol there, from the
  linker map, must have the entry's name. A toolchain or parked entry (D23, D29) verifies by that alone; a function
  entry is compared as below, and a data entry over its size;
- any other function of the original: the candidate's target must be a function whose instructions compare equal,
  recursively (a cycle verifies);
- any other data: the candidate's target must start a map symbol. The bytes are compared over the longer of that
  symbol's extent and the original object's, which ends at the next address the original's code or pointers refer
  to. A pointer inside data (a base relocation on both sides) is checked as a target in turn.

An instruction that holds an image address is compared byte for byte with only that address's field masked; a field
that cannot be located fails. A function whose start the whole-image decode does not reach as an instruction boundary
(data in .text before it) is decoded again on its own with `dumpbin /range`.

A target that none of these rules settles is unverified, and fails the export unless --allow-unverified is given.
Comparisons read bytes in memory; nothing of the original is written out (D15).
"""
import argparse
import bisect
import difflib
import re
import struct
import subprocess
import sys
from dataclasses import dataclass, field
from pathlib import Path

from native_bytes import DEFAULT_TARGETS, PeImage, Result, _thunk_slot, anonymous, load_targets

LINE = re.compile(r"^\s+([0-9A-F]{16}):\s+(\S+)[ \t]*(.*)$")
ADDRESS = re.compile(r"\b([0-9A-F]{16})h?\b")
MAP_SYMBOL = re.compile(r"^\s*[0-9a-fA-F]{4}:[0-9a-fA-F]{8}\s+(\S+)\s+([0-9a-fA-F]{16})\b")
IMAGE_REL_BASED_DIR64 = 10


def dumpbin():
    """dumpbin.exe of the pinned MSVC instance."""
    from release_il import msvc_instance
    found = sorted(msvc_instance().glob("VC/Tools/MSVC/*/bin/Hostx64/x64/dumpbin.exe"))
    if not found:
        raise RuntimeError("the pinned MSVC instance has no dumpbin.exe")
    return found[-1]


def disassemble(dll, tool=None, span=None, image_base=None):
    """RVA -> (mnemonic, operand text) for every instruction dumpbin decodes in `dll`, or in the RVA `span`."""
    base = image_base if image_base is not None else PeImage(dll).image_base
    command = [str(tool or dumpbin()), "/nologo", "/disasm:nobytes"]
    if span:
        command.append(f"/range:{base + span[0]:#x},{base + span[1]:#x}")
    text = subprocess.run([*command, str(dll)], capture_output=True, text=True, check=True).stdout
    out = parse_disassembly(text, base)
    return {rva: ins for rva, ins in out.items() if span[0] <= rva < span[1]} if span else out


def parse_disassembly(text, image_base):
    out = {}
    for line in text.splitlines():
        if m := LINE.match(line):
            out[int(m.group(1), 16) - image_base] = (m.group(2), m.group(3).strip())
    return out


def parse_map(text):
    """Decorated name -> RVA, and RVA -> names, from a linker /MAP file (publics and static symbols)."""
    base = None
    names, by_rva = {}, {}
    for line in text.splitlines():
        if base is None and "Preferred load address is" in line:
            base = int(line.split()[-1], 16)
            continue
        m = MAP_SYMBOL.match(line)
        if m and base is not None and int(m.group(2), 16) > base:
            rva = int(m.group(2), 16) - base
            names.setdefault(m.group(1), rva)
            by_rva.setdefault(rva, set()).add(anonymous(m.group(1)))
    return names, by_rva


def base_relocations(image):
    """RVAs of the image's 64-bit base relocations (the pointers its data and code hold)."""
    data = image.data
    pe = struct.unpack_from("<I", data, 0x3C)[0]
    rva, size = struct.unpack_from("<II", data, pe + 24 + 112 + 5 * 8)
    out = set()
    if not rva:
        return out
    block = image.read(rva, size)
    at = 0
    while at + 8 <= len(block):
        page, length = struct.unpack_from("<II", block, at)
        if length < 8:
            break
        for k in range(8, length, 2):
            entry = struct.unpack_from("<H", block, at + k)[0]
            if entry >> 12 == IMAGE_REL_BASED_DIR64:
                out.add(page + (entry & 0xFFF))
        at += length
    return out


@dataclass
class Linked:
    """One linked image as the comparison sees it."""
    image: object
    instructions: dict  # RVA -> (mnemonic, operands)
    names: dict = field(default_factory=dict)  # RVA -> {anonymous decorated names} (exports and map symbols)
    relocations: set = field(default_factory=set)
    decoder: object = None  # (begin, end) -> instructions decoded from `begin`; None fails closed

    def __post_init__(self):
        self.addresses = sorted(self.instructions)
        self.size = max((va + max(vsize, raw) for va, vsize, _, raw in self.image.sections), default=0)
        self.exports_at = {}
        for name, rva in self.image.exports.items():
            self.exports_at.setdefault(rva, set()).add(name)
        self.starts = sorted(self.names)

    @classmethod
    def load(cls, dll, map_text=None, tool=None):
        image = PeImage(dll)
        by_rva = parse_map(map_text)[1] if map_text else {}
        tool = tool or dumpbin()
        decoder = lambda begin, end: disassemble(dll, tool, (begin, end), image.image_base)
        return cls(image, disassemble(dll, tool, None, image.image_base), by_rva, base_relocations(image), decoder)

    def body(self, begin, end):
        """[(rva, length, mnemonic, operands)] of the instructions in [begin, end), decoded from `begin`, or None when
        `begin` is not an instruction boundary and no decoder can decode from it."""
        if begin in self.instructions:
            instructions, addresses = self.instructions, self.addresses
        elif self.decoder is not None:
            instructions = self.decoder(begin, end)
            addresses = sorted(instructions)
            if not addresses or addresses[0] != begin:
                return None
        else:
            return None
        i, j = bisect.bisect_left(addresses, begin), bisect.bisect_left(addresses, end)
        rows = []
        for k in range(i, j):
            rva = addresses[k]
            nxt = addresses[k + 1] if k + 1 < len(addresses) else end
            rows.append((rva, min(nxt, end) - rva, *instructions[rva]))
        return rows

    def boundaries(self):
        """Sorted RVAs that something in the image refers to: instruction operands, pointers, exports, symbols."""
        if not hasattr(self, "_boundaries"):
            found = set(self.image.exports.values()) | set(self.names)
            for _, operands in self.instructions.values():
                found.update(a for a in map(self.address, ADDRESS.findall(operands)) if a is not None)
            for at in self.relocations:
                if self.image.contains(at, 8):
                    target = struct.unpack("<Q", self.image.read(at, 8))[0] - self.image.image_base
                    if 0 <= target < self.size:
                        found.add(target)
            self._boundaries = sorted(found)
        return self._boundaries

    def object_end(self, rva):
        """Where the object at `rva` ends: the next referenced address, within its section."""
        bounds = self.boundaries()
        k = bisect.bisect_right(bounds, rva)
        end = bounds[k] if k < len(bounds) else self.size
        for va, vsize, _, raw in self.image.sections:
            if va <= rva < va + max(vsize, raw):
                end = min(end, va + max(vsize, raw))
        return end

    def label(self, rva):
        """A name for `rva`: an import, an export or map symbol, or the nearest preceding symbol plus an offset."""
        name = Comparison.imported(self, rva) or sorted(self.exports_at.get(rva) or self.names.get(rva) or {""})[0]
        if name:
            return name
        k = bisect.bisect_right(self.starts, rva) - 1
        if k >= 0:
            start = self.starts[k]
            return f"{sorted(self.names[start])[0]}+{rva - start:#x}"
        return f"{rva:#x}"

    def address(self, token):
        """The RVA an operand's 16-digit hex token names when it lies in the image, else None."""
        rva = int(token, 16) - self.image.image_base
        return rva if 0 <= rva < self.size else None

    def symbol_extent(self, rva):
        """(rva, end) of the map symbol starting at `rva`: up to the next symbol, within its section."""
        k = bisect.bisect_right(self.starts, rva)
        end = self.starts[k] if k < len(self.starts) else self.size
        for va, vsize, _, raw in self.image.sections:
            if va <= rva < va + max(vsize, raw):
                end = min(end, va + max(vsize, raw))
        return rva, end

    def executable(self, rva):
        return any(b <= rva < e for b, e in self.image.code_sections)


class Comparison:
    """Export-by-export comparison of a candidate image with the original under the original's manifest."""

    def __init__(self, original, candidate, entries):
        self.o, self.c = original, candidate
        self.entries = entries
        self.entry_at = {e.rva: (n, e) for n, e in entries.items()}
        self.memo = {}

    def export(self, name):
        """Result for the export `name` of the candidate."""
        if name not in self.o.image.exports:
            return Result(name, False, "not an export of the original")
        ao, ac = self.o.image.exports[name], self.c.image.exports[name]
        self.memo = {}  # a result assumed while a cycle was open must not reach another export
        if not self.o.executable(ao):
            status, reason, unverified = self.data(ao, ac, self.c.symbol_extent(ac))
        else:
            status, reason, unverified = self.function(ao, ac)
        return Result(name, status != "mismatch", reason or "instructions and targets match", unverified)

    def similarity(self, name):
        """How closely the candidate export's own instructions follow the original's, from 0 to 1: the difflib ratio of
        mnemonic and operand text with image addresses set aside. Callees and bytes are not compared, so 1.0 does not
        mean a match; export() decides that. Search tools (native_permute.py) use it to rank near misses."""
        rows = []
        for side in (self.o, self.c):
            extent = side.image.function_extent(side.image.exports[name])
            body = side.body(*extent) if extent else None
            if not body:
                return 0.0
            rows.append([f"{m} {ADDRESS.sub('X', p)}" for _, _, m, p in body])
        return difflib.SequenceMatcher(None, *rows, autojunk=False).ratio()

    def function(self, ao, ac):
        """(status, reason, unverified) for the original's function at `ao` against the candidate's at `ac`."""
        key = ("f", ao, ac)
        if key in self.memo:
            return self.memo[key]
        self.memo[key] = ("ok", None, 0)  # a cycle verifies
        eo, ec = self.o.image.function_extent(ao), self.c.image.function_extent(ac)
        if eo is None or ec is None:
            result = ("mismatch", f"RVA {ac:#x} does not start a candidate function", 0) if ec is None else \
                ("mismatch", f"original RVA {ao:#x} does not start a function", 0)
        else:
            result = self.instructions(eo, ec)
        self.memo[key] = result
        return result

    def instructions(self, eo, ec):
        if eo[1] - eo[0] != ec[1] - ec[0]:
            return "mismatch", f"length {ec[1] - ec[0]} != original {eo[1] - eo[0]}", 0
        bo, bc = self.o.body(*eo), self.c.body(*ec)
        if bo is None or bc is None:
            return "mismatch", "cannot decode from the function's start", 0
        if len(bo) != len(bc):
            return "mismatch", f"{len(bc)} instructions != original {len(bo)}", 0
        unverified = 0
        for (ro, lo, mo, po), (rc, lc, mc, pc) in zip(bo, bc):
            at = f"+{ro - eo[0]:#x}"
            if lo != lc or mo != mc or ADDRESS.sub("X", po) != ADDRESS.sub("X", pc):
                return "mismatch", f"{at}: {mc} {pc} != original {mo} {po}", 0
            to = [a for a in map(self.o.address, ADDRESS.findall(po)) if a is not None]
            tc = [a for a in map(self.c.address, ADDRESS.findall(pc)) if a is not None]
            if len(to) != len(tc):
                return "mismatch", f"{at}: image addresses differ in number", 0
            xo, xc = self.o.image.read(ro, lo), self.c.image.read(rc, lc)
            if to:
                mo, mc = address_fields(xo, ro, to, self.o.image.image_base), \
                    address_fields(xc, rc, tc, self.c.image.image_base)
                if mo is None or mc is None:
                    return "mismatch", f"{at}: cannot locate the address field of {mc_text(mo, pc, po)}", 0
                xo, xc = masked_bytes(xo, mo | mc), masked_bytes(xc, mo | mc)
            if xo != xc:
                return "mismatch", f"{at}: bytes differ", 0
            for ao, ac in zip(to, tc):
                status, reason, n = self.target(ao, ac, eo, ec)
                if status == "mismatch":
                    return "mismatch", f"{at}: {reason}", 0
                unverified += n
        return verdict(unverified)

    def target(self, ao, ac, eo=None, ec=None):
        """(status, reason, unverified) of the candidate address `ac` standing for the original's `ao`."""
        o, c = self.o, self.c
        if eo and (eo[0] <= ao < eo[1] or ec[0] <= ac < ec[1]):
            same = ao - eo[0] == ac - ec[0]
            return ("ok", None, 0) if same else ("mismatch", "local target differs", 0)
        io, ic = self.imported(o, ao), self.imported(c, ac)
        if io or ic:
            return ("ok", None, 0) if io == ic else ("mismatch", f"import {ic} != original {io}", 0)
        xo, xc = o.exports_at.get(ao, set()), c.exports_at.get(ac, set())
        if xo or xc:
            return ("ok", None, 0) if xo & xc else ("mismatch", f"export {sorted(xc)} != original {sorted(xo)}", 0)
        if ao in self.entry_at:
            name, entry = self.entry_at[ao]
            if anonymous(name) not in c.names.get(ac, set()):
                return "mismatch", f"candidate target {sorted(c.names.get(ac, set())) or hex(ac)} is not {name}", 0
            if entry.toolchain or entry.parked:
                return "ok", None, 0
            if entry.size is not None:
                return self.data(ao, ac, (ac, ac + entry.size))
            return self.function(ao, ac)
        if o.executable(ao) and c.executable(ac):
            if o.image.function_extent(ao) is not None and c.image.function_extent(ac) is not None:
                return self.function(ao, ac)
            return "unverified", None, 1
        if not o.executable(ao) and not c.executable(ac) and ac in c.names:
            begin, end = c.symbol_extent(ac)
            return self.data(ao, ac, (begin, max(end, begin + o.object_end(ao) - ao)))
        return "unverified", None, 1

    @staticmethod
    def imported(image, rva):
        if rva in image.image.imports:
            return image.image.imports[rva]
        slot = _thunk_slot(image.image, rva)
        return image.image.imports.get(slot) if slot is not None else None

    def data(self, ao, ac, extent):
        """(status, reason, unverified) of the candidate's data over `extent` against the original's at `ao`."""
        key = ("d", ao, ac)
        if key in self.memo:
            return self.memo[key]
        self.memo[key] = ("ok", None, 0)
        size = extent[1] - extent[0]
        if size <= 0 or not self.o.image.contains(ao, size):
            result = ("mismatch", f"{size} bytes at original RVA {ao:#x} are not in a section", 0)
        elif not self.c.image.contains(ac, size):
            result = ("mismatch", f"the candidate's data at RVA {ac:#x} is shorter than the original's {size} bytes", 0)
        else:
            result = self.data_bytes(ao, ac, size)
        self.memo[key] = result
        return result

    def data_bytes(self, ao, ac, size):
        bo, bc = self.o.image.read(ao, size), self.c.image.read(ac, size)
        unverified, k = 0, 0
        while k < size:
            po, pc = ao + k in self.o.relocations, ac + k in self.c.relocations
            if po != pc:
                return "mismatch", f"data +{k:#x}: a pointer on one side only", 0
            if po and k + 8 <= size:
                to = struct.unpack_from("<Q", bo, k)[0] - self.o.image.image_base
                tc = struct.unpack_from("<Q", bc, k)[0] - self.c.image.image_base
                status, reason, n = self.target(to, tc)
                if status == "mismatch":
                    return "mismatch", f"data +{k:#x}: {reason}", 0
                unverified += n
                k += 8
                continue
            if bo[k] != bc[k]:
                return "mismatch", f"data +{k:#x}: bytes differ", 0
            k += 1
        return verdict(unverified)


def address_fields(code, rva, targets, image_base):
    """Byte offsets of the fields in the instruction `code` at `rva` that hold `targets`: a rel32 displacement relative
    to the instruction's end, an absolute 64-bit address, or a short branch's rel8 (its last byte). None when a
    target's field cannot be located."""
    end, out = rva + len(code), set()
    for target in targets:
        found = None
        for k in range(len(code) - 4, -1, -1):
            if struct.unpack_from("<i", code, k)[0] == target - end:
                found = range(k, k + 4)
                break
        if found is None:
            for k in range(len(code) - 8, -1, -1):
                if struct.unpack_from("<Q", code, k)[0] == image_base + target:
                    found = range(k, k + 8)
                    break
        if found is None and len(code) >= 2 and struct.unpack_from("<b", code, len(code) - 1)[0] == target - end:
            found = range(len(code) - 1, len(code))  # a short branch's rel8, always its last byte
        if found is None:
            return None
        out.update(found)
    return out


def masked_bytes(code, positions):
    return bytes(0 if k in positions else b for k, b in enumerate(code))


def mc_text(found, candidate, original):
    return f"original {original}" if found is None else f"candidate {candidate}"


def verdict(unverified):
    """(status, reason, unverified count) once every instruction or pointer matched."""
    return ("unverified", f"{unverified} unverified targets", unverified) if unverified else ("ok", None, 0)


def load_comparison(original_dll, candidate_dll, map_text, targets=None, tool=None):
    """The Comparison of a candidate DLL (with its linker map) against the original under the manifest `targets`."""
    original = Linked.load(original_dll, None, tool)
    candidate = Linked.load(candidate_dll, map_text, tool)
    return Comparison(original, candidate, load_targets(targets, original.image) if targets else {})


def results_of(comparison, symbols=None):
    """Results for the candidate's exports (or the named ones) against the original."""
    names = symbols or sorted(comparison.c.image.exports)
    results = [comparison.export(n) if n in comparison.c.image.exports
               else Result(n, False, "not exported by the candidate") for n in names]
    if not symbols:
        for name in sorted(n for n, e in comparison.entries.items() if e.parked):
            results.append(Result(f"{name} (parked entry)", True, "RVA checked at each site; bytes not compared (D29)"))
    return results


def fingerprint_text(candidate, name):
    """The candidate export's instructions with image addresses replaced by what they name, for change detection. A
    data export, or one without a function extent, is its bytes over its symbol's extent."""
    rva = candidate.image.exports[name]
    extent = candidate.image.function_extent(rva) if candidate.executable(rva) else None
    rows = candidate.body(*extent) if extent else None
    if rows is None:
        begin, end = candidate.symbol_extent(rva)
        return f"bytes {candidate.image.read(begin, end - begin).hex()}"
    begin, end = extent
    out = []
    for at, length, mnemonic, operands in rows:
        def named(m):
            a = candidate.address(m.group(1))
            if a is None:
                return m.group(0)
            return f"+{a - begin:#x}" if begin <= a < end else candidate.label(a)
        out.append(f"{mnemonic} {ADDRESS.sub(named, operands)}")
    return anonymous("\n".join(out))


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--original", type=Path, required=True, help="original DLL (read only)")
    parser.add_argument("--candidate-dll", type=Path, required=True, help="the candidate DLL, linked with /LTCG")
    parser.add_argument("--map", type=Path, required=True, help="the candidate's linker /MAP file")
    parser.add_argument("--symbol", action="append", help="compare only these exports (repeatable)")
    parser.add_argument("--targets", type=Path, help="manifest of internal targets (default: the repository's)")
    parser.add_argument("--allow-unverified", action="store_true", help="report unverified targets without failing")
    parser.add_argument("--score", action="store_true", help="add each failing export's similarity (0-1)")
    args = parser.parse_args(argv)
    targets = args.targets or (DEFAULT_TARGETS if DEFAULT_TARGETS.is_file() else None)
    comparison = load_comparison(args.original, args.candidate_dll,
                                 args.map.read_text(encoding="utf-8", errors="replace"), targets)
    results = results_of(comparison, args.symbol)
    failed = 0
    for r in results:
        ok = r.ok and (args.allow_unverified or not r.unverified)
        failed += not ok
        note = f" [{r.unverified} unverified targets]" if r.unverified else ""
        if args.score and not r.ok and r.name in comparison.c.image.exports and r.name in comparison.o.image.exports:
            note += f" [similarity {comparison.similarity(r.name):.3f}]"
        print(f"{'OK  ' if ok else 'FAIL'} {r.name}: {r.reason}{note}")
    print(f"{len(results) - failed}/{len(results)} exports match")
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
