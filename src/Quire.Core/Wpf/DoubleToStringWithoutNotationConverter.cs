#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class DoubleToStringWithoutNotationConverter
{
    [MirrorVerified("PtcWpfDoubleToStringWithoutNotationConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfDoubleToStringWithoutNotationConverterMechanicalDifferentialTests")]
    public DoubleToStringWithoutNotationConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double num = (double)value;
        if (num == 0.0)
        {
            return 0;
        }
        if (-ScientificNotationThreshold < num && num < ScientificNotationThreshold)
        {
            return num.ToString("0.###############", CultureInfo.InvariantCulture);
        }
        return num.ToString(CultureInfo.InvariantCulture);
    }
}
