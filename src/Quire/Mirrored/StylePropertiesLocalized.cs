#nullable disable

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;

namespace Quire;

public partial class StylePropertiesLocalized
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is ResourceDictionary resourceDictionary && resourceDictionary.Contains(value))
        {
            return resourceDictionary[value];
        }

        return string.Join(" ", Regex.Split((string)value, "(?<!^)(?=[A-Z])"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
