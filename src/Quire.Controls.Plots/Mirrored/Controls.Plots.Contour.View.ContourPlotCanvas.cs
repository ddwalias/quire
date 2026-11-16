using System.Windows;
using System.Windows.Controls;
using Quire.Core;

namespace Quire.Controls.Plots.Contour.View;

public partial class ContourPlotCanvas : Panel
{
    public ContourPlotCanvas()
    {
    }

    private ContourPlotControl GetParent()
    {
        ContourPlotControl obj = Parent as ContourPlotControl;
        DebugTools.DevAssert(obj != null, null);
        return obj;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (!GetParent().HasWorksheetProperties)
        {
            return base.MeasureOverride(availableSize);
        }
        return GetParent().CanvasMeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (!GetParent().HasWorksheetProperties)
        {
            return base.ArrangeOverride(finalSize);
        }
        return GetParent().CanvasArrangeOverride(finalSize);
    }
}
