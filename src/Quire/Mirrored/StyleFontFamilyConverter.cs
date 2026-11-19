#nullable disable

using System;
using System.Globalization;
using System.Windows.Media;
using Quire.Controls.Core;

namespace Quire;

public partial class StyleFontFamilyConverter
{
    public StyleFontFamilyConverter() { }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }

        string source = ((FontFamily)value).Source;
        if (source == null)
        {
            return null;
        }

        if (source.Contains(FontConstants.EmbedFontDelimiter))
        {
            source = source.Substring(source.IndexOf(FontConstants.EmbedFontDelimiter, StringComparison.Ordinal) + 1);
        }

        if (source.Equals("Mathcad UniMath") && !LocalizedFontNameConverter.IsUniMathFontExist())
        {
            return "Mathcad UniMath Prime";
        }

        if (LocalizedFontNameConverter.DoesFontFamilyExist((FontFamily)value))
        {
            return source;
        }

        if (source.Equals("Mathcad UniMath") || source.Equals("Mathcad UniMath Prime"))
        {
            return source;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || value.ToString() == string.Empty)
        {
            return null;
        }

        return value;
    }
}
