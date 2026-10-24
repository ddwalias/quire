#nullable disable
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Core;

// Keep after generated members and these two trailing members to preserve Mathcad's closure ordinal.
public partial class TextFormat
{
    public static void ApplyPropertyValueFormat(DependencyProperty property, object value, System.Windows.Documents.TextRange textRange)
    {
        if (value == null)
        {
            return;
        }
        if (property == FontFamilyProperty && !value.Equals(textRange.GetPropertyValue(FontFamilyProperty)))
        {
            textRange.ApplyPropertyValue(FontFamilyProperty, (FontFamily)value);
        }
        else if (property == FontSizeProperty && !value.Equals(textRange.GetPropertyValue(FontSizeProperty)))
        {
            double fontSize = (double)value;
            SynchFontSizeOnScriptRunsWrapperCmd command = SynchFontSizeOnScriptRunsWrapperCmd.CreateSynchFontSizeOnScriptRunsWrapperCmd(
                textRange,
                fontSize,
                () => textRange.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize),
                "Set FontSize on text");
            if (command != null)
            {
                global::Quire.Undo.CommandBuffer.Execute(command);
            }
        }
        else if (property == FontWeightProperty && !value.Equals(textRange.GetPropertyValue(FontWeightProperty)))
        {
            textRange.ApplyPropertyValue(FontWeightProperty, (FontWeight)value);
        }
        else if (property == FontStyleProperty && !value.Equals(textRange.GetPropertyValue(FontStyleProperty)))
        {
            textRange.ApplyPropertyValue(FontStyleProperty, (FontStyle)value);
        }
        else if (property == TextDecorationsProperty && !value.Equals(textRange.GetPropertyValue(TextDecorationsProperty)))
        {
            textRange.ApplyPropertyValue(TextDecorationsProperty, value);
        }
        else if (property == VariantsProperty && !value.Equals(textRange.GetPropertyValue(VariantsProperty)))
        {
            textRange.ApplyPropertyValue(VariantsProperty, value);
        }
        else if (property == BackgroundProperty && !value.Equals(textRange.GetPropertyValue(BackgroundProperty)))
        {
            textRange.ApplyPropertyValue(BackgroundProperty, (Brush)value);
        }
        else if (property == ForegroundProperty && !value.Equals(textRange.GetPropertyValue(ForegroundProperty)))
        {
            textRange.ApplyPropertyValue(ForegroundProperty, (Brush)value);
        }
        else if (property == TextAlignmentProperty && !value.Equals(textRange.GetPropertyValue(TextAlignmentProperty)))
        {
            textRange.ApplyPropertyValue(TextAlignmentProperty, (TextAlignment?)value);
        }
    }

    [MirrorVerified("Release IL")]
    public Brush Background { get => (Brush)base.GetValue(BackgroundProperty); set => SetValueSafely(BackgroundProperty, value); }

    [MirrorVerified("Release IL")]
    public void ApplyAllTextFormatValuesToControl(ITextFormatCreation control, Block block = null)
    {
        control.ApplyTextFormatPropertyValueToControl(FontFamilyProperty, FontFamily, block);
        control.ApplyTextFormatPropertyValueToControl(FontSizeProperty, FontSize, block);
        control.ApplyTextFormatPropertyValueToControl(FontWeightProperty, FontWeight, block);
        control.ApplyTextFormatPropertyValueToControl(FontStyleProperty, FontStyle, block);
        control.ApplyTextFormatPropertyValueToControl(TextDecorationsProperty, TextDecorations, block);
        control.ApplyTextFormatPropertyValueToControl(VariantsProperty, Variants);
        control.ApplyTextFormatPropertyValueToControl(BackgroundProperty, Background, block);
        control.ApplyTextFormatPropertyValueToControl(ForegroundProperty, Foreground, block);
        control.ApplyTextFormatPropertyValueToControl(TextAlignmentProperty, TextAlignment);
        control.ApplyTextFormatPropertyValueToControl(MarkerStyleProperty, MarkerStyle);
        control.ApplyTextFormatPropertyValueToControl(MarginProperty, Margin);
        if (control is ICurrentTextFormatProperty)
        {
            ((ICurrentTextFormatProperty)control).CurrentTextFormat = Clone();
        }
    }
}
