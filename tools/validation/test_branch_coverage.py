import tempfile
from pathlib import Path
import unittest
from branch_coverage import gaps, read_cobertura


def report(directory, *lines):
    path = Path(directory) / f'r{len(list(Path(directory).iterdir()))}.xml'
    body = ''.join(f'<line number="{n}" hits="{h}" branch="{"True" if c else "False"}"'
                   + (f' condition-coverage="x% ({c})"' if c else '') + ' />' for n, h, c in lines)
    path.write_text(f'<coverage><packages><package><classes><class filename="/src/T.cs"><lines>{body}</lines>'
                    '</class></classes></package></packages></coverage>')
    return path


class BranchCoverageTests(unittest.TestCase):
    def test_partial_entry_is_not_hidden_by_a_full_one(self):
        with tempfile.TemporaryDirectory() as directory:
            lines = read_cobertura([report(directory, (5, 1, '1/2')), report(directory, (5, 1, '2/2'))])
        self.assertEqual(lines[('/src/T.cs', 5)], (1, 1, 2))

    def test_every_document_of_a_method_is_checked(self):
        lines = {('/src/A.cs', 1): (1, 0, 0), ('/src/B.cs', 9): (0, 0, 0)}
        gap = gaps([dict(file='/src/A.cs', lines=[1]), dict(file='/src/B.cs', lines=[9])], lines)
        self.assertEqual(gap['uncovered_lines'], ['/src/B.cs:9'])

    def test_empty_reports_rejected(self):
        with tempfile.TemporaryDirectory() as directory:
            with self.assertRaises(ValueError):
                read_cobertura([report(directory)])


if __name__ == '__main__':
    unittest.main()
