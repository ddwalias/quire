#nullable disable
using System.Windows;
using System.Windows.Documents;
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

    public static Rect SafeComputeInkBoundingBox(GlyphRun glyphRun)
    {
        Rect inkBox = glyphRun.ComputeInkBoundingBox();
        if (!inkBox.IsEmpty)
        {
            return inkBox;
        }
        double advanceWidth = 1.0;
        if (glyphRun.AdvanceWidths.Count > 0)
        {
            advanceWidth = glyphRun.AdvanceWidths[0];
        }
        return new Rect(new Point(0.0, 0.0), new Point(advanceWidth, glyphRun.BaselineOrigin.Y));
    }

    private static GlyphRun CreateGlyphRun(string text, Uri fontUri, double fontSize)
    {
        return new Glyphs
        {
            FontRenderingEmSize = fontSize,
            FontUri = fontUri,
            UnicodeString = text
        }.ToGlyphRun();
    }

    private static void HandleExtendedFontPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var extendedFont = d as ExtendedFont;
        if (extendedFont == null)
        {
            return;
        }
        if (e.Property == ForegroundProperty)
        {
            var brush = e.NewValue as Brush;
            if (DebugTools.AssertNotNull(brush, null).Passed && brush.CanFreeze)
            {
                brush.Freeze();
            }
        }
        extendedFont.RevisionNumber++;
        extendedFont.CalculateTypeface();
        if (extendedFont.FormatChanged != null)
        {
            extendedFont.FormatChanged(e);
        }
    }

    public void CopyTo(ExtendedFont target)
    {
        if (target == null)
        {
            return;
        }
        if (!target.FontFamily.Equals(FontFamily))
        {
            target.FontFamily = FontFamily;
        }
        if (!target.FontSize.Equals(FontSize))
        {
            target.FontSize = FontSize;
        }
        if (!target.FontWeight.Equals(FontWeight))
        {
            target.FontWeight = FontWeight;
        }
        if (!target.FontStyle.Equals(FontStyle))
        {
            target.FontStyle = FontStyle;
        }
        if (!target.FontStretch.Equals(FontStretch))
        {
            target.FontStretch = FontStretch;
        }
        if (!((object)target.TextDecorations).Equals(TextDecorations))
        {
            target.TextDecorations = TextDecorations;
        }
        if (!((object)target.Foreground).Equals(Foreground))
        {
            target.Foreground = Foreground;
        }
    }

    object ICloneable.Clone() => Clone();
}
