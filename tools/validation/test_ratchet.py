import json
import tempfile
from pathlib import Path
import unittest
from ratchet import SCHEMA, check, compare

ZERO = dict.fromkeys(('total', 'compared', 'declaration_only', 'excluded', 'missing', 'structure_mismatch', 'uncovered', 'verified_unsupported'), 0)
METHODS = {'Quire.T::A()': 'compared', 'Quire.T::B()': 'missing'}


def counts(**changes):
    return dict(ZERO, **changes)


class RatchetTests(unittest.TestCase):
    def setUp(self):
        self.baseline = {'schema': SCHEMA, 'profile': 'focused', 'types': {'Quire.T': counts(compared=3, missing=2)},
                         'methods': METHODS, 'lint': ['a.cs | goto statement | goto x;']}

    def current(self, lint=('a.cs | goto statement | goto x;',), profile='focused', methods=None, **types):
        return {'profile': profile, 'types': types, 'methods': METHODS if methods is None else methods,
                'lint': list(lint)}

    def test_improvement_and_equal_pass(self):
        better = dict(METHODS, **{'Quire.T::B()': 'compared'})
        self.assertEqual(compare(self.baseline, self.current(**{'Quire.T': counts(compared=5)}, lint=(), methods=better)), [])
        self.assertEqual(compare(self.baseline, self.current(**{'Quire.T': counts(compared=3, missing=2)})), [])

    def test_each_regression_reported(self):
        for change, text in [(dict(compared=2, missing=2), 'compared fell'), (dict(compared=3, missing=3), 'missing rose'),
                             (dict(compared=3, missing=2, structure_mismatch=1), 'structure_mismatch rose'),
                             (dict(compared=3, missing=2, uncovered=1), 'uncovered rose'),
                             (dict(compared=3, missing=2, verified_unsupported=1), 'verified_unsupported rose')]:
            with self.subTest(text=text):
                self.assertIn(text, compare(self.baseline, self.current(**{'Quire.T': counts(**change)}))[0])

    def test_swapped_findings_fail_although_counts_hold(self):
        same = {'Quire.T': counts(compared=3, missing=2)}
        self.assertEqual(compare(self.baseline, self.current(lint=['b.cs | goto statement | goto y;'], **same)),
                         ['new lint: b.cs | goto statement | goto y;'])
        twice = ['a.cs | goto statement | goto x;'] * 2
        self.assertEqual(compare(self.baseline, self.current(lint=twice, **same)), ['new lint: a.cs | goto statement | goto x;'])
        swapped = {'Quire.T::A()': 'missing', 'Quire.T::B()': 'compared'}
        self.assertEqual(compare(self.baseline, self.current(methods=swapped, **same)), ['Quire.T::A(): compared became missing'])

    def test_deleted_methods_fail_but_lambdas_may_come_and_go(self):
        baseline = dict(self.baseline, types={'Quire.T': counts(total=5, compared=3, missing=2)})
        self.assertIn('total fell', compare(baseline, self.current(**{'Quire.T': counts(total=4, compared=3, missing=1)}))[0])
        lambdas = dict(METHODS, **{'Quire.T+<>c::<A>b__0_0()': 'excluded'})
        self.assertEqual(compare(self.baseline, self.current(methods=lambdas, **{'Quire.T': counts(total=1, compared=3, missing=2, excluded=1)})), [])
        before = dict(self.baseline, methods=lambdas, types={'Quire.T': counts(total=6, compared=3, missing=2, excluded=1)})
        self.assertEqual(compare(before, self.current(**{'Quire.T': counts(total=5, compared=3, missing=2)})), [])
        gone = {'Quire.T::A()': 'compared'}
        self.assertIn('Quire.T::B(): method disappeared from the inventory', compare(self.baseline, self.current(methods=gone, **{'Quire.T': counts(compared=3, missing=2)})))

    def test_baseline_from_another_profile_or_schema_rejected(self):
        same = {'Quire.T': counts(compared=3, missing=2)}
        self.assertIn('profile', compare(self.baseline, self.current(profile='phase', **same))[0])
        self.assertIn('schema', compare(dict(self.baseline, schema=1), self.current(**same))[0])

    def test_removed_type_fails_full_run_but_not_scoped_run(self):
        self.assertIn('disappeared', compare(self.baseline, self.current(methods={}))[0])
        self.assertEqual(compare(self.baseline, self.current(methods={}), scoped=True), [])

    def test_new_type_starts_its_own_baseline_unless_it_has_no_counterpart(self):
        types = {'Quire.T': counts(compared=3, missing=2), 'Quire.New': counts(missing=1)}
        self.assertEqual(compare(self.baseline, self.current(methods=dict(METHODS, **{'Quire.New::C()': 'missing'}), **types)), [])
        invented = dict(METHODS, **{'Quire.New::C()': 'no_counterpart'})
        self.assertEqual(compare(self.baseline, self.current(methods=invented, **types)),
                         ['Quire.New::C(): new method has no Mathcad counterpart'])

    def test_missing_baseline_fails(self):
        with tempfile.TemporaryDirectory() as directory:
            coverage = {'by_type': {'Quire.T': counts()}, 'methods': [
                {'type': 'Quire.T', 'signature': 'A()', 'validation': 'compared', 'original': {'token': 1}}]}
            integrity = {'lint': []}
            report = check(coverage, integrity, False, 'focused', Path(directory) / 'none.json')
            self.assertFalse(report['ok'])
            self.assertEqual(report['snapshot']['methods'], {'Quire.T::A()': 'compared'})
            lambda_ = {'type': 'Quire.T+<>c', 'signature': '<A>b__0_0()', 'validation': 'excluded', 'cases': []}
            invented = {'type': 'Quire.T', 'signature': 'B()', 'validation': 'missing', 'cases': []}
            snapshot = check(dict(coverage, methods=coverage['methods'] + [lambda_, invented]), integrity, False,
                             'focused', Path(directory) / 'none.json')['snapshot']
            self.assertEqual((snapshot['methods']['Quire.T+<>c::<A>b__0_0()'], snapshot['methods']['Quire.T::B()']),
                             ('excluded', 'no_counterpart'))
            path = Path(directory) / 'baseline.json'
            path.write_text(json.dumps(report['snapshot']))
            self.assertTrue(check(coverage, integrity, False, 'focused', path)['ok'])


if __name__ == '__main__':
    unittest.main()
