#nullable disable
using System.Windows.Media;

namespace Quire.Controls.Core;

public partial class StaticMathFormat : IMathFormat
{
    public string FontFamilyName { get; set; }

    public double? FontSize { get; set; }

    public bool? IsBold { get; set; }

    public bool? IsItalic { get; set; }

    public bool? IsUnderline { get; set; }

    public Color? FontColor { get; set; }

    public Color? HighlightColor { get; set; }

    public StaticMathFormat()
    {
        FontFamilyName = null;
        FontSize = null;
        IsItalic = null;
        IsBold = null;
        IsUnderline = null;
        FontColor = null;
        HighlightColor = null;
    }

    public StaticMathFormat(IMathFormat other)
    {
        FontFamilyName = other.FontFamilyName;
        FontSize = other.FontSize;
        IsItalic = other.IsItalic;
        IsBold = other.IsBold;
        IsUnderline = other.IsUnderline;
        FontColor = other.FontColor;
        HighlightColor = other.HighlightColor;
    }

    public StaticMathFormat(string fontFamilyName, double? fontSize, bool? isItalic, bool? isBold, bool? isUnderline, Color? fontColor, Color? highlightColor)
    {
        FontFamilyName = fontFamilyName;
        FontSize = fontSize;
        IsItalic = isItalic;
        IsBold = isBold;
        IsUnderline = isUnderline;
        FontColor = fontColor;
        HighlightColor = highlightColor;
    }
}
