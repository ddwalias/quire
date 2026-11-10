using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualLeafOperatorNode
{
    public VisualLeafOperatorNode(string symbol)
        : base(symbol, 0)
    {
        VisualMathNode.SetOperatorFontProperties(MathFormat);
    }

    public override IVisualMathNode[] NavigatableParts => new IVisualMathNode[1] { this };

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
    }

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        if (hitPoint.X <= 0.0)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Left, Direction.Indeterminate, hitPoint);
        }
        if (hitPoint.X >= DesiredSize.Width)
        {
            return new VisualMathNodeHitTestResult(this, Direction.Right, Direction.Indeterminate, hitPoint);
        }
        return new VisualMathNodeHitTestResult(this, Direction.Indeterminate, Direction.Indeterminate, hitPoint);
    }
}
