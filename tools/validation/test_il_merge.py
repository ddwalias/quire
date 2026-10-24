"""Exercise gate decisions in a disposable Git repository; no actual build or push."""
import argparse
import json
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest import mock

import il_merge


class GateRepo(unittest.TestCase):
    """main with src/P, a work branch, and il_merge.ROOT pointed at the repository."""
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory(prefix='quire-il-gate-test-')
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.git('init', '-q', '-b', 'main')
        self.git('config', 'user.name', 'Gate test')
        self.git('config', 'user.email', 'gate-test@example.invalid')
        self.git('config', 'core.hooksPath', '')
        (self.root / '.gitignore').write_text('.mirror-cache/\n')
        (self.root / 'README.md').write_text('base\n')
        path = self.root / 'src/P/P.csproj'
        path.parent.mkdir(parents=True)
        path.write_text('<Project />\n')
        self.git('add', '.')
        self.git('commit', '-qm', 'base')
        self.base = self.git('rev-parse', 'HEAD')
        self.git('checkout', '-qb', 'work')
        self.patch = mock.patch.object(il_merge, 'ROOT', self.root)
        self.patch.start()
        self.addCleanup(self.patch.stop)

    def git(self, *args):
        return subprocess.run(['git', *args], cwd=self.root, check=True, capture_output=True, text=True).stdout.strip()

    def candidate(self, runtime=False):
        path = self.root / ('src/P/T.cs' if runtime else 'README.md')
        path.write_text('candidate\n')
        self.git('add', '.')
        self.git('commit', '-qm', 'candidate')
        tip = self.git('rev-parse', 'HEAD')
        self.git('checkout', '-q', 'main')
        return tip

    def args(self):
        return argparse.Namespace(branch='work', base='main', apply=True, project=[], type=[], method=[],
                                  results=self.root / '.mirror-cache/result', dotnet='dotnet', timeout=20)


class IlMergeTests(GateRepo):
    def test_prose_only_lands_without_building(self):
        tip = self.candidate()
        # Ignore receipts just as the real repo does.
        with mock.patch.object(il_merge.release_il, 'check') as check:
            self.assertEqual(il_merge.gate(self.args()), 0)
            check.assert_not_called()
        self.assertEqual(self.git('rev-parse', 'main'), tip)

    def test_il_failure_does_not_move_main(self):
        self.candidate(runtime=True)
        with mock.patch.object(il_merge.release_il, 'check', return_value={'passed': False, 'failures': []}) as check:
            self.assertEqual(il_merge.gate(self.args()), 1)
            self.assertEqual(check.call_args.kwargs['root'].name, 'tree')
            self.assertNotEqual(check.call_args.kwargs['root'], self.root)
        self.assertEqual(self.git('rev-parse', 'main'), self.base)

    def test_a_moved_candidate_is_not_landed(self):
        self.candidate(runtime=True)
        def move_candidate(*args, **kwargs):
            self.git('update-ref', 'refs/heads/work', self.base)
            return {'passed': True, 'checked': []}
        with mock.patch.object(il_merge.release_il, 'check', side_effect=move_candidate):
            with self.assertRaisesRegex(RuntimeError, 'moved during validation'):
                il_merge.gate(self.args())
        self.assertEqual(self.git('rev-parse', 'main'), self.base)

    def test_a_check_against_a_full_ref_never_applies(self):
        self.candidate(runtime=True)
        args = self.args()
        args.base = 'refs/heads/main'
        with self.assertRaisesRegex(RuntimeError, 'branch name'):
            il_merge.gate(args)
        args.apply, args.results = False, self.root / '.mirror-cache/ref'
        with mock.patch.object(il_merge.release_il, 'check', return_value={'passed': True, 'checked': []}):
            self.assertEqual(il_merge.gate(args), 0)
        self.assertEqual(self.git('rev-parse', 'main'), self.base)


class NativeGateTests(GateRepo):
    """Native projects run native_gate after the IL check; a missing native_gate fails closed."""

    def native_candidate(self, path='src/N/n.cpp'):
        self.git('checkout', '-q', 'main')
        (self.root / 'src/N').mkdir(parents=True)
        (self.root / 'src/N/N.vcxproj').write_text('<Project />\n')
        (self.root / 'src/M').mkdir(parents=True)
        (self.root / 'src/M/M.vcxproj').write_text('<Project />\n')
        self.git('add', '.')
        self.git('commit', '-qm', 'native')
        self.base = self.git('rev-parse', 'HEAD')
        self.git('checkout', '-q', 'work')
        self.git('-c', 'core.hooksPath=', 'merge', '-q', '--ff-only', 'main')
        (self.root / path).parent.mkdir(parents=True, exist_ok=True)
        (self.root / path).write_text('candidate\n')
        self.git('add', '.')
        self.git('commit', '-qm', 'candidate')
        tip = self.git('rev-parse', 'HEAD')
        self.git('checkout', '-q', 'main')
        return tip

    def fake(self, passed):
        module = mock.Mock(spec=['native_projects', 'check'])
        module.native_projects.side_effect = il_merge.native_projects
        module.check.return_value = {'passed': passed, 'checked': [], 'failures': []}
        return mock.patch.dict('sys.modules', {'native_gate': module}), module

    def test_affected_native_projects_are_checked_after_il(self):
        tip = self.native_candidate()
        patch, module = self.fake(True)
        with patch, mock.patch.object(il_merge.release_il, 'check') as check:
            self.assertEqual(il_merge.gate(self.args()), 0)
            check.assert_not_called()  # no managed project changed
        base, projects, results, root = module.check.call_args.args
        self.assertEqual((base, projects, results.name, root.name), (self.base, ['N'], 'native', 'tree'))
        self.assertEqual(self.git('rev-parse', 'main'), tip)

    def test_a_native_failure_blocks(self):
        self.native_candidate()
        patch, module = self.fake(False)
        with patch:
            self.assertEqual(il_merge.gate(self.args()), 1)
        self.assertEqual(self.git('rev-parse', 'main'), self.base)

    def test_shared_native_inputs_select_every_native_project(self):
        self.native_candidate('docs/mirror/native/internal-targets.json')
        patch, module = self.fake(True)
        with patch:
            self.assertEqual(il_merge.gate(self.args()), 0)
        self.assertEqual(module.check.call_args.args[1], ['M', 'N'])
        self.assertEqual(il_merge.affected_native(['src/P/x.cs', 'src/N/a.h'], {'N', 'M'}), ['N'])
        self.assertEqual(il_merge.affected_native(['tools/validation/native_bytes.py'], {'N', 'M'}), ['M', 'N'])

    def test_a_missing_native_gate_fails_closed(self):
        self.native_candidate()
        with mock.patch.dict('sys.modules', {'native_gate': None}):
            self.assertIsNone(il_merge.native_gate())
            self.assertEqual(il_merge.gate(self.args()), 1)
        record = json.loads((self.root / '.mirror-cache/result/gate.json').read_text())
        self.assertIn('native_gate.py is missing', record['native_validation']['error'])
        self.assertEqual(self.git('rev-parse', 'main'), self.base)

    def test_unaffected_native_projects_need_no_native_gate(self):
        self.native_candidate('README.md')
        with mock.patch.dict('sys.modules', {'native_gate': None}):
            self.assertEqual(il_merge.gate(self.args()), 0)


if __name__ == '__main__':
    unittest.main()
