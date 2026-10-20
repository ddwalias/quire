#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class DateFormatConverter
{
    [MirrorVerified("PtcWpfDateFormatConverterMechanicalDifferentialTests")]
    public DateFormatConverter() { }

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
