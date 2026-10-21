using System.Runtime.InteropServices;
using System.Threading;

namespace Quire.utl_mgd;

public partial class MessageQueue<Owner, Data, Msg>
{
    private unsafe static void MsgLoop(object obj)
    {
        Tuple<WeakReference, Data, Mailbox<Msg>, OnInit, OnMessage, long> tuple = obj as Tuple<WeakReference, Data, Mailbox<Msg>, OnInit, OnMessage, long>;
        WeakReference item = tuple.Item1;
        Data item2 = tuple.Item2;
        Mailbox<Msg> item3 = tuple.Item3;
        OnInit item4 = tuple.Item4;
        OnMessage item5 = tuple.Item5;
        void* ptr = (void*)tuple.Item6;
        if (item4 != null)
        {
            object target = item.Target;
            if (target != null)
            {
                item4((Owner)target, item2);
            }
        }

        Msg msg;
        object target2;
        do
        {
            msg = default;
            if (!item3.GetMessage(ref msg, ptr))
            {
                break;
            }

            target2 = item.Target;
        }
        while (target2 != null && item5((Owner)target2, item2, msg));

        if ((nint)ptr != -1L)
        {
            IntPtr module = NativeLibrary.Load("kernel32.dll");
            try
            {
                ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "CloseHandle"))((IntPtr)ptr);
            }
            finally
            {
                NativeLibrary.Free(module);
            }
        }
    }

    private unsafe void Init(int stack_size)
    {
        m_mailbox = new Mailbox<Msg>();
        m_death_handle = (void*)ulong.MaxValue;
        Thread thread = new Thread(MsgLoop, stack_size);
        thread.IsBackground = true;
        m_thread = thread;
    }

    public MessageQueue(Owner owner, Data data, OnInit oi, OnMessage om)
    {
        Init(1048576);
        Start(owner, data, oi, om, bStopThreadOnFinalize: true);
    }

    public MessageQueue(Owner owner, Data data, OnInit oi, OnMessage om, int stack_size)
    {
        Init(stack_size);
        Start(owner, data, oi, om, bStopThreadOnFinalize: true);
    }

    public MessageQueue(int stack_size) => Init(stack_size);

    private void _bangMessageQueue() => Stop();

    private void _tildeMessageQueue() => _bangMessageQueue();

    public unsafe void Start(Owner owner, Data data, OnInit oi, OnMessage om, [MarshalAs(UnmanagedType.U1)] bool bStopThreadOnFinalize)
    {
        if (m_thread.Name == null)
        {
            Thread thread = m_thread;
            string text = ")";
            string text2 = owner.ToString();
            string name = string.Concat("MsgLoop(" + text2, text);
            thread.Name = name;
        }

        if (bStopThreadOnFinalize)
        {
            IntPtr module = NativeLibrary.Load("kernel32.dll");
            try
            {
                m_death_handle = (void*)((delegate* unmanaged[Stdcall]<IntPtr, int, int, char*, IntPtr>)NativeLibrary.GetExport(module, "CreateEventW"))(IntPtr.Zero, 1, 0, null);
            }
            finally
            {
                NativeLibrary.Free(module);
            }
        }

        Thread thread2 = m_thread;
        void* death_handle = m_death_handle;
        Tuple<WeakReference, Data, Mailbox<Msg>, OnInit, OnMessage, long> parameter = Tuple.Create(item3: m_mailbox, item1: new WeakReference(owner), item2: data, item4: oi, item5: om, item6: (long)(nint)death_handle);
        thread2.Start(parameter);
        GC.KeepAlive(this);
    }

    public unsafe void Stop()
    {
        if ((nint)m_death_handle != -1L)
        {
            IntPtr module = NativeLibrary.Load("kernel32.dll");
            try
            {
                ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "SetEvent"))((IntPtr)m_death_handle);
            }
            finally
            {
                NativeLibrary.Free(module);
            }

            m_death_handle = (void*)ulong.MaxValue;
        }
    }

    public Mailbox<Msg> GetMailbox() => m_mailbox;

    public Thread GetThread() => m_thread;

    protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
    {
        if (A_0)
        {
            _tildeMessageQueue();
            return;
        }

        _bangMessageQueue();
    }

    public void Dispose()
    {
        Dispose(A_0: true);
        GC.SuppressFinalize(this);
        GC.KeepAlive(this);
    }

    ~MessageQueue() => Dispose(A_0: false);
}
