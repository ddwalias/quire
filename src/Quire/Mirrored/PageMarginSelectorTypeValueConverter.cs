#nullable disable

using System;
using System.Globalization;
using System.Windows;
using Quire.Controls.Core;

namespace Quire;

public partial class PageMarginSelectorTypeValueConverter
{
    public static PageMarginSelectorTypeValueConverter Instance = new PageMarginSelectorTypeValueConverter();

    private PageMarginSelectorTypeValueConverter()
    {
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        PageMarginSelectorType result = PageMarginSelectorType.Standard;
        if (values[0] is PageMarginType pageMarginType)
        {
            if (pageMarginType == PageMarginType.Custom)
            {
                if (values[1] is Thickness thickness && values[2] is Thickness thickness2 && thickness.IsEqualTo(thickness2))
                {
                    result = PageMarginSelectorType.LastCustom;
                }
                else
                {
                    result = PageMarginSelectorType.Custom;
                }
            }
            else
            {
                DebugTools.DevAssert(Enum.TryParse<PageMarginSelectorType>(pageMarginType.ToString(), out result), null);
            }
        }
        else
        {
            DebugTools.DevAssert(values[0] == DependencyProperty.UnsetValue, values[0]?.GetType().Name ?? "");
        }

        return result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
