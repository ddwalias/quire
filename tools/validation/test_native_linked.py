"""Checks for the linked-DLL comparison (D30): instruction pairing and the target rules, on synthetic images."""
import os
import struct
import unittest
from pathlib import Path

from native_bytes import Entry
from native_linked import ADDRESS, Comparison, Linked, parse_disassembly, parse_map

BASE = 0x180000000


class Image:
    """Stands in for PeImage: code at 0x1000-0x2000, data at 0x2000-0x3000, a flat memory keyed by RVA."""

    image_base = BASE

    def __init__(self, functions, exports=None, imports=None, memory=None):
        self.functions = functions
        self.exports = exports or {}
        self.imports = imports or {}
        self.memory = memory if memory is not None else bytearray(0x3000)
        self.sections = [(0x1000, 0x1000, 0x400, 0x1000), (0x2000, 0x1000, 0x1400, 0x1000)]
        self.code_sections = [(0x1000, 0x2000)]

    def function_extent(self, rva, starts=()):
        return (rva, self.functions[rva]) if rva in self.functions else None

    def read(self, rva, size):
        return bytes(self.memory[rva:rva + size])

    def contains(self, rva, size):
        return 0 <= rva and rva + size <= len(self.memory)


def at(rva):
    return f"{BASE + rva:016X}"


def caller(target, mnemonic="call"):
    """A function at 0x1000: `<mnemonic> <target>` (5 bytes), then `ret` (1 byte)."""
    return {0x1000: (mnemonic, at(target)), 0x1005: ("ret", "")}


def encode(memory, ins):
    """Write each address-holding instruction's field as a linker would: rel32 in its last 4 bytes, or a short
    branch's rel8 in its last byte. Other bytes keep what the test put there."""
    addresses = sorted(ins)
    for k, rva in enumerate(addresses):
        end = addresses[k + 1] if k + 1 < len(addresses) else rva + 1
        for token in ADDRESS.findall(ins[rva][1]):
            target = int(token, 16) - BASE
            if end - rva >= 5:
                struct.pack_into("<i", memory, end - 4, target - end)
            else:
                struct.pack_into("<b", memory, end - 1, target - end)
    return memory


class Pairing:
    def pair(self, o_ins, c_ins, o_functions, c_functions, o_names=None, c_names=None, entries=None, **kw):
        mo = encode(kw.get("o_memory") if kw.get("o_memory") is not None else bytearray(0x3000), o_ins)
        mc = encode(kw.get("c_memory") if kw.get("c_memory") is not None else bytearray(0x3000), c_ins)
        o = Linked(Image(o_functions, kw.get("o_exports", {"f": 0x1000}), kw.get("o_imports"), mo),
                   o_ins, o_names or {}, kw.get("o_relocs", set()))
        c = Linked(Image(c_functions, kw.get("c_exports", {"f": 0x1000}), kw.get("c_imports"), mc),
                   c_ins, c_names or {}, kw.get("c_relocs", set()))
        return Comparison(o, c, entries or {}).export("f")


