"""lane_merge's optimistic validation and its landing paths, in disposable repositories; no build, real pushes to a
local bare origin."""
import json
import os
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest import mock

import il_merge
import lane_merge


def git(cwd, *args):
    return subprocess.run(['git', *args], cwd=cwd, check=True, capture_output=True, text=True).stdout.strip()


class LaneMergeTests(unittest.TestCase):
    def setUp(self):
        temp = tempfile.TemporaryDirectory(prefix='quire-lane-merge-test-')
        self.addCleanup(temp.cleanup)
        top = Path(temp.name)
        self.origin, self.main, self.lane, self.other = top / 'origin.git', top / 'main', top / 'lane', top / 'other'
        git(top, 'init', '-q', '--bare', '-b', 'main', str(self.origin))
        git(top, 'clone', '-q', str(self.origin), str(self.main))
        for key, value in (('user.name', 'T'), ('user.email', 't@x.invalid'), ('core.hooksPath', '')):
            git(self.main, 'config', key, value)
        self.write(self.main, '.gitignore', '.mirror-cache/\n')
        self.write(self.main, 'README.md', 'base\n')
        self.write(self.main, 'src/P/P.csproj', '<Project />\n')
        self.write(self.main, 'src/P/T.cs', 'base\n')
        self.write(self.main, 'src/Q/Q.csproj', '<Project />\n')
        self.commit(self.main, 'base')
        git(self.main, 'push', '-q', 'origin', 'main')
        git(self.main, 'worktree', 'add', '-q', '-b', 'work', str(self.lane))
        git(top, 'clone', '-q', str(self.origin), str(self.other))
        for key, value in (('user.name', 'O'), ('user.email', 'o@x.invalid'), ('core.hooksPath', '')):
            git(self.other, 'config', key, value)
        self.write(self.lane, 'src/P/T.cs', 'candidate\n')
        self.commit(self.lane, 'candidate')
        cwd = os.getcwd()
        os.chdir(self.lane)
        self.addCleanup(os.chdir, cwd)
        for patch in (mock.patch.dict(os.environ, {'QUIRE_MERGE_LOCK': str(top / 'lock')}),
                      mock.patch.object(il_merge, 'ROOT', il_merge.ROOT)):
            patch.start()
            self.addCleanup(patch.stop)
        self.checks = []

    def write(self, root, path, text):
        (root / path).parent.mkdir(parents=True, exist_ok=True)
        (root / path).write_text(text)

    def commit(self, root, message):
        git(root, 'add', '-A')
        git(root, 'commit', '-qm', message)
        return git(root, 'rev-parse', 'HEAD')

    def upstream(self, path, text='upstream\n'):
        """Another lane lands on origin."""
        git(self.other, 'pull', '-q', '--ff-only')
        self.write(self.other, path, text)
        self.commit(self.other, 'upstream')
        git(self.other, 'push', '-q', 'origin', 'main')

    def land(self, during=None):
        def check(base, projects, results, *args, **kwargs):
            self.checks.append(projects)
            if during and len(self.checks) == 1:
                self.upstream(during)
            return {'passed': True, 'checked': [], 'failures': []}
        with mock.patch.object(il_merge.release_il, 'check', side_effect=check):
            code = lane_merge.main(['work', '--main', str(self.main)])
        receipts = sorted((self.main / '.mirror-cache').glob('gate-il-*/gate.json'), key=os.path.getmtime)
        landing = next((json.loads(r.read_text()).get('landing') for r in receipts
                        if 'landing' in json.loads(r.read_text())), None)
        return code, landing

    def test_an_unmoved_main_lands_the_validated_tip(self):
        code, landing = self.land()
        self.assertEqual(code, 0)
        self.assertEqual(self.checks, [['P']])
        self.assertEqual(landing['path'], 'validated')
        self.assertEqual(git(self.origin, 'rev-parse', 'main'), git(self.lane, 'rev-parse', 'work'))

    def test_an_unrelated_upstream_change_lands_without_building_again(self):
        code, landing = self.land(during='README.md')
        self.assertEqual(code, 0)
        self.assertEqual(self.checks, [['P']])
        self.assertEqual(landing['path'], 'rebased-unaffected')
        self.assertEqual(git(self.origin, 'rev-parse', 'main'), git(self.lane, 'rev-parse', 'work'))
        self.assertEqual(git(self.origin, 'show', 'main:README.md'), 'upstream')

    def test_an_upstream_change_to_a_compared_project_is_revalidated(self):
        code, landing = self.land(during='src/P/Other.cs')
        self.assertEqual(code, 0)
        self.assertEqual(self.checks, [['P'], ['P']])
        self.assertEqual(landing['path'], 'revalidated')
        self.assertEqual(git(self.origin, 'rev-parse', 'main'), git(self.lane, 'rev-parse', 'work'))

    def test_an_upstream_move_out_of_a_compared_project_is_revalidated(self):
        git(self.other, 'pull', '-q', '--ff-only')
        git(self.other, 'mv', 'src/P/P.csproj', 'src/Q/Moved.csproj')
        self.commit(self.other, 'move')
        git(self.other, 'push', '-q', 'origin', 'main')
        moved = git(self.other, '-c', 'diff.renames=true', 'diff', '--name-only', 'HEAD~1', 'HEAD').splitlines()
        self.assertNotIn('src/P/P.csproj', moved)  # what a rename-detecting diff would have missed
        self.assertFalse(lane_merge.inputs_untouched(
            lane_merge.git('diff', '--name-only', 'HEAD~1', 'HEAD', cwd=self.other).splitlines(), self.lane, ['P'], []))

    def test_a_lane_copy_runs_mains_gate(self):
        self.write(self.main, 'tools/validation/lane_merge.py', 'import sys\nsys.exit(7)\n')
        self.commit(self.main, 'main gate')
        with mock.patch.dict(os.environ, {lane_merge.REEXEC: ''}):
            self.assertEqual(lane_merge.main(['work', '--main', str(self.main)]), 7)
        self.assertEqual(self.checks, [])

    def test_shared_inputs_are_revalidated(self):
        for path in ('Directory.Build.props', 'docs/mirror/type-map.csv', 'tools/Quire.Tools.MethodAudit/Program.cs'):
            self.assertFalse(lane_merge.inputs_untouched([path], self.lane, ['P'], []), path)
        self.assertTrue(lane_merge.inputs_untouched(['docs/x.md', 'src/Q/Q.cs'], self.lane, ['P'], []))
        self.assertFalse(lane_merge.inputs_untouched(['src/Directory.Build.props'], self.lane, ['P'], []))
        self.assertFalse(lane_merge.inputs_untouched(['docs/mirror/native/x.json'], self.lane, [], ['N']))
        # A project the compared one references, here through its Generated/Mirror.props.
        self.write(self.lane, 'src/P/Generated/Mirror.props', r'<ProjectReference Include="..\Q\Q.csproj" />')
        self.assertFalse(lane_merge.inputs_untouched(['src/Q/Q.cs'], self.lane, ['P'], []))

    def test_a_conflicting_rebase_is_aborted_and_reported(self):
        tip = git(self.lane, 'rev-parse', 'work')
        self.upstream('src/P/T.cs', 'other\n')
        with mock.patch('sys.stderr') as stderr:
            code, _ = self.land()
        self.assertEqual(code, 1)
        message = ''.join(call.args[0] for call in stderr.write.call_args_list)
        self.assertIn('was aborted', message)
        self.assertIn('src/P/T.cs', message)
        self.assertEqual(git(self.lane, 'rev-parse', 'work'), tip)
        self.assertEqual(git(self.lane, 'status', '--porcelain'), '')
        self.assertFalse((Path(git(self.lane, 'rev-parse', '--git-dir')) / 'rebase-merge').exists())
        self.assertEqual(self.checks, [])


if __name__ == '__main__':
    unittest.main()
