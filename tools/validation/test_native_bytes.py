"""Checks for the native per-function byte harness: comparison rules, COFF reading, and the installed Mathcad DLL."""
import contextlib
import io
import json
import os
import struct
import tempfile
import unittest
from pathlib import Path
from unittest import mock
from native_bytes import (Entry, Function, InternalTargets, ManifestError, Reloc, PeImage, check_targets,
                          classify_targets, compare_code, compare_object, definitions_in_object,
                          definitions_in_objects, functions_in_object,
                          load_targets, masked, read_coff, system_ordinal_names, targets_from, _thunk_slot)
from native_bytes import main as native_bytes_main
from native_targets import record

MATHCAD_DLL = Path(os.environ.get("MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0")) / "efistatsci_.dll"
MSHEX_DLL = MATHCAD_DLL.with_name("mshex.dll")
UTL_DLL = MATHCAD_DLL.with_name("utl.dll")
MSPCONS_DLL = MATHCAD_DLL.with_name("mspcons.dll")
OLEAUT32_DLL = Path(os.environ.get("SystemRoot", r"C:\Windows")) / "System32" / "oleaut32.dll"


def coff(functions, externals=(), statics=(), local=(), data=(), comdat=None, labels=(), weak=()):
    """A minimal x64 COFF object: one .text section per function, each an external function symbol with its relocations.

    `functions` is a list of (decorated name, code bytes, [(offset, reloc type, symbol index)]). Undefined external
    symbols named in `externals` follow the function symbols, then local (static) symbols named in `statics`, so
    relocations can target either. Weak externals in `weak`, as (name, index of the default symbol), follow those, each
    with its auxiliary record. Defined symbols named in `local` are static, and those named in `data` are not
    functions. `comdat` maps a function name to the COMDAT selection of its section, and `labels` adds code labels
    ($LN5) as (name, function index, offset); both come after every symbol a relocation can name. Long names go through
    the string table.
    """
    comdat = comdat or {}
    code_start = 20 + 40 * len(functions)
    reloc_start = code_start + sum(len(code) for _, code, _ in functions)
    total_relocs = sum(len(rels) for _, _, rels in functions)
    sections, raw, relocs, symbols, strings = b"", b"", b"", b"", b""
    body, reloc_count = code_start, 0
    for name, code, rels in functions:
        ptr = reloc_start + 10 * reloc_count
        flags = 0x60500020 | (0x1000 if name in comdat else 0)
        sections += struct.pack("<8sIIIIIIHHI", b".text", 0, 0, len(code), body, ptr, 0, len(rels), 0, flags)
        body += len(code)
        reloc_count += len(rels)
        raw += code
        relocs += b"".join(struct.pack("<IIH", offset, sym, kind) for offset, kind, sym in rels)
    symtab = reloc_start + 10 * total_relocs

    def symbol(name, section, typ, storage, value=0, aux=0):
        nonlocal strings
        if len(name) <= 8:
            raw_name = name.encode().ljust(8, b"\0")
        else:
            raw_name = struct.pack("<II", 0, 4 + len(strings))  # offset in the string table, which starts after its size
            strings += name.encode() + b"\0"
        return struct.pack("<8sIhHBB", raw_name, value, section, typ, storage, aux)

    for i, (name, _, _) in enumerate(functions):
        symbols += symbol(name, i + 1, 0 if name in data else 0x20, 3 if name in local else 2)
    for name in externals:
        symbols += symbol(name, 0, 0, 2)
    for name in statics:
        symbols += symbol(name, 0, 0, 3)
    for name, default in weak:  # IMAGE_WEAK_EXTERN_SEARCH_ALIAS (3) in the auxiliary record
        symbols += symbol(name, 0, 0, 105, aux=1) + struct.pack("<II10x", default, 3)
    count = len(functions) + len(externals) + len(statics) + 2 * len(weak)
    for i, (name, code, _) in enumerate(functions):
        if name in comdat:  # the section definition symbol and its auxiliary record, selection at offset 14
            symbols += symbol(".text", i + 1, 0, 3, aux=1) + struct.pack("<IHHIHB3x", len(code), 0, 0, 0, 0, comdat[name])
            count += 2
    for name, index, offset in labels:
        symbols += symbol(name, index + 1, 0, 6, value=offset)
        count += 1
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

    def read(self, rva, size):
        return bytes(self.memory[rva:rva + size])

    def contains(self, rva, size):
        return 0 <= rva and rva + size <= len(self.memory)

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
        self.assertTrue(compare_code("f", b"\x90\xc3", b"\x90\xc3", []).ok)

    def test_length_difference_fails(self):
        result = compare_code("f", b"\x90\xc3", b"\x90\x90\xc3", [])
        self.assertFalse(result.ok)
        self.assertIn("length", result.reason)

    def test_byte_difference_reports_its_offset(self):
        result = compare_code("f", b"\x83\xc0\x03\xc3", b"\x83\xc0\x04\xc3", [])
        self.assertFalse(result.ok)
        self.assertIn("+0x2", result.reason)

    def test_displacement_at_a_relocation_site_is_ignored(self):
        # The linked original has a different displacement where the object has a relocation.
        original = b"\x48\x8d\x05\x11\x22\x33\x44\xc3"
        candidate = b"\x48\x8d\x05\x00\x00\x00\x00\xc3"
        self.assertTrue(compare_code("f", original, candidate, [Reloc(3, 0x0004)]).ok)


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

    def thunk(self, slot):
        """An import thunk at 0x1100 (the call's target) that jumps through `slot`."""
        self.memory[0x1100:0x1102] = b"\xff\x25"
        struct.pack_into("<i", self.memory, 0x1102, slot - (0x1100 + 6))

    def test_call_through_an_import_thunk_for_the_name_verifies(self):
        self.thunk(0x1800)
        self.assertEqual(check_targets(FakeImage({}, self.memory, imports={0x1800: "callee"}), 0x1000, self.call),
                         ([], 0))

    def test_call_through_an_import_thunk_for_another_name_is_a_mismatch(self):
        self.thunk(0x1800)
        mismatches, _ = check_targets(FakeImage({}, self.memory, imports={0x1800: "strlen"}), 0x1000, self.call)
        self.assertEqual(len(mismatches), 1)
        self.assertIn("import thunk for strlen", mismatches[0])

    def test_jump_through_a_slot_that_is_not_an_import_stays_unverified(self):
        self.thunk(0x1800)
        self.assertEqual(check_targets(FakeImage({}, self.memory), 0x1000, self.call), ([], 1))

    def test_absolute_address_resolves_against_the_image_base(self):
        # mov rax, imm64: the field holds the candidate's addend (0).
        struct.pack_into("<Q", self.memory, 0x1002, FakeImage.image_base + 0x1300)
        addr = Function("loadaddr", b"\x48\xb8" + bytes(8) + b"\xc3", [Reloc(2, 0x0001, 0)], {2: "data"})
        self.assertEqual(self.check({"data": 0x1300}, function=addr), ([], 0))
        self.assertEqual(len(self.check({"data": 0x1400}, function=addr)[0]), 1)

    def load(self, addend, kind=0x0004, extra=b""):
        """`mov rax, [rip+disp]` (field at +3) whose candidate field holds `addend`; `extra` follows the field."""
        code = bytearray(b"\x48\x8b\x05\0\0\0\0" + extra + b"\xc3")
        struct.pack_into("<i", code, 3, addend)
        return Function("load", bytes(code), [Reloc(3, kind, 0)], {3: "s_data"})

    def point(self, rva, n=0):
        """The original's field at 0x1003 reaches `rva` (REL32_n: n immediate bytes follow the field)."""
        struct.pack_into("<i", self.memory, 0x1003, rva - (0x1003 + 4 + n))

    def test_export_target_includes_the_candidates_addend(self):
        self.point(0x1100)
        mismatches, _ = self.check({"s_data": 0x1100}, function=self.load(4))
        self.assertEqual(len(mismatches), 1)
        self.assertIn("s_data + 0x4", mismatches[0])
        self.point(0x1104)
        self.assertEqual(self.check({"s_data": 0x1100}, function=self.load(4)), ([], 0))
        self.assertEqual(len(self.check({"s_data": 0x1100}, function=self.load(0))[0]), 1)

    def test_rel32_n_target_includes_the_addend(self):
        # cmp dword [rip+disp], imm32 is REL32_4: the field is followed by four immediate bytes.
        self.point(0x1108, n=4)
        self.assertEqual(self.check({"s_data": 0x1100}, function=self.load(8, 0x0008, b"\x05\0\0\0")), ([], 0))
        self.assertEqual(len(self.check({"s_data": 0x1100}, function=self.load(0, 0x0008, b"\x05\0\0\0"))[0]), 1)

    def test_import_slot_includes_the_addend(self):
        self.point(0x1100)
        load = self.load(8)
        load.targets = {3: "__imp_alloc"}
        mismatches, _ = check_targets(FakeImage({}, self.memory, imports={0x1100: "alloc"}), 0x1000, load)
        self.assertIn("not an import slot + 0x8", mismatches[0])

    def test_image_base_is_rva_zero(self):
        self.point(0)
        load = self.load(0)
        load.targets = {3: "__ImageBase"}
        self.assertEqual(self.check({}, function=load), ([], 0))
        self.point(0x1100)
        self.assertIn("image base is 0x0", self.check({}, function=load)[0][0])


