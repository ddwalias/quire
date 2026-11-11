#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualIntegralOperatorNode : VisualMathNode
{
    private static readonly MathNodeDescriptor _integralDescriptor = DefaultDescriptorTable.Default["INTEGRAL"];
    private readonly VisualMathNode[] _nodeChildren = new VisualMathNode[_integralDescriptor.ChildCount];
    private readonly Fence _integralSymbol;
    private readonly VisualSymbolNode _variableOfIntegrationPrefix;
    private double AdjacentUpperLimitHeight { get; set; }
    private double AdjacentLowerLimitHeight { get; set; }
    private IntegralLayoutManager _layout;

    public override IVisualMathNode this[int index]
    {
        get => _nodeChildren[index];
        set
        {
            if (index > 3)
            {
                throw new InvalidOperationException(string.Format("Index cannot be greater than {0} when adding childnodes to an Integral.", 3));
            }
            VisualMathNode visualMathNode = value as VisualMathNode;
            _nodeChildren[index] = visualMathNode;
            if (visualMathNode != null && (index == 0 || index == 1))
            {
                double scaledFontSize = VisualMathNode.FontManager.GetScaledFontSize(MathFormat, FontConstants.ScriptFontSizeRatio);
                DebugTools.DevAssert(visualMathNode.MathFormat.FontSize == scaledFontSize, null);
            }
            int navigablePartCount = _integralDescriptor.NavigablePartCount;
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
                UpdateIntegralBoundsInactiveVisibility();
            }
        }
    }

    public VisualMathNode[] NodeChildren => _nodeChildren;

    public Fence IntegralSymbol => _integralSymbol;

    public VisualSymbolNode VariableOfIntegrationPrefix => _variableOfIntegrationPrefix;

    public Point IntegralSymbolTopLeft => _layout.IntegralSymbolRect.TopLeft;

    public Point VariablePrefixTopLeft => _layout.VariablePrefixRect.TopLeft;

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[2] { IntegralSymbol, VariableOfIntegrationPrefix };

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
            return _layout.LowerLimitRect.Top + _nodeChildren[0].EffectiveBaselineY;
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
            return _layout.IntegrandRect.Top + _nodeChildren[2].EffectiveScriptBaselineY;
        }
    }

    public VisualIntegralOperatorNode()
    {
        _integralSymbol = VerticalFence.CreateIntegralSymbol();
        _variableOfIntegrationPrefix = new VisualSymbolNode('d');
        Children.Add(_integralSymbol);
        Children.Add(_variableOfIntegrationPrefix);
    }

    public override void ModifyChildMathFormat(int childIndex, MathFormatValues childMathFormat)
    {
        if (childIndex == 0 || childIndex == 1)
        {
            childMathFormat.FontSize = VisualMathNode.FontManager.GetScaledFontSize(MathFormat, FontConstants.ScriptFontSizeRatio);
        }
    }

    private void UpdateIntegralBoundsInactiveVisibility()
    {
        VisualMathNode visualMathNode = this[0] as VisualPlaceholderNode;
        VisualMathNode visualMathNode2 = this[1] as VisualPlaceholderNode;
        if (visualMathNode != null && visualMathNode2 != null)
        {
            visualMathNode.ShowOperatorOnlyWhenActive = true;
            visualMathNode2.ShowOperatorOnlyWhenActive = true;
        }
    }

    public void SetVisibilityOfOperatorSymbol(Visibility visibility)
    {
        IntegralSymbol.Visibility = visibility;
        VariableOfIntegrationPrefix.Visibility = visibility;
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
        foreach (VisualMathNode node in _nodeChildren)
        {
            node.Measure(availableSize);
        }
        VariableOfIntegrationPrefix.Measure(availableSize);
        VisualMathNode visualMathNode = _nodeChildren[2];
        Size desiredSize = visualMathNode.DesiredSize;
        bool flag = false;
        VisualIntegralOperatorNode visualIntegralOperatorNode = visualMathNode as VisualIntegralOperatorNode;
        if (visualIntegralOperatorNode != null)
        {
            flag = true;
        }
        if (flag)
        {
            desiredSize.Height = visualIntegralOperatorNode.IntegralSymbol.ContentSize.Height;
        }
        else
        {
            desiredSize.Height += VariableOfIntegrationPrefix.DesiredSize.Height / 2.0;
        }
        IntegralSymbol.ContentSize = desiredSize;
        IntegralSymbol.Measure(availableSize);
        LayoutItem[] childItems = new LayoutItem[4]
        {
            new LayoutItem(_nodeChildren[0].DesiredSize),
            new LayoutItem(_nodeChildren[1].DesiredSize),
            new LayoutItem(_nodeChildren[2].DesiredSize, _nodeChildren[2].TextBaselineY),
            new LayoutItem(_nodeChildren[3].DesiredSize, _nodeChildren[3].TextBaselineY)
        };
        LayoutItem[] operatorPartItems = new LayoutItem[2]
        {
            new LayoutItem(_integralSymbol.DesiredSize),
            new LayoutItem(_variableOfIntegrationPrefix.DesiredSize, _variableOfIntegrationPrefix.TextBaselineY)
        };
        if (flag)
        {
            AdjacentUpperLimitHeight = Math.Max(visualIntegralOperatorNode.AdjacentUpperLimitHeight, visualIntegralOperatorNode.NodeChildren[1].DesiredSize.Height);
            AdjacentLowerLimitHeight = Math.Max(visualIntegralOperatorNode.AdjacentLowerLimitHeight, visualIntegralOperatorNode.NodeChildren[0].DesiredSize.Height);
        }
        _layout = new IntegralLayoutManager(childItems, operatorPartItems, flag, AdjacentUpperLimitHeight, AdjacentLowerLimitHeight);
        _layout.Arrange();
        return _layout.DesiredSize;
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        _nodeChildren[1].ArrangeMathNode(_layout.UpperLimitRect);
        _nodeChildren[0].ArrangeMathNode(_layout.LowerLimitRect);
        _nodeChildren[2].ArrangeMathNode(_layout.IntegrandRect);
        _nodeChildren[3].ArrangeMathNode(_layout.VariableRect);
        IntegralSymbol.ArrangeMathNode(_layout.IntegralSymbolRect);
        VariableOfIntegrationPrefix.ArrangeMathNode(_layout.VariablePrefixRect);
        return DesiredSize;
    }

    private double LimitsAndIntegralSymbolWidth()
    {
        double width = _nodeChildren[1].DesiredSize.Width;
        double width2 = IntegralSymbol.DesiredSize.Width;
        double width3 = _nodeChildren[0].DesiredSize.Width;
        return Math.Max(width, Math.Max(width2, width3));
    }
}
