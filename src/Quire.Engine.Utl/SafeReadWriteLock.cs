namespace Quire.utl_mgd;

public partial class SafeReadWriteLock
{
    public SafeReadWriteLock()
    {
        m_write_lock = new SafeMutex("SafeReadWriteLock:write");
        m_reader_count_lock = new SafeMutex("SafeReadWriteLock:rcount");
        m_reader_count = 0;
        m_no_readers = new SafeEvent("SafeReadWriteLock:no readers");
        m_no_readers.Set();
        GC.KeepAlive(this);
    }

    public void WaitReadLock()
    {
        m_write_lock.WaitOne();
        m_reader_count_lock.WaitOne();
        try
        {
            if (++m_reader_count == 1)
            {
                m_no_readers.Reset();
            }
        }
        finally
        {
            m_reader_count_lock.ReleaseMutex();
            m_write_lock.ReleaseMutex();
        }

        GC.KeepAlive(this);
    }

    public void ReleaseReadLock()
    {
        m_reader_count_lock.WaitOne();
        try
        {
            if ((m_reader_count += -1) == 0)
            {
                m_no_readers.Set();
            }
        }
        finally
        {
            m_reader_count_lock.ReleaseMutex();
        }

        GC.KeepAlive(this);
    }

    public void WaitWriteLock()
    {
        m_write_lock.WaitOne();
        m_no_readers.WaitOne();
        GC.KeepAlive(this);
    }

    public void ReleaseWriteLock()
    {
        m_write_lock.ReleaseMutex();
        GC.KeepAlive(this);
    }
}