class ManifestImage(FakeImage):
    """FakeImage with what the manifest needs: a name and hash to bind to, .pdata function extents, and data reads."""

    name = "fake.dll"
    sha256 = "ab" * 32

    def __init__(self, memory, functions, exports=None, imports=None, leaves=None):
        super().__init__(exports or {}, memory, imports)
        self.functions = functions
        self.leaves = leaves or {}

    def function_extent(self, rva, starts=()):
        end = self.functions.get(rva, self.leaves.get(rva))
        return None if end is None else (rva, end)

    def export_extent(self, name):
        rva = self.exports.get(name)
        return None if rva is None else (rva, self.functions[rva])

    def export_code(self, name):
        rva = self.exports[name]
        return self.read(rva, self.functions[rva] - rva)


class StrictRunTests(unittest.TestCase):
    """native_bytes.main end to end, on synthetic objects and an image where export `caller` (RVA 0x1000) calls the
    internal function at RVA 0x1100."""

    def setUp(self):
        memory = bytearray(0x2000)
        memory[0x1000:0x1006] = b"\xe8\0\0\0\0\xc3"
        struct.pack_into("<i", memory, 0x1001, 0x1100 - (0x1001 + 4))
        memory[0x1100:0x1103] = b"\x33\xc0\xc3"
        self.image = ManifestImage(memory, {0x1000: 0x1006, 0x1100: 0x1103}, {"caller": 0x1000})
        tmp = tempfile.TemporaryDirectory()
        self.addCleanup(tmp.cleanup)
        self.tmp = Path(tmp.name)
        self.caller = self.obj("caller.obj", [("caller", b"\xe8\0\0\0\0\xc3", [(1, 0x0004, 1)])], externals=["callee"])
        self.callee = self.obj("callee.obj", [("callee", b"\x33\xc0\xc3", [])])
        self.manifest = self.write("targets.json", {"fake.dll": {"sha256": ManifestImage.sha256,
                                                                 "targets": {"callee": {"rva": "0x1100"}}}})

    def obj(self, name, functions, **kw):
        path = self.tmp / name
        path.write_bytes(coff(functions, **kw))
        return str(path)

    def write(self, name, doc):
        path = self.tmp / name
        path.write_text(json.dumps(doc))
        return str(path)

    def run_main(self, *args, flags=("--strict",)):
        out = io.StringIO()
        with mock.patch("native_bytes.PeImage", lambda path: self.image), contextlib.redirect_stdout(out):
            code = native_bytes_main(["--original", "fake.dll", *flags, *args])
        return code, out.getvalue()

    def test_unverified_targets_fail_by_default(self):
        args = ("--candidate", self.caller, "--targets", self.write("empty.json", {}))
        self.assertEqual(self.run_main(*args, flags=())[0], 1)
        code, out = self.run_main(*args, flags=("--allow-unverified",))
        self.assertEqual(code, 0, out)
        self.assertIn("(unverified allowed)", out)

    def test_an_unsupported_relocation_fails_only_its_function(self):
        bad = self.obj("bad.obj", [("caller", b"\xe8\0\0\0\0\xc3", [(1, 0x0004, 2)]),
                                   ("secrel", b"\x90\0\0\0\0\xc3", [(1, 0x000B, 2)])], externals=["callee"])
        self.image.exports["secrel"] = 0x1100
        code, out = self.run_main("--candidate", bad, "--candidate", self.callee, "--targets", self.manifest)
        self.assertEqual(code, 1)
        self.assertIn("OK   caller", out)
        self.assertIn("FAIL secrel: unsupported relocation type 0xb at +0x1", out)

    def test_an_unexported_inline_comdat_is_skipped_in_an_unfiltered_run(self):
        inline = self.obj("inline.obj", [("caller", b"\xe8\0\0\0\0\xc3", [(1, 0x0004, 2)]), ("??_Gx", b"\x90\xc3", [])],
                          externals=["callee"], comdat={"caller": 1, "??_Gx": 2})
        code, out = self.run_main("--candidate", inline, "--candidate", self.callee, "--targets", self.manifest)
        self.assertEqual(code, 0, out)
        self.assertNotIn("??_Gx", out)
        # Named explicitly, it is still compared; an ordinary (NODUPLICATES) definition is never skipped.
        self.assertIn("FAIL ??_Gx: not an export", self.run_main("--candidate", inline, "--symbol", "??_Gx")[1])
        plain = self.obj("plain.obj", [("helper", b"\x90\xc3", [])], comdat={"helper": 1})
        self.assertIn("FAIL helper: not an export", self.run_main("--candidate", self.caller, "--candidate", plain,
                                                                  "--candidate", self.callee,
                                                                  "--targets", self.manifest)[1])

    def test_an_exported_inline_comdat_is_compared(self):
        inline = self.obj("inline.obj", [("caller", b"\xe8\0\0\0\0\xcc", [(1, 0x0004, 1)])], externals=["callee"],
                          comdat={"caller": 2})
        code, out = self.run_main("--candidate", inline, "--candidate", self.callee, "--targets", self.manifest)
        self.assertEqual(code, 1)
        self.assertIn("FAIL caller: first byte difference", out)

    def test_exported_data_is_compared_for_the_candidates_size(self):
        self.image.memory[0x1900:0x1904] = b"abcd"
        self.image.exports["s_data"] = 0x1900
        for constant, verdict in ((b"abcd", "OK  "), (b"abce", "FAIL")):
            data = self.obj(f"data-{verdict.strip()}.obj", [("s_data", constant, [])], data=["s_data"])
            code, out = self.run_main("--candidate", self.caller, "--candidate", self.callee, "--candidate", data,
                                      "--targets", self.manifest)
            self.assertIn(f"{verdict} s_data: ", out)
            self.assertEqual(code, int(verdict == "FAIL"), out)

    def test_code_labels_do_not_end_a_function(self):
        labelled = self.obj("label.obj", [("caller", b"\xe8\0\0\0\0\xc3", [(1, 0x0004, 1)])], externals=["callee"],
                            labels=[("$LN3", 0, 5)])
        (func,) = functions_in_object(labelled)
        self.assertEqual(func.code, b"\xe8\0\0\0\0\xc3")

    def test_internal_call_verified_by_the_manifest_passes_strict(self):
        code, out = self.run_main("--candidate", self.caller, "--candidate", self.callee, "--targets", self.manifest)
        self.assertEqual(code, 0, out)
        self.assertIn("1 verified by manifest", out)
        self.assertIn("OK   callee (at manifest RVA)", out)

    def test_toolchain_entries_are_listed(self):
        manifest = self.write("toolchain.json", {"fake.dll": {"sha256": ManifestImage.sha256,
                                                              "targets": {"callee": {"rva": "0x1100", "toolchain": True}}}})
        code, out = self.run_main("--candidate", self.caller, "--targets", manifest)
        self.assertEqual(code, 0, out)
        self.assertIn("OK   callee (toolchain entry)", out)

    def test_a_candidate_function_defining_a_toolchain_entry_fails(self):
        manifest = self.write("toolchain.json", {"fake.dll": {"sha256": ManifestImage.sha256,
                                                              "targets": {"callee": {"rva": "0x1800", "toolchain": True}}}})
        code, out = self.run_main("--candidate", self.callee, "--targets", manifest)
        self.assertEqual(code, 1)
        self.assertIn("FAIL callee: the candidate objects define a toolchain entry", out)

    def test_parked_entries_are_listed_and_their_bytes_not_compared(self):
        manifest = self.write("parked.json", {"fake.dll": {"sha256": ManifestImage.sha256,
                                                           "targets": {"callee": {"rva": "0x1100", "parked": True}}}})
        code, out = self.run_main("--candidate", self.caller, "--candidate", self.callee, "--targets", manifest)
        self.assertEqual(code, 0, out)
        self.assertIn("OK   callee (parked entry)", out)
        self.assertNotIn("callee (at manifest RVA)", out)

    def test_without_the_manifest_the_internal_call_is_unverified(self):
        code, out = self.run_main("--candidate", self.caller, "--candidate", self.callee, "--symbol", "caller",
                                  "--targets", self.write("empty.json", {}))
        self.assertEqual(code, 1)
        self.assertIn("1 unverified targets", out)
        code, out = self.run_main("--candidate", self.caller, "--targets", str(self.tmp / "missing.json"))
        self.assertEqual(code, 1)

    def test_callee_with_other_bytes_fails(self):
        other = self.obj("other.obj", [("callee", b"\x31\xc0\xc3", [])])
        code, out = self.run_main("--candidate", self.caller, "--candidate", other, "--targets", self.manifest)
        self.assertEqual(code, 1)
        self.assertIn("FAIL manifest entry callee: mismatch", out)

    def test_two_objects_defining_a_name_differently_fail(self):
        other = self.obj("other.obj", [("callee", b"\x31\xc0\xc3", [])])
        code, out = self.run_main("--candidate", self.caller, "--candidate", self.callee, "--candidate", other,
                                  "--targets", self.manifest)
        self.assertEqual(code, 1)
        self.assertIn("defined differently", out)

    def test_an_entry_nothing_references_is_still_checked(self):
        stale = self.obj("stale.obj", [("old", b"\x90\x90\xc3", [])], local=["old"])
        manifest = self.write("stale.json", {"fake.dll": {"sha256": ManifestImage.sha256, "targets": {
            "callee": {"rva": "0x1100"}, "old": {"rva": "0x1100"}}}})
        code, out = self.run_main("--candidate", self.caller, "--candidate", self.callee, "--candidate", stale,
                                  "--targets", manifest)
        self.assertEqual(code, 1)
        self.assertIn("FAIL old (manifest entry)", out)

    def test_an_entry_recorded_under_another_anonymous_namespace_tag_verifies(self):
        # The same helper built from another checkout path: MSVC tags its anonymous namespace differently.
        here, there = "?h@?A0x4b90f393@u@@YAXXZ", "?h@?A0x377d720e@u@@YAXXZ"
        caller = self.obj("anon-caller.obj", [("caller", b"\xe8\0\0\0\0\xc3", [(1, 0x0004, 1)])], statics=[here])
        helper = self.obj("anon-helper.obj", [(here, b"\x33\xc0\xc3", [])], local=[here])
        manifest = self.write("anon.json", {"fake.dll": {"sha256": ManifestImage.sha256,
                                                         "targets": {there: {"rva": "0x1100"}}}})
        code, out = self.run_main("--candidate", caller, "--candidate", helper, "--targets", manifest)
        self.assertEqual(code, 0, out)
        self.assertIn("OK   caller: 6 bytes, 1 relocation sites masked [1 verified by manifest]", out)

    def test_helpers_that_differ_only_in_their_anonymous_namespace_tag_fail_closed(self):
        a = self.obj("a.obj", [("?h@?A0x00000001@u@@YAXXZ", b"\xc3", [])], local=["?h@?A0x00000001@u@@YAXXZ"])
        b = self.obj("b.obj", [("?h@?A0x00000002@u@@YAXXZ", b"\xc3", [])], local=["?h@?A0x00000002@u@@YAXXZ"])
        code, out = self.run_main("--candidate", self.caller, "--candidate", a, "--candidate", b,
                                  "--targets", self.manifest)
        self.assertEqual(code, 1)
        self.assertIn("differ only in their anonymous-namespace tag", out)

    def test_manifest_for_another_build_fails_the_run(self):
        manifest = self.write("other.json", {"fake.dll": {"sha256": "cd" * 32, "targets": {}}})
        code, out = self.run_main("--candidate", self.caller, "--targets", manifest)
        self.assertEqual(code, 1)
        self.assertIn("SHA-256", out)


