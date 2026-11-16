using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Quire.Controls.Worksheet;

internal partial class DragZombieAdorner : Adorner
{
    private Rect _childBoundaries;
    private readonly VisualBrush _visual;

    public DragZombieAdorner(UIElement elementToAdorn, Visual originalAdorner, Rect elementBoundaries)
        : base(elementToAdorn)
    {
        _visual = new VisualBrush(originalAdorner);
        _childBoundaries = elementBoundaries;
        IsHitTestVisible = false;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        drawingContext.DrawRectangle(_visual, null, new Rect(_childBoundaries.TopLeft, _childBoundaries.Size));
    }
}
