#nullable disable

namespace Quire.Controls;

public partial class AlphaColorToPrefixConverter
{
    private const string AlphaDefaultValue = "#FF";

    public object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        if (value != null)
        {
            return value.ToString().Substring(0, 3);
        }
        return "#FF";
    }

    public object ConvertBack(object value, global::System.Type targetTypes, object parameter, global::System.Globalization.CultureInfo culture)
    {
        return null;
    }
}
