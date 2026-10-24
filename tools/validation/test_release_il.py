"""Acceptance checks for the Release IL gate, without executing production code."""
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest
from unittest import mock
import release_il
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

    def test_engine_native_keeps_exact_obligations_and_new_unpaired_rows_fail(self):
        native = method(exact=False, engine_native=True)
        self.assertTrue(compare(inventory(dict(native, body_sha256='a')), inventory(dict(native, body_sha256='b')))['passed'])
        self.assertTrue(compare(inventory(), inventory(native))['passed'])  # new, paired: D10 exemption
        self.assertFalse(compare(inventory(method()), inventory(dict(native, body_sha256='b')))['passed'])
        new_unpaired = compare(inventory(), inventory(dict(native, original=None)))
        self.assertEqual(new_unpaired['failures'][0]['reason'], 'no Mathcad counterpart')

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

    def test_native_pair_exact_in_a_mixed_mode_base_still_pairs(self):
        native = dict(method('N', exact=True, mixed_mode=True, unsupported='native code body has no IL to compare',
                             original={'assembly': 'utl-mgd', 'native_code': True}), shape_matches=True)
        self.assertTrue(compare(inventory(native), inventory(dict(native)))['passed'])
        # Losing the native counterpart is still a failure, even when the member was native before.
        lost = dict(native, original=None)
        self.assertFalse(compare(inventory(native), inventory(lost))['passed'])
        # Exact before, not exact now (an import attribute changed): the exemption no longer applies.
        changed = dict(native, exact_match=False, structure_differences=['import attributes differ'])
        result = compare(inventory(native), inventory(changed))
        self.assertFalse(result['passed'])
        self.assertEqual(result['failures'][0]['reason'], 'import attributes differ')

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

    def test_reads_install_scans_every_project_the_build_references(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            (root / 'src' / 'Utl' / 'MixedMode').mkdir(parents=True)
            (root / 'src' / 'Utl' / 'Utl.csproj').write_text('<Project />')
            (root / 'src' / 'Utl' / 'MixedMode' / 'Utl.vcxproj').write_text('<Project />')
            (root / 'src' / 'Engine' / 'Generated').mkdir(parents=True)
            (root / 'src' / 'Engine' / 'Engine.csproj').write_text('<Project />')
            (root / 'src' / 'Engine' / 'Generated' / 'Mirror.props').write_text(
                r'<ProjectReference Include="..\Mirror\Mirror.csproj" />')
            (root / 'src' / 'Mirror').mkdir()
            (root / 'src' / 'Mirror' / 'Mirror.csproj').write_text(r'<Project>C:\Program Files\PTC\Mathcad Prime 12</Project>')
            self.assertEqual(reads_install(root, ['Engine']), [str(Path('src/Mirror/Mirror.csproj'))])
            self.assertEqual(reads_install(root, ['Utl']), [])

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


class Repo:
    """A disposable Git repository with src projects and the judge's tool trees."""
    def __init__(self, test):
        temp = tempfile.TemporaryDirectory(prefix='quire-release-il-test-')
        test.addCleanup(temp.cleanup)
        self.root = Path(temp.name)
        for args in (('init', '-q', '-b', 'main'), ('config', 'user.name', 'T'), ('config', 'user.email', 't@x.invalid'),
                     ('config', 'core.hooksPath', '')):
            self.git(*args)
        self.write('tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj', '<Project />')
        self.write('tools/Quire.Tools.MirrorGen/Model.cs', '')
        self.write('docs/mirror/type-map.csv', 'a\n')
        self.write('.gitignore', '.mirror-cache/\ninstall/\n')

    def git(self, *args):
        return subprocess.run(['git', *args], cwd=self.root, check=True, capture_output=True, text=True).stdout.strip()

    def write(self, path, text):
        (self.root / path).parent.mkdir(parents=True, exist_ok=True)
        (self.root / path).write_text(text)

    def project(self, name, *references, props=False):
        refs = ''.join(f'<ProjectReference Include="..\\{r}\\{r}.csproj" />' for r in references)
        if props:  # MirrorGen's Generated/Mirror.props, imported by the project
            self.write(f'src/{name}/Generated/Mirror.props', f'<Project>{refs}</Project>')
            refs = ''
        self.write(f'src/{name}/{name}.csproj', f'<Project>{refs}</Project>')

    def commit(self):
        self.git('add', '-A')
        self.git('commit', '-qm', 'c')
        return self.git('rev-parse', 'HEAD')


class ScopeTests(unittest.TestCase):
    def setUp(self):
        self.repo = Repo(self)
        self.repo.project('Emit')
        self.repo.project('Mpl', 'Emit', props=True)
        self.repo.project('Top', 'Mpl')
        self.repo.project('Other')
        self.base = self.repo.commit()

    def affected(self):
        return release_il.affected_projects(self.base, self.repo.root)

    def test_reverse_dependencies_through_props_are_affected(self):
        self.repo.write('src/Emit/Consts.cs', 'const int X = 2;')
        self.assertEqual(self.affected(), ['Emit', 'Mpl', 'Top'])
        self.repo.commit()
        self.assertEqual(self.affected(), ['Emit', 'Mpl', 'Top'])

    def test_a_file_moved_out_of_a_project_affects_both_sides(self):
        self.repo.write('src/Emit/Moved.cs', '')
        self.base = self.repo.commit()
        self.repo.git('mv', 'src/Emit/Moved.cs', 'src/Other/Moved.cs')
        self.repo.commit()
        self.assertEqual(self.affected(), ['Emit', 'Mpl', 'Other', 'Top'])

    def test_a_leaf_change_selects_only_itself(self):
        self.repo.write('src/Top/T.cs', '')
        self.assertEqual(self.affected(), ['Top'])

    def test_type_map_selects_every_project_but_the_judge_selects_none(self):
        self.repo.write('docs/mirror/type-map.csv', 'b\n')
        self.assertEqual(self.affected(), ['Emit', 'Mpl', 'Other', 'Top'])
        self.repo.git('checkout', '--', 'docs')
        self.repo.write('tools/Quire.Tools.MethodAudit/Program.cs', '')
        self.repo.write('tools/Quire.Tools.MirrorGen/Model.cs', 'x')
        self.assertEqual(self.affected(), [])


class CheckTests(unittest.TestCase):
    """check() with builds replaced: where the judge comes from, and the base inventory cache."""
    def setUp(self):
        self.repo = Repo(self)
        self.repo.project('P')
        self.base = self.repo.commit()
        self.repo.write('src/P/T.cs', 'candidate')
        self.tip = self.repo.commit()
        self.install = self.repo.root / 'install'
        (self.install / 'sub').mkdir(parents=True)
        (self.install / 'sub' / 'Ptc.dll').write_bytes(b'x')
        self.cache = self.repo.root / '.mirror-cache' / 'inventory-cache'
        self.runs, self.dotnet, self.scans = 0, [], []
        real_run = release_il.run
        def run(command, cwd, log, timeout, env=None):
            if command[0] == 'git':
                return real_run(command, cwd, log, timeout, env)
            self.dotnet.append((list(map(str, command)), Path(cwd)))
        def scan(root, projects, tool, work, *args, **kwargs):
            self.scans.append(Path(root))
            return {'methods': [method(exact=False, original={'assembly': 'A', 'sha256': 's', 'mvid': 'm'})]}
        for target, value in (('run', run), ('build_and_scan', scan)):
            patcher = mock.patch.object(release_il, target, value)
            patcher.start()
            self.addCleanup(patcher.stop)

    def check(self, base=None, **kwargs):
        self.runs += 1
        return release_il.check(base or self.base, ['P'], self.repo.root / '.mirror-cache' / f'r{self.runs}',
                                root=self.repo.root, install=self.install, cache=self.cache, **kwargs)

    def test_the_judge_is_built_from_the_base_commit(self):
        report = self.check()
        self.assertTrue(report['passed'], report)
        command, cwd = self.dotnet[0]
        self.assertTrue(command[2].endswith('Quire.Tools.MethodAudit.csproj'))
        self.assertFalse(Path(command[2]).is_relative_to(self.repo.root))
        self.assertNotEqual(cwd, self.repo.root)

    def test_a_base_without_the_judge_fails_closed(self):
        self.repo.git('rm', '-rq', 'tools/Quire.Tools.MethodAudit')
        no_judge = self.repo.commit()
        report = self.check(no_judge)
        self.assertFalse(report['passed'])
        self.assertIn('no MethodAudit', report['error'])
        self.assertEqual(self.scans, [])

    def test_base_inventories_are_cached_and_keyed_by_install_and_projects(self):
        self.assertFalse(self.check()['base_cached'])
        self.assertEqual(len(self.scans), 2)
        self.assertTrue(self.check()['base_cached'])
        self.assertEqual(len(self.scans), 3)  # the candidate only
        (self.install / 'sub' / 'Ptc.dll').write_bytes(b'xy')  # another install: a miss
        self.assertFalse(self.check()['base_cached'])
        for entry in self.cache.glob('*.json'):  # corrupt entries fall back to building
            entry.write_text('{')
        self.assertFalse(self.check()['base_cached'])
        key = release_il.inventory_key
        self.assertNotEqual(key(self.repo.root, self.base, self.base, ['P'], self.install),
                            key(self.repo.root, self.base, self.base, ['P', 'Q'], self.install))

    def test_only_a_committed_passing_candidate_is_stored(self):
        key = release_il.inventory_key(self.repo.root, self.tip, self.base, ['P'], self.install)
        self.repo.write('src/P/Dirty.cs', '')
        self.check(store_candidate=True)
        self.assertIsNone(release_il.cached_inventory(self.cache, key))
        (self.repo.root / 'src/P/Dirty.cs').unlink()
        self.check()
        self.assertIsNone(release_il.cached_inventory(self.cache, key))
        self.check(store_candidate=True)
        self.assertIsNotNone(release_il.cached_inventory(self.cache, key))
        # The next run whose base is that tip starts from the stored inventory.
        self.repo.write('src/P/U.cs', '')
        self.repo.commit()
        self.assertTrue(self.check(self.tip)['base_cached'])
        # A candidate that changed the judge was judged by the old one: its entry never serves as a base.
        self.repo.write('tools/Quire.Tools.MethodAudit/Program.cs', '')
        judge_tip = self.repo.commit()
        self.check(self.tip, store_candidate=True)
        self.repo.write('src/P/V.cs', '')
        self.repo.commit()
        self.assertFalse(self.check(judge_tip)['base_cached'])

    def test_a_failed_scan_keeps_its_inventory_and_reports_errors(self):
        mock.patch.stopall()
        work = self.repo.root / 'work'
        keep = self.repo.root / 'kept.json'
        def audit(command, cwd, log, timeout, env=None):
            if '--output' in map(str, command):
                Path(command[command.index('--output') + 1]).write_text(
                    json.dumps({'methods': [], 'errors': [f'e{i}' for i in range(25)]}))
                raise RuntimeError('command failed (1)')
        with mock.patch.object(release_il, 'run', audit), \
                mock.patch.object(release_il, 'reads_install', return_value=[]), \
                mock.patch.object(release_il, 'section_names', return_value=[]), \
                mock.patch.object(release_il, 'msvc_mismatch', return_value=None):
            with self.assertRaises(RuntimeError) as raised:
                release_il.build_and_scan(self.repo.root, ['P'], 'tool.dll', work, self.install, 'dotnet',
                                          work / 'log', 20, keep=keep)
        message = str(raised.exception)
        self.assertIn('e0; e1', message)
        self.assertIn('e19; 5 more', message)
        self.assertIn('command failed (1)', message)
        self.assertEqual(len(json.loads(keep.read_text())['errors']), 25)
        # One build invocation names every project.
        self.assertIn(str((self.repo.root / 'src/P/P.csproj').resolve()), (work / 'build.slnx').read_text())


if __name__ == '__main__':
    unittest.main()
