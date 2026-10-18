#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire;

public partial class FrameworkElementPositionWrapper
{
    [MirrorVerified("PtcFrameworkElementPositionWrapperMechanicalDifferentialTests")]
    private FrameworkElement _element;

    [MirrorVerified("PtcFrameworkElementPositionWrapperMechanicalDifferentialTests")]
    public FrameworkElementPositionWrapper(FrameworkElement element)
    {
        _element = element;
    }

    [MirrorVerified("PtcFrameworkElementPositionWrapperMechanicalDifferentialTests")]
    public Point TranslatePoint(Point point, UIElement relativeTo) => _element.TranslatePoint(point, relativeTo);

    [MirrorVerified("PtcFrameworkElementPositionWrapperMechanicalDifferentialTests")]
    public Point PointToScreen(Point point) => _element.PointToScreen(point);

    [MirrorVerified("PtcFrameworkElementPositionWrapperMechanicalDifferentialTests")]
    public double ActualHeight => _element.ActualHeight;

    [MirrorVerified("PtcFrameworkElementPositionWrapperMechanicalDifferentialTests")]
    public double ActualWidth => _element.ActualWidth;

    [MirrorVerified("PtcFrameworkElementPositionWrapperMechanicalDifferentialTests")]
    public bool IsVisible => _element.IsVisible;
}
