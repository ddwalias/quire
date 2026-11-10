#nullable disable

namespace Quire.Controls;

public partial class ColorInfoToRedGradientStartConverter
{
    public virtual object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        return global::System.Windows.Media.Color.FromRgb(0, (byte)((ColorInfo)value).Green, (byte)((ColorInfo)value).Blue);
    }

    public virtual object ConvertBack(object value, global::System.Type targetTypes, object parameter, global::System.Globalization.CultureInfo culture)
    {
        return global::System.Windows.Media.Colors.AliceBlue;
    }
}
