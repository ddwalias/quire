import json
import tempfile
from pathlib import Path
import unittest
from ratchet import check, compare

ZERO = dict.fromkeys(('compared', 'declaration_only', 'excluded', 'missing', 'structure_mismatch', 'uncovered', 'verified_unsupported'), 0)


def counts(**changes):
    return dict(ZERO, **changes)


class RatchetTests(unittest.TestCase):
    def setUp(self):
        self.baseline = {'types': {'Quire.T': counts(compared=3, missing=2)}, 'lint': 1, 'listing_errors': 4}

    def current(self, lint=1, listing_errors=4, **types):
        return {'types': types, 'lint': lint, 'listing_errors': listing_errors}

    def test_improvement_and_equal_pass(self):
        self.assertEqual(compare(self.baseline, self.current(**{'Quire.T': counts(compared=5)}, lint=0)), [])
        self.assertEqual(compare(self.baseline, self.current(**{'Quire.T': counts(compared=3, missing=2)})), [])

    def test_each_regression_reported(self):
        for change, text in [(dict(compared=2, missing=2), 'compared fell'), (dict(compared=3, missing=3), 'missing rose'),
                             (dict(compared=3, missing=2, structure_mismatch=1), 'structure_mismatch rose'),
                             (dict(compared=3, missing=2, uncovered=1), 'uncovered rose'),
                             (dict(compared=3, missing=2, verified_unsupported=1), 'verified_unsupported rose')]:
            with self.subTest(text=text):
                self.assertIn(text, compare(self.baseline, self.current(**{'Quire.T': counts(**change)}))[0])
        self.assertIn('lint rose', compare(self.baseline, self.current(**{'Quire.T': counts(compared=3, missing=2)}, lint=2))[0])

    def test_removed_type_fails_full_run_but_not_scoped_run(self):
        self.assertIn('disappeared', compare(self.baseline, self.current())[0])
        self.assertEqual(compare(self.baseline, self.current(), scoped=True), [])

    def test_new_type_starts_its_own_baseline(self):
        current = self.current(**{'Quire.T': counts(compared=3, missing=2), 'Quire.New': counts(missing=9)})
        self.assertEqual(compare(self.baseline, current), [])

    def test_missing_baseline_fails(self):
        with tempfile.TemporaryDirectory() as directory:
            coverage = {'by_type': {'Quire.T': counts()}}
            report = check(coverage, {'lint': [], 'listings': {'errors': []}}, False, Path(directory) / 'none.json')
            self.assertFalse(report['ok'])
            path = Path(directory) / 'baseline.json'
            path.write_text(json.dumps(report['snapshot']))
            self.assertTrue(check(coverage, {'lint': [], 'listings': {'errors': []}}, False, path)['ok'])


if __name__ == '__main__':
    unittest.main()
