#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire;

public partial class TextStyleBase
{
    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public double Weight { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public string Name { get; set; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public string StyleKey { get; set; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public Style Style { get; private set; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected string _baseOnKey;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public string BasedOnKey { get => _baseOnKey; set => _baseOnKey = value; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public double Stretch { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected FontFamily _fontFamily;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public FontFamily FontFamily { get => _fontFamily; set => Update("FontFamily", value); }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public bool Bold { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public bool Italic { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public bool Normal { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public bool Oblique { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected Brush _background;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public Brush Background { get => _background; set => Update("Background", value); }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected Brush _foreground;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public Brush Foreground { get => _foreground; set => Update("Foreground", value); }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected TextDecorationCollection _textDecorationCollection;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public TextDecorationCollection TextDecorations { get => _textDecorationCollection; set => Update("TextDecorations", value); }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected TextEffectCollection _textEffectCollection;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public TextEffectCollection TextEffects => _textEffectCollection;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public bool Condenst { get; }
}
