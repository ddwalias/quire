#nullable disable

using System;
using System.Globalization;

namespace Quire;

internal partial class BooleanToMultiThreadingValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool)
        {
            return Convert((bool)value);
        }

        return 1;
    }

    internal static int Convert(bool checkedState)
    {
        if (!checkedState)
        {
            return 1;
        }

        return global::Quire.Properties.Settings.Default.MaxNumberOfThreads;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
