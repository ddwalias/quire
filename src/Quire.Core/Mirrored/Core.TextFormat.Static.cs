#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Core;

public partial class TextFormat
{
    // Explicit static constructor: Mathcad's .cctor assigns in this order. The four fields it fills are
    // declared in Core/TextFormat.cs after Initialize; this constructor must follow ApplyPropertyValueFormat
    // in compile order so it does not shift that member's closure ordinal.
    static TextFormat()
    {
        ClearFormatPropsList = new DependencyProperty[]
        {
            TextElement.ForegroundProperty,
            TextElement.FontFamilyProperty,
            TextElement.FontWeightProperty,
            TextElement.FontStyleProperty,
            TextElement.FontSizeProperty,
            TextElement.BackgroundProperty,
            Inline.TextDecorationsProperty,
            Block.MarginProperty,
        };
        TextAlignmentExtProperty = DependencyProperty.Register("TextAlignmentExt", typeof(TextAlignmentExt), typeof(TextFormat));
        StyleKeyProperty = DependencyProperty.Register("StyleKey", typeof(string), typeof(TextFormat));
        NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(TextFormat));
        TextFormatProperty = DependencyProperty.RegisterAttached("TextFormat", typeof(TextFormat), typeof(RichTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        MarginProperty = Block.MarginProperty.AddOwner(typeof(TextFormat));
        FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(TextFormat), new FrameworkPropertyMetadata(new PropertyChangedCallback(HandleTextFormatPropertyChanged)));
        FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(TextFormat), new FrameworkPropertyMetadata(new PropertyChangedCallback(HandleTextFormatPropertyChanged)));
        FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(TextFormat), new FrameworkPropertyMetadata(new PropertyChangedCallback(HandleTextFormatPropertyChanged)));
        FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(TextFormat), new FrameworkPropertyMetadata(new PropertyChangedCallback(HandleTextFormatPropertyChanged)));
        TextDecorationsProperty = Inline.TextDecorationsProperty.AddOwner(typeof(TextFormat), new FrameworkPropertyMetadata(new PropertyChangedCallback(HandleTextFormatPropertyChanged)));
        VariantsProperty = Typography.VariantsProperty.AddOwner(typeof(TextFormat), new FrameworkPropertyMetadata(new PropertyChangedCallback(HandleTextFormatPropertyChanged)));
        BackgroundProperty = TextElement.BackgroundProperty.AddOwner(typeof(TextFormat), new FrameworkPropertyMetadata(new PropertyChangedCallback(HandleTextFormatPropertyChanged), new CoerceValueCallback(HandleBrushCoerceValue)));
        ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(TextFormat), new FrameworkPropertyMetadata(new PropertyChangedCallback(HandleTextFormatPropertyChanged), new CoerceValueCallback(HandleBrushCoerceValue)));
        TextAlignmentProperty = Block.TextAlignmentProperty.AddOwner(typeof(TextFormat), new FrameworkPropertyMetadata(new PropertyChangedCallback(HandleTextAlignmentChanged)));
        MarkerStyleProperty = List.MarkerStyleProperty.AddOwner(typeof(TextFormat));
        TextIndentProperty = Paragraph.TextIndentProperty.AddOwner(typeof(TextFormat));
        DefaultTextFormat = new TextFormat();
    }
}
