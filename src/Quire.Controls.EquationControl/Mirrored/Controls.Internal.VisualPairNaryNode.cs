using System;
using System.Windows;
using System.Windows.Media;
using Quire.Core;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Quire.Controls.Internal;

internal partial class VisualPairNaryNode
{
    private const double OPERATOR_PAD = 4.0;
    private const double BODY_LEFT_PAD = 8.0;

    private VerticalFence _operatorRightMinorPart;
    private Point _operatorRightMinorPartTopLeft;
    private double _verticalOffset;
    private VisualMathNode[] _nodeChildren;
    private VisualSymbolNode[] _operatorParts;
    private Point[] _operatorPartsTopLeft;
    private Point[] _operandsTopLeft;
    private string _firstOperatorPartSymbol;
    private string _middleOperatorPartsSymbol;
    private string _lastOperatorPartSymbol;
    private bool _hasLastSymbol;

    public override double EffectiveBaselineY => DesiredSize.Height;

    public override double EffectiveScriptBaselineY => 0.0;

    public override double TextBaselineY => _nodeChildren[0].TextBaselineY + _verticalOffset;

    public IVisualMathNode[] NodeChildren => _nodeChildren;

    public IVisualMathNode[] OperatorParts => _operatorParts;

    public Point[] OperatorPartstopLeft => _operatorPartsTopLeft;

    public override IVisualMathNode this[int index]
    {
        get => _nodeChildren[index];
        set
        {
            VisualMathNode visualMathNode = value as VisualMathNode;
            _nodeChildren[index] = visualMathNode;
            Children.Add(visualMathNode);
        }
    }

    public override IVisualMathNode[] NavigatableParts => _operatorParts;

    public VisualPairNaryNode(string firstOperatorPartSymbol, string middleOperatorPartsSymbol, string lastOperatorPartSymbol)
    {
        _firstOperatorPartSymbol = firstOperatorPartSymbol;
        _middleOperatorPartsSymbol = middleOperatorPartsSymbol;
        _lastOperatorPartSymbol = lastOperatorPartSymbol;
        _operatorRightMinorPart = VerticalFence.CreateVerticalLine();
        _operatorRightMinorPart.Opacity = 0.3;
        Children.Add(_operatorRightMinorPart);
    }

    public override void BuildChildren(IVisualMathNode[] children)
    {
        PrepareVisualChildren(children.Length);
        base.BuildChildren(children);
    }

