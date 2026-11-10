using System;
using System.Windows;
using System.Windows.Media;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualStepRangeOperatorNode
{
    private double _baseline;
    private double _effectiveBaseline;
    private double _effectiveScriptBaseline;

    public override double EffectiveBaselineY => _effectiveBaseline;

    public override double EffectiveScriptBaselineY => _effectiveScriptBaseline;

    public override double TextBaselineY => _baseline;

    public VisualStepRangeOperatorNode()
        : base("STEPRANGE", new VisualSymbolNode(','), new VisualSymbolNode('‥'))
    {
    }

    protected override Size MeasureStaticLayout(Size availableSize)
    {
        _baseline = 0.0;
        _effectiveBaseline = 0.0;
        _effectiveScriptBaseline = 0.0;
        double num = 0.0;
        double num2 = 0.0;
        foreach (VisualMathNode child in Children)
        {
            child.MeasureMathNode(WpfUtils.SizeInfinity);
            num2 += child.DesiredSize.Width;
            if (!(child is VisualSymbolNode))
            {
                _baseline = Math.Max(_baseline, child.TextBaselineY);
                _effectiveBaseline = Math.Max(_effectiveBaseline, child.EffectiveBaselineY);
                _effectiveScriptBaseline = Math.Min(_effectiveScriptBaseline, child.EffectiveScriptBaselineY);
                num = Math.Max(num, child.DepthBelowBaseline);
            }
        }

        double num3 = _baseline + num;
        return new Size(num2, num3);
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        double num = 0.0;
        num += ArrangeChild(Operands[0], num);
        num += ArrangeChild(OperatorSymbols[0], num);
        num += ArrangeChild(Operands[1], num);
        num += ArrangeChild(OperatorSymbols[1], num);
        num += ArrangeChild(Operands[2], num);
        double num2 = num;
        Size desiredSize = DesiredSize;
        return new Size(num2, desiredSize.Height);
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        foreach (VisualMathNode child in Children)
        {
            GeneralTransform transform = TransformToDescendant(child);
            Point point = transform.Transform(hitPoint);

            // Release IL tests the type with isinst and then casts with castclass; a pattern variable emits one isinst.
            if (child is VisualSymbolNode)
            {
                VisualSymbolNode symbol = (VisualSymbolNode)child;
                if (point.X >= symbol.SymbolBounds.Left - symbol.Margin.Left && point.X <= symbol.SymbolBounds.Right + symbol.Margin.Right)
                {
                    if (symbol == OperatorSymbols[0])
                    {
                        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Left, hitPoint);
                    }

                    return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Right, hitPoint);
                }

                continue;
            }

            if (child == Operands[0])
            {
                if (point.X <= child.AlignmentPoint.X + child.ActualWidth + child.Margin.Right && !child.HideSubTreeFromClicks)
                {
                    return child.GetNodeAtPoint(transform.Transform(hitPoint));
                }
            }

            if (child == Operands[2])
            {
                if (point.X > child.AlignmentPoint.X - child.Margin.Left && !child.HideSubTreeFromClicks)
                {
                    return child.GetNodeAtPoint(transform.Transform(hitPoint));
                }
            }

            if (point.X >= child.AlignmentPoint.X - child.Margin.Left && point.X <= child.AlignmentPoint.X + child.ActualWidth + child.Margin.Right && !child.HideSubTreeFromClicks)
            {
                return child.GetNodeAtPoint(transform.Transform(hitPoint));
            }
        }

        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Indeterminate, hitPoint);
    }
}
