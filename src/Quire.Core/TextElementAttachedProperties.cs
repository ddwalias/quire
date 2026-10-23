#nullable disable
using System.Windows;
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
}
