"""The merge lock every lane holds while it lands a branch (lane_merge.py): one exclusive lock per machine and
operating system, on a well-known file. POSIX locks /tmp/quire-merge.lock with fcntl.flock, the lock
`flock /tmp/quire-merge.lock` takes too; Windows locks the first byte of %TEMP%\\quire-merge.lock with msvcrt.locking,
shared by every session of the user. A WSL session and a Windows session do not share a lock (WSL's /tmp is not
%TEMP%, and an flock on a /mnt/c file is not a Windows byte-range lock), so do not land from both at once: a push
then fails as non-fast-forward rather than corrupting main, and the lane integrates and retries. QUIRE_MERGE_LOCK
names another file.

The holder writes its process id and start time into the file after the first byte, so a waiting landing can say who
it is waiting for. A wait is reported every minute, and gives up with an error after QUIRE_MERGE_LOCK_TIMEOUT seconds
(default 7200) instead of waiting forever.

Waiters are served in the order they arrived. Each one files a ticket in a queue directory beside the lock file
(`<lock>.queue`), keeps it fresh while it waits, and tries the lock only when no live ticket is older than its own, so
a landing that has waited longest goes first instead of losing every race to a lane that happens to retry at the right
second. A ticket not refreshed for STALE_TICKET seconds belongs to a killed or hung waiter and is dropped."""
import contextlib
import os
import sys
import tempfile
import time
from pathlib import Path

DEFAULT_TIMEOUT = 7200
REPORT_EVERY = 60
STALE_TICKET = 60


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


def queue_dir(path):
    return Path(f'{path}.queue')


class Queue:
    """First come, first served among the landings waiting for the lock: one ticket file per waiter, named by arrival
    time so a sort puts the longest waiter first."""

    def __init__(self, directory, wall_clock=time.time):
        self.directory = Path(directory)
        self.wall_clock = wall_clock
        self.name = f'{time.time_ns():020d}-{os.getpid()}'
        self.path = self.directory / self.name

    def join(self):
        self.directory.mkdir(parents=True, exist_ok=True)
        self.path.write_bytes(b'')

    def touch(self):
        """Mark this waiter alive; recreates a ticket another waiter dropped as stale (the name, so the place in line,
        is unchanged)."""
        try:
            os.utime(self.path)
        except OSError:
            self.join()

    def leave(self):
        self.path.unlink(missing_ok=True)

    def ahead(self):
        """The tickets of live waiters that arrived before this one. Tickets nobody has refreshed lately are dropped."""
        now = self.wall_clock()
        waiting = []
        for ticket in sorted(self.directory.iterdir()):
            if ticket.name >= self.name:
                break
            try:
                age = now - ticket.stat().st_mtime
            except OSError:
                continue
            if age > STALE_TICKET:
                ticket.unlink(missing_ok=True)
            else:
                waiting.append(ticket.name)
        return waiting


def wait_for_lock(try_lock, handle, timeout, clock=time.monotonic, sleep=time.sleep, queue=None):
    """Retry try_lock until it succeeds, reporting the holder every REPORT_EVERY seconds; raise TimeoutError after
    `timeout` seconds. With a queue, try only when no live waiter arrived earlier. Returns nothing; the lock is held
    when it returns."""
    start = clock()
    last_report = start
    ahead = []
    if queue:
        queue.join()
    try:
        while True:
            ahead = queue.ahead() if queue else []
            if not ahead:
                try:
                    try_lock()
                    return
                except OSError:
                    pass
            now = clock()
            if now - start >= timeout:
                raise TimeoutError(f'merge lock still held after {timeout:.0f}s by {holder_text(handle)}')
            if now - last_report >= REPORT_EVERY:
                place = f', {len(ahead)} landing(s) ahead in the queue' if ahead else ''
                report(f'waiting for the merge lock held by {holder_text(handle)} ({now - start:.0f}s so far{place})')
                last_report = now
            if queue:
                queue.touch()
            sleep(1)
    finally:
        if queue:
            queue.leave()


@contextlib.contextmanager
def held(path=None, timeout=None):
    """Hold the merge lock for the `with` body. Waits while another holder keeps it, reporting who it is waiting for,
    and gives up after `timeout` seconds (default QUIRE_MERGE_LOCK_TIMEOUT)."""
    timeout = timeout_seconds() if timeout is None else timeout
    path = path or lock_path()
    with open(path, 'a+b') as handle:
        if os.name == 'nt':
            import msvcrt
            handle.seek(0)

            def try_lock():
                handle.seek(0)
                msvcrt.locking(handle.fileno(), msvcrt.LK_NBLCK, 1)

            wait_for_lock(try_lock, handle, timeout, queue=Queue(queue_dir(path)))
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

            wait_for_lock(try_lock, handle, timeout, queue=Queue(queue_dir(path)))
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
