#nullable disable
#pragma warning disable

namespace Quire.Win32;

static partial class User32
{

    partial struct HWND
    {
        public static global::Quire.Win32.User32.HWND NULL
        {
            get
            {
                global::Quire.Win32.User32.HWND hwnd = default;
                hwnd._value = global::System.IntPtr.Zero;
                return hwnd;
            }
        }
        public static global::Quire.Win32.User32.HWND Cast(nint hwnd)
        {
            global::Quire.Win32.User32.HWND result = default;
            result._value = hwnd;
            return result;
        }
    }
}
