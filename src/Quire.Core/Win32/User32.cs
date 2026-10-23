#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Win32;

public static partial class User32
{
    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetCursorPos(ref POINT pt);

    partial struct HWND
    {
        [MirrorVerified("PtcWin32User32MechanicalDifferentialTests")]
        private nint _value;

        [MirrorVerified("PtcWin32User32MechanicalDifferentialTests")]
        public static implicit operator nint(User32.HWND hwnd) => hwnd._value;
    }

    partial struct HWND
    {
        [MirrorVerified("Release IL")]
        public static bool operator ==(User32.HWND lhs, User32.HWND rhs) => lhs._value == rhs._value;

        [MirrorVerified("Release IL")]
        public static bool operator !=(HWND lhs, HWND rhs) => lhs._value != rhs._value;

        [MirrorVerified("Release IL")]
        public override int GetHashCode() => (int)_value;
    }
}
