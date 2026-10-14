import os
import stat
import sys
import tempfile
from pathlib import Path
import unittest
from unittest import mock
from mutate import key, run, select

FAKE_DOTNET = """#!{python}
import os, sys
from pathlib import Path
args = sys.argv
image = (Path(args[2]).parent / 'Quire.Mpl.dll').read_bytes()
scratch = Path(args[args.index('--results-directory') + 1])
kills = Path(os.environ['QUIRE_MUTATION_KILLS'])
TEST = 'Ns.Tests.Compare'
if 'FAKE_DOTNET_LOG' in os.environ:
    with open(os.environ['FAKE_DOTNET_LOG'], 'a') as log:
        log.write(os.getcwd() + '|' + Path('global.json').read_text() + '\\n')

def finish(failed, *recorded):
    if recorded:
        kills.write_text(''.join(f'{{TEST}}|{{r}}\\n' for r in recorded))
    outcome = 'Failed' if failed else 'Passed'
    (scratch / 'run.trx').write_text(
        '<TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010"><TestDefinitions>'
        '<UnitTest id="1"><TestMethod className="Ns.Tests" name="Compare"/></UnitTest></TestDefinitions>'
        f'<Results><UnitTestResult testId="1" outcome="{{outcome}}"/></Results></TestRun>')
    sys.exit(1 if failed else 0)

if image[0] == 0xFF:
    finish(True)  # a broken suite: even the control run fails
if args[args.index('--filter') + 1] == 'FullyQualifiedName=Ns.Tests.Alone':
    finish(True, 'mvid:100663299')  # passes after its siblings, mismatches when run alone
if image[0] == 0xFD:
    (Path(args[2]).parent / 'planted.dll').write_bytes(b'x')  # a test rewrites the output later runs load
if image[0] == 0xFE:
    finish(False, 'mvid:100663297')  # a test swallows an always-mismatching comparison
if image[4] == 0x59:  # add -> sub: the method's own comparison mismatches and the harness records it
    finish(True, 'mvid:100663297')
if image[6] == 0x5B:  # mul -> div: another method compared first mismatches; also one in a foreign assembly
    finish(True, 'other:100663297', 'mvid:100663298')
if image[8] == 0x2D and image[9] == 1:  # only a foreign assembly's comparison mismatched
    finish(True, 'other:100663297')
if image[8] == 0x2D and image[9] == 2:  # the mismatch is swallowed; the test fails for another reason
    kills.write_text(f'{{TEST}}|mvid:100663297\\n')
    sys.exit(1)
# brfalse -> brtrue at offset 8: the test fails, but no comparison reported a mismatch
finish(image[8] == 0x2D)
"""


class MutationTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()
        self.addCleanup(self.directory.cleanup)
        root = Path(self.directory.name)
        self.tests = root / 'bin'
        self.tests.mkdir()
        self.image = bytes([0, 0, 0, 0, 0x58, 0, 0x5A, 0, 0x2C, 0])
        (self.tests / 'Quire.Mpl.dll').write_bytes(self.image)
        (self.tests / 'Quire.Differential.Tests.dll').write_bytes(b'')
        self.dotnet = root / 'dotnet'
        self.dotnet.write_text(FAKE_DOTNET.format(python=sys.executable))
        self.dotnet.chmod(self.dotnet.stat().st_mode | stat.S_IEXEC)
        killed = dict(il_offset=1, file_offset=4, original='58', replacement='59', description='add -> sub')
        survived = dict(il_offset=5, file_offset=8, original='2C', replacement='2D', description='brfalse.s -> brtrue.s')
        caller = dict(il_offset=3, file_offset=6, original='5A', replacement='5B', description='mul -> div')
        self.method = dict(type='Quire.T', signature='M()', validation='compared', mutants=[killed, survived, caller],
                           cases=[dict(test='Ns.Tests.Compare')], quire_mvid='mvid', token=0x06000001)
        self.coverage = {'methods': [self.method, dict(self.method, type='Quire.Other'),
                                     dict(self.method, signature='Stub()', validation='missing')]}

    def mutate(self, accepted=frozenset(), jobs=1):
        return run(self.coverage, {'Quire.T'}, self.tests / 'Quire.Differential.Tests.dll', 'Quire.Mpl',
                   str(self.dotnet), jobs=jobs, accepted_keys=accepted)

    def test_only_compared_methods_of_selected_types(self):
        self.assertEqual(len(select(self.coverage, {'Quire.T'})), 3)

    def test_compiler_generated_methods_use_their_compared_owner_tests(self):
        lambda_ = dict(self.method, type='Quire.T+<>c', signature='<M>b__0_0()', validation='excluded', cases=[],
                       owner=dict(type='Quire.T', name='M'), token=0x06000005)
        orphan = dict(lambda_, signature='<Stub>b__1_0()', owner=dict(type='Quire.T', name='Stub'))
        self.coverage['methods'] += [lambda_, orphan]
        chosen = [(m['signature'], m['cases']) for m, _ in select(self.coverage, {'Quire.T'})]
        self.assertIn(('<M>b__0_0()', [dict(test='Ns.Tests.Compare')]), chosen)
        self.assertNotIn('<Stub>b__1_0()', [name for name, _ in chosen])  # its owner is not compared

    def test_survivor_fails_until_accepted(self):
        for jobs in (1, 2):
            with self.subTest(jobs=jobs):
                report = self.mutate(jobs=jobs)
                self.assertEqual((report['total'], report['killed']), (3, 2))
                self.assertEqual(report['survivors'], ['Quire.T::M()@IL_0005 brfalse.s -> brtrue.s'])
                self.assertFalse(report['complete'])
        survivor = key(self.method, self.method['mutants'][1])
        self.assertTrue(self.mutate({survivor})['complete'])

    def test_only_a_recorded_mismatch_kills(self):
        result = {r['key']: r['outcome'] for r in self.mutate()['mutants']}
        self.assertEqual(result['Quire.T::M()@IL_0005 brfalse.s -> brtrue.s'], 'survived (tests failed without a mismatch)')
        self.assertEqual(result['Quire.T::M()@IL_0001 add -> sub'], 'killed')
        self.assertEqual(result['Quire.T::M()@IL_0003 mul -> div'], 'killed (via another method compared first)')

    def test_mismatch_in_another_assembly_or_a_passing_test_is_not_a_kill(self):
        for marker in (1, 2):
            with self.subTest(marker=marker):
                image = bytearray(self.image); image[9] = marker
                (self.tests / 'Quire.Mpl.dll').write_bytes(bytes(image))
                result = {r['key']: r['outcome'] for r in self.mutate()['mutants']}
                self.assertEqual(result['Quire.T::M()@IL_0005 brfalse.s -> brtrue.s'],
                                 'survived (tests failed without a mismatch)')

    def test_control_run_with_a_swallowed_mismatch_rejected(self):
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes([0xFE]) + self.image[1:])
        with self.assertRaisesRegex(ValueError, 'swallowed'):
            self.mutate()

    def test_each_test_subset_needs_a_passing_control(self):
        alone = dict(self.method, signature='N()', token=0x06000003, cases=[dict(test='Ns.Tests.Alone')])
        self.coverage['methods'].append(alone)
        with self.assertRaisesRegex(ValueError, "control run of \\['Ns.Tests.Alone'\\] failed"):
            self.mutate()

    def test_failed_subset_control_stops_every_worker(self):
        alone = dict(self.method, signature='N()', token=0x06000003, cases=[dict(test='Ns.Tests.Alone')])
        self.coverage['methods'].append(alone)
        with self.assertRaisesRegex(ValueError, 'control run'):
            run(self.coverage, {'Quire.T'}, self.tests / 'Quire.Differential.Tests.dll', 'Quire.Mpl', str(self.dotnet),
                jobs=3, accepted_keys=frozenset())

    def test_tests_rewriting_their_output_rejected(self):
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes([0xFD]) + self.image[1:])
        with self.assertRaisesRegex(ValueError, 'changed their own build output'):
            self.mutate()

    def test_failing_control_run_rejected(self):
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes([0xFF]) + self.image[1:])
        with self.assertRaises(ValueError):
            self.mutate()

    def test_each_run_gets_a_new_directory_holding_the_global_json(self):
        log = Path(self.directory.name) / 'runs.log'
        with mock.patch.dict(os.environ, FAKE_DOTNET_LOG=str(log)):
            run(self.coverage, {'Quire.T'}, self.tests / 'Quire.Differential.Tests.dll', 'Quire.Mpl', str(self.dotnet),
                accepted_keys=frozenset(), global_json=b'{"sdk": {}}')
        runs = [line.split('|') for line in log.read_text().splitlines()]
        self.assertEqual(len(runs), 5)  # two controls and three mutants
        self.assertEqual(len({cwd for cwd, _ in runs}), 5)
        self.assertEqual({content for _, content in runs}, {'{"sdk": {}}'})
        self.assertFalse([cwd for cwd, _ in runs if Path(cwd).exists()])

    def test_original_assembly_left_unchanged(self):
        self.mutate()
        self.assertEqual((self.tests / 'Quire.Mpl.dll').read_bytes(), self.image)

    def test_stale_inventory_rejected(self):
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes(10))
        with self.assertRaises(ValueError):
            self.mutate()


if __name__ == '__main__':
    unittest.main()
