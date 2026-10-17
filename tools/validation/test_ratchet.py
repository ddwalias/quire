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
        for change, text in [(dict(compared=2, missing=2), 'compared fell'), (dict(compared=3, missing=3), 'unfinished methods rose'),
                             (dict(compared=3, missing=2, structure_mismatch=1), 'unfinished methods rose from 2 to 3'),
                             (dict(compared=3, missing=2, uncovered=1), 'unfinished methods rose'),
                             (dict(compared=3, missing=2, verified_unsupported=1), 'verified_unsupported rose')]:
            with self.subTest(text=text):
                self.assertIn(text, compare(self.baseline, self.current(**{'Quire.T': counts(**change)}))[0])

    def test_a_type_of_only_excluded_methods_may_disappear(self):
        self.baseline['types']['Quire.T+<>c'] = counts(excluded=3, total=3)
        self.assertEqual(compare(self.baseline, self.current(**{'Quire.T': counts(compared=3, missing=2)})), [])
        self.baseline['types']['Quire.T+<>c'] = counts(excluded=2, missing=1, total=3)
        self.assertEqual(compare(self.baseline, self.current(**{'Quire.T': counts(compared=3, missing=2)})),
                         ['Quire.T+<>c: type disappeared from the inventory'])

    def test_swapped_findings_fail_although_counts_hold(self):
        same = {'Quire.T': counts(compared=3, missing=2)}
        self.assertEqual(compare(self.baseline, self.current(lint=['b.cs | goto statement | goto y;'], **same)),
                         ['new lint: b.cs | goto statement | goto y;'])
        twice = ['a.cs | goto statement | goto x;'] * 2
        self.assertEqual(compare(self.baseline, self.current(lint=twice, **same)), ['new lint: a.cs | goto statement | goto x;'])
        swapped = {'Quire.T::A()': 'missing', 'Quire.T::B()': 'compared'}
        self.assertEqual(compare(self.baseline, self.current(methods=swapped, **same)), ['Quire.T::A(): compared became missing'])

    def test_unreachable_line_exemptions_move_methods_both_ways(self):
        # Accepting an unreachable line upgrades uncovered to compared; removing it again is a backward move.
        uncovered = dict(METHODS, **{'Quire.T::B()': 'uncovered'})
        compared = dict(METHODS, **{'Quire.T::B()': 'compared'})
        baseline = dict(self.baseline, types={'Quire.T': counts(compared=3, uncovered=2)}, methods=uncovered)
        self.assertEqual(compare(baseline, self.current(methods=compared, **{'Quire.T': counts(compared=4, uncovered=1)})), [])
        baseline = dict(self.baseline, types={'Quire.T': counts(compared=4, uncovered=1)}, methods=compared)
        self.assertEqual(compare(baseline, self.current(methods=uncovered, **{'Quire.T': counts(compared=3, uncovered=2)})),
                         ['Quire.T::B(): compared became uncovered', 'Quire.T: compared fell from 4 to 3',
                          'Quire.T: unfinished methods rose from 1 to 2'])

    def test_methods_may_move_forward_between_unfinished_stages_but_not_back(self):
        same = {'Quire.T': counts(compared=3, missing=2)}
        stages = lambda b: dict(METHODS, **{'Quire.T::B()': b})
        for old, new in [('missing', 'structure_mismatch'), ('structure_mismatch', 'untested'), ('untested', 'uncovered'),
                         ('missing', 'uncovered'), ('no_counterpart', 'missing'), ('untested', 'unchecked_state'),
                         ('structure_mismatch', 'unchecked_state'), ('unchecked_state', 'uncovered')]:
            with self.subTest(old=old, new=new):
                baseline = dict(self.baseline, methods=stages(old))
                self.assertEqual(compare(baseline, self.current(methods=stages(new), **same)), [])
                self.assertEqual(compare(dict(self.baseline, methods=stages(new)), self.current(methods=stages(old), **same)),
                                 [f'Quire.T::B(): {new} became {old}'])
        for old, new in [('missing', 'excluded'), ('missing', 'declaration_only'), ('declaration_only', 'untested'),
                         ('excluded', 'missing'), ('compared', 'untested')]:
            with self.subTest(old=old, new=new):
                self.assertEqual(compare(dict(self.baseline, methods=stages(old)), self.current(methods=stages(new), **same)),
                                 [f'Quire.T::B(): {old} became {new}'])
        # Fixing a structure mismatch moves it to the untested 'missing' bucket: the type's unfinished total holds.
        baseline = dict(self.baseline, types={'Quire.T': counts(compared=3, missing=1, structure_mismatch=1)})
        self.assertEqual(compare(baseline, self.current(**same)), [])

    def test_new_method_with_a_gap_is_not_paid_for_by_progress_elsewhere(self):
        better = {'Quire.T': counts(compared=4, missing=1, structure_mismatch=1)}
        for gap in ('structure_mismatch', 'unchecked_state', 'uncovered'):
            with self.subTest(gap=gap):
                methods = {'Quire.T::A()': 'compared', 'Quire.T::B()': 'compared', 'Quire.T::C()': gap}
                self.assertEqual(compare(self.baseline, self.current(methods=methods, **better)),
                                 [f'Quire.T::C(): new method is {gap}'])
        # A new stub or untested method is still governed by the type's unfinished total; new types start fresh.
        stub = {'Quire.T::A()': 'compared', 'Quire.T::B()': 'compared', 'Quire.T::C()': 'missing'}
        self.assertEqual(compare(self.baseline, self.current(methods=stub, **{'Quire.T': counts(compared=4, missing=2)})), [])
        fresh = dict(METHODS, **{'Quire.New::C()': 'structure_mismatch'})
        self.assertEqual(compare(self.baseline, self.current(methods=fresh, **{'Quire.T': counts(compared=3, missing=2),
                                                                                 'Quire.New': counts(structure_mismatch=1)})), [])

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
        # Schema 3 lacks only the body hashes, which narrow mutation; the ratchet reads it unchanged.
        self.assertEqual(compare(dict(self.baseline, schema=3), self.current(**same)), [])

    def test_snapshot_records_body_hashes(self):
        coverage = {'by_type': {}, 'methods': [
            {'type': 'Quire.T', 'signature': 'A()', 'validation': 'compared', 'original': {}, 'body_sha256': 'ab'},
            {'type': 'Quire.T', 'signature': 'B()', 'validation': 'missing', 'original': {}, 'body_sha256': None}]}
        snapshot = check(coverage, {'lint': []}, False, 'focused', baseline=None)['snapshot']
        self.assertEqual((snapshot['schema'], snapshot['bodies']), (SCHEMA, {'Quire.T::A()': 'ab'}))

    def test_exact_il_matches_may_not_be_lost(self):
        same = {'Quire.T': counts(compared=3, missing=2)}
        baseline = dict(self.baseline, exact=['Quire.T::A()'])
        self.assertEqual(compare(baseline, dict(self.current(**same), exact=['Quire.T::A()', 'Quire.T::B()'])), [])
        self.assertEqual(compare(baseline, dict(self.current(**same), exact=[])),
                         ["Quire.T::A(): IL no longer matches Mathcad's exactly"])
        # A new exact method is welcome; a new inexact one is not held (it never matched).
        fresh = dict(METHODS, **{'Quire.T::C()': 'missing'})
        self.assertEqual(compare(baseline, dict(self.current(methods=fresh, **{'Quire.T': counts(compared=3, missing=2)}),
                                                exact=['Quire.T::A()'])), [])
        # A method that disappeared or was renamed is reported as such, not as a lost match; a scoped run skips it.
        gone = {'Quire.T::B()': 'missing'}
        self.assertEqual(compare(baseline, dict(self.current(methods=gone, **same), exact=[])),
                         ['Quire.T::A(): method disappeared from the inventory'])
        self.assertEqual(compare(baseline, dict(self.current(methods=gone, **same), exact=[]), scoped=True), [])
        # One that is no longer compared says why.
        excluded = dict(METHODS, **{'Quire.T::A()': 'excluded'})
        self.assertIn("Quire.T::A(): matched Mathcad's IL exactly but is now excluded, so its IL is not compared",
                      compare(baseline, dict(self.current(methods=excluded, **same), exact=[])))

    def test_baseline_without_exact_data_holds_no_exact_matches(self):
        same = {'Quire.T': counts(compared=3, missing=2)}
        for schema in (3, 4):
            with self.subTest(schema=schema):
                self.assertEqual(compare(dict(self.baseline, schema=schema), dict(self.current(**same), exact=[])), [])

    def test_snapshot_records_exact_matches(self):
        coverage = {'by_type': {}, 'methods': [
            {'type': 'Quire.T', 'signature': 'A()', 'validation': 'compared', 'original': {}, 'exact_match': True},
            {'type': 'Quire.T', 'signature': 'B()', 'validation': 'compared', 'original': {}, 'exact_match': False},
            {'type': 'Quire.T', 'signature': 'C()', 'validation': 'missing', 'original': {}, 'exact_match': None},
            {'type': 'Quire.T', 'signature': 'D()', 'validation': 'missing', 'original': {}}]}
        self.assertEqual(check(coverage, {'lint': []}, False, 'focused', baseline=None)['snapshot']['exact'], ['Quire.T::A()'])

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
            untested = {'type': 'Quire.T', 'signature': 'C()', 'validation': 'missing', 'stage': 'untested',
                        'original': {'token': 3}, 'cases': []}
            snapshot = check(dict(coverage, methods=coverage['methods'] + [lambda_, invented, untested]), integrity, False,
                             'focused', Path(directory) / 'none.json')['snapshot']
            self.assertEqual([snapshot['methods'][k] for k in ('Quire.T+<>c::<A>b__0_0()', 'Quire.T::B()', 'Quire.T::C()')],
                             ['excluded', 'no_counterpart', 'untested'])
            path = Path(directory) / 'baseline.json'
            path.write_text(json.dumps(report['snapshot']))
            self.assertTrue(check(coverage, integrity, False, 'focused', path)['ok'])


