using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.View;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Quire.Controls.Internal;

internal partial class VisualRangedPrefixNode : VisualUnaryOperatorNodeForEditor, IVisualUnaryOperatorNode, IVisualMathNode, IVisualNode
{
    private readonly MathNodeDescriptor _operatorDescriptor = DefaultDescriptorTable.Default["SUMMATION"];

    private readonly VisualMathNode[] _nodeChildren;

    private readonly VisualSymbolNode _operatorSymbol;

    private RangedPrefixLayoutManager _layout;

    public override IVisualMathNode this[int index]
    {
        get => _nodeChildren[index];
        set
        {
            if (index > 2)
            {
                throw new InvalidOperationException(string.Format("Index cannot be greater than {0} when adding childnodes to a ranged prefix operator.", _operatorDescriptor.ChildCount));
            }
            VisualMathNode visualMathNode = value as VisualMathNode;
            _nodeChildren[index] = visualMathNode;
            int navigablePartCount;
            if (visualMathNode != null && (index == 0 || index == 1))
            {
                double scaledFontSize = FontManager.GetScaledFontSize(MathFormat, FontConstants.ScriptFontSizeRatio);
                DebugTools.DevAssert(visualMathNode.MathFormat.FontSize == scaledFontSize, null);
            }
            navigablePartCount = _operatorDescriptor.NavigablePartCount;
            if (Children.Count > index + navigablePartCount)
            {
                Children[index + navigablePartCount] = visualMathNode;
            }
            else
            {
                Children.Add(visualMathNode);
            }
            if (index == 0 || index == 1)
            {
                UpdateRangeBoundsInactiveVisibility();
            }
        }
    }

    public VisualMathNode[] NodeChildren => _nodeChildren;

    public VisualSymbolNode OperatorSymbol => _operatorSymbol;

    public Point OperatorSymbolTopLeft
    {
        get
        {
            Rect operatorSymbolRect = _layout.OperatorSymbolRect;
            return operatorSymbolRect.TopLeft;
        }
    }

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

    public override double EffectiveBaselineY
    {
        get
        {
            if (_layout == null)
            {
                return 0.0;
            }
            Rect lowerRangeBoundRect = _layout.LowerRangeBoundRect;
            return lowerRangeBoundRect.Top + _nodeChildren[0].EffectiveBaselineY;
        }
    }

    public override double EffectiveScriptBaselineY
    {
        get
        {
            if (_layout == null)
            {
                return 0.0;
            }
            Rect principalOperandRect = _layout.PrincipalOperandRect;
            return principalOperandRect.Top + _nodeChildren[2].EffectiveScriptBaselineY;
        }
    }

    public override VisualMathNode Child
    {
        get => _nodeChildren[2];
        set => this[2] = value;
    }

    public override VisualMathNode OperatorElement => OperatorSymbol;

    public IVisualMathNode Operator => OperatorSymbol;

    IVisualMathNode IVisualUnaryOperatorNode.Child
    {
        get => Child;
        set => Child = (VisualMathNode)value;
    }

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[1] { OperatorElement };

    event RoutedEventHandler IVisualNode.Loaded
    {
        add => Loaded += value;
        remove => Loaded -= value;
    }

    bool IVisualNode.IsLoaded => IsLoaded;

    public VisualRangedPrefixNode(MathNodeDescriptor operatorDescriptor)
    {
        if (operatorDescriptor.ChildCount != 3 || operatorDescriptor.ContainerSlots.Length != 2 || operatorDescriptor.IsContainerSlot(2))
        {
            throw new ArgumentException("logic error: operator does not meet requirements to be a ranged prefix operator", "operatorDescriptor");
        }
        _operatorDescriptor = operatorDescriptor;
        _nodeChildren = new VisualMathNode[_operatorDescriptor.ChildCount];
        _operatorSymbol = new VisualSymbolNode(_operatorDescriptor.OperatorSymbol.Value);
        Children.Add(_operatorSymbol);
    }

    public override void ModifyChildMathFormat(int childIndex, MathFormatValues childMathFormat)
    {
        if (childIndex == 0 || childIndex == 1)
        {
            childMathFormat.FontSize = FontManager.GetScaledFontSize(MathFormat, FontConstants.ScriptFontSizeRatio);
        }
    }

    private void UpdateRangeBoundsInactiveVisibility()
    {
        VisualPlaceholderNode visualPlaceholderNode = this[1] as VisualPlaceholderNode;
        VisualPlaceholderNode visualPlaceholderNode2 = this[0] as VisualPlaceholderNode;
        if (visualPlaceholderNode != null)
        {
            visualPlaceholderNode.ShowOperatorOnlyWhenActive = true;
            if (visualPlaceholderNode2 != null)
            {
                visualPlaceholderNode2.ShowOperatorOnlyWhenActive = true;
            }
        }
        else if (visualPlaceholderNode2 != null)
        {
            visualPlaceholderNode2.ShowOperatorOnlyWhenActive = false;
        }
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
        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, (childIndex == 0) ? Direction.Left : Direction.Right, hitPoint);
    }

    protected override Size MeasureStaticLayout(Size availableSize)
    {
        LayoutItem[] childItems;
        LayoutItem[] operatorPartItems;
        VisualMathNode[] nodeChildren = _nodeChildren;
        for (int i = 0; i < nodeChildren.Length; i++)
        {
            nodeChildren[i].Measure(availableSize);
        }
        OperatorSymbol.MeasureMathNode(availableSize);
        childItems = new LayoutItem[3]
        {
            new LayoutItem(_nodeChildren[0].DesiredSize),
            new LayoutItem(_nodeChildren[1].DesiredSize),
            new LayoutItem(_nodeChildren[2].DesiredSize, _nodeChildren[2].TextBaselineY)
        };
        operatorPartItems = new LayoutItem[1]
        {
            new LayoutItem(_operatorSymbol.DesiredSize, _operatorSymbol.TextBaselineY)
        };
        _layout = new RangedPrefixLayoutManager(childItems, operatorPartItems);
        _layout.Arrange();
        return _layout.DesiredSize;
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        _nodeChildren[1].ArrangeMathNode(_layout.UpperRangeBoundRect);
        _nodeChildren[0].ArrangeMathNode(_layout.LowerRangeBoundRect);
        _nodeChildren[2].ArrangeMathNode(_layout.PrincipalOperandRect);
        OperatorSymbol.ArrangeMathNode(_layout.OperatorSymbolRect);
        return DesiredSize;
    }
}
