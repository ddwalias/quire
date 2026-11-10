using System.Windows;

namespace Quire.Controls.Internal;

internal partial class VisualMatrixNodeEditor
{
    public VisualMatrixNodeEditor(VisualOutfixNode editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        VisualMatrixNode matrix = AdornedElement as VisualMatrixNode;
        if (matrix != null)
        {
            double width = matrix.RowHeading.Width;
            _leftFence.Arrange(new Rect(new Point(width, _firstFenceOffset.Y), _leftFence.DesiredSize));
            width += DesiredSize.Width;
            width -= _rightFence.DesiredSize.Width;
            _rightFence.Arrange(new Rect(new Point(width, _firstFenceOffset.Y), _rightFence.DesiredSize));
        }

        return DesiredSize;
    }
}
