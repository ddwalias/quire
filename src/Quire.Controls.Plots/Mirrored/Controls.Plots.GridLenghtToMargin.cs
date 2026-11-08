#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Quire.Controls.Plots;

public partial class GridLenghtToMargin
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is GridLength))
        {
            return null;
        }
        string text = parameter.ToString().ToUpper();
        GridLength gridLength = (GridLength)value;
        // Mathcad returns from each case directly; a switch expression joins the arms through a shared local.
        switch (text)
        {
            case "UP":
                return new Thickness(0.0, 10.0 - gridLength.Value, 0.0, 0.0);
            case "DOWN":
                return new Thickness(0.0, 0.0, 0.0, 10.0 - gridLength.Value);
            case "LEFT":
                return new Thickness(10.0 - gridLength.Value, 0.0, 0.0, 0.0);
            case "RIGHT":
                return new Thickness(0.0, 0.0, 10.0 - gridLength.Value, 0.0);
            default:
                return default(Thickness);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
