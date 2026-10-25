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
    private static void SetTextStyleMarker(FrameworkContentElement element, ITextStyle textStyle, bool isApply, bool isSelected, Block block, System.Windows.Controls.RichTextBox rtb = null)
    {
        if ((textStyle.MarkerStyle == TextMarkerStyle.None && block is List discList && discList.MarkerStyle == TextMarkerStyle.Disc)
            || (textStyle.MarkerStyle == TextMarkerStyle.Disc && (block is Paragraph || block is List nonDiscList && nonDiscList.MarkerStyle != TextMarkerStyle.Disc)))
        {
            List list = block as List;
            if (isApply || !textStyle.IsModifyProperty(List.MarkerStyleProperty.Name, list?.MarkerStyle ?? TextMarkerStyle.None))
            {
                if (list != null)
                {
                    list.MarkerStyle = textStyle.MarkerStyle.Value;
                }
                else
                {
                    EditingCommands.ToggleBullets.Execute(null, rtb);
                }
            }
        }
        else if ((textStyle.MarkerStyle == TextMarkerStyle.None && block is List decimalList && decimalList.MarkerStyle == TextMarkerStyle.Decimal)
            || (textStyle.MarkerStyle == TextMarkerStyle.Decimal && (block is Paragraph || block is List nonDecimalList && nonDecimalList.MarkerStyle != TextMarkerStyle.Decimal)))
        {
            List list = block as List;
            if (isApply || !textStyle.IsModifyProperty(List.MarkerStyleProperty.Name, list?.MarkerStyle ?? TextMarkerStyle.None))
            {
                if (list != null)
                {
                    list.MarkerStyle = textStyle.MarkerStyle.Value;
                }
                else
                {
                    EditingCommands.ToggleNumbering.Execute(null, block);
                }
            }
        }
    }

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

    public static void SetTextStyleOnElement(FrameworkContentElement element, ITextStyle textStyle, bool isApply, bool isSelected)
    {
        List list = global::Quire.Wpf.WpfUtils.FindLogicalAncestor<List>(element);
        if (element is Run run && run.Parent is Paragraph && list != null)
        {
            list = null;
        }
        if (textStyle.FontFamily != null && (isApply || !textStyle.IsModifyProperty(TextElement.FontFamilyProperty.Name, element.GetValue(TextElement.FontFamilyProperty))))
        {
            element.SetValue(TextElement.FontFamilyProperty, textStyle.FontFamily);
            list?.Parent.SetValue(TextElement.FontFamilyProperty, textStyle.FontFamily);
        }
        if (textStyle.FontSize.HasValue)
        {
            double value = ((element is global::Quire.Wpf.ICustomFontSizeOwner customFontSizeOwner) ? customFontSizeOwner.FontSizeForReport : (double)element.GetValue(TextElement.FontSizeProperty));
            if (isApply || !textStyle.IsModifyProperty(TextElement.FontSizeProperty.Name, value, 0.0001))
            {
                element.SetValue(TextElement.FontSizeProperty, textStyle.FontSize);
                list?.Parent.SetValue(TextElement.FontSizeProperty, textStyle.FontSize);
            }
        }
        else if (textStyle.IsDeletedProperty(TextElement.FontSizeProperty.Name, (double)element.GetValue(TextElement.FontSizeProperty)))
        {
            element.SetValue(TextElement.FontSizeProperty, global::Quire.Wpf.WpfUtils.PointsToDips(11.0));
            list?.Parent.SetValue(TextElement.FontSizeProperty, global::Quire.Wpf.WpfUtils.PointsToDips(11.0));
        }
        if (textStyle.FontWeight.HasValue)
        {
            if (isApply || !textStyle.IsModifyProperty(TextElement.FontWeightProperty.Name, element.GetValue(TextElement.FontWeightProperty)))
            {
                element.SetValue(TextElement.FontWeightProperty, textStyle.FontWeight);
                list?.Parent.SetValue(TextElement.FontWeightProperty, textStyle.FontWeight);
            }
        }
        else if (textStyle.IsDeletedProperty(TextElement.FontWeightProperty.Name, element.GetValue(TextElement.FontWeightProperty)))
        {
            element.SetValue(TextElement.FontWeightProperty, FontWeights.Normal);
            list?.Parent.SetValue(TextElement.FontWeightProperty, FontWeights.Normal);
        }
        if (textStyle.FontStyle.HasValue)
        {
            if (isApply || !textStyle.IsModifyProperty(TextElement.FontStyleProperty.Name, element.GetValue(TextElement.FontStyleProperty)))
            {
                element.SetValue(TextElement.FontStyleProperty, textStyle.FontStyle);
                list?.Parent.SetValue(TextElement.FontStyleProperty, textStyle.FontStyle);
            }
        }
        else if (textStyle.IsDeletedProperty(TextElement.FontStyleProperty.Name, element.GetValue(TextElement.FontStyleProperty)))
        {
            element.SetValue(TextElement.FontStyleProperty, FontStyles.Normal);
            list?.Parent.SetValue(TextElement.FontStyleProperty, FontStyles.Normal);
        }
        if (textStyle.Background != null)
        {
            if (isApply || !textStyle.IsModifyPropertyString(TextElement.BackgroundProperty.Name, element.GetValue(TextElement.BackgroundProperty)))
            {
                element.SetValue(TextElement.BackgroundProperty, textStyle.Background);
                list?.Parent.SetValue(TextElement.BackgroundProperty, textStyle.Background);
            }
        }
        else if (textStyle.IsDeletedProperty(TextElement.BackgroundProperty.Name, element.GetValue(TextElement.BackgroundProperty)))
        {
            element.SetValue(TextElement.BackgroundProperty, null);
            list?.Parent.SetValue(TextElement.BackgroundProperty, null);
        }
        if (textStyle.Foreground != null && !(element.Parent is Hyperlink))
        {
            if (isApply || !textStyle.IsModifyPropertyString(TextElement.ForegroundProperty.Name, element.GetValue(TextElement.ForegroundProperty)))
            {
                element.SetValue(TextElement.ForegroundProperty, textStyle.Foreground);
                list?.Parent.SetValue(TextElement.ForegroundProperty, textStyle.Foreground);
            }
        }
        else if (textStyle.IsDeletedProperty(TextElement.ForegroundProperty.Name, element.GetValue(TextElement.ForegroundProperty)) && !isApply && !(element.Parent is Hyperlink))
        {
            element.SetValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
            list?.Parent.SetValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
        }
        if (textStyle.FontVariants.HasValue)
        {
            element.SetValue(Typography.VariantsProperty, textStyle.FontVariants);
        }
        if (textStyle.TextDecorations != null && ((textStyle.TextDecorations.Count >= 0) | isApply))
        {
            if (isApply || !textStyle.IsModifyPropertyCollection(Inline.TextDecorationsProperty.Name, (TextDecorationCollection)element.GetValue(Inline.TextDecorationsProperty)))
            {
                element.SetValue(Inline.TextDecorationsProperty, textStyle.TextDecorations);
            }
        }
        else if (textStyle.IsDeletedProperty(Inline.TextDecorationsProperty.Name, element.GetValue(Inline.TextDecorationsProperty)))
        {
            element.SetValue(Inline.TextDecorationsProperty, null);
            list?.Parent.SetValue(Inline.TextDecorationsProperty, null);
        }
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
