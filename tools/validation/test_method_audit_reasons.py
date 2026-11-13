"""MethodAudit records the part that differs, and the gate's failure reason quotes it (built assemblies; no install)."""
import tempfile
import unittest

from method_audit_fixture import Case, build_audit
from release_il import compare

ORIGINAL = '''namespace N
{
    public class T
    {
        public int Same(int x) => x + 1;
        public int Constant(int x) => x + 3;
        public string Call(object o) => o.ToString();
        internal int Access() => 1;
        public int Init { get; init; }
        public virtual int ReadOnly(in int x) => x;
        public int Overload(int x) => x;
        public int Overload(string text) => text.Length;
    }
}
'''
QUIRE = '''namespace Quire.N
{
    [Quire.Mirror.MirrorOf("N.T, Orig")]
    public class T
    {
        public int Same(int x) => x + 1;
        public int Constant(int x) => x + 2;
        public string Call(object o) => o.GetHashCode().ToString();
        public int Access() => 1;
        public int Init { get; set; }
        public virtual int ReadOnly(ref int x) => x;
        public int Overload(long x) => (int)x;
        public int Extra() => 0;
    }
    public class Unbound { public void M() { } }
}
'''


class FailureDetailTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.temp = tempfile.TemporaryDirectory(prefix='quire-method-audit-test-')
        cls.addClassCleanup(cls.temp.cleanup)
        case = Case(cls.temp.name, build_audit(cls.temp.name))
        case.original('Orig', ORIGINAL)
        case.quire('Quire.P', QUIRE)
        cls.inventory = case.audit([('Orig.dll', 'N.T', 'Quire.N.T', 'missing')])
        cls.rows = {f"{row['type']}::{row['signature'].split('`')[0]}": row for row in cls.inventory['methods']}
        # Every member is new to the gate, so each one that does not match fails with its reason.
        failures = compare({'methods': []}, cls.inventory)['failures']
        by_id = {row['id']: name for name, row in cls.rows.items()}
        cls.reasons = {by_id[failure['method']]: failure['reason'] for failure in failures}

    def test_a_matching_member_records_no_difference(self):
        self.assertEqual(self.inventory['errors'], [])
        row = self.rows['Quire.N.T::Same']
        self.assertEqual((row['exact_match'], row['first_difference'], row['declaration_differences'], row['pairing_miss']),
                         (True, None, [], None))
        self.assertNotIn('Quire.N.T::Same', self.reasons)

    def test_a_differing_constant_is_shown(self):
        reason = self.reasons['Quire.N.T::Constant']
        self.assertIn('constants differ at #0: Quire i4 2, Mathcad i4 3', reason)
        self.assertRegex(reason, r'first difference at #\d+ \(Quire has \d+ entries, Mathcad \d+\): '
                                 r'Quire ldc\.i4\.2, Mathcad ldc\.i4\.3$')

    def test_a_differing_call_target_is_shown(self):
        reason = self.reasons['Quire.N.T::Call']
        self.assertIn('calls differ at #0: Quire', reason)
        self.assertIn('GetHashCode', reason)
        self.assertIn('Mathcad', reason)
        self.assertIn('ToString', reason)

    def test_a_differing_declaration_names_the_part(self):
        self.assertEqual(self.rows['Quire.N.T::Access']['declaration_differences'], ['access: Quire Public, Mathcad Internal'])
        self.assertEqual(self.reasons['Quire.N.T::Access'], 'declaration differs: access: Quire Public, Mathcad Internal')
        # An init accessor returns void modreq(IsExternalInit); a set accessor returns void.
        self.assertEqual(self.reasons['Quire.N.T::set_Init'],
                         'declaration differs: custom modifiers of the return type: Quire none, '
                         'Mathcad modreq(System.Runtime.CompilerServices.IsExternalInit)')

    def test_an_unpaired_member_shows_what_mathcad_has_under_its_name(self):
        reason = self.reasons['Quire.N.T::Overload']
        self.assertTrue(reason.startswith('no Mathcad counterpart: this member is Overload`0([System.Runtime]System.Int64)'), reason)
        self.assertIn('Orig N.T has Overload`0([System.Runtime]System.Int32)->[System.Runtime]System.Int32|False | '
                      'Overload`0([System.Runtime]System.String)', reason)
        self.assertEqual(self.reasons['Quire.N.T::Extra'], 'no Mathcad counterpart: Orig N.T has no member named Extra')
        self.assertEqual(self.reasons['Quire.N.Unbound::M'],
                         'no Mathcad counterpart: Quire.N.Unbound is not bound to a Mathcad type')
        # The parameter that kept the pair apart shows: Mathcad's is a read-only reference.
        self.assertEqual(self.reasons['Quire.N.T::ReadOnly'],
                         'no Mathcad counterpart: this member is ReadOnly`0([System.Runtime]System.Int32&)->[System.Runtime]System.Int32|False; '
                         'Orig N.T has ReadOnly`0([System.Runtime]System.Int32&in)->[System.Runtime]System.Int32|False')


if __name__ == '__main__':
    unittest.main()
