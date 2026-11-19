#nullable disable

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Quire;

public partial class TextStylePropertyToVisibility
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Style style && (Setter)style.Setters.FirstOrDefault((SetterBase prop) => ((Setter)prop).Property.Name == parameter.ToString()) != null)
        {
            return Visibility.Visible;
        }

        return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public TextStylePropertyToVisibility() { }
}
