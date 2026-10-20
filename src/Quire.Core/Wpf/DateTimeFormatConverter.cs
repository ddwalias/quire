#nullable disable
using System;
using System.Globalization;
using System.Threading;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class DateTimeFormatConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType == typeof(string) && parameter != null && parameter is string)
        {
            return string.Format(Thread.CurrentThread.CurrentUICulture, parameter as string, value);
        }
        if (!targetType.IsValueType)
        {
            return null;
        }
        return Activator.CreateInstance(targetType);
    }

    [MirrorVerified("PtcWpfDateTimeFormatConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfDateTimeFormatConverterMechanicalDifferentialTests")]
    public DateTimeFormatConverter() { }
}
