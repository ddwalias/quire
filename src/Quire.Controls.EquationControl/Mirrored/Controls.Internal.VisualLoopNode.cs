using System;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.View;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualLoopNode
{
    private const double OPERATOR_PAD = 4d;
    private const double BODY_LEFT_PAD = 8d;

    private VisualSymbolNode _operator;
    private VerticalFence _operatorRightMinorPart;
    private double _verticalOffset;

    public VisualLoopNode(string loopSymbol)
    {
        _operator = new VisualSymbolNode(loopSymbol, 0);
        SetOperatorFontProperties(_operator.MathFormat);
        Children.Add(_operator);
        _operatorRightMinorPart = VerticalFence.CreateVerticalLine();
        _operatorRightMinorPart.Opacity = 0.3;
        Children.Add(_operatorRightMinorPart);
    }

    public override double EffectiveBaselineY => DesiredSize.Height;

    public override double EffectiveScriptBaselineY => 0.0;

    public override double TextBaselineY => FirstMathNode.TextBaselineY + _verticalOffset;

    public override IVisualMathNode Operator => _operator;

    public override VisualMathNode OperatorElement => _operator;

    protected override Size MeasureOverride(Size availableSize)
    {
        FirstMathNode.Measure(WpfUtils.SizeInfinity);
        SecondMathNode.Measure(WpfUtils.SizeInfinity);
        OperatorElement.Measure(WpfUtils.SizeInfinity);
        Size size = default;
        size.Width = Math.Max(OperatorElement.DesiredSize.Width + OPERATOR_PAD + FirstMathNode.DesiredSize.Width, BODY_LEFT_PAD + SecondMathNode.DesiredSize.Width);
        size.Height = FirstMathNode.DesiredSize.Height + SecondMathNode.DesiredSize.Height;
        _operatorRightMinorPart.ContentSize = size;
        _operatorRightMinorPart.Measure(WpfUtils.SizeInfinity);
        Size result = default;
        result.Width = size.Width + _operatorRightMinorPart.DesiredSize.Width;
        result.Height = _operatorRightMinorPart.DesiredSize.Height;
        _verticalOffset = (result.Height - size.Height) / 2.0;
        FireMeasureOverideCompleted();
        return result;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        OperatorElement.Arrange(new Rect(new Point(0.0, TextBaselineY - OperatorElement.TextBaselineY), OperatorElement.DesiredSize));
        FirstMathNode.Arrange(new Rect(new Point(OperatorElement.DesiredSize.Width + OPERATOR_PAD, _verticalOffset), FirstMathNode.DesiredSize));
        SecondMathNode.Arrange(new Rect(new Point(BODY_LEFT_PAD, _verticalOffset + FirstMathNode.DesiredSize.Height), SecondMathNode.DesiredSize));
        _operatorRightMinorPart.Arrange(new Rect(new Point(_operatorRightMinorPart.ContentSize.Width, 0.0), _operatorRightMinorPart.DesiredSize));
        return DesiredSize;
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        Point point = TransformToDescendant(_operatorRightMinorPart).Transform(hitPoint);
        if (point.X >= 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Right, Direction.Indeterminate, hitPoint);
        }

        Point point2 = TransformToDescendant(OperatorElement).Transform(hitPoint);
        if (point2.X <= 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Left, Direction.Indeterminate, hitPoint);
        }

        Point point3 = TransformToDescendant(SecondMathNode).Transform(hitPoint);
        if (point3.Y >= 0.0)
        {
            return SecondMathNode.GetNodeAtPoint(point3);
        }

        Point point4 = TransformToDescendant(FirstMathNode).Transform(hitPoint);
        if (point4.X + 2.0 >= 0.0)
        {
            return FirstMathNode.GetNodeAtPoint(point4);
        }

        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Indeterminate, hitPoint);
    }

    public void SetVisibilityOfOperatorSymbol(Visibility visibility)
    {
        _operator.Visibility = visibility;
        _operatorRightMinorPart.Visibility = visibility;
    }
}
