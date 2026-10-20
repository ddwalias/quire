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
}
