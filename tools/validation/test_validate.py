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

    def types(self, *names, status='M'):
        from validate import changed_types
        return changed_types(self.coverage, ''.join(f'{status}\0{n}\0' for n in names))

    def test_changed_source_and_changed_tests_select_types(self):
        self.assertEqual(self.types('src/Quire.Mpl/Mcd/Dim.cs'), {'Quire.Mcd.Dim'})
        self.assertEqual(self.types('tests/Quire.Differential.Tests/NumValDifferentialTests.cs'), {'Quire.Mcd.NumVal'})
        self.assertEqual(self.types('docs/mirror/WORK.md'), set())

    def test_nested_and_case_variant_test_classes_map_to_their_file(self):
        self.coverage['methods'][1]['cases'] = [dict(test='Quire.Differential.Tests.NumValDifferentialTests+More.Compare')]
        self.assertEqual(self.types('tests/Quire.Differential.Tests/numvaldifferentialtests.cs'), {'Quire.Mcd.NumVal'})

    def test_cases_removed_from_a_changed_file_still_select_their_type(self):
        from validate import changed_types
        previous = {'Quire.Mcd.Dim::Pwr()': ['dimdifferentialtests'], 'Quire.Gone::X()': ['dimdifferentialtests']}
        for status in 'MD':  # cases cut from the file, or the whole file deleted
            names = f'{status}\0tests/Quire.Differential.Tests/DimDifferentialTests.cs\0'
            self.assertEqual(changed_types(self.coverage, names, previous), {'Quire.Mcd.Dim'})
        self.assertEqual(self.types('src/Quire.Mpl/Mcd/Dim.cs', status='D'), set())

    def test_source_without_methods_selects_nothing(self):
        self.assertEqual(self.types('src/Quire.Mpl/Mcd/Kinds.cs'), set())

    def test_source_outside_the_repository_fails(self):
        self.coverage['methods'][0]['source'][0]['file'] = '/elsewhere/Dim.cs'
        with self.assertRaises(ValueError):
            self.types('src/Quire.Mpl/Mcd/Dim.cs')


class SealTests(unittest.TestCase):
    def test_changes_to_late_read_files_or_outputs_detected(self):
        from validate import Seal
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / 'docs').mkdir()
            (root / 'docs/type-map.csv').write_text('map')
            (root / 'bin').mkdir()
            (root / 'bin/Harness.dll').write_bytes(b'a')
            (root / 'src.cs').write_text('read before tests; not sealed')
            def seal():
                return Seal(root, [root / 'docs/type-map.csv', root / 'docs/absent.md'], [root / 'bin'])
            unchanged = seal()
            (root / 'src.cs').write_text('changed')
            unchanged.verify('run')
            for change in (lambda: (root / 'docs/type-map.csv').write_text('forged'),
                           lambda: (root / 'docs/absent.md').write_text('planted'),
                           lambda: (root / 'bin/Harness.dll').write_bytes(b'b'),
                           lambda: (root / 'bin/planted.dll').write_bytes(b'c')):
                sealed = seal()
                change()
                with self.subTest(change=change), self.assertRaises(ValueError):
                    sealed.verify('run')


class PrivateCopyTests(unittest.TestCase):
    def test_validator_runs_isolated_from_a_copy(self):
        import os, shutil, subprocess, sys
        with tempfile.TemporaryDirectory() as directory:
            copy = Path(directory) / "validation"
            shutil.copytree(Path(__file__).parent, copy, ignore=shutil.ignore_patterns("__pycache__"))
            result = subprocess.run([sys.executable, "-I", "-B", str(copy / "validate.py"), "--help"],
                                    env=dict(os.environ, QUIRE_VALIDATOR_REPO=directory), capture_output=True, text=True)
            self.assertEqual(result.returncode, 0, result.stderr)
            self.assertFalse(list(copy.rglob("*.pyc")))


if __name__ == "__main__":
    unittest.main()
