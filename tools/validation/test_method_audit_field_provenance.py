"""A field MirrorGen renamed for a name clash (Quire `a_` for Mathcad `a`) is the Mathcad field to the gate, when its
[MirrorName] claim holds; a claim that does not hold is an error (built assemblies; no install)."""
from pathlib import Path
import tempfile
import unittest

from method_audit_fixture import Case, build_audit

MIRROR_NAME = '''namespace Quire.Mirror
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public sealed class MirrorNameAttribute : System.Attribute { public MirrorNameAttribute(string name) { } }
}
'''
ORIGINAL = '''namespace N
{
    public class T
    {
        public int a;
        public int n;
        public static int s;
        public int ReadA() => a;
        public void WriteA(int x) { a = x; }
        public int ReadN() => n;
        public int ReadS() => s;
    }
}
'''
GOOD = '''namespace Quire.N
{
    [Quire.Mirror.MirrorOf("N.T, Orig")]
    public class T
    {
        [Quire.Mirror.MirrorName("a")] public int a_;
        public int n;
        public static int s;
        public int ReadA() => a_;
        public void WriteA(int x) { a_ = x; }
        public int ReadN() => n;
        public int ReadS() => s;
    }
}
'''
UNRENAMED = GOOD.replace('[Quire.Mirror.MirrorName("a")] ', '')  # the gate's old failure: ldfld a_ against ldfld a
DOUBLE = GOOD.replace('public int n;', '[Quire.Mirror.MirrorName("a")] public int a__; public int n;')
BAD_SHAPE = GOOD.replace('a_;', 'b_;').replace('=> a_', '=> b_').replace('{ a_ =', '{ b_ =')
WRONG_TYPE = GOOD.replace('public int a_;', 'public long a_;').replace('=> a_', '=> (int)a_').replace('{ a_ = x;', '{ a_ = x;')
WRONG_STATIC = GOOD.replace('public int a_;', 'public static int a_;')
NO_FIELD = GOOD.replace('MirrorName("a")', 'MirrorName("missing")').replace('a_', 'missing_')


class FieldProvenanceTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.temp = tempfile.TemporaryDirectory(prefix='quire-field-provenance-')
        cls.addClassCleanup(cls.temp.cleanup)
        cls.tool = build_audit(cls.temp.name)

    def audit(self, name, source):
        work = Path(self.temp.name) / name
        work.mkdir()
        case = Case(work, self.tool)
        case.original('Orig', ORIGINAL)
        case.quire('Quire.P', MIRROR_NAME + source)
        inventory = case.audit([('Orig.dll', 'N.T', 'Quire.N.T', 'missing')])
        rows = {row['signature'].split('`')[0]: row for row in inventory['methods']}
        return inventory, rows

    def test_a_renamed_field_is_the_mathcad_field(self):
        inventory, rows = self.audit('good', GOOD)
        self.assertEqual(inventory['errors'], [])
        for member in ('ReadA', 'WriteA', 'ReadN', 'ReadS'):
            self.assertTrue(rows[member]['exact_match'], rows[member])

    def test_without_the_claim_the_renamed_field_differs(self):
        inventory, rows = self.audit('unrenamed', UNRENAMED)
        self.assertEqual(inventory['errors'], [])
        self.assertFalse(rows['ReadA']['exact_match'])
        self.assertFalse(rows['WriteA']['exact_match'])
        self.assertTrue(rows['ReadN']['exact_match'])  # a field that was never renamed is unaffected

    def assert_rejected(self, name, source, text):
        inventory, rows = self.audit(name, source)
        self.assertTrue(any(text in error for error in inventory['errors']), inventory['errors'])
        self.assertFalse(rows['ReadA']['exact_match'], rows['ReadA'])
        self.assertTrue(rows['ReadN']['exact_match'])

    def test_a_double_claim_is_rejected(self):
        self.assert_rejected('double', DOUBLE, 'is claimed twice')

    def test_a_rename_that_is_not_the_name_plus_underscores_is_rejected(self):
        self.assert_rejected('shape', BAD_SHAPE, 'is not a plus underscores')

    def test_a_claim_on_a_field_of_another_type_or_access_is_rejected(self):
        self.assert_rejected('type', WRONG_TYPE, 'differs from N.T::a in static, access or type')
        self.assert_rejected('static', WRONG_STATIC, 'differs from N.T::a in static, access or type')

    def test_a_claim_naming_no_mathcad_field_is_rejected(self):
        self.assert_rejected('nofield', NO_FIELD, 'no field N.T::missing')


if __name__ == '__main__':
    unittest.main()
