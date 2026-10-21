using System.Runtime.InteropServices;

namespace Quire.utl_mgd;

public partial class SafeMutex
{
    public unsafe SafeMutex(string A_0, int A_1)
        : base(default(string), default(void*))
    {
        string name = "Global\\" + A_0.Replace("\\", "_");
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        try
        {
            fixed (char* pName = name)
            {
                m_handle = (void*)((delegate* unmanaged[Stdcall]<IntPtr, int, char*, IntPtr>)NativeLibrary.GetExport(module, "CreateMutexW"))(IntPtr.Zero, 0, pName);
            }
        }
        finally
        {
            NativeLibrary.Free(module);
        }

        m_name = $"SafeMutex {A_0}";
    }

    public unsafe SafeMutex(string A_0)
        : base(default(string), default(void*))
    {
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        try
        {
            m_handle = (void*)((delegate* unmanaged[Stdcall]<IntPtr, int, char*, IntPtr>)NativeLibrary.GetExport(module, "CreateMutexW"))(IntPtr.Zero, 0, null);
        }
        finally
        {
            NativeLibrary.Free(module);
        }

        m_name = $"SafeMutex {A_0}";
    }

    private unsafe void _bangSafeMutex()
    {
        uint num = 0u;
        while (true)
        {
            IntPtr module = NativeLibrary.Load("kernel32.dll");
            int released;
            try
            {
                released = ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "ReleaseMutex"))((IntPtr)m_handle);
            }
            finally
            {
                NativeLibrary.Free(module);
            }

            if (released == 0)
            {
                break;
            }
            num++;
            if (num == 1000)
            {
                break;
            }
        }

        GC.KeepAlive(this);
    }

    private unsafe void _tildeSafeMutex()
    {
        uint num = 0u;
        while (true)
        {
            IntPtr module = NativeLibrary.Load("kernel32.dll");
            int released;
            try
            {
                released = ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "ReleaseMutex"))((IntPtr)m_handle);
            }
            finally
            {
                NativeLibrary.Free(module);
            }

            if (released == 0)
            {
                break;
            }
            num++;
            if (num == 1000)
            {
                break;
            }
        }

        GC.KeepAlive(this);
    }

    public unsafe void ReleaseMutex()
    {
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        try
        {
            ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "ReleaseMutex"))((IntPtr)m_handle);
        }
        finally
        {
            NativeLibrary.Free(module);
        }
        GC.KeepAlive(this);
    }

    public unsafe void EnsureReleased(uint max_tries)
    {
        uint num = 0u;
        if (max_tries != 0)
        {
            while (true)
            {
                IntPtr module = NativeLibrary.Load("kernel32.dll");
                int released;
                try
                {
                    released = ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "ReleaseMutex"))((IntPtr)m_handle);
                }
                finally
                {
                    NativeLibrary.Free(module);
                }

                if (released == 0)
                {
                    break;
                }
                num++;
                if (num == max_tries)
                {
                    break;
                }
            }
        }

        GC.KeepAlive(this);
    }

    public unsafe void EnsureReleased()
    {
        uint num = 0u;
        while (true)
        {
            IntPtr module = NativeLibrary.Load("kernel32.dll");
            int released;
            try
            {
                released = ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "ReleaseMutex"))((IntPtr)m_handle);
            }
            finally
            {
                NativeLibrary.Free(module);
            }

            if (released == 0)
            {
                break;
            }
            num++;
            if (num == 1000)
            {
                break;
            }
        }

        GC.KeepAlive(this);
    }

    protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
    {
        if (A_0)
        {
            try
            {
                _tildeSafeMutex();
                return;
            }
            finally
            {
                base.Dispose(A_0: true);
            }
        }

        try
        {
            _bangSafeMutex();
        }
        finally
        {
            base.Dispose(A_0: false);
        }
    }

}
