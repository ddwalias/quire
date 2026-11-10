#nullable disable

namespace Quire.Controls;

public partial class ColorInfoToHueConverter
{
    public override object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        _colorInfo = (ColorInfo)value;
        return _colorInfo.Hue;
    }

    public override object ConvertBack(object value, global::System.Type targetTypes, object parameter, global::System.Globalization.CultureInfo culture)
    {
        if (_colorInfo.Lum == 0f || _colorInfo.Lum == 100f)
        {
            _colorInfo.Lum = 50f;
        }
        if (value != null && value.ToString() != "")
        {
            return new ColorInfo(double.Parse(value.ToString()), _colorInfo.Saturation, _colorInfo.Lum);
        }
        return new ColorInfo(0.0, _colorInfo.Saturation, _colorInfo.Lum);
    }
}
