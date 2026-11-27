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
        PageSize pageSize;
        PageMarginType marginType;
        object pageSizeValue = values[0];
        if (pageSizeValue is PageSize)
        {
            pageSize = (PageSize)pageSizeValue;
        }
        else
        {
            pageSize = PageSize.Letter;
            DebugTools.DevAssert(values[0] == DependencyProperty.UnsetValue, values[0]?.GetType().Name ?? "");
        }

        marginType = (PageMarginType)values[1];
        return WorksheetPageLayout.BuiltInPageSizes[pageSize].PageMargins[marginType];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
