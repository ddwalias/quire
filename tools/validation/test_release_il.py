"""Acceptance checks for the Release IL gate, without executing production code."""
import json
import hashlib
import os
from pathlib import Path
import stat
import subprocess
import sys
import tempfile
import time
import unittest
from unittest import mock
import release_il
from release_il import (LINKER_PRODUCT, MSVC_BUILD, builds_cppcli, compare, msvc_mismatch, ordinalless)


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

    def test_new_generated_stub_is_unfinished_unless_unpaired_or_misdeclared(self):
        self.assertTrue(compare(inventory(), inventory(method(exact=False, stub_body=True)))['passed'])
        self.assertFalse(compare(inventory(), inventory(method(exact=False, stub_body=True, original=None)))['passed'])
        self.assertFalse(compare(inventory(), inventory(method(exact=False, mirror_stub=True, shape_matches=False)))['passed'])
        self.assertFalse(compare(inventory(), inventory(method(exact=False)))['passed'])

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


class ClonePairingTests(unittest.TestCase):
    def test_abstract_clone_methodimpl_identity_is_narrow(self):
        root = Path(__file__).resolve().parents[2]
        with tempfile.TemporaryDirectory(prefix='quire-clone-pairing-') as directory:
            work = Path(directory)
            model = root / 'tools/Quire.Tools.MirrorGen/Model.cs'
            (work / 'Test.csproj').write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
<ItemGroup><Compile Include="{model}" Link="Model.cs" /></ItemGroup></Project>''')
            (work / 'Program.cs').write_text('''
using Quire.Tools.MirrorGen;
TypeModel Type(bool abstractClass, TypeKind kind = TypeKind.Class) => new() {
    Assembly = "A", FullName = "T", Namespace = "", Name = "T",
    Kind = kind, Access = Access.Private, Abstract = abstractClass };
var type = Type(true);
var method = new MethodModel("c", Access.Private, false, false, true, true, true, false,
    new NamedSig("System.Runtime", "System.Object"), [], [],
    [(new NamedSig("System.Runtime", "System.ICloneable"), "Clone")], false, false, false);
