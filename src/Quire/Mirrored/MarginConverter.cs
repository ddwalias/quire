#nullable disable

using System;
using System.Globalization;
using System.Windows;
using Quire.Controls.Core;

namespace Quire;

internal partial class MarginConverter
{
    public static MarginConverter Instance = new MarginConverter();

    private MarginConverter()
    {
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        PageSize key;
        PageMarginType key2;
        object obj = values[0];
        if (obj is PageSize)
        {
            key = (PageSize)obj;
        }
        else
        {
            key = PageSize.Letter;
            DebugTools.DevAssert(values[0] == DependencyProperty.UnsetValue, values[0]?.GetType().Name ?? "");
        }

        key2 = (PageMarginType)values[1];
        return WorksheetPageLayout.BuiltInPageSizes[key].PageMargins[key2];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
