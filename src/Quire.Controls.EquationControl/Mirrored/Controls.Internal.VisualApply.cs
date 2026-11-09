using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal partial class VisualApply
{
    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        VisualMathNodeHitTestResult result = base.GetNodeAtPoint(hitPoint) as VisualMathNodeHitTestResult;
        if (result != null && (result.NodeHit == this || (result.NodeHit == SecondMathNode && result.CaretSideHit == global::Quire.Direction.Left)))
        {
            GeneralTransform transform = TransformToDescendant(FirstMathNode);
            return FirstMathNode.GetNodeAtPoint(transform.Transform(hitPoint));
        }

        return result;
    }

    public VisualApply()
    {
    }
}
