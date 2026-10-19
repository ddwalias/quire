import tempfile
from pathlib import Path
import unittest
import unittest.mock
from validate import read_trx


class MutationJobsTests(unittest.TestCase):
    def test_default_is_bounded_by_cpu_count(self):
        from validate import mutation_jobs
        for cpus, expected in [(None, 1), (1, 1), (2, 2), (8, 8), (12, 12), (64, 12)]:
            with self.subTest(cpus=cpus), unittest.mock.patch('validate.os.cpu_count', return_value=cpus):
                self.assertEqual(mutation_jobs(), expected)

    def test_explicit_worker_count_is_positive_and_can_override_the_cap(self):
        import argparse
        from validate import mutation_jobs
        self.assertEqual(mutation_jobs('1'), 1)
        self.assertEqual(mutation_jobs('16'), 16)
        for invalid in ('0', '-1', 'auto', ''):
            with self.subTest(invalid=invalid), self.assertRaises(argparse.ArgumentTypeError):
                mutation_jobs(invalid)

    def test_both_commands_reject_invalid_worker_counts_before_running(self):
        import subprocess, sys
        for script, args in [('validate.py', []), ('merge_gate.py', ['unused-branch'])]:
            for invalid in ('0', '-1', 'auto'):
                with self.subTest(script=script, jobs=invalid):
                    result = subprocess.run([sys.executable, str(Path(__file__).parent / script), *args,
                                             '--jobs', invalid], capture_output=True, text=True)
                    self.assertEqual(result.returncode, 2)
                    self.assertIn('--jobs must be a positive integer', result.stderr)


class PhaseProjectsTests(unittest.TestCase):
    def test_phase_includes_every_current_solution_test_project(self):
        import validate
        projects = validate.phase_projects()
        self.assertIn('Quire.Engine.Tests', projects)
        self.assertIn('Quire.Controls.Tests', projects)
        self.assertNotIn('Quire.Focused.Differential.Tests', projects)

    def test_solution_tests_cannot_be_silently_omitted(self):
        from validate import phase_projects
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / 'tests/Extra.Tests').mkdir(parents=True)
            (root / 'tests/Extra.Tests/Extra.Tests.csproj').write_text('')
            (root / 'Quire.slnx').write_text('<Solution><Project Path="tests/New.Tests/New.Tests.csproj" /></Solution>')
            with self.assertRaisesRegex(ValueError, 'phase profile would omit.*New.Tests'):
                phase_projects(root)
            (root / 'Quire.slnx').write_text('<Solution/>')
            self.assertEqual(phase_projects(root), ['Extra.Tests'])

    def test_solution_paths_accept_windows_separators(self):
        from validate import phase_projects
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / 'tests/New.Tests').mkdir(parents=True)
            (root / 'tests/New.Tests/New.Tests.csproj').write_text('')
            (root / 'Quire.slnx').write_text('<Solution><Project Path=".\\tests\\New.Tests\\New.Tests.csproj" /></Solution>')
            self.assertEqual(phase_projects(root), ['New.Tests'])


