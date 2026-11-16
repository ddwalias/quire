using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.Controls.Plots;

internal partial class EquationListLayoutManager : TableLayoutManager
{
    public EquationListLayoutManager()
    {
    }

    private EquationListLayoutManager(EquationListLayoutManager other)
        : base(other)
    {
    }

    protected override MaxMatrixWidth[] CalcMaxDecimalPerColumn(IList<LayoutMetrics> cellsInfo, UIElementCollection cellsContent, int rows)
    {
        return new MaxMatrixWidth[cellsContent.Count];
    }

    public override object Clone()
    {
        return new EquationListLayoutManager(this);
    }

    public int GetNumberOfColumnsFitWidth(double newWidth, UIElementCollection cellsContent, ColumnAlignments alignment)
    {
        if (_cellsInfo == null)
        {
            return Columns;
        }

        var clone = (TableLayoutManager)Clone();
        if (DoubleComparer.DefaultDoubleComparer.Compare(newWidth, clone.MatrixSize.Width) > 0)
        {
            while (clone.Columns < _cellsInfo.Count)
            {
                ++clone.Columns;
                clone.Measure(cellsContent, alignment);
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
                clone.Measure(cellsContent, alignment);
            }
        }

        return clone.Columns;
    }
}
