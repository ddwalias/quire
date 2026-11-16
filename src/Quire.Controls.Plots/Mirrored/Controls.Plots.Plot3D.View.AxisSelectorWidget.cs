using System;
using System.Windows;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class AxisSelectorWidget : VisualAdorner, IDisposable
{
    public AxisSelectorWidget(UIElement adornedElement, UIElement visualChild)
        : base(adornedElement, false)
    {
        IsHitTestVisible = true;
        AddVisualChild(visualChild);
    }

    protected UIElement VisualChild
    {
        get
        {
            if (VisualChildren.Count != 1)
            {
                return null;
            }
            return VisualChildren[0];
        }
    }

    protected override Size MeasureOverride(Size constraint)
    {
        if (DebugTools.AssertNotNull(VisualChild, null).Failed)
        {
            return base.MeasureOverride(constraint);
        }
        VisualChild.Measure(constraint);
        return VisualChild.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (DebugTools.AssertNotNull(VisualChild, null).Failed)
        {
            return base.ArrangeOverride(finalSize);
        }
        Size desiredSize = VisualChild.DesiredSize;
        Point point = new Point(AdornedElement.RenderSize.Width, -desiredSize.Height);
        VisualChild.Arrange(new Rect(point, desiredSize));
        return finalSize;
    }

    public void Dispose()
    {
        Remove();
        VisualChildren.Clear();
    }
}