class ResultTests(unittest.TestCase):
    def check(self, outcomes, summary="Completed", allow=False):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "result.trx"
            path.write_text('<TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">'
                            '<Results>' + ''.join(f'<UnitTestResult outcome="{o}" />' for o in outcomes)
                            + f'</Results><ResultSummary outcome="{summary}" /></TestRun>')
            return read_trx(path, allow)

    def test_pass(self):
        self.assertEqual(self.check(["Passed", "Passed"]), {"passed": 2, "failed": 0, "skipped": 0})

    def test_phase_gate_can_defer_differential_failures_to_the_method_ratchet(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "result.trx"
            path.write_text('<TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">'
                            '<Results><UnitTestResult outcome="Passed" /><UnitTestResult outcome="Failed" />'
                            '</Results><ResultSummary outcome="Failed" /></TestRun>')
            self.assertEqual(read_trx(path, allow_failures=True), {"passed": 1, "failed": 1, "skipped": 0})

    def test_false_green_runs_rejected(self):
        for outcomes, summary in [([], "Completed"), (["NotExecuted"], "Completed"),
                                  (["Passed", "Failed"], "Failed"), (["Passed"], "Aborted"),
                                  (["Passed", "NotExecuted"], "Completed"), (["Passed", "Timeout"], "Completed")]:
            with self.subTest(outcomes=outcomes, summary=summary), self.assertRaises(ValueError):
                self.check(outcomes, summary)

    def test_allowed_skips_are_counted_but_all_skipped_rejected(self):
        self.assertEqual(self.check(["Passed", "NotExecuted"], allow=True)["skipped"], 1)
        with self.assertRaises(ValueError):
            self.check(["NotExecuted"], allow=True)



class ChangedTypeTests(unittest.TestCase):
    def setUp(self):
        from validate import ROOT
        root = ROOT.resolve().as_posix()
        self.coverage = {'methods': [
            dict(type='Quire.Mcd.Dim', source=[dict(file=f'{root}/src/Quire.Mpl/Mcd/Dim.cs', lines=[1])], cases=[]),
            dict(type='Quire.Mcd.NumVal', source=[dict(file=f'{root}/src/Quire.Mpl/Mcd/NumVal.cs', lines=[1])],
                 cases=[dict(test='Quire.Differential.Tests.NumValDifferentialTests.Compare')])]}

    TEST_TYPES = [dict(name=f'Quire.Differential.Tests.{n}DifferentialTests', documents=[], references=[])
                  for n in ('Dim', 'NumVal')]

    def types(self, *names, status='M'):
        from validate import changed_types
        return changed_types(self.coverage, ''.join(f'{status}\0{n}\0' for n in names), sources={},
                             test_types=self.TEST_TYPES)

    def test_changed_source_and_changed_tests_select_types(self):
        self.assertEqual(self.types('src/Quire.Mpl/Mcd/Dim.cs'), {'Quire.Mcd.Dim'})
        self.assertEqual(self.types('tests/Quire.Differential.Tests/NumValDifferentialTests.cs'), {'Quire.Mcd.NumVal'})
        self.assertEqual(self.types('docs/mirror/WORK.md'), set())

    def test_nested_and_case_variant_test_classes_map_to_their_file(self):
        self.coverage['methods'][1]['cases'] = [dict(test='Quire.Differential.Tests.NumValDifferentialTests+More.Compare')]
        self.assertEqual(self.types('tests/Quire.Differential.Tests/numvaldifferentialtests.cs'), {'Quire.Mcd.NumVal'})

    def test_cases_removed_from_a_changed_file_still_select_their_type(self):
        from validate import changed_types
        previous = {'Quire.Mcd.Dim::Pwr()': ['dimdifferentialtests'], 'Quire.Gone::X()': ['dimdifferentialtests']}
        for status in 'MD':  # cases cut from the file, or the whole file deleted
            names = f'{status}\0tests/Quire.Differential.Tests/DimDifferentialTests.cs\0'
            self.assertEqual(changed_types(self.coverage, names, previous, sources={}, test_types=self.TEST_TYPES),
                             {'Quire.Mcd.Dim'})
        self.assertEqual(self.types('src/Quire.Mpl/Mcd/Dim.cs', status='D'), set())

    def test_source_without_methods_selects_nothing(self):
        self.assertEqual(self.types('src/Quire.Mpl/Mcd/Kinds.cs'), set())

    def test_source_outside_the_repository_fails(self):
        self.coverage['methods'][0]['source'][0]['file'] = '/elsewhere/Dim.cs'
        with self.assertRaises(ValueError):
            self.types('src/Quire.Mpl/Mcd/Dim.cs')


class ChangedMethodTests(unittest.TestCase):
    """The gate mutates methods whose IL, comparing tests, callees or the fields they read changed, not whole types."""

    def setUp(self):
        def method(name, body, calls=(), virtual_calls=(), cases=(), validation='compared', slot=None, token=0,
                   reads=(), writes=(), owner=None, type='Quire.T'):
            return dict(type=type, signature=f'{name}`0()->[System.Runtime]System.Void|False', project='Quire.Mpl',
                        token=token, body_sha256=body, calls=list(calls), virtual_calls=list(virtual_calls),
                        is_virtual=bool(slot), slot=slot, validation=validation, field_reads=list(reads),
                        field_writes=list(writes), owner=owner,
                        cases=[dict(test=f'Quire.Differential.Tests.{c}.Compare') for c in cases])
        self.methods = {
            'A': method('A', 'a', cases=['ADifferentialTests'], token=1),
            'B': method('B', 'b', cases=['BDifferentialTests'], token=2),
            'Caller': method('Caller', 'c', calls=[1], cases=['CDifferentialTests'], token=3),
            'Virtual': method('System.IFoo.Run', 'v', slot='Run`0/0', token=4),
            'Dispatcher': method('D', 'd', virtual_calls=['Run`0/0'], token=5),
            'Ctor': method('.ctor', 'k', writes=['Quire.T::f'], token=6),
            'Reader': method('R', 'r', reads=['Quire.T::f'], token=7),
            'Iterator': method('MoveNext', 'i', validation='excluded', owner=dict(type='Quire.T', name='B'),
                               type='Quire.T+<B>d__1', token=8),
            'Async': method('MoveNext', 'as', validation='excluded', owner=dict(type='Quire.T', name='A'),
                            type='Quire.T+<A>d__2', token=11),
            'BCaller': method('BC', 'bc', calls=[2], token=9),
        }
        self.baseline = {'bodies': {self.key(m): m['body_sha256'] for m in self.methods.values()},
                         'methods': {self.key(m): 'compared' for m in self.methods.values()},
                         'tests': {}, 'writes': {}}

    @staticmethod
    def key(m):
        return f"{m['type']}::{m['signature']}"

    def selected(self, names='', sources=None, test_types=None):
        from validate import changed_methods
        status = ''.join(f'M\0{n}\0' for n in names.split()) if names else ''
        if test_types is None:  # an inventory whose IL relates no test classes
            test_types = [dict(name=f'Quire.Differential.Tests.{c}DifferentialTests', documents=[], references=[])
                          for c in 'ABC']
            sources = {} if sources is None else sources
        reasons, _ = changed_methods({'methods': list(self.methods.values())}, status, self.baseline, sources, test_types)
        return {k.split('::')[1].split('`')[0] for k in reasons}

    def test_unchanged_build_selects_nothing(self):
        self.assertEqual(self.selected('src/Quire.Mpl/T.cs'), set())

    def test_changed_il_selects_the_method_and_its_direct_callers_only(self):
        self.methods['A']['body_sha256'] = 'a2'
        self.assertEqual(self.selected(), {'A', 'Caller'})
        self.methods['A']['body_sha256'], self.methods['Reader']['body_sha256'] = 'a', 'r2'
        self.assertEqual(self.selected(), {'R'})

    def test_virtual_calls_reach_overrides_and_interface_implementations(self):
        self.methods['Virtual']['body_sha256'] = 'v2'
        self.assertEqual(self.selected(), {'System.IFoo.Run', 'D'})
        # A non-virtual method is reached only through its token.
        self.methods['Virtual'].update(body_sha256='v3', slot=None, is_virtual=False)
        self.assertEqual(self.selected(), {'System.IFoo.Run'})

    def test_a_removed_override_selects_callers_of_its_slot(self):
        del self.methods['Virtual']
        self.assertEqual(self.selected(), {'D'})

    def test_readers_of_fields_a_changed_or_removed_method_writes(self):
        self.methods['Ctor']['body_sha256'] = 'k2'
        self.assertEqual(self.selected(), {'.ctor', 'R'})
        # A write the change removed still counts, from the baseline.
        self.methods['Ctor']['field_writes'] = []
        self.baseline['writes'] = {self.key(self.methods['Ctor']): ['Quire.T::f']}
        self.assertEqual(self.selected(), {'.ctor', 'R'})
        self.methods['Ctor']['body_sha256'] = 'k'
        del self.methods['Ctor']
        self.assertEqual(self.selected(), {'R'})

    def test_a_changed_state_machine_changes_its_owner_and_the_owners_callers(self):
        self.methods['Iterator']['body_sha256'] = 'i2'
        self.assertEqual(self.selected(), {'MoveNext', 'B', 'BC'})
        self.methods['Iterator']['body_sha256'] = 'i'
        self.methods['Async']['body_sha256'] = 'as2'  # an async method's state machine
        self.assertEqual(self.selected(), {'MoveNext', 'A', 'Caller'})

    def test_changed_or_deleted_tests_select_the_methods_they_compare(self):
        self.assertEqual(self.selected('tests/Quire.Differential.Tests/BDifferentialTests.cs'), {'B'})
        self.baseline['tests'] = {self.key(self.methods['A']): ['gonedifferentialtests']}
        self.assertEqual(self.selected('tests/Quire.Differential.Tests/GoneDifferentialTests.cs'), {'A'})

    def test_classes_using_a_helper_of_a_changed_test_file_count_as_changed(self):
        sources = {'tests/Quire.Differential.Tests/BDifferentialTests.cs': 'class BDifferentialTests { Helper h; }',
                   'tests/Quire.Differential.Tests/ADifferentialTests.cs': 'class ADifferentialTests { Values.X(); }',
                   'tests/Quire.Differential.Tests/Values.cs': 'class Values { X() => BDifferentialTests.Storage(); }',
                   'tests/Quire.Differential.Tests/CDifferentialTests.cs': 'class CDifferentialTests { }'}
        # B's helper reaches A through the protected Values class; C names neither. The IL relates none of them.
        self.assertEqual(self.selected('tests/Quire.Differential.Tests/BDifferentialTests.cs', sources), {'A', 'B'})

    def test_il_and_text_dependencies_are_united(self):
        from validate import changed_test_classes
        ns = 'Quire.Differential.Tests.'
        # C uses B's extension method (IL only); A names C (text only, say through an attribute the IL walk missed).
        test_types = [dict(name=ns + n, documents=[], references=r) for n, r in
                      [('BDifferentialTests', []), ('CDifferentialTests', [ns + 'BDifferentialTests']),
                       ('ADifferentialTests', []), ('DDifferentialTests', [])]]
        sources = {f'tests/Quire.Differential.Tests/{n}DifferentialTests.cs': f'class {n}DifferentialTests {{ {body} }}'
                   for n, body in [('B', ''), ('C', 'void M() => 1.Render();'), ('A', 'CDifferentialTests c;'),
                                   ('D', '')]}
        classes, how = changed_test_classes([('M', 'tests/Quire.Differential.Tests/BDifferentialTests.cs')],
                                            sources, test_types)
        self.assertEqual(classes, {'adifferentialtests', 'bdifferentialtests', 'cdifferentialtests'})
        self.assertEqual(how, 'IL and text')

    def test_an_inventory_without_test_types_counts_every_test_class_as_changed(self):
        from validate import ALL_TEST_CLASSES, changed_test_classes
        sources = {'tests/Quire.Differential.Tests/BDifferentialTests.cs': 'class BDifferentialTests { }'}
        for test_types in (None, [], [dict(name='Quire.Differential.Tests.BDifferentialTests', documents=[])]):
            with self.subTest(test_types=test_types):
                self.assertEqual(changed_test_classes([('M', 'tests/Quire.Differential.Tests/BDifferentialTests.cs')],
                                                      sources, test_types), (ALL_TEST_CLASSES, 'all'))

    def test_test_class_dependencies_come_from_the_built_assembly(self):
        from validate import ROOT
        file = f'{ROOT.resolve().as_posix()}/tests/Quire.Differential.Tests/BDifferentialTests.cs'
        ns = 'Quire.Differential.Tests.'
        test_types = [dict(name=ns + 'BDifferentialTests', documents=[file], references=[]),
                      # A calls an extension method B declares: no textual mention of B, but an IL reference.
                      dict(name=ns + 'ADifferentialTests', documents=[], references=[ns + 'BDifferentialTests']),
                      dict(name=ns + 'CDifferentialTests', documents=[], references=[ns + 'ADifferentialTests'])]
        self.assertEqual(self.selected('tests/Quire.Differential.Tests/BDifferentialTests.cs', test_types=test_types),
                         {'A', 'B', 'Caller'})

    def test_without_the_assembly_or_sources_every_test_class_counts_as_changed(self):
        from validate import changed_methods
        reasons, notes = changed_methods({'methods': list(self.methods.values())},
                                         'M\0tests/Quire.Differential.Tests/BDifferentialTests.cs\0', self.baseline)
        self.assertEqual({k.split('::')[1].split('`')[0] for k in reasons}, {'A', 'B', 'Caller'})
        self.assertIn('test class dependencies: all', notes)

    def test_reading_a_field_that_holds_a_mutable_reference_counts_as_writing_it(self):
        # Def fills the table a field holds (ldsfld; stelem): readers of the field see the change.
        self.methods['Def'] = dict(self.methods['B'], signature='Def`0()->[System.Runtime]System.Void|False', token=10,
                                   body_sha256='def', cases=[], field_reads=['Quire.T::f'], reference_reads=['Quire.T::f'])
        self.baseline['bodies'][self.key(self.methods['Def'])] = 'def2'
        self.assertEqual(self.selected(), {'Def', 'R'})

    def test_callers_of_accessors_reading_a_changed_field_selected_through_accessor_chains(self):
        getter = lambda name, calls, token: dict(self.methods['Reader'], signature=f'{name}`0()->[System.Runtime]System.Int32|False',
                                                  token=token, calls=calls, field_reads=[], body_sha256=name)
        self.methods['Reader']['signature'] = 'get_F`0()->[System.Runtime]System.Int32|False'
        self.methods['G'] = getter('get_G', [7], 20)     # a getter calling the field-reading getter
        self.methods['U'] = getter('Use', [20], 21)      # an ordinary method calling that getter
        self.methods['V'] = getter('Other', [21], 22)    # two calls away from an ordinary method: not followed
        for m in self.methods.values():
            self.baseline['bodies'][self.key(m)], self.baseline['methods'][self.key(m)] = m['body_sha256'], 'compared'
        self.methods['Ctor']['body_sha256'] = 'k2'
        self.assertEqual(self.selected(), {'.ctor', 'get_F', 'get_G', 'Use'})

    def test_new_and_newly_compared_methods_selected(self):
        del self.baseline['bodies'][self.key(self.methods['B'])]
        self.baseline['methods'][self.key(self.methods['A'])] = 'uncovered'
        self.assertEqual(self.selected(), {'A', 'B', 'BC'})

    def test_a_build_without_body_hashes_fails(self):
        del self.methods['A']['body_sha256']
        with self.assertRaises(ValueError):
            self.selected()

    def test_slots_derive_from_inventory_keys(self):
        from validate import slot_of_key
        for key, slot in [('ToString`0()->[System.Runtime]System.String|False', 'ToString`0/0'),
                          ('System.Collections.Generic.IDictionary<K,V>.Add`0(!0,!1)->[System.Runtime]System.Void|False',
                           'Add`0/2'),
                          ('M`1([System.Runtime]System.Collections.Generic.Dictionary`2<[System.Runtime]System.String,'
                           '!!0>,[System.Runtime]System.Int32[,])->!!0|False', 'M`1/2')]:
            self.assertEqual(slot_of_key(key), slot)
        self.methods['Virtual']['slot'] = 'Run`0/1'
        with self.assertRaises(ValueError):
            self.selected()

    def test_reasons_recorded(self):
        from validate import changed_methods
        self.methods['A']['body_sha256'] = 'a2'
        reasons, notes = changed_methods({'methods': list(self.methods.values())},
                                         'M\0tests/Quire.Differential.Tests/ADifferentialTests.cs\0', self.baseline, {},
                                         [dict(name='Quire.Differential.Tests.ADifferentialTests', documents=[],
                                               references=[])])
        self.assertEqual(reasons[self.key(self.methods['A'])], ['IL changed', 'comparing tests changed'])
        self.assertEqual(reasons[self.key(self.methods['Caller'])], ['calls a changed method'])
        self.assertEqual(notes, ['test class dependencies: IL and text'])


class StaleAcceptedTests(unittest.TestCase):
    def test_a_scoped_run_checks_entries_against_the_whole_inventory(self):
        from method_coverage import audit
        from validate import stale_accepted
        body = 'ab' * 32
        def method(type_, token):
            return dict(project='Quire.Mpl', type=type_, signature='M()', token=token, status='Unclassified',
                        access='Public', exclusion='compiler-generated', body_sha256=body,
                        mutants=[dict(il_offset=1, description='add -> sub')])
        inventory = {'schema': 2, 'errors': [], 'methods': [method('Quire.A', 1), method('Quire.B', 2)]}
        entries = {'Quire.B::M()@IL_0001 add -> sub': (body, 'equivalent')}
        with tempfile.TemporaryDirectory() as repo:
            scoped = audit(inventory, [], set(), 'run', repo=repo, types=['Quire.A'], unreachable={})
        self.assertEqual([m['type'] for m in scoped['methods']], ['Quire.A'])  # what a --method-type run keeps
        self.assertEqual(stale_accepted(inventory, entries), [])
        self.assertEqual(stale_accepted(dict(inventory, methods=inventory['methods'][:1]), entries)[0]['problem'],
                         'no such method')


class SurvivorDetailsTests(unittest.TestCase):
    def test_each_survivor_with_its_repository_line_and_text(self):
        from validate import survivor_details
        with tempfile.TemporaryDirectory() as root:
            root = Path(root).resolve()
            (root / 'src').mkdir()
            (root / 'src/T.cs').write_text('class T {\n    int M(int a) => a + 1;\n}\n')
            mutation = {'survivors': ['T::M@IL_0002 add -> sub', 'T::N@IL_0000 nop', 'T::O@IL_0000 nop'], 'mutants': [
                {'key': 'T::M@IL_0002 add -> sub', 'line': f'{root}/src/T.cs:2'},
                {'key': 'T::K@IL_0000 nop', 'line': f'{root}/src/T.cs:1'},  # killed or accepted: not listed
                {'key': 'T::N@IL_0000 nop', 'line': None},
                {'key': 'T::O@IL_0000 nop', 'line': '/elsewhere/T.cs:2'}]}
            self.assertEqual(survivor_details(mutation, root), [
                {'key': 'T::M@IL_0002 add -> sub', 'location': 'src/T.cs:2', 'source': 'int M(int a) => a + 1;'},
                {'key': 'T::N@IL_0000 nop', 'location': None, 'source': None},
                {'key': 'T::O@IL_0000 nop', 'location': None, 'source': None}])


class VerdictTests(unittest.TestCase):
    BASE = dict(projects=[dict(project='P', exit_code=0, passed=3, failed=0, skipped=0)], ratchet=dict(ok=True, regressions=[]),
                mutation=dict(survivors=[], proposed_survivors=[]), executed_unreachable=[],
                method_coverage=dict(compared=5, missing=0), complete=True, gate_passed=True)

    def verdict(self, **changes):
        from validate import verdict
        return verdict(dict(self.BASE, **changes))

    def test_outcomes_follow_the_exit_status(self):
        self.assertEqual(self.verdict()['line'], 'LOCAL CHECKS: pass · GATE: would pass')
        gaps = self.verdict(complete=False, method_coverage=dict(compared=5, missing=2))
        self.assertEqual(gaps['line'], 'LOCAL CHECKS: pass · GATE: would pass with gaps (2 missing)')
        stale = self.verdict(complete=False, stale_accepted=[dict(key='k', problem='method body changed')])
        self.assertEqual(stale['line'], 'LOCAL CHECKS: pass · GATE: would pass with gaps (1 stale accepted-mutant entries)')
        blocked = self.verdict(complete=False, gate_passed=False, mutation=dict(survivors=['k'], proposed_survivors=[]))
        self.assertEqual(blocked['line'], 'LOCAL CHECKS: fail · GATE: would block (1 surviving mutants)')

    def test_pending_proposals_block_the_gate_but_pass_locally(self):
        result = self.verdict(complete=False, gate_passed=False, proposed_exempt_lines=1, proposed_mutants='/p',
                              mutation=dict(survivors=[], proposed_survivors=[dict(key='k', reason='r')] * 2))
        self.assertEqual(result['line'], 'LOCAL CHECKS: pass · GATE: would block (2 proposed mutants pending approval; '
                                         '1 proposed unreachable lines pending approval)')
        self.assertEqual(len(result['pending_approval']), 2)

    def test_gaps_name_incomplete_integrity_and_mutation(self):
        result = self.verdict(complete=False, integrity=dict(lint=1, complete=False),
                              mutation=dict(survivors=[], proposed_survivors=[], complete=False))
        self.assertEqual(result['reasons'], ['integrity incomplete', 'mutation incomplete'])

    def test_errors_and_scoped_runs_are_labelled(self):
        result = self.verdict(error='build failed', ratchet=None, complete=False, gate_passed=False)
        self.assertEqual(result['local_checks'], 'fail')
        self.assertIn('validation error: build failed', result['line'])
        self.assertTrue(self.verdict(method_types=['Quire.T'])['line'].endswith('[scoped to 1 types; the gate validates all]'))

    def test_local_reports_disclose_deferred_mutation(self):
        result = self.verdict(profile='focused', mutation_requested=False, mutation=None)
        self.assertTrue(result['line'].endswith('[mutation deferred to gate]'))
        self.assertNotIn('deferred', self.verdict(profile='focused', mutation_requested=True)['line'])
        self.assertNotIn('deferred', self.verdict(profile='phase', mutation_requested=False)['line'])


class SealTests(unittest.TestCase):
    def test_changes_to_late_read_files_or_outputs_detected(self):
        from validate import Seal
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / 'docs').mkdir()
            (root / 'docs/type-map.csv').write_text('map')
            (root / 'bin').mkdir()
            (root / 'bin/Harness.dll').write_bytes(b'a')
            (root / 'src.cs').write_text('read before tests; not sealed')
            def seal():
                return Seal(root, [root / 'docs/type-map.csv', root / 'docs/absent.md'], [root / 'bin'])
            unchanged = seal()
            (root / 'src.cs').write_text('changed')
            unchanged.verify('run')
            for change in (lambda: (root / 'docs/type-map.csv').write_text('forged'),
                           lambda: (root / 'docs/absent.md').write_text('planted'),
                           lambda: (root / 'bin/Harness.dll').write_bytes(b'b'),
                           lambda: (root / 'bin/planted.dll').write_bytes(b'c')):
                sealed = seal()
                change()
                with self.subTest(change=change), self.assertRaises(ValueError):
                    sealed.verify('run')


