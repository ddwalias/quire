using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualLimitSideNode : VisualMathNode, IVisualLimitSideNode, IVisualMathNode, IVisualNode
{
    public override INodeInfo NodeInfo
    {
        get => base.NodeInfo;
        set
        {
            base.NodeInfo = value;
            CreateVisualChildren();
        }
    }

    public VisualMathNode Child
    {
        get
        {
            if (Children.Count != 1)
            {
                return null;
            }
            return (VisualMathNode)Children[0];
        }
    }

    public bool IsPlaceholder => Child is IVisualPlaceholderNode;

    public override double EffectiveBaselineY
    {
        get
        {
            if (Child == null)
            {
                return 0.0;
            }
            return Child.EffectiveBaselineY;
        }
    }

    public override double EffectiveScriptBaselineY
    {
        get
        {
            if (Child == null)
            {
                return 0.0;
            }
            return Child.EffectiveScriptBaselineY;
        }
    }

    public override double TextBaselineY
    {
        get
        {
            if (Child == null)
            {
                return 0.0;
            }
            return Child.TextBaselineY;
        }
    }

    private void CreateVisualChildren()
    {
        Children.Clear();
        LimitSideNodeInfo limitSideNodeInfo = (LimitSideNodeInfo)NodeInfo;
        VisualMathNode visualMathNode = null;
        switch (limitSideNodeInfo.Value)
        {
            case LimitSideNodeInfo.LimitSide.TwoSided:
                visualMathNode = new VisualPlaceholderNode();
                break;
            case LimitSideNodeInfo.LimitSide.LeftHandSide:
                visualMathNode = new VisualSymbolNode('-');
                break;
            case LimitSideNodeInfo.LimitSide.RightHandSide:
                visualMathNode = new VisualSymbolNode('+');
                break;
        }
        if (visualMathNode != null)
        {
            Children.Add(visualMathNode);
        }
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Indeterminate, hitPoint);
    }

    protected override void OnShowOperatorOnlyWhenActiveChanged(bool value)
    {
        if (value)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath(EquationControl.ActiveElementOpacityProperty);
            SetBinding(UIElement.OpacityProperty, binding);
        }
    }

    protected override Size MeasureStaticLayout(Size availableSize)
    {
        if (Children.Count != 1)
        {
            return new Size(0.0, 0.0);
        }
        Children[0].Measure(availableSize);
        return Children[0].DesiredSize;
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        ((VisualMathNode)Children[0]).ArrangeMathNode(new Rect(DesiredSize));
        return DesiredSize;
    }

    public VisualLimitSideNode()
    {
    }
}
