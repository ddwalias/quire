"""Acceptance checks for the native gate's comparison and project mapping, on fabricated results (no MSVC needed)."""
import contextlib
import io
import json
from pathlib import Path
import struct
import tempfile
import unittest
from unittest import mock

from native_bytes import Function, Reloc
from native_gate import (MANIFEST, anonymous, check, compare, fingerprint, linked, native_projects,
                         object_fingerprints, original_dll, scan)
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

    def test_linked_mode_regression_of_an_unchanged_export_fails_and_is_listed(self):
        result = compare({'f': row(), 'g': row()}, {'f': row(ok=False), 'g': row()}, linked_mode=True)
        self.assertFalse(result['passed'])
        self.assertEqual([r['method'] for r in result['regressions']], ['f'])
        self.assertEqual([f['method'] for f in result['failures']], ['f'])
        # A changed export must still pass, and removing a passing one still fails.
        self.assertFalse(compare({'f': row()}, {'f': row(ok=False, sha='b')}, linked_mode=True)['passed'])
        self.assertFalse(compare({'f': row()}, {}, linked_mode=True)['passed'])

    def test_per_object_regression_of_an_unchanged_export_fails_without_a_regressions_list(self):
        result = compare({'f': row()}, {'f': row(ok=False)})
        self.assertFalse(result['passed'])
        self.assertEqual(result['regressions'], [])

    def test_a_first_switch_to_gl_fails_when_a_passing_export_stops_passing(self):
        # The base is per object, so both sides carry per-object fingerprints (native_gate.check): same bytes, unchanged.
        result = compare({'f': row(sha='p')}, {'f': row(ok=False, sha='p')}, linked_mode=True)
        self.assertFalse(result['passed'])
        self.assertEqual(result['regressions'], [{'method': 'f', 'reason': 'passed in base: length 3 != original 4'}])
        self.assertFalse(compare({'f': row(sha='p')}, {'f': row(ok=False, sha=None)}, linked_mode=True)['passed'])

    def test_object_fingerprints_name_each_function_by_its_anonymous_name(self):
        with tempfile.TemporaryDirectory() as tmp:
            obj = Path(tmp) / 'f.obj'
            obj.write_bytes(coff([('?f@@YAXXZ', b'\xc3', [])]))
            fingerprints = object_fingerprints([obj])
        self.assertEqual(set(fingerprints), {'?f@@YAXXZ'})
        self.assertEqual(len(fingerprints['?f@@YAXXZ']), 64)


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

    def test_a_project_building_with_whole_program_optimization_is_linked(self):
        for flag, expected in (('true', True), ('false', False)):
            project = self.root / 'src' / 'Quire.Native.Linked'
            project.mkdir(parents=True, exist_ok=True)
            (project / 'Quire.Native.Linked.vcxproj').write_text(
                f'<Project><PropertyGroup><WholeProgramOptimization>{flag}</WholeProgramOptimization></PropertyGroup>'
                '</Project>', encoding='utf-8')
            self.assertEqual(linked(self.root, 'Quire.Native.Linked'), expected)

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

    def test_export_targets_still_require_the_original_address(self):
        memory = bytearray(0x2000)
        memory[0x1000:0x1006] = b'\xe8\0\0\0\0\xc3'
        struct.pack_into('<i', memory, 0x1001, 0x1200 - (0x1001 + 4))
        image = ManifestImage(memory, {0x1000: 0x1006, 0x1100: 0x1103, 0x1200: 0x1203},
                              {'caller': 0x1000, 'exported': 0x1100})
        with tempfile.TemporaryDirectory() as tmp, mock.patch('native_bytes.PeImage', lambda path: image):
            obj = Path(tmp) / 'caller.obj'
            obj.write_bytes(coff([('caller', b'\xe8\0\0\0\0\xc3', [(1, 0x0004, 1)])],
                                 externals=['exported']))
            rows = scan('fake.dll', [obj], None)
        self.assertFalse(rows['caller']['ok'])
        self.assertIn('call target differs', rows['caller']['reason'])

    def parked_data_rows(self, target, data=b'abc'):
        """scan() of `caller`, whose original call reaches `target`, under a parked data entry of 3 bytes at 0x1100 that
        the candidate defines as `data`."""
        memory = bytearray(0x2000)
        memory[0x1000:0x1006] = b'\xe8\0\0\0\0\xc3'
        struct.pack_into('<i', memory, 0x1001, target - (0x1001 + 4))
        image = ManifestImage(memory, {0x1000: 0x1006, 0x1100: 0x1103, 0x1200: 0x1203}, {'caller': 0x1000})
        manifest = {'fake.dll': {'sha256': image.sha256, 'targets': {
            'parked_data': {'rva': '0x1100', 'size': 3, 'parked': True}}}}
        with tempfile.TemporaryDirectory() as tmp, mock.patch('native_bytes.PeImage', lambda path: image):
            obj = Path(tmp) / 'caller.obj'
            obj.write_bytes(coff([('caller', b'\xe8\0\0\0\0\xc3', [(1, 0x0004, 1)]),
                                  ('parked_data', data, [])], externals=['parked_data'], data=['parked_data']))
            manifest_path = Path(tmp) / 'targets.json'
            manifest_path.write_text(json.dumps(manifest), encoding='utf-8')
            return scan('fake.dll', [obj], manifest_path)

    def test_parked_data_target_verifies_by_its_rva_name_and_size(self):
        rows = self.parked_data_rows(0x1100)
        self.assertTrue(rows['caller']['ok'], rows['caller']['reason'])
        # D29: every site must resolve to the entry's RVA, parked data included.
        elsewhere = self.parked_data_rows(0x1200)
        self.assertFalse(elsewhere['caller']['ok'])
        self.assertIn('manifest entry gives 0x1100', elsewhere['caller']['reason'])

    def test_a_failing_row_is_not_replaced_by_a_later_passing_row_of_its_name(self):
        # compare_object lists "parked_data (manifest entry)", failing on its size, then "parked_data (parked entry)", OK.
        rows = self.parked_data_rows(0x1100, data=b'abcd')
        self.assertFalse(rows['parked_data']['ok'])
        self.assertIn('size 4 != manifest 3', rows['parked_data']['reason'])

    def test_anonymous_namespace_tags_do_not_count_as_a_change(self):
        # MSVC tags an anonymous namespace with a hash of the source path, which differs between base and candidate.
        call = lambda tag: Function('f', b'\xe8\0\0\0\0\xc3', [Reloc(1, 4, 0)], {1: None}, {1: f'?g@?A0x{tag}@u@@YAXXZ'})
        self.assertEqual(fingerprint(call('377d720e')), fingerprint(call('4b90f393')))
        self.assertEqual(anonymous('?g@?A0x377d720e@u@@YAXXZ'), '?g@?A0x@u@@YAXXZ')


