using System.Runtime.InteropServices;

namespace Quire.utl_mgd;

public abstract partial class SafeWaitHandle
{
    protected unsafe SafeWaitHandle(string A_0, void* A_1)
    {
        m_name = A_0;
        m_handle = A_1;
    }

    private unsafe void _bangSafeWaitHandle()
    {
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        try
        {
            ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "CloseHandle"))((IntPtr)m_handle);
        }
        finally
        {
            NativeLibrary.Free(module);
        }

        GC.KeepAlive(this);
    }

    private unsafe void _tildeSafeWaitHandle()
    {
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        try
        {
            ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "CloseHandle"))((IntPtr)m_handle);
        }
        finally
        {
            NativeLibrary.Free(module);
        }

        GC.KeepAlive(this);
    }

    public unsafe void WaitOne()
    {
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        try
        {
            ((delegate* unmanaged[Stdcall]<IntPtr, uint, uint>)NativeLibrary.GetExport(module, "WaitForSingleObject"))((IntPtr)m_handle, uint.MaxValue);
        }
        finally
        {
            NativeLibrary.Free(module);
        }

        GC.KeepAlive(this);
    }

    public unsafe bool WaitOneWithTimeout(int A_0)
    {
        if (A_0 == -1)
        {
            IntPtr module = NativeLibrary.Load("kernel32.dll");
            try
            {
                ((delegate* unmanaged[Stdcall]<IntPtr, uint, uint>)NativeLibrary.GetExport(module, "WaitForSingleObject"))((IntPtr)m_handle, uint.MaxValue);
            }
            finally
            {
                NativeLibrary.Free(module);
            }

            GC.KeepAlive(this);
            return true;
        }

        IntPtr module2 = NativeLibrary.Load("kernel32.dll");
        uint waitResult;
        try
        {
            waitResult = ((delegate* unmanaged[Stdcall]<IntPtr, uint, uint>)NativeLibrary.GetExport(module2, "WaitForSingleObject"))((IntPtr)m_handle, (uint)A_0);
        }
        finally
        {
            NativeLibrary.Free(module2);
        }

        bool num = waitResult != 258;
        GC.KeepAlive(this);
        return num;
    }

    public unsafe bool WaitTwoWithTimeout(void* A_0)
    {
        if ((nint)A_0 == -1L)
        {
            IntPtr module = NativeLibrary.Load("kernel32.dll");
            try
            {
                ((delegate* unmanaged[Stdcall]<IntPtr, uint, uint>)NativeLibrary.GetExport(module, "WaitForSingleObject"))((IntPtr)m_handle, uint.MaxValue);
            }
            finally
            {
                NativeLibrary.Free(module);
            }

            GC.KeepAlive(this);
            return true;
        }

        IntPtr[] handles = { (IntPtr)m_handle, (IntPtr)A_0 };
        IntPtr module2 = NativeLibrary.Load("kernel32.dll");
        uint waitResult;
        try
        {
            fixed (IntPtr* pHandles = handles)
            {
                waitResult = ((delegate* unmanaged[Stdcall]<uint, IntPtr*, int, uint, uint>)NativeLibrary.GetExport(module2, "WaitForMultipleObjects"))(2u, pHandles, 0, uint.MaxValue);
            }
        }
        finally
        {
            NativeLibrary.Free(module2);
        }

        if (waitResult == 0)
        {
            GC.KeepAlive(this);
            return true;
        }

        return false;
    }

    public void Free()
    {
        ((IDisposable)this)?.Dispose();
        GC.KeepAlive(this);
    }

    public unsafe void* GetUnmanagedHandle() => m_handle;

    public override string ToString() => m_name;

    protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
    {
        if (A_0)
        {
            _tildeSafeWaitHandle();
            return;
        }

        _bangSafeWaitHandle();
    }

    public void Dispose()
    {
        Dispose(A_0: true);
        GC.SuppressFinalize(this);
        GC.KeepAlive(this);
    }

    ~SafeWaitHandle()
    {
        Dispose(A_0: false);
    }
}
