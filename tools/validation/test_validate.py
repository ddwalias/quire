import tempfile
from pathlib import Path
import unittest
import unittest.mock
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


class SealTreeTests(unittest.TestCase):
    def test_records_added_nested_or_deleted_at_any_depth_detected(self):
        from validate import Seal
        def plant(root, name):
            (root / name).parent.mkdir(parents=True, exist_ok=True)
            (root / name).write_text('planted')
        for change in (lambda root: (root / 'evidence/sub/A.md').write_text('forged'),
                       lambda root: (root / 'evidence/sub/A.md').unlink(),
                       lambda root: plant(root, 'evidence/new/deep/B.txt'),
                       lambda root: plant(root, 'manifest/C.json')):
            with self.subTest(change=change), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                plant(root, 'evidence/sub/A.md')
                sealed = Seal(root, [], [root / 'evidence', root / 'manifest'])
                change(root)
                with self.assertRaises(ValueError):
                    sealed.verify('run')


class SdkDirectoryTests(unittest.TestCase):
    def test_each_command_gets_a_new_directory_holding_the_global_json_read_before_tests(self):
        from validate import sdk_directory
        with sdk_directory(b'{"sdk": {"version": "10.0.100"}}\r\n') as first:
            self.assertEqual((first / 'global.json').read_bytes(), b'{"sdk": {"version": "10.0.100"}}\r\n')
            (first / 'global.json').write_text('{"sdk": {"paths": ["/fake"]}}')  # rewritten by a lingering process
        self.assertFalse(first.exists())
        with sdk_directory(b'{}\n') as second:
            self.assertNotEqual(second, first)
            self.assertEqual((second / 'global.json').read_bytes(), b'{}\n')
        self.assertFalse(second.exists())


class DotnetTests(unittest.TestCase):
    def test_cli_resolved_to_an_absolute_path_outside_the_repository(self):
        import os, stat, sys
        from validate import resolve_dotnet
        with tempfile.TemporaryDirectory() as repo, tempfile.TemporaryDirectory() as tools:
            for directory in (repo, tools):
                cli = Path(directory) / 'dotnet'
                cli.write_text('#!/bin/sh\n')
                cli.chmod(cli.stat().st_mode | stat.S_IEXEC)
            self.assertEqual(resolve_dotnet(str(Path(tools) / 'dotnet'), repo), str((Path(tools) / 'dotnet').resolve()))
            with unittest.mock.patch.dict(os.environ, PATH=tools):
                self.assertEqual(resolve_dotnet('dotnet', repo), str((Path(tools) / 'dotnet').resolve()))
            with unittest.mock.patch.dict(os.environ, PATH=os.pathsep.join([repo, tools])), \
                    self.assertRaisesRegex(ValueError, 'inside the repository'):
                resolve_dotnet('dotnet', repo)
            with self.assertRaisesRegex(ValueError, 'not found'):
                resolve_dotnet(str(Path(tools) / 'missing'), repo)


class MissingShapeTests(unittest.TestCase):
    """Every line of the maintainer-owned manifest must still match the build; added lines are fine."""
    def test_missing_and_changed_lines_are_reported(self):
        import subprocess
        import validate
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / 'docs/mirror/manifest').mkdir(parents=True)
            (root / 'docs/mirror/manifest/Quire.Mpl.txt').write_text(
                '# header\nT Quire.Mcd.T class public :  | \n  Quire.Mcd.T F private System.UInt32 m_kept\n'
                '  Quire.Mcd.T F private System.UInt32 m_gone\n  Quire.Mcd.T M public virtual Run`0() -> System.Void\n')
            built = ('T Quire.Mcd.T class public :  | \n  Quire.Mcd.T F private System.UInt32 m_kept\n'
                     '  Quire.Mcd.T M public Run`0() -> System.Void\n  Quire.Mcd.T M private Helper`0() -> System.Void\n')
            with unittest.mock.patch.object(validate, 'ROOT', root), unittest.mock.patch.object(
                    validate.subprocess, 'run', return_value=subprocess.CompletedProcess([], 0, built, '')):
                self.assertEqual(validate.missing_shape('dotnet', root, b'{}', root / 'Quire.Mpl.dll', 10),
                                 ['Quire.Mcd.T F private System.UInt32 m_gone', 'Quire.Mcd.T M public virtual Run`0() -> System.Void'])


class PrivateCopyTests(unittest.TestCase):
    def test_gate_runs_never_use_the_mutant_cache(self):
        from types import SimpleNamespace
        from validate import mutant_cache_allowed
        local = dict(no_mutant_cache=False, mutate_since=None, write_baseline=None)
        self.assertTrue(mutant_cache_allowed(SimpleNamespace(**local)))
        for gate in (dict(no_mutant_cache=True), dict(mutate_since='main'), dict(write_baseline=Path('b.json'))):
            with self.subTest(gate=gate):
                self.assertFalse(mutant_cache_allowed(SimpleNamespace(**dict(local, **gate))))

    def test_gate_runs_refuse_proposals(self):
        import os, subprocess, sys
        with tempfile.TemporaryDirectory() as directory:
            proposals = Path(directory) / "proposed.txt"
            proposals.write_text("")
            for flag in ("--proposed-mutants", "--proposed-unreachable"):
                for gate in (["--mutate-since", "main"], ["--write-baseline", str(Path(directory) / "b.json")]):
                    with self.subTest(flag=flag, gate=gate):
                        result = subprocess.run([sys.executable, "-I", "-B", str(Path(__file__).parent / "validate.py"),
                                                 flag, str(proposals), *gate],
                                                env=dict(os.environ, QUIRE_VALIDATOR_REPO=directory),
                                                capture_output=True, text=True)
                        self.assertEqual(result.returncode, 2)
                        self.assertIn(f"{flag} is for local runs", result.stderr)

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