class CheckTests(unittest.TestCase):
    """check() with git and the builds replaced: `caller` passes only under a manifest that lists `callee`."""

    def run_check(self, base_targets, candidate_targets):
        def tree(root, targets):
            (root / 'src' / 'Quire.Native.A').mkdir(parents=True, exist_ok=True)
            (root / 'src' / 'Quire.Native.A' / 'Quire.Native.A.vcxproj').write_text('<Project/>', encoding='utf-8')
            (root / MANIFEST).parent.mkdir(parents=True, exist_ok=True)
            (root / MANIFEST).write_text(json.dumps(targets), encoding='utf-8')

        def run(command, cwd, *args):  # the base checkout writes the base tree
            if 'checkout' in command:
                tree(Path(cwd), base_targets)

        def scan_project(original, root, project, work, log, timeout, manifest, *_):
            ok = manifest is not None and 'callee' in json.loads(Path(manifest).read_text(encoding='utf-8'))
            return {'caller': {'ok': ok, 'reason': None if ok else '1 unverified call/address targets', 'sha256': 's'}}

        with tempfile.TemporaryDirectory() as tmp:
            root, install = Path(tmp) / 'root', Path(tmp) / 'install'
            tree(root, candidate_targets)
            install.mkdir()
            (install / 'a.dll').write_bytes(b'')
            with mock.patch('native_gate.git', return_value='0' * 40), mock.patch('native_gate.run', run), \
                    mock.patch('native_gate.original_dll', return_value='a.dll'), \
                    mock.patch('native_gate.linked', return_value=False), \
                    mock.patch('native_gate.scan_project', scan_project), \
                    mock.patch('native_bytes.PeImage', lambda path: mock.Mock(sha256='ab' * 32, exports={})), \
                    contextlib.redirect_stdout(io.StringIO()):
                return check('main', ['Quire.Native.A'], Path(tmp) / 'results', root=root, install=install)

    def test_a_candidate_that_only_removes_a_manifest_entry_does_not_pass(self):
        listed = {'callee': {'rva': '0x1100'}}
        self.assertTrue(self.run_check(listed, listed)['passed'])
        report = self.run_check(listed, {})
        self.assertFalse(report['passed'], report)
        self.assertEqual(report['failures'], [{'method': 'Quire.Native.A::caller',
                                               'reason': 'passed in base: 1 unverified call/address targets'}])


if __name__ == '__main__':
    unittest.main()
