#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Quire.Core;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class FontManager
{
    partial class FontKey
    {
        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public FontKey(string fontFamilyName, double fontPointSize, bool isItalic, bool isBold, bool isUnderline, Color fontColor) : base(fontFamilyName, fontPointSize, isItalic, isBold, isUnderline, fontColor) { }

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public string FontFamilyName => base.Item1;

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public double FontPointSize => base.Item2;

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public bool IsItalic => base.Item3;

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public bool IsBold => base.Item4;

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public bool IsUnderline => base.Item5;

        [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
        public Color FontColor => base.Item6;
    }

    private const int MaxCacheSize = 100;

    private TextDecorationCollection _emptyTextDecorations = new TextDecorationCollection();

    private TextDecorationCollection _underlineTextDecorations = new TextDecorationCollection(TextDecorations.Underline.AsEnumerable());

    private readonly Dictionary<FontKey, ExtendedFont> _fontCache = new Dictionary<FontKey, ExtendedFont>();

    private readonly Dictionary<FontKey, DateTime> _fontCacheMru = new Dictionary<FontKey, DateTime>();

    public ExtendedFont GetFont(string fontFamilyName, double fontPointSize, bool isItalic, bool isBold, bool isUnderline, Color? fontColor)
    {
        if (fontPointSize < FontConstants.MinimumFontSizeInPoints)
        {
            fontPointSize = FontConstants.MinimumFontSizeInPoints;
        }
        FontKey key = new FontKey(fontFamilyName, fontPointSize, isItalic, isBold, isUnderline, fontColor ?? Colors.Black);
        return GetFont(key);
    }

    public ExtendedFont GetFont(IMathFormat mathFormat)
    {
        return GetFont(mathFormat.FontFamilyName ?? "Mathcad UniMath Prime", mathFormat.FontSize ?? FontConstants.DefaultMathFontSizeInPoints, mathFormat.IsItalic.GetValueOrDefault(), mathFormat.IsBold.GetValueOrDefault(), mathFormat.IsUnderline.GetValueOrDefault(), mathFormat.FontColor ?? Colors.Black);
    }

    public ExtendedFont GetScaledFont(IMathFormat mathFormat, double scaleFactor)
    {
        StaticMathFormat staticMathFormat = new StaticMathFormat(mathFormat);
        staticMathFormat.FontSize = GetScaledFontSize(staticMathFormat, scaleFactor);
        return GetFont(staticMathFormat);
    }

    public double GetScaledFontSize(IMathFormat mathFormat, double scaleFactor)
    {
        double num = mathFormat.FontSize ?? FontConstants.DefaultMathFontSizeInPoints;
        return Math.Max(FontConstants.MinimumFontSizeInPoints, Math.Round(num * scaleFactor, 2, MidpointRounding.AwayFromZero));
    }

    private ExtendedFont GetFont(FontKey key)
    {
        if (_fontCache.TryGetValue(key, out var value))
        {
            _fontCacheMru[key] = DateTime.Now;
            return value;
        }
        FontFamily fontFamily;
        if (key.FontFamilyName == "Mathcad UniMath Prime" || key.FontFamilyName == "Mathcad UniMath")
        {
            fontFamily = new FontFamily(new Uri("pack://application:,,,"), FontConstants.EmbedFontFullPath + key.FontFamilyName);
        }
        else
        {
            fontFamily = new FontFamily(key.FontFamilyName);
        }
        double fontSize = WpfUtils.PointsToDips(key.FontPointSize);
        FontStyle fontStyle = (key.IsItalic ? FontStyles.Italic : FontStyles.Normal);
        FontWeight fontWeight = (key.IsBold ? FontWeights.Bold : FontWeights.Normal);
        TextDecorationCollection textDecorations = (key.IsUnderline ? _underlineTextDecorations : _emptyTextDecorations);
        Brush foreground = new SolidColorBrush(key.FontColor);
        value = new ExtendedFont(fontFamily, fontSize, fontStyle, fontWeight, FontStretches.Normal, textDecorations, foreground);
        AddFontToCache(key, value);
        return value;
    }

    private void AddFontToCache(FontKey key, ExtendedFont font)
    {
        _fontCache[key] = font;
        _fontCacheMru[key] = DateTime.Now;
        if (_fontCache.Count > 100)
        {
            FontKey key2 = _fontCacheMru.MinBy(kv => kv.Value).Key;
            _fontCache.Remove(key2);
            _fontCacheMru.Remove(key2);
        }
    }

    [MirrorVerified("PtcControlsCoreFontManagerMechanicalDifferentialTests")]
    public bool IsConnected { get; set; }

    public FontManager()
    {
    }
}
