import contextlib
import io
import os
import tempfile
import time
import unittest
from pathlib import Path
from unittest import mock
import sweep


class SweepTests(unittest.TestCase):
    def setUp(self):
        directory = tempfile.TemporaryDirectory()
        self.addCleanup(directory.cleanup)
        self.root = Path(directory.name)
        old = time.time() - 13 * 3600
        def made(name, *, age=old, inner=None):
            path = self.root / name
            path.mkdir()
            (path / 'file').write_text('x')
            for item in (path / 'file', path):
                os.utime(item, (age, age))
            if inner is not None:  # a file rewritten deep inside: the directory's own time does not change
                (path / 'sub').mkdir()
                (path / 'sub/live').write_text('y')
                os.utime(path / 'sub/live', (inner, inner))
                os.utime(path / 'sub', (old, old))
                os.utime(path, (old, old))
            return path
        self.stale = [made('quire-validation-a'), made('quire-coverage-b'), made('quire-audit-release-c'),
                      made('quire-gate-results-d')]
        made('quire-gate-e', inner=time.time())       # a running gate still writes beneath it
        made('quire-validation-f', age=time.time())   # fresh
        made('quire-mutant-g')                        # mutate.sweep_stale's, not ours
        (self.root / 'quire-audit-file').write_text('not a directory')
        os.utime(self.root / 'quire-audit-file', (old, old))

    def test_only_directories_unchanged_for_twelve_hours_throughout(self):
        self.assertEqual(sorted(sweep.stale(self.root)), sorted(self.stale))

    def test_main_removes_them_and_dry_run_only_lists(self):
        with mock.patch.object(sweep.tempfile, 'gettempdir', lambda: str(self.root)):
            out = io.StringIO()
            with contextlib.redirect_stdout(out):
                self.assertEqual(sweep.main(['--dry-run']), 0)
            self.assertEqual(out.getvalue().count('would remove: '), 4)
            self.assertTrue(all(p.exists() for p in self.stale))
            with contextlib.redirect_stdout(io.StringIO()):
                sweep.main([])
        self.assertFalse(any(p.exists() for p in self.stale))
        self.assertEqual(sorted(p.name for p in self.root.iterdir()),
                         ['quire-audit-file', 'quire-gate-e', 'quire-mutant-g', 'quire-validation-f'])


if __name__ == '__main__':
    unittest.main()
