#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class DoubleToStringWithoutNotationConverter
{
    [MirrorVerified("PtcWpfDoubleToStringWithoutNotationConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfDoubleToStringWithoutNotationConverterMechanicalDifferentialTests")]
    public DoubleToStringWithoutNotationConverter() { }
}