class LinkedTests(Pairing, unittest.TestCase):

    def test_same_instructions_and_a_call_to_the_same_export_match(self):
        ins = {**caller(0x1100), 0x1100: ("ret", "")}
        exports = {"f": 0x1000, "g": 0x1100}
        r = self.pair(ins, ins, {0x1000: 0x1006, 0x1100: 0x1101}, {0x1000: 0x1006, 0x1100: 0x1101},
                      o_exports=exports, c_exports=exports)
        self.assertEqual((r.ok, r.unverified), (True, 0), r.reason)

    def test_a_register_difference_is_a_mismatch(self):
        o = {0x1000: ("mov", "r10,rcx"), 0x1003: ("ret", "")}
        c = {0x1000: ("mov", "r9,rcx"), 0x1003: ("ret", "")}
        r = self.pair(o, c, {0x1000: 0x1004}, {0x1000: 0x1004})
        self.assertFalse(r.ok)
        self.assertIn("r9,rcx != original mov r10,rcx", r.reason)

    def test_bytes_must_match_where_no_image_address_is_held(self):
        ins = {0x1000: ("ret", "")}
        memory = bytearray(0x3000)
        memory[0x1000] = 0xC3
        r = self.pair(ins, ins, {0x1000: 0x1001}, {0x1000: 0x1001}, o_memory=memory)
        self.assertIn("bytes differ", r.reason)

    def test_an_internal_callee_is_compared_recursively(self):
        # The candidate places the callee elsewhere (0x1200); its instructions decide.
        o = {**caller(0x1100), 0x1100: ("xor", "eax,eax"), 0x1102: ("ret", "")}
        same = {**caller(0x1200), 0x1200: ("xor", "eax,eax"), 0x1202: ("ret", "")}
        other = {**caller(0x1200), 0x1200: ("xor", "ecx,ecx"), 0x1202: ("ret", "")}
        fo, fc = {0x1000: 0x1006, 0x1100: 0x1103}, {0x1000: 0x1006, 0x1200: 0x1203}
        self.assertTrue(self.pair(o, same, fo, fc).ok)
        bad = self.pair(o, other, fo, fc)
        self.assertFalse(bad.ok)
        self.assertIn("ecx,ecx", bad.reason)

    def test_an_import_slot_or_thunk_must_name_the_same_import(self):
        o = {0x1000: ("call", f"qword ptr [{at(0x2000)}h]"), 0x1006: ("ret", "")}
        functions = {0x1000: 0x1007}
        ok = self.pair(o, o, functions, functions, o_imports={0x2000: "malloc"}, c_imports={0x2000: "malloc"})
        self.assertTrue(ok.ok, ok.reason)
        bad = self.pair(o, o, functions, functions, o_imports={0x2000: "malloc"}, c_imports={0x2000: "free"})
        self.assertIn("import free != original malloc", bad.reason)

    def test_a_manifest_entry_needs_the_candidate_symbol_of_its_name(self):
        o = {**caller(0x1100), 0x1100: ("ret", "")}
        c = {**caller(0x1200), 0x1200: ("int", "3")}  # bytes differ, but the entry is parked
        fo, fc = {0x1000: 0x1006, 0x1100: 0x1101}, {0x1000: 0x1006, 0x1200: 0x1201}
        entries = {"helper": Entry(0x1100, None, False, True)}
        ok = self.pair(o, c, fo, fc, c_names={0x1200: {"helper"}}, entries=entries)
        self.assertEqual((ok.ok, ok.unverified), (True, 0), ok.reason)
        bad = self.pair(o, c, fo, fc, c_names={0x1200: {"other"}}, entries=entries)
        self.assertIn("is not helper", bad.reason)

    def test_data_verifies_by_the_content_of_the_candidate_symbol(self):
        o = {0x1000: ("lea", f"rcx,[{at(0x2100)}h]"), 0x1007: ("ret", "")}
        c = {0x1000: ("lea", f"rcx,[{at(0x2200)}h]"), 0x1007: ("ret", "")}
        mo, mc = bytearray(0x3000), bytearray(0x3000)
        mo[0x2100:0x2105] = mc[0x2200:0x2205] = b"null\0"
        names = {0x2200: {"??_C@_05null@"}, 0x2205: {"next"}}
        functions = {0x1000: 0x1008}
        bound = {0x2105: {"next"}}  # the original's object ends where its next referenced address starts
        ok = self.pair(o, c, functions, functions, o_names=bound, c_names=names, o_memory=mo, c_memory=mc)
        self.assertTrue(ok.ok, ok.reason)
        mc[0x2201] = ord("x")
        self.assertIn("bytes differ", self.pair(o, c, functions, functions, o_names=bound, c_names=names,
                                                o_memory=mo, c_memory=mc).reason)

    def test_data_shorter_than_the_original_object_is_a_mismatch(self):
        # The original's table runs 16 bytes; the candidate's symbol holds the first 8, then another object.
        o = {0x1000: ("lea", f"rcx,[{at(0x2100)}h]"), 0x1007: ("ret", "")}
        c = {0x1000: ("lea", f"rcx,[{at(0x2200)}h]"), 0x1007: ("ret", "")}
        mo, mc = bytearray(0x3000), bytearray(0x3000)
        mo[0x2100:0x2110] = bytes(range(1, 17))
        mc[0x2200:0x2208] = bytes(range(1, 9))
        r = self.pair(o, c, {0x1000: 0x1008}, {0x1000: 0x1008}, o_names={0x2110: {"next"}},
                      c_names={0x2200: {"table"}, 0x2208: {"other"}}, o_memory=mo, c_memory=mc)
        self.assertIn("data +0x8: bytes differ", r.reason)

    def test_a_pointer_in_data_is_checked_as_a_target(self):
        o = {0x1000: ("lea", f"rcx,[{at(0x2100)}h]"), 0x1007: ("ret", ""), 0x1100: ("ret", "")}
        mo, mc = bytearray(0x3000), bytearray(0x3000)
        struct.pack_into("<Q", mo, 0x2100, BASE + 0x1100)
        struct.pack_into("<Q", mc, 0x2100, BASE + 0x1100)
        functions = {0x1000: 0x1008, 0x1100: 0x1101}
        names = {0x2100: {"vt"}, 0x2108: {"next"}}
        mo[0x1100] = mc[0x1100] = 0xC3
        ok = self.pair(o, o, functions, functions, c_names=names, o_memory=mo, c_memory=mc,
                       o_relocs={0x2100}, c_relocs={0x2100})
        self.assertTrue(ok.ok, ok.reason)
        bad = self.pair(o, o, functions, functions, c_names=names, o_memory=mo, c_memory=mc, o_relocs={0x2100})
        self.assertIn("a pointer on one side only", bad.reason)

    def test_a_target_no_rule_settles_is_unverified(self):
        # Data the candidate has no map symbol for.
        o = {0x1000: ("lea", f"rcx,[{at(0x2100)}h]"), 0x1007: ("ret", "")}
        functions = {0x1000: 0x1008}
        r = self.pair(o, o, functions, functions)
        self.assertEqual((r.ok, r.unverified), (True, 1))

    def test_local_branches_keep_their_offset(self):
        o = {0x1000: ("je", at(0x1004)), 0x1002: ("nop", ""), 0x1003: ("nop", ""), 0x1004: ("ret", "")}
        c = {0x1000: ("je", at(0x1003)), 0x1002: ("nop", ""), 0x1003: ("nop", ""), 0x1004: ("ret", "")}
        functions = {0x1000: 0x1005}
        self.assertTrue(self.pair(o, o, functions, functions).ok)
        self.assertIn("local target differs", self.pair(o, c, functions, functions).reason)


