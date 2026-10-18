"""The merge lock every lane holds while it lands a branch (lane_merge.py): one exclusive lock per machine and
operating system, on a well-known file. POSIX locks /tmp/quire-merge.lock with fcntl.flock, the lock
`flock /tmp/quire-merge.lock` takes too; Windows locks the first byte of %TEMP%\\quire-merge.lock with msvcrt.locking,
shared by every session of the user. A WSL session and a Windows session do not share a lock (WSL's /tmp is not
%TEMP%, and an flock on a /mnt/c file is not a Windows byte-range lock), so do not land from both at once: a push
then fails as non-fast-forward rather than corrupting main, and the lane integrates and retries. QUIRE_MERGE_LOCK
names another file."""
import contextlib
import os
import tempfile
import time
from pathlib import Path


def lock_path(environ=os.environ):
    if environ.get('QUIRE_MERGE_LOCK'):
        return Path(environ['QUIRE_MERGE_LOCK'])
    return Path(tempfile.gettempdir()) / 'quire-merge.lock' if os.name == 'nt' else Path('/tmp/quire-merge.lock')


@contextlib.contextmanager
def held(path=None):
    """Hold the merge lock for the `with` body, waiting as long as another holder keeps it."""
    with open(path or lock_path(), 'a+b') as handle:
        if os.name == 'nt':
            import msvcrt
            handle.seek(0)
            while True:
                try:
                    msvcrt.locking(handle.fileno(), msvcrt.LK_NBLCK, 1)
                    break
                except OSError:
                    time.sleep(1)
            try:
                yield
            finally:
                handle.seek(0)
                msvcrt.locking(handle.fileno(), msvcrt.LK_UNLCK, 1)
        else:
            import fcntl
            fcntl.flock(handle, fcntl.LOCK_EX)
            try:
                yield
            finally:
                fcntl.flock(handle, fcntl.LOCK_UN)


def is_held(path=None):
    """Whether another holder has the lock now (tests, and a quick look before landing)."""
    with open(path or lock_path(), 'a+b') as handle:
        if os.name == 'nt':
            import msvcrt
            handle.seek(0)
            try:
                msvcrt.locking(handle.fileno(), msvcrt.LK_NBLCK, 1)
            except OSError:
                return True
            handle.seek(0)
            msvcrt.locking(handle.fileno(), msvcrt.LK_UNLCK, 1)
            return False
        import fcntl
        try:
            fcntl.flock(handle, fcntl.LOCK_EX | fcntl.LOCK_NB)
        except OSError:
            return True
        fcntl.flock(handle, fcntl.LOCK_UN)
        return False
