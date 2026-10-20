#nullable disable
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
}
