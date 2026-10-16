import tempfile
from pathlib import Path
import unittest
from integrity import lint, test_types


class IntegrityTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()
        self.addCleanup(self.directory.cleanup)
        self.repo = Path(self.directory.name)
        (self.repo / 'src/Quire.Mpl/Generated').mkdir(parents=True)
        (self.repo / 'docs/mirror').mkdir(parents=True)

    def source(self, text, name='Quire.Mpl/Dim.cs'):
        (self.repo / 'src' / name).write_text(text)
        return [f['rule'] for f in lint(self.repo)]

    def test_decompiler_artifacts_found(self):
        for text, rule in [('goto IL_0010;', 'goto statement'), ('var x = <>c.Instance;', 'compiler-generated name'),
                           ('int num2 = 0;', 'decompiler local name'), ('Type type2 = FType(a);', 'decompiler local name'),
                           ('foreach (var item2 in list)', 'decompiler local name'), ('// Warning: some IL', 'decompiler comment')]:
            with self.subTest(text=text):
                self.assertIn(rule, self.source(text))

    def test_mirrored_names_and_generics_are_not_artifacts(self):
        self.assertEqual(self.source('SCons.list2(x, y); var d = typeof(Dictionary<,>); Field1 = list3(a, b, c);'), [])

    def test_only_ilspy_is_matched_case_insensitively(self):
        self.assertEqual(self.source('// warning: keep in step with Dim'), [])
        for text in ('// ILSpy-generated view', '// Warning: some IL'):
            with self.subTest(text=text):
                self.assertIn('decompiler comment', self.source(text))

    def test_generated_stubs_skipped_and_only_exact_allowlisted_lines_exempt(self):
        self.assertEqual(self.source('int num2 = 0;', 'Quire.Mpl/Generated/Stub.g.cs'), [])
        self.assertEqual(self.source('int num2 = 0; // quire-lint: allow'), ['decompiler local name'])
        (self.repo / 'docs/mirror/lint-allowed.txt').write_text(
            '# reviewed\nsrc/Quire.Mpl/Dim.cs | decompiler local name | int num2 = 0;\n')
        self.assertEqual(self.source('int num2 = 0;'), [])
        self.assertEqual(self.source('int num2 = 0;\nint num2 = 0;'), ['decompiler local name'])  # a copy needs review
        self.assertEqual(self.source('int num2 = 0; int num3 = 1;'), ['decompiler local name', 'decompiler local name'])
        self.assertEqual(self.source('long num3 = 0;'), ['decompiler local name'])

    def test_quire_mpl_cannot_reach_the_harness(self):
        for text in ('[ModuleInitializer] static void Init() { }', 'var t = "QUIRE_MUTATION_KILLS";',
                     'Environment.GetEnvironmentVariable(name);', 'AppDomain.CurrentDomain.GetAssemblies();',
                     'foreach (var e in Environment.GetEnvironmentVariables()) { }',
                     'Environment.ExpandEnvironmentVariables(x);', 'namespace Quire.Differential.Tests { }'):
            with self.subTest(text=text):
                self.assertIn('forbidden harness reach', self.source(text))
        self.assertEqual(self.source('#if QUIRE_MIRROR_STUB_FREE\n#endif'), [])
        self.assertIn('forbidden harness reach', self.source('[ModuleInitializer] static void I() { }', 'Quire.Mpl/Evil.CS'))
        (self.repo / 'src/Quire.Engine').mkdir()
        self.source('[ModuleInitializer] static void Init() { }', 'Quire.Engine/X.cs')
        self.assertEqual([f for f in lint(self.repo) if 'Engine' in f['file']], [])

    def test_src_compiles_the_same_in_debug_and_release(self):
        for text in ('#if DEBUG\n#endif', '  # elif !RELEASE && X', '#if X || DEBUG'):
            with self.subTest(text=text):
                self.assertEqual(self.source(text, 'Quire.Engine.cs'), ['configuration-dependent code'])
        for text in ('#if QUIRE_MIRROR_STUB_FREE\n#endif', 'x = Conditional(a, b);', '// Debug.Assert(x);'):
            with self.subTest(text=text):
                self.assertEqual(self.source(text, 'Quire.Engine.cs'), [])

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
                           (header + 'var t = typeof(Mcd).Assembly.DefinedTypes;', 'forbidden test API'),
                           (header + 'var t = asm.ExportedTypes.First();', 'forbidden test API'),
                           (header + 'var t = asm.GetExportedTypes();', 'forbidden test API'),
                           (header + 'catch (Xunit.Sdk.XunitException) { }', 'forbidden test API'),
                           (header + 'var t = Type.GetType(name);', 'forbidden test API'),
                           (header + 'var t = typeof(Dim).Assembly.GetType("Quire.Mcd.Dim");', 'forbidden test API'),
                           (header + 'var t = a.GetType( $"Quire.{n}");', 'forbidden test API'),
                           (header + 'var f = typeof(MethodComparison).GetMethods();', 'forbidden test API'),
                           (header + 'file static class Assert { }', 'file-local type'),
                           (header + '[Obsolete] file sealed record Assert;', 'file-local type'),
                           (header + '// note\u2028file class Assert { }', 'file-local type'),
                           (header + '[assembly: Xunit.TestFramework("X", "Y")]', 'assembly or module attribute'),
                           (header + '// ILSpy-generated view', 'decompiler comment'),
                           (header + '// ilspycmd 11.1.0.9782', 'decompiler comment'),
                           (header + '// SHA-256: 1527', 'decompiler comment'),
                           ('#line hidden', 'line directive')]:
            with self.subTest(text=text):
                self.assertIn(rule, self.differential(text))

    def test_file_in_line_comments_and_simple_strings_is_not_a_file_local_type(self):
        text = 'namespace Quire.Differential.Tests;\npublic class XDifferentialTests\n{\n' \
               '    // one file record per case\n    string s = "file struct", t = $"{n} file class"; // file enum\n}\n'
        self.assertEqual(self.differential(text), [])

    def test_constructs_that_fool_a_lexer_cannot_hide_a_file_local_type(self):
        header = 'namespace Quire.Differential.Tests;\npublic class XDifferentialTests { }\n'
        for text in ['[A($"{M("/*")}")]\nfile class Assert { }\n// */',  # nested quotes in an interpolation hole
                     '#region @"\nfile class Assert { }\n#endregion "',  # preprocessor text is not a string
                     '[A(@$"\\")] file class Assert { } //")]',  # @$ is verbatim: the backslash escapes nothing
                     '[A(@$"\nx")] file class Assert { } // "',  # the line starts inside a verbatim string
                     '/* a file class */', 'file/**/class Assert { }']:  # block comments no longer hide prose
            with self.subTest(text=text):
                self.assertIn('file-local type', self.differential(header + text))

    def test_compiled_test_types_must_come_from_their_own_file(self):
        root = self.repo.resolve().as_posix()
        tests = f'{root}/tests/Quire.Differential.Tests'
        def types(*entries):
            return [f['text'] for f in test_types({'test_types': [dict(name=n, documents=d) for n, d in entries]}, self.repo)]
        self.assertEqual(types(('Quire.Differential.Tests.XDifferentialTests', [f'{tests}/XDifferentialTests.cs']),
                               ('Quire.Differential.Tests.Reflect', [f'{tests}/Reflect.cs']),
                               ('Quire.Differential.Tests.HarnessException', [f'{tests}/Reflect.cs']),
                               ('<>f__AnonymousType0`2', []), ('<PrivateImplementationDetails>', []),
                               ('AutoGeneratedProgram', ['/root/.nuget/packages/microsoft.net.test.sdk/Program.cs'])), [])
        for entry, text in [
                (('Quire.Differential.Tests.Assert', [f'{tests}/XDifferentialTests.cs']), 'may contribute only its own class'),
                (('Assert', [f'{tests}/XDifferentialTests.cs']), 'may contribute only its own class'),
                (('Quire.Differential.Tests.Reflect', [f'{tests}/Reflect.cs', f'{tests}/XDifferentialTests.cs']),
                 'may contribute only its own class'),
                (('Quire.Differential.Tests.Hidden', []), 'no source lines'),
                (('Quire.Differential.Tests.Assert', ['/root/.nuget/packages/x/Assert.cs']), 'outside the repository'),
                (('Assert', ['/elsewhere/Assert.cs']), 'outside the repository'),
                (('<XDifferentialTests>F1A2B3__Assert', [f'{tests}/XDifferentialTests.cs']), 'compiler-named type')]:
            with self.subTest(entry=entry):
                self.assertIn(text, types(entry)[0])

    def test_case_variant_and_nested_test_files(self):
        bad = 'namespace Quire;\nstatic class Assert { QUIRE_X }\n'
        self.assertIn('forbidden test API', self.differential(bad, 'Evildifferentialtests.cs'))
        (self.repo / 'tests/Quire.Differential.Tests/Evildifferentialtests.cs').unlink()
        # Protected subdirectory files are maintainer-reviewed and not linted as agent tests.
        self.assertEqual(self.differential(bad, 'Sub/EvilDifferentialTests.cs'), [])


if __name__ == '__main__':
    unittest.main()
