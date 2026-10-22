#nullable disable
#pragma warning disable

namespace Quire;

partial interface IPositionProvider
{
    double ActualHeight { get; }
    double ActualWidth { get; }
    bool IsVisible { get; }
    global::System.Windows.Point TranslatePoint(global::System.Windows.Point point, global::System.Windows.UIElement relativeTo);
    global::System.Windows.Point PointToScreen(global::System.Windows.Point point);
}