class SealTreeTests(unittest.TestCase):
    def test_records_added_nested_or_deleted_at_any_depth_detected(self):
        from validate import Seal
        def plant(root, name):
            (root / name).parent.mkdir(parents=True, exist_ok=True)
            (root / name).write_text('planted')
        for change in (lambda root: (root / 'evidence/sub/A.md').write_text('forged'),
                       lambda root: (root / 'evidence/sub/A.md').unlink(),
                       lambda root: plant(root, 'evidence/new/deep/B.txt'),
                       lambda root: plant(root, 'manifest/C.json')):
            with self.subTest(change=change), tempfile.TemporaryDirectory() as directory:
                root = Path(directory)
                plant(root, 'evidence/sub/A.md')
                sealed = Seal(root, [], [root / 'evidence', root / 'manifest'])
                change(root)
                with self.assertRaises(ValueError):
                    sealed.verify('run')


class SdkDirectoryTests(unittest.TestCase):
    def test_each_command_gets_a_new_directory_holding_the_global_json_read_before_tests(self):
        from validate import sdk_directory
        with sdk_directory(b'{"sdk": {"version": "10.0.100"}}\r\n') as first:
            self.assertEqual((first / 'global.json').read_bytes(), b'{"sdk": {"version": "10.0.100"}}\r\n')
            (first / 'global.json').write_text('{"sdk": {"paths": ["/fake"]}}')  # rewritten by a lingering process
        self.assertFalse(first.exists())
        with sdk_directory(b'{}\n') as second:
            self.assertNotEqual(second, first)
            self.assertEqual((second / 'global.json').read_bytes(), b'{}\n')
        self.assertFalse(second.exists())


