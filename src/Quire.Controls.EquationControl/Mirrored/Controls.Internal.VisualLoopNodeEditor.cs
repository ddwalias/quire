using System.Windows;

namespace Quire.Controls.Internal;

internal partial class VisualLoopNodeEditor
{
    public VisualLoopNodeEditor(VisualMathNode editNode, VisualMathRegionNode region)
        : base((VisualLoopNode)editNode, region)
    {
    }

    public override void ShowOperatorHighlight(bool show)
    {
        ((VisualLoopNode)EditNode).SetVisibilityOfOperatorSymbol(show ? Visibility.Hidden : Visibility.Visible);
        _showOperatorHighlight = show;
        InvalidateVisual();
    }
}
