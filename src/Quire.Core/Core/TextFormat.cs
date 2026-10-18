#nullable disable
using System.ComponentModel;
using System.Windows;
using Quire.Mirror;

namespace Quire.Core;

public partial class TextFormat
{
    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public TextFormat Clone() => new TextFormat(this);

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public TextStyleData TextStyleData { get; private set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public string BasedOnKey { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedFontFamily { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedFontWeight { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedFontStyle { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedTextDecorations { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedForeground { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedBackground { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedMarkerStyle { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedStyle { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public bool MixedTextAlignment { get; set; }

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public event Delegate1<DependencyPropertyChangedEventArgs> FormatChanged;

    [MirrorVerified("PtcCoreTextFormatMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;
}
