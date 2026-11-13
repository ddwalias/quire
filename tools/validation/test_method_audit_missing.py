"""MethodAudit lists the Mathcad members no Quire member pairs with (built assemblies; no Mathcad install)."""
import tempfile
import unittest

from method_audit_fixture import Case, build_audit
from release_il import MISSING, compare

ORIGINAL = '''namespace N
{
    public class T
    {
        public int Same(int x) => x + 1;
        public int Differs(int x) => x + 3;
        public int Absent() => 7;
        public int Absent(string text) => text.Length;
    }
    public class Unmirrored { public void M() { } }
}
'''
QUIRE = '''namespace Quire.N
{
    [Quire.Mirror.MirrorOf("N.T, Orig")]
    public class T
    {
        public int Same(int x) => x + 1;
        public int Differs(int x) => x + 2;
        public int Extra() => 0;
    }
}
'''


class MissingMemberTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.temp = tempfile.TemporaryDirectory(prefix='quire-method-audit-test-')
        cls.addClassCleanup(cls.temp.cleanup)
        case = Case(cls.temp.name, build_audit(cls.temp.name))
        case.original('Orig', ORIGINAL)
        case.quire('Quire.P', QUIRE)
        cls.inventory = case.audit([('Orig.dll', 'N.T', 'Quire.N.T', 'missing'),
                                    ('Orig.dll', 'N.Unmirrored', 'Quire.N.Unmirrored', 'missing')])

    def test_the_fixture_pairs_and_compares(self):
        self.assertEqual(self.inventory['errors'], [])
        verdicts = {row['signature'].split('`')[0]: (row['original'] is not None, row['exact_match'])
                    for row in self.inventory['methods'] if row['type'] == 'Quire.N.T'}
        self.assertEqual(verdicts, {'Same': (True, True), 'Differs': (True, False), 'Extra': (False, None),
                                    '.ctor': (True, True)})

    def test_mathcad_members_of_a_bound_type_without_a_quire_member_are_listed(self):
        missing = self.inventory['missing']
        self.assertEqual([(m['project'], m['type'], m['original']['assembly'], m['original']['type'],
                           m['original']['signature'].split('->')[0]) for m in missing],
                         [('Quire.P', 'Quire.N.T', 'Orig', 'N.T', 'Absent`0()'),
                          ('Quire.P', 'Quire.N.T', 'Orig', 'N.T', 'Absent`0([System.Runtime]System.String)')])
        self.assertTrue(all(m['original']['token_hex'].startswith('0x06') for m in missing))
        # A Mathcad type no Quire type binds is the type map's to report, not a member list.
        self.assertFalse(any(m['original']['type'] == 'N.Unmirrored' for m in missing))

    def test_the_gate_names_them_and_a_completeness_audit_fails_on_them(self):
        result = compare(self.inventory, self.inventory)
        self.assertTrue(result['passed'])
        self.assertEqual(result['missing'], 2)
        self.assertEqual(len(result['skipped'][MISSING]), 2)
        self.assertIn('Quire.P::Quire.N.T::Absent`0()', result['skipped'][MISSING][0])
        audit = compare(self.inventory, self.inventory, everything=True)
        self.assertEqual(sorted(f['reason'] for f in audit['failures'] if 'Absent' in f['method']), [MISSING, MISSING])


if __name__ == '__main__':
    unittest.main()
