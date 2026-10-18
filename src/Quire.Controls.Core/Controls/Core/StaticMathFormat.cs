#nullable disable
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class StaticMathFormat
{
    [MirrorVerified("PtcControlsCoreStaticMathFormatMechanicalDifferentialTests")]
    public string FontFamilyName { get; set; }

    [MirrorVerified("PtcControlsCoreStaticMathFormatMechanicalDifferentialTests")]
    public Nullable<double> FontSize { get; set; }

    [MirrorVerified("PtcControlsCoreStaticMathFormatMechanicalDifferentialTests")]
    public Nullable<bool> IsBold { get; set; }

    [MirrorVerified("PtcControlsCoreStaticMathFormatMechanicalDifferentialTests")]
    public Nullable<bool> IsItalic { get; set; }

    [MirrorVerified("PtcControlsCoreStaticMathFormatMechanicalDifferentialTests")]
    public Nullable<bool> IsUnderline { get; set; }

    [MirrorVerified("PtcControlsCoreStaticMathFormatMechanicalDifferentialTests")]
    public Nullable<Color> FontColor { get; set; }

    [MirrorVerified("PtcControlsCoreStaticMathFormatMechanicalDifferentialTests")]
    public Nullable<Color> HighlightColor { get; set; }
}
