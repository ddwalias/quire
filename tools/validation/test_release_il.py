"""Acceptance checks for the Release IL gate, without executing production code."""
from pathlib import Path
import sys
import tempfile
import unittest
from release_il import (LINKER_PRODUCT, MSVC_BUILD, builds_cppcli, compare, msvc_mismatch, ordinalless, reads_install,
                        section_names, without_install)


def method(name='M', body='same', exact=True, **extra):
    return dict(dict(id=f'P::T::{name}', type='T', signature=name, body_sha256=body,
                     exact_match=exact, shape_matches=True, declaration_only=False,
                     original={'assembly': 'Mathcad'}), **extra)


def inventory(*rows):
    return {'methods': list(rows)}


class ReleaseIlAcceptanceTests(unittest.TestCase):
    def test_changed_body_must_match_even_when_it_was_unfinished(self):
        result = compare(inventory(method(body='old', exact=False)), inventory(method(body='new', exact=False)))
        self.assertFalse(result['passed'])
        self.assertEqual(result['checked'], ['P::T::M'])

    def test_changed_body_passes_on_exact_match_without_test_or_status_records(self):
        self.assertTrue(compare(inventory(method(body='old', exact=False)), inventory(method(body='new')))['passed'])

    def test_moved_stub_is_unchanged_but_an_implemented_stub_is_checked(self):
        moved = compare(inventory(method(body='a.g.cs', exact=False, stub_body=True)),
                        inventory(method(body='b.cs', exact=False, stub_body=True)))
        self.assertTrue(moved['passed'])
        self.assertEqual(moved['checked'], [])
        implemented = compare(inventory(method(body='a.g.cs', exact=False, stub_body=True)),
                              inventory(method(body='real', exact=False, stub_body=False)))
        self.assertFalse(implemented['passed'])
        self.assertFalse(compare(inventory(method(body='a.g.cs', stub_body=True)),
                                 inventory(method(body='b.cs', exact=False, stub_body=True)))['passed'])

    def test_regenerated_attribute_stub_is_unchanged(self):
        # A constructor stub whose chaining MirrorGen changed is still a stub.
        result = compare(inventory(method(body='base()', exact=False, mirror_stub=True)),
                         inventory(method(body='this()', exact=False, mirror_stub=True)))
        self.assertTrue(result['passed'])

    def test_removing_a_never_exact_generated_stub_passes(self):
        self.assertTrue(compare(inventory(method(exact=False, mirror_stub=True)), inventory())['passed'])
        self.assertTrue(compare(inventory(method(exact=False, stub_body=True)), inventory())['passed'])
        self.assertFalse(compare(inventory(method(exact=True, mirror_stub=True)), inventory())['passed'])
        self.assertFalse(compare(inventory(method(exact=False)), inventory())['passed'])

    def test_renumbered_generated_member_is_the_same_member(self):
        def lambda_row(n, body='same'):
            row = method(f'<AddLine>b__{n}_5', body=body, exact=False)
            row.update(id=f'P::T+<>c::<AddLine>b__{n}_5', type='T+<>c', original=None)
            return row
        self.assertTrue(compare(inventory(lambda_row(249)), inventory(lambda_row(240)))['passed'])
        self.assertFalse(compare(inventory(lambda_row(249)), inventory(lambda_row(240, body='other')))['passed'])
        self.assertEqual(ordinalless('T+<>c__DisplayClass249_0::<M>b__249_5|<M>g__F|251_0|<M>d__205|<>9__12_3'),
                         'T+<>c__DisplayClass#_0::<M>b__#_5|<M>g__F|#_0|<M>d__#|<>9__#_3')

    def test_unchanged_unfinished_methods_do_not_block(self):
        result = compare(inventory(method(exact=False)), inventory(method(exact=False)))
        self.assertTrue(result['passed'])
        self.assertEqual(result['checked'], [])

    def test_previously_exact_methods_cannot_regress_even_with_unchanged_body_hash(self):
        self.assertFalse(compare(inventory(method()), inventory(method(exact=False)))['passed'])

    def test_removed_method_blocks(self):
        result = compare(inventory(method(exact=False)), inventory())
        self.assertEqual(result['failures'][0]['reason'], 'method removed')

    def test_renumbered_generated_members_without_counterpart_may_be_removed(self):
        stale = method('MoveNext', exact=False)
        stale.update(id='P::T+<Get>d__13::MoveNext', type='T+<Get>d__13', original=None)
        renumbered = method('MoveNext')
        renumbered.update(id='P::T+<Get>d__10::MoveNext', type='T+<Get>d__10')
        self.assertTrue(compare(inventory(stale), inventory(renumbered))['passed'])
        closure = method('<Recalculate>b__7_0', exact=False)
        closure['original'] = None
        self.assertTrue(compare(inventory(closure), inventory())['passed'])
        mapped = method('<Recalculate>b__7_0', exact=False)
        self.assertFalse(compare(inventory(mapped), inventory())['passed'])

    def test_generated_helpers_and_missing_counterparts_are_not_exempt(self):
        generated = method('<M>b__0', exact=False)
        generated['original'] = None
        result = compare(inventory(), inventory(generated))
        self.assertEqual(result['failures'][0]['reason'], 'no Mathcad counterpart')

    def test_explicit_selection_checks_an_unchanged_unfinished_member(self):
        row = method(exact=False)
        self.assertFalse(compare(inventory(row), inventory(row), types=['T'])['passed'])
        self.assertFalse(compare(inventory(row), inventory(row), methods=['T::M'])['passed'])

    def test_missing_selections_fail(self):
        self.assertFalse(compare(inventory(), inventory(), types=['Missing'])['passed'])
        self.assertFalse(compare(inventory(), inventory(), methods=['Missing::M'])['passed'])

    def test_unsupported_mixed_mode_rows_always_fail_even_unchanged_or_engine_native(self):
        row = method('<Module>::f', exact=None, unsupported='global <Module> method has no reviewed Mathcad binding',
                     engine_native=True)
        result = compare(inventory(row), inventory(row))
        self.assertEqual(result['failures'], [{'method': 'P::T::<Module>::f', 'reason': row['unsupported']}])

    def test_mixed_mode_toolset_support_without_counterpart_is_reported_not_failed(self):
        def global_row(signature, mixed=True, taken=False):
            row = method(signature, exact=None, original=None, mixed_mode=mixed, original_name_taken=taken,
                         unsupported='global <Module> method has no reviewed Mathcad binding')
            row.update(id=f'P::<Module>::{signature}', type='<Module>')
            return row
        support = global_row('std.basic_string<char>.{ctor}`0()->Void|True')
        result = compare(inventory(), inventory(support, global_row('delete[]`0(Void*)->Void|True')))
        self.assertTrue(result['passed'])
        self.assertEqual(result['toolset_support'], 2)
        self.assertEqual(result['unfinished'], 0)
        crt_type = method('Throw', exact=False, original=None, mixed_mode=True, original_name_taken=False)
        crt_type.update(type='<CrtImplementationDetails>.Exception')
        self.assertTrue(compare(inventory(), inventory(crt_type))['passed'])
        # Our own templates, IL-only assemblies, pairing misses and old inventories get no allowance.
        self.assertFalse(compare(inventory(), inventory(global_row('Substring<char const >.{ctor}`0()->Void|True')))['passed'])
        self.assertFalse(compare(inventory(), inventory(global_row('std.endl`0()->Void|True', mixed=False)))['passed'])
        self.assertFalse(compare(inventory(), inventory(global_row('std.endl`0()->Void|True', taken=True)))['passed'])
        untagged = global_row('std.endl`0()->Void|True')
        del untagged['original_name_taken']
        self.assertFalse(compare(inventory(), inventory(untagged))['passed'])
        # Exact names are whole names, not prefixes.
        self.assertFalse(compare(inventory(), inventory(global_row('PtrToStringCharsX`0()->Void|True')))['passed'])
        self.assertFalse(compare(inventory(), inventory(global_row('newFoo`0()->Void|True')))['passed'])

    def test_toolset_allowance_never_covers_a_member_that_was_exact_or_paired(self):
        before = method('std.vector<Foo>.Get`0()->Void|True', mixed_mode=True)
        after = method('std.vector<Foo>.Get`0()->Void|True', body='other', exact=None, original=None,
                       mixed_mode=True, original_name_taken=False)
        after['type'] = before['type'] = 'std.vector<Foo>'
        self.assertFalse(compare(inventory(before), inventory(after))['passed'])
        before['exact_match'] = False
        self.assertFalse(compare(inventory(before), inventory(after))['passed'])

    def test_mixed_mode_members_with_a_counterpart_must_still_match(self):
        row = method('std.endl`0()->Void|True', exact=False, mixed_mode=True)
        self.assertFalse(compare(inventory(), inventory(row))['passed'])

    def test_native_body_passes_only_against_a_native_counterpart(self):
        def native(original_native, mixed=True):
            return method('N', exact=None, mixed_mode=mixed, unsupported='native code body has no IL to compare',
                          original={'assembly': 'utl-mgd', 'native_code': original_native})
        result = compare(inventory(), inventory(native(True)))
        self.assertTrue(result['passed'])
        self.assertEqual(result['native_pairs'], 1)
        # The declaration still has to match.
        self.assertFalse(compare(inventory(), inventory(dict(native(True), shape_matches=False)))['passed'])
        # A C# member exempted as engine-native may become a native body against a native counterpart.
        csharp = method('N', exact=None, engine_native=True, original={'assembly': 'utl-mgd'})
        self.assertTrue(compare(inventory(csharp), inventory(native(True)))['passed'])
        self.assertFalse(compare(inventory(), inventory(native(False)))['passed'])
        self.assertFalse(compare(inventory(), inventory(native(True, mixed=False)))['passed'])

    def test_counterpartless_csharp_stand_ins_may_be_replaced_by_a_mixed_mode_build(self):
        stand_in = method('_tildeSafeMutex', exact=False, original=None, project='P')
        mixed = method('Other', mixed_mode=True, project='P')
        self.assertTrue(compare(inventory(stand_in), inventory(mixed))['passed'])
        mapped = method('Enter', exact=False, project='P')
        self.assertFalse(compare(inventory(mapped), inventory(mixed))['passed'])

    def test_existing_product_only_members_may_change_or_be_removed(self):
        product = method('get_DisplayValue', exact=False, original=None)
        self.assertTrue(compare(inventory(product), inventory())['passed'])
        changed = compare(inventory(product), inventory(dict(product, body_sha256='rewired')))
        self.assertTrue(changed['passed'])
        self.assertEqual(changed['checked'], ['P::T::get_DisplayValue'])
        self.assertFalse(compare(inventory(), inventory(product))['passed'])
        self.assertFalse(compare(inventory(method('get_Value', exact=False)), inventory())['passed'])
        lost = compare(inventory(method(exact=False)), inventory(method(body='other', exact=False, original=None)))
        self.assertEqual(lost['failures'][0]['reason'], 'no Mathcad counterpart')
        # Any project, not only mixed-mode ones (D19 allowed it there first).
        self.assertTrue(compare(inventory(dict(product, project='P')), inventory(method('Other', project='P')))['passed'])
        # Explicitly selected members are still judged.
        self.assertFalse(compare(inventory(product), inventory(dict(product, body_sha256='rewired')),
                                 methods=['T::get_DisplayValue'])['passed'])

    def test_generated_helpers_of_mirrored_types_are_not_product_only(self):
        def closure(body, type_='T+<>c'):
            row = method('<Get>b__5_0', body=body, exact=False, original=None)
            row.update(id=f'P::{type_}::<Get>b__5_0', type=type_)
            return row
        mirrored = method('Get', exact=True)
        self.assertFalse(compare(inventory(mirrored, closure('a')), inventory(mirrored, closure('b')))['passed'])
        record = dict(closure('a', 'R'), id='P::R::<Clone>$', signature='<Clone>$')
        self.assertTrue(compare(inventory(mirrored, record), inventory(mirrored, dict(record, body_sha256='b')))['passed'])

    def test_builds_never_see_the_mathcad_install(self):
        env, hidden = without_install(Path('work'), {'MATHCAD_DIR': r'C:\PTC', 'PATH': 'p'})
        self.assertEqual(env, {'PATH': 'p'})
        self.assertTrue(hidden.startswith('-p:MathcadInstallDir=') and 'no-mathcad-install' in hidden)
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            (root / 'src' / 'P' / 'obj').mkdir(parents=True)
            (root / 'src' / 'P' / 'P.csproj').write_text('<Project />')
            (root / 'src' / 'P' / 'obj' / 'x.props').write_text('MATHCAD_DIR')
            self.assertEqual(reads_install(root, ['P']), [])
            (root / 'src' / 'P' / 'P.vcxproj').write_text(r'<D>C:\Program Files\PTC\Mathcad Prime 12</D>')
            self.assertEqual(reads_install(root, ['P']), [str(Path('src/P/P.vcxproj'))])
            (root / 'src' / 'P' / 'P.vcxproj').write_text('<Project />')
            (root / 'src' / 'P' / 'X.Targets').write_text('$(MATHCAD_DIR)')
            (root / 'Directory.Build.props').write_text('$(MathcadInstallDir)')
            self.assertEqual(reads_install(root, ['P']), [str(Path('Directory.Build.props')), str(Path('src/P/X.Targets'))])

    def test_dependents_of_a_cppcli_project_build_with_the_pinned_msvc(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            for name in ('Utl', 'Engine', 'Mcd', 'Plain', 'A', 'B'):
                (root / 'src' / name / 'Generated').mkdir(parents=True)
                (root / 'src' / name / f'{name}.csproj').write_text('<Project />')
            (root / 'src' / 'Utl' / 'MixedMode').mkdir()
            (root / 'src' / 'Utl' / 'MixedMode' / 'Utl.vcxproj').write_text('<Project />')
            (root / 'src' / 'Engine' / 'Generated' / 'Mirror.props').write_text(
                r'<ProjectReference Include="..\Utl\Utl.csproj" />')
            self.assertTrue(builds_cppcli(root, ['Utl']))
            self.assertTrue(builds_cppcli(root, ['Engine']))
            (root / 'src' / 'Mcd' / 'Mcd.csproj').write_text(r"<ProjectReference Condition='x' Include='..\Engine\Engine.csproj' />")
            self.assertTrue(builds_cppcli(root, ['Mcd']))
            (root / 'src' / 'A' / 'A.csproj').write_text(r'<ProjectReference Include="..\B\B.csproj" />')
            (root / 'src' / 'B' / 'B.csproj').write_text(r'<ProjectReference Include="..\A\A.csproj" />')
            self.assertFalse(builds_cppcli(root, ['A']))
            self.assertFalse(builds_cppcli(root, ['Plain']))

    def test_mixed_mode_dll_must_come_from_the_pinned_msvc(self):
        def image(entries, key=0x1234ABCD):
            words = [0x536E6144 ^ key, key, key, key]
            for product, build in entries:
                words += [(product << 16 | build) ^ key, 1 ^ key]
            rich = b''.join(w.to_bytes(4, 'little') for w in words) + b'Rich' + key.to_bytes(4, 'little')
            data = bytearray(0x80) + rich + bytearray(8)
            data[0x3c:0x40] = len(data).to_bytes(4, 'little')
            return bytes(data) + b'PE\0\0'
        with tempfile.TemporaryDirectory() as temp:
            dll = Path(temp) / 'x.dll'
            dll.write_bytes(image([(261, 35721), (261, MSVC_BUILD), (LINKER_PRODUCT, MSVC_BUILD)]))
            self.assertIsNone(msvc_mismatch(dll))
            dll.write_bytes(image([(261, MSVC_BUILD + 14), (LINKER_PRODUCT, MSVC_BUILD + 14)]))
            self.assertIn('required', msvc_mismatch(dll))
            dll.write_bytes(image([(261, MSVC_BUILD)]))  # no linker record
            self.assertIn('required', msvc_mismatch(dll))
            no_rich = bytearray(0x80)
            no_rich[0x3c:0x40] = (0x80).to_bytes(4, 'little')
            dll.write_bytes(bytes(no_rich) + b'PE\0\0')
            self.assertIsNone(msvc_mismatch(dll))

    def test_section_names_reads_the_pe_section_table(self):
        self.assertIn('.text', section_names(sys.executable))

    def test_member_moved_to_another_project_keeps_its_obligations(self):
        def row(project, exact=True, **extra):
            return dict(method(exact=exact, project=project, **extra), id=f'{project}::T::M')
        self.assertTrue(compare(inventory(row('Utl')), inventory(row('Misc')))['passed'])
        self.assertFalse(compare(inventory(row('Utl')), inventory(row('Misc', exact=False)))['passed'])
        self.assertTrue(compare(inventory(row('Utl', exact=False, stub_body=True)),
                                inventory(row('Misc', exact=False, stub_body=True)))['passed'])
        self.assertFalse(compare(inventory(row('Utl')), inventory())['passed'])
        # Signatures name the declaring Quire assembly, which the move changes.
        utl = dict(row('Utl'), signature='M`0()->[Quire.Engine.Utl]T|True')
        misc = dict(row('Misc'), signature='M`0()->[Quire.Engine.Misc]T|True')
        self.assertTrue(compare(inventory(utl), inventory(misc))['passed'])

    def test_matching_declaration_needs_no_body(self):
        row = method(exact=None)
        row.update(body_sha256=None, declaration_only=True)
        self.assertTrue(compare(inventory(), inventory(row))['passed'])


if __name__ == '__main__':
    unittest.main()
