"""cache_prune keeps every landing receipt and removes only old run folders, in a disposable cache directory."""
import json
import os
from pathlib import Path
import stat
import tempfile
import time
import unittest
from unittest import mock

import cache_prune
import il_merge
import release_il

HOUR = 3600


class CachePruneTests(unittest.TestCase):
    def setUp(self):
        temp = tempfile.TemporaryDirectory(prefix='quire-cache-prune-test-')
        self.addCleanup(temp.cleanup)
        self.cache, self.temp = Path(temp.name) / '.mirror-cache', Path(temp.name) / 'temp'
        self.cache.mkdir()
        self.temp.mkdir()
        self.now = time.time()

    def folder(self, name, age_hours, record=None, parent=None, read_only=False):
        """A run folder with an inventory, last written age_hours ago; record is its gate.json."""
        folder = (parent or self.cache) / name
        (folder / 'run').mkdir(parents=True)
        files = [folder / 'run' / 'inventory.json']
        files[0].write_text('x' * 1000)
        if record is not None:
            files.append(folder / 'gate.json')
            files[1].write_text(json.dumps(record))
        when = self.now - age_hours * HOUR
        for path in (*files, folder / 'run', folder):
            os.utime(path, (when, when))
        if read_only:
            files[0].chmod(stat.S_IREAD)
        return folder

    def prune(self, **kwargs):
        return cache_prune.prune(self.cache, temp=self.temp, now=self.now, **kwargs)

    def receipts(self):
        return sorted(path.name for path in (self.cache / il_merge.RECEIPTS).glob('*'))

    def test_a_landed_runs_receipt_is_kept_before_its_folder_goes(self):
        pushed = {'tip': 'a' * 40, 'branch': 'mirror/x', 'applied': False, 'landing': {'path': 'validated', 'pushed': True}}
        landed = self.folder('gate-il-landed', 48, pushed)
        report = self.prune(apply=True)
        self.assertFalse(landed.exists())
        self.assertEqual(self.receipts(), ['gate-il-landed.json'])
        kept = json.loads((self.cache / il_merge.RECEIPTS / 'gate-il-landed.json').read_text())
        self.assertEqual({key: kept[key] for key in pushed}, pushed)
        # The time it landed, from the receipt's own file time, not the time of the prune.
        self.assertEqual(kept['landed_at'], time.strftime('%Y-%m-%dT%H:%M:%SZ', time.gmtime(self.now - 48 * HOUR)))
        self.assertEqual((report['receipts_kept'], report['removed'], report['failed']),
                         (['gate-il-landed'], [str(landed)], []))
        self.assertGreater(report['bytes'], 1000)

    def test_receipts_are_kept_whatever_the_folders_age_and_only_once(self):
        recent = self.folder('gate-il-recent', 0.1, {'applied': True, 'tip': 'b' * 40})
        self.assertEqual(self.prune(apply=True)['receipts_kept'], ['gate-il-recent'])
        self.assertTrue(recent.exists())  # too recent to remove, but its receipt is already safe
        self.assertEqual(self.receipts(), ['gate-il-recent.json'])
        first = (self.cache / il_merge.RECEIPTS / 'gate-il-recent.json').read_text()
        self.assertEqual(self.prune(apply=True)['receipts_kept'], [])
        self.assertEqual((self.cache / il_merge.RECEIPTS / 'gate-il-recent.json').read_text(), first)

    def test_runs_that_did_not_land_leave_no_receipt(self):
        failed = self.folder('gate-il-failed', 48, {'passed': False, 'applied': False})
        retried = self.folder('gate-il-retry', 48, {'passed': True, 'applied': False, 'landing': {'path': 'retry'}})
        unreadable = self.folder('gate-il-broken', 48)
        (unreadable / 'gate.json').write_text('{')
        for path in (unreadable / 'gate.json', unreadable):  # writing the file touched the folder
            os.utime(path, (self.now - 48 * HOUR,) * 2)
        report = self.prune(apply=True)
        self.assertEqual(report['receipts_kept'], [])
        self.assertFalse((self.cache / il_merge.RECEIPTS).exists())
        self.assertFalse(any(folder.exists() for folder in (failed, retried, unreadable)))

    def test_only_old_run_folders_are_removed(self):
        old = [self.folder(name, 30, read_only=True) for name in ('gate-il-old', 'release-il-old', 'native-gate-old')]
        young = [self.folder(name, 2) for name in ('gate-il-young', 'release-il-young', 'native-gate-young')]
        # A folder whose report was just written is in use, however old the folder itself is.
        busy = self.folder('gate-il-busy', 30)
        (busy / 'run' / 'build.log').write_text('still building')
        others = [self.cache / name for name in ('inventory-cache', 'gate-work', 'receipts', 'msclr', 'gate-ilx')]
        for path in others:
            path.mkdir()
            (path / 'keep.json').write_text('{}')
            os.utime(path / 'keep.json', (0, 0))
            os.utime(path, (0, 0))
        report = self.prune(apply=True)
        self.assertEqual(sorted(report['removed']), sorted(map(str, old)))
        self.assertFalse(any(folder.exists() for folder in old))
        self.assertTrue(all(folder.exists() for folder in (*young, busy)))
        self.assertTrue(all((path / 'keep.json').is_file() for path in others))
        self.assertEqual(len(self.prune(apply=True, hours=1)['removed']), 3)  # the young ones, with a shorter limit

    def test_without_apply_nothing_is_written_or_deleted(self):
        landed = self.folder('gate-il-landed', 48, {'applied': True})
        tree = self.folder('quire-il-gate-killed', 48, parent=self.temp)
        report = self.prune()
        self.assertEqual((report['applied'], report['receipts_kept']), (False, ['gate-il-landed']))
        self.assertEqual(sorted(report['removed']), sorted([str(landed), str(tree)]))
        self.assertTrue(landed.exists() and tree.exists())
        self.assertFalse((self.cache / il_merge.RECEIPTS).exists())

    def test_a_killed_runs_temporary_tree_goes_after_the_gates_own_limit(self):
        limit = release_il.STALE_WORK / HOUR
        killed = [self.folder(name, limit + 1, parent=self.temp) for name in ('quire-il-gate-a', 'quire-release-il-b')]
        running = self.folder('quire-il-gate-running', limit - 1, parent=self.temp)
        other = self.folder('someone-elses', limit + 100, parent=self.temp)
        # --hours is for run folders: it never shortens the limit for a tree a running gate may still use.
        report = self.prune(apply=True, hours=0)
        self.assertEqual(sorted(report['removed']), sorted(map(str, killed)))
        self.assertTrue(running.exists() and other.exists())

    def test_a_folder_that_cannot_be_removed_is_reported(self):
        self.folder('release-il-open', 48)
        with mock.patch.object(cache_prune, 'remove', side_effect=PermissionError('in use')):
            report = self.prune(apply=True)
        self.assertEqual(report['removed'], [])
        self.assertIn('in use', report['failed'][0])

    def test_the_command_lists_by_default(self):
        self.folder('gate-il-landed', 48, {'applied': True})
        with mock.patch('builtins.print') as printed, mock.patch.object(cache_prune.tempfile, 'gettempdir',
                                                                      return_value=str(self.temp)):
            self.assertEqual(cache_prune.main(['--cache', str(self.cache)]), 0)
            lines = [call.args[0] for call in printed.call_args_list]
            self.assertTrue(lines[0].startswith('would keep receipt'))
            self.assertIn('pass --apply to do it', lines[-1])
            self.assertTrue((self.cache / 'gate-il-landed').exists())
            self.assertEqual(cache_prune.main(['--cache', str(self.cache), '--apply']), 0)
        self.assertFalse((self.cache / 'gate-il-landed').exists())
        self.assertEqual(self.receipts(), ['gate-il-landed.json'])


if __name__ == '__main__':
    unittest.main()
