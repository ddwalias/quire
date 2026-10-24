#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Win32;

public static partial class User32
{
    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    public static extern int SendInput(int nInputs, ref INPUT mi, int cbSize);

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetCursorPos(ref POINT pt);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public static extern int MapVirtualKey(int nVirtKey, int nMapType);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public static extern int GetAsyncKeyState(int nVirtKey);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public static extern int GetKeyState(int nVirtKey);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
    internal static extern int GetKeyboardState(byte[] keystate);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, EntryPoint = "keybd_event", ExactSpelling = true)]
    internal static extern void Keybd_event(byte vk, byte scan, int flags, int extrainfo);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
    internal static extern int SetKeyboardState(byte[] keystate);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public static extern nint SendMessage(HWND hWnd, int nMsg, nint wParam, nint lParam);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public static extern nint SendMessage(HWND hWnd, int nMsg, nint wParam, System.Text.StringBuilder lParam);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int metric);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowLong(HWND hWnd, int nIndex);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern uint GetDoubleClickTime();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern nint GetSystemMenu(nint hWnd, bool bRevert);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool DeleteMenu(nint hMenu, uint uPosition, uint uFlags);

    public partial struct HWND
    {
        [MirrorVerified("PtcWin32User32MechanicalDifferentialTests")]
        private nint _value;

        [MirrorVerified("PtcWin32User32MechanicalDifferentialTests")]
        public static implicit operator nint(User32.HWND hwnd) => hwnd._value;
    }

    public partial struct HWND
    {
        [MirrorVerified("Release IL")]
        public static bool operator ==(User32.HWND lhs, User32.HWND rhs) => lhs._value == rhs._value;

        [MirrorVerified("Release IL")]
        public static bool operator !=(HWND lhs, HWND rhs) => lhs._value != rhs._value;

        [MirrorVerified("Release IL")]
        public override int GetHashCode() => (int)_value;

        public override bool Equals(object oCompare)
        {
            HWND hWND = Cast((nint)(HWND)oCompare);
            return _value == hWND._value;
        }
    }

    public partial struct INPUT
    {
        public int type;
        public INPUTUNION union;
    }

    public partial struct INPUTUNION
    {
        public MOUSEINPUT mouseInput;
        public KEYBDINPUT keyboardInput;
    }

    public partial struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public int mouseData;
        public int dwFlags;
        public int time;
        public nint dwExtraInfo;
    }

    public partial struct POINT
    {
        public int x;
        public int y;
    }

    public partial struct KEYBDINPUT
    {
        public short wVk;
        public short wScan;
        public int dwFlags;
        public int time;
        public nint dwExtraInfo;
    }
}