class LoadTargetsTests(unittest.TestCase):
    def setUp(self):
        self.image = ManifestImage(bytearray(0x2000), {0x1100: 0x1103})
        self.tmp = tempfile.TemporaryDirectory()
        self.addCleanup(self.tmp.cleanup)

    def load(self, doc):
        path = Path(self.tmp.name) / "targets.json"
        path.write_text(json.dumps(doc))
        return load_targets(path, self.image)

    def bound(self, targets, sha256=ManifestImage.sha256, dll="FAKE.dll"):
        return {dll: {"sha256": sha256, "targets": targets}}

    def test_entries_of_the_original_are_read_by_case_insensitive_dll_name(self):
        entries = self.load(self.bound({"f": {"rva": "0x1100"}, "vt": {"rva": "0x1800", "size": 16}}))
        self.assertEqual(entries, {"f": Entry(0x1100), "vt": Entry(0x1800, 16)})

    def test_a_dll_the_manifest_does_not_list_has_no_entries(self):
        self.assertEqual(self.load(self.bound({"f": {"rva": "0x1100"}}, dll="other.dll")), {})

    def test_entries_for_another_build_fail(self):
        with self.assertRaisesRegex(ManifestError, "SHA-256"):
            self.load(self.bound({"f": {"rva": "0x1100"}}, sha256="cd" * 32))

    def test_function_entry_must_start_a_pdata_function(self):
        with self.assertRaisesRegex(ManifestError, ".pdata"):
            self.load(self.bound({"f": {"rva": "0x1101"}}))

    def test_function_entry_may_name_a_leaf_function(self):
        self.image.leaves = {0x1200: 0x1203}
        self.assertEqual(self.load(self.bound({"leaf": {"rva": "0x1200"}})), {"leaf": Entry(0x1200)})

    def test_data_entry_must_lie_in_a_section(self):
        with self.assertRaisesRegex(ManifestError, "not in a section"):
            self.load(self.bound({"vt": {"rva": "0x1ff8", "size": 16}}))
        with self.assertRaisesRegex(ManifestError, "not in a section"):
            self.load(self.bound({"vt": {"rva": "0x1800", "size": 0}}))

    def test_malformed_entry_and_missing_file_fail(self):
        with self.assertRaisesRegex(ManifestError, "malformed"):
            self.load(self.bound({"f": {"offset": "0x1100"}}))
        with self.assertRaises(ManifestError):
            load_targets(Path(self.tmp.name) / "missing.json", self.image)

    def test_entries_that_differ_only_in_their_anonymous_namespace_tag_fail(self):
        with self.assertRaisesRegex(ManifestError, "anonymous-namespace tag"):
            self.load(self.bound({"?f@?A0x1@u@@YAXXZ": {"rva": "0x1100"}, "?f@?A0x2@u@@YAXXZ": {"rva": "0x1100"}}))

    def test_toolchain_entry_takes_an_rva_in_a_section_and_no_size(self):
        self.assertEqual(self.load(self.bound({"t": {"rva": "0x1800", "toolchain": True}})), {"t": Entry(0x1800, None, True)})
        for bad in ({"rva": "0x1800", "size": 8, "toolchain": True}, {"rva": "0x3000", "toolchain": True}):
            with self.assertRaisesRegex(ManifestError, "toolchain entry"):
                self.load(self.bound({"t": bad}))
        with self.assertRaisesRegex(ManifestError, "malformed"):
            self.load(self.bound({"t": {"rva": "0x1800", "toolchain": "yes"}}))

    def test_parked_entry_is_a_function_without_size(self):
        self.assertEqual(self.load(self.bound({"t": {"rva": "0x1100", "parked": True}})),
                         {"t": Entry(0x1100, None, False, True)})
        for bad in ({"rva": "0x1800", "size": 8, "parked": True}, {"rva": "0x1100", "toolchain": True, "parked": True}):
            with self.assertRaisesRegex(ManifestError, "parked entry"):
                self.load(self.bound({"t": bad}))
        with self.assertRaisesRegex(ManifestError, "malformed"):
            self.load(self.bound({"t": {"rva": "0x1100", "parked": "yes"}}))


