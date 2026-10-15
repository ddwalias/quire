import os
import stat
import sys
import tempfile
import time
from pathlib import Path
import unittest
from unittest import mock
from mutate import cache_directory, key, reasons, run, select

FAKE_DOTNET = """#!{python}
import os, sys
from pathlib import Path
args = sys.argv
if args[1] in ('--version', '--list-runtimes'):
    print(os.environ.get('FAKE_DOTNET_RUNTIME', 'Microsoft.NETCore.App 10.0.0'))
    sys.exit(0)
if 'QUIRE_MUTANT_CACHE' in os.environ:
    sys.exit(7)  # code under test must not learn where the cache lives
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
if image[8] == 0x2D and image[9] == 3:  # the mutant loops forever
    import time; time.sleep(120)
if image[8] == 0x2D and image[9] == 4:  # the test host crashes (stack overflow): VSTest records no test result
    (scratch / 'run.trx').write_text(
        '<TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010"><ResultSummary outcome="Failed">'
        '<Counters total="0" executed="0" passed="0" failed="0"/><RunInfos><RunInfo outcome="Error"><Text>'
        'The active test run was aborted. Reason: Test host process crashed : Stack overflow.</Text></RunInfo>'
        '</RunInfos></ResultSummary></TestRun>')
    sys.exit(1)
if image[8] == 0x2D and image[9] == 5:  # the run fails before writing any report
    sys.exit(1)
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
        self.cache = root / 'cache'
        self.mathcad = root / 'mathcad'
        self.mathcad.mkdir()
        (self.mathcad / 'mplrun.dll').write_bytes(b'mathcad')
        patcher = mock.patch.dict(os.environ, QUIRE_MUTANT_CACHE=str(self.cache))
        patcher.start()
        self.addCleanup(patcher.stop)
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

    def mutate(self, accepted=frozenset(), jobs=1, proposed=None, use_cache=True):
        return run(self.coverage, {'Quire.T'}, self.tests / 'Quire.Differential.Tests.dll', 'Quire.Mpl',
                   str(self.dotnet), jobs=jobs, accepted_keys=accepted, proposed=proposed, use_cache=use_cache,
                   mathcad=self.mathcad)

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

    def test_proposed_survivors_reported_apart(self):
        survivor = key(self.method, self.method['mutants'][1])
        killed = key(self.method, self.method['mutants'][0])
        path = Path(self.directory.name) / 'proposed.txt'
        path.write_text(f'# proposals\n{survivor} # dead branch\n{killed} # wrong\nQuire.Gone::X()@IL_0000 nop # gone\n')
        report = self.mutate(proposed=reasons(path))
        self.assertFalse(report['complete'])  # passes only with proposed mutants
        self.assertEqual(report['survivors'], [])
        self.assertEqual(report['proposed_survivors'], [{'key': survivor, 'reason': 'dead branch'}])
        self.assertEqual(report['stale_proposed'], sorted([killed, 'Quire.Gone::X()@IL_0000 nop']))
        accepted_too = self.mutate({survivor}, proposed=reasons(path))
        self.assertEqual((accepted_too['proposed_survivors'], accepted_too['accepted_survivors']), ([], 1))

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

    def test_a_looping_mutant_is_cut_off_relative_to_its_own_tests(self):
        image = bytearray(self.image); image[9] = 3
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes(image))
        started = time.monotonic()
        with mock.patch('mutate.MUTANT_FLOOR', 2):
            report = self.mutate()
        outcomes = {r['key'].split(' ', 1)[1]: r['outcome'] for r in report['mutants']}
        self.assertEqual(outcomes['brfalse.s -> brtrue.s'], 'killed (timeout)')
        self.assertEqual(report['timeouts'], 1)
        self.assertLess(report['max_mutant_timeout_seconds'], 60)
        self.assertLess(time.monotonic() - started, 30)

    def test_a_crash_under_a_passing_control_kills(self):
        image = bytearray(self.image); image[9] = 4
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes(image))
        report = self.mutate()
        outcomes = {r['key'].split(' ', 1)[1]: r['outcome'] for r in report['mutants']}
        self.assertEqual(outcomes['brfalse.s -> brtrue.s'], 'killed (crash)')
        self.assertEqual((report['crashes'], report['killed'], report['survivors']), (1, 3, []))

    def test_a_failure_without_a_crash_report_is_not_a_kill(self):
        image = bytearray(self.image); image[9] = 5
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes(image))
        report = self.mutate()
        outcomes = {r['key'].split(' ', 1)[1]: r['outcome'] for r in report['mutants']}
        self.assertEqual(outcomes['brfalse.s -> brtrue.s'], 'survived (tests failed without a mismatch)')
        self.assertEqual(report['crashes'], 0)

    def test_unchanged_build_reuses_outcomes(self):
        first = self.mutate()
        self.assertEqual(first['cached'], 0)
        log = Path(self.directory.name) / 'runs.log'
        with mock.patch.dict(os.environ, FAKE_DOTNET_LOG=str(log)):
            second = self.mutate()
        self.assertEqual(second['cached'], 3)
        self.assertEqual([r['outcome'] for r in second['mutants']], [r['outcome'] for r in first['mutants']])
        self.assertEqual(log.read_text().count('|'), 2)  # only the two controls ran
        self.assertFalse(list(self.cache.rglob('.entry-*')))

    def test_changed_bytes_or_runtime_miss_the_cache(self):
        self.mutate()
        (self.tests / 'Quire.Differential.Tests.dll').write_bytes(b'changed test')
        self.assertEqual(self.mutate()['cached'], 0)
        self.assertEqual(self.mutate()['cached'], 3)
        with mock.patch.dict(os.environ, FAKE_DOTNET_RUNTIME='Microsoft.NETCore.App 10.0.1'):
            self.assertEqual(self.mutate()['cached'], 0)
        (self.mathcad / 'mplrun.dll').write_bytes(b'another Mathcad')
        self.assertEqual(self.mutate()['cached'], 0)
        self.assertEqual(self.mutate()['cached'], 3)
        harness = Path(self.directory.name) / 'harness'
        harness.mkdir()
        for name in ('mutate.py', 'validate.py'):
            (harness / name).write_bytes((Path(__file__).with_name(name)).read_bytes())
        (harness / 'mutate.py').write_bytes(b'# edited\n' + (harness / 'mutate.py').read_bytes())
        with mock.patch('mutate.__file__', str(harness / 'mutate.py')):
            self.assertEqual(self.mutate()['cached'], 0)

    def test_gate_mode_neither_reads_nor_writes_the_cache(self):
        self.assertEqual(self.mutate(use_cache=False)['cached'], 0)
        self.assertFalse(self.cache.exists())
        self.mutate()
        entries = {p: p.read_bytes() for p in self.cache.rglob('*') if p.is_file()}
        self.assertEqual(len(entries), 3)
        self.assertEqual(self.mutate(use_cache=False)['cached'], 0)
        self.assertEqual({p: p.read_bytes() for p in self.cache.rglob('*') if p.is_file()}, entries)

    def test_crashes_are_not_cached(self):
        image = bytearray(self.image); image[9] = 4
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes(image))
        self.mutate()
        cached = {r['key'].split(' ', 1)[1]: (r['outcome'], r['cached']) for r in self.mutate()['mutants']}
        self.assertEqual(cached['brfalse.s -> brtrue.s'], ('killed (crash)', False))

    def test_timeouts_are_not_cached(self):
        image = bytearray(self.image); image[9] = 3
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes(image))
        with mock.patch('mutate.MUTANT_FLOOR', 2):
            self.mutate()
            report = self.mutate()
        cached = {r['key'].split(' ', 1)[1]: r['cached'] for r in report['mutants']}
        self.assertEqual(cached, {'add -> sub': True, 'mul -> div': True, 'brfalse.s -> brtrue.s': False})
        self.assertEqual(report['timeouts'], 1)

    def test_cached_outcomes_survive_a_different_timeout(self):
        self.mutate()
        with mock.patch('mutate.MUTANT_FLOOR', 5):
            report = self.mutate()
        self.assertEqual((report['cached'], report['max_mutant_timeout_seconds']), (3, 5))

    def test_cache_inside_the_repository_rejected(self):
        from validate import ROOT
        with mock.patch.dict(os.environ, QUIRE_MUTANT_CACHE=str(ROOT / 'cache')), \
                self.assertRaisesRegex(ValueError, 'inside the repository'):
            cache_directory()

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
