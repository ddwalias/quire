#nullable disable
using System;
using System.Windows.Media;
using Quire.Mirror;
using Quire.Services;

namespace Quire.Controls.Core;

public partial class FontConstants
{
    public const string DefaultMathFontFamilyName = "Mathcad UniMath Prime";

    public const string DefaultConverterMathFontFamilyName = "Mathcad UniMath";

    public static readonly string EmbedFontsLibraryPath = "Ptc.Controls.Core;component/Fonts/";

    public static readonly string EmbedFontDelimiter = "#";

    public static readonly string EmbedFontFullPath = EmbedFontsLibraryPath + EmbedFontDelimiter;

    public static readonly double DefaultMathFontSizeInPoints = 11.0;

    public static readonly SolidColorBrush DefaultMathFontColor = Brushes.Black;

    public static readonly SolidColorBrush DefaultNonEditableMathFontColor = Brushes.DimGray;

    public static readonly double ScriptFontSizeRatio = 7.0 / DefaultMathFontSizeInPoints;

    public static readonly double SubscriptIDFontSizeRatio = 0.7;

    public static readonly double MinimumFontSizeInPoints = 4.0;

    public static readonly FontFamily DefaultMathFontFamily = new FontFamily(new Uri("pack://application:,,,"), EmbedFontFullPath + "Mathcad UniMath Prime");

    public static readonly FontFamily DefaultConverterMathFontFamily = new FontFamily(new Uri("pack://application:,,,"), EmbedFontFullPath + "Mathcad UniMath");

    public static readonly StaticMathFormat DefaultMathFormat = new StaticMathFormat
    {
        FontFamilyName = "Mathcad UniMath Prime",
        FontSize = DefaultMathFontSizeInPoints,
        IsBold = false,
        IsItalic = false,
        IsUnderline = false,
        FontColor = DefaultMathFontColor.Color,
        HighlightColor = Colors.Transparent
    };

    public static ExtendedFont GetDefaultFontForMath()
    {
        return AppServiceRegistry.GetService<IFontManager>().GetFont("Mathcad UniMath Prime", DefaultMathFormat.FontSize.GetValueOrDefault(), DefaultMathFormat.IsItalic == true, DefaultMathFormat.IsBold == true, DefaultMathFormat.IsUnderline == true, Colors.Black);
    }

    public static ExtendedFont GetDefaultIdLabelFormat(IdLabels? label)
    {
        string fontFamilyName = "Mathcad UniMath Prime";
        double defaultMathFontSizeInPoints = DefaultMathFontSizeInPoints;
        bool isBold = false;
        bool isItalic = true;
        bool isUnderline = false;
        Color value = Colors.Black;
        switch (label)
        {
        case IdLabels.Variable:
            break;
        case IdLabels.Unit:
            isBold = true;
            value = Colors.DarkBlue;
            break;
        case IdLabels.Constant:
            isBold = true;
            value = Colors.DarkOliveGreen;
            break;
        case IdLabels.Function:
            isItalic = false;
            break;
        case IdLabels.System:
        case IdLabels.Keyword:
            isItalic = false;
            isBold = true;
            break;
        }
        return AppServiceRegistry.GetService<IFontManager>().GetFont(fontFamilyName, defaultMathFontSizeInPoints, isItalic, isBold, isUnderline, value);
    }

    [MirrorVerified("PtcControlsCoreFontConstantsMechanicalDifferentialTests")]
    public FontConstants() { }
}
