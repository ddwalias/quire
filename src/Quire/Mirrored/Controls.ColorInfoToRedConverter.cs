#nullable disable

namespace Quire.Controls;

public partial class ColorInfoToRedConverter
{
    protected ColorInfo _colorInfo;

    public virtual object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
    {
        _colorInfo = (ColorInfo)value;
        return ((ColorInfo)value).Red;
    }

    public virtual object ConvertBack(object value, global::System.Type targetTypes, object parameter, global::System.Globalization.CultureInfo culture)
    {
        if (value != null && value.ToString() != "")
        {
            return new ColorInfo(double.Parse(value.ToString()), _colorInfo.Green, _colorInfo.Blue);
        }
        return new ColorInfo(0.0, _colorInfo.Green, _colorInfo.Blue);
    }
}
