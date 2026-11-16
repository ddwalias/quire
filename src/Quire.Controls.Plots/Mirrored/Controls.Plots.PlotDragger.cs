#nullable disable
#pragma warning disable

using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Plots.PlotCommands;

namespace Quire.Controls.Plots;

public partial class PlotDragger
{
    public PlotDragger(PlotControl container, Point startPosition, DragMode mode, UIElement elementToDrag)
        : this(container, startPosition, mode, elementToDrag, automation: false)
    {
    }

    public PlotDragger(PlotControl container, Point startPosition, DragMode mode, UIElement elementToDrag, bool automation)
        : base(container, startPosition, false, mode, elementToDrag)
    {
        onDragStart = null;
        onDragEnd = null;
        onDragProgress = null;
        if (_mode == DragMode.ResizeRight)
        {
            onDragStart = CacheOriginalWidths;
            onDragEnd = ResizeRightMode;
            onDragProgress = ResizeRightMode;
        }
        else
        {
            return;
        }
        if (elementToDrag is EquationList equationList)
        {
            // Release IL requires callvirt on newobj without a local slot.
            (new LegendResizeCommand(equationList) switch { var c => c }).Execute();
        }
        if (elementToDrag is PlotLegend plotLegend)
        {
            // Release IL requires callvirt on newobj without a local slot.
            (new PlotLegendResizeCommand(plotLegend) switch { var c => c }).Execute();
        }
        if (automation)
        {
            CacheOriginalWidths();
        }
        else
        {
            StartCaptureEvents(_startPoint);
        }
    }

    private void ResizeRightMode(Point endPoint)
    {
        Vector offset = OffsetTo(endPoint);
        MouseMove_ResizeRightMode(offset, _elementsToDrag[0]);
    }

    public Point ResizeBy(Vector offset)
    {
        Point val = _startPoint + offset;
        ResizeRightMode(val);
        return val;
    }

    public void ResizeEnd(Point currentPosition)
    {
        ResizeRightMode(currentPosition);
    }
}
