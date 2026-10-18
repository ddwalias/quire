#nullable disable
using Quire.Mirror;

namespace Quire.Win32;

public static partial class User32
{
    partial struct HWND
    {
        [MirrorVerified("PtcWin32User32MechanicalDifferentialTests")]
        private nint _value;

        [MirrorVerified("PtcWin32User32MechanicalDifferentialTests")]
        public static implicit operator nint(User32.HWND hwnd) => hwnd._value;
    }
}
