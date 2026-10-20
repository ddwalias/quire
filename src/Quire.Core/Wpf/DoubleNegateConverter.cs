#nullable disable
using System.Globalization;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class DoubleNegateConverter
{
    [MirrorVerified("PtcWpfDoubleNegateConverterMechanicalDifferentialTests")]
    public DoubleNegateConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => -(double)value;

    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => -(double)value;
}
