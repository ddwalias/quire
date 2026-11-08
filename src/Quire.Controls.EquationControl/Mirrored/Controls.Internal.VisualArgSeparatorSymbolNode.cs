using System.Windows;

namespace Quire.Controls.Internal;

internal partial class VisualArgSeparatorSymbolNode
{
    public VisualArgSeparatorSymbolNode()
        : base(',')
    {
        _automaticallyAlign = false;
        _alignmentPoint = new Point(0.0, -4.4);
        HorizontalMargin = 2;
    }
}
