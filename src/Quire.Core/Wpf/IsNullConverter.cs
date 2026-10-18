#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class IsNullConverter
{
    [MirrorVerified("PtcWpfIsNullConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException("IsNullConverter can only be used OneWay.");

    [MirrorVerified("PtcWpfIsNullConverterMechanicalDifferentialTests")]
    public IsNullConverter() { }
}
