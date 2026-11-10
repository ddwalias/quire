#nullable disable

namespace Quire.Controls;

public partial class ColorInfoToGreenGradientStartConverter
{
    public new virtual object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        return global::System.Windows.Media.Color.FromRgb((byte)((ColorInfo)value).Red, 0, (byte)((ColorInfo)value).Blue);
    }
}
