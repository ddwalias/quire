#nullable disable
using System.Windows;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public static partial class TextElementAttachedProperties
{
    private static void HandleTextStyleDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        FrameworkContentElement frameworkContentElement = d as FrameworkContentElement;
        if (frameworkContentElement != null
            && Quire.Wpf.WpfUtils.HasLocalValue(frameworkContentElement, StyleKeyProperty)
            && e.NewValue == null)
        {
            ClearTextStyleData(frameworkContentElement);
        }
    }

    [MirrorVerified("PtcTextElementAttachedPropertiesMechanicalDifferentialTests")]
    public static DependencyProperty StyleKeyProperty;

    [MirrorVerified("PtcTextElementAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearTextStyleData(DependencyObject element) => element.ClearValue(StyleKeyProperty);

    [MirrorVerified("Release IL")]
    public static string GetStyleKey(DependencyObject element) => element.GetValue(StyleKeyProperty) as string;

    public static void SetStyleKey(DependencyObject element, string value)
    {
        if (value.IsNullOrEmpty())
        {
            value = DefaultTextStyle;
        }
        element.SetValue(StyleKeyProperty, value);
    }

    public static void SetTextStyleAndBind(DependencyObject element, TextStyleData styleData, bool isApply, bool isSelected, System.Windows.Controls.RichTextBox rtb = default)
    {
        FrameworkContentElement frameworkContentElement = element as FrameworkContentElement;
        if (frameworkContentElement == null)
        {
            return;
        }
        if (styleData != null)
        {
            frameworkContentElement.Tag = styleData.StyleKey;
            TextStyle textStyle = new TextStyle(styleData, true);
            SetTextStyleOnTextRegion(frameworkContentElement, textStyle, isApply, isSelected, rtb);
            SetTextStyleOnElement(frameworkContentElement, textStyle, isApply, isSelected);
            SetTextStyleOnParagraph(frameworkContentElement, textStyle, isApply, isSelected);
            SetStyleKey(frameworkContentElement, styleData.StyleKey);
            return;
        }
        frameworkContentElement.ClearValue(FrameworkElement.StyleProperty);
    }

    private static void SetTextStyleOnParagraph(FrameworkContentElement element, ITextStyle textStyle, bool isApply, bool isSelected)
    {
        FrameworkContentElement frameworkContentElement;
        double? textIndent;
        Paragraph paragraph;
        TextAlignment? textAlignment;
        Thickness? margin;

        frameworkContentElement = Quire.Wpf.WpfUtils.FindLogicalAncestor<List>(element);
        if (frameworkContentElement == null)
        {
            frameworkContentElement = Quire.Wpf.WpfUtils.FindLogicalAncestor<Paragraph>(element);
        }
        if (frameworkContentElement != null)
        {
            textIndent = textStyle.TextIndent;
            if (textIndent.HasValue)
            {
                if (!isApply)
                {
                    paragraph = element.Parent as Paragraph;
                    if (paragraph == null || textStyle.IsModifyProperty(Paragraph.TextIndentProperty.Name, paragraph.GetValue(Paragraph.TextIndentProperty)))
                    {
                        goto AfterTextIndent;
                    }
                }
                frameworkContentElement.SetValue(Paragraph.TextIndentProperty, textStyle.TextIndent);
            AfterTextIndent:;
            }
            else if (textStyle.IsDeletedProperty(Block.TextAlignmentProperty.Name, frameworkContentElement.GetValue(Paragraph.TextIndentProperty)))
            {
                frameworkContentElement.SetValue(Paragraph.TextIndentProperty, 0.0);
            }

            textAlignment = textStyle.TextAlignment;
            if (textAlignment.HasValue)
            {
                if (isApply || !textStyle.IsModifyProperty(Block.TextAlignmentProperty.Name, element.GetValue(Block.TextAlignmentProperty)))
                {
                    frameworkContentElement.SetValue(Block.TextAlignmentProperty, textStyle.TextAlignment);
                }
            }
            else if (textStyle.IsDeletedProperty(Block.TextAlignmentProperty.Name, element.GetValue(Block.TextAlignmentProperty)))
            {
                frameworkContentElement.SetValue(Block.TextAlignmentProperty, TextAlignment.Left);
            }

            margin = textStyle.Margin;
            if (margin.HasValue)
            {
                frameworkContentElement.SetValue(Block.MarginProperty, textStyle.Margin);
            }
        }
    }

    private static void SetTextStyleOnTextRegion(FrameworkContentElement element, ITextStyle textStyle, bool isApply, bool isSelected, System.Windows.Controls.RichTextBox rtb = default)
    {
        if (rtb != null)
        {
            if (textStyle.MarkerStyle.HasValue)
            {
                if (isApply)
                {
                    if ((textStyle.MarkerStyle != TextMarkerStyle.None) & isSelected)
                    {
                        rtb.SelectAll();
                    }
                    Block block = Quire.Wpf.WpfUtils.FindLogicalAncestor<List>(element);
                    if (block == null)
                    {
                        block = Quire.Wpf.WpfUtils.FindLogicalAncestor<Paragraph>(element);
                    }
                    SetTextStyleMarker(element, textStyle, isApply, isSelected, block, rtb);
                }
                else
                {
                    foreach (Block item in rtb.Document.Blocks.ToList())
                    {
                        SetTextStyleMarker(element, textStyle, isApply, isSelected, item, rtb);
                    }
                }
            }
            else
            {
                List lastBlockForBullets = rtb.Document.Blocks.LastBlock as List;
                if (lastBlockForBullets != null
                    && lastBlockForBullets.MarkerStyle == TextMarkerStyle.Disc
                    && textStyle.IsDeletedProperty(Quire.Core.TextFormat.MarkerStyleProperty.Name, TextMarkerStyle.Disc))
                {
                    EditingCommands.ToggleBullets.Execute(null, rtb);
                }
                else
                {
                    List lastBlockForNumbering = rtb.Document.Blocks.LastBlock as List;
                    if (lastBlockForNumbering != null
                        && lastBlockForNumbering.MarkerStyle == TextMarkerStyle.Decimal
                        && textStyle.IsDeletedProperty(Quire.Core.TextFormat.MarkerStyleProperty.Name, TextMarkerStyle.Decimal))
                    {
                        EditingCommands.ToggleNumbering.Execute(null, rtb);
                    }
                }
            }
            if (textStyle.Background != null
                && (
                    (isSelected & isApply)
                    || (
                        rtb.Document.Background != null
                        && ((SolidColorBrush)rtb.Document.Background).Color != Colors.Transparent
                        && !isApply
                        && !textStyle.IsModifyPropertyString(TextElement.BackgroundProperty.Name, rtb.Document.GetValue(TextElement.BackgroundProperty))
                    )
                )
            )
            {
                rtb.Document.SetValue(TextElement.BackgroundProperty, textStyle.Background);
            }
        }
    }
}
