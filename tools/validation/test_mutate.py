import os
import stat
import sys
import tempfile
from pathlib import Path
import unittest
from mutate import key, run, select

FAKE_DOTNET = """#!{python}
import sys
from pathlib import Path
image = (Path(sys.argv[2]).parent / 'Quire.Mpl.dll').read_bytes()
# The fake suite notices only the mutant at offset 4 (add -> sub); offset 8 is never checked.
sys.exit(1 if image[4] == 0x59 else 0)
"""


class MutationTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()
        self.addCleanup(self.directory.cleanup)
        root = Path(self.directory.name)
        self.tests = root / 'bin'
        self.tests.mkdir()
        self.image = bytes([0, 0, 0, 0, 0x58, 0, 0, 0, 0x2C, 0])
        (self.tests / 'Quire.Mpl.dll').write_bytes(self.image)
        (self.tests / 'Quire.Differential.Tests.dll').write_bytes(b'')
        self.dotnet = root / 'dotnet'
        self.dotnet.write_text(FAKE_DOTNET.format(python=sys.executable))
        self.dotnet.chmod(self.dotnet.stat().st_mode | stat.S_IEXEC)
        killed = dict(il_offset=1, file_offset=4, original='58', replacement='59', description='add -> sub')
        survived = dict(il_offset=5, file_offset=8, original='2C', replacement='2D', description='brfalse.s -> brtrue.s')
        self.method = dict(type='Quire.T', signature='M()', validation='compared', mutants=[killed, survived],
                           cases=[dict(test='Ns.Tests.Compare')])
        self.coverage = {'methods': [self.method, dict(self.method, type='Quire.Other'),
                                     dict(self.method, signature='Stub()', validation='missing')]}

    def mutate(self, accepted=frozenset(), jobs=1):
        return run(self.coverage, {'Quire.T'}, self.tests / 'Quire.Differential.Tests.dll', 'Quire.Mpl',
                   str(self.dotnet), jobs=jobs, accepted_keys=accepted)

    def test_only_compared_methods_of_selected_types(self):
        self.assertEqual(len(select(self.coverage, {'Quire.T'})), 2)

    def test_survivor_fails_until_accepted(self):
        for jobs in (1, 2):
            with self.subTest(jobs=jobs):
                report = self.mutate(jobs=jobs)
                self.assertEqual((report['total'], report['killed']), (2, 1))
                self.assertEqual(report['survivors'], ['Quire.T::M()@IL_0005 brfalse.s -> brtrue.s'])
                self.assertFalse(report['complete'])
        survivor = key(self.method, self.method['mutants'][1])
        self.assertTrue(self.mutate({survivor})['complete'])

    def test_original_assembly_left_unchanged(self):
        self.mutate()
        self.assertEqual((self.tests / 'Quire.Mpl.dll').read_bytes(), self.image)

    def test_stale_inventory_rejected(self):
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes(10))
        with self.assertRaises(ValueError):
            self.mutate()


if __name__ == '__main__':
    unittest.main()
