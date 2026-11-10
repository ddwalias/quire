#nullable disable

using System;
using System.Globalization;
using Quire.Controls.Core;

namespace Quire;

public partial class ComplexNumberStyleToStringConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }

        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch (value as string)
        {
            case "CartesianI":
                return NumberFormat.ComplexStyles.CartesianI;
            case "CartesianJ":
                return NumberFormat.ComplexStyles.CartesianJ;
            case "PolarDegrees":
                return NumberFormat.ComplexStyles.PolarDegrees;
            case "PolarPiRadians":
                return NumberFormat.ComplexStyles.PolarPiRadians;
            case "PolarRadians":
                return NumberFormat.ComplexStyles.PolarRadians;
            default:
                return null;
        }
    }
}