class DotnetTests(unittest.TestCase):
    def test_cli_resolved_to_an_absolute_path_outside_the_repository(self):
        import os, stat, sys
        from validate import resolve_dotnet
        with tempfile.TemporaryDirectory() as repo, tempfile.TemporaryDirectory() as tools:
            for directory in (repo, tools):
                cli = Path(directory) / ('dotnet.exe' if os.name == 'nt' else 'dotnet')
                cli.write_text('#!/bin/sh\n')
                cli.chmod(cli.stat().st_mode | stat.S_IEXEC)
            found = os.path.normcase(str((Path(tools) / cli.name).resolve()))
            self.assertEqual(os.path.normcase(resolve_dotnet(str(Path(tools) / 'dotnet'), repo)), found)
            with unittest.mock.patch.dict(os.environ, PATH=tools):
                self.assertEqual(os.path.normcase(resolve_dotnet('dotnet', repo)), found)
            with unittest.mock.patch.dict(os.environ, PATH=os.pathsep.join([repo, tools])), \
                    self.assertRaisesRegex(ValueError, 'inside the repository'):
                resolve_dotnet('dotnet', repo)
            with self.assertRaisesRegex(ValueError, 'not found'):
                resolve_dotnet(str(Path(tools) / 'missing'), repo)


class MethodAuditTests(unittest.TestCase):
    def test_structure_is_compared_on_a_release_build_of_the_debug_assemblies_host(self):
        import validate
        for profile, host, framework in [('focused', 'Quire.Focused.Differential.Tests', 'net10.0'),
                                         ('phase', 'Quire.Mirror.Tests', 'net10.0-windows')]:
            (project, build), scan = validate.method_audit('dotnet', Path('tool'), Path('release'), Path('install'),
                                                           Path('inventory.json'), profile)
            self.assertEqual(project, validate.ROOT / 'tests' / host / f'{host}.csproj')
            self.assertEqual(build, ['-c', 'Release', '--no-incremental', '-o', 'release'])
            self.assertEqual(scan[scan.index('--structure-assemblies') + 1], 'release')
            self.assertEqual(Path(scan[scan.index('--assemblies') + 1]), validate.ROOT / 'tests' / host / 'bin/Debug' / framework)
            self.assertEqual('--project' in scan, profile == 'focused')


    def test_a_release_build_changed_after_it_was_sealed_is_detected(self):
        import validate
        with tempfile.TemporaryDirectory() as tool, tempfile.TemporaryDirectory() as release:
            (Path(release) / 'Quire.Mpl.dll').write_bytes(b'built')
            sealed = validate.digest_outputs(tool, release)
            self.assertEqual(validate.digest_outputs(tool, release), sealed)
            (Path(release) / 'Quire.Mpl.dll').write_bytes(b'planted')
            self.assertNotEqual(validate.digest_outputs(tool, release), sealed)


