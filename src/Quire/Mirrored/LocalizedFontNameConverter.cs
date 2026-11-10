#nullable disable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Quire;

public partial class LocalizedFontNameConverter
{
    private readonly Dictionary<string, FontDataObject> _collectedFontsDict = new Dictionary<string, FontDataObject>();

    private readonly Dictionary<string, FontDataObject> _missingFontsDict = new Dictionary<string, FontDataObject>();

    public static LocalizedFontNameConverter Instance => _instance ?? (_instance = new LocalizedFontNameConverter());

    public IEnumerable<FontDataObject> FontNames => _collectedFonts;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is FontDataObject fontDataObject))
        {
            return DependencyProperty.UnsetValue;
        }

        if (targetType == typeof(FontFamily))
        {
            return fontDataObject.FontFamily;
        }

        return fontDataObject.FontFamily.Source;
    }

    public static bool IsUniMathFontExist()
    {
        return Instance.FontNames.Any(UniMathFontExist);

        static bool UniMathFontExist(FontDataObject fontDataObject)
        {
            return fontDataObject.LocalizedName == "Mathcad UniMath";
        }
    }

    public static bool DoesFontFamilyExist(FontFamily fontFamily)
    {
        return Instance.FontNames.Any(FontExists);

        bool FontExists(FontDataObject fontDataObject)
        {
            return fontDataObject.FontFamily.Source == fontFamily.Source;
        }
    }
}
