#nullable disable

using System;
using System.Globalization;

namespace Quire;

internal partial class MultiThreadingValueToBooleanConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert(value);
    }

    internal static bool Convert(object numberOfThreads)
    {
        return !object.Equals(numberOfThreads, 1);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool)
        {
            return (bool)value ? global::Quire.Properties.Settings.Default.MaxNumberOfThreads : 1;
        }

        return 1;
    }
}
