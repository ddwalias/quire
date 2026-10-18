#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class BooleanInverseConverter
{
    [MirrorVerified("PtcWpfBooleanInverseConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value, targetType, parameter, culture);

    [MirrorVerified("PtcWpfBooleanInverseConverterMechanicalDifferentialTests")]
    public BooleanInverseConverter() { }
}
