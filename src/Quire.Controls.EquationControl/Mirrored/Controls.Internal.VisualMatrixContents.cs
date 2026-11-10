using System.Windows;

namespace Quire.Controls.Internal;

internal partial class VisualMatrixContents
{
    internal VisualMathNode CellFromPoint(Point p)
    {
        int index = _layoutManager.CellIndexFromPoint(p);
        return (VisualMathNode)Children[index];
    }
}
