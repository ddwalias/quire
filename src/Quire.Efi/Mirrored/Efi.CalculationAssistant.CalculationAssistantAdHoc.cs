#nullable disable
#pragma warning disable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi.CalculationAssistant;

public partial class CalculationAssistantAdHoc
{
    protected partial class InnerTask
    {
        public TaskDefinition taskDef { get; set; }

        public CancellationTokenSource cancelTokenSrc { get; set; }

        public int taskId { get; set; }
    }

    protected partial class ConcurrentQueueWithSignal
    {
        private ConcurrentQueue<ReadinessInfo> m_queue;

        private ManualResetEvent m_available;

        private object m_lock;

        public ConcurrentQueueWithSignal()
        {
            m_queue = new ConcurrentQueue<ReadinessInfo>();
            m_available = new ManualResetEvent(initialState: false);
            m_lock = new object();
        }

        public bool Wait(int timeout)
        {
            return m_available.WaitOne(timeout);
        }

        public void putToQueue(ReadinessInfo param)
        {
            lock (m_lock)
            {
                m_available.Reset();
                m_queue.Enqueue(param);
                if (m_queue.Count > 0)
                {
                    m_available.Set();
                }
            }
        }

        public void putToQueue(List<ReadinessInfo> param)
        {
            lock (m_lock)
            {
                m_available.Reset();
                foreach (ReadinessInfo item in param)
                {
                    m_queue.Enqueue(item);
                }
                if (m_queue.Count > 0)
                {
                    m_available.Set();
                }
            }
        }

        public bool getFromQueue(out ReadinessInfo param)
        {
            lock (m_lock)
            {
                m_available.Reset();
                if (m_queue.TryDequeue(out param))
                {
                    if (m_queue.Count > 0)
                    {
                        m_available.Set();
                    }
                    return true;
                }
                if (m_queue.Count > 0)
                {
                    m_available.Set();
                }
                return false;
            }
        }
    }

    protected partial class ReadinessInfo
    {
        public int subtaskNumber { get; set; }

        public Exception exception { get; set; }

        public ReadinessInfo()
        {
            exception = null;
        }
    }

    private static int ms_threads = 0;

    private static int ms_maxInstances = 0;

    private static int ms_isSettingsCalled = 0;

    private static object ms_lockSetup = new object();

    private static object ms_lockInstanceCounter = new object();

    private static int ms_instancesCounter = 0;

    private static readonly int ms_waitTimeout = 200;

    private static long ms_overheadTime_mksec = 0L;

    private ConcurrentQueueWithSignal m_queue;

    private List<InnerTask> m_innerTaskList;

    private CancellationTokenSource m_source;

    private List<Thread> m_thrds;

    private bool m_hasException;

    private Exception m_exception;

    private bool m_cancelIsSet;

    public static int Threads
    {
        get
        {
            setup();
            return ms_threads;
        }
    }

    internal static CalculationAssistantAdHoc Instance()
    {
        setup();
        lock (ms_lockInstanceCounter)
        {
            if (ms_instancesCounter < ms_maxInstances)
            {
                Interlocked.Increment(ref ms_instancesCounter);
                return new CalculationAssistantAdHoc();
            }
            return null;
        }
    }

    private CalculationAssistantAdHoc()
    {
        m_queue = new ConcurrentQueueWithSignal();
        m_hasException = false;
        m_exception = null;
    }

    public void StartTasks(List<TaskDefinition> listOfTasks)
    {
        if (listOfTasks.Count > ms_threads)
        {
            throw new ArgumentException($"Number of tasks {listOfTasks.Count} exceeds the limit {ms_threads}.");
        }
        m_innerTaskList = new List<InnerTask>(listOfTasks.Count);
        m_source = new CancellationTokenSource();
        _ = m_source.Token;
        m_hasException = false;
        m_exception = null;
        m_cancelIsSet = false;
        for (int i = 0; i < listOfTasks.Count; i++)
        {
            m_innerTaskList.Add(new InnerTask
            {
                taskDef = listOfTasks[i],
                cancelTokenSrc = m_source,
                taskId = i
            });
        }
        startThreads(m_innerTaskList);
        waitResultTasks();
    }

    public bool HasException(out Exception exception)
    {
        exception = (m_hasException ? m_exception : null);
        return m_hasException;
    }

    public void Abort()
    {
        if (!m_cancelIsSet)
        {
            m_source.Cancel();
            m_cancelIsSet = true;
        }
    }

    public void Release()
    {
        lock (ms_lockInstanceCounter)
        {
            Interlocked.Decrement(ref ms_instancesCounter);
        }
    }

    private void waitResultTasks()
    {
        int num = m_innerTaskList.Count;
        bool flag = false;
        while (num > 0)
        {
            bool isValid = m_queue.Wait(ms_waitTimeout);
            if (!flag)
            {
                flag = Rts.IsCancellationRequested();
                if (flag)
                {
                    m_hasException = true;
                    m_exception = new AbortRequestedException();
                    if (!m_cancelIsSet)
                    {
                        m_source.Cancel();
                        m_cancelIsSet = true;
                    }
                }
            }
            if (!isValid || !m_queue.getFromQueue(out var param))
            {
                continue;
            }
            num--;
            if (!m_hasException && param.exception != null)
            {
                if (!m_cancelIsSet)
                {
                    m_source.Cancel();
                    m_cancelIsSet = true;
                }
                m_hasException = true;
                m_exception = param.exception;
            }
        }
    }
}
