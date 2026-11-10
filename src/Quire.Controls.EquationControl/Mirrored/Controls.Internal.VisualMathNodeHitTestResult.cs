using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal partial class VisualMathNodeHitTestResult
{
    public readonly VisualMathNode NodeHit;
    public readonly Direction CaretSideHit;
    public readonly Direction NodeSideHit;
    public readonly int? OperatorPartIndexHit;
    public bool IsOperatorHit;

    public VisualMathNodeHitTestResult(VisualMathNode nodeHit, Direction caretSideHit, Direction nodeSideHit, Point pointHit)
        : this(nodeHit, caretSideHit, nodeSideHit, default(int?), pointHit)
    {
    }

    public VisualMathNodeHitTestResult(VisualMathNode nodeHit, Direction caretSideHit, int? operatorPartIndexHit, Point pointHit)
        : this(nodeHit, caretSideHit, Direction.Indeterminate, operatorPartIndexHit, pointHit)
    {
    }

    public VisualMathNodeHitTestResult(VisualMathNode nodeHit, Direction caretSideHit, Direction nodeSideHit, int? operatorPartIndexHit, Point pointHit)
        : base(nodeHit, pointHit)
    {
        NodeHit = nodeHit;
        CaretSideHit = caretSideHit;
        NodeSideHit = nodeSideHit;
        OperatorPartIndexHit = operatorPartIndexHit;
    }
}
