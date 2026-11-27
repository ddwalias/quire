import unittest

import lint


def rules(source):
    return [(rule, line) for _, rule, line in lint.violations(source)]


class LintTests(unittest.TestCase):
    def test_flags_numbered_locals(self):
        for line in ('int num2 = 0;', 'string text3;', 'var list2 = new List<int>();', 'List<string> value2;',
                     'foreach (Item item2 in items)', 'if (!dict.TryGetValue(k, out var result2))',
                     'for (int count2 = 0; count2 < n; count2++)'):
            self.assertEqual([('ilspy-name', line)], rules(line), line)

    def test_ignores_non_locals_and_good_names(self):
        for line in ('int index = 0;', 'public int num2;', 'private string text2;', 'void M(int num2, string text2)',
                     'var pair = (Item1: 1, Item2: 2);', 'x = num2 + 1;', 'int num = 0;', 'int i2 = 0;',
                     'string s = "int num2 = 0;";', '// int num2 = 0;', 'int num2 = 0; // lint-ok: required'):
            self.assertEqual([], rules(line), line)

    def test_goto_needs_comment(self):
        self.assertEqual([('goto', 'goto IL_0010;')], rules('goto IL_0010;'))
        self.assertEqual([('goto', 'IL_0010:')], rules('IL_0010:'))
        self.assertEqual([], rules('goto IL_0010; // the IL needs the shared tail'))
        self.assertEqual([], rules('// the IL needs the shared tail\nIL_0010:'))
        self.assertEqual([], rules('case 1:\ndefault:\nelse:'))

    def test_block_comment_is_skipped(self):
        self.assertEqual([], rules('/*\nint num2 = 0;\ngoto x;\n*/'))

    def test_generated_and_non_src_are_not_checked(self):
        self.assertTrue(lint.checked('src/Quire.Efi/Mirrored/X.cs'))
        self.assertFalse(lint.checked('src/Quire.Efi/Generated/X.g.cs'))
        self.assertFalse(lint.checked('tools/x/X.cs'))
        self.assertFalse(lint.checked('src/Quire.Efi/X.cpp'))


if __name__ == '__main__':
    unittest.main()
