import unittest
from lane_merge import allow_only_unreachable_protected_change


class LaneMergeTests(unittest.TestCase):
    def setUp(self):
        self.rules = ['tools/*', 'docs/mirror/*', '!docs/mirror/evidence/*']

    def test_auto_allows_only_the_unreachable_line_ledger(self):
        self.assertTrue(allow_only_unreachable_protected_change(
            ['src/Quire.Mpl/Mcd/Dim.cs', 'docs/mirror/unreachable-lines.txt'], self.rules))

    def test_other_protected_changes_still_need_a_maintainer(self):
        self.assertFalse(allow_only_unreachable_protected_change(
            ['docs/mirror/unreachable-lines.txt', 'tools/validation/validate.py'], self.rules))
        self.assertFalse(allow_only_unreachable_protected_change(
            ['docs/mirror/accepted-mutants.txt'], self.rules))

    def test_non_protected_changes_do_not_trigger_an_override(self):
        self.assertFalse(allow_only_unreachable_protected_change(
            ['src/Quire.Mpl/Mcd/Dim.cs', 'tests/Quire.Mpl.Tests/DimTests.cs'], self.rules))


if __name__ == '__main__':
    unittest.main()
