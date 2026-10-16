import json
import tempfile
import unittest
from pathlib import Path

from accept import Failure, coverage_rejections, outcomes, owners, test_owner

GET = 'Quire.Fx.T::GetX`0()->[System.Runtime]System.Int32|False'
REPORT = {
    'members': [{'id': GET, 'methods': [f'Fx::{GET}'], 'test_class': 'MTMechanicalDifferentialTests', 'tests': ['GetX_06000001']},
                {'id': 'Quire.Fx.T::P', 'methods': ['Fx::Quire.Fx.T::get_P`0()->[System.Runtime]System.Int32|False'],
                 'test_class': 'MTMechanicalDifferentialTests', 'tests': ['P_get_06000002']}],
    'files': [{'file': 'src/Fx/T.cs', 'members': [
        {'member': {'id': 'Quire.Fx.T::m_x', 'owner': GET, 'kind': 'field'}, 'first': 3, 'last': 4},
        {'member': {'id': GET, 'kind': 'method'}, 'first': 6, 'last': 7}]}],
}
TESTS = """namespace Quire.Differential.Tests;

public class MTMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetX_06000001() => Mechanical.Compare(typeof(global::Quire.Fx.T),
        0x06000001);

    [SkippableFact]
    public void Other_06000009() => Mechanical.Compare(typeof(global::Quire.Fx.T), 0x06000009);
}
"""


class OwnersTests(unittest.TestCase):
    def test_a_moved_field_belongs_to_the_member_that_moved_it(self):
        lines, _ = owners(REPORT)
        self.assertEqual(lines[('src/Fx/T.cs', 3)], GET)
        self.assertEqual(lines[('src/Fx/T.cs', 7)], GET)
        self.assertNotIn(('src/Fx/T.cs', 5), lines)

    def test_tests_map_to_their_member(self):
        _, tests = owners(REPORT)
        self.assertEqual(tests['Quire.Differential.Tests.MTMechanicalDifferentialTests.P_get_06000002'], 'Quire.Fx.T::P')

    def test_a_test_file_line_belongs_to_the_fact_above_it(self):
        _, tests = owners(REPORT)
        with tempfile.TemporaryDirectory() as root:
            path = Path(root, 'tests/Quire.Differential.Tests/MTMechanicalDifferentialTests.cs')
            path.parent.mkdir(parents=True)
            path.write_text(TESTS)
            relative = 'tests/Quire.Differential.Tests/MTMechanicalDifferentialTests.cs'
            self.assertEqual(test_owner(relative, 7, tests, Path(root)), GET)
            self.assertIsNone(test_owner(relative, 10, tests, Path(root)))   # not a generated fact
            self.assertIsNone(test_owner(relative, 2, tests, Path(root)))    # above every fact


class OutcomesTests(unittest.TestCase):
    TRX = ('<TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010"><Results>{}</Results></TestRun>')

    def write(self, root, *results):
        path = Path(root, 'tests.trx')
        path.write_text(self.TRX.format(''.join(f'<UnitTestResult testName="{n}" outcome="{o}" />' for n, o in results)))
        return path

    def test_every_generated_test_must_have_run(self):
        _, tests = owners(REPORT)
        names = sorted(tests)
        with tempfile.TemporaryDirectory() as root:
            with self.assertRaises(Failure):
                outcomes(Path(root, 'missing.trx'), tests)
            with self.assertRaises(Failure):
                outcomes(self.write(root, (names[0], 'Passed')), tests)
            results = outcomes(self.write(root, (names[0], 'Passed'), (names[1], 'Failed')), tests)
            self.assertEqual(results[names[1]], 'Failed')


class CoverageTests(unittest.TestCase):
    def test_members_not_compared_or_with_survivors_are_rejected(self):
        with tempfile.TemporaryDirectory() as root:
            Path(root, 'method-coverage.json').write_text(json.dumps({'methods': [
                {'type': 'Quire.Fx.T', 'signature': GET.split('::')[1], 'stage': 'compared'},
                {'type': 'Quire.Fx.T', 'signature': 'get_P`0()->[System.Runtime]System.Int32|False', 'stage': 'uncovered',
                 'reason': 'lines unexecuted'},
                {'type': 'Quire.Fx.T', 'signature': 'Other`0()->[System.Runtime]System.Void|False', 'stage': 'uncovered'}]}))
            Path(root, 'summary.json').write_text(json.dumps({'mutation': {'survivors': [f'{GET}@IL_0004 and -> or']}}))
            rejected = coverage_rejections(Path(root), REPORT)
        self.assertEqual(rejected, {'Quire.Fx.T::P': 'coverage: uncovered: lines unexecuted',
                                    GET: 'mutation: survivor IL_0004 and -> or'})


if __name__ == '__main__':
    unittest.main()
