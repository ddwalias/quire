using System;
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal partial class VisualExponentNode
{
    private static DependencyProperty VerticalAdjustProperty = DependencyProperty.Register(
        "VerticalAdjust",
        typeof(double),
        typeof(VisualExponentNode),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

    private const double PAD = 1.0;

    public VisualExponentNode()
    {
    }

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
        double width = firstMathNode.DesiredSize.Width + Math.Max(secondMathNode.DesiredSize.Width + 1.0, _operator.DesiredSize.Width - 1.0);
        FireMeasureOverideCompleted();
        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        VisualMathNode firstMathNode = FirstMathNode;
        VisualMathNode secondMathNode = SecondMathNode;
        double num = (double)GetValue(VerticalAdjustProperty);
        firstMathNode.Arrange(new Rect(new Point(0.0, _pushFirstOpDown), firstMathNode.DesiredSize));
        secondMathNode.Arrange(new Rect(new Point(firstMathNode.DesiredSize.Width + 1.0, num), secondMathNode.DesiredSize));
        _operator.Arrange(new Rect(new Point(firstMathNode.DesiredSize.Width - 1.0, secondMathNode.EffectiveBaselineY - _operator.GlyphGap + 1.0 + num), _operator.DesiredSize));
        return DesiredSize;
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        VisualSymbolNode visualSymbolNode = OperatorElement as VisualSymbolNode;
        Rect rect;
        if (visualSymbolNode != null)
        {
            rect = visualSymbolNode.SymbolBounds;
        }
        else
        {
            rect = new Rect(new Size(OperatorElement.Width, OperatorElement.Height));
        }
        Point point = TransformToDescendant(OperatorElement).Transform(hitPoint);
        if (rect.Contains(point))
        {
            return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Indeterminate, hitPoint);
        }
        if (hitPoint.X > ActualWidth && point.Y >= rect.Top)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Right, Direction.Indeterminate, hitPoint);
        }
        GeneralTransform generalTransform;
        GeneralTransform generalTransform2;
        if (point.X < rect.Left)
        {
            generalTransform = TransformToDescendant(FirstMathNode);
            return FirstMathNode.GetNodeAtPoint(generalTransform.Transform(hitPoint));
        }
        if (point.Y < rect.Height || point.X > rect.Right)
        {
            generalTransform2 = TransformToDescendant(SecondMathNode);
            return SecondMathNode.GetNodeAtPoint(generalTransform2.Transform(hitPoint));
        }
        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Indeterminate, hitPoint);
    }
}
