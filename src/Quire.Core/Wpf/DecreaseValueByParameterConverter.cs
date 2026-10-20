#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class DecreaseValueByParameterConverter
{
    [MirrorVerified("PtcWpfDecreaseValueByParameterConverterMechanicalDifferentialTests")]
    public DecreaseValueByParameterConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter != null && double.TryParse(parameter.ToString(), out var result))
        {
            return (double)value - result;
        }
        return value;
    }

    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter != null && double.TryParse(parameter.ToString(), out var result))
        {
            return (double)value + result;
        }
        return value;
    }
}
