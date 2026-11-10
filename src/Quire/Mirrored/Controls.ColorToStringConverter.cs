#nullable disable

namespace Quire.Controls;

public partial class ColorToStringConverter
{
    private const string AlphaColor = "#FF";
    private const int ColorValueLength = 6;

    public object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }
        return value.ToString().Substring(3);
    }

    public object ConvertBack(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        return new ColorInfo("#FF" + ((string)value)?.PadLeft(6, '0'));
    }
}
