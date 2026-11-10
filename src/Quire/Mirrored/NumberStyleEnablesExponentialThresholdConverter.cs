#nullable disable

using System;
using System.Globalization;
using Quire.Controls.Core;

namespace Quire;

public partial class NumberStyleEnablesExponentialThresholdConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null || value.ToString() == NumberFormat.NumberStyles.General.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
