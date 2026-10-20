"""Exercise gate decisions in a disposable Git repository; no actual build or push."""
import argparse
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest import mock

import il_merge


class IlMergeTests(unittest.TestCase):
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


if __name__ == '__main__':
    unittest.main()
