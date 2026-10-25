#nullable disable
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Quire.Controls.Core;

public partial class SelectionDraggerRubberBand : Adorner
{
    private Rect _bounds;

    private readonly Pen _pen;

    public Rect Bounds
    {
        set
        {
            _bounds = value;
            InvalidateVisual();
        }
    }

    public SelectionDraggerRubberBand(UIElement adornedElement)
        : this(adornedElement, Brushes.LightGray)
    {
    }

    public SelectionDraggerRubberBand(UIElement adornedElement, Brush brush)
        : base(adornedElement)
    {
        IsHitTestVisible = false;
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
        if (adornerLayer != null)
        {
            adornerLayer.Add(this);
        }
        _pen = new Pen(brush, 1.0)
        {
            DashStyle = DashStyles.Dash
        };
    }

    public void Remove()
    {
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
        if (adornerLayer != null)
        {
            adornerLayer.Remove(this);
        }
    }

    protected override void OnRender(DrawingContext dc)
    {
        dc.DrawRectangle(null, _pen, _bounds);
    }
}
