#nullable disable
using System.Windows;
using Quire.Mirror;
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
}
