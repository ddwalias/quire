using System.Runtime.InteropServices;

namespace Quire.utl_mgd;

public partial class TaskQueue
{
    private SafeMutex m_empty_mutex = new SafeMutex("task queue empty mutex");
    private SafeEvent m_empty = new SafeEvent("task queue empty");

    [return: MarshalAs(UnmanagedType.U1)]
    private static bool ExecuteTask(TaskQueue q, int __unnamed001, Function f)
    {
        f.Invoke0();
        q.OnTaskExecuted();
        return true;
    }

    protected void OnTaskExecuted()
    {
        m_empty_mutex.WaitOne();
        try
        {
            if (GetMailbox().Count() == 0)
            {
                m_empty.Set();
            }
        }
        finally
        {
            m_empty_mutex.EnsureReleased(1u);
        }

        GC.KeepAlive(this);
    }

    public TaskQueue()
        : base(1048576)
    {
        Start(this, 0, null, ExecuteTask, bStopThreadOnFinalize: true);
        try
        {
            m_empty.Set();
            GC.KeepAlive(this);
            return;
        }
        catch
        {
            Dispose(A_0: true);
            throw;
        }
    }

    public void EnqueueTask(Function f)
    {
        m_empty_mutex.WaitOne();
        try
        {
            m_empty.Reset();
            GetMailbox().PutMessage(f);
        }
        finally
        {
            m_empty_mutex.EnsureReleased(1u);
        }

        GC.KeepAlive(this);
    }

    public void WaitEmpty()
    {
        m_empty.WaitOne();
        GC.KeepAlive(this);
    }

    public new int Count()
    {
        int result = GetMailbox().Count();
        GC.KeepAlive(this);
        return result;
    }
}
