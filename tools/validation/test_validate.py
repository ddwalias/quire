import tempfile
from pathlib import Path
import unittest
from validate import read_trx


class ResultTests(unittest.TestCase):
    def check(self, outcomes, summary="Completed", allow=False):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "result.trx"
            path.write_text('<TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">'
                            '<Results>' + ''.join(f'<UnitTestResult outcome="{o}" />' for o in outcomes)
                            + f'</Results><ResultSummary outcome="{summary}" /></TestRun>')
            return read_trx(path, allow)

    def test_pass(self):
        self.assertEqual(self.check(["Passed", "Passed"]), {"passed": 2, "failed": 0, "skipped": 0})

    def test_false_green_runs_rejected(self):
        for outcomes, summary in [([], "Completed"), (["NotExecuted"], "Completed"),
                                  (["Passed", "Failed"], "Failed"), (["Passed"], "Aborted"),
                                  (["Passed", "NotExecuted"], "Completed"), (["Passed", "Timeout"], "Completed")]:
            with self.subTest(outcomes=outcomes, summary=summary), self.assertRaises(ValueError):
                self.check(outcomes, summary)

    def test_allowed_skips_are_counted_but_all_skipped_rejected(self):
        self.assertEqual(self.check(["Passed", "NotExecuted"], allow=True)["skipped"], 1)
        with self.assertRaises(ValueError):
            self.check(["NotExecuted"], allow=True)



class ChangedTypeTests(unittest.TestCase):
    def setUp(self):
        from validate import ROOT
        root = ROOT.resolve().as_posix()
        self.coverage = {'methods': [
            dict(type='Quire.Mcd.Dim', source=[dict(file=f'{root}/src/Quire.Mpl/Mcd/Dim.cs', lines=[1])], cases=[]),
            dict(type='Quire.Mcd.NumVal', source=[dict(file=f'{root}/src/Quire.Mpl/Mcd/NumVal.cs', lines=[1])],
                 cases=[dict(test='Quire.Differential.Tests.NumValDifferentialTests.Compare')])]}

    def types(self, *names):
        from validate import changed_types
        return changed_types(self.coverage, 'base', lambda *args: '\0'.join(names) + '\0')

    def test_changed_source_and_changed_tests_select_types(self):
        self.assertEqual(self.types('src/Quire.Mpl/Mcd/Dim.cs'), {'Quire.Mcd.Dim'})
        self.assertEqual(self.types('tests/Quire.Differential.Tests/NumValDifferentialTests.cs'), {'Quire.Mcd.NumVal'})
        self.assertEqual(self.types('docs/mirror/WORK.md'), set())

    def test_nested_and_case_variant_test_classes_map_to_their_file(self):
        self.coverage['methods'][1]['cases'] = [dict(test='Quire.Differential.Tests.NumValDifferentialTests+More.Compare')]
        self.assertEqual(self.types('tests/Quire.Differential.Tests/numvaldifferentialtests.cs'), {'Quire.Mcd.NumVal'})

    def test_source_without_methods_selects_nothing(self):
        self.assertEqual(self.types('src/Quire.Mpl/Mcd/Kinds.cs'), set())

    def test_source_outside_the_repository_fails(self):
        self.coverage['methods'][0]['source'][0]['file'] = '/elsewhere/Dim.cs'
        with self.assertRaises(ValueError):
            self.types('src/Quire.Mpl/Mcd/Dim.cs')


if __name__ == "__main__":
    unittest.main()
