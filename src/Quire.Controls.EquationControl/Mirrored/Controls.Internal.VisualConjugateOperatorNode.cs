#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualConjugateOperatorNode : VisualUnaryOperatorNodeForEditor, IVisualUnaryOperatorNode, IVisualMathNode, IVisualNode
{
    private static readonly MathNodeDescriptor _conjugateDescriptor = DefaultDescriptorTable.Default["CONJUGATE"];
    private readonly VisualMathNode[] _nodeChildren;
    private readonly Fence _operatorSymbol;
    private ConjugateLayoutManager _layout;

    public override IVisualMathNode this[int index]
    {
        get => _nodeChildren[index];
        set
        {
            if (index > 1)
            {
                throw new InvalidOperationException("Index cannot be greater than 0 when adding child nodes to an Conjugate.");
            }
            VisualMathNode visualMathNode = value as VisualMathNode;
            _nodeChildren[index] = visualMathNode;
            Children.Add(visualMathNode);
        }
    }

    public VisualMathNode[] NodeChildren => _nodeChildren;

    public Fence OperatorSymbol => _operatorSymbol;

    public Point ConjugateSymbolTopLeft => _layout.OperatorSymbolRect.TopLeft;

    public override double TextBaselineY
    {
        get
        {
            if (_layout == null)
            {
                return 0.0;
            }
            return _layout.TextBaselineOffset;
        }
    }

    public override double EffectiveBaselineY => _nodeChildren[0].EffectiveBaselineY + _nodeChildren[0].EffectiveScriptBaselineY;

    public override double EffectiveScriptBaselineY => _nodeChildren[0].EffectiveScriptBaselineY;

    public override VisualMathNode Child
    {
        get => _nodeChildren[0];
        set => this[0] = value;
    }

    public override VisualMathNode OperatorElement => OperatorSymbol;

    public IVisualMathNode Operator => OperatorSymbol;

    IVisualMathNode IVisualUnaryOperatorNode.Child
    {
        get => Child;
        set => Child = (VisualMathNode)value;
    }

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[1] { OperatorElement };

    public VisualConjugateOperatorNode()
    {
        _nodeChildren = new VisualMathNode[_conjugateDescriptor.ChildCount];
        _operatorSymbol = new Overline();
        Children.Add(_operatorSymbol);
    }

    public void SetVisibilityOfOperatorSymbol(Visibility visibility)
    {
        OperatorSymbol.Visibility = visibility;
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        _layout.FindClosestChild(hitPoint, out var childType, out var childIndex, out var childSideHit);
        if (childType == LayoutChildType.Child)
        {
            VisualMathNode visualMathNode = _nodeChildren[childIndex];
            GeneralTransform generalTransform = TransformToDescendant(visualMathNode);
            return visualMathNode.GetNodeAtPoint(generalTransform.Transform(hitPoint));
        }
        if (childIndex == 0 && childSideHit == Direction.Left)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Left, Direction.Indeterminate, hitPoint);
        }
        if (childIndex == 0 && childSideHit == Direction.Right)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Right, Direction.Indeterminate, hitPoint);
        }
        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Indeterminate, hitPoint);
    }

    protected override Size MeasureStaticLayout(Size availableSize)
    {
        double width;
        LayoutItem[] childItems;
        LayoutItem[] operatorPartItems;

        VisualMathNode[] nodeChildren = _nodeChildren;
        for (int i = 0; i < nodeChildren.Length; i++)
        {
            nodeChildren[i].Measure(availableSize);
        }
        if (_nodeChildren[0].DesiredSize.Width < OperatorSymbol.Last.Width)
        {
            width = OperatorSymbol.Last.Width;
        }
        else
        {
            width = _nodeChildren[0].DesiredSize.Width;
        }
        OperatorSymbol.ContentSize = new Size(width, _nodeChildren[0].DesiredSize.Height);
        OperatorSymbol.Measure(OperatorSymbol.ContentSize);
        childItems = new LayoutItem[1]
        {
            new LayoutItem(_nodeChildren[0].DesiredSize, _nodeChildren[0].TextBaselineY)
        };
        operatorPartItems = new LayoutItem[1]
        {
            new LayoutItem(_operatorSymbol.DesiredSize, _operatorSymbol.TextBaselineY)
        };
        _layout = new ConjugateLayoutManager(childItems, operatorPartItems);
        _layout.Arrange();
        return _layout.DesiredSize;
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        _nodeChildren[0].ArrangeMathNode(_layout.ConjugatedExprRect);
        OperatorSymbol.ArrangeMathNode(_layout.OperatorSymbolRect);
        return DesiredSize;
    }
}
