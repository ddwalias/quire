#nullable disable

namespace Quire.Controls;

public partial class ColorInfoToLumConverter
{
    public override object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        _colorInfo = (ColorInfo)value;
        return _colorInfo.Lum;
    }

    public override object ConvertBack(object value, global::System.Type targetTypes, object parameter, global::System.Globalization.CultureInfo culture)
    {
        if (value != null && value.ToString() != "")
        {
            return new ColorInfo(_colorInfo.Hue, _colorInfo.Saturation, float.Parse(value.ToString()));
        }
        return new ColorInfo(_colorInfo.Hue, _colorInfo.Saturation, 0f);
    }
}
