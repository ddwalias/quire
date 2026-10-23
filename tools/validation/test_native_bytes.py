"""Checks for the native per-function byte harness: comparison rules, COFF reading, and the installed Mathcad DLL."""
import os
import struct
import tempfile
import unittest
from pathlib import Path
from native_bytes import (Function, Reloc, PeImage, check_targets, compare_code, compare_object, functions_in_object, masked,
                          read_coff)

MATHCAD_DLL = Path(os.environ.get("MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0")) / "efistatsci_.dll"


def coff(functions, externals=(), statics=()):
    """A minimal x64 COFF object: one .text section per function, each an external function symbol with its relocations.

    `functions` is a list of (decorated name, code bytes, [(offset, reloc type, symbol index)]). Undefined external
    symbols named in `externals` follow the function symbols, then local (static) symbols named in `statics`, so
    relocations can target either. Long names go through the string table.
    """
    code_start = 20 + 40 * len(functions)
    reloc_start = code_start + sum(len(code) for _, code, _ in functions)
    total_relocs = sum(len(rels) for _, _, rels in functions)
    sections, raw, relocs, symbols, strings = b"", b"", b"", b"", b""
    body, reloc_count = code_start, 0
    for name, code, rels in functions:
        ptr = reloc_start + 10 * reloc_count
        sections += struct.pack("<8sIIIIIIHHI", b".text", 0, 0, len(code), body, ptr, 0, len(rels), 0, 0x60500020)
        body += len(code)
        reloc_count += len(rels)
        raw += code
        relocs += b"".join(struct.pack("<IIH", offset, sym, kind) for offset, kind, sym in rels)
    symtab = reloc_start + 10 * total_relocs

    def symbol(name, section, typ, storage):
        nonlocal strings
        if len(name) <= 8:
            raw_name = name.encode().ljust(8, b"\0")
        else:
            raw_name = struct.pack("<II", 0, 4 + len(strings))  # offset in the string table, which starts after its size
            strings += name.encode() + b"\0"
        return struct.pack("<8sIhHBB", raw_name, 0, section, typ, storage, 0)

    for i, (name, _, _) in enumerate(functions):
        symbols += symbol(name, i + 1, 0x20, 2)
    for name in externals:
        symbols += symbol(name, 0, 0, 2)
    for name in statics:
        symbols += symbol(name, 0, 0, 3)
    count = len(functions) + len(externals) + len(statics)
    table = struct.pack("<I", 4 + len(strings)) + strings
    head = struct.pack("<HHIIIHH", 0x8664, len(functions), 0, symtab, count, 0, 0)
    return head + sections + raw + relocs + symbols + table


class FakeImage:
    """Stands in for PeImage in call-target tests: exports by name, and a flat memory image keyed by RVA."""

    image_base = 0x180000000

    def __init__(self, exports, memory, imports=None):
        self.exports = exports
        self.memory = memory
        self.imports = imports or {}

    def read_i32(self, rva):
        return struct.unpack_from("<i", self.memory, rva)[0]

    def read_u32(self, rva):
        return struct.unpack_from("<I", self.memory, rva)[0]

    def read_u64(self, rva):
        return struct.unpack_from("<Q", self.memory, rva)[0]


class MaskingTests(unittest.TestCase):
    def test_relocation_sites_are_zeroed_at_their_width(self):
        buf, mask = masked(bytes(range(16)), [Reloc(2, 0x0004), Reloc(8, 0x0001)])
        self.assertEqual(buf[2:6], b"\0\0\0\0")
        self.assertEqual(buf[8:16], b"\0" * 8)
        self.assertEqual([i for i, m in enumerate(mask) if not m], [2, 3, 4, 5, 8, 9, 10, 11, 12, 13, 14, 15])

    def test_unknown_relocation_type_is_rejected(self):
        with self.assertRaises(ValueError):
            masked(b"\0" * 8, [Reloc(0, 0x0010)])


class CompareCodeTests(unittest.TestCase):
    def test_identical_bodies_match(self):
        self.assertTrue(compare_code("f", b"\x90\xc3", [], b"\x90\xc3", []).ok)

    def test_length_difference_fails(self):
        result = compare_code("f", b"\x90\xc3", [], b"\x90\x90\xc3", [])
        self.assertFalse(result.ok)
        self.assertIn("length", result.reason)

    def test_byte_difference_reports_its_offset(self):
        result = compare_code("f", b"\x83\xc0\x03\xc3", [], b"\x83\xc0\x04\xc3", [])
        self.assertFalse(result.ok)
        self.assertIn("+0x2", result.reason)

    def test_displacement_at_a_relocation_site_is_ignored(self):
        # The linked original has a different displacement where the object has a relocation.
        original = b"\x48\x8d\x05\x11\x22\x33\x44\xc3"
        candidate = b"\x48\x8d\x05\x00\x00\x00\x00\xc3"
        self.assertTrue(compare_code("f", original, [Reloc(3, 0x0004)], candidate, [Reloc(3, 0x0004)]).ok)

    def test_relocation_positions_must_agree(self):
        code = b"\x90" * 8
        self.assertFalse(compare_code("f", code, [Reloc(0, 0x0004)], code, [Reloc(4, 0x0004)]).ok)


