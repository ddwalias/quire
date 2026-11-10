#nullable disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Quire;

internal partial class CalculationStatusConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 2)
        {
            return Brushes.Red;
        }

        bool isCalculating = values[0] != DependencyProperty.UnsetValue && (bool)values[0];
        bool isCalculated = values[1] != DependencyProperty.UnsetValue && (bool)values[1];
        if (isCalculating)
        {
            return Brushes.Orange;
        }

        if (!isCalculated)
        {
            return Brushes.Gray;
        }

        return Brushes.Green;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
