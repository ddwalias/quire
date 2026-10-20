#nullable disable
using System;
using System.Globalization;
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class NumberToStringConverter
{
    [MirrorVerified("Release IL")]
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double num)
        {
            if (!(parameter is string text))
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
            return num.ToString(text, CultureInfo.InvariantCulture);
        }
        return DependencyProperty.UnsetValue;
    }

    [MirrorVerified("Release IL")]
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            return double.Parse(value as string, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    [MirrorVerified("PtcWpfNumberToStringConverterMechanicalDifferentialTests")]
    public NumberToStringConverter() { }
}
