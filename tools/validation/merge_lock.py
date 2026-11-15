"""The merge lock every lane holds while it lands a branch (lane_merge.py): one exclusive lock per machine and
operating system, on a well-known file. POSIX locks /tmp/quire-merge.lock with fcntl.flock, the lock
`flock /tmp/quire-merge.lock` takes too; Windows locks the first byte of %TEMP%\\quire-merge.lock with msvcrt.locking,
shared by every session of the user. A WSL session and a Windows session do not share a lock (WSL's /tmp is not
%TEMP%, and an flock on a /mnt/c file is not a Windows byte-range lock), so do not land from both at once: a push
then fails as non-fast-forward rather than corrupting main, and the lane integrates and retries. QUIRE_MERGE_LOCK
names another file.

The holder writes its process id and start time into the file after the first byte, so a waiting landing can say who
it is waiting for. A wait is reported every minute, and gives up with an error after QUIRE_MERGE_LOCK_TIMEOUT seconds
(default 7200) instead of waiting forever."""
import contextlib
import os
import sys
import tempfile
import time
from pathlib import Path

DEFAULT_TIMEOUT = 7200
REPORT_EVERY = 60


def lock_path(environ=os.environ):
    if environ.get('QUIRE_MERGE_LOCK'):
        return Path(environ['QUIRE_MERGE_LOCK'])
    return Path(tempfile.gettempdir()) / 'quire-merge.lock' if os.name == 'nt' else Path('/tmp/quire-merge.lock')


def timeout_seconds(environ=os.environ):
    return float(environ.get('QUIRE_MERGE_LOCK_TIMEOUT', DEFAULT_TIMEOUT))


def holder_text(handle):
    """The holder's note (pid and start time) from after the locked first byte, or 'unknown'."""
    handle.seek(1)
    text = handle.read(200).decode(errors='replace').strip()
    return text or 'unknown'


def write_holder(handle):
    handle.seek(1)
    handle.truncate()
    handle.write(f'pid {os.getpid()} since {time.strftime("%Y-%m-%d %H:%M:%S")}\n'.encode())
    handle.flush()


def report(message):
    print(message, file=sys.stderr, flush=True)


def wait_for_lock(try_lock, handle, timeout, clock=time.monotonic, sleep=time.sleep):
    """Retry try_lock until it succeeds, reporting the holder every REPORT_EVERY seconds; raise TimeoutError after
    `timeout` seconds. Returns nothing; the lock is held when it returns."""
    start = clock()
    last_report = start
    while True:
        try:
            try_lock()
            return
        except OSError:
            now = clock()
            if now - start >= timeout:
                raise TimeoutError(f'merge lock still held after {timeout:.0f}s by {holder_text(handle)}')
            if now - last_report >= REPORT_EVERY:
                report(f'waiting for the merge lock held by {holder_text(handle)} ({now - start:.0f}s so far)')
                last_report = now
            sleep(1)


@contextlib.contextmanager
def held(path=None, timeout=None):
    """Hold the merge lock for the `with` body. Waits while another holder keeps it, reporting who it is waiting for,
    and gives up after `timeout` seconds (default QUIRE_MERGE_LOCK_TIMEOUT)."""
    timeout = timeout_seconds() if timeout is None else timeout
    with open(path or lock_path(), 'a+b') as handle:
        if os.name == 'nt':
            import msvcrt
            handle.seek(0)

            def try_lock():
                handle.seek(0)
                msvcrt.locking(handle.fileno(), msvcrt.LK_NBLCK, 1)

            wait_for_lock(try_lock, handle, timeout)
            write_holder(handle)
            try:
                yield
            finally:
                handle.seek(0)
                msvcrt.locking(handle.fileno(), msvcrt.LK_UNLCK, 1)
        else:
            import fcntl

            def try_lock():
                fcntl.flock(handle, fcntl.LOCK_EX | fcntl.LOCK_NB)

            wait_for_lock(try_lock, handle, timeout)
            write_holder(handle)
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
