#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ExtendedFont
{
    [MirrorVerified("PtcControlsCoreExtendedFontMechanicalDifferentialTests")]
    public ExtendedFont() { }

    [MirrorVerified("PtcControlsCoreExtendedFontMechanicalDifferentialTests")]
    protected override Freezable CreateInstanceCore() => new ExtendedFont();

    [MirrorVerified("PtcControlsCoreExtendedFontMechanicalDifferentialTests")]
    public int RevisionNumber { get; private set; }

    [MirrorVerified("PtcControlsCoreExtendedFontMechanicalDifferentialTests")]
    public event Delegate1<DependencyPropertyChangedEventArgs> FormatChanged;
}
