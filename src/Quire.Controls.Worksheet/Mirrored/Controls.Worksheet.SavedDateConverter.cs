using System;
using System.Globalization;
using System.Windows.Data;

namespace Quire.Controls.Worksheet;

public partial class SavedDateConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        string text = values[0] as string;
        DateTime dateTime = ((DateTime?)values[1]) ?? DateTime.Now;
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }
        switch (text)
        {
            case "System_Short":
                return dateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture);
            case "System_Long":
                return dateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern, CultureInfo.CurrentCulture);
            case "yyyy_mm_dd":
                return dateTime.ToString(text.Replace("_", "-").Replace("mm", "MM"), CultureInfo.CurrentUICulture);
            default:
                return dateTime.ToString(text.Replace("_", "/").Replace("mm", "MM"), CultureInfo.CurrentUICulture);
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
