#nullable disable

namespace Quire.Controls;

public partial class ColorInfoToColorConverter
{
    public object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        return global::System.Windows.Media.Color.FromRgb(
            (byte)((ColorInfo)value).Red,
            (byte)((ColorInfo)value).Green,
            (byte)((ColorInfo)value).Blue);
    }

    public object ConvertBack(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        return new ColorInfo(
            (double)((global::System.Windows.Media.Color)value).R,
            (double)((global::System.Windows.Media.Color)value).G,
            (double)((global::System.Windows.Media.Color)value).B);
    }
}
