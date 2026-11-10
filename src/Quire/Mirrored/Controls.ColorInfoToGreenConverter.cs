#nullable disable

namespace Quire.Controls;

public partial class ColorInfoToGreenConverter
{
    public override object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        _colorInfo = (ColorInfo)value;
        return ((ColorInfo)value).Green;
    }

    public override object ConvertBack(object value, global::System.Type targetTypes, object parameter, global::System.Globalization.CultureInfo culture)
    {
        if (value != null && value.ToString() != "")
        {
            return new ColorInfo(_colorInfo.Red, double.Parse(value.ToString()), _colorInfo.Blue);
        }
        return new ColorInfo(_colorInfo.Red, 0.0, _colorInfo.Blue);
    }
}