class InternalTargetTests(unittest.TestCase):
    """A caller at RVA 0x1000 calls (E8, displacement at +1) the internal function `callee`, at RVA 0x1100 (xor eax,
    eax; ret), which the original does not export."""

    CALLEE = b"\x33\xc0\xc3"

    def setUp(self):
        self.memory = bytearray(0x2000)
        struct.pack_into("<i", self.memory, 0x1001, 0x1100 - (0x1001 + 4))
        self.memory[0x1100:0x1103] = self.CALLEE
        self.functions = {0x1100: 0x1103, 0x1200: 0x1206}
        self.call = Function("caller", b"\xe8\0\0\0\0\xc3", [Reloc(1, 0x0004, 0)], {1: "callee"})
        self.definitions = {"callee": Function("callee", self.CALLEE, [], {})}

    def classify(self, entries, function=None, definitions=None):
        image = ManifestImage(self.memory, self.functions)
        internal = InternalTargets(image, entries, self.definitions if definitions is None else definitions)
        return classify_targets(image, 0x1000, function or self.call, internal)

    def test_call_to_an_entry_whose_bytes_match_verifies(self):
        found = self.classify({"callee": Entry(0x1100)})
        self.assertEqual((found.mismatches, found.unverified, found.verified, found.entries), ([], 0, 1, {}))

    def test_static_target_is_looked_up_by_its_name(self):
        static_call = Function("caller", self.call.code, self.call.relocs, {1: None}, {1: "callee"})
        self.assertEqual(self.classify({"callee": Entry(0x1100)}, function=static_call).verified, 1)

    def test_target_without_an_entry_stays_unverified(self):
        found = self.classify({})
        self.assertEqual((found.mismatches, found.unverified, found.verified), ([], 1, 0))
        image = ManifestImage(self.memory, self.functions)
        self.assertEqual(check_targets(image, 0x1000, self.call), ([], 1))

    def test_entry_at_another_rva_than_the_call_site_is_a_mismatch(self):
        found = self.classify({"callee": Entry(0x1200)})
        self.assertEqual((len(found.mismatches), found.verified), (1, 0))
        self.assertIn("manifest entry gives 0x1200", found.mismatches[0])
        self.assertEqual(found.entries["callee"][0], "mismatch")

    def test_callee_whose_bytes_differ_is_a_mismatch(self):
        found = self.classify({"callee": Entry(0x1100)}, definitions={"callee": Function("callee", b"\x31\xc0\xc3", [], {})})
        self.assertEqual(len(found.mismatches), 1)
        self.assertIn("first byte difference", found.mismatches[0])
        longer = self.classify({"callee": Entry(0x1100)}, definitions={"callee": Function("callee", b"\x90" * 4, [], {})})
        self.assertIn("length", longer.mismatches[0])

    def test_toolchain_entry_verifies_by_its_rva(self):
        found = self.classify({"callee": Entry(0x1100, None, True)}, definitions={})
        self.assertEqual((found.mismatches, found.unverified, found.verified), ([], 0, 1))
        elsewhere = self.classify({"callee": Entry(0x1200, None, True)}, definitions={})
        self.assertIn("manifest entry gives 0x1200", elsewhere.mismatches[0])

    def test_manifest_entry_takes_precedence_over_an_import_thunk(self):
        self.memory[0x1100:0x1102] = b"\xff\x25"
        struct.pack_into("<i", self.memory, 0x1102, 0x1800 - (0x1100 + 6))
        image = ManifestImage(self.memory, self.functions, imports={0x1800: "other"})
        internal = InternalTargets(image, {"callee": Entry(0x1100, None, True)}, {})
        found = classify_targets(image, 0x1000, self.call, internal)
        self.assertEqual((found.mismatches, found.verified), ([], 1))

    def test_toolchain_entry_the_candidates_define_is_a_mismatch(self):
        found = self.classify({"callee": Entry(0x1100, None, True)})
        self.assertEqual(found.entries["callee"], ("mismatch", "the candidate objects define a toolchain entry"))

    def test_parked_entry_verifies_by_its_rva_whatever_the_candidate_defines(self):
        attempt = {"callee": Function("callee", b"\x31\xc0\xc3", [], {})}  # differs from the original's bytes
        for definitions in (attempt, {}):
            found = self.classify({"callee": Entry(0x1100, None, False, True)}, definitions=definitions)
            self.assertEqual((found.mismatches, found.unverified, found.verified), ([], 0, 1))
        elsewhere = self.classify({"callee": Entry(0x1200, None, False, True)}, definitions=attempt)
        self.assertIn("manifest entry gives 0x1200", elsewhere.mismatches[0])

    def test_leaf_entry_is_compared_over_its_inferred_extent(self):
        image = ManifestImage(self.memory, {0x1200: 0x1206}, leaves={0x1100: 0x1103})
        internal = InternalTargets(image, {"callee": Entry(0x1100)}, self.definitions)
        found = classify_targets(image, 0x1000, self.call, internal)
        self.assertEqual((found.mismatches, found.unverified, found.verified), ([], 0, 1))

    def test_entry_the_candidates_do_not_define_is_unverified(self):
        found = self.classify({"callee": Entry(0x1100)}, definitions={})
        self.assertEqual((found.mismatches, found.unverified, found.verified), ([], 1, 0))
        self.assertEqual(found.entries["callee"], ("unverified", "not defined by the candidate objects"))

    def test_entry_that_references_an_unverified_target_is_unverified(self):
        # callee at 0x1200: jmp to 0x1100, which has no entry.
        struct.pack_into("<i", self.memory, 0x1201, 0x1100 - (0x1201 + 4))
        callee = Function("callee", b"\xe9\0\0\0\0\xc3", [Reloc(1, 0x0004, 1)], {1: "helper"})
        self.memory[0x1200] = 0xE9
        self.memory[0x1205] = 0xC3
        struct.pack_into("<i", self.memory, 0x1001, 0x1200 - (0x1001 + 4))
        found = self.classify({"callee": Entry(0x1200)}, definitions={"callee": callee})
        self.assertEqual((found.mismatches, found.unverified, found.verified), ([], 1, 0))
        # With the helper recorded and defined, the chain verifies.
        found = self.classify({"callee": Entry(0x1200), "helper": Entry(0x1100)},
                              definitions={"callee": callee, "helper": Function("helper", self.CALLEE, [], {})})
        self.assertEqual((found.mismatches, found.unverified, found.verified), ([], 0, 1))

    def mutual(self, b_code):
        """callee (0x1100) and peer (0x1200) each jump to the other; the original's peer is `jmp; ret`."""
        self.memory[0x1100:0x1106] = b"\xe9\0\0\0\0\xc3"
        struct.pack_into("<i", self.memory, 0x1101, 0x1200 - (0x1101 + 4))
        self.memory[0x1200:0x1206] = b"\xe9\0\0\0\0\xc3"
        struct.pack_into("<i", self.memory, 0x1201, 0x1100 - (0x1201 + 4))
        self.functions[0x1100] = 0x1106
        return self.classify({"callee": Entry(0x1100), "peer": Entry(0x1200)}, definitions={
            "callee": Function("callee", b"\xe9\0\0\0\0\xc3", [Reloc(1, 0x0004, 0)], {1: "peer"}),
            "peer": Function("peer", b_code, [Reloc(1, 0x0004, 0)], {1: "callee"})})

    def test_entries_that_reference_each_other_verify_together(self):
        found = self.mutual(b"\xe9\0\0\0\0\xc3")
        self.assertEqual((found.mismatches, found.unverified, found.verified), ([], 0, 1))

    def test_a_cycle_with_one_bad_entry_fails_through_the_good_one(self):
        found = self.mutual(b"\xe9\0\0\0\0\xcc")
        self.assertEqual(len(found.mismatches), 1)
        self.assertIn("references peer", found.mismatches[0])

    def vtable(self, slots_code=None, size=8, addend=0):
        """caller loads the address of vtable `vt` (RVA 0x1800, one slot holding callee's address) into rax."""
        struct.pack_into("<Q", self.memory, 0x1800, FakeImage.image_base + 0x1100)
        code = bytearray(b"\x48\x8d\x05\0\0\0\0\xc3")
        struct.pack_into("<i", code, 3, addend)
        struct.pack_into("<i", self.memory, 0x1003, 0x1800 + addend - (0x1003 + 4))
        caller = Function("caller", bytes(code), [Reloc(3, 0x0004, 0)], {3: "vt"})
        vt = Function("vt", slots_code or bytes(8), [Reloc(0, 0x0001, 0)], {0: "callee"}, data=True)
        return self.classify({"vt": Entry(0x1800, size), "callee": Entry(0x1100)}, function=caller,
                             definitions={"vt": vt, "callee": self.definitions["callee"]})

    def test_vtable_verifies_through_the_functions_its_slots_hold(self):
        self.assertEqual(self.vtable().verified, 1)
        self.assertEqual(self.vtable(addend=8).verified, 1)

    def test_vtable_slot_to_a_function_whose_bytes_differ_is_a_mismatch(self):
        self.definitions["callee"] = Function("callee", b"\x31\xc0\xc3", [], {})
        found = self.vtable()
        self.assertEqual(len(found.mismatches), 1)
        self.assertIn("references callee", found.mismatches[0])

    def test_data_size_must_match_the_entry(self):
        self.assertIn("size 16 != manifest 8", self.vtable(slots_code=bytes(16)).mismatches[0])

    def test_function_recorded_as_data_is_a_mismatch(self):
        found = self.classify({"callee": Entry(0x1100, 3)})
        self.assertIn("manifest entry is data", found.mismatches[0])

    def test_constant_compares_byte_for_byte(self):
        self.memory[0x1900:0x1904] = b"abcd"
        caller = Function("caller", b"\x48\x8d\x05\0\0\0\0\xc3", [Reloc(3, 0x0004, 0)], {3: "k"})
        struct.pack_into("<i", self.memory, 0x1003, 0x1900 - (0x1003 + 4))
        for constant, verified in ((b"abcd", 1), (b"abce", 0)):
            found = self.classify({"k": Entry(0x1900, 4)}, function=caller,
                                  definitions={"k": Function("k", constant, [], {}, data=True)})
            self.assertEqual((found.verified, len(found.mismatches)), (verified, 1 - verified))


