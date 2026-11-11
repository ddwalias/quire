using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Internal;

internal partial class VisualMatrixContents
{
    public static double GeneralRowSpacing = 1.0;

    public VisualMatrixContents()
        : base(new MatrixLayoutManager())
    {
        Columns = 10;
        ColumnSpacing = 8.0;
        RowSpacing = GeneralRowSpacing;
        Padding = new Thickness(2.0, 0.0, 2.0, 0.0);
        ColumnAlignment = ColumnAlignments.CenterAlign;
    }

    internal VisualMathNode CellFromPoint(Point p)
    {
        int index = _layoutManager.CellIndexFromPoint(p);
        return (VisualMathNode)Children[index];
    }
}
