import os
import tempfile
import time
import unittest
from pathlib import Path

import merge_lock


class WaitTests(unittest.TestCase):
    def test_acquires_at_once_when_free(self):
        calls = []
        merge_lock.wait_for_lock(lambda: calls.append(1), None, timeout=5, sleep=lambda s: None)
        self.assertEqual(calls, [1])

    def test_gives_up_after_the_timeout_with_the_holder_named(self):
        now = [0.0]

        def clock():
            return now[0]

        def sleep(seconds):
            now[0] += seconds

        class Handle:
            def seek(self, offset):
                pass

            def read(self, n):
                return b'pid 4242 since 2026-11-16 04:00:00'

        def busy():
            raise OSError('locked')

        with self.assertRaises(TimeoutError) as caught:
            merge_lock.wait_for_lock(busy, Handle(), timeout=120, clock=clock, sleep=sleep)
        self.assertIn('pid 4242', str(caught.exception))
        self.assertIn('120s', str(caught.exception))

    def test_reports_the_wait_once_a_minute(self):
        now = [0.0]
        messages = []
        attempts = [0]

        def clock():
            return now[0]

        def sleep(seconds):
            now[0] += seconds

        class Handle:
            def seek(self, offset):
                pass

            def read(self, n):
                return b'pid 7 since now'

        def busy_then_free():
            attempts[0] += 1
            if attempts[0] < 150:
                raise OSError('locked')

        original = merge_lock.report
        merge_lock.report = messages.append
        try:
            merge_lock.wait_for_lock(busy_then_free, Handle(), timeout=10_000, clock=clock, sleep=sleep)
        finally:
            merge_lock.report = original
        self.assertTrue(messages)
        self.assertTrue(all('pid 7' in m for m in messages))
        self.assertLessEqual(len(messages), 3)


class QueueTests(unittest.TestCase):
    def ticket(self, directory, name, age=0.0):
        """A waiter's ticket file as another process would leave it, `age` seconds since it last refreshed."""
        path = Path(directory) / name
        path.write_bytes(b'')
        stamp = time.time() - age
        os.utime(path, (stamp, stamp))
        return path

    def test_a_waiter_tries_only_when_nobody_earlier_is_waiting(self):
        with tempfile.TemporaryDirectory() as tmp:
            self.ticket(tmp, '00000000000000000001-111')  # arrived first, so it goes first
            later = merge_lock.Queue(tmp)
            later.join()
            self.assertEqual(later.ahead(), ['00000000000000000001-111'])
            (Path(tmp) / '00000000000000000001-111').unlink()
            self.assertEqual(later.ahead(), [])

    def test_later_arrivals_do_not_hold_a_waiter_back(self):
        with tempfile.TemporaryDirectory() as tmp:
            first = merge_lock.Queue(tmp)
            first.join()
            self.ticket(tmp, '99999999999999999999-222')
            self.assertEqual(first.ahead(), [])

    def test_a_ticket_nobody_refreshes_is_dropped(self):
        with tempfile.TemporaryDirectory() as tmp:
            stale = self.ticket(tmp, '00000000000000000001-111', age=merge_lock.STALE_TICKET + 30)
            waiter = merge_lock.Queue(tmp)
            waiter.join()
            self.assertEqual(waiter.ahead(), [])
            self.assertFalse(stale.exists())

    def test_the_longest_waiter_gets_the_lock_first(self):
        with tempfile.TemporaryDirectory() as tmp:
            self.ticket(tmp, '00000000000000000001-111')  # older, still refreshing
            order = []
            tries = [0]

            def sleep(seconds):
                tries[0] += 1
                if tries[0] == 3:  # the older waiter takes the lock and leaves its ticket
                    (Path(tmp) / '00000000000000000001-111').unlink()

            def try_lock():
                order.append('locked')

            queue = merge_lock.Queue(tmp)
            merge_lock.wait_for_lock(try_lock, None, 100, clock=lambda: 0.0, sleep=sleep, queue=queue)
            self.assertEqual(tries[0], 3)  # it slept while the older ticket was there, and did not try the lock
            self.assertEqual(order, ['locked'])
            self.assertFalse(queue.path.exists())  # and it leaves its own ticket on the way out

    def test_a_timeout_leaves_no_ticket(self):
        with tempfile.TemporaryDirectory() as tmp:
            self.ticket(tmp, '00000000000000000001-111')
            now = [0.0]

            def clock():
                return now[0]

            def sleep(seconds):
                now[0] += seconds

            class Handle:
                def seek(self, offset):
                    pass

                def read(self, n):
                    return b'pid 5 since now'

            queue = merge_lock.Queue(tmp)
            with self.assertRaises(TimeoutError):
                merge_lock.wait_for_lock(lambda: None, Handle(), 3, clock=clock, sleep=sleep, queue=queue)
            self.assertFalse(queue.path.exists())


class HeldTests(unittest.TestCase):
    def test_writes_the_holder_then_releases(self):
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / 'lock'
            with merge_lock.held(path, timeout=5):
                with open(path, 'rb') as handle:
                    handle.seek(1)  # byte 0 is the locked byte on Windows; the holder's note follows it
                    self.assertIn(f'pid {os.getpid()}'.encode(), handle.read())
            self.assertFalse(merge_lock.is_held(path))

    @unittest.skipUnless(os.name == 'nt', 'byte-range lock check is Windows-specific')
    def test_a_held_lock_times_out_with_a_message(self):
        import msvcrt
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / 'lock'
            other = open(path, 'a+b')
            try:
                msvcrt.locking(other.fileno(), msvcrt.LK_NBLCK, 1)
                started = time.monotonic()
                with self.assertRaises(TimeoutError):
                    with merge_lock.held(path, timeout=2):
                        pass
                self.assertLess(time.monotonic() - started, 30)
            finally:
                other.seek(0)
                msvcrt.locking(other.fileno(), msvcrt.LK_UNLCK, 1)
                other.close()


if __name__ == '__main__':
    unittest.main()