class ReviewRegressionTests(Pairing, unittest.TestCase):
    """Holes the D30 review found; each must stay closed."""

    def test_a_cycle_assumption_does_not_reach_another_export(self):
        # e1 calls F, e2 calls G; F and G call each other; F's last instruction differs. Both exports must fail,
        # whichever is compared first.
        def image(last):
            return {0x1000: ("call", at(0x1100)), 0x1005: ("ret", ""),
                    0x1010: ("call", at(0x1200)), 0x1015: ("ret", ""),
                    0x1100: ("call", at(0x1200)), 0x1105: ("ret", last),
                    0x1200: ("call", at(0x1100)), 0x1205: ("ret", "")}
        functions = {0x1000: 0x1006, 0x1010: 0x1016, 0x1100: 0x1106, 0x1200: 0x1206}
        exports = {"e1": 0x1000, "e2": 0x1010}
        o_ins, c_ins = image(""), image("8")
        mo, mc = encode(bytearray(0x3000), o_ins), encode(bytearray(0x3000), c_ins)
        for order in (["e1", "e2"], ["e2", "e1"]):
            comparison = Comparison(Linked(Image(functions, exports, None, mo), o_ins),
                                    Linked(Image(functions, exports, None, mc), c_ins), {})
            results = {n: comparison.export(n) for n in order}
            self.assertEqual([results[n].ok for n in order], [False, False], order)

    def test_a_start_the_decode_does_not_reach_fails_without_a_decoder(self):
        # The whole-image decode swallowed 0x1000 into a preceding instruction; it resumes at 0x1001.
        o = {0x1000: ("ret", "")}
        c = {0x0FFF: ("add", "byte ptr [rax],al"), 0x1001: ("ret", "")}
        functions = {0x1000: 0x1002}
        r = self.pair(o, c, functions, functions)
        self.assertIn("cannot decode from the function's start", r.reason)

    def test_a_start_the_decode_does_not_reach_is_decoded_again_on_its_own(self):
        o_ins = {0x1000: ("ret", "")}
        c_ins = {0x0FFF: ("add", "byte ptr [rax],al"), 0x1001: ("ret", "")}
        functions = {0x1000: 0x1001}
        mo, mc = bytearray(0x3000), bytearray(0x3000)
        mo[0x1000] = mc[0x1000] = 0xC3
        candidate = Linked(Image(functions, {"f": 0x1000}, None, mc), c_ins,
                           decoder=lambda begin, end: {0x1000: ("ret", "")})
        r = Comparison(Linked(Image(functions, {"f": 0x1000}, None, mo), o_ins), candidate, {}).export("f")
        self.assertTrue(r.ok, r.reason)

    def test_an_encoding_difference_beside_an_address_field_is_a_mismatch(self):
        ins = {**caller(0x1100), 0x1100: ("ret", "")}
        exports = {"f": 0x1000, "g": 0x1100}
        functions = {0x1000: 0x1006, 0x1100: 0x1101}
        mo, mc = bytearray(0x3000), bytearray(0x3000)
        mo[0x1000], mc[0x1000] = 0xE8, 0x2E  # same text, a different opcode byte before the rel32
        r = self.pair(ins, ins, functions, functions, o_exports=exports, c_exports=exports, o_memory=mo, c_memory=mc)
        self.assertIn("bytes differ", r.reason)

    def test_fingerprints_name_unnamed_addresses_by_symbol_and_offset(self):
        from native_linked import fingerprint_text
        def fingerprint(offset):
            ins = {0x1000: ("lea", f"rcx,[{at(0x2100 + offset)}h]"), 0x1007: ("ret", "")}
            linked = Linked(Image({0x1000: 0x1008}, {"f": 0x1000}), ins, {0x2100: {"arr"}})
            return fingerprint_text(linked, "f")
        self.assertIn("arr+0x4", fingerprint(4))
        self.assertNotEqual(fingerprint(4), fingerprint(8))


