#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class CommonDragger
{
    [MirrorVerified("PtcControlsCoreCommonDraggerMechanicalDifferentialTests")]
    protected Point _startPoint;

    [MirrorVerified("PtcControlsCoreCommonDraggerMechanicalDifferentialTests")]
    protected Vector OffsetTo(Point p) => p - _startPoint;

    [MirrorVerified("PtcControlsCoreCommonDraggerMechanicalDifferentialTests")]
    protected virtual void MouseUp_ResizeRightMode(Vector offset, UIElement element) => MouseMove_ResizeRightMode(offset, element);

    [MirrorVerified("PtcControlsCoreCommonDraggerMechanicalDifferentialTests")]
    protected virtual void MouseUp_ResizeBottomMode(Vector offset, UIElement element) => MouseMove_ResizeBottomMode(offset, element);
}
