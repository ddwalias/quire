#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class SolidColorBrushToColorConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }
        return ((System.Windows.Media.SolidColorBrush)value).Color;
    }

    [MirrorVerified("Release IL")]
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }
        return new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)value);
    }

    [MirrorVerified("PtcWpfSolidColorBrushToColorConverterMechanicalDifferentialTests")]
    public SolidColorBrushToColorConverter() { }
}
