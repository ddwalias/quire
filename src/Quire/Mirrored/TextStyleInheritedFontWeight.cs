using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Quire;

public partial class TextStyleInheritedFontWeight
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (((TextStylePropertyInfoExt)value).IsInherited)
        {
            return FontWeights.Normal;
        }

        return FontWeights.Bold;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    public TextStyleInheritedFontWeight() { }
}