class ParseTests(unittest.TestCase):
    def test_dumpbin_lines_become_instructions_by_rva(self):
        text = ("Dump of file x.dll\n\n  0000000180001000: mov         qword ptr [rsp+8],rbx\n"
                "  0000000180001005: ret\n")
        self.assertEqual(parse_disassembly(text, BASE),
                         {0x1000: ("mov", "qword ptr [rsp+8],rbx"), 0x1005: ("ret", "")})

    def test_map_symbols_give_names_by_rva(self):
        text = (" Preferred load address is 0000000180000000\n\n"
                "  Address         Publics by Value              Rva+Base               Lib:Object\n\n"
                " 0000:00000000       ___safe_se_handler_count   0000000000000000     <absolute>\n"
                " 0001:00000000       ?f@@YAXXZ                  0000000180001000 f   a.obj\n"
                " Static symbols\n\n"
                " 0001:00000100       ?g@?A0x1234@@YAXXZ         0000000180001100 f   a.obj\n")
        names, by_rva = parse_map(text)
        self.assertEqual(names["?f@@YAXXZ"], 0x1000)
        self.assertEqual(by_rva[0x1100], {"?g@?A0x@@YAXXZ"})
        self.assertNotIn(0, by_rva)


@unittest.skipUnless(Path(os.environ.get("MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0"),
                          "efistatsci_.dll").is_file(), "Mathcad Prime install not present")
class InstalledTests(unittest.TestCase):
    def test_base_relocations_of_the_installed_original_are_read(self):
        from native_bytes import PeImage
        from native_linked import base_relocations
        image = PeImage(Path(os.environ.get("MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0"),
                             "efistatsci_.dll"))
        relocs = base_relocations(image)
        self.assertTrue(relocs)
        self.assertTrue(all(image.contains(r, 8) for r in relocs))


if __name__ == "__main__":
    unittest.main()
