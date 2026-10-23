#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class BooleanInverseConverter
{
    [MirrorVerified("Release IL")]
    public static BooleanInverseConverter Instance = new BooleanInverseConverter();

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is bool))
        {
            return false;
        }
        return !(bool)value;
    }

    [MirrorVerified("PtcWpfBooleanInverseConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value, targetType, parameter, culture);

    [MirrorVerified("PtcWpfBooleanInverseConverterMechanicalDifferentialTests")]
    public BooleanInverseConverter() { }
}