class PhaseKeyTests(unittest.TestCase):
    """The phase inventory spans every manifest project, and one type name can be compiled into several (shared source
    such as ProductInfo.Constants, shared native headers, compiler helpers): its baseline keys carry the project."""
    def coverage(self, second='compared'):
        def row(project, validation, exact):
            return {'project': project, 'type': 'Quire.ProductInfo.Constants', 'signature': '.ctor`0()->Void|False',
                    'validation': validation, 'original': {'token': 1}, 'body_sha256': project, 'exact_match': exact,
                    'status_supported': True, 'cases': []}
        rows = [row('Quire.Calculation', 'compared', True), row('Quire.Controls', second, second == 'compared')]
        return {'by_type': {}, 'methods': rows}

    def test_methods_and_types_of_one_name_in_several_projects_stay_apart(self):
        snapshot = check(self.coverage(), {'lint': []}, False, 'phase', baseline=None)['snapshot']
        keys = ['Quire.Calculation|Quire.ProductInfo.Constants::.ctor`0()->Void|False',
                'Quire.Controls|Quire.ProductInfo.Constants::.ctor`0()->Void|False']
        self.assertEqual(sorted(snapshot['methods']), keys)
        self.assertEqual(snapshot['exact'], keys)
        self.assertEqual(sorted(snapshot['bodies'].values()), ['Quire.Calculation', 'Quire.Controls'])
        self.assertEqual(sorted(snapshot['types']), ['Quire.Calculation|Quire.ProductInfo.Constants',
                                                     'Quire.Controls|Quire.ProductInfo.Constants'])
        self.assertEqual(snapshot['types']['Quire.Controls|Quire.ProductInfo.Constants']['compared'], 1)

    def test_a_regression_in_one_project_is_not_hidden_by_another(self):
        baseline = check(self.coverage(), {'lint': []}, False, 'phase', baseline=None)['snapshot']
        regressions = check(self.coverage(second='missing'), {'lint': []}, False, 'phase', baseline=baseline)['regressions']
        self.assertIn('Quire.Controls|Quire.ProductInfo.Constants::.ctor`0()->Void|False: compared became missing', regressions)
        self.assertIn("Quire.Controls|Quire.ProductInfo.Constants::.ctor`0()->Void|False: IL no longer matches Mathcad's exactly",
                      regressions)
        self.assertFalse(any(r.startswith('Quire.Calculation|') for r in regressions), regressions)

    def test_the_focused_baseline_keeps_its_keys(self):
        coverage = self.coverage()
        coverage['methods'] = coverage['methods'][:1]
        snapshot = check(coverage, {'lint': []}, False, 'focused', baseline=None)['snapshot']
        self.assertEqual(list(snapshot['methods']), ['Quire.ProductInfo.Constants::.ctor`0()->Void|False'])

if __name__ == '__main__':
    unittest.main()
