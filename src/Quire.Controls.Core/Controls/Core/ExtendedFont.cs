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

    public FontFamily FontFamily
    {
        get => (FontFamily)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public FontWeight FontWeight
    {
        get => (FontWeight)GetValue(FontWeightProperty);
        set => SetValue(FontWeightProperty, value);
    }

    public FontStyle FontStyle
    {
        get => (FontStyle)GetValue(FontStyleProperty);
        set => SetValue(FontStyleProperty, value);
    }

    public TextDecorationCollection TextDecorations
    {
        get => (TextDecorationCollection)GetValue(TextDecorationsProperty);
        set => SetValue(TextDecorationsProperty, value);
    }

    public Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public FontStretch FontStretch
    {
        get => (FontStretch)GetValue(FontStretchProperty);
        set => SetValue(FontStretchProperty, value);
    }

    public Typeface FontTypeface
    {
        get
        {
            ReadPreamble();
            return _fontTypeface;
        }
    }

    public GlyphRun n_Run
    {
        get
        {
            ReadPreamble();
            return _n_Run;
        }
    }

    public GlyphRun j_Run
    {
        get
        {
            ReadPreamble();
            return _j_Run;
        }
    }

    public GlyphTypeface GlyphTypeface
    {
        get
        {
            ReadPreamble();
            return _glyphTypeface;
        }
    }

    public double BaselineAdjustment
    {
        get
        {
            ReadPreamble();
            return _baselineAdjustment;
        }
    }
}
