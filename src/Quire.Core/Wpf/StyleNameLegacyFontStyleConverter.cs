#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class StyleNameLegacyFontStyleConverter
{
    [MirrorVerified("Release IL")]
    public static StyleNameLegacyFontStyleConverter Instance = new StyleNameLegacyFontStyleConverter();

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string text = (string)value;
        if (text != null && text.StartsWith("Legacy"))
        {
            return "Italic";
        }
        return "Normal";
    }

    [MirrorVerified("PtcWpfStyleNameLegacyFontStyleConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    [MirrorVerified("PtcWpfStyleNameLegacyFontStyleConverterMechanicalDifferentialTests")]
    public StyleNameLegacyFontStyleConverter() { }
}
