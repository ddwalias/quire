import unittest
from accept import owners


class OwnersTests(unittest.TestCase):
    def test_compiler_errors_in_moved_fields_are_attributed_to_the_owning_method(self):
        report = {'files': [{'file': 'src/P/T.cs', 'members': [
            {'member': {'id': 'field', 'owner': 'method'}, 'first': 3, 'last': 4},
            {'member': {'id': 'method'}, 'first': 6, 'last': 7}]}]}
        lines = owners(report)
        self.assertEqual(lines[('src/P/T.cs', 3)], 'method')
        self.assertEqual(lines[('src/P/T.cs', 7)], 'method')
        self.assertNotIn(('src/P/T.cs', 5), lines)


if __name__ == '__main__':
    unittest.main()
