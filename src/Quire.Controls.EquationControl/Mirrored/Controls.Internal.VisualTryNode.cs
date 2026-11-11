#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.View;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualTryNode : VisualMathNode
{
    private const double BODY_LEFT_PAD = 8.0;
    private VisualSymbolNode _tryOperator;
    private VisualSymbolNode _onErrorOperator;
    private VerticalFence _operatorRightMinorPart;
    private double _verticalOffset;
    private readonly VisualMathNode[] _nodeChildren;
    private Point _tryOperatorTopLeft;
    private Point _onErrorOperatorTopLeft;
    private Point _firstChildTopLeft;
    private Point _secondChildTopLeft;
    private Point _operatorRightMinorPartTopLeft;

    public VisualMathNode[] NodeChildren => _nodeChildren;

    public VisualSymbolNode TryOperator => _tryOperator;

    public VisualSymbolNode OnErrorOperator => _onErrorOperator;

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[2] { TryOperator, OnErrorOperator };

    public override double EffectiveBaselineY => DesiredSize.Height;

    public override double EffectiveScriptBaselineY => 0.0;

    public override double TextBaselineY => TryOperator.TextBaselineY + _verticalOffset;

    public override IVisualMathNode this[int index]
    {
        get => _nodeChildren[index];
        set
        {
            if (index > 1)
            {
                throw new InvalidOperationException("Try statement has only 2 children.");
            }
            VisualMathNode visualMathNode = value as VisualMathNode;
            _nodeChildren[index] = visualMathNode;
            Children.Add(visualMathNode);
        }
    }

    public Point TryOperatorTopLeft => _tryOperatorTopLeft;

    public Point OnErrorOperatorTopLeft => _onErrorOperatorTopLeft;

    public VisualTryNode()
    {
        _nodeChildren = new VisualMathNode[2];
        _tryOperator = new VisualSymbolNode("try", 0);
        VisualMathNode.SetOperatorFontProperties(_tryOperator.MathFormat);
        Children.Add(_tryOperator);
        _onErrorOperator = new VisualSymbolNode("on error", 0);
        VisualMathNode.SetOperatorFontProperties(_onErrorOperator.MathFormat);
        Children.Add(_onErrorOperator);
        _operatorRightMinorPart = VerticalFence.CreateVerticalLine();
        _operatorRightMinorPart.Opacity = 0.3;
        Children.Add(_operatorRightMinorPart);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        _nodeChildren[0].Measure(WpfUtils.SizeInfinity);
        _nodeChildren[1].Measure(WpfUtils.SizeInfinity);
        _tryOperator.Measure(WpfUtils.SizeInfinity);
        _onErrorOperator.Measure(WpfUtils.SizeInfinity);

        Size contentSize = default;
        contentSize.Width = Math.Max(
            Math.Max(_tryOperator.DesiredSize.Width, _onErrorOperator.DesiredSize.Width),
            8.0 + Math.Max(_nodeChildren[0].DesiredSize.Width, _nodeChildren[1].DesiredSize.Width)
        );
        contentSize.Height = _tryOperator.DesiredSize.Height + _nodeChildren[0].DesiredSize.Height + _onErrorOperator.DesiredSize.Height + _nodeChildren[1].DesiredSize.Height;

        _operatorRightMinorPart.ContentSize = contentSize;
        _operatorRightMinorPart.Measure(WpfUtils.SizeInfinity);

        Size result = default;
        result.Width = contentSize.Width + _operatorRightMinorPart.DesiredSize.Width;
        result.Height = _operatorRightMinorPart.DesiredSize.Height;
        _verticalOffset = (result.Height - contentSize.Height) / 2.0;

        double num = 0.0;
        double num2 = num + 8.0;
        double num3 = TextBaselineY - _tryOperator.TextBaselineY;
        _tryOperatorTopLeft = new Point(num, num3);
        num3 += _tryOperator.DesiredSize.Height;
        _firstChildTopLeft = new Point(num2, num3);
        num3 += _nodeChildren[0].DesiredSize.Height;
        _onErrorOperatorTopLeft = new Point(num, num3);
        num3 += _onErrorOperator.DesiredSize.Height;
        _secondChildTopLeft = new Point(num2, num3);
        double width = _operatorRightMinorPart.ContentSize.Width;
        num3 = 0.0;
        _operatorRightMinorPartTopLeft = new Point(width, num3);
        FireMeasureOverideCompleted();
        return result;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _tryOperator.Arrange(new Rect(_tryOperatorTopLeft, _tryOperator.DesiredSize));
        _nodeChildren[0].Arrange(new Rect(_firstChildTopLeft, _nodeChildren[0].DesiredSize));
        _onErrorOperator.Arrange(new Rect(_onErrorOperatorTopLeft, _onErrorOperator.DesiredSize));
        _nodeChildren[1].Arrange(new Rect(_secondChildTopLeft, _nodeChildren[1].DesiredSize));
        _operatorRightMinorPart.Arrange(new Rect(_operatorRightMinorPartTopLeft, _operatorRightMinorPart.DesiredSize));
        return DesiredSize;
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        Point val = TransformToDescendant(_operatorRightMinorPart).Transform(hitPoint);
        if (val.X >= 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Right, Direction.Indeterminate, hitPoint);
        }
        Point val2 = TransformToDescendant(_tryOperator).Transform(hitPoint);
        if (val2.X <= 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Left, Direction.Indeterminate, hitPoint);
        }
        Point hitPoint2 = TransformToDescendant(_nodeChildren[1]).Transform(hitPoint);
        if (hitPoint2.Y >= 0.0)
        {
            return _nodeChildren[1].GetNodeAtPoint(hitPoint2);
        }
        Point val3 = TransformToDescendant(_onErrorOperator).Transform(hitPoint);
        if (val3.Y >= 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Right, hitPoint);
        }
        Point hitPoint3 = TransformToDescendant(_nodeChildren[0]).Transform(hitPoint);
        if (hitPoint3.Y >= 0.0)
        {
            return _nodeChildren[0].GetNodeAtPoint(hitPoint3);
        }
        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Left, hitPoint);
    }

    public void SetVisibilityOfOperatorSymbol(Visibility visibility)
    {
        _tryOperator.Visibility = visibility;
        _onErrorOperator.Visibility = visibility;
        _operatorRightMinorPart.Visibility = visibility;
    }
}
