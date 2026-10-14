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

    def test_generated_stubs_skipped_and_allow_marker_respected(self):
        self.assertEqual(self.source('int num2 = 0;', 'Quire.Mpl/Generated/Stub.g.cs'), [])
        self.assertEqual(self.source('int num2 = 0; // quire-lint: allow Mathcad local name'), [])

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
