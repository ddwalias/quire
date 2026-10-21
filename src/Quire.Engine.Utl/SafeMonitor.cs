using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.utl_mgd;

public partial class SafeMonitor
{
    private static Hashtable s_csecmap;
    private static Stack s_csecpool;
    private static SafeMutex s_mutex;

    private partial class CSecRecord
    {
        public CSecRecord(long c, int cn)
        {
            csec = c;
            count = cn;
        }
    }

    static unsafe SafeMonitor()
    {
        s_csecmap = new Hashtable();
        s_csecpool = new Stack();
        s_mutex = new SafeMutex("SafeMonitor");
        IntPtr module = NativeLibrary.Load("ucrtbase.dll");
        try
        {
            ((delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<int>, IntPtr>)NativeLibrary.GetExport(module, "_onexit"))(&on_exit);
        }
        finally
        {
            NativeLibrary.Free(module);
        }
    }

    public SafeMonitor()
    {
    }

    private static unsafe nint GetCriticalSection(object o, int inc)
    {
        s_mutex.WaitOne();
        try
        {
            nint ptr = 0;
            CSecRecord cSecRecord = s_csecmap[o] as CSecRecord;
            if (cSecRecord != null)
            {
                ptr = (nint)cSecRecord.csec;
                cSecRecord.count += inc;
            }
            else if (s_csecpool.Count == 0)
            {
                ptr = (nint)NativeMemory.Alloc(40u);
                IntPtr module = NativeLibrary.Load("kernel32.dll");
                try
                {
                    ((delegate* unmanaged[Stdcall]<IntPtr, void>)NativeLibrary.GetExport(module, "InitializeCriticalSection"))((IntPtr)ptr);
                }
                finally
                {
                    NativeLibrary.Free(module);
                }

                cSecRecord = new CSecRecord(ptr, inc);
                s_csecmap[o] = cSecRecord;
            }
            else
            {
                ptr = (nint)(long)s_csecpool.Pop();
                cSecRecord = new CSecRecord(ptr, inc);
                s_csecmap[o] = cSecRecord;
            }

            if (cSecRecord.count == 0)
            {
                s_csecmap.Remove(o);
                long csec = cSecRecord.csec;
                s_csecpool.Push(csec);
            }

            return ptr;
        }
        finally
        {
            s_mutex.ReleaseMutex();
        }
    }

    private static unsafe void Free()
    {
        s_mutex.WaitOne();
        try
        {
            while (s_csecpool.Count != 0)
            {
                nint ptr = (nint)(long)s_csecpool.Pop();
                IntPtr module = NativeLibrary.Load("kernel32.dll");
                try
                {
                    ((delegate* unmanaged[Stdcall]<IntPtr, void>)NativeLibrary.GetExport(module, "DeleteCriticalSection"))((IntPtr)ptr);
                }
                finally
                {
                    NativeLibrary.Free(module);
                }

                NativeMemory.Free((void*)ptr);
            }
        }
        finally
        {
            s_mutex.Free();
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe int on_exit()
    {
        Free();
        return 0;
    }

    public static unsafe void Enter(object o)
    {
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        try
        {
            ((delegate* unmanaged[Stdcall]<IntPtr, void>)NativeLibrary.GetExport(module, "EnterCriticalSection"))((IntPtr)GetCriticalSection(o, 1));
        }
        finally
        {
            NativeLibrary.Free(module);
        }
    }

    public static unsafe bool TryEnter(object o)
    {
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        int result;
        try
        {
            result = ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "TryEnterCriticalSection"))((IntPtr)GetCriticalSection(o, 1));
        }
        finally
        {
            NativeLibrary.Free(module);
        }

        if (result != 0)
        {
            return true;
        }

        GetCriticalSection(o, -1);
        return false;
    }

    public static unsafe void Exit(object o)
    {
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        try
        {
            ((delegate* unmanaged[Stdcall]<IntPtr, void>)NativeLibrary.GetExport(module, "LeaveCriticalSection"))((IntPtr)GetCriticalSection(o, -1));
        }
        finally
        {
            NativeLibrary.Free(module);
        }
    }
}
