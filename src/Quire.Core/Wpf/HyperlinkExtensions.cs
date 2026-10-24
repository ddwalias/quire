#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class HyperlinkExtensions
{
    [MirrorVerified("Release IL")]
    public static void ClearForegroundInDepth(this Hyperlink link)
    {
        link.ClearLocalValuesRecursively(TextElement.ForegroundProperty);
    }

    [MirrorVerified("Release IL")]
    public static void ResetLinkStyleProps(this Hyperlink link)
    {
        link.ClearValue(TextElement.ForegroundProperty);
        link.ClearValue(TextBlock.TextDecorationsProperty);
        link.ClearValue(FrameworkContentElement.ToolTipProperty);
    }

    [MirrorVerified("Release IL")]
    public static string GetText(this Hyperlink link)
    {
        System.Windows.Documents.TextRange textRange = new System.Windows.Documents.TextRange(link.ContentStart, link.ContentEnd);
        return textRange.Text;
    }

    public static BlockCollection GetParentBlocks(this Block block)
    {
        DependencyObject parent = block.Parent;
        if (parent == null)
        {
            return null;
        }
        switch (parent)
        {
            case FlowDocument flowDocument:
                return flowDocument.Blocks;
            case ListItem listItem:
                return listItem.Blocks;
            case Section section:
                return section.Blocks;
            case TableCell tableCell:
                return tableCell.Blocks;
            default:
                return null;
        }
    }
}
