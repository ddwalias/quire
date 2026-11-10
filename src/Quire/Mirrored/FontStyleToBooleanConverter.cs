#nullable disable

using System;
using System.Globalization;
using System.Windows;

namespace Quire;

public partial class FontStyleToBooleanConverter
{
    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return null;
        }

        bool isChecked = (bool)value;
        string fontStyleName = parameter.ToString();
        if (isChecked)
        {
            switch (fontStyleName)
            {
                case "Italic":
                    return FontStyles.Italic;
                case "Normal":
                    return FontStyles.Normal;
                case "Oblique":
                    return FontStyles.Oblique;
            }
        }

        return FontStyles.Normal;
    }
}