class CoffTests(unittest.TestCase):
    def test_reads_functions_with_short_and_long_names(self):
        long_name = "?wavuniv_transform_gabor_2d@mutils@@YAHXZ"
        data = coff([("add3", b"\x8d\x41\x03\xc3", []), (long_name, b"\x90\xc3", [(0, 0x0004, 2)])], externals=["callee"])
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "t.obj"
            path.write_bytes(data)
            funcs = {f.name: f for f in functions_in_object(path)}
        self.assertEqual(set(funcs), {"add3", long_name})
        self.assertEqual(funcs["add3"].code, b"\x8d\x41\x03\xc3")
        self.assertEqual(funcs[long_name].relocs, [Reloc(0, 0x0004, 2)])
        self.assertEqual(funcs[long_name].targets, {0: "callee"})

    def test_relocations_to_static_symbols_have_no_external_target(self):
        data = coff([("f", b"\x48\x8d\x05\x00\x00\x00\x00\xc3", [(3, 0x0004, 1)])], statics=["local"])
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "t.obj"
            path.write_bytes(data)
            (func,) = functions_in_object(path)
        self.assertEqual(func.targets, {3: None})

    def test_rejects_non_x64_objects(self):
        data = bytearray(coff([("f", b"\xc3", [])]))
        struct.pack_into("<H", data, 0, 0x14c)
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "x86.obj"
            path.write_bytes(bytes(data))
            with self.assertRaises(ValueError):
                read_coff(path)

    @unittest.skipUnless(MATHCAD_DLL.is_file(), "Mathcad Prime install not present")
    def test_object_without_functions_fails_closed(self):
        with tempfile.TemporaryDirectory() as tmp:
            obj = Path(tmp) / "empty.obj"
            obj.write_bytes(coff([]))
            results = compare_object(MATHCAD_DLL, obj)
        self.assertEqual([(r.name, r.ok) for r in results], [("(object)", False)])


class CallTargetTests(unittest.TestCase):
    """A call at RVA 0x1000 (opcode E8, displacement at +1) whose original displacement points at RVA 0x1100."""

    def setUp(self):
        self.memory = bytearray(0x2000)
        struct.pack_into("<i", self.memory, 0x1001, 0x1100 - (0x1001 + 4))
        self.call = Function("caller", b"\xe8\0\0\0\0\xc3", [Reloc(1, 0x0004, 0)], {1: "callee"})

    def check(self, exports, function=None, memory=None):
        image = FakeImage(exports, memory if memory is not None else self.memory)
        return check_targets(image, 0x1000, function or self.call)

    def test_call_to_the_export_it_names_verifies(self):
        self.assertEqual(self.check({"callee": 0x1100}), ([], 0))

    def test_call_to_a_different_export_is_a_mismatch(self):
        mismatches, unverified = self.check({"callee": 0x1100, "other": 0x1200})
        self.assertEqual(unverified, 0)
        self.assertEqual(len(mismatches), 0)
        retargeted = Function("caller", self.call.code, self.call.relocs, {1: "other"})
        mismatches, _ = self.check({"callee": 0x1100, "other": 0x1200}, function=retargeted)
        self.assertEqual(len(mismatches), 1)
        self.assertIn("other", mismatches[0])

    def test_target_that_is_not_an_export_is_unverified_not_a_mismatch(self):
        self.assertEqual(self.check({"elsewhere": 0x1100}), ([], 1))
        self.assertEqual(self.check({}), ([], 1))

    def test_rel32_n_counts_the_extra_bytes_after_the_field(self):
        struct.pack_into("<i", self.memory, 0x1001, 0x1100 - (0x1001 + 4 + 1))
        rel32_1 = Function("caller", self.call.code, [Reloc(1, 0x0005, 0)], {1: "callee"})
        self.assertEqual(self.check({"callee": 0x1100}, function=rel32_1), ([], 0))

    def test_call_through_the_import_slot_it_names_verifies(self):
        call_import = Function("caller", self.call.code, [Reloc(1, 0x0004, 0)], {1: "__imp_alloc"})
        image = FakeImage({}, self.memory, imports={0x1100: "alloc"})
        self.assertEqual(check_targets(image, 0x1000, call_import), ([], 0))

    def test_call_through_a_different_import_is_a_mismatch(self):
        call_import = Function("caller", self.call.code, [Reloc(1, 0x0004, 0)], {1: "__imp_free"})
        image = FakeImage({}, self.memory, imports={0x1100: "alloc"})
        mismatches, _ = check_targets(image, 0x1000, call_import)
        self.assertEqual(len(mismatches), 1)
        self.assertIn("alloc", mismatches[0])

    def test_call_through_a_non_import_slot_is_a_mismatch(self):
        call_import = Function("caller", self.call.code, [Reloc(1, 0x0004, 0)], {1: "__imp_alloc"})
        mismatches, _ = check_targets(FakeImage({}, self.memory), 0x1000, call_import)
        self.assertEqual(len(mismatches), 1)
        self.assertIn("not an import slot", mismatches[0])

    def test_absolute_address_resolves_against_the_image_base(self):
        struct.pack_into("<Q", self.memory, 0x1000, FakeImage.image_base + 0x1300)
        addr = Function("loadaddr", b"\x48\x8b\x05\0\0\0\0\xc3", [Reloc(0, 0x0001, 0)], {0: "data"})
        self.assertEqual(self.check({"data": 0x1300}, function=addr), ([], 0))
        self.assertEqual(len(self.check({"data": 0x1400}, function=addr)[0]), 1)


@unittest.skipUnless(MATHCAD_DLL.is_file(), "Mathcad Prime install not present")
class InstalledImageTests(unittest.TestCase):
    def test_efistatsci_export_table_matches_the_inventory(self):
        image = PeImage(MATHCAD_DLL)
        self.assertEqual(len(image.exports), 781)

    def test_every_export_has_code_bytes(self):
        image = PeImage(MATHCAD_DLL)
        self.assertTrue(all(image.export_code(name) for name in image.exports))


if __name__ == "__main__":
    unittest.main()