class RecordTests(unittest.TestCase):
    """The export `caller` (RVA 0x1000) calls the internal `callee` (RVA 0x1100), which jumps to `helper` (0x1200)."""

    def setUp(self):
        self.memory = bytearray(0x2000)
        self.memory[0x1000:0x1006] = b"\xe8\0\0\0\0\xc3"
        struct.pack_into("<i", self.memory, 0x1001, 0x1100 - (0x1001 + 4))
        self.memory[0x1100:0x1106] = b"\xe9\0\0\0\0\xc3"
        struct.pack_into("<i", self.memory, 0x1101, 0x1200 - (0x1101 + 4))
        self.memory[0x1200:0x1203] = b"\x33\xc0\xc3"
        self.image = ManifestImage(self.memory, {0x1000: 0x1006, 0x1100: 0x1106, 0x1200: 0x1203}, {"caller": 0x1000})
        self.definitions = {
            "caller": Function("caller", b"\xe8\0\0\0\0\xc3", [Reloc(1, 0x0004, 0)], {1: "callee"}),
            "callee": Function("callee", b"\xe9\0\0\0\0\xc3", [Reloc(1, 0x0004, 0)], {1: "helper"}),
            "helper": Function("helper", b"\x33\xc0\xc3", [], {}),
        }

    def record(self, names, doc=None):
        return record(self.image, self.definitions, names, {} if doc is None else doc)

    def test_targets_are_recorded_from_matching_call_sites_and_bound_to_the_original(self):
        doc = self.record(["callee", "helper"])
        self.assertEqual(doc, {"fake.dll": {"sha256": ManifestImage.sha256, "targets": {
            "callee": {"rva": "0x1100"}, "helper": {"rva": "0x1200"}}}})
        self.assertEqual(targets_from(doc, self.image), {"callee": Entry(0x1100), "helper": Entry(0x1200)})

    def test_a_leaf_target_is_recorded(self):
        self.image = ManifestImage(self.memory, {0x1000: 0x1006, 0x1100: 0x1106}, {"caller": 0x1000},
                                   leaves={0x1200: 0x1203})
        self.assertEqual(self.record(["callee", "helper"])["fake.dll"]["targets"],
                         {"callee": {"rva": "0x1100"}, "helper": {"rva": "0x1200"}})

    def test_an_existing_entry_is_searched_for_further_targets(self):
        doc = self.record(["callee"])
        self.record(["helper"], doc)
        self.assertEqual(set(doc["fake.dll"]["targets"]), {"callee", "helper"})

    def test_data_takes_the_candidates_size(self):
        self.definitions["helper"] = Function("helper", b"\x33\xc0\xc3", [], {}, data=True)
        self.image.functions.pop(0x1200)
        self.assertEqual(self.record(["callee", "helper"])["fake.dll"]["targets"]["helper"], {"rva": "0x1200", "size": 3})

    def test_a_name_no_matching_function_references_fails(self):
        with self.assertRaisesRegex(ValueError, "not referenced"):
            self.record(["elsewhere"])
        self.definitions["caller"] = Function("caller", b"\xe8\0\0\0\0\xcc", [Reloc(1, 0x0004, 0)], {1: "callee"})
        with self.assertRaisesRegex(ValueError, "not referenced"):
            self.record(["callee"])

    def test_sites_that_disagree_fail(self):
        self.definitions["callee"] = Function("callee", b"\xe9\0\0\0\0\xc3", [Reloc(1, 0x0004, 0)], {1: "callee"})
        with self.assertRaisesRegex(ValueError, "disagree"):
            self.record(["callee"])

    def test_data_outside_the_sections_is_not_recorded(self):
        self.definitions["helper"] = Function("helper", b"\x33\xc0\xc3", [], {}, data=True)
        struct.pack_into("<i", self.memory, 0x1101, 0x1ffe - (0x1101 + 4))
        self.memory[0x1ffe:0x2000] = b"\x33\xc0"
        with self.assertRaisesRegex(ValueError, "not in a section"):
            self.record(["callee", "helper"])

    def test_toolchain_names_are_recorded_by_rva_only(self):
        del self.definitions["helper"]
        doc = self.record(["callee"])
        record(self.image, self.definitions, [], doc, toolchain=["helper"])
        self.assertEqual(doc["fake.dll"]["targets"]["helper"], {"rva": "0x1200", "toolchain": True})

    def test_a_toolchain_name_the_candidates_define_is_not_recorded(self):
        with self.assertRaisesRegex(ValueError, "not a toolchain entry"):
            record(self.image, self.definitions, ["callee"], {}, toolchain=["helper"])
        # Refused for any definition, even one no matching function references.
        self.definitions["unused"] = Function("unused", b"\xc3", [], {})
        with self.assertRaisesRegex(ValueError, "unused: defined by the candidate objects, so not a toolchain entry"):
            record(self.image, self.definitions, [], {}, toolchain=["unused"])

    def test_parked_names_are_recorded_by_rva_and_their_targets_not_followed(self):
        # The candidate's attempt at callee differs from the original and still names helper; neither matters.
        self.definitions["callee"] = Function("callee", b"\xe9\0\0\0\0\x90", [Reloc(1, 0x0004, 0)], {1: "helper"})
        doc = record(self.image, self.definitions, [], {}, parked=["callee"])
        self.assertEqual(doc["fake.dll"]["targets"], {"callee": {"rva": "0x1100", "parked": True}})

    def test_an_entry_stored_under_another_anonymous_namespace_tag_keeps_its_name(self):
        here, there = "?callee@?A0x2@u@@YAXXZ", "?callee@?A0x1@u@@YAXXZ"
        self.definitions["caller"].targets = {1: here}
        callee = self.definitions.pop("callee")
        self.definitions["?callee@?A0x@u@@YAXXZ"] = Function(here, callee.code, callee.relocs, callee.targets)
        doc = {"fake.dll": {"sha256": ManifestImage.sha256, "targets": {there: {"rva": "0x1100"}}}}
        self.record([here, "helper"], doc)
        self.assertEqual(doc["fake.dll"]["targets"], {there: {"rva": "0x1100"}, "helper": {"rva": "0x1200"}})

    def test_an_entry_is_not_silently_changed(self):
        doc = {"fake.dll": {"sha256": ManifestImage.sha256, "targets": {"callee": {"rva": "0x1200"}}}}
        with self.assertRaisesRegex(ValueError, "remove the entry"):
            self.record(["callee"], doc)


