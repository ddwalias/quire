import unittest
from accept import il_rejection, owners


class IlRejectionTests(unittest.TestCase):
    def test_a_member_is_kept_only_when_its_instructions_and_declaration_match(self):
        row = {'exact_match': True, 'shape_matches': True, 'structure_differences': [], 'declaration_differences': []}
        self.assertIsNone(il_rejection(row))
        self.assertEqual(il_rejection(None), 'absent from the inventory')
        self.assertEqual(il_rejection({**row, 'exact_match': None}), 'no Mathcad counterpart')
        self.assertEqual(il_rejection({**row, 'exact_match': False}), 'opcode sequence differs')
        self.assertEqual(il_rejection({**row, 'shape_matches': False,
                                       'declaration_differences': ['access: Quire Public, Mathcad Internal']}),
                         'declaration differs: access: Quire Public, Mathcad Internal')


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
