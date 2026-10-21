#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ScaleConverter
{
    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double number;
        double scale;
        if (!IsNumeric(value, out number) || !IsNumeric(parameter, out scale))
        {
            return 0;
        }
        return number * scale;
    }

    [MirrorVerified("Release IL")]
    public bool IsNumeric(object expression, out double number)
    {
        number = 0.0;
        if (expression == null)
        {
            return false;
        }
        return double.TryParse(
            global::System.Convert.ToString(expression, CultureInfo.InvariantCulture),
            NumberStyles.Any,
            NumberFormatInfo.InvariantInfo,
            out number);
    }

    [MirrorVerified("PtcWpfScaleConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfScaleConverterMechanicalDifferentialTests")]
    public ScaleConverter() { }
}
