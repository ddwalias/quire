#nullable disable
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ExtendedFont
{
    private static object CoerceFontFamily(DependencyObject d, object basevalue)
    {
        var extendedFont = d as ExtendedFont;
        var fontFamily = basevalue as FontFamily;
        if (extendedFont == null || fontFamily == null)
        {
            return basevalue;
        }

        var typeface = new Typeface(fontFamily, extendedFont.FontStyle, extendedFont.FontWeight, extendedFont.FontStretch);
        typeface.TryGetGlyphTypeface(out GlyphTypeface glyphTypeface);
        if (glyphTypeface == null)
        {
            return FontConstants.DefaultMathFontFamily;
        }
        return basevalue;
    }

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

    public ExtendedFont Clone()
    {
        Brush brush = Foreground;
        if (DebugTools.DevAssert(brush.IsFrozen, null).Failed)
        {
            brush = brush.Clone();
        }
        return new ExtendedFont(FontFamily, FontSize, FontStyle, FontWeight, FontStretch, TextDecorations, brush);
    }

    public ExtendedFont(FontFamily fontFamily, double fontSize, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, TextDecorationCollection textDecorations, Brush foreground)
    {
        FontFamily = fontFamily;
        FontSize = fontSize;
        FontStyle = fontStyle;
        FontWeight = fontWeight;
        FontStretch = fontStretch;
        TextDecorations = textDecorations;
        if (foreground is SolidColorBrush solidColorBrush && solidColorBrush.Color == Colors.Black)
        {
            Foreground = Brushes.Black;
        }
        else
        {
            Foreground = foreground;
        }
        CalculateTypeface();
    }

    public ExtendedFont(Typeface typeface, double fontSize)
        : this(typeface.FontFamily, fontSize, typeface.Style, typeface.Weight, typeface.Stretch, new TextDecorationCollection(), Brushes.Black)
    {
    }

    private void CalculateTypeface()
    {
        WritePreamble();
        _fontTypeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        if (!FontTypeface.TryGetGlyphTypeface(out _glyphTypeface))
        {
            _glyphTypeface = new GlyphTypeface(new Uri("pack://application:,,,/Quire.Controls.Core;component/Fonts/mathcadunimathprime.otf"));
        }
        _M_Run = GenerateGlyphRun("M");
        _n_Run = GenerateGlyphRun("n");
        _j_Run = GenerateGlyphRun("j");
        GlyphRun glyphRun = GenerateGlyphRun('-'.ToString());
        _baselineAdjustment = SafeComputeInkBoundingBox(glyphRun).Bottom;
        WritePostscript();
    }

    public static GlyphRun GenerateGlyphRun(string text, Uri fontUri, double fontSize)
    {
        if (!ControlSettings.Instance.EnableFontCaching)
        {
            return CreateGlyphRun(text, fontUri, fontSize);
        }
        string key = string.Format("{0}{1}{2}", text, fontUri, fontSize);
        if (!GlyphRunCache.TryGetValue(key, out var value))
        {
            value = CreateGlyphRun(text, fontUri, fontSize);
            if (value != null)
            {
                GlyphRunCache.Add(key, value);
            }
        }
        return value;
    }

    private GlyphRun GenerateGlyphRun(string text)
    {
        return GenerateGlyphRun(text, GlyphTypeface.FontUri, FontSize);
    }
}
