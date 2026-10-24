#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class HyperlinkExtensions
{
    public static Hyperlink CreateLink(Uri uri, TextPointer startPos, TextPointer endPos)
    {
        Hyperlink hyperlink = new Hyperlink(startPos, endPos)
        {
            NavigateUri = uri
        };
        hyperlink.RecursiveRemoveEmptyInlinesIncludingSelf();
        return hyperlink;
    }

    public static Hyperlink CreateLink(Uri uri, Inline childInline, TextPointer insertionPosition = default)
    {
        Hyperlink hyperlink = insertionPosition == null ? new Hyperlink(childInline) : new Hyperlink(childInline, insertionPosition);
        hyperlink.NavigateUri = uri;
        hyperlink.RecursiveRemoveEmptyInlinesIncludingSelf();
        return hyperlink;
    }

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

    public static InlineCollection GetParentInlines(this Inline inline)
    {
        DependencyObject parent = inline.Parent;
        if (parent == null)
        {
            return null;
        }
        switch (parent)
        {
            case Span span:
                return span.Inlines;
            case Paragraph paragraph:
                return paragraph.Inlines;
            default:
                return null;
        }
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
