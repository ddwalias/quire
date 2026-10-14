import tempfile
from pathlib import Path
import unittest
from integrity import check, lint, listings

SHA, MVID = 'a' * 64, '12345678-1234-1234-1234-123456789abc'


class IntegrityTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()
        self.addCleanup(self.directory.cleanup)
        self.repo = Path(self.directory.name)
        (self.repo / 'src/Quire.Mpl/Generated').mkdir(parents=True)
        (self.repo / 'docs/mirror/il/mplrun').mkdir(parents=True)
        self.method = dict(type='Quire.Mcd.Dim', status='Verified',
                           original=dict(assembly='mplrun', sha256=SHA, mvid=MVID, type='Mcd.Dim+Inner'))
        self.inventory = dict(methods=[self.method])

    def source(self, text, name='Quire.Mpl/Dim.cs'):
        (self.repo / 'src' / name).write_text(text)
        return [f['rule'] for f in lint(self.repo)]

    def listing(self, sha=SHA, mvid=MVID, name='Mcd.Dim.il'):
        (self.repo / 'docs/mirror/il/mplrun' / name).write_text(
            f'// Mathcad IL listing: mplrun Mcd.Dim\n// Tool: ilspycmd 11.1.0.9782\n// SHA-256: {sha}\n// MVID: {mvid}\n.class Mcd.Dim\n')

    def test_decompiler_artifacts_found(self):
        for text, rule in [('goto IL_0010;', 'goto statement'), ('var x = <>c.Instance;', 'compiler-generated name'),
                           ('int num2 = 0;', 'decompiler local name'), ('Type type2 = FType(a);', 'decompiler local name'),
                           ('foreach (var item2 in list)', 'decompiler local name'), ('// Warning: some IL', 'decompiler comment')]:
            with self.subTest(text=text):
                self.assertIn(rule, self.source(text))

    def test_mirrored_names_and_generics_are_not_artifacts(self):
        self.assertEqual(self.source('SCons.list2(x, y); var d = typeof(Dictionary<,>); Field1 = list3(a, b, c);'), [])

    def test_generated_stubs_skipped_and_only_exact_allowlisted_lines_exempt(self):
        self.assertEqual(self.source('int num2 = 0;', 'Quire.Mpl/Generated/Stub.g.cs'), [])
        self.assertEqual(self.source('int num2 = 0; // quire-lint: allow'), ['decompiler local name'])
        (self.repo / 'docs/mirror/lint-allowed.txt').write_text(
            '# reviewed\nsrc/Quire.Mpl/Dim.cs | decompiler local name | int num2 = 0;\n')
        self.assertEqual(self.source('int num2 = 0;'), [])
        self.assertEqual(self.source('int num2 = 0; int num3 = 1;'), ['decompiler local name', 'decompiler local name'])
        self.assertEqual(self.source('long num3 = 0;'), ['decompiler local name'])

    def test_quire_mpl_cannot_reach_the_harness(self):
        for text in ('[ModuleInitializer] static void Init() { }', 'var t = "QUIRE_MUTATION_KILLS";',
                     'Environment.GetEnvironmentVariable(name);', 'AppDomain.CurrentDomain.GetAssemblies();'):
            with self.subTest(text=text):
                self.assertIn('forbidden harness reach', self.source(text))
        self.assertEqual(self.source('#if QUIRE_MIRROR_STUB_FREE\n#endif'), [])
        (self.repo / 'src/Quire.Engine').mkdir()
        self.source('[ModuleInitializer] static void Init() { }', 'Quire.Engine/X.cs')
        self.assertEqual([f for f in lint(self.repo) if 'Engine' in f['file']], [])

    def differential(self, text, name='XDifferentialTests.cs'):
        directory = self.repo / 'tests/Quire.Differential.Tests'
        (directory / name).parent.mkdir(parents=True, exist_ok=True)
        (directory / name).write_text(text)
        return [f['rule'] for f in lint(self.repo) if f['file'].startswith('tests/')]

    def test_differential_tests_cannot_shadow_or_escape_the_harness(self):
        good = 'namespace Quire.Differential.Tests;\n[Trait("a", "b")]\npublic class XDifferentialTests\n{\n' \
               '    private sealed class Helper { }\n    string s = "class Assert";\n}\n'
        self.assertEqual(self.differential(good), [])
        header = 'namespace Quire.Differential.Tests;\npublic class XDifferentialTests { }\n'
        for text, rule in [(header + 'static class Assert { }', 'test file shape'),
                           (header + '    [System.Obsolete] static class Reflect { }', 'test file shape'),
                           ('public class XDifferentialTests { }', 'test file shape'),
                           ('namespace Quire.Differential { public class XDifferentialTests { } }', 'test file shape'),
                           ('namespace Quire;\npublic class XDifferentialTests { }', 'test file shape'),
                           ('namespace Quire.Differential.Tests;\npublic class YDifferentialTests { }', 'test file shape'),
                           (header + 'global using Assert = Fake;', 'global using'),
                           (header + 'var e = Environment.GetEnvironmentVariable("X");', 'forbidden test API'),
                           (header + 'var s = "QUIRE_MUTATION_KILLS";', 'forbidden test API'),
                           (header + 'if (typeof(X).Assembly.Location.Contains("mutant")) { }', 'forbidden test API'),
                           (header + 'var il = m.GetMethodBody()!.GetILAsByteArray();', 'forbidden test API'),
                           (header + 'Thread.Sleep(100000);', 'forbidden test API'),
                           (header + 'catch (Xunit.Sdk.XunitException) { }', 'forbidden test API'),
                           (header + 'var t = Type.GetType(name);', 'forbidden test API'),
                           (header + 'var f = typeof(MethodComparison).GetMethods();', 'forbidden test API'),
                           ('#line hidden', 'line directive')]:
            with self.subTest(text=text):
                self.assertIn(rule, self.differential(text))

    def test_case_variant_and_nested_test_files(self):
        bad = 'namespace Quire;\nstatic class Assert { QUIRE_X }\n'
        self.assertIn('forbidden test API', self.differential(bad, 'Evildifferentialtests.cs'))
        (self.repo / 'tests/Quire.Differential.Tests/Evildifferentialtests.cs').unlink()
        # Protected subdirectory files are maintainer-reviewed and not linted as agent tests.
        self.assertEqual(self.differential(bad, 'Sub/EvilDifferentialTests.cs'), [])

    def test_analyzed_type_requires_listing_for_top_level_type(self):
        self.assertEqual(listings(self.inventory, self.repo)['errors'],
                         ['Quire.Mcd.Dim: missing IL listing il/mplrun/Mcd.Dim.il'])
        self.listing()
        self.assertTrue(check(self.inventory, self.repo)['complete'])

    def test_stub_types_and_scoped_out_types_need_no_listing(self):
        self.assertEqual(listings(dict(methods=[dict(self.method, status='Stub')]), self.repo)['errors'], [])
        self.assertEqual(listings(self.inventory, self.repo, types=['Quire.Other'])['errors'], [])

    def test_listing_must_name_the_loaded_artifact(self):
        self.listing(sha='b' * 64)
        self.assertIn('header names another artifact', listings(self.inventory, self.repo)['errors'][0])
        (self.repo / 'docs/mirror/il/mplrun/Mcd.Dim.il').write_text('.class Mcd.Dim\n')
        self.assertIn('header lacks', listings(self.inventory, self.repo)['errors'][0])

    def test_listing_for_unloaded_assembly_reported_unchecked(self):
        (self.repo / 'docs/mirror/il/iengine').mkdir()
        (self.repo / 'docs/mirror/il/iengine/Engine.IEngine.il').write_text('')
        self.assertEqual(listings(self.inventory, self.repo)['unchecked'],
                         ['iengine/Engine.IEngine.il: assembly not loaded by this audit'])


if __name__ == '__main__':
    unittest.main()
