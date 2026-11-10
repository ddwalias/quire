using System.Windows;
using System.Windows.Media;
namespace Quire.Controls.Internal;

internal partial class VisualStepRangeOperatorNode
{
    public override double EffectiveBaselineY => _effectiveBaseline;

    public override double EffectiveScriptBaselineY => _effectiveScriptBaseline;

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
