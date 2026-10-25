#nullable disable
using System;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class MathFormatValues
{
    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public MathFormatValues() { }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public string FontFamilyName { get; set; }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public double? FontSize { get; set; }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public bool? IsBold { get; set; }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public bool? IsItalic { get; set; }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public bool? IsUnderline { get; set; }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public Color? FontColor { get; set; }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public Color? HighlightColor { get; set; }

    public void SetPropertiesFrom(IMathFormat mathFormat)
    {
        FontFamilyName = mathFormat.FontFamilyName;
        FontSize = mathFormat.FontSize;
        IsItalic = mathFormat.IsItalic;
        IsBold = mathFormat.IsBold;
        IsUnderline = mathFormat.IsUnderline;
        FontColor = mathFormat.FontColor;
        HighlightColor = mathFormat.HighlightColor;
    }
}
