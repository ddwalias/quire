using System.Runtime.InteropServices;
using System.Text;

namespace Quire.utl_mgd;

public partial class Logging
{
    public static unsafe bool IsLoggingEnabled()
    {
        IntPtr module = NativeLibrary.Load("UTL.DLL");
        try
        {
            byte enabled = ((delegate* unmanaged[Cdecl]<byte>)NativeLibrary.GetExport(module, "?IsLoggingEnabled@utl@@YA_NXZ"))();
            return enabled != 0;
        }
        finally
        {
            NativeLibrary.Free(module);
        }
    }

    public static unsafe void Log(string logname, string message)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(logname.ToString());
        byte[] messageBytes = Encoding.UTF8.GetBytes(message.ToString());
        IntPtr utlModule = NativeLibrary.Load("UTL.DLL");
        try
        {
            IntPtr stream;
            fixed (byte* name = nameBytes)
            {
                IntPtr* nameView = stackalloc IntPtr[2];
                nameView[0] = (IntPtr)name;
                nameView[1] = (IntPtr)(name + nameBytes.Length);
                stream = ((delegate* unmanaged[Cdecl]<IntPtr, IntPtr>)NativeLibrary.GetExport(utlModule, "?Log@utl@@YAAEAV?$basic_ostream@DU?$char_traits@D@std@@@std@@V?$Substring@$$CBD@@@Z"))((IntPtr)nameView);
            }

            IntPtr streamModule = NativeLibrary.Load("msvcp140.dll");
            try
            {
                delegate* unmanaged[Cdecl]<IntPtr, byte, IntPtr> put = (delegate* unmanaged[Cdecl]<IntPtr, byte, IntPtr>)NativeLibrary.GetExport(streamModule, "?put@?$basic_ostream@DU?$char_traits@D@std@@@std@@QEAAAEAV12@D@Z");
                fixed (byte* data = messageBytes)
                {
                    for (int i = 0; i < messageBytes.Length; i++)
                    {
                        put(stream, data[i]);
                    }
                }
            }
            finally
            {
                NativeLibrary.Free(streamModule);
            }
        }
        finally
        {
            NativeLibrary.Free(utlModule);
        }
    }

    public static unsafe void LogLine(string logname, string message)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(logname.ToString());
        byte[] messageBytes = Encoding.UTF8.GetBytes(message.ToString());
        IntPtr utlModule = NativeLibrary.Load("UTL.DLL");
        try
        {
            IntPtr stream;
            fixed (byte* name = nameBytes)
            {
                IntPtr* nameView = stackalloc IntPtr[2];
                nameView[0] = (IntPtr)name;
                nameView[1] = (IntPtr)(name + nameBytes.Length);
                stream = ((delegate* unmanaged[Cdecl]<IntPtr, IntPtr>)NativeLibrary.GetExport(utlModule, "?Log@utl@@YAAEAV?$basic_ostream@DU?$char_traits@D@std@@@std@@V?$Substring@$$CBD@@@Z"))((IntPtr)nameView);
            }

            IntPtr streamModule = NativeLibrary.Load("msvcp140.dll");
            try
            {
                delegate* unmanaged[Cdecl]<IntPtr, byte, IntPtr> put = (delegate* unmanaged[Cdecl]<IntPtr, byte, IntPtr>)NativeLibrary.GetExport(streamModule, "?put@?$basic_ostream@DU?$char_traits@D@std@@@std@@QEAAAEAV12@D@Z");
                fixed (byte* data = messageBytes)
                {
                    for (int i = 0; i < messageBytes.Length; i++)
                    {
                        put(stream, data[i]);
                    }
                }

                put(stream, 10);
                ((delegate* unmanaged[Cdecl]<IntPtr, IntPtr>)NativeLibrary.GetExport(streamModule, "?flush@?$basic_ostream@DU?$char_traits@D@std@@@std@@QEAAAEAV12@XZ"))(stream);
            }
            finally
            {
                NativeLibrary.Free(streamModule);
            }
        }
        finally
        {
            NativeLibrary.Free(utlModule);
        }
    }
}
