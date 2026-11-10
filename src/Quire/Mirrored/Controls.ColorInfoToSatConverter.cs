#nullable disable

namespace Quire.Controls;

public partial class ColorInfoToSatConverter
{
    public override object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        _colorInfo = (ColorInfo)value;
        return _colorInfo.Saturation;
    }

    public override object ConvertBack(object value, global::System.Type targetTypes, object parameter, global::System.Globalization.CultureInfo culture)
    {
        if (value != null && value.ToString() != "")
        {
            return new ColorInfo(_colorInfo.Hue, double.Parse(value.ToString()), _colorInfo.Lum);
        }
        return new ColorInfo(_colorInfo.Hue, 0.0, _colorInfo.Lum);
    }
}
