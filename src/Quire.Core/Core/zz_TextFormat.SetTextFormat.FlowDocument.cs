#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Quire.Core;

public partial class TextFormat
{
    public static void SetTextFormat(DependencyProperty property, object value, FlowDocument document)
    {
        if (property == FontFamilyProperty)
        {
            document.FontFamily = (FontFamily)value;
            return;
        }
        if (property == FontSizeProperty)
        {
            document.FontSize = (double)value;
            return;
        }
        if (property == FontWeightProperty)
        {
            document.FontWeight = (FontWeight)value;
            return;
        }
        if (property == FontStyleProperty)
        {
            document.FontStyle = (FontStyle)value;
            return;
        }
        if (property == TextDecorationsProperty)
        {
            ((DependencyObject)document).SetValue(TextDecorationsProperty, value);
            foreach (Block block in document.Blocks)
            {
                if (!(block is Paragraph paragraph))
                {
                    continue;
                }
                foreach (Inline inline in paragraph.Inlines)
                {
                    TextDecorationCollection textDecorationCollection = (TextDecorationCollection)((DependencyObject)inline).GetValue(TextDecorationsProperty);
                    int? num = ((TextDecorationCollection)value)?.Count;
                    if (value != null && textDecorationCollection != null && num > 0 && num != textDecorationCollection.Count)
                    {
                        ((DependencyObject)inline).SetValue(TextDecorationsProperty, value);
                    }
                }
            }
            // Keep the protected foreach's leave target at the shared method epilogue, as in Mathcad Release IL.
            goto done;
        }
        if (property == VariantsProperty)
        {
            ((DependencyObject)document).SetValue(VariantsProperty, value);
        }
        else if (property == BackgroundProperty)
        {
            ((DependencyObject)document).SetValue(BackgroundProperty, value);
        }
        else if (property == ForegroundProperty)
        {
            ((DependencyObject)document).SetValue(ForegroundProperty, value);
        }
        else if (property == MarginProperty)
        {
            ((DependencyObject)document).SetValue(MarginProperty, value);
        }
        else if (property == TextAlignmentProperty)
        {
            ((DependencyObject)document).SetValue(TextAlignmentProperty, value);
        }
        else if (property == MarkerStyleProperty)
        {
            ((DependencyObject)document).SetValue(List.MarkerStyleProperty, value);
        }
        else if (property == FrameworkElement.StyleProperty)
        {
            document.Style = (Style)value;
        }
        else if (property == StyleKeyProperty)
        {
            ((DependencyObject)document).SetValue(StyleKeyProperty, value);
        }
    done:;
    }
}
