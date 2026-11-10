#nullable disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Quire;

public partial class IsCalcNowEnabledConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
        {
            return false;
        }

        bool isCalculating = (bool)values[0];
        bool isCalculated = (bool)values[1];
        return !isCalculating && !isCalculated;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
