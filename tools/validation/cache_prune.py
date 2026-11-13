"""Delete old gate run folders from .mirror-cache, keeping every landing receipt.

    python tools/validation/cache_prune.py [--hours 24] [--apply]

This is the cleanup for gate output; do not delete .mirror-cache/gate-il-* by hand or from a script. A run folder
(.mirror-cache/gate-il-<id>, release-il-<id>, native-gate-<id>) holds one run's inventories, build log and reports.
The gate-il folder of a run that landed also holds its receipt (gate.json). Every such receipt not yet in
.mirror-cache/receipts/ is copied there first, whatever the folder's age; then run folders whose newest file is
older than --hours are removed. Temporary trees a killed gate left in the system temp directory (quire-il-gate-*,
quire-release-il-*) are removed once older than release_il.STALE_WORK, as the gate's own sweep treats a killed run.
Without --apply nothing is written or deleted: the script lists what it would do.

Never touched: receipts/, inventory-cache/ (evicts itself), gate-work/ (purged by each run), the reference caches
and anything else in .mirror-cache.
"""
import argparse
import json
import os
from pathlib import Path
import shutil
import stat
import sys
import tempfile
import time

import il_merge
import release_il

ROOT = Path(__file__).resolve().parents[2]
RUN_FOLDERS = ('gate-il-*', 'release-il-*', 'native-gate-*')
TEMP_TREES = ('quire-il-gate-*', 'quire-release-il-*')


def newest(folder):
    """The latest modification time in a folder (a run writes its report last), or None when it cannot be read."""
    latest = None
    try:
        for path, _, names in os.walk(folder):
            for entry in (path, *(os.path.join(path, name) for name in names)):
                latest = max(latest or 0, os.stat(entry).st_mtime)
    except OSError:
        return None
    return latest


def landing(folder):
    """The landing record of a gate-il run folder, or None when the run did not land or left no readable record."""
    try:
        record = json.loads((folder / 'gate.json').read_text(encoding='utf-8'))
    except (OSError, ValueError):
        return None
    return record if isinstance(record, dict) and il_merge.landed(record) else None


def size(folder):
    total = 0
    for path, _, names in os.walk(folder):
        for name in names:
            try:
                total += os.stat(os.path.join(path, name)).st_size
            except OSError:
                pass
    return total


def remove(folder):
    def writable(function, path, error):  # Git marks object files read-only
        os.chmod(path, stat.S_IWRITE)
        function(path)
    shutil.rmtree(folder, onexc=writable)


def prune(cache, hours=24, apply=False, temp=None, now=None):
    """Keep the receipt of every landed gate-il folder, then remove run folders older than `hours` and killed runs'
    temporary trees. Returns what was (or with apply=False, would be) kept and removed, and what could not be."""
    cache, now = Path(cache), now or time.time()
    report = {'applied': apply, 'receipts_kept': [], 'removed': [], 'bytes': 0, 'failed': []}
    receipts = cache / il_merge.RECEIPTS
    for folder in sorted(cache.glob('gate-il-*')):
        record = folder.is_dir() and landing(folder)
        if record and not (receipts / f'{folder.name}.json').is_file():
            if apply:
                il_merge.keep_receipt(record, folder.name, cache, landed_at=(folder / 'gate.json').stat().st_mtime)
            report['receipts_kept'].append(folder.name)
    old = [(folder, hours * 3600) for pattern in RUN_FOLDERS for folder in sorted(cache.glob(pattern))]
    old += [(tree, release_il.STALE_WORK) for pattern in TEMP_TREES
            for tree in sorted(Path(temp or tempfile.gettempdir()).glob(pattern))]
    for folder, limit in old:
        modified = folder.is_dir() and newest(folder)
        if not modified or now - modified <= limit:
            continue
        # A landed run's folder goes only once its receipt is safe.
        if folder.parent == cache and landing(folder) and apply and not (receipts / f'{folder.name}.json').is_file():
            report['failed'].append(f'{folder}: its receipt was not kept')
            continue
        freed = size(folder)
        if apply:
            try:
                remove(folder)
            except OSError as error:  # in use: a later prune removes it
                report['failed'].append(f'{folder}: {error}')
                continue
        report['removed'].append(str(folder))
        report['bytes'] += freed
    return report


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--hours', type=float, default=24, help='remove run folders whose newest file is older (default 24)')
    parser.add_argument('--apply', action='store_true', help='keep the receipts and delete; without it, only list')
    parser.add_argument('--cache', type=Path, default=ROOT / '.mirror-cache')
    args = parser.parse_args(argv)
    if args.hours < 0:
        parser.error('--hours must not be negative')
    report = prune(args.cache, args.hours, args.apply)
    verb = ('kept', 'removed') if args.apply else ('would keep', 'would remove')
    for name in report['receipts_kept']:
        print(f"{verb[0]} receipt {args.cache / il_merge.RECEIPTS / (name + '.json')}")
    for folder in report['removed']:
        print(f'{verb[1]} {folder}')
    for failure in report['failed']:
        print(f'not removed: {failure}', file=sys.stderr)
    print(f"{verb[0]} {len(report['receipts_kept'])} receipts; {verb[1]} {len(report['removed'])} folders, "
          f"{report['bytes'] / 1e6:.1f} MB" + ('' if args.apply else '; pass --apply to do it'))
    return 1 if report['failed'] else 0


if __name__ == '__main__':
    sys.exit(main())
