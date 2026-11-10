#nullable disable

namespace Quire.Controls;

public partial class AlphaColorToBackgroundConverter
{
    private const byte MaxByteValue = 255;

    public object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        if (value != null && ((global::System.Windows.Media.Color)value).A != 255)
        {
            return parameter;
        }
        return new global::System.Windows.Media.SolidColorBrush((global::System.Windows.Media.Color)value);
    }

    public object ConvertBack(object value, global::System.Type targetTypes, object parameter, global::System.Globalization.CultureInfo culture)
    {
        return global::System.Windows.Media.Colors.White;
    }
}
