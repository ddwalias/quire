import unittest

from batch_picker import build, classify, rank_members


class BatchPickerTests(unittest.TestCase):
    def test_classifies_focus_report_states(self):
        self.assertEqual(classify({'validation': 'missing', 'stage': 'untested', 'exact_match': True}),
                         'exact-but-untested')
        self.assertEqual(classify({'validation': 'missing', 'exact_match': False}), 'missing')
        self.assertEqual(classify({'validation': 'uncovered'}), 'uncovered')
        self.assertEqual(classify({'validation': 'compared', 'exact_match': False}), 'structure_mismatch')
        self.assertIsNone(classify({'validation': 'compared', 'exact_match': True}))

    def test_filters_tier_members_and_reports_absent_methods(self):
        tier = ['N.A::A()', 'N.B::B()']
        report = {'methods': [{'type': 'N.A', 'signature': 'A()', 'validation': 'missing',
                               'stage': 'untested', 'exact_match': True, 'project': 'N', 'token': 1}]}
        selected = build(report, tier, {'N.A', 'N.B'})
        self.assertEqual(selected['N.A::A()']['status'], 'exact-but-untested')
        self.assertEqual(selected['N.B::B()']['status'], 'missing')
        self.assertEqual(set(build(report, tier, {'N.A'})), {'N.A::A()'})
        self.assertEqual(set(build(report, tier, set(), ['N.A', 'N.B'])), {'N.A::A()', 'N.B::B()'})

    def test_leaf_callee_precedes_caller(self):
        methods = {
            'N.T::Caller': {'key': 'N.T::Caller', 'type': 'N.T', 'signature': 'Caller',
                            'project': 'N', 'token': 1, 'calls': [2], 'status': 'exact-but-untested'},
            'N.T::Callee': {'key': 'N.T::Callee', 'type': 'N.T', 'signature': 'Callee',
                            'project': 'N', 'token': 2, 'calls': [], 'status': 'missing'},
        }
        self.assertEqual(rank_members(methods), ['N.T::Callee', 'N.T::Caller'])


if __name__ == '__main__':
    unittest.main()
