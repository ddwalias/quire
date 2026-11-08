#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Quire.Controls.Plots;

public partial class MultiGridLengthsToMargin
{
    private const double ConstMargin = 10.0;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length == 0)
        {
            return default(Thickness);
        }
        double num = 0.0;
        foreach (object obj in values)
        {
            if (obj is GridLength gridLength)
            {
                num += gridLength.Value;
            }
            else if (obj is double num2)
            {
                num += num2;
            }
        }
        // Mathcad returns from each case directly, so the IL has no shared result local.
        string text = (parameter ?? "").ToString().ToUpperInvariant();
        switch (text)
        {
            case "LEFT":
                return new Thickness(10.0 - num, 0.0, 0.0, 0.0);
            case "RIGHT":
                return new Thickness(0.0, 0.0, 10.0 - num, 0.0);
            case "UP":
                return new Thickness(0.0, 10.0 - num, 0.0, 0.0);
            case "DOWN":
                return new Thickness(0.0, 0.0, 0.0, 10.0 - num);
            default:
                return default(Thickness);
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
