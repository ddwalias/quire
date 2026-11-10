#nullable disable

namespace Quire.Controls;

public partial class NewColorToEnabledConverter
{
    public object Convert(object[] values, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        return values[0].ToString().Substring(3) != values[1].ToString().Substring(3);
    }

    public object[] ConvertBack(object value, global::System.Type[] targetTypes, object parameter, global::System.Globalization.CultureInfo culture)
    {
        return null;
    }
}
