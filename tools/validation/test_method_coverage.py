import copy
import hashlib
import tempfile
from pathlib import Path
import unittest
from method_coverage import audit


class CoverageTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()
        self.addCleanup(self.directory.cleanup)
        self.repo = Path(self.directory.name)
        evidence = self.repo / 'docs/mirror/evidence'
        evidence.mkdir(parents=True)
        contract = b'Original hash original-sha MVID original-mvid; private method token 0x06000002'
        (evidence / 'Contract.md').write_bytes(contract)
        self.method = dict(project='Quire.Mpl', type='Quire.T', signature='private()', token=0x06000001,
                           access='Private', status='Verified', is_static=True, is_constructor=False, quire_sha256='quire-sha', quire_mvid='quire-mvid',
                           original=dict(assembly='mplrun', sha256='original-sha', mvid='original-mvid', token=0x06000002),
                           declaration_only=False, exclusion=None, shape_matches=True)
        self.record = dict(schema=1, run='fresh', test='Tests.Compare', case_id='edge case', inputs=[1], outcome='1', arguments_after=[1],
                           contract='Contract', contract_sha256=hashlib.sha256(contract).hexdigest(),
                           quire=dict(assembly='Quire.Mpl', sha256='quire-sha', mvid='quire-mvid', token=0x06000001),
                           original=self.method['original'])

    def check(self, methods=None, records=None, tests=None, types=()):
        return audit(dict(schema=1, errors=[], methods=methods if methods is not None else [self.method]),
                     records if records is not None else [self.record], tests if tests is not None else {'Tests.Compare'},
                     'fresh', self.repo, types)

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
        self.assertFalse(self.check(methods=[method])['complete'])
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


if __name__ == '__main__':
    unittest.main()