void Check(MethodModel m, TypeModel? t, string expected)
{
    string actual = Canonical.MethodName(m, declaringType: t);
    if (actual != expected) throw new Exception($"{actual} != {expected}");
}
Check(method, type, "System.ICloneable.Clone");
Check(method with { Name = "System.ICloneable.Clone" }, type, "System.ICloneable.Clone");
Check(method, null, "c");
Check(method, Type(false), "c");
Check(method, Type(true, TypeKind.Interface), "c");
Check(method with { Access = Access.Public }, type, "c");
Check(method with { Static = true }, type, "c");
Check(method with { Return = new NamedSig("System.Runtime", "System.String") }, type, "c");
Check(method with { Implements = [] }, type, "c");
Check(method with { Implements = [(new NamedSig("System.Runtime", "System.IDisposable"), "Dispose")] }, type, "c");
Check(method with { Implements = [(new NamedSig("System.Runtime", "System.ICloneable"), "Other")] }, type, "c");
''')
            result = subprocess.run(['dotnet', 'run', '--project', str(work / 'Test.csproj'),
                                     '--configuration', 'Release'], capture_output=True, text=True)
            self.assertEqual(result.returncode, 0, result.stdout + result.stderr)


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
        self.runs, self.dotnet, self.scans, self.scanned = 0, [], [], []
        self.rows = self.row
        real_run = release_il.run
        def run(command, cwd, log, timeout, env=None):
            if command[0] in ('git', 'tar'):
                return real_run(command, cwd, log, timeout, env)
            self.dotnet.append((list(map(str, command)), Path(cwd)))
        def scan(root, projects, tool, work, *args, **kwargs):
            self.scans.append(Path(root))
            self.scanned.append(list(projects))
            return {'methods': [row for project in projects for row in self.rows(project)]}
        self.purged = []
        for target, value in (('run', run), ('build_and_scan', scan), ('purge', lambda *paths: self.purged.append(paths))):
            patcher = mock.patch.object(release_il, target, value)
            patcher.start()
            self.addCleanup(patcher.stop)

    @staticmethod
    def row(project, **extra):
        """MethodAudit's rows of one project: each names the project it was read from."""
        return [method(**dict(dict(id=f'{project}::T::M', project=project, exact=False,
                                   original={'assembly': 'A', 'sha256': 's', 'mvid': 'm'}), **extra))]

    def check(self, base=None, projects=('P',), **kwargs):
        self.runs += 1
        return release_il.check(base or self.base, list(projects), self.repo.root / '.mirror-cache' / f'r{self.runs}',
                                root=self.repo.root, install=self.install, cache=self.cache, **kwargs)

    def test_timings_are_written_without_changing_verdict(self):
        report = self.check()
        self.assertTrue(report['passed'])
        self.assertEqual(set(report['timings']), {
            'judge_build', 'base_checkout', 'base_build', 'base_scan',
            'candidate_build', 'candidate_scan', 'compare', 'total'})
        for duration in report['timings'].values():
            self.assertGreaterEqual(duration, 0)
        saved = json.loads((self.repo.root / '.mirror-cache/r1/release-il.json').read_text())
        self.assertEqual(saved['timings'], report['timings'])
        self.assertGreater(report['timings']['total'], 0)
        cached = self.check()
        self.assertEqual(cached['timings']['base_build'], 0)
        self.assertEqual(cached['timings']['base_scan'], 0)

    def test_the_judge_is_built_from_the_base_commit(self):
        report = self.check()
        self.assertTrue(report['passed'], report)
        command, cwd = self.dotnet[0]
        self.assertTrue(command[2].endswith('Quire.Tools.MethodAudit.csproj'))
        self.assertTrue(Path(command[2]).is_relative_to(self.cache.parent / 'gate-work'))
        self.assertNotEqual(cwd, self.repo.root)

    def test_a_base_without_the_judge_fails_closed(self):
        self.repo.git('rm', '-rq', 'tools/Quire.Tools.MethodAudit')
        no_judge = self.repo.commit()
        report = self.check(no_judge)
        self.assertFalse(report['passed'])
        self.assertIn('no MethodAudit', report['error'])
        self.assertEqual(self.scans, [])

    def test_base_inventories_are_cached_and_keyed_by_install(self):
        self.assertFalse(self.check()['base_cached'])
        self.assertEqual(len(self.scans), 2)
        self.assertTrue(self.check()['base_cached'])
        self.assertEqual(len(self.scans), 3)  # the candidate only
        (self.install / 'sub' / 'Ptc.dll').write_bytes(b'xy')  # another install: a miss
        self.assertFalse(self.check()['base_cached'])
        for entry in self.cache.glob('*.json'):  # corrupt entries fall back to building
            entry.write_text('{')
        self.assertFalse(self.check()['base_cached'])

    def three_projects(self):
        """P; Q referencing P through Generated/Mirror.props; R on its own."""
        self.repo.project('Q', 'P', props=True)
        self.repo.project('R')
        self.base = self.repo.commit()

    def test_a_base_hits_on_rows_stored_for_other_project_sets_and_a_partial_hit_builds(self):
        self.three_projects()
        self.assertFalse(self.check(projects=['P', 'R'])['base_cached'])
        self.assertTrue(self.check(projects=['P'])['base_cached'])
        self.assertTrue(self.check(projects=['R'])['base_cached'])
        scans = len(self.scanned)
        self.assertFalse(self.check(projects=['P', 'Q'])['base_cached'])  # Q was never scanned: build both
        self.assertEqual(self.scanned[scans], ['P', 'Q'])
        self.assertTrue(self.check(projects=['Q', 'P', 'R'])['base_cached'])
        # The assembled inventory is what one scan of these projects in this order writes.
        base = json.loads((self.repo.root / '.mirror-cache' / f'r{self.runs}' / 'base-inventory.json').read_text())
        self.assertEqual([m['id'] for m in base['methods']], ['Q::T::M', 'P::T::M', 'R::T::M'])

    def test_rows_are_reused_whether_or_not_their_references_were_scanned_with_them(self):
        """MethodAudit binds a reference alike whether or not it audits it, so the scanned set is not keyed."""
        self.three_projects()
        self.check(projects=['P', 'Q'])
        self.assertTrue(self.check(projects=['Q'])['base_cached'])
        self.assertTrue(self.check(projects=['Q', 'P'])['base_cached'])
        self.assertFalse(self.check(projects=['Q', 'R'])['base_cached'])  # R never scanned: a partial hit builds
        self.assertTrue(self.check(projects=['R', 'Q'])['base_cached'])

    def test_whether_a_mixed_mode_reference_was_scanned_is_keyed(self):
        """Unscanned, a mixed-mode reference is built in Debug: its IL-only C# stand-in, not the mixed DLL."""
        self.repo.project('U')
        self.repo.write('src/U/MixedMode/U.vcxproj', '<Project />')
        self.repo.project('E', 'U')
        self.base = self.repo.commit()
        self.assertFalse(self.check(projects=['E'])['base_cached'])
        self.assertFalse(self.check(projects=['E', 'U'])['base_cached'])
        self.assertTrue(self.check(projects=['E', 'U'])['base_cached'])
        self.assertTrue(self.check(projects=['E'])['base_cached'])

    def test_a_changed_reference_misses_and_an_unrelated_change_hits(self):
        self.three_projects()
        self.check(projects=['Q'])
        self.repo.write('src/R/U.cs', 'unrelated')
        self.assertTrue(self.check(self.repo.commit(), projects=['Q'])['base_cached'])
        self.repo.write('src/P/U.cs', 'compiled into what Q references')
        self.assertFalse(self.check(self.repo.commit(), projects=['Q'])['base_cached'])

    def test_the_type_map_root_files_src_projects_and_judge_are_keyed_but_not_root_prose(self):
        self.three_projects()
        self.check(projects=['Q'])
        for path in ('docs/mirror/type-map.csv', 'Directory.Build.props', '.gitattributes', 'src/S/S.csproj',
                     'tools/Quire.Tools.MethodAudit/Program.cs', 'tools/Quire.Tools.MirrorGen/Model.cs'):
            with self.subTest(path):
                self.repo.write(path, path)
                base = self.repo.commit()
                self.assertFalse(self.check(base, projects=['Q'])['base_cached'])
                self.assertTrue(self.check(base, projects=['Q'])['base_cached'])
        self.repo.write('README.md', 'prose')
        self.assertTrue(self.check(self.repo.commit(), projects=['Q'])['base_cached'])

    def test_the_gate_script_is_keyed(self):
        """A landed change to the build recipe (flags, the MSVC pin, MethodAudit's arguments) invalidates the rows."""
        self.check()
        self.assertTrue(self.check()['base_cached'])
        changed = self.repo.root / 'release_il_changed.py'
        changed.write_bytes(Path(release_il.__file__).read_bytes() + b'# another recipe\n')
        with mock.patch.object(release_il, '__file__', str(changed)):
            self.assertFalse(self.check()['base_cached'])
            self.assertTrue(self.check()['base_cached'])

    def test_a_reference_the_commit_lacks_fails_closed(self):
        self.repo.project('Q', 'Gone')
        self.base = self.repo.commit()
        self.assertFalse(self.check(projects=['Q'])['base_cached'])
        self.assertFalse(self.check(projects=['Q'])['base_cached'])
        self.assertEqual(list(self.cache.glob('*.json')), [])

    def test_a_build_input_outside_the_closure_fails_closed(self):
        """A key covers the closure's directories; an Import or linked item elsewhere would go unseen."""
        self.repo.project('R')
        for case, text in (('linked source', '<Project><ItemGroup><Compile Include="..\\R\\F.cs" /></ItemGroup></Project>'),
                           ('imported props', '<Project><Import Project="..\\..\\R\\X.props" /></Project>'),
                           ('property path', '<Project><ItemGroup><None Include="$(SolutionDir)x.txt" /></ItemGroup></Project>')):
            with self.subTest(case):
                self.repo.write('src/P/Generated/Mirror.props', text)
                self.base = self.repo.commit()
                self.assertFalse(self.check()['base_cached'])
                self.assertFalse(self.check()['base_cached'])
        self.repo.write('src/P/Generated/Mirror.props', '<Project><ItemGroup><Compile Include="Local\\F.cs" />'
                        '<Import Project="$(VCTargetsPath)\\Microsoft.Cpp.props" /></ItemGroup></Project>')
        self.base = self.repo.commit()
        self.check()
        self.assertTrue(self.check()['base_cached'])

    def test_a_project_reference_in_a_targets_file_is_in_the_closure(self):
        self.repo.project('R')
        self.repo.write('src/P/P.targets', '<Project><ItemGroup><ProjectReference Include="..\\R\\R.csproj" />'
                        '</ItemGroup></Project>')
        self.base = self.repo.commit()
        self.check()
        self.repo.write('src/R/U.cs', 'referenced through a .targets file')
        self.assertFalse(self.check(self.repo.commit())['base_cached'])

    def test_rows_of_an_assembly_not_named_as_its_project_are_not_stored(self):
        self.rows = lambda project: self.row('Other')
        self.check()
        self.assertEqual(list(self.cache.glob('*.json')), [])

    def test_only_a_committed_passing_candidate_is_stored(self):
        entries = lambda: len(list(self.cache.glob('*.json')))
        self.repo.write('src/P/Dirty.cs', '')
        self.check(store_candidate=True)
        self.assertEqual(entries(), 1)  # the base's
        (self.repo.root / 'src/P/Dirty.cs').unlink()
        self.check()
        self.assertEqual(entries(), 1)
        # A clean tree that was not checked out with committed bytes (il_merge's RAW_BYTES) may hold converted EOLs.
        self.check(store_candidate=True)
        self.assertEqual(entries(), 1)
        (self.repo.root / '.git/info').mkdir(exist_ok=True)
        (self.repo.root / '.git/info/attributes').write_text(release_il.RAW_BYTES, encoding='utf-8')
        self.check(store_candidate=True)
        self.assertEqual(entries(), 2)
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

    def test_runs_discard_their_work_and_a_killed_runs_but_not_a_running_one(self):
        gate_work = self.cache.parent / 'gate-work'
        killed, running = gate_work / 'quire-release-il-killed', gate_work / 'quire-release-il-running'
        for directory in (killed, running):
            (directory / 'base').mkdir(parents=True)
        old = time.time() - release_il.STALE_WORK - 60
        os.utime(killed, (old, old))
        self.assertTrue(self.check()['passed'])
        self.assertEqual([p.name for p in gate_work.glob('quire-release-il-*')], [running.name])
        trashed = {p.name for p in (gate_work / '.trash').iterdir()}
        self.assertEqual(len(trashed), 2)
        self.assertIn(killed.name, trashed)
        self.assertEqual(self.purged, [(gate_work / '.trash',)])
        # A failing run discards its work too.
        self.repo.git('rm', '-rq', 'tools/Quire.Tools.MethodAudit')
        self.assertFalse(self.check(self.repo.commit())['passed'])
        self.assertEqual([p.name for p in gate_work.glob('quire-release-il-*')], [running.name])
        self.assertEqual(len(self.purged), 2)

    def test_an_unrenamable_run_is_purged_in_place_but_another_runs_is_left(self):
        gate_work = self.cache.parent / 'gate-work'
        killed = gate_work / 'quire-release-il-killed'
        killed.mkdir(parents=True)
        old = time.time() - release_il.STALE_WORK - 60
        os.utime(killed, (old, old))
        real_replace = os.replace
        def replace(source, target):  # a file still open in every work directory
            if Path(source).name.startswith('quire-release-il-'):
                raise PermissionError(source)
            return real_replace(source, target)
        with mock.patch.object(release_il.os, 'replace', replace):
            self.assertTrue(self.check()['passed'])
        [(trash, own)] = self.purged
        self.assertEqual(trash, gate_work / '.trash')
        self.assertEqual(own.parent, gate_work)
        self.assertNotEqual(own, killed)
        self.assertTrue(killed.is_dir())

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


class JudgeCacheTests(unittest.TestCase):
    def setUp(self):
        self.repo = Repo(self)
        self.base = self.repo.commit()
        self.cache = self.repo.root / '.mirror-cache'

    def build(self):
        return release_il.build_judge(self.repo.root, self.base, self.cache, 'dotnet',
                                      self.cache / 'build.log', 60)

    def fake_build(self, command, *args):
        output = Path(command[command.index('-o') + 1])
        (output / 'Quire.Tools.MethodAudit.dll').write_bytes(b'judge')

    def test_miss_then_hit_and_changed_source_misses(self):
        with mock.patch.object(release_il.subprocess, 'check_output', return_value='10.0.100'), \
                mock.patch.object(release_il, 'run', side_effect=self.fake_build) as build:
            first = self.build()
            self.assertEqual(build.call_count, 1)
            self.assertEqual(first, self.build())
            self.assertEqual(build.call_count, 1)
            self.repo.write('tools/Quire.Tools.MirrorGen/Model.cs', 'changed')
            self.base = self.repo.commit()
            self.assertNotEqual(first, self.build())
            self.assertEqual(build.call_count, 2)
        with mock.patch.object(release_il.subprocess, 'check_output', return_value='10.0.101'), \
                mock.patch.object(release_il, 'run', side_effect=self.fake_build) as build:
            self.build()
            self.assertEqual(build.call_count, 1)

    def test_checkout_file_list_and_hashes_equal_commit(self):
        self.repo.write('.gitattributes', '*.txt export-ignore\n*.cs export-subst\n')
        self.repo.write('included.txt', 'line one\nline two\n')
        self.repo.write('tools/Quire.Tools.MirrorGen/Model.cs', '$Format:%H$\n')
        self.base = self.repo.commit()
        work = Path(tempfile.mkdtemp(prefix='quire-checkout-test-'))
        original = work / 'base'
        release_il.checkout_base(self.repo.root, self.base, original, work / 'log', 60)
        files = sorted(p.relative_to(original).as_posix() for p in original.rglob('*')
                       if p.is_file() and '.git' not in p.relative_to(original).parts)
        expected = self.repo.git('ls-tree', '-r', '--name-only', self.base).splitlines()
        self.assertEqual(files, sorted(expected))
        for name in expected:
            blob = subprocess.check_output(['git', 'show', f'{self.base}:{name}'], cwd=self.repo.root)
            self.assertEqual(hashlib.sha256(blob).digest(), hashlib.sha256((original / name).read_bytes()).digest())


class WorkCleanupTests(unittest.TestCase):
    def test_purge_empties_trash_and_deletes_unmoved_directories_with_read_only_files(self):
        temp = tempfile.TemporaryDirectory(prefix='quire-purge-test-')
        self.addCleanup(temp.cleanup)
        root = Path(temp.name)
        trash, unmoved = root / '.trash', root / 'quire-release-il-open'
        for directory in (trash / 'quire-release-il-a' / '.git' / 'objects', unmoved / 'after'):
            directory.mkdir(parents=True)
            (directory / 'f').write_bytes(b'x')
            os.chmod(directory / 'f', stat.S_IREAD)
        subprocess.run([sys.executable, '-c', release_il.PURGE, trash, unmoved], check=True, timeout=60)
        self.assertEqual(list(trash.iterdir()), [])
        self.assertFalse(unmoved.exists())



if __name__ == '__main__':
    unittest.main()
