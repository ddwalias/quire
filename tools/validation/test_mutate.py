import os
import stat
import sys
import tempfile
import time
from pathlib import Path
import unittest
from unittest import mock
from mutate import annotate, cache_directory, key, reasons, run, select, stale_entries, write_bytes_retry, replace_retry

BODY = 'ab' * 32

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
if 'FAKE_DOTNET_PATHS' in os.environ:
    with open(os.environ['FAKE_DOTNET_PATHS'], 'a') as log:
        log.write(args[2] + '\\n' + str(scratch) + '\\n')

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
if args[args.index('--filter') + 1].strip('"') == 'FullyQualifiedName=Ns.Tests.Alone':
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
if image[9] in (7, 8, 10, 11):  # parallel retry fixtures: four independent looping mutants
    import time; time.sleep(120)
if image[9] in (17, 18):  # time out once under load, then complete on retry
    state = Path(os.environ['FAKE_DOTNET_STATE'] + str(image[9]))
    if not state.exists():
        state.write_text('slow')
        import time; time.sleep(120)
    finish(False)
if image[8] == 0x2D and image[9] == 4:  # the test host crashes (stack overflow): VSTest records no test result
    (scratch / 'run.trx').write_text(
        '<TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010"><ResultSummary outcome="Failed">'
        '<Counters total="0" executed="0" passed="0" failed="0"/><RunInfos><RunInfo outcome="Error"><Text>'
        'The active test run was aborted. Reason: Test host process crashed : Stack overflow.</Text></RunInfo>'
        '</RunInfos></ResultSummary></TestRun>')
    sys.exit(1)
