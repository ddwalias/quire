using System;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.EquationEditor.View;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualProgramNode : VisualUnaryOperatorNodeForEditor, IVisualUnaryOperatorNode, IVisualMathNode, IVisualNode
{
    private const double VERTICAL_PAD = 4.0;

    private VerticalFence _operator;

    private VerticalFence _operatorRightMinorPart;

    private bool _hasMinorFence;

    private VisualMathNode[] _nodeChildren;

    private VisualMatrixContents _operandsPanel;

    public override double EffectiveBaselineY
    {
        get
        {
            Size desiredSize = DesiredSize;
            return desiredSize.Height;
        }
    }

    public override double EffectiveScriptBaselineY => 0.0;

    public override double TextBaselineY => _nodeChildren[0].TextBaselineY + (_operator.DesiredSize.Height - _operandsPanel.DesiredSize.Height) / 2.0;

    public override IVisualMathNode this[int index]
    {
        get => _nodeChildren[index];
        set
        {
            VisualMathNode visualMathNode = (VisualMathNode)value;
            if (visualMathNode is VisualPlaceholderNode)
            {
                visualMathNode.ShowOperatorOnlyWhenActive = true;
            }
            _nodeChildren[index] = visualMathNode;
            _operandsPanel.Children.Add(visualMathNode);
        }
    }

    public override VisualMathNode Child
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IVisualMathNode Operator => OperatorElement;

    public override VisualMathNode OperatorElement => _operator;

    IVisualMathNode IVisualUnaryOperatorNode.Child
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[1] { OperatorElement };

    public VisualMathNode[] NodeChildren => _nodeChildren;

    public VisualProgramNode(bool hasMinorFence)
    {
        _hasMinorFence = hasMinorFence;
        _operator = VerticalFence.CreateDoubleVerticalLine();
        Children.Add(_operator);
        if (_hasMinorFence)
        {
            _operatorRightMinorPart = VerticalFence.CreateVerticalLine();
            _operatorRightMinorPart.Opacity = 0.3;
            Children.Add(_operatorRightMinorPart);
        }
        _operandsPanel = new VisualMatrixContents();
        _operandsPanel.Columns = 1;
        _operandsPanel.ColumnAlignment = ColumnAlignments.LeftAlign;
        Children.Add(_operandsPanel);
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        Point p = TransformToDescendant(_operandsPanel).Transform(hitPoint);
        VisualMathNode visualMathNode;
        GeneralTransform generalTransform;
        if (p.X < 0.0)
        {
            Point val = TransformToDescendant(_operator).Transform(hitPoint);
            if (val.X < 0.0)
            {
                return new VisualMathNodeHitTestResult(this, Direction.Left, Direction.Indeterminate, hitPoint);
            }
            return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Left, hitPoint);
        }
        if (p.X > _operandsPanel.ActualWidth)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Right, Direction.Indeterminate, hitPoint);
        }
        visualMathNode = _operandsPanel.CellFromPoint(p);
        generalTransform = TransformToDescendant(visualMathNode);
        return visualMathNode.GetNodeAtPoint(generalTransform.Transform(hitPoint));
    }

    public override void BuildChildren(IVisualMathNode[] children)
    {
        _nodeChildren = new VisualMathNode[children.Length];
        base.BuildChildren(children);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        _operandsPanel.Measure(availableSize);
        Size contentSize = new Size(_operandsPanel.DesiredSize.Width, _operandsPanel.DesiredSize.Height + 8.0);
        _operator.ContentSize = contentSize;
        _operator.Measure(WpfUtils.SizeInfinity);
        Size result = contentSize;
        result.Height = _operator.DesiredSize.Height;
        result.Width = _operator.DesiredSize.Width + _operandsPanel.DesiredSize.Width;
        if (_hasMinorFence)
        {
            _operatorRightMinorPart.ContentSize = contentSize;
            _operatorRightMinorPart.Measure(WpfUtils.SizeInfinity);
            result.Width += _operatorRightMinorPart.DesiredSize.Width;
        }
        FireMeasureOverideCompleted();
        return result;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _operator.Arrange(new Rect(new Point(0.0, 0.0), _operator.DesiredSize));
        _operandsPanel.Arrange(new Rect(new Point(_operator.DesiredSize.Width, (_operator.DesiredSize.Height - _operandsPanel.DesiredSize.Height) / 2.0), _operandsPanel.DesiredSize));
        if (_hasMinorFence)
        {
            _operatorRightMinorPart.Arrange(new Rect(new Point(_operator.DesiredSize.Width + _operandsPanel.DesiredSize.Width, 0.0), _operatorRightMinorPart.DesiredSize));
        }
        return DesiredSize;
    }

    public void SetVisibilityOfOperatorSymbol(Visibility visibility)
    {
        _operator.Visibility = visibility;
        if (_hasMinorFence)
        {
            _operatorRightMinorPart.Visibility = visibility;
        }
    }
}
