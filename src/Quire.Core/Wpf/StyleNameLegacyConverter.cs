#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class StyleNameLegacyConverter
{
    [MirrorVerified("PtcWpfStyleNameLegacyConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    [MirrorVerified("PtcWpfStyleNameLegacyConverterMechanicalDifferentialTests")]
    public StyleNameLegacyConverter() { }
}
