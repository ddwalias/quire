import types
import unittest
from unittest import mock

import quick_check


class QuickCheckTests(unittest.TestCase):
    def setUp(self):
        self.key = "Quire.T::M`0()->[System.Runtime]System.Int32|True"
        self.row = {"type": "Quire.T", "signature": "M`0()->[System.Runtime]System.Int32|True"}
        self.inventory = {"methods": [self.row]}
        self.baseline = {"tests": {self.key: ["mdifferentialtests"]}, "methods": {self.key: "compared"}}

    def test_il_labels_distinguish_exact_structure_only_and_mismatch(self):
        self.assertEqual(quick_check.il_label({"exact_match": True, "structure_matches": True}), "exact IL")
        self.assertEqual(quick_check.il_label({"exact_match": False, "structure_matches": True}), "structure only")
        self.assertEqual(quick_check.il_label({"exact_match": False, "structure_matches": False}), "mismatch")

    def test_class_filter_ors_only_the_selected_comparison_classes(self):
        self.assertEqual(quick_check.class_filter({"bdifferentialtests", "adifferentialtests"}),
                         "FullyQualifiedName~adifferentialtests|FullyQualifiedName~bdifferentialtests")

    def test_branch_diff_starts_at_the_fork_point_and_includes_worktree_changes(self):
        with mock.patch("quick_check.git", side_effect=["base-commit", "M\0src/T.cs\0"]) as git:
            self.assertEqual(quick_check.changes_since("main"), "M\0src/T.cs\0")
        self.assertEqual(git.call_args_list, [mock.call("merge-base", "main", "HEAD"),
                                              mock.call("diff", "-z", "--no-renames", "--name-status", "base-commit")])

    def test_explicit_type_uses_mutation_test_selection(self):
        args = types.SimpleNamespace(method=[], method_type=["Quire.T"])
        with mock.patch("quick_check.mutate.select", return_value=[(self.row, {})]) as select:
            selected, notes = quick_check.select_methods(self.inventory, self.baseline, args, "", {})
        self.assertEqual(selected, {self.key})
        self.assertEqual(notes, ["explicit method types"])
        select.assert_called_once_with(self.inventory, set(), {self.key})

    def test_default_selection_delegates_diff_logic_to_validate(self):
        args = types.SimpleNamespace(method=[], method_type=[])
        with mock.patch("quick_check.validate.changed_methods", return_value=({self.key: ["IL changed"]}, ["shared logic"])) as changed, \
                mock.patch("quick_check.mutate.select", return_value=[]):
            selected, notes = quick_check.select_methods(self.inventory, self.baseline, args, "M\0src/T.cs", {})
        changed.assert_called_once()
        self.assertEqual(selected, {self.key})
        self.assertEqual(notes, ["shared logic"])


if __name__ == "__main__":
    unittest.main()
