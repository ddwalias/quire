#nullable disable
using System.Windows;
using System.Windows;
using Quire.Mirror;
using Quire.Mirror;
using Quire.Win32;
using Quire.Win32;

namespace Quire;

public partial class MouseEventGenerator
{
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
