using System;
using System.Globalization;
using System.Windows.Data;
using Quire.Core;

namespace Quire.Controls.Worksheet;

public partial class FileNameConverter : IMultiValueConverter
{
    public static readonly FileNameConverter Instance = new FileNameConverter();

    private FileNameConverter()
    {
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (!((string)values[0]).IsNotEmpty())
        {
            return values[1];
        }
        return values[0];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("it is supposed to be one way binding");
    }
}
