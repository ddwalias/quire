using System;
using System.Globalization;
using Quire.Controls.Worksheet;

namespace Quire;

public partial class PageNumberIsCheckedConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // The Release IL tests `value as IPageNumberControl` and reads it again, with no cast.
        if (value as IPageNumberControl != null)
        {
            return (value as IPageNumberControl).TemplateKey;
        }

        return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return parameter;
    }
    public PageNumberIsCheckedConverter() { }
}
