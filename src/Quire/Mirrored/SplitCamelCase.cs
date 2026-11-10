#nullable disable

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Quire;

public partial class SplitCamelCase
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return string.Join(" ", Regex.Split((string)value, "(?<!^)(?=[A-Z])"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
