using System.Threading;

namespace Quire.utl_mgd;

public partial class InterruptControl
{
    [ThreadStatic]
    private static InterruptControl s_interrupt_control;

    private SafeMutex m_request_mutex = new SafeMutex("InterruptControl request mutex");
    private SafeMutex m_abort_mutex = new SafeMutex("InterruptControl abort mutex");
    private bool m_interrupt_requested;
    private CancellationTokenSource m_cts = new CancellationTokenSource();

    public CancellationTokenSource Cts => m_cts;

    public void BeginRequestInterrupt()
    {
        m_request_mutex.WaitOne();
        m_abort_mutex.WaitOne();
        m_interrupt_requested = true;
        m_request_mutex.ReleaseMutex();
        GC.KeepAlive(this);
    }

    public void EndRequestInterrupt()
    {
        m_abort_mutex.ReleaseMutex();
        m_request_mutex.WaitOne();
        m_interrupt_requested = false;
        m_request_mutex.ReleaseMutex();
        GC.KeepAlive(this);
    }

    public static void ThreadInitialize(InterruptControl c) => s_interrupt_control = c;

    public static void ProhibitInterruption()
    {
        InterruptControl interruptControl = s_interrupt_control;
        if (interruptControl == null)
        {
            return;
        }

        try
        {
            interruptControl.m_request_mutex.WaitOne();
            if (interruptControl.m_interrupt_requested)
            {
                interruptControl.m_request_mutex.ReleaseMutex();
                throw new Exception("abort current thread");
            }

            interruptControl.m_abort_mutex.WaitOne();
        }
        finally
        {
            interruptControl.m_request_mutex.EnsureReleased(1u);
        }
    }

    public static void AllowInterruption() => s_interrupt_control?.m_abort_mutex.EnsureReleased(1u);

    public static int ProhibitInterruption_(int A_0)
    {
        ProhibitInterruption();
        return 0;
    }

    public static int AllowInterruption_(int A_0)
    {
        s_interrupt_control?.m_abort_mutex.EnsureReleased(1u);
        return 0;
    }
}
