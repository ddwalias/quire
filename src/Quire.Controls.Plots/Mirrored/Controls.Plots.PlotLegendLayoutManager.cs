using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.Controls.Plots;

internal partial class PlotLegendLayoutManager
{
    public PlotLegendLayoutManager()
    {
    }

    private PlotLegendLayoutManager(PlotLegendLayoutManager other)
        : base(other)
    {
    }

    protected override MaxMatrixWidth[] CalcMaxDecimalPerColumn(IList<LayoutMetrics> cellsInfo, UIElementCollection cellsContent, int rows)
    {
        return new MaxMatrixWidth[cellsContent.Count];
    }

    public override object Clone()
    {
        return new PlotLegendLayoutManager(this);
    }

    public int GetNumberOfColumnsFitWidth(double newWidth, UIElementCollection children, ColumnAlignments columnAlignment)
    {
        if (_cellsInfo == null)
        {
            return Columns;
        }

        var clone = (PlotLegendLayoutManager)Clone();
        int maxCols = Math.Min(3, _cellsInfo.Count);
        if (DoubleComparer.DefaultDoubleComparer.Compare(newWidth, clone.MatrixSize.Width) > 0)
        {
            while (clone.Columns < maxCols)
            {
                ++clone.Columns;
                clone.Measure(children, columnAlignment);
                if (DoubleComparer.DefaultDoubleComparer.Compare(clone.MatrixSize.Width, newWidth) > 0)
                {
                    --clone.Columns;
                    break;
                }
            }
        }
        else
        {
            while (DoubleComparer.DefaultDoubleComparer.Compare(newWidth, clone.MatrixSize.Width) < 0 && clone.Columns > 1)
            {
                --clone.Columns;
                clone.Measure(children, columnAlignment);
            }
        }

        return clone.Columns;
    }
}
