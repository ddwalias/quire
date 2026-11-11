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

internal partial class VisualGradientOperatorNode : VisualMathNode
{
    private static readonly MathNodeDescriptor GradientDescriptor = DefaultDescriptorTable.Default["GRADIENT"];

    private GradientLayoutManager _layout;

    private const int VariableListOperandIndex = 0;

    private const int FunctionOperandIndex = 1;

    internal VisualSymbolNode GradientSymbol { get; }

    internal VisualMathNode[] NodeChildren { get; }

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[1] { GradientSymbol };

    public override IEnumerable<UIElement> CoreElements => new UIElement[2]
    {
        GradientSymbol,
        (UIElement)this[0]
    };

    public override double TextBaselineY => _layout?.TextBaselineOffset ?? 0.0;

    public override double EffectiveBaselineY
    {
        get
        {
            if (_layout != null)
            {
                Rect variableListRect = _layout.VariableListRect;
                return variableListRect.Top + NodeChildren[0].EffectiveBaselineY;
            }
            return 0.0;
        }
    }

    public override double EffectiveScriptBaselineY
    {
        get
        {
            if (_layout != null)
            {
                Rect functionRect = _layout.FunctionRect;
                return functionRect.Top + NodeChildren[1].EffectiveScriptBaselineY;
            }
            return 0.0;
        }
    }

    public override IVisualMathNode this[int index]
    {
        get => NodeChildren[index];
        set
        {
            VisualMathNode visualMathNode = value as VisualMathNode;
            NodeChildren[index] = visualMathNode;
            if (visualMathNode != null && index == 0)
            {
                double scaledFontSize = FontManager.GetScaledFontSize(MathFormat, FontConstants.ScriptFontSizeRatio);
                DebugTools.DevAssert(visualMathNode.MathFormat.FontSize.HasValue && visualMathNode.MathFormat.FontSize.Value.AreSame(scaledFontSize), null);
                if (visualMathNode is VisualPlaceholderNode visualPlaceholderNode)
                {
                    visualPlaceholderNode.ShowOperatorOnlyWhenActive = true;
                }
            }
            int navigablePartCount = GradientDescriptor.NavigablePartCount;
            if (index + navigablePartCount < Children.Count)
            {
                Children[index + navigablePartCount] = visualMathNode;
            }
            else if (visualMathNode != null)
            {
                Children.Add(visualMathNode);
            }
        }
    }

    public VisualGradientOperatorNode()
    {
        NodeChildren = new VisualMathNode[GradientDescriptor.ChildCount];
        char opSymbol = DebugTools.AssertNotNull(GradientDescriptor.OperatorSymbol, null).Passed ? GradientDescriptor.OperatorSymbol.Value : '∇';
        GradientSymbol = new VisualSymbolNode(opSymbol);
        Children.Add(GradientSymbol);
    }

    public override void ModifyChildMathFormat(int childIndex, MathFormatValues childMathFormat)
    {
        if (childIndex == 0)
        {
            childMathFormat.FontSize = FontManager.GetScaledFontSize(MathFormat, FontConstants.ScriptFontSizeRatio);
        }
    }

    public void SetVisibilityOfOperatorSymbol(Visibility visibility)
    {
        GradientSymbol.Visibility = visibility;
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        NodeChildren[0].ArrangeMathNode(_layout.VariableListRect);
        NodeChildren[1].ArrangeMathNode(_layout.FunctionRect);
        GradientSymbol.ArrangeMathNode(_layout.GradientSymbolRect);
        return DesiredSize;
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        _layout.FindClosestChild(hitPoint, out var childType, out var childIndex, out var childSideHit);
        HitTestResult result;
        if (childType == LayoutChildType.Child)
        {
            VisualMathNode visualMathNode = NodeChildren[childIndex];
            GeneralTransform generalTransform = TransformToDescendant(visualMathNode);
            result = visualMathNode.GetNodeAtPoint(generalTransform.Transform(hitPoint));
        }
        else
        {
            Direction caretSideHit = ((childSideHit == Direction.Left) ? Direction.Left : Direction.Indeterminate);
            result = new VisualMathNodeHitTestResult(this, caretSideHit, Direction.Indeterminate, hitPoint);
        }
        return result;
    }
}