class MissingShapeTests(unittest.TestCase):
    """Every line of the maintainer-owned manifest must still match the build; added lines are fine."""
    def test_missing_and_changed_lines_are_reported(self):
        import subprocess
        import validate
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / 'docs/mirror/manifest').mkdir(parents=True)
            (root / 'docs/mirror/manifest/Quire.Mpl.txt').write_text(
                '# header\nT Quire.Mcd.T class public :  | \n  Quire.Mcd.T F private System.UInt32 m_kept\n'
                '  Quire.Mcd.T F private System.UInt32 m_gone\n  Quire.Mcd.T M public virtual Run`0() -> System.Void\n')
            built = ('T Quire.Mcd.T class public :  | \n  Quire.Mcd.T F private System.UInt32 m_kept\n'
                     '  Quire.Mcd.T M public Run`0() -> System.Void\n  Quire.Mcd.T M private Helper`0() -> System.Void\n')
            with unittest.mock.patch.object(validate, 'ROOT', root), unittest.mock.patch.object(
                    validate.subprocess, 'run', return_value=subprocess.CompletedProcess([], 0, built, '')):
                self.assertEqual(validate.missing_shape('dotnet', root, b'{}', root / 'Quire.Mpl.dll', 10),
                                 ['Quire.Mcd.T F private System.UInt32 m_gone', 'Quire.Mcd.T M public virtual Run`0() -> System.Void'])


