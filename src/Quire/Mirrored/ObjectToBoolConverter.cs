#nullable disable

using System;
using System.Globalization;
using Quire.Controls;

namespace Quire;

internal partial class ObjectToBoolConverter
{
    public static readonly ObjectToBoolConverter Instance = new ObjectToBoolConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return values.Length == 1 && values[0] != null;
        }

        if (values[1] is EquationControl equationControl && equationControl.SelectedOperator.Equals("@ComboBoxControl"))
        {
            return false;
        }

        return values[0] != null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
