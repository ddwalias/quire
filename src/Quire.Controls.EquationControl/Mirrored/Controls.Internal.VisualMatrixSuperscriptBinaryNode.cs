using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal partial class VisualMatrixSuperscriptBinaryNode
{
    protected override Size MeasureOverride(Size availableSize)
    {
        VisualMathNode firstMathNode = FirstMathNode;
        VisualMathNode secondMathNode = SecondMathNode;
        firstMathNode.Measure(availableSize);
        secondMathNode.Measure(availableSize);
        _operator.Measure(availableSize);
        _pushFirstOpDown = secondMathNode.EffectiveBaselineY - firstMathNode.EffectiveScriptBaselineY;
        double height;
        ref Point alignmentPoint = ref _alignmentPoint;
        Point alignmentPoint2 = firstMathNode.AlignmentPoint;
        alignmentPoint.Y = alignmentPoint2.Y + _pushFirstOpDown;
        height = firstMathNode.DesiredSize.Height + _pushFirstOpDown;
        double width = firstMathNode.DesiredSize.Width + secondMathNode.DesiredSize.Width;
        FireMeasureOverideCompleted();
        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        VisualMathNode firstMathNode = FirstMathNode;
        VisualMathNode secondMathNode = SecondMathNode;
        firstMathNode.Arrange(new Rect(new Point(0.0, _pushFirstOpDown), firstMathNode.DesiredSize));
        secondMathNode.Arrange(new Rect(new Point(firstMathNode.DesiredSize.Width, 0.0), secondMathNode.DesiredSize));
        _operator.Arrange(new Rect(new Point(firstMathNode.DesiredSize.Width - 1.0, 0.0), _operator.DesiredSize));
        return DesiredSize;
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        if (hitPoint.X > ActualWidth)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Right, Direction.Indeterminate, hitPoint);
        }
        GeneralTransform generalTransform;
        GeneralTransform generalTransform2;
        if (hitPoint.X > FirstMathNode.ActualWidth)
        {
            generalTransform = TransformToDescendant(SecondMathNode);
            return SecondMathNode.GetNodeAtPoint(generalTransform.Transform(hitPoint));
        }
        generalTransform2 = TransformToDescendant(FirstMathNode);
        return FirstMathNode.GetNodeAtPoint(generalTransform2.Transform(hitPoint));
    }

    public VisualMatrixSuperscriptBinaryNode()
    {
    }
}