class PrivateCopyTests(unittest.TestCase):
    def test_gate_runs_never_use_the_mutant_cache(self):
        from types import SimpleNamespace
        from validate import mutant_cache_allowed
        local = dict(no_mutant_cache=False, mutate_since=None, write_baseline=None)
        self.assertTrue(mutant_cache_allowed(SimpleNamespace(**local)))
        for gate in (dict(no_mutant_cache=True), dict(mutate_since='main'), dict(write_baseline=Path('b.json'))):
            with self.subTest(gate=gate):
                self.assertFalse(mutant_cache_allowed(SimpleNamespace(**dict(local, **gate))))

    def test_gate_runs_refuse_proposals(self):
        import os, subprocess, sys
        with tempfile.TemporaryDirectory() as directory:
            proposals = Path(directory) / "proposed.txt"
            proposals.write_text("")
            for flag in ("--proposed-mutants", "--proposed-unreachable"):
                for gate in (["--mutate-since", "main"], ["--write-baseline", str(Path(directory) / "b.json")]):
                    with self.subTest(flag=flag, gate=gate):
                        result = subprocess.run([sys.executable, "-I", "-B", str(Path(__file__).parent / "validate.py"),
                                                 flag, str(proposals), *gate],
                                                env=dict(os.environ, QUIRE_VALIDATOR_REPO=directory),
                                                capture_output=True, text=True)
                        self.assertEqual(result.returncode, 2)
                        self.assertIn(f"{flag} is for local runs", result.stderr)

    def test_validator_runs_isolated_from_a_copy(self):
        import os, shutil, subprocess, sys
        with tempfile.TemporaryDirectory() as directory:
            copy = Path(directory) / "validation"
            shutil.copytree(Path(__file__).parent, copy, ignore=shutil.ignore_patterns("__pycache__"))
            result = subprocess.run([sys.executable, "-I", "-B", str(copy / "validate.py"), "--help"],
                                    env=dict(os.environ, QUIRE_VALIDATOR_REPO=directory), capture_output=True, text=True)
            self.assertEqual(result.returncode, 0, result.stderr)
            self.assertFalse(list(copy.rglob("*.pyc")))


