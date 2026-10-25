#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using Quire.Core;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class RichTextBoxExtensions
{
    [MirrorVerified("Release IL")]
    private static TextDecorationCollection _textDecorations;

    [MirrorVerified("Release IL")]
    public static void ApplyTextFormatValues(this FlowDocument doc, TextFormat textFormat)
    {
        doc.Background = textFormat.Background;
        doc.Foreground = textFormat.Foreground;
        doc.FontWeight = textFormat.FontWeight;
        doc.FontStyle = textFormat.FontStyle;
        doc.FontSize = textFormat.FontSize;
        doc.FontFamily = textFormat.FontFamily;
        _textDecorations = textFormat.TextDecorations.Clone();
    }

    // Release IL requires these ordered type branches to target their bodies and shared return directly.
    public static void ClearTextFormatPropsRecursively(this Block block, global::Quire.DelegateFunction1<bool, TextElement> condition, params DependencyProperty[] props)
    {
        if (condition(block))
        {
            ((IEnumerable<DependencyProperty>)props).Each((Action<DependencyProperty>)((DependencyObject)block).ClearValue);
        }

        Paragraph paragraph = block as Paragraph;
        Table table;
        Section section;
        List list;
        if (paragraph != null)
        {
            goto ParagraphBody;
        }
        table = block as Table;
        if (table != null)
        {
            goto TableBody;
        }
        section = block as Section;
        if (section != null)
        {
            goto SectionBody;
        }
        list = block as List;
        if (list != null)
        {
            goto ListBody;
        }
        if (!(block is BlockUIContainer))
        {
            goto Failure;
        }
        goto Return;

    ParagraphBody:
        paragraph.Inlines.Each((Inline inline) => inline.ClearTextFormatPropsRecursively(condition, props));
        return;

    TableBody:
        table.RowGroups.Each((TableRowGroup rowGroup) => rowGroup.ClearTextFormatPropsRecursively(condition, props));
        return;

    SectionBody:
        section.Blocks.Each((Block sectionBlock) => sectionBlock.ClearTextFormatPropsRecursively(condition, props));
        return;

    ListBody:
        list.ListItems.Each((ListItem listItem) => listItem.ClearTextFormatPropsRecursively(condition, props));
        return;

    Failure:
        DebugTools.FailAssertion(block?.GetType().Name);

    Return:
        return;
    }

    public static void ClearTextFormatPropsRecursively(this TextElement textElement, global::Quire.DelegateFunction1<bool, TextElement> condition, params DependencyProperty[] props)
    {
        if (condition(textElement))
        {
            ((IEnumerable<DependencyProperty>)props).Each((Action<DependencyProperty>)((DependencyObject)textElement).ClearValue);
        }

        switch (textElement)
        {
            case Block block:
                block.ClearTextFormatPropsRecursively(condition, props);
                return;
            case Inline inline:
                inline.ClearTextFormatPropsRecursively(condition, props);
                return;
            case ListItem listItem:
                listItem.Blocks.Each((Block childBlock) => childBlock.ClearTextFormatPropsRecursively(condition, props));
                return;
            case TableCell tableCell:
                tableCell.Blocks.Each((Block childBlock) => childBlock.ClearTextFormatPropsRecursively(condition, props));
                return;
            case TableRow tableRow:
                tableRow.Cells.Each((TableCell cell) => cell.ClearTextFormatPropsRecursively(condition, props));
                return;
            case TableRowGroup tableRowGroup:
                tableRowGroup.Rows.Each((TableRow row) => row.ClearTextFormatPropsRecursively(condition, props));
                return;
            default:
                DebugTools.FailAssertion(textElement?.GetType().Name);
                return;
        }
    }

    // Release IL requires these ordered type branches to target their bodies and shared return directly.
    public static void ClearTextFormatPropsRecursively(this Inline inline, global::Quire.DelegateFunction1<bool, TextElement> condition, params DependencyProperty[] props)
    {
        if (condition(inline))
        {
            ((IEnumerable<DependencyProperty>)props).Each((Action<DependencyProperty>)((DependencyObject)inline).ClearValue);
        }

        AnchoredBlock anchoredBlock = inline as AnchoredBlock;
        Span span;
        Run run;
        if (anchoredBlock != null)
        {
            goto AnchoredBlockBody;
        }
        span = inline as Span;
        if (span != null)
        {
            goto SpanBody;
        }
        if (inline is InlineUIContainer)
        {
            goto Return;
        }
        if (inline is LineBreak)
        {
            goto Return;
        }
        run = inline as Run;
        if (run == null)
        {
            goto Failure;
        }
        goto RunBody;

    AnchoredBlockBody:
        anchoredBlock.Blocks.Each((Block block) => block.ClearTextFormatPropsRecursively(condition, props));
        return;

    SpanBody:
        span.Inlines.Each((Inline spanInline) => spanInline.ClearTextFormatPropsRecursively(condition, props));
        return;

    RunBody:
        if (!condition(run))
        {
            goto Return;
        }
        ((IEnumerable<DependencyProperty>)props).Each((Action<DependencyProperty>)((DependencyObject)inline).ClearValue);
        inline.TextDecorations = _textDecorations;
        TextElementAttachedProperties.SetStyleKey((DependencyObject)run, "Normal");
        return;

    Failure:
        DebugTools.FailAssertion(inline?.GetType().Name);

    Return:
        return;
    }

    // Release IL comparison requires these labels to preserve Mathcad's recursive branch order.
    public static bool Satisfy(this TextElement textElement, global::Quire.DelegateFunction1<bool, TextElement> condition)
    {
        if (!condition(textElement))
        {
            return false;
        }
        Block block = textElement as Block;
        Inline inline;
        ListItem listItem;
        TableCell tableCell;
        TableRow tableRow;
        TableRowGroup tableRowGroup;
        bool result;
        if (block != null)
        {
            goto BlockBody;
        }
        inline = textElement as Inline;
        if (inline != null)
        {
            goto InlineBody;
        }
        listItem = textElement as ListItem;
        if (listItem != null)
        {
            goto ListItemBody;
        }
        tableCell = textElement as TableCell;
        if (tableCell != null)
        {
            goto TableCellBody;
        }
        tableRow = textElement as TableRow;
        if (tableRow != null)
        {
            goto TableRowBody;
        }
        tableRowGroup = textElement as TableRowGroup;
        if (tableRowGroup == null)
        {
            goto FalseResult;
        }
        else
        {
            goto TableRowGroupBody;
        }

    BlockBody:
        result = block.Satisfy(condition);
        goto ReturnResult;

    InlineBody:
        result = inline.Satisfy(condition);
        goto ReturnResult;

    ListItemBody:
        result = listItem.Blocks.All((Block block2) => block2.Satisfy(condition));
        goto ReturnResult;

    TableCellBody:
        result = tableCell.Blocks.All((Block block2) => block2.Satisfy(condition));
        goto ReturnResult;

    TableRowBody:
        result = tableRow.Cells.All((TableCell cell) => cell.Satisfy(condition));
        goto ReturnResult;

    TableRowGroupBody:
        result = tableRowGroup.Rows.All((TableRow row) => row.Satisfy(condition));
        goto ReturnResult;

    FalseResult:
        result = false;

    ReturnResult:
        return result;
    }

    public static bool Satisfy(this Block block, global::Quire.DelegateFunction1<bool, TextElement> condition)
    {
        if (!condition(block))
        {
            return false;
        }
        Paragraph paragraph = block as Paragraph;
        Section section;
        List list;
        Table table;
        bool result;
        if (paragraph != null)
        {
            goto ParagraphBody;
        }
        section = block as Section;
        if (section != null)
        {
            goto SectionBody;
        }
        list = block as List;
        if (list != null)
        {
            goto ListBody;
        }
        table = block as Table;
        if (table != null)
        {
            goto TableBody;
        }
        if (!(block is BlockUIContainer))
        {
            goto FalseResult;
        }
        else
        {
            goto BlockUiResult;
        }

    ParagraphBody:
        result = paragraph.Inlines.All((Inline inline) => inline.Satisfy(condition));
        goto ReturnResult;

    SectionBody:
        result = section.Blocks.All((Block sectionBlock) => sectionBlock.Satisfy(condition));
        goto ReturnResult;

    ListBody:
        result = list.ListItems.All((ListItem listItem) => listItem.Satisfy(condition));
        goto ReturnResult;

    TableBody:
        result = table.RowGroups.All((TableRowGroup rowGroup) => rowGroup.Satisfy(condition));
        goto ReturnResult;

    BlockUiResult:
        result = true;
        goto ReturnResult;

    FalseResult:
        result = false;

    ReturnResult:
        return result;
    }

    public static bool Satisfy(this Inline inline, global::Quire.DelegateFunction1<bool, TextElement> condition)
    {
        if (!condition(inline))
        {
            return false;
        }
        AnchoredBlock anchoredBlock = inline as AnchoredBlock;
        Span span;
        bool result;
        if (anchoredBlock != null)
        {
            goto AnchoredBlockBody;
        }
        span = inline as Span;
        if (span != null)
        {
            goto SpanBody;
        }
        if (inline is InlineUIContainer)
        {
            goto InlineUIResult;
        }
        if (inline is LineBreak)
        {
            goto LineBreakResult;
        }
        if (!(inline is Run))
        {
            goto FalseResult;
        }
        else
        {
            goto RunResult;
        }

    AnchoredBlockBody:
        result = anchoredBlock.Blocks.All((Block block) => block.Satisfy(condition));
        goto ReturnResult;

    SpanBody:
        result = span.Inlines.All((Inline spanInline) => spanInline.Satisfy(condition));
        goto ReturnResult;

    InlineUIResult:
        result = true;
        goto ReturnResult;

    LineBreakResult:
        result = true;
        goto ReturnResult;

    RunResult:
        result = true;
        goto ReturnResult;

    FalseResult:
        result = false;

    ReturnResult:
        return result;
    }
}
