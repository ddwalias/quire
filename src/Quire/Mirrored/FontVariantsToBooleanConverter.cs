#nullable disable

using System;
using System.Globalization;
using System.Windows;

namespace Quire;

public partial class FontVariantsToBooleanConverter
{
    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return null;
        }

        bool isChecked = (bool)value;
        string fontVariantName = parameter.ToString();
        if (isChecked)
        {
            switch (fontVariantName)
            {
                case "Inferior":
                    return FontVariants.Inferior;
                case "Normal":
                    return FontVariants.Normal;
                case "Ordinal":
                    return FontVariants.Ordinal;
                case "Ruby":
                    return FontVariants.Ruby;
                case "Subscript":
                    return FontVariants.Subscript;
                case "Superscript":
                    return FontVariants.Superscript;
            }
        }

        return FontVariants.Normal;
    }
}
