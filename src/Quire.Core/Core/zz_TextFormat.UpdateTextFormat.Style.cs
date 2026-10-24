#nullable disable
using System.Windows;
using System.Windows.Media;

namespace Quire.Core;

public partial class TextFormat
{
    public void UpdateTextFormat(Style style)
    {
        TextStyle textStyle = new TextStyle(style);
        FontSizeInPoints = 11.0;
        if (textStyle.FontWeight.HasValue)
        {
            FontWeight = textStyle.FontWeight.Value;
        }
        if (textStyle.FontStyle.HasValue)
        {
            FontStyle = textStyle.FontStyle.Value;
        }
        if (textStyle.TextIndent.HasValue)
        {
            // Retain Mathcad's Release conv.r8 after Nullable<double>.get_Value.
            TextIndent = (double)textStyle.TextIndent.Value;
        }
        else
        {
            TextIndent = 0.0;
        }
        if (textStyle.FontFamily != null)
        {
            FontFamily = textStyle.FontFamily;
        }
        if (textStyle.MarkerStyle.HasValue)
        {
            MarkerStyle = textStyle.MarkerStyle.Value;
        }
        if (textStyle.FontSize.HasValue)
        {
            // Retain Mathcad's Release conv.r8 after Nullable<double>.get_Value.
            FontSize = (double)textStyle.FontSize.Value;
        }
        TextDecorations = textStyle.TextDecorations;
        if (textStyle.FontVariants.HasValue)
        {
            Variants = textStyle.FontVariants.Value;
        }
        else
        {
            Variants = FontVariants.Normal;
        }
        if (textStyle.Background != null)
        {
            Background = textStyle.Background;
        }
        if (textStyle.Foreground != null)
        {
            Foreground = textStyle.Foreground;
        }
        else if (Foreground == null)
        {
            Foreground = new SolidColorBrush(Colors.Black);
        }
        if (textStyle.TextAlignment.HasValue)
        {
            TextAlignment = textStyle.TextAlignment.Value;
        }
        if (textStyle.TextAlignmentExt.HasValue)
        {
            TextAlignmentExt = textStyle.TextAlignmentExt.Value;
        }
    }
}
