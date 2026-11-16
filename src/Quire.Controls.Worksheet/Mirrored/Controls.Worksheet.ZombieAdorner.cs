using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Quire.Controls.Worksheet;

internal partial class ZombieAdorner : Adorner
{
    internal VisualBrush Visual;
    private readonly Rect _bounds;

    public ZombieAdorner(UIElement adornedElement, VisualBrush adornerContents, Rect bounds)
        : base(adornedElement)
    {
        _bounds = bounds;
        Visual = adornerContents;
        IsHitTestVisible = false;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        drawingContext.DrawRectangle(Visual, null, _bounds);
    }
}
