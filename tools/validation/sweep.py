"""Remove stale validation scratch before a new unit, on Windows or Linux:

    python tools/validation/sweep.py [--dry-run]

Removes the quire-validation-*, quire-coverage-*, quire-audit-* and quire-gate-* directories in the temporary
directory (tempfile.gettempdir()) in which nothing changed for 12 hours (mutate.newest_change, which looks at every
file beneath them, not just the directory's own time), so never a running validation's or gate's. Links and files
are left alone, and on POSIX so are other users' directories. A directory Windows will not fully remove (a file
still open) is reported as kept."""
import argparse
import os
import shutil
import stat
import sys
import tempfile
import time
from pathlib import Path
from mutate import newest_change

PATTERNS = ('quire-validation-*', 'quire-coverage-*', 'quire-audit-*', 'quire-gate-*')
STALE_SECONDS = 12 * 3600


def stale(directory=None, age=STALE_SECONDS, now=None):
    """The scratch directories in `directory` (default: the temporary directory) unchanged for `age` seconds."""
    cutoff, found = (now or time.time()) - age, []
    for pattern in PATTERNS:
        for path in sorted(Path(directory or tempfile.gettempdir()).glob(pattern)):
            try:
                info = path.lstat()
                if not stat.S_ISDIR(info.st_mode) or hasattr(os, 'getuid') and info.st_uid != os.getuid():
                    continue
                newest = newest_change(path)
            except OSError:
                continue
            if newest is not None and newest < cutoff:
                found.append(path)
    return found


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--dry-run', action='store_true', help='list what would be removed')
    args = parser.parse_args(argv)
    for path in stale():
        if not args.dry_run:
            shutil.rmtree(path, ignore_errors=True)
        print(f"{'would remove' if args.dry_run else 'removed' if not path.exists() else 'in use, kept'}: {path}")
    return 0


if __name__ == '__main__':
    sys.exit(main())