def bss(functions, **kw):
    """coff() whose first section holds uninitialized data: no file bytes (PointerToRawData 0), as MSVC emits .bss."""
    data = bytearray(coff(functions, **kw))
    struct.pack_into("<I", data, 20 + 20, 0)
    struct.pack_into("<I", data, 20 + 36, 0xC0300080)
    return bytes(data)


class DefinitionTests(unittest.TestCase):
    def test_uninitialized_data_reads_as_zeros_not_the_file_header(self):
        # The same .bss symbol in objects with different section counts, which a read from offset 0 would see.
        with tempfile.TemporaryDirectory() as tmp:
            paths = []
            for i, extra in enumerate(([], [("f", b"\xc3", [])])):
                path = Path(tmp) / f"{i}.obj"
                path.write_bytes(bss([("_WeakValue", bytes(4), [])] + extra, data=["_WeakValue"]))
                paths.append(path)
            defs = definitions_in_objects(paths)
        self.assertEqual(defs["_WeakValue"].code, bytes(4))

    def test_static_functions_and_data_are_defined_and_static_targets_named(self):
        data = coff([("f", b"\xe8\0\0\0\0\xc3", [(1, 0x0004, 1)]), ("helper", b"\xc3", []), ("vt", bytes(8), [])],
                    local=["helper"], data=["vt"])
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "t.obj"
            path.write_bytes(data)
            funcs = functions_in_object(path)
            defs = definitions_in_object(path)
        self.assertEqual([f.name for f in funcs], ["f"])
        self.assertEqual(set(defs), {"f", "helper", "vt"})
        self.assertEqual((defs["f"].targets, defs["f"].statics), ({1: None}, {1: "helper"}))
        self.assertEqual((defs["helper"].data, defs["vt"].data), (False, True))

    def test_relocation_to_a_weak_external_names_its_default(self):
        # MSVC's vtable slot names the vector deleting destructor ??_E, an undefined weak external whose default is
        # the scalar deleting destructor ??_G (symbol 0). The slot refers to ??_G.
        data = coff([("??_G", b"\xc3", []), ("vt", bytes(8), [(0, 0x0001, 2)])],
                    data=["vt"], weak=[("??_E", 0)])
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "t.obj"
            path.write_bytes(data)
            defs = definitions_in_object(path)
        self.assertEqual(defs["vt"].targets, {0: "??_G"})