if image[8] == 0x2D and image[9] == 6:  # slow only the first time (as under load), then it survives
    state = Path(os.environ['FAKE_DOTNET_STATE'])
    if not state.exists():
        state.write_text('slow')
        import time; time.sleep(120)
    finish(False)
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
    def test_write_bytes_retry_recovers_from_transient_permission_error(self):
        path = Path(self.directory.name) / 'retry.dll'
        write = Path.write_bytes
        attempts = []

        def flaky_write(target, data):
            attempts.append(target)
            if len(attempts) < 3:
                raise PermissionError('temporary sharing violation')
            return write(target, data)

        with mock.patch('mutate.time.sleep'), mock.patch.object(Path, 'write_bytes', flaky_write):
            write_bytes_retry(path, b'assembly')

        self.assertEqual(len(attempts), 3)
        self.assertEqual(path.read_bytes(), b'assembly')

    def test_replace_retry_recovers_from_transient_os_error(self):
        source = Path(self.directory.name) / 'src.txt'
        dest = Path(self.directory.name) / 'dst.txt'
        source.write_text('content')
        replace = Path.replace
        attempts = []

        def flaky_replace(src, dst):
            attempts.append((src, dst))
            if len(attempts) < 3:
                raise OSError(32, 'The process cannot access the file because it is being used by another process')
            return replace(src, dst)

        with mock.patch('mutate.time.sleep'), mock.patch.object(Path, 'replace', flaky_replace):
            replace_retry(source, dest)

        self.assertEqual(len(attempts), 3)
        self.assertEqual(dest.read_text(), 'content')
        self.assertFalse(source.exists())

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
        (root / 'swept').mkdir()
        sweeper = mock.patch('mutate.SWEEP_DIRECTORY', root / 'swept')  # never the real temporary directory
        sweeper.start()
        self.addCleanup(sweeper.stop)
        self.dotnet = root / 'dotnet'
        self.dotnet.write_text(FAKE_DOTNET.format(python=sys.executable))
        self.dotnet.chmod(self.dotnet.stat().st_mode | stat.S_IEXEC)
        if os.name == 'nt':  # no shebangs: a batch file runs the script with this Python
            self.dotnet = root / 'dotnet.cmd'
            self.dotnet.write_text(f'@"{sys.executable}" "%~dp0dotnet" %*\r\n')
        killed = dict(il_offset=1, file_offset=4, original='58', replacement='59', description='add -> sub')
        survived = dict(il_offset=5, file_offset=8, original='2C', replacement='2D', description='brfalse.s -> brtrue.s')
        caller = dict(il_offset=3, file_offset=6, original='5A', replacement='5B', description='mul -> div')
        self.method = dict(type='Quire.T', signature='M()', validation='compared', mutants=[killed, survived, caller],
                           cases=[dict(test='Ns.Tests.Compare')], quire_mvid='mvid', token=0x06000001,
                           body_sha256=BODY)
        self.coverage = {'methods': [self.method, dict(self.method, type='Quire.Other'),
                                     dict(self.method, signature='Stub()', validation='missing')]}

    def mutate(self, accepted=None, jobs=1, proposed=None, use_cache=True):
        return run(self.coverage, {'Quire.T'}, self.tests / 'Quire.Differential.Tests.dll', 'Quire.Mpl',
                   str(self.dotnet), jobs=jobs, accepted_keys=accepted or {}, proposed=proposed, use_cache=use_cache,
                   mathcad=self.mathcad)

    def test_only_compared_methods_of_selected_types(self):
        self.assertEqual(len(select(self.coverage, {'Quire.T'})), 3)

    def test_worker_count_respects_process_cpu_affinity(self):
        from mutate import effective_jobs
        if hasattr(os, 'sched_getaffinity'):
            with mock.patch('mutate.os.sched_getaffinity', return_value={0, 1, 2, 3}):
                self.assertEqual(effective_jobs(14), 4)
                self.assertEqual(effective_jobs(2), 2)
        else:
            with mock.patch('mutate.os.cpu_count', return_value=4):
                self.assertEqual(effective_jobs(14), 4)
                self.assertEqual(effective_jobs(2), 2)

    def test_mutant_timeout_scales_with_measured_control_speed_and_keeps_a_floor(self):
        from mutate import mutant_timeout
        with mock.patch('mutate.MUTANT_FLOOR', 3), mock.patch('mutate.MUTANT_MARGIN', 4):
            self.assertEqual(mutant_timeout(0.5, 60), 3)
            self.assertEqual(mutant_timeout(2, 60), 8)
            self.assertEqual(mutant_timeout(20, 60), 60)

    def test_compiler_generated_methods_use_their_compared_owner_tests(self):
        lambda_ = dict(self.method, type='Quire.T+<>c', signature='<M>b__0_0()', validation='excluded', cases=[],
                       owner=dict(type='Quire.T', name='M'), token=0x06000005)
        orphan = dict(lambda_, signature='<Stub>b__1_0()', owner=dict(type='Quire.T', name='Stub'))
        self.coverage['methods'] += [lambda_, orphan]
        chosen = [(m['signature'], m['cases']) for m, _ in select(self.coverage, {'Quire.T'})]
        self.assertIn(('<M>b__0_0()', [dict(test='Ns.Tests.Compare')]), chosen)
        self.assertNotIn('<Stub>b__1_0()', [name for name, _ in chosen])  # its owner is not compared

    def test_selected_methods_and_the_lambdas_they_own(self):
        lambda_ = dict(self.method, type='Quire.T+<>c', signature='<M>b__0_0()', validation='excluded', cases=[],
                       owner=dict(type='Quire.T', name='M'), token=0x06000005)
        self.coverage['methods'].append(lambda_)
        chosen = lambda methods: sorted({(m['type'], m['signature']) for m, _ in select(self.coverage, set(), methods)})
        self.assertEqual(chosen({'Quire.Other::M()'}), [('Quire.Other', 'M()')])
        self.assertEqual(chosen({'Quire.T::M()'}), [('Quire.T', 'M()'), ('Quire.T+<>c', '<M>b__0_0()')])
        self.assertEqual(chosen({'Quire.T+<>c::<M>b__0_0()'}), [('Quire.T+<>c', '<M>b__0_0()')])
        self.assertEqual(chosen({'Quire.T::Stub()'}), [])  # not compared

    def test_survivor_fails_until_accepted(self):
        for jobs in (1, 2):
            with self.subTest(jobs=jobs):
                report = self.mutate(jobs=jobs)
                self.assertEqual((report['total'], report['killed']), (3, 2))
                self.assertEqual(report['mutated_methods'], ['Quire.T::M()'])  # not Quire.Other, nor the stub
                self.assertEqual(report['survivors'], ['Quire.T::M()@IL_0005 brfalse.s -> brtrue.s'])
                self.assertFalse(report['complete'])
        survivor = key(self.method, self.method['mutants'][1])
        self.assertTrue(self.mutate({survivor: (BODY, 'dead branch')})['complete'])

    def test_results_carry_the_mutant_source_line(self):
        self.method['mutants'][1]['line'] = '/repo/src/T.cs:9'
        lines = {r['key']: r['line'] for r in self.mutate(use_cache=False)['mutants']}
        self.assertEqual(lines[key(self.method, self.method['mutants'][1])], '/repo/src/T.cs:9')
        self.assertIsNone(lines[key(self.method, self.method['mutants'][0])])

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
        accepted_too = self.mutate({survivor: (BODY, 'dead branch')}, proposed=reasons(path))
        self.assertEqual((accepted_too['proposed_survivors'], accepted_too['accepted_survivors']), ([], 1))

    def test_an_accepted_entry_excuses_only_the_body_it_was_reviewed_for(self):
        survivor = key(self.method, self.method['mutants'][1])
        for entry in ((None, 'no hash'), ('cd' * 32, 'another body')):
            with self.subTest(entry=entry):
                report = self.mutate({survivor: entry})
                self.assertEqual(report['survivors'], [survivor])
                self.assertEqual(report['accepted_survivors'], 0)
        self.assertEqual(stale_entries(self.coverage['methods'], {
            survivor: (BODY, 'ok'), survivor.replace('@IL_0005', '@IL_0009'): (BODY, 'no mutant'),
            'Quire.T::M()@IL_0001 add -> sub': (None, 'unhashed'), 'Quire.T::M()@IL_0003 mul -> div': ('cd' * 32, 'old'),
            'Quire.Gone::X()@IL_0000 nop': (BODY, 'gone')}), [
            dict(key='Quire.Gone::X()@IL_0000 nop', reason='gone', source='accepted', problem='no such method'),
            dict(key='Quire.T::M()@IL_0001 add -> sub', reason='unhashed', source='accepted', problem='no body hash'),
            dict(key='Quire.T::M()@IL_0003 mul -> div', reason='old', source='accepted', problem='method body changed'),
            dict(key='Quire.T::M()@IL_0009 brfalse.s -> brtrue.s', reason='no mutant', source='accepted',
                 problem='no such mutant')])
        # A proposal may omit the hash; one with a hash must match the current body.
        self.assertEqual(stale_entries(self.coverage['methods'], {survivor: (None, 'p')}, 'proposed'), [])
        report = self.mutate(proposed={survivor: ('cd' * 32, 'old proof')})
        self.assertEqual((report['survivors'], report['stale_proposed']), ([survivor], [survivor]))

    def test_entries_parse_with_and_without_a_body_hash(self):
        path = Path(self.directory.name) / 'entries.txt'
        path.write_text(f'# header\nQuire.T::M()@IL_0005 brfalse.s -> brtrue.s body={BODY} # dead branch\n'
                        'Quire.T::M()@IL_0001 add -> sub # no hash\n')
        self.assertEqual(reasons(path), {'Quire.T::M()@IL_0005 brfalse.s -> brtrue.s': (BODY, 'dead branch'),
                                         'Quire.T::M()@IL_0001 add -> sub': (None, 'no hash')})

    def test_malformed_hashes_and_duplicate_keys_excuse_nothing(self):
        survivor = key(self.method, self.method['mutants'][1])
        path = Path(self.directory.name) / 'entries.txt'
        path.write_text(f'{survivor} body={BODY[:-1]} # short hash\nQuire.T::M()@IL_0001 add -> sub body={BODY} # once\n'
                        f'Quire.T::M()@IL_0001 add -> sub body={BODY} # twice\n')
        entries = reasons(path)
        self.assertEqual(sorted(e['problem'] for e in stale_entries(self.coverage['methods'], entries)),
                         ['duplicate entry', 'malformed body hash'])
        self.assertEqual(self.mutate(entries)['survivors'], [survivor])
        self.assertEqual(self.mutate(proposed=entries)['survivors'], [survivor])

    def test_annotate_adds_the_current_body_hash(self):
        path = Path(self.directory.name) / 'accepted.txt'
        path.write_text('# header\n\nQuire.T::M()@IL_0005 brfalse.s -> brtrue.s # dead branch\n'
                        f'Quire.T::M()@IL_0001 add -> sub body={"cd" * 32} # rehashed\n'
                        'Quire.Gone::X()@IL_0000 nop # gone\n')
        self.assertEqual(annotate(path, self.coverage['methods']),
                         ([('Quire.Gone::X()@IL_0000 nop', 'no such method')], ['Quire.T::M()@IL_0001 add -> sub']))
        self.assertEqual(path.read_text(), f'# header\n\nQuire.T::M()@IL_0005 brfalse.s -> brtrue.s body={BODY} # dead branch\n'
                         f'Quire.T::M()@IL_0001 add -> sub body={BODY} # rehashed\nQuire.Gone::X()@IL_0000 nop # gone\n')
        self.assertEqual(stale_entries(self.coverage['methods'], {k: v for k, v in reasons(path).items()
                                                                  if 'Gone' not in k}), [])

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

    def test_a_timeout_is_rerun_alone_and_only_a_second_timeout_kills(self):
        image = bytearray(self.image); image[9] = 6
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes(image))
        log = Path(self.directory.name) / 'runs.log'
        floor = 10 if os.name == 'nt' else 2
        with mock.patch('mutate.MUTANT_FLOOR', floor), mock.patch.dict(os.environ, FAKE_DOTNET_LOG=str(log),
                FAKE_DOTNET_STATE=str(Path(self.directory.name) / 'state')):
            report = self.mutate(jobs=2, use_cache=False)
        result = {r['key'].split(' ', 1)[1]: r for r in report['mutants']}['brfalse.s -> brtrue.s']
        self.assertEqual((result['outcome'], result['parallel_outcome']), ('survived', 'killed (timeout)'))
        self.assertEqual((report['timeouts'], report['timed_out'], report['timeout_retries']), (0, [], 1))
        self.assertEqual(report['survivors'], ['Quire.T::M()@IL_0005 brfalse.s -> brtrue.s'])
        # Two controls and three mutants, then a fresh control and the rerun.
        self.assertEqual(log.read_text().count('|'), 7)

    def test_a_mutant_that_times_out_again_is_killed_and_listed(self):
        image = bytearray(self.image); image[9] = 3
        (self.tests / 'Quire.Mpl.dll').write_bytes(bytes(image))
        self.method['mutants'].append(dict(il_offset=7, file_offset=8, original='2C', replacement='2D',
                                           description='brfalse.s -> brtrue.s (again)'))
        log = Path(self.directory.name) / 'runs.log'
        with mock.patch('mutate.MUTANT_FLOOR', 2), mock.patch.dict(os.environ, FAKE_DOTNET_LOG=str(log)):
            report = self.mutate(use_cache=False)
        self.assertEqual((report['timeouts'], report['timeout_retries']), (2, 2))
        self.assertEqual(report['timed_out'], ['Quire.T::M()@IL_0005 brfalse.s -> brtrue.s',
                                               'Quire.T::M()@IL_0007 brfalse.s -> brtrue.s (again)'])
        # Two controls and four mutants, then one fresh control for the shared test set and two reruns.
        self.assertEqual(log.read_text().count('|'), 9)

    def test_timed_out_mutants_retry_in_parallel_with_affinity_calibrated_controls(self):
        loops = [dict(il_offset=offset, file_offset=9, original='00', replacement=f'{value:02x}',
                      description=f'loop {value}')
                 for offset, value in zip((7, 8, 10, 11), (7, 8, 10, 11))]
        method = dict(self.method, mutants=loops)
        coverage = {'methods': [method]}
        with mock.patch('mutate.MUTANT_FLOOR', 1), mock.patch('mutate.MUTANT_MARGIN', 4):
            started = time.monotonic()
            report = run(coverage, {'Quire.T'}, self.tests / 'Quire.Differential.Tests.dll', 'Quire.Mpl',
                         str(self.dotnet), jobs=4, accepted_keys={}, use_cache=False)
            elapsed = time.monotonic() - started
        self.assertEqual((report['total'], report['timeouts'], report['timeout_retries']), (4, 4, 4))
        self.assertEqual((report['timeout_retry_jobs'], report['timeout_retry_controls']), (4, 4))
        self.assertEqual((report['jobs'], report['requested_jobs']), (4, 4))
        self.assertTrue(report['complete'])
        self.assertEqual({r['outcome'] for r in report['mutants']}, {'killed (timeout)'})
        self.assertLess(report['timeout_retry_elapsed_seconds'], 60 if os.name == 'nt' else 3)
        self.assertLess(elapsed, 90 if os.name == 'nt' else 10)

    def test_parallel_retries_still_accept_a_slow_mutant_that_finishes_on_retry(self):
        loops = [dict(il_offset=offset, file_offset=9, original='00', replacement=f'{value:02x}',
                      description=f'slow {value}')
                 for offset, value in zip((17, 18), (17, 18))]
        coverage = {'methods': [dict(self.method, mutants=loops)]}
        with mock.patch('mutate.MUTANT_FLOOR', 1), mock.patch.dict(os.environ,
                FAKE_DOTNET_STATE=str(Path(self.directory.name) / 'parallel-state')):
            report = run(coverage, {'Quire.T'}, self.tests / 'Quire.Differential.Tests.dll', 'Quire.Mpl',
                         str(self.dotnet), jobs=2, accepted_keys={}, use_cache=False)
        self.assertEqual((report['timeout_retries'], report['timeouts']), (2, 0))
        self.assertEqual(report['timeout_retry_jobs'], 2)
        self.assertEqual({r['parallel_outcome'] for r in report['mutants']}, {'killed (timeout)'})
        self.assertEqual({r['outcome'] for r in report['mutants']}, {'survived'})
        self.assertEqual(report['survivors'], sorted(r['key'] for r in report['mutants']))

    def test_copies_and_run_directories_live_under_the_given_scratch_directory(self):
        scratch = Path(self.directory.name) / 'results/mutation-scratch'
        paths = Path(self.directory.name) / 'paths.log'
        with mock.patch.dict(os.environ, FAKE_DOTNET_PATHS=str(paths)):
            run(self.coverage, {'Quire.T'}, self.tests / 'Quire.Differential.Tests.dll', 'Quire.Mpl', str(self.dotnet),
                accepted_keys={}, scratch=scratch)
        used = paths.read_text().splitlines()
        self.assertEqual(len(used), 10)
        self.assertTrue(all(Path(p).is_relative_to(scratch) for p in used), used)
        self.assertEqual(list(scratch.iterdir()), [])

    def test_stale_mutant_directories_of_this_user_are_swept(self):
        from mutate import sweep_stale
        tmp = Path(self.directory.name) / 'tmp'
        tmp.mkdir()
        old = time.time() - 2 * 24 * 3600
        busy = tmp / 'quire-mutant-busy'
        (busy / 'copy').mkdir(parents=True)
        (busy / 'copy/Quire.Mpl.dll').write_text('rewritten by every mutant')  # recent, deep inside an old directory
        for path in (busy / 'copy', busy):
            os.utime(path, (old, old))
        for name in ('quire-mutant-old', 'quire-mutant-run-old', 'quire-mutant-new', 'other-old', 'quire-mutant-file'):
            path = tmp / name
            path.mkdir() if name != 'quire-mutant-file' else path.write_text('x')
            if name != 'quire-mutant-new':
                os.utime(path, (old, old))
        try:
            (tmp / 'quire-mutant-link').symlink_to(tmp / 'other-old')
            os.utime(tmp / 'quire-mutant-link', (old, old), follow_symlinks=False)
        except OSError:  # Windows without the symlink privilege: nothing to leave alone
            (tmp / 'quire-mutant-link').write_text('x')
        if hasattr(os, 'getuid'):
            with mock.patch('mutate.os.getuid', return_value=os.getuid() + 1):
                self.assertEqual(sweep_stale(tmp), [])  # someone else's
        self.assertEqual(sorted(p.name for p in sweep_stale(tmp)), ['quire-mutant-old', 'quire-mutant-run-old'])
        self.assertEqual(sorted(p.name for p in tmp.iterdir()),
                         ['other-old', 'quire-mutant-busy', 'quire-mutant-file', 'quire-mutant-link', 'quire-mutant-new'])
        with mock.patch('mutate.SWEEP_ENTRIES', 1):
            os.utime(busy / 'copy/Quire.Mpl.dll', (old, old))
            self.assertEqual(sweep_stale(tmp), [])  # too large to examine: left alone
        self.assertEqual(sweep_stale(tmp), [busy])

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
                jobs=3, accepted_keys={})

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
                accepted_keys={}, global_json=b'{"sdk": {}}')
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
