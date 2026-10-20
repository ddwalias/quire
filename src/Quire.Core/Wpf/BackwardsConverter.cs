#nullable disable
using System;
using System.Globalization;
using System.Windows.Data;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class BackwardsConverter
{
    [MirrorVerified("PtcWpfBackwardsConverterMechanicalDifferentialTests")]
    public BackwardsConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(parameter is IValueConverter valueConverter))
        {
            return null;
        }
        return valueConverter.ConvertBack(value, targetType, null, culture);
    }

    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(parameter is IValueConverter valueConverter))
        {
            return null;
        }
        return valueConverter.Convert(value, targetType, null, culture);
    }
}
