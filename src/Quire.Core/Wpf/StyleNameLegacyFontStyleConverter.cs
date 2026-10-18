#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class StyleNameLegacyFontStyleConverter
{
    [MirrorVerified("PtcWpfStyleNameLegacyFontStyleConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    [MirrorVerified("PtcWpfStyleNameLegacyFontStyleConverterMechanicalDifferentialTests")]
    public StyleNameLegacyFontStyleConverter() { }
}
