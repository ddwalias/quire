#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class NumberMatchToBooleanConverter
{
    [MirrorVerified("Release IL")]
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return false;
        }
        double num = NumberOrStringToDouble(value, culture);
        double num2 = NumberOrStringToDouble(parameter, culture);
        return num == num2;
    }

    [MirrorVerified("Release IL")]
    private static double NumberOrStringToDouble(object value, CultureInfo culture)
    {
        if (value is double || value is int || value is float)
        {
            return (double)value;
        }
        if (value is string)
        {
            return double.Parse((string)value, culture);
        }
        return double.NaN;
    }

    [MirrorVerified("PtcWpfNumberMatchToBooleanConverterMechanicalDifferentialTests")]
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfNumberMatchToBooleanConverterMechanicalDifferentialTests")]
    public NumberMatchToBooleanConverter() { }
}
