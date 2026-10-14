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


if __name__ == "__main__":
    unittest.main()
