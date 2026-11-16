using System;
using System.Globalization;
using System.Windows.Data;

namespace Quire.Controls.Worksheet;

public partial class PageNumberConverter : IMultiValueConverter
{
    private string _fallbackValue = string.Empty;

    public string FallbackValue
    {
        get => _fallbackValue;
        set => _fallbackValue = value;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (object.Equals(values[1], 0))
        {
            return FallbackValue;
        }
        string format = (string)values[0];
        int num = (int)values[1];
        int num2 = (int)values[2];
        if (values.Length == 5)
        {
            int num3 = (int)values[3] - 1;
            int num4 = (int)values[4];
            num += num3;
            num2 += num3 + num4;
        }
        return string.Format(format, num, num2);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("It is not possible to convert the formatted page number string back into the format, current page number and total page numbers");
    }
}
