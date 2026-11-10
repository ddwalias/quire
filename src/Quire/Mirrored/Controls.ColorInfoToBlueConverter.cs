#nullable disable

namespace Quire.Controls;

public partial class ColorInfoToBlueConverter
{
    public override object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        _colorInfo = (ColorInfo)value;
        return ((ColorInfo)value).Blue;
    }

    public override object ConvertBack(object value, global::System.Type targetTypes, object parameter, global::System.Globalization.CultureInfo culture)
    {
        if (value != null && value.ToString() != "")
        {
            return new ColorInfo(_colorInfo.Red, _colorInfo.Green, double.Parse(value.ToString()));
        }
        return new ColorInfo(_colorInfo.Red, _colorInfo.Green, 0.0);
    }
}
