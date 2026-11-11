using System;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualLimitOperatorNode : VisualMathNode
{
    private static readonly MathNodeDescriptor _limitDescriptor = DefaultDescriptorTable.Default["LIMIT"];

    private readonly VisualMathNode[] _nodeChildren = new VisualMathNode[_limitDescriptor.ChildCount];

    private readonly VisualSymbolNode _limitSymbol;

    private readonly VisualSymbolNode _limitArrow;

    private const int OperatorPartCount = 2;

    private LimitLayoutManager _layout;

    private const int LimitVariableOperandIndex = 0;

    private const int LimitVariableExpressionOperandIndex = 1;

    private const int LimitSideOperandIndex = 2;

    private const int LimitExpressionOperandIndex = 3;

    public override IVisualMathNode this[int index]
    {
        get => _nodeChildren[index];
        set
        {
            if (index >= _nodeChildren.Length)
            {
                throw new InvalidOperationException(string.Format("Index cannot be greater than {0} when adding child nodes to a limit operator.", _nodeChildren.Length - 1));
            }
            VisualMathNode visualMathNode = (VisualMathNode)value;
            _nodeChildren[index] = visualMathNode;
            if (Children.Count > index + 2)
            {
                Children[index + 2] = visualMathNode;
            }
            else
            {
                Children.Add(visualMathNode);
            }
            if (visualMathNode != null && index == 2)
            {
                VisualLimitSideNode visualLimitSideNode = (VisualLimitSideNode)visualMathNode;
                double scaledFontSize = FontManager.GetScaledFontSize(MathFormat, FontConstants.ScriptFontSizeRatio);
                DebugTools.DevAssert(visualLimitSideNode.MathFormat.FontSize == scaledFontSize, null);
                if (visualLimitSideNode.IsPlaceholder)
                {
                    visualMathNode.ShowOperatorOnlyWhenActive = true;
                }
            }
        }
    }

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[2] { LimitSymbol, LimitArrow };

    public VisualSymbolNode LimitSymbol => _limitSymbol;

    public VisualSymbolNode LimitArrow => _limitArrow;

    public VisualMathNode[] NodeChildren => _nodeChildren;

    public Point LimitSymbolTopLeft
    {
        get
        {
            Rect limitSymbolRect = _layout.LimitSymbolRect;
            return limitSymbolRect.TopLeft;
        }
    }

    public Point LimitArrowTopLeft
    {
        get
        {
            Rect limitArrowRect = _layout.LimitArrowRect;
            return limitArrowRect.TopLeft;
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
            Size desiredSize = DesiredSize;
            return desiredSize.Height;
        }
    }

    public override double EffectiveScriptBaselineY => _nodeChildren[3].EffectiveScriptBaselineY;

    public VisualLimitOperatorNode()
    {
        _limitSymbol = new VisualSymbolNode("lim");
        _limitArrow = new VisualSymbolNode('→');
        Children.Add(_limitSymbol);
        Children.Add(_limitArrow);
    }

    public override void ModifyChildMathFormat(int childIndex, MathFormatValues childMathFormat)
    {
        if (childIndex == 2)
        {
            childMathFormat.FontSize = FontManager.GetScaledFontSize(MathFormat, FontConstants.ScriptFontSizeRatio);
        }
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
        LimitSymbol.Measure(availableSize);
        LimitArrow.Measure(availableSize);
        childItems = new LayoutItem[4]
        {
            new LayoutItem(_nodeChildren[0].DesiredSize, _nodeChildren[0].TextBaselineY),
            new LayoutItem(_nodeChildren[1].DesiredSize, _nodeChildren[1].TextBaselineY),
            new LayoutItem(_nodeChildren[2].DesiredSize, _nodeChildren[2].TextBaselineY),
            new LayoutItem(_nodeChildren[3].DesiredSize, _nodeChildren[3].TextBaselineY)
        };
        operatorPartItems = new LayoutItem[2]
        {
            new LayoutItem(_limitSymbol.DesiredSize, _limitSymbol.TextBaselineY),
            new LayoutItem(_limitArrow.DesiredSize, _limitArrow.TextBaselineY)
        };
        _layout = new LimitLayoutManager(childItems, operatorPartItems);
        _layout.Arrange();
        return _layout.DesiredSize;
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        _nodeChildren[0].ArrangeMathNode(_layout.LimitVariableRect);
        _nodeChildren[1].ArrangeMathNode(_layout.LimitVariableExpressionRect);
        _nodeChildren[2].ArrangeMathNode(_layout.LimitSideRect);
        _nodeChildren[3].ArrangeMathNode(_layout.LimitExpressionRect);
        LimitSymbol.ArrangeMathNode(_layout.LimitSymbolRect);
        LimitArrow.ArrangeMathNode(_layout.LimitArrowRect);
        return DesiredSize;
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        _layout.FindClosestChild(hitPoint, out var childType, out var childIndex, out var childSideHit);
        Direction caretSideHit;
        if (childType == LayoutChildType.Child)
        {
            VisualMathNode visualMathNode = _nodeChildren[childIndex];
            GeneralTransform generalTransform = TransformToDescendant(visualMathNode);
            return visualMathNode.GetNodeAtPoint(generalTransform.Transform(hitPoint));
        }
        caretSideHit = ((childSideHit == Direction.Left) ? Direction.Left : Direction.Indeterminate);
        return new VisualMathNodeHitTestResult(this, caretSideHit, Direction.Indeterminate, hitPoint);
    }
}
