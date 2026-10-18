#nullable disable
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
    public Nullable<double> FontSize { get; set; }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public Nullable<bool> IsBold { get; set; }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public Nullable<bool> IsItalic { get; set; }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public Nullable<bool> IsUnderline { get; set; }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public Nullable<Color> FontColor { get; set; }

    [MirrorVerified("PtcControlsCoreMathFormatValuesMechanicalDifferentialTests")]
    public Nullable<Color> HighlightColor { get; set; }
}
