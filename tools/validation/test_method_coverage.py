import copy
import hashlib
import tempfile
from pathlib import Path
import unittest
import method_coverage
from method_coverage import audit, passed_tests as passed_tests_from_trx, unreachable_lines
from mutate import select


class CoverageTests(unittest.TestCase):
    def test_phase_scoped_skips_are_not_counted_as_passing_comparisons(self):
        trx = Path(self.directory.name) / 'phase.trx'
        trx.write_text('''<?xml version="1.0"?>
<TestRun xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
  <TestDefinitions><UnitTest id="pass"><TestMethod className="Ns.Tests.Compare" name="Passed" /></UnitTest>
    <UnitTest id="skip"><TestMethod className="Ns.Tests.Compare" name="Skipped" /></UnitTest></TestDefinitions>
  <Results><UnitTestResult testId="pass" outcome="Passed" /><UnitTestResult testId="skip" outcome="NotExecuted" /></Results>
  <ResultSummary outcome="Completed" />
</TestRun>''')

        with self.assertRaisesRegex(ValueError, 'unsuccessful test results'):
            passed_tests_from_trx([trx])
        self.assertEqual(passed_tests_from_trx([trx], allow_skips=True), {'Ns.Tests.Compare.Passed'})

    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()
        self.addCleanup(self.directory.cleanup)
        self.repo = Path(self.directory.name).resolve()
        self.file = (self.repo / 'src/T.cs').as_posix()
        self.write_source()
        evidence = self.repo / 'docs/mirror/evidence'
        evidence.mkdir(parents=True)
        contract = b'Original hash original-sha MVID original-mvid; private method token 0x06000002'
        (evidence / 'Contract.md').write_bytes(contract)
        self.method = dict(project='Quire.Mpl', type='Quire.T', signature='private()', token=0x06000001,
                           access='Private', status='Verified', is_static=True, is_constructor=False, quire_sha256='quire-sha', quire_mvid='quire-mvid',
                           original=dict(assembly='mplrun', sha256='original-sha', mvid='original-mvid', token=0x06000002),
                           declaration_only=False, exclusion=None, shape_matches=True,
                           structure_matches=True, structure_differences=[], source=[self.doc([10, 11])])
        self.lines = {(self.file, 10): (3, 0, 0), (self.file, 11): (3, 2, 2)}
        self.record = dict(schema=1, run='fresh', test='Tests.Compare', case_id='edge case', inputs=[1], outcome='1', arguments_after=[1],
                           contract='Contract', contract_sha256=hashlib.sha256(contract).hexdigest(),
                           quire=dict(assembly='Quire.Mpl', sha256='quire-sha', mvid='quire-mvid', token=0x06000001),
                           original=self.method['original'])

    def check(self, methods=None, records=None, tests=None, types=(), lines=False, unreachable=None, proposed=None,
              sources=None):
        return audit(dict(schema=2, errors=[], methods=methods if methods is not None else [self.method]),
                     records if records is not None else [self.record], tests if tests is not None else {'Tests.Compare'},
                     'fresh', self.repo, types, self.lines if lines is False else lines,
                     unreachable=unreachable, proposed_unreachable=proposed, sources=sources)

    def test_private_method_requires_its_own_comparison(self):
        self.assertFalse(self.check(records=[])['complete'])
        self.assertTrue(self.check()['complete'])

    def test_public_caller_does_not_cover_private_method(self):
        record = copy.deepcopy(self.record)
        record['quire']['token'] += 1
        self.assertFalse(self.check(records=[record])['complete'])

    def test_changed_binary_wrong_original_and_overload_do_not_count(self):
        for side, field, value in [('quire', 'sha256', 'changed'), ('quire', 'mvid', 'changed'),
                                   ('original', 'sha256', 'changed'), ('original', 'token', 0x06000003)]:
            record = copy.deepcopy(self.record)
            record[side][field] = value
            with self.subTest(side=side, field=field):
                try:
                    self.assertFalse(self.check(records=[record])['complete'])
                except ValueError:
                    pass  # Changed original contracts must be rejected before counting.

    def test_failed_test_and_stale_run_rejected(self):
        with self.assertRaises(ValueError):
            self.check(tests=set())
        record = dict(self.record, run='old')
        with self.assertRaises(ValueError):
            self.check(records=[record])

    def test_empty_and_unknown_type_cannot_pass(self):
        with self.assertRaises(ValueError):
            self.check(methods=[], records=[])
        with self.assertRaises(ValueError):
            self.check(types=['Missing.Type'])

    def test_constructor_accessor_initializer_and_explicit_method_each_required(self):
        for signature in ['.ctor()', '.cctor()', 'get_Value()', 'Ns.IThing.Run()']:
            extra = dict(self.method, signature=signature, token=0x06000004)
            with self.subTest(signature=signature):
                self.assertFalse(self.check(methods=[self.method, extra])['complete'])

    def test_instance_requires_state_assertions(self):
        method = dict(self.method, is_static=False)
        report = self.check(methods=[method])
        self.assertFalse(report['complete'])
        self.assertEqual((report['methods'][0]['validation'], report['methods'][0]['stage']), ('missing', 'unchecked_state'))
        self.assertTrue(self.check(methods=[method], records=[dict(self.record, state_checked=True)])['complete'])

    def test_unmatched_and_stub_methods_cannot_pass(self):
        for change in [dict(original=None), dict(status='Stub'), dict(shape_matches=False)]:
            with self.subTest(change=change):
                self.assertFalse(self.check(methods=[dict(self.method, **change)])['complete'])

    def test_bodyless_and_compiler_cases_reported_separately(self):
        declaration = dict(self.method, declaration_only=True)
        self.assertEqual(self.check(methods=[declaration], records=[])['counts']['declaration_only'], 1)
        generated = dict(self.method, exclusion='compiler-generated')
        self.assertEqual(self.check(methods=[generated], records=[])['counts']['excluded'], 1)

    def test_contract_changes_and_missing_token_rejected(self):
        path = self.repo / 'docs/mirror/evidence/Contract.md'
        path.write_bytes(b'changed')
        with self.assertRaises(ValueError):
            self.check()
        content = b'original-sha original-mvid but no method token'
        path.write_bytes(content)
        record = dict(self.record, contract_sha256=hashlib.sha256(content).hexdigest())
        with self.assertRaises(ValueError):
            self.check(records=[record])

    def test_structural_difference_fails_unless_accepted_deviation(self):
        differs = dict(self.method, structure_matches=False, structure_differences=['calls differ at #0'])
        report = self.check(methods=[differs])
        self.assertFalse(report['complete'])
        self.assertEqual(report['counts']['structure_mismatch'], 1)
        self.assertIn('calls differ at #0', report['methods'][0]['reason'])
        self.assertFalse(self.check(methods=[dict(self.method, structure_matches=None)])['complete'])
        deviation = dict(differs, status='Deviation', evidence='DEV-0001')
        self.assertFalse(self.check(methods=[deviation])['complete'])
        ledger = self.repo / 'docs/mirror/deviations.md'
        ledger.write_text('| DEV-0001 | Mcd.T | differs | proposed | x |\n')
        self.assertFalse(self.check(methods=[deviation])['complete'])
        ledger.write_text('| DEV-0001 | `Quire.U::private` | differs | accepted-temporary | x |\n')
        self.assertFalse(self.check(methods=[deviation])['complete'])  # accepted, but for another member
        ledger.write_text('| DEV-0001 | `Quire.T::private` | differs | accepted-temporary | x |\n')
        self.assertTrue(self.check(methods=[deviation])['complete'])

    def test_structure_alone_does_not_replace_a_direct_comparison(self):
        report = self.check(records=[])
        self.assertEqual(report['counts']['missing'], 1)
        self.assertEqual(report['methods'][0]['stage'], 'untested')
        self.assertEqual(self.check(methods=[dict(self.method, status='Stub')])['methods'][0]['stage'], 'missing')
        self.assertEqual(self.check()['methods'][0]['stage'], 'compared')

    def test_every_line_and_branch_must_execute(self):
        for lines, detail in [({(self.file, 10): (3, 0, 0)}, f"['{self.file}:11']"), ({**self.lines, (self.file, 11): (3, 1, 2)}, '11 (1/2)'),
                              (None, 'not collected')]:
            with self.subTest(detail=detail):
                report = self.check(lines=lines)
                self.assertEqual(report['counts']['uncovered'], 1)
                self.assertIn(detail, report['methods'][0]['reason'])
        self.assertEqual(self.check(methods=[dict(self.method, source=None)])['counts']['uncovered'], 1)

    def test_verified_status_needs_this_runs_evidence(self):
        self.assertEqual(self.check()['counts']['verified_unsupported'], 0)
        report = self.check(records=[])
        self.assertEqual(report['counts']['verified_unsupported'], 1)
        self.assertFalse(report['methods'][0]['status_supported'])
        self.assertEqual(self.check(methods=[dict(self.method, status='Analyzed')], records=[])['counts']['verified_unsupported'], 0)

    def test_counts_by_type(self):
        other = dict(self.method, type='Quire.U', token=0x06000009)
        by_type = self.check(methods=[self.method, other])['by_type']
        self.assertEqual(by_type['Quire.T']['compared'], 1)
        self.assertEqual(by_type['Quire.T']['total'], 1)
        self.assertEqual(by_type['Quire.U']['missing'], 1)
        self.assertEqual(by_type['Quire.U']['verified_unsupported'], 1)

    SOURCE = ['class T', '{', '    object M()', '    {', '        x();', '', '', '', '', '        Throw();',
              '        return null;  ', '    }', '}']

    def write_source(self, lines=None):
        (self.repo / 'src').mkdir(exist_ok=True)
        (self.repo / 'src/T.cs').write_text('\n'.join(lines or self.SOURCE) + '\n')

    def doc(self, lines, file=None, path='src/T.cs'):
        """A PDB document as MethodAudit exports it, with the checksum of the file as it is now (as compiled)."""
        return dict(file=file or self.file, lines=lines,
                    checksum=dict(algorithm='sha256', hash=hashlib.sha256((self.repo / path).read_bytes()).hexdigest()))

    def entry(self, location='src/T.cs:11', key='Quire.T::private()', methods=None, reason='return after a throwing helper'):
        return method_coverage.entry(methods or [self.method], key, location, reason, self.repo)

    def unreachable(self, *texts):
        path = self.repo / 'docs/mirror/unreachable-lines.txt'
        path.write_text('# header\n  # indented comment\n\n' + ''.join(t + '\n' for t in texts))
        return unreachable_lines(path, self.repo)

    def test_listed_unreachable_line_is_exempt(self):
        lines = {(self.file, 10): (3, 0, 0)}
        text = self.entry()
        expected = hashlib.sha256('1\n        Throw();\n        return null;'.encode()).hexdigest()
        self.assertEqual(text, f'Quire.T::private() src/T.cs:11 sha256={expected} # return after a throwing helper')
        entries = self.unreachable(text)
        report = self.check(lines=lines, unreachable=entries)
        self.assertTrue(report['complete'])
        self.assertEqual(report['methods'][0]['validation'], 'compared')
        self.assertEqual(report['methods'][0]['exempt_lines'], [f'{self.file}:11'])
        self.assertEqual(report['stale_unreachable'], [])
        # The repository's file is the default; no entry exempts nothing.
        self.assertTrue(self.check(lines=lines)['complete'])
        self.assertEqual(self.check(lines=lines, unreachable={})['counts']['uncovered'], 1)

    def test_changed_method_source_or_position_voids_the_entry(self):
        lines = {(self.file, 10): (3, 0, 0), (self.file, 12): (3, 0, 0)}
        entries = self.unreachable(self.entry())
        # Another line of the same span, hashed for line 11: the position is part of the hash.
        moved = dict(self.method, source=[self.doc([10, 11, 12])])
        shifted = {'Quire.T::private()': {('src/T.cs', 12): entries['Quire.T::private()'][('src/T.cs', 11)]}}
        lines11 = {(self.file, 10): (3, 0, 0), (self.file, 11): (3, 0, 0)}
        report = self.check(methods=[moved], lines=lines11, unreachable=shifted)
        self.assertEqual([e['problem'] for e in report['stale_unreachable']], ['method source changed'])
        # A rebuilt method whose source changed.
        self.write_source(self.SOURCE[:9] + ['        Throw(1);'] + self.SOURCE[10:])
        rebuilt = dict(self.method, source=[self.doc([10, 11])])
        report = self.check(methods=[rebuilt], lines=lines, unreachable=entries)
        self.assertEqual(report['methods'][0]['validation'], 'uncovered')
        self.assertEqual([e['problem'] for e in report['stale_unreachable']], ['method source changed'])
        self.assertFalse(report['complete'])
        # Trailing whitespace and lines outside the method do not matter.
        self.write_source([line + '  ' for line in self.SOURCE[:9]] + self.SOURCE[9:])
        rebuilt = dict(self.method, source=[self.doc([10, 11])])
        self.assertTrue(self.check(methods=[rebuilt], lines=lines, unreachable=entries)['complete'])

    def test_checksum_mismatch_not_exempted(self):
        lines = {(self.file, 10): (3, 0, 0)}
        entries = self.unreachable(self.entry())
        # Sources read before the build are what the compiled checksum must match, whatever the file holds later.
        sources = method_coverage.read_sources(entries, repo=self.repo)
        self.write_source(self.SOURCE + ['// changed after the build'])
        self.assertTrue(self.check(lines=lines, unreachable=entries, sources=sources)['complete'])
        sha1 = dict(algorithm='sha1', hash=hashlib.sha1((self.repo / 'src/T.cs').read_bytes()).hexdigest())
        for checksum in [dict(algorithm='sha256', hash='0' * 64), dict(algorithm='unknown-guid', hash='00'), None, sha1]:
            with self.subTest(checksum=checksum):
                method = dict(self.method, source=[dict(self.method['source'][0], checksum=checksum)])
                report = self.check(methods=[method], lines=lines, unreachable=entries, sources=sources)
                self.assertEqual(report['methods'][0]['validation'], 'uncovered')
                self.assertEqual([e['problem'] for e in report['stale_unreachable']], ['document checksum mismatch'])
        # An inventory from before MethodAudit exported checksums.
        method = dict(self.method, source=[{k: v for k, v in self.method['source'][0].items() if k != 'checksum'}])
        report = self.check(methods=[method], lines=lines, unreachable=entries, sources=sources)
        self.assertEqual([e['problem'] for e in report['stale_unreachable']], ['inventory lacks checksums; rerun'])
        # The file compiled differs from the one read before the build.
        report = self.check(lines=lines, unreachable=entries)
        self.assertEqual([e['problem'] for e in report['stale_unreachable']], ['document checksum mismatch'])

    def test_malformed_duplicate_and_outside_entries_rejected(self):
        good = self.entry()
        digest = good.split()[2]
        for texts in [[good.replace(' # return after a throwing helper', '')], [good.replace(digest + ' ', '')],
                      [good.replace('src/T.cs:11', 'src/T.cs')], [good.replace('sha256=', 'sha1=')],
                      [good, good], [good.replace('src/T.cs', 'tests/T.cs')], [good.replace('src/T.cs', 'src/U.cs')],
                      [good.replace('src/T.cs', 'src/../src/T.cs')]]:
            with self.subTest(texts=texts), self.assertRaises(ValueError):
                self.unreachable(*texts)
        with self.assertRaises(ValueError):
            self.entry(key='Quire.Gone::X()')
        with self.assertRaises(ValueError):
            self.entry(location='src/T.cs:12')  # not a statement line of the method

    def test_shared_method_keys_are_ambiguous_only_when_listed(self):
        twin = dict(self.method, project='Quire.Other', source=[self.doc([10, 11])])
        report = self.check(methods=[self.method, twin], records=[], unreachable={})
        self.assertEqual(report['counts']['missing'], 2)
        entries = self.unreachable(self.entry())
        report = self.check(methods=[self.method, twin], lines={(self.file, 10): (3, 0, 0)}, unreachable=entries)
        self.assertEqual([e['problem'] for e in report['stale_unreachable']], ['ambiguous method key'])
        self.assertEqual(report['methods'][0]['validation'], 'uncovered')
        with self.assertRaises(ValueError):
            self.entry(methods=[self.method, twin])

    def test_document_paths_match_exactly_relative_to_the_repository(self):
        self.assertEqual(method_coverage.relative(self.file, self.repo), 'src/T.cs')
        self.assertIsNone(method_coverage.relative('/elsewhere/src/T.cs', self.repo))
        self.assertTrue(method_coverage.matches('C:\\Clone\\Src\\T.cs', 'src/T.cs', 'c:/clone'))
        self.assertFalse(method_coverage.matches('C:\\Clone\\Sub\\src\\T.cs', 'src/T.cs', 'c:/clone'))
        self.assertTrue(method_coverage.look_alike('C:\\Clone\\Sub\\src\\T.cs', 'src/T.cs', 'c:/clone'))
        # A POSIX path remains case-sensitive even when this test runs on Windows.
        self.assertFalse(method_coverage.matches('/repo/SRC/T.cs', 'src/T.cs', '/repo'))

    def test_nested_look_alike_not_exempted(self):
        (self.repo / 'src/Sub/src').mkdir(parents=True)
        (self.repo / 'src/Sub/src/T.cs').write_bytes((self.repo / 'src/T.cs').read_bytes())
        nested = (self.repo / 'src/Sub/src/T.cs').as_posix()
        entries = self.unreachable(self.entry())
        for source in [[self.doc([10, 11], nested, 'src/Sub/src/T.cs')],
                       [self.doc([10, 11]), self.doc([12], nested, 'src/Sub/src/T.cs')],
                       [self.doc([10, 11], '/tmp/quire-gate-abc/src/T.cs')]]:
            with self.subTest(source=source):
                method = dict(self.method, source=source)
                report = self.check(methods=[method], lines={(d['file'], n): (3, 0, 0) for d in source for n in d['lines'] if n != 11},
                                    unreachable=entries)
                self.assertEqual(report['methods'][0]['validation'], 'uncovered')
                self.assertEqual([e['problem'] for e in report['stale_unreachable']], ['document path mismatch'])
                self.assertFalse(report['complete'])

    def test_executed_listed_line_fails(self):
        entries = self.unreachable(self.entry('src/T.cs:10'))
        report = self.check(unreachable=entries)
        self.assertFalse(report['complete'])
        self.assertEqual(report['methods'][0]['validation'], 'compared')
        self.assertEqual([(e['key'], e['line'], e['problem']) for e in report['stale_unreachable']],
                         [('Quire.T::private()', 'src/T.cs:10', 'executed')])

    def test_stale_accepted_entries_reported_and_incomplete(self):
        text = self.entry()
        entries = self.unreachable(text.replace('Quire.T::private()', 'Quire.Gone::X()'), text.replace(':11', ':99'))
        report = self.check(unreachable=entries)
        self.assertFalse(report['complete'])
        self.assertEqual([(e['key'], e['problem']) for e in report['stale_unreachable']],
                         [('Quire.Gone::X()', 'no such method'), ('Quire.T::private()', 'not a source line of the method')])
        # A method outside a scoped run's types still exists.
        (self.repo / 'src/U.cs').write_bytes((self.repo / 'src/T.cs').read_bytes())
        other = dict(self.method, type='Quire.U', token=0x06000009,
                     source=[self.doc([10, 11], (self.repo / 'src/U.cs').as_posix(), 'src/U.cs')])
        entries = self.unreachable(self.entry('src/U.cs:11', key='Quire.U::private()', methods=[other]))
        stale = self.check(methods=[self.method, other], types=['Quire.T'], lines={(self.file, 10): (3, 0, 0)},
                           unreachable=entries)['stale_unreachable']
        self.assertEqual(stale, [])

    def test_line_shared_with_another_method_is_not_exempt(self):
        lambda_ = dict(self.method, signature='<private>b__0_0()', token=0x06000005, exclusion='compiler-generated',
                       source=[self.doc([11])])
        entries = self.unreachable(self.entry())
        report = self.check(methods=[self.method, lambda_], lines={(self.file, 10): (3, 0, 0)}, unreachable=entries)
        self.assertEqual(report['methods'][0]['validation'], 'uncovered')
        self.assertEqual([e['problem'] for e in report['stale_unreachable']], ['line shared with Quire.T::<private>b__0_0()'])
        self.assertFalse(report['complete'])
        with self.assertRaises(ValueError):
            self.entry(methods=[self.method, lambda_])

    def test_partial_branch_keeps_method_uncovered(self):
        method = dict(self.method, source=[self.doc([10, 11, 12])])
        lines = {**self.lines, (self.file, 11): (3, 1, 2)}
        entries = self.unreachable(self.entry('src/T.cs:12', methods=[method]))
        report = self.check(methods=[method], lines=lines, unreachable=entries)
        self.assertEqual(report['methods'][0]['validation'], 'uncovered')
        self.assertEqual(report['methods'][0]['exempt_lines'], [f'{self.file}:12'])
        self.assertIn('11 (1/2)', report['methods'][0]['reason'])

    def test_proposed_lines_exempt_but_leave_run_incomplete(self):
        lines = {(self.file, 10): (3, 0, 0)}
        report = self.check(lines=lines, unreachable={}, proposed=self.unreachable(self.entry()))
        self.assertEqual(report['methods'][0]['validation'], 'compared')
        self.assertEqual(report['methods'][0]['proposed_exempt_lines'], [f'{self.file}:11'])
        self.assertFalse(report['complete'])
        stale = self.check(unreachable={}, proposed=self.unreachable(self.entry('src/T.cs:10')))['stale_unreachable']
        self.assertEqual([(e['source'], e['problem']) for e in stale], [('proposed', 'executed')])

    def test_methods_compared_through_exemptions_are_mutated(self):
        method = dict(self.method, mutants=[dict(il_offset=0, description='nop')])
        lines = {(self.file, 10): (3, 0, 0)}
        entries = self.unreachable(self.entry())
        self.assertEqual(select(self.check(methods=[method], lines=lines, unreachable={}), {'Quire.T'}), [])
        chosen = select(self.check(methods=[method], lines=lines, unreachable=entries), {'Quire.T'})
        self.assertEqual([(m['signature'], mutant['description']) for m, mutant in chosen], [('private()', 'nop')])

    def test_entry_command_prints_a_hashed_entry(self):
        import json, os, subprocess, sys
        report = self.repo / 'method-coverage.json'
        report.write_text(json.dumps({'methods': [self.method]}))
        result = subprocess.run([sys.executable, str(Path(method_coverage.__file__)), 'unreachable-entry', str(report),
                                 'Quire.T::private()', 'src/T.cs:11', 'return after a throwing helper'],
                                env=dict(os.environ, QUIRE_VALIDATOR_REPO=str(self.repo)), capture_output=True, text=True)
        self.assertEqual(result.returncode, 0, result.stderr)
        self.assertEqual(result.stdout.strip(), self.entry())

    def test_older_inventory_schema_rejected(self):
        with self.assertRaises(ValueError):
            audit(dict(schema=1, errors=[], methods=[self.method]), [self.record], {'Tests.Compare'}, 'fresh', self.repo)


if __name__ == '__main__':
    unittest.main()
