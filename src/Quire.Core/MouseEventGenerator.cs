#nullable disable
using System.Windows;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using Quire.Mirror;
using Quire.Mirror;
using Quire.Win32;
using Quire.Win32;

namespace Quire;

public partial class MouseEventGenerator
{
    private const SendMouseInputFlags _moveFlags = (SendMouseInputFlags)49153;
    private const SendMouseInputFlags _moveRelativeFlags = (SendMouseInputFlags)16385;
    private const SendMouseInputFlags _downHereFlags = (SendMouseInputFlags)16386;
    private const SendMouseInputFlags _upHereFlags = (SendMouseInputFlags)16388;
    private const SendMouseInputFlags _downFlags = (SendMouseInputFlags)49154;
    private const SendMouseInputFlags _upFlags = (SendMouseInputFlags)49156;

    public static void GenerateMouseMove(Point point)
    {
        NormalizeCoordinates(point, out var x, out var y);
        User32.POINT pt = default;
        User32.GetCursorPos(ref pt);
        NormalizeCoordinates(new Point(pt.x, pt.y), out var x2, out var y2);
        int num = System.Math.Max(System.Math.Abs(x - x2), System.Math.Abs(y - y2));
        int num2 = 1;
        if (num > 50)
        {
            num2 = System.Math.Min(100, num / 50);
        }
        int num3 = num / num2 - 1;
        double num4 = (double)(x - x2) / (double)num3;
        double num5 = (double)(y - y2) / (double)num3;
        for (int i = 0; i < num3; i++)
        {
            int x3 = x2 + (int)((double)i * num4);
            int y3 = y2 + (int)((double)i * num5);
            SendMouseInput(x3, y3, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute | SendMouseInputFlags.VirtualDesk);
            Quire.Wpf.WpfUtils.DoEvents();
        }
        SendMouseInput(x, y, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute | SendMouseInputFlags.VirtualDesk);
        Quire.Wpf.WpfUtils.DoEvents();
    }

    [MirrorVerified("PtcMouseEventGeneratorMechanicalDifferentialTests")]
    public static void GenerateMouseUp() => SendMouseInput(0, 0, (SendMouseInputFlags)16388);

    [MirrorVerified("PtcMouseEventGeneratorMechanicalDifferentialTests")]
    public static void GenerateMouseDown() => SendMouseInput(0, 0, (SendMouseInputFlags)16386);

    [MirrorVerified("PtcMouseEventGeneratorMechanicalDifferentialTests")]
    public static void GenerateMouseClick(Point screenPoint) => GenerateMouseClick(screenPoint, 1);

    [MirrorVerified("Release IL")]
    public static void GenerateMouseClick(Point wpfScreenPoint, int clickCount)
    {
        NormalizeCoordinates(wpfScreenPoint, out int num, out int num2);
        for (int i = 0; i < clickCount; i++)
        {
            MouseClickFromNormalizedCoordinates(num, num2);
        }
        Quire.Wpf.WpfUtils.DoEvents();
        System.Threading.Thread.Sleep(unchecked((int)(User32.GetDoubleClickTime() + 1)));
    }

    [MirrorVerified("PtcMouseEventGeneratorMechanicalDifferentialTests")]
    public MouseEventGenerator() { }

    [MirrorVerified("Release IL")]
    public static void GenerateMouseClick(double screenX, double screenY, int repeatCount) => GenerateMouseClick(new Point(screenX, screenY), repeatCount);

    [MirrorVerified("Release IL")]
    public static void GenerateMouseClick(double screenX, double screenY) => GenerateMouseClick(new Point(screenX, screenY), 1);

    [MirrorVerified("Release IL")]
    public static void GenerateMouseDrag(Point startScreenPoint, Point endScreenPoint)
    {
        GenerateMouseMove(startScreenPoint);
        GenerateMouseDown();
        GenerateMouseMove(endScreenPoint);
        GenerateMouseUp();
    }

    [MirrorVerified("Release IL")]
    private static void MouseClickFromNormalizedCoordinates(int x, int y)
    {
        SendMouseInput(x, y, (SendMouseInputFlags)49153);
        SendMouseInput(0, 0, (SendMouseInputFlags)16386);
        SendMouseInput(0, 0, (SendMouseInputFlags)16388);
    }

    [MirrorVerified("Release IL")]
    private static void SendMouseInput(int x, int y, SendMouseInputFlags flags)
    {
        User32.INPUT input = default;
        input.type = 0;
        input.union.mouseInput.dx = x;
        input.union.mouseInput.dy = y;
        input.union.mouseInput.mouseData = 0;
        input.union.mouseInput.dwFlags = (int)flags;
        input.union.mouseInput.time = 0;
        input.union.mouseInput.dwExtraInfo = new IntPtr(0);
        if (User32.SendInput(1, ref input, Marshal.SizeOf(input)) == 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    [MirrorVerified("Release IL")]
    private static void NormalizeCoordinates(Point rawPoint, out int x, out int y)
    {
        int num = User32.GetSystemMetrics(78);
        int num2 = User32.GetSystemMetrics(79);
        int num3 = User32.GetSystemMetrics(76);
        int num4 = User32.GetSystemMetrics(77);
        double num5 = (rawPoint.X - (double)num3) * 65536.0 / (double)num + (double)(65536 / (num * 2));
        double num6 = (rawPoint.Y - (double)num4) * 65536.0 / (double)num2 + (double)(65536 / (num2 * 2));
        x = (int)num5;
        y = (int)num6;
    }
}
