#nullable disable
using System.Linq;
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
    public static void ResetLinkStyleProps(this Hyperlink link)
    {
        link.ClearValue(TextElement.ForegroundProperty);
        link.ClearValue(TextBlock.TextDecorationsProperty);
        link.ClearValue(FrameworkContentElement.ToolTipProperty);
    }

    public static void SetTextFormatForLinkRemoval(this Hyperlink link, out List<Inline> inlines)
    {
        inlines = link.Inlines.ToList();
        if (global::Quire.DebugTools.DevAssert(inlines.IsNotEmpty(), null).Failed)
        {
            return;
        }
        DependencyProperty[] array = new DependencyProperty[8]
        {
            TextElement.FontFamilyProperty,
            TextElement.FontSizeProperty,
            TextElement.FontWeightProperty,
            TextElement.FontStyleProperty,
            Inline.TextDecorationsProperty,
            Typography.VariantsProperty,
            TextElement.ForegroundProperty,
            TextElement.BackgroundProperty
        };
        foreach (DependencyProperty prop in array)
        {
            if (DependencyPropertyHelper.GetValueSource(link, prop).BaseValueSource != BaseValueSource.Local)
            {
                continue;
            }
            object value = link.GetValue(prop);
            inlines.Each((Inline inline) =>
            {
                if (DependencyPropertyHelper.GetValueSource(inline, prop).BaseValueSource != BaseValueSource.Local)
                {
                    inline.SetValue(prop, value);
                }
            });
        }
        if (!ApplyNeighborFormattingValueIfUnset(inlines, link.ContentStart, LogicalDirection.Backward))
        {
            ApplyNeighborFormattingValueIfUnset(inlines, link.ContentEnd, LogicalDirection.Forward);
        }
    }

    // Preserve Mathcad's preceding helper position for the callback closure ordinal.
    private static bool ApplyNeighborFormattingValueIfUnset(List<Inline> inlines, TextPointer currentPosition, LogicalDirection direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    [MirrorVerified("Release IL")]
    public static void ClearForegroundInDepth(this Hyperlink link)
    {
        link.ClearLocalValuesRecursively(TextElement.ForegroundProperty);
    }

    public static void ClearLocalValuesRecursively(this Inline inline, params DependencyProperty[] properties)
    {
        ((IEnumerable<DependencyProperty>)properties).Each((Action<DependencyProperty>)((DependencyObject)inline).ClearValue);
        if (inline is Span span)
        {
            span.Inlines.ToList().Each((Inline inlineChild) =>
            {
                inlineChild.ClearLocalValuesRecursively(properties);
            });
        }
    }

    public static Span RecursiveRemoveEmptyInlinesIncludingSelf(this Span parentElement)
    {
        foreach (Inline item in parentElement.Inlines.ToList())
        {
            switch (item)
            {
                case Span childSpan:
                    childSpan.RecursiveRemoveEmptyInlinesIncludingSelf();
                    break;
                case Run run when run.Text.IsNullOrEmpty():
                    parentElement.Inlines.Remove(run);
                    break;
            }
        }
        Span result = parentElement;
        if (result.Inlines.IsNullOrEmpty())
        {
            result.GetParentInlines().Remove(result);
            result = null;
        }
        return result;
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