    private void PrepareVisualChildren(int childCount)
    {
        _nodeChildren = new VisualMathNode[childCount];
        _operandsTopLeft = new Point[childCount];
        int num = (childCount + 1) / 2;
        _operatorParts = new VisualSymbolNode[num];
        _operatorPartsTopLeft = new Point[num];
        _hasLastSymbol = childCount % 2 == 1;
        VisualSymbolNode visualSymbolNode;
        for (int i = 0; i < num; i++)
        {
            if (i == 0)
            {
                visualSymbolNode = new VisualSymbolNode(_firstOperatorPartSymbol);
            }
            else if (i == num - 1 && _hasLastSymbol)
            {
                visualSymbolNode = new VisualSymbolNode(_lastOperatorPartSymbol);
            }
            else
            {
                visualSymbolNode = new VisualSymbolNode(_middleOperatorPartsSymbol);
            }
            VisualMathNode.SetOperatorFontProperties(visualSymbolNode.MathFormat);
            _operatorParts[i] = visualSymbolNode;
            Children.Add(visualSymbolNode);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        int num = _nodeChildren.Length;
        int operatorCount = _operatorParts.Length;
        Size contentSize;
        double leftContentWidth;
        double height;
        double rightContentWidth;
        double height2;
        Size result;
        double x;
        double childX;
        double y;
        for (int i = 0; i < num; i++)
        {
            _nodeChildren[i].Measure(WpfUtils.SizeInfinity);
        }
        for (int j = 0; j < operatorCount; j++)
        {
            _operatorParts[j].Measure(WpfUtils.SizeInfinity);
        }
        contentSize = new Size(0.0, 0.0);
        for (int k = 0; k < operatorCount; k++)
        {
            if (k == operatorCount - 1 && _hasLastSymbol)
            {
                leftContentWidth = _operatorParts[k].DesiredSize.Width;
                height = _operatorParts[k].DesiredSize.Height;
                rightContentWidth = 8.0 + _nodeChildren[k * 2].DesiredSize.Width;
                height2 = _nodeChildren[k * 2].DesiredSize.Height;
            }
            else
            {
                leftContentWidth = _operatorParts[k].DesiredSize.Width + 4.0 + _nodeChildren[k * 2].DesiredSize.Width;
                height = _nodeChildren[k * 2].DesiredSize.Height;
                rightContentWidth = 8.0 + _nodeChildren[k * 2 + 1].DesiredSize.Width;
                height2 = _nodeChildren[k * 2 + 1].DesiredSize.Height;
            }
            contentSize.Height += height + height2;
            contentSize.Width = Math.Max(contentSize.Width, Math.Max(leftContentWidth, rightContentWidth));
        }
        _operatorRightMinorPart.ContentSize = contentSize;
        _operatorRightMinorPart.Measure(WpfUtils.SizeInfinity);
        result = default;
        result.Width = contentSize.Width + _operatorRightMinorPart.DesiredSize.Width;
        result.Height = _operatorRightMinorPart.DesiredSize.Height;
        _verticalOffset = (result.Height - contentSize.Height) / 2.0;
        x = 0.0;
        childX = x + 8.0;
        y = _verticalOffset;
        for (int l = 0; l < operatorCount; l++)
        {
            if (l == operatorCount - 1 && _hasLastSymbol)
            {
                _operatorPartsTopLeft[l] = new Point(x, y);
                y += _operatorParts[l].DesiredSize.Height;
                _operandsTopLeft[l * 2] = new Point(childX, y);
                y += _nodeChildren[l * 2].DesiredSize.Height;
            }
            else
            {
                _operatorPartsTopLeft[l] = new Point(x, y + _nodeChildren[l * 2].TextBaselineY - _operatorParts[l].TextBaselineY);
                _operandsTopLeft[l * 2] = new Point(x + _operatorParts[l].DesiredSize.Width + 4.0, y);
                y += _nodeChildren[l * 2].DesiredSize.Height;
                _operandsTopLeft[l * 2 + 1] = new Point(childX, y);
                y += _nodeChildren[l * 2 + 1].DesiredSize.Height;
            }
        }
        _operatorRightMinorPartTopLeft = new Point(_operatorRightMinorPart.ContentSize.Width, 0.0);
        FireMeasureOverideCompleted();
        return result;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int num = _nodeChildren.Length;
        int operatorCount = _operatorParts.Length;
        for (int i = 0; i < operatorCount; i++)
        {
            _operatorParts[i].Arrange(new Rect(_operatorPartsTopLeft[i], _operatorParts[i].DesiredSize));
        }
        for (int j = 0; j < num; j++)
        {
            _nodeChildren[j].Arrange(new Rect(_operandsTopLeft[j], _nodeChildren[j].DesiredSize));
        }
        _operatorRightMinorPart.Arrange(new Rect(_operatorRightMinorPartTopLeft, _operatorRightMinorPart.DesiredSize));
        return DesiredSize;
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        Point val = TransformToDescendant(_operatorRightMinorPart).Transform(hitPoint);
        if (val.X >= 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Right, null, hitPoint);
        }
        Point firstOperatorPoint = TransformToDescendant(_operatorParts[0]).Transform(hitPoint);
        if (firstOperatorPoint.X <= 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Left, null, hitPoint);
        }
        int num = 0;
        int operatorCount = _operatorParts.Length;
        int childIndex;
        Point hitPoint2;
        for (int i = 0; i < operatorCount; i++)
        {
            double upperBoundary;
            if (i == 0)
            {
                upperBoundary = double.NegativeInfinity;
            }
            else if (i == operatorCount - 1 && _hasLastSymbol)
            {
                upperBoundary = _operatorPartsTopLeft[i].Y;
            }
            else
            {
                upperBoundary = _operandsTopLeft[i * 2].Y;
            }
            double lowerBoundary;
            if (i == operatorCount - 1)
            {
                lowerBoundary = double.PositiveInfinity;
            }
            else if (i == operatorCount - 2 && _hasLastSymbol)
            {
                lowerBoundary = _operatorPartsTopLeft[i + 1].Y;
            }
            else
            {
                lowerBoundary = _operandsTopLeft[(i + 1) * 2].Y;
            }
            if (hitPoint.Y >= upperBoundary && hitPoint.Y <= lowerBoundary)
            {
                num = i;
                break;
            }
        }
        childIndex = num * 2 + 1;
        if (num == operatorCount - 1 && _hasLastSymbol)
        {
            childIndex = num * 2;
        }
        hitPoint2 = TransformToDescendant(_nodeChildren[childIndex]).Transform(hitPoint);
        if (hitPoint2.Y >= 0.0)
        {
            return _nodeChildren[childIndex].GetNodeAtPoint(hitPoint2);
        }
        if (num != operatorCount - 1 || !_hasLastSymbol)
        {
            int firstChildIndex = num * 2;
            Point hitPoint3 = TransformToDescendant(_nodeChildren[firstChildIndex]).Transform(hitPoint);
            if (hitPoint3.X >= 0.0)
            {
                return _nodeChildren[firstChildIndex].GetNodeAtPoint(hitPoint3);
            }
        }
        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, num, hitPoint);
    }

    public void SetVisibilityOfOperatorSymbol(Visibility visibility)
    {
        int num = _operatorParts.Length;
        for (int i = 0; i < num; i++)
        {
            _operatorParts[i].Visibility = visibility;
        }
        _operatorRightMinorPart.Visibility = visibility;
    }
}
