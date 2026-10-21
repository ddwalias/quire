using System.Runtime.InteropServices;

namespace Quire.utl_mgd;

public partial class SafeEvent
{
    public unsafe SafeEvent(string A_0)
        : base(default(string), default(void*))
    {
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        try
        {
            m_name = $"SafeEvent {A_0}";
            m_handle = (void*)((delegate* unmanaged[Stdcall]<IntPtr, int, int, char*, IntPtr>)NativeLibrary.GetExport(module, "CreateEventW"))(IntPtr.Zero, 1, 0, null);
        }
        finally
        {
            NativeLibrary.Free(module);
        }
    }

    public unsafe void Set()
    {
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        try
        {
            ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "SetEvent"))((IntPtr)m_handle);
        }
        finally
        {
            NativeLibrary.Free(module);
        }

        GC.KeepAlive(this);
    }

    public unsafe void Reset()
    {
        IntPtr module = NativeLibrary.Load("kernel32.dll");
        try
        {
            ((delegate* unmanaged[Stdcall]<IntPtr, int>)NativeLibrary.GetExport(module, "ResetEvent"))((IntPtr)m_handle);
        }
        finally
        {
            NativeLibrary.Free(module);
        }

        GC.KeepAlive(this);
    }
}
