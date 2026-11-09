using System;
using System.Globalization;
using System.Windows.Data;

namespace Quire.Controls;

public partial class MultiStartAndOptimizeEnabledConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string item = value?.ToString() ?? string.Empty;
        return EquationControlConstants.EnabledAlgorithmsForOptions.Contains(item);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}