class WslPhaseTests(unittest.TestCase):
    """The phase profile from WSL: Windows' dotnet.exe gets Windows paths and WSLENV, and its outputs are read back."""
    def test_windows_dotnet_precedence(self):
        from validate import WINDOWS_DOTNET, windows_dotnet
        installed = unittest.mock.patch('validate.Path.is_file', return_value=True)
        missing = unittest.mock.patch('validate.Path.is_file', return_value=False)
        with installed:
            self.assertEqual(windows_dotnet('/mnt/d/dotnet/dotnet.exe', {}), '/mnt/d/dotnet/dotnet.exe')
            self.assertIsNone(windows_dotnet('/usr/bin/dotnet', {}))  # an explicit Linux CLI is not silently replaced
            self.assertEqual(windows_dotnet('dotnet', {'QUIRE_WINDOWS_DOTNET': '/mnt/e/x/dotnet.exe'}), '/mnt/e/x/dotnet.exe')
            self.assertIsNone(windows_dotnet('dotnet', {'QUIRE_WINDOWS_DOTNET': '/usr/bin/dotnet'}))
            self.assertEqual(windows_dotnet('dotnet', {}), WINDOWS_DOTNET)
        with missing:
            self.assertIsNone(windows_dotnet('dotnet', {}))

    def test_wsl_is_detected_only_off_windows(self):
        import validate
        with unittest.mock.patch.object(validate.os, 'name', 'posix'):
            self.assertTrue(validate.is_wsl({'WSL_DISTRO_NAME': 'Ubuntu'}))
            self.assertFalse(validate.is_wsl({}))
        with unittest.mock.patch.object(validate.os, 'name', 'nt'):
            self.assertFalse(validate.is_wsl({'WSL_DISTRO_NAME': 'Ubuntu'}))

    def test_paths_are_converted_and_only_windows_drives_qualify(self):
        import subprocess
        import validate
        def wslpath(output, code=0):
            return unittest.mock.patch.object(validate.subprocess, 'run',
                                              return_value=subprocess.CompletedProcess([], code, output + '\n', ''))
        with wslpath(r'C:\Projects\Quire\tests\T.csproj') as run:
            self.assertEqual(validate.windows_path('/mnt/c/Projects/Quire/tests/T.csproj'), r'C:\Projects\Quire\tests\T.csproj')
            self.assertEqual(run.call_args[0][0][:2], ['wslpath', '-w'])
        for output, code in [(r'\\wsl.localhost\Ubuntu\tmp\x', 0), ('', 0), ('', 1)]:
            with self.subTest(output=output, code=code), wslpath(output, code), \
                    self.assertRaisesRegex(ValueError, 'not on a Windows drive'):
                validate.windows_path('/tmp/x')

    def test_method_audit_paths_go_through_the_converter(self):
        import validate
        (project, build), scan = validate.method_audit('dotnet.exe', Path('tool'), Path('release'), Path('install'),
                                                       Path('inventory.json'), 'phase', lambda p: f'W[{p}]')
        self.assertEqual(build[-1], 'W[release]')
        for flag in ('--repo', '--assemblies', '--structure-assemblies', '--install', '--output'):
            with self.subTest(flag=flag):
                self.assertTrue(scan[scan.index(flag) + 1].startswith('W['))
        self.assertTrue(scan[1].startswith('W[') and scan[1].endswith('Quire.Tools.MethodAudit.dll]'))

    def test_windows_outputs_are_read_back_through_their_mount(self):
        from validate import wsl_document, wsl_inventory
        root, windows_root = Path('/mnt/c/Projects/Quire'), 'C:\\Projects\\Quire'
        # Inside the repository: case-insensitive, as on Windows, and mapped onto the repository's own path.
        self.assertEqual(wsl_document('c:\\projects\\QUIRE\\src\\A.cs', windows_root, root), '/mnt/c/Projects/Quire/src/A.cs')
        self.assertEqual(wsl_document('C:/Projects/Quire/src/A.cs', windows_root, root), '/mnt/c/Projects/Quire/src/A.cs')
        self.assertEqual(wsl_document('C:\\Users\\u\\.nuget\\packages\\x\\a.cs', windows_root, root),
                         '/mnt/c/Users/u/.nuget/packages/x/a.cs')
        self.assertEqual(wsl_document('C:\\Projects\\QuireOther\\a.cs', windows_root, root), '/mnt/c/Projects/QuireOther/a.cs')
        for unchanged in ('/_/src/a.cs', 'relative.cs', ''):
            self.assertEqual(wsl_document(unchanged, windows_root, root), unchanged)
        inventory = wsl_inventory({'methods': [{'source': [{'file': 'C:\\Projects\\Quire\\src\\A.cs'}]}, {'source': None}],
                                   'test_types': [{'documents': ['C:\\Projects\\Quire\\tests\\T.cs']}]}, windows_root, root)
        self.assertEqual(inventory['methods'][0]['source'][0]['file'], '/mnt/c/Projects/Quire/src/A.cs')
        self.assertEqual(inventory['test_types'][0]['documents'], ['/mnt/c/Projects/Quire/tests/T.cs'])

    def test_coverage_file_names_are_mapped_before_duplicates_merge(self):
        from branch_coverage import read_cobertura
        with tempfile.TemporaryDirectory() as directory:
            report = Path(directory) / 'a.cobertura.xml'
            report.write_text('<coverage><packages><package><classes>'
                              '<class filename="C:\\R\\A.cs"><lines><line number="3" hits="1" condition-coverage="50% (1/2)"/></lines></class>'
                              '<class filename="c:\\r\\A.cs"><lines><line number="3" hits="2" condition-coverage="100% (2/2)"/></lines></class>'
                              '</classes></package></packages></coverage>')
            lines = read_cobertura([report], lambda name: '/mnt/c/R/' + name.replace('\\', '/').rsplit('/', 1)[-1])
        self.assertEqual(lines, {('/mnt/c/R/A.cs', 3): (2, 1, 2)})  # the partial branch is not hidden

    def test_wslenv_forwards_what_tests_read(self):
        from validate import FORWARDED, wslenv
        env = wslenv({'WSLENV': 'WT_SESSION:QUIRE_REQUIRE_MATHCAD/u:MATHCAD_DIR', 'PATH': '/usr/bin'})
        names = env['WSLENV'].split(':')
        self.assertEqual(names, ['WT_SESSION', *FORWARDED])
        self.assertEqual(env['PATH'], '/usr/bin')
        for name in ('QUIRE_REQUIRE_MATHCAD', 'QUIRE_METHOD_CASES', 'QUIRE_VALIDATION_RUN', 'MATHCAD_DIR/p'):
            self.assertIn(name, names)
        self.assertEqual(wslenv({})['WSLENV'], ':'.join(FORWARDED))

    def test_windows_temp_must_be_on_a_windows_drive(self):
        import validate
        with tempfile.TemporaryDirectory() as directory:
            target = Path(directory) / 'scratch'
            with unittest.mock.patch.object(validate, 'windows_path', side_effect=ValueError('not on a Windows drive')), \
                    self.assertRaisesRegex(ValueError, 'Windows drive'):
                validate.windows_temp({'QUIRE_WINDOWS_TEMP': str(target)})
            self.assertFalse(target.exists())
            with unittest.mock.patch.object(validate, 'windows_path', return_value='C:\\scratch'):
                self.assertEqual(validate.windows_temp({'QUIRE_WINDOWS_TEMP': str(target)}), str(target.resolve()))
                self.assertTrue(target.is_dir())
                self.assertEqual(validate.windows_temp({}, Path(directory) / 'repo'),
                                 str((Path(directory) / '.quire-windows-temp').resolve()))

    def run_validate(self, *args, env=None):
        import os, subprocess, sys
        with tempfile.TemporaryDirectory() as directory:
            environ = {k: v for k, v in os.environ.items() if not k.startswith(('WSL', 'QUIRE_WINDOWS'))}
            return subprocess.run([sys.executable, "-I", "-B", str(Path(__file__).parent / "validate.py"), *args],
                                  env=dict(environ, QUIRE_VALIDATOR_REPO=directory, **(env or {})),
                                  capture_output=True, text=True)

    def test_phase_off_windows_needs_wsl_and_a_windows_cli(self):
        import os
        if os.name == 'nt':
            self.skipTest('native Windows runs the phase profile directly')
        result = self.run_validate('--profile', 'phase')
        self.assertEqual(result.returncode, 2)
        self.assertIn('from WSL with a Windows dotnet.exe', result.stderr)
        result = self.run_validate('--profile', 'phase', '--dotnet', '/usr/bin/dotnet', env={'WSL_DISTRO_NAME': 'x'})
        self.assertEqual(result.returncode, 2)
        self.assertIn('from WSL with a Windows dotnet.exe', result.stderr)

    def test_phase_from_wsl_refuses_a_checkout_off_the_windows_drives(self):
        import os, shutil
        if os.name == 'nt' or not shutil.which('wslpath'):
            self.skipTest('needs WSL')
        # The test repository is a new directory in /tmp, which Windows reaches only through \\wsl.localhost.
        result = self.run_validate('--profile', 'phase', '--dotnet', '/mnt/c/x/dotnet.exe',
                                   env={'WSL_DISTRO_NAME': 'x', 'QUIRE_WINDOWS_TEMP': '/tmp'})
        self.assertEqual(result.returncode, 2)
        self.assertIn('not on a Windows drive', result.stderr)

    def test_phase_runs_ratchet_against_their_own_baseline(self):
        import ratchet
        from validate import default_baseline
        self.assertNotEqual(ratchet.PHASE_BASELINE, ratchet.BASELINE)
        self.assertEqual(ratchet.PHASE_BASELINE.parent, ratchet.BASELINE.parent)
        self.assertEqual(default_baseline("phase"), ratchet.PHASE_BASELINE)
        self.assertEqual(default_baseline("focused"), ratchet.BASELINE)
        # main takes it from there, so neither profile can read the other's baseline.
        source = (Path(__file__).parent / "validate.py").read_text()
        self.assertIn("baseline_path = args.baseline or default_baseline(args.profile)", source)


if __name__ == "__main__":
    unittest.main()
