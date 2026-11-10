using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal partial class VisualSymKeywordsStackNode
{
    public override double EffectiveBaselineY => DesiredSize.Height;

    public override double EffectiveScriptBaselineY => 0.0;

    internal override HitTestResult GetNodeAtPoint(Point hitPoint)
    {
        Point point = TransformToDescendant(_operandsPanel).Transform(hitPoint);
        VisualMathNode node = _operandsPanel.CellFromPoint(point);
        GeneralTransform transform = TransformToDescendant(node);
        return node.GetNodeAtPoint(transform.Transform(hitPoint));
    }
}
