import subprocess
import unittest
from pathlib import Path
from unittest import mock
import il_diff
from release_il import mathcad_install


class IlDiffTests(unittest.TestCase):
    def test_runs_method_audit_in_diff_mode_without_an_inventory(self):
        calls = []
        def run(command, **kwargs):
            calls.append(command)
            return subprocess.CompletedProcess(command, 0, '', '')
        with mock.patch.object(il_diff.subprocess, 'run', run):
            self.assertEqual(il_diff.main(['Quire.Mcd.MatDim', 'Quire.Mcd.Dim::Div']), 0)
        builds, scan = calls[:-1], calls[-1]
        self.assertEqual([Path(c[2]).name for c in builds], ['Quire.Tools.MethodAudit.csproj',
                         'Quire.Mpl.csproj'])
        for build in builds:
            self.assertEqual(build[build.index('-c') + 1], 'Release')
        self.assertNotIn('Debug', str(calls))
        self.assertIn('--release-il', scan)
        self.assertNotIn('--output', scan)
        self.assertEqual(scan[-4:], ['--diff', 'Quire.Mcd.MatDim', '--diff', 'Quire.Mcd.Dim::Div'])
        self.assertEqual(scan[scan.index('--project') + 1], 'Quire.Mpl')

    def test_a_failed_build_stops_before_the_audit(self):
        calls = []
        def run(command, **kwargs):
            calls.append(command)
            return subprocess.CompletedProcess(command, 1, 'error CS0001', '')
        with mock.patch.object(il_diff.subprocess, 'run', run), \
                mock.patch('sys.stderr'):
            self.assertEqual(il_diff.main(['Quire.Mcd.MatDim']), 2)
        self.assertEqual(len(calls), 1)

    def test_mathcad_install_honours_mathcad_dir(self):
        self.assertEqual(mathcad_install({'MATHCAD_DIR': '/opt/mathcad'}), Path('/opt/mathcad'))
        self.assertIn('Mathcad Prime', str(mathcad_install({})))


if __name__ == '__main__':
    unittest.main()
