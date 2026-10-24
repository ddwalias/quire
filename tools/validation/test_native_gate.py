"""Acceptance checks for the native gate's comparison and project mapping, on fabricated results (no MSVC needed)."""
import json
from pathlib import Path
import struct
import tempfile
import unittest
from unittest import mock

from native_bytes import Function, Reloc
from native_gate import anonymous, compare, fingerprint, native_projects, original_dll, scan
from test_native_bytes import ManifestImage, coff


def row(ok=True, sha='a', reason=None):
    return {'ok': ok, 'reason': reason if reason or ok else 'length 3 != original 4', 'sha256': sha}


class CompareTests(unittest.TestCase):
    def test_a_function_that_passed_must_still_pass(self):
        result = compare({'f': row()}, {'f': row(ok=False)})
        self.assertFalse(result['passed'])
        self.assertIn('passed in base', result['failures'][0]['reason'])

    def test_new_or_changed_functions_must_pass(self):
        self.assertFalse(compare({}, {'f': row(ok=False)})['passed'])
        self.assertFalse(compare({'f': row(ok=False)}, {'f': row(ok=False, sha='b')})['passed'])
        self.assertTrue(compare({'f': row(ok=False)}, {'f': row(sha='b')})['passed'])

    def test_unchanged_unfinished_functions_do_not_block(self):
        result = compare({'f': row(ok=False), 'g': row()}, {'f': row(ok=False), 'g': row()})
        self.assertEqual((result['passed'], result['checked'], result['unfinished']), (True, ['g'], 1))

    def test_removing_a_passing_function_fails_but_an_unfinished_one_may_go(self):
        self.assertEqual(compare({'f': row()}, {})['failures'], [{'method': 'f', 'reason': 'function removed'}])
        self.assertTrue(compare({'f': row(ok=False)}, {})['passed'])


class ProjectTests(unittest.TestCase):
    def setUp(self):
        tmp = tempfile.TemporaryDirectory()
        self.addCleanup(tmp.cleanup)
        self.root = Path(tmp.name)
        for name, files in (('Quire.Native.A', ['Quire.Native.A.vcxproj']), ('Quire.Mixed', ['Quire.Mixed.vcxproj',
                                                                                              'Quire.Mixed.csproj']),
                            ('Quire.Managed', ['Quire.Managed.csproj'])):
            (self.root / 'src' / name).mkdir(parents=True)
            for f in files:
                (self.root / 'src' / name / f).write_text('')
        (self.root / 'docs' / 'mirror').mkdir(parents=True)
        (self.root / 'docs' / 'mirror' / 'assembly-map.json').write_text(json.dumps({'units': [
            {'name': 'Quire.Native.A', 'original': {'name': 'a', 'file': 'a.dll'}},
            {'name': 'Quire.Native.B', 'original': None}]}))

    def test_native_projects_have_a_vcxproj_and_no_csproj(self):
        self.assertEqual(native_projects(self.root), {'Quire.Native.A'})

    def test_original_comes_from_the_assembly_map_and_fails_closed(self):
        self.assertEqual(original_dll(self.root, 'Quire.Native.A'), 'a.dll')
        for unknown in ('Quire.Native.B', 'Quire.Native.C'):
            with self.assertRaisesRegex(RuntimeError, 'no single original DLL'):
                original_dll(self.root, unknown)


class ScanTests(unittest.TestCase):
    """A strict scan: export `caller` (RVA 0x1000) calls RVA 0x1100, which only the manifest can verify."""

    def test_unverified_targets_do_not_pass_and_rows_carry_a_fingerprint(self):
        memory = bytearray(0x2000)
        memory[0x1000:0x1006] = b'\xe8\0\0\0\0\xc3'
        struct.pack_into('<i', memory, 0x1001, 0x1100 - (0x1001 + 4))
        image = ManifestImage(memory, {0x1000: 0x1006, 0x1100: 0x1103}, {'caller': 0x1000})
        with tempfile.TemporaryDirectory() as tmp, mock.patch('native_bytes.PeImage', lambda path: image):
            obj = Path(tmp) / 'caller.obj'
            obj.write_bytes(coff([('caller', b'\xe8\0\0\0\0\xc3', [(1, 0x0004, 1)])], externals=['callee']))
            rows = scan('fake.dll', [obj], None)
            obj.write_bytes(coff([('caller', b'\xe8\0\0\0\0\xc3', [(1, 0x0004, 1)])], externals=['other']))
            retargeted = scan('fake.dll', [obj], None)
        self.assertEqual((rows['caller']['ok'], rows['caller']['reason']), (False, '1 unverified call/address targets'))
        self.assertNotEqual(rows['caller']['sha256'], retargeted['caller']['sha256'])

    def test_anonymous_namespace_tags_do_not_count_as_a_change(self):
        # MSVC tags an anonymous namespace with a hash of the source path, which differs between base and candidate.
        call = lambda tag: Function('f', b'\xe8\0\0\0\0\xc3', [Reloc(1, 4, 0)], {1: None}, {1: f'?g@?A0x{tag}@u@@YAXXZ'})
        self.assertEqual(fingerprint(call('377d720e')), fingerprint(call('4b90f393')))
        self.assertEqual(anonymous('?g@?A0x377d720e@u@@YAXXZ'), '?g@?A0x@u@@YAXXZ')


if __name__ == '__main__':
    unittest.main()
