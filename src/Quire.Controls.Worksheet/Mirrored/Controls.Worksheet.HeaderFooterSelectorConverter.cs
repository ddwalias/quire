using System;
using System.Globalization;

namespace Quire.Controls.Worksheet;

public partial class HeaderFooterSelectorConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 4)
        {
            return null;
        }
        int num = (int)values[0];
        bool flag = (bool)values[1];
        IHeaderFooterControl headerFooterControl = (IHeaderFooterControl)values[3];
        if (((num == 1) & flag) && headerFooterControl != null)
        {
            return headerFooterControl;
        }
        return (IHeaderFooterControl)values[2];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("HeaderFooterSelectorConverter does not support backward conversion");
    }

    public HeaderFooterSelectorConverter() { }
}
