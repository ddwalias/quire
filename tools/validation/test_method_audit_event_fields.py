"""A Quire auto-event's compiler-generated backing field is the Mathcad field its original accessors operate on, with no
claim written; accessors on different fields, or two events on one field, are errors (built assemblies; no install)."""
from pathlib import Path
import tempfile
import unittest

from method_audit_fixture import Case, build_audit

TEMPLATE = '''namespace N
{
    public class T
    {
        {declare}
        public event System.Action ValueChanged
        {
            add
            {
                System.Action current = {add};
                System.Action seen;
                do
                {
                    seen = current;
                    System.Action combined = (System.Action)System.Delegate.Combine(seen, value);
                    current = System.Threading.Interlocked.CompareExchange(ref {add}, combined, seen);
                }
                while ((object)current != (object)seen);
            }
            remove
            {
                System.Action current = {remove};
                System.Action seen;
                do
                {
                    seen = current;
                    System.Action removed = (System.Action)System.Delegate.Remove(seen, value);
                    current = System.Threading.Interlocked.CompareExchange(ref {remove}, removed, seen);
                }
                while ((object)current != (object)seen);
            }
        }
        public void Fire() { {add}?.Invoke(); }
        public int Plain;
    }
}
'''
QUIRE = '''namespace Quire.N
{
    [Quire.Mirror.MirrorOf("N.T, Orig")]
    public class T
    {
        public event System.Action ValueChanged;
        public void Fire() { ValueChanged?.Invoke(); }
        public int Plain;
    }
}
'''
def obfuscated(add, remove, extra=''):
    declare = ' '.join(f'private System.Action {name};' for name in dict.fromkeys([add, remove]))
    return TEMPLATE.replace('{declare}', declare).replace('{add}', add).replace('{remove}', remove).replace('public int Plain;', 'public int Plain;' + extra)


# Two Quire events whose originals both operate on field b.
TWO_ORIGINAL = obfuscated('b', 'b', ' public event System.Action Other { add { b += value; } remove { b -= value; } }')
TWO_QUIRE = QUIRE.replace('public int Plain;', 'public int Plain; public event System.Action Other;')


class EventFieldTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.temp = tempfile.TemporaryDirectory(prefix='quire-event-fields-')
        cls.addClassCleanup(cls.temp.cleanup)
        cls.tool = build_audit(cls.temp.name)

    def audit(self, name, original, quire):
        work = Path(self.temp.name) / name
        work.mkdir()
        case = Case(work, self.tool)
        case.original('Orig', original)
        case.quire('Quire.P', quire)
        inventory = case.audit([('Orig.dll', 'N.T', 'Quire.N.T', 'missing')])
        return inventory, {row['signature'].split('`')[0]: row for row in inventory['methods']}

    def test_an_obfuscated_backing_field_is_the_events_mathcad_field(self):
        inventory, rows = self.audit('good', obfuscated('b', 'b'), QUIRE)
        self.assertEqual(inventory['errors'], [])
        for member in ('add_ValueChanged', 'remove_ValueChanged', 'Fire'):
            self.assertTrue(rows[member]['exact_match'], rows[member])

    def test_accessors_on_different_fields_are_rejected(self):
        inventory, rows = self.audit('split', obfuscated('b', 'c'), QUIRE)
        self.assertTrue(any('add accessor of N.T::ValueChanged uses [b] and the remove accessor [c]' in e
                            for e in inventory['errors']), inventory['errors'])
        self.assertFalse(rows['add_ValueChanged']['exact_match'])
        self.assertFalse(rows['Fire']['exact_match'])

    def test_two_events_on_one_original_field_are_rejected(self):
        inventory, rows = self.audit('shared', TWO_ORIGINAL, TWO_QUIRE)
        self.assertTrue(any('is also the target of another field rename' in e for e in inventory['errors']), inventory['errors'])

    def test_a_field_that_is_not_an_event_backing_field_is_unaffected(self):
        inventory, rows = self.audit('plain', obfuscated('b', 'b'), QUIRE)
        self.assertEqual(inventory['errors'], [])


if __name__ == '__main__':
    unittest.main()
