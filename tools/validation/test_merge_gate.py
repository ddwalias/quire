from pathlib import Path
import unittest
from merge_gate import case_collisions, patterns, protected, verdict

RULES = patterns((Path(__file__).parent / 'protected.txt').read_text())
PASSED = {'projects': [{'project': 'P', 'exit_code': 0, 'passed': 3, 'failed': 0, 'skipped': 0}],
          'ratchet': {'ok': True, 'regressions': []}, 'gate_passed': True}


class MergeGateTests(unittest.TestCase):
    def test_harness_and_owned_documents_are_protected(self):
        self.assertEqual(protected([
            'tools/validation/mutate.py', 'tools/Quire.Tools.MethodAudit/Structure.cs',
            'tests/Quire.Differential.Tests/MethodComparison.cs', 'tests/Quire.Differential.Tests/Mathcad.cs',
            'docs/mirror/validation-baseline.json', 'docs/mirror/accepted-mutants.txt', 'docs/mirror/deviations.md',
            'src/Quire.Mirror/MirrorAttributes.cs', 'Directory.Build.props', 'tests/Quire.Differential.Tests/Reflect.cs',
            'tests/Quire.Differential.Tests/NewHelper.cs', 'src/Directory.Build.targets', 'tests/x/NuGet.Config',
            'global.json', 'src/Quire.Mpl/Quire.Mpl.csproj', 'tests/a.runsettings'], RULES), sorted([
            'tools/validation/mutate.py', 'tools/Quire.Tools.MethodAudit/Structure.cs',
            'tests/Quire.Differential.Tests/MethodComparison.cs', 'tests/Quire.Differential.Tests/Mathcad.cs',
            'docs/mirror/validation-baseline.json', 'docs/mirror/accepted-mutants.txt', 'docs/mirror/deviations.md',
            'src/Quire.Mirror/MirrorAttributes.cs', 'Directory.Build.props', 'tests/Quire.Differential.Tests/Reflect.cs',
            'tests/Quire.Differential.Tests/NewHelper.cs', 'src/Directory.Build.targets', 'tests/x/NuGet.Config',
            'global.json', 'src/Quire.Mpl/Quire.Mpl.csproj', 'tests/a.runsettings']))

    def test_work_paths_are_not_protected(self):
        self.assertEqual(protected([
            'src/Quire.Mpl/Mcd/NumVal.cs', 'tests/Quire.Differential.Tests/NumValDifferentialTests.cs',
            'tests/Quire.Mpl.Tests/Mcd/NumValTests.cs', 'docs/mirror/evidence/Mcd.NumVal.md',
            'docs/mirror/il/mplrun/Mcd.NumVal.il', 'docs/mirror/WORK.md', 'docs/mirror/status/Quire.Mpl.txt'], RULES), [])

    def test_case_variants_and_subdirectories_stay_protected(self):
        self.assertEqual(protected([
            'tests/Quire.Differential.Tests/Sub/EvilDifferentialTests.cs', 'src/Quire.Mpl/Generated/Mirror/X.cs',
            'src/Quire.Mpl/Mcd/Generated/Y.cs', 'TOOLS/validation/validate.py'], RULES), sorted([
            'tests/Quire.Differential.Tests/Sub/EvilDifferentialTests.cs', 'src/Quire.Mpl/Generated/Mirror/X.cs',
            'src/Quire.Mpl/Mcd/Generated/Y.cs', 'TOOLS/validation/validate.py']))
        # Writable, so integrity.py lints every such file whatever its case.
        self.assertEqual(protected(['tests/Quire.Differential.Tests/Evildifferentialtests.cs'], RULES), [])

    def test_case_collisions_found(self):
        self.assertEqual(case_collisions(['src/Quire.Mpl/A.cs', 'Src/Quire.Engine/B.cs', 'docs/x.md']), ['Src / src'])
        self.assertEqual(case_collisions(['src/a/A.cs', 'src/a/B.cs']), [])

    def test_clean_summary_passes(self):
        self.assertEqual(verdict(PASSED), [])

    def test_every_failure_blocks(self):
        cases = [
            (dict(PASSED, error='boom'), 'validation error'),
            (dict(PASSED, projects=[dict(PASSED['projects'][0], failed=1)]), 'tests did not all pass'),
            (dict(PASSED, projects=[dict(PASSED['projects'][0], skipped=1)]), 'tests did not all pass'),
            (dict(PASSED, projects=[dict(PASSED['projects'][0], passed=0)]), 'tests did not all pass'),
            ({k: v for k, v in PASSED.items() if k != 'ratchet'}, 'ratchet did not run'),
            (dict(PASSED, ratchet={'ok': False, 'regressions': ['T: missing rose']}), 'regression: T: missing rose'),
            (dict(PASSED, mutation={'survivors': ['T::M()@IL_0004 add -> sub']}), 'surviving mutant'),
            (dict(PASSED, gate_passed=False), 'did not report gate_passed'),
        ]
        for summary, reason in cases:
            with self.subTest(reason=reason):
                self.assertTrue(any(reason in r for r in verdict(summary)), verdict(summary))


if __name__ == '__main__':
    unittest.main()
