using System;
using System.Windows;
using System.Windows.Media;
using Quire.Core;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Quire.Controls.Internal;

internal partial class VisualForLoopNode
{
    private const double OPERATOR_PAD = 4.0;
    private const double BODY_LEFT_PAD = 8.0;

    private VisualSymbolNode _forOperator;
    private VisualSymbolNode _inOperator;
    private VerticalFence _operatorRightMinorPart;
    private double _verticalOffset;
    private readonly VisualMathNode[] _nodeChildren;
    private Point _forOperatorTopLeft;
    private Point _inOperatorTopLeft;
    private Point _firstChildTopLeft;
    private Point _secondChildTopLeft;
    private Point _thirdChildTopLeft;
    private Point _operatorRightMinorPartTopLeft;

    public VisualMathNode[] NodeChildren => _nodeChildren;

    public VisualSymbolNode ForOperator => _forOperator;

    public VisualSymbolNode InOperator => _inOperator;

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[2] { ForOperator, InOperator };

    public override double EffectiveBaselineY => DesiredSize.Height;

    public override double EffectiveScriptBaselineY => 0.0;

    public override double TextBaselineY => _nodeChildren[1].TextBaselineY + _verticalOffset;

    public override IVisualMathNode this[int index]
    {
        get => _nodeChildren[index];
        set
        {
            if (index > 2)
            {
                throw new InvalidOperationException("For loop has only 3 children.");
            }
            VisualMathNode visualMathNode = value as VisualMathNode;
            _nodeChildren[index] = visualMathNode;
            Children.Add(visualMathNode);
        }
    }

    public Point ForOperatorTopLeft => _forOperatorTopLeft;

    public Point InOperatorTopLeft => _inOperatorTopLeft;

    public VisualForLoopNode()
    {
        _nodeChildren = new VisualMathNode[3];
        _forOperator = new VisualSymbolNode("for", 0);
        VisualMathNode.SetOperatorFontProperties(_forOperator.MathFormat);
        Children.Add(_forOperator);
        _inOperator = new VisualSymbolNode("∊", 0);
        VisualMathNode.SetOperatorFontProperties(_inOperator.MathFormat);
        Children.Add(_inOperator);
        _operatorRightMinorPart = VerticalFence.CreateVerticalLine();
        _operatorRightMinorPart.Opacity = 0.3;
        Children.Add(_operatorRightMinorPart);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        _nodeChildren[0].Measure(WpfUtils.SizeInfinity);
        _nodeChildren[1].Measure(WpfUtils.SizeInfinity);
        _nodeChildren[2].Measure(WpfUtils.SizeInfinity);
        _forOperator.Measure(WpfUtils.SizeInfinity);
        _inOperator.Measure(WpfUtils.SizeInfinity);
        Size contentSize = default;
        contentSize.Width = Math.Max(_forOperator.DesiredSize.Width + 4.0 + _nodeChildren[0].DesiredSize.Width + 4.0 + _inOperator.DesiredSize.Width + 4.0 + _nodeChildren[1].DesiredSize.Width, 8.0 + _nodeChildren[2].DesiredSize.Width);
        contentSize.Height = _nodeChildren[1].DesiredSize.Height + _nodeChildren[2].DesiredSize.Height;
        _operatorRightMinorPart.ContentSize = contentSize;
        _operatorRightMinorPart.Measure(WpfUtils.SizeInfinity);
        Size result = default;
        result.Width = contentSize.Width + _operatorRightMinorPart.DesiredSize.Width;
        result.Height = _operatorRightMinorPart.DesiredSize.Height;
        _verticalOffset = (result.Height - contentSize.Height) / 2.0;
        double num4 = 0.0;
        double num5 = TextBaselineY - _forOperator.TextBaselineY;
        _forOperatorTopLeft = new Point(num4, num5);
        num4 += 4.0 + _forOperator.DesiredSize.Width;
        num5 = TextBaselineY - _nodeChildren[0].TextBaselineY;
        _firstChildTopLeft = new Point(num4, num5);
        num4 += 4.0 + _nodeChildren[0].DesiredSize.Width;
        num5 = TextBaselineY - _inOperator.TextBaselineY;
        _inOperatorTopLeft = new Point(num4, num5);
        num4 += 4.0 + _inOperator.DesiredSize.Width;
        num5 = _verticalOffset;
        _secondChildTopLeft = new Point(num4, num5);
        num4 = 8.0;
        num5 += _nodeChildren[1].DesiredSize.Height;
        _thirdChildTopLeft = new Point(num4, num5);
        num4 = _operatorRightMinorPart.ContentSize.Width;
        num5 = 0.0;
        _operatorRightMinorPartTopLeft = new Point(num4, num5);
        FireMeasureOverideCompleted();
        return result;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _forOperator.Arrange(new Rect(_forOperatorTopLeft, _forOperator.DesiredSize));
        _nodeChildren[0].Arrange(new Rect(_firstChildTopLeft, _nodeChildren[0].DesiredSize));
        _inOperator.Arrange(new Rect(_inOperatorTopLeft, _inOperator.DesiredSize));
        _nodeChildren[1].Arrange(new Rect(_secondChildTopLeft, _nodeChildren[1].DesiredSize));
        _nodeChildren[2].Arrange(new Rect(_thirdChildTopLeft, _nodeChildren[2].DesiredSize));
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
        Point val2 = TransformToDescendant(_forOperator).Transform(hitPoint);
        if (val2.X <= 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Left, Direction.Indeterminate, hitPoint);
        }
        Point hitPoint2 = TransformToDescendant(_nodeChildren[2]).Transform(hitPoint);
        if (hitPoint2.Y >= 0.0)
        {
            return _nodeChildren[2].GetNodeAtPoint(hitPoint2);
        }
        Point hitPoint3 = TransformToDescendant(_nodeChildren[1]).Transform(hitPoint);
        if (hitPoint3.X >= 0.0)
        {
            return _nodeChildren[1].GetNodeAtPoint(hitPoint3);
        }
        Point val3 = TransformToDescendant(_inOperator).Transform(hitPoint);
        if (val3.X >= 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Right, hitPoint);
        }
        Point hitPoint4 = TransformToDescendant(_nodeChildren[0]).Transform(hitPoint);
        if (hitPoint4.X >= 0.0)
        {
            return _nodeChildren[0].GetNodeAtPoint(hitPoint4);
        }
        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Left, hitPoint);
    }

    public void SetVisibilityOfOperatorSymbol(Visibility visibility)
    {
        _forOperator.Visibility = visibility;
        _inOperator.Visibility = visibility;
        _operatorRightMinorPart.Visibility = visibility;
    }
}