@unittest.skipUnless(MSHEX_DLL.is_file(), "Mathcad Prime install not present")
class InstalledManifestTests(unittest.TestCase):
    def test_entries_are_checked_against_the_installed_mshex(self):
        image = PeImage(MSHEX_DLL)
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "targets.json"
            start = min(image.functions)
            path.write_text(json.dumps({"mshex.dll": {"sha256": image.sha256, "targets": {"f": {"rva": hex(start)}}}}))
            self.assertEqual(load_targets(path, image), {"f": Entry(start)})
            path.write_text(json.dumps({"mshex.dll": {"sha256": image.sha256, "targets": {"f": {"rva": hex(start + 1)}}}}))
            with self.assertRaises(ManifestError):
                load_targets(path, image)


@unittest.skipUnless(MATHCAD_DLL.is_file(), "Mathcad Prime install not present")
class InstalledImageTests(unittest.TestCase):
    def test_efistatsci_export_table_matches_the_inventory(self):
        image = PeImage(MATHCAD_DLL)
        self.assertEqual(len(image.exports), 781)

    def test_every_export_has_code_bytes(self):
        image = PeImage(MATHCAD_DLL)
        self.assertTrue(all(image.export_code(name) for name in image.exports))


@unittest.skipUnless(MSPCONS_DLL.is_file(), "Mathcad Prime install not present")
class InstalledMspconsTests(unittest.TestCase):
    """mspcons.dll: `?write@tagged` (0x1410) has three chained .pdata entries, and `anyval::equals(char const *)` calls
    import thunks at 0x26A3 and 0x26A9."""

    def test_chained_pdata_entries_extend_their_primary(self):
        image = PeImage(MSPCONS_DLL)
        self.assertEqual(image.functions[0x1410], 0x151D)
        self.assertFalse({0x143D, 0x14EB, 0x14F6} & set(image.functions))

    def test_import_thunks_lead_to_their_imports(self):
        image = PeImage(MSPCONS_DLL)
        self.assertEqual([image.imports.get(_thunk_slot(image, rva)) for rva in (0x26A3, 0x26A9)],
                         ["__RTDynamicCast", "strcmp"])


