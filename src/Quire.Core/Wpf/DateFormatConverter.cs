#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class DateFormatConverter
{
    [MirrorVerified("PtcWpfDateFormatConverterMechanicalDifferentialTests")]
    public DateFormatConverter() { }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string text = "00";
        if (parameter != null)
        {
            text = parameter.ToString().Trim();
        }
        if (value is int)
        {
            return string.Format("{0:" + text + "}", value);
        }
        if (value is DateTime) goto DateTimeValue;
        return string.Empty;

    DateTimeValue:
        DateTime dateTime = (DateTime)value;
        return dateTime.ToString(text, culture);
    }

    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(DateTime))
        {
            return null;
        }
        string s = value.ToString();
        DateTime result = DateTime.MinValue;
        if (!DateTime.TryParse(s, culture, DateTimeStyles.None, out result))
        {
            return DateTime.MinValue;
        }
        return result;
    }
}
