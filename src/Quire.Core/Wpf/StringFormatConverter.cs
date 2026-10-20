#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class StringFormatConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return string.Format((parameter as string) ?? "{0}", value);
    }

    [MirrorVerified("PtcWpfStringFormatConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfStringFormatConverterMechanicalDifferentialTests")]
    public StringFormatConverter() { }
}
