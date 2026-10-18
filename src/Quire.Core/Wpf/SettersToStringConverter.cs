#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class SettersToStringConverter
{
    [MirrorVerified("PtcWpfSettersToStringConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    [MirrorVerified("PtcWpfSettersToStringConverterMechanicalDifferentialTests")]
    public SettersToStringConverter() { }
}
