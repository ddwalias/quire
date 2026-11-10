#nullable disable

namespace Quire.Controls;

public partial class ColorInfoToGreenGradientEndConverter
{
    public override object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        return global::System.Windows.Media.Color.FromRgb((byte)((ColorInfo)value).Red, 255, (byte)((ColorInfo)value).Blue);
    }
}