@unittest.skipUnless(OLEAUT32_DLL.is_file(), "System32 OLEAUT32.dll not present")
class SystemOrdinalTests(unittest.TestCase):
    def test_ordinal_names_come_from_the_system_dll(self):
        self.assertEqual(system_ordinal_names("oleaut32.dll")[4], "SysAllocStringLen")

    def test_a_missing_system_dll_has_no_names(self):
        self.assertEqual(system_ordinal_names("no-such-quire-test.dll"), {})


@unittest.skipUnless(UTL_DLL.is_file(), "Mathcad Prime install not present")
class LoadedSectionTests(unittest.TestCase):
    """utl.dll's .data has raw size 0x800 and virtual size 0xA20; its flags at 0x22862 lie in the zero-filled tail."""

    def test_bytes_past_the_raw_size_read_as_zeros(self):
        image = PeImage(UTL_DLL)
        self.assertTrue(image.contains(0x22862, 2))
        self.assertEqual(image.read(0x22862, 2), bytes(2))

    def test_a_read_across_the_raw_size_keeps_the_file_bytes(self):
        image = PeImage(UTL_DLL)
        va, vsize, raw, raw_size = next(s for s in image.sections if s[0] <= 0x22862 < s[0] + s[1])
        self.assertGreater(vsize, raw_size)
        end = va + raw_size
        self.assertEqual(image.read(end - 4, 8), image.data[raw + raw_size - 4:raw + raw_size] + bytes(4))
        self.assertFalse(image.contains(va + vsize - 1, 2))

    def test_ordinal_import_is_named_after_the_system_export(self):
        self.assertEqual(PeImage(UTL_DLL).imports[0x16430], "SysAllocStringLen")


if __name__ == "__main__":
    unittest.main()


@unittest.skipUnless(MSPCONS_DLL.exists(), "Mathcad install not found")
class LeafExtentTests(unittest.TestCase):
    """Leaf functions of the installed mspcons.dll: pstring's vtable slots type (0x1260) and tagged::get_tag (0x1190)."""

    def setUp(self):
        self.image = PeImage(MSPCONS_DLL)

    def test_a_leaf_ends_before_its_int3_padding(self):
        self.assertEqual(self.image.function_extent(0x1260), (0x1260, 0x1268))
        self.assertEqual(self.image.function_extent(0x1190), (0x1190, 0x1195))

    def test_a_pdata_function_keeps_its_pdata_extent(self):
        self.assertEqual(self.image.function_extent(0x11D0), (0x11D0, self.image.functions[0x11D0]))

    def test_an_rva_inside_a_pdata_function_or_outside_code_is_not_a_function(self):
        self.assertIsNone(self.image.function_extent(0x11D2))
        self.assertIsNone(self.image.function_extent(0x3250))  # pstring's vftable, in .rdata

    def test_a_known_start_ends_a_leaf(self):
        self.assertEqual(self.image.function_extent(0x1260, [0x1264]), (0x1260, 0x1264))
