#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Win32;

public static partial class User32
{
    public static readonly nint INVALID_HANDLE_VALUE = new nint(-1);
    public const int VK_SHIFT = 16;
    public const int VK_CONTROL = 17;
    public const int VK_MENU = 18;
    public const int KEYEVENTF_EXTENDEDKEY = 1;
    public const int KEYEVENTF_KEYUP = 2;
    public const int KEYEVENTF_UNICODE = 4;
    public const int KEYEVENTF_SCANCODE = 8;
    public const int MOUSEEVENTF_VIRTUALDESK = 16384;
    public const int EM_SETSEL = 177;
    public const int EM_GETLINECOUNT = 186;
    public const int EM_LINEFROMCHAR = 201;
    public const int SM_CXMAXTRACK = 59;
    public const int SM_CYMAXTRACK = 60;
    public const int SM_XVIRTUALSCREEN = 76;
    public const int SM_YVIRTUALSCREEN = 77;
    public const int SM_CXVIRTUALSCREEN = 78;
    public const int SM_CYVIRTUALSCREEN = 79;
    public const int SM_SWAPBUTTON = 23;
    public const int GWL_STYLE = -16;
    public const int GWL_EXSTYLE = -20;
    public const int WS_VSCROLL = 2097152;
    public const int WS_HSCROLL = 1048576;
    public const int ES_MULTILINE = 4;
    public const int ES_AUTOVSCROLL = 64;
    public const int ES_AUTOHSCROLL = 128;
    public const int INPUT_MOUSE = 0;
    public const int INPUT_KEYBOARD = 1;
    public const uint MF_BYCOMMAND = 0u;
    public const uint SC_RESTORE = 61728u;
    public const uint SC_MINIMIZE = 61472u;
    public const uint SC_MAXIMIZE = 61488u;

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
