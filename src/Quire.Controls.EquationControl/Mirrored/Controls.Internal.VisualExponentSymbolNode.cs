using System.Windows;

namespace Quire.Controls.Internal;

internal partial class VisualExponentSymbolNode
{
    public VisualExponentSymbolNode()
        : base('^')
    {
    }

    protected override Size ArrangeStaticLayout(Size finalSize)
    {
        return new Size(DesiredSize.Width - Margin.Left - Margin.Right, DesiredSize.Height - Margin.Top - Margin.Bottom);
    }
}
