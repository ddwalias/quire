using System.Collections;

namespace Quire.utl_mgd;

public partial class Mailbox<T>
{
    public Mailbox()
    {
        m_mutex = new SafeMutex("mailbox mutex");
        m_nonempty = new SafeEvent("mailbox non-empty");
        m_queue = new Queue<T>();
        m_nonempty.Reset();
        GC.KeepAlive(this);
    }

    public unsafe T GetMessage()
    {
        T msg = default;
        GetMessage(ref msg, (void*)ulong.MaxValue);
        return msg;
    }

    public unsafe bool GetMessage(ref T msg, void* timeout)
    {
        do
        {
            m_mutex.WaitOne();
            try
            {
                if (m_queue.Count > 0)
                {
                    msg = m_queue.Dequeue();
                    if (m_queue.Count == 0)
                    {
                        m_nonempty.Reset();
                    }

                    return true;
                }
            }
            finally
            {
                m_mutex.EnsureReleased(1u);
            }
        }
        while (m_nonempty.WaitTwoWithTimeout(timeout));

        return false;
    }

    public void PutMessage(T msg)
    {
        m_mutex.WaitOne();
        try
        {
            m_queue.Enqueue(msg);
            m_nonempty.Set();
        }
        finally
        {
            m_mutex.EnsureReleased(1u);
        }

        GC.KeepAlive(this);
    }

    public int Count()
    {
        m_mutex.WaitOne();
        try
        {
            int count = m_queue.Count;
            GC.KeepAlive(this);
            return count;
        }
        finally
        {
            m_mutex.EnsureReleased(1u);
        }
    }
}
