#nullable disable

using System;
using System.Globalization;
using System.Windows;
using Quire.Controls.Core;

namespace Quire;

internal partial class DipsToDisplayUnitsConverter
{
    public static DipsToDisplayUnitsConverter Instance = new DipsToDisplayUnitsConverter();

    private static readonly int _numOfDigitsForRound = 2;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        string result = string.Empty;
        if (values[0] is double numInDips && values[1] is PageSizeUnits pageSizeUnits)
        {
            double value = Math.Round(DisplayUnitsUtils.DipsToDisplayUnits(numInDips, pageSizeUnits), _numOfDigitsForRound);
            string value2 = pageSizeUnits == PageSizeUnits.Inches ? "\"" : "mm";
            result = $"{value}{value2}";
        }
        else
        {
            DebugTools.DevAssert(values[0] == DependencyProperty.UnsetValue, values[0]?.GetType().Name ?? "");
        }

        return result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
