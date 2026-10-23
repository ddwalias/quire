#nullable disable
using System.Globalization;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class BooleanToLayoutTransformConverter
{
    [MirrorVerified("PtcWpfBooleanToLayoutTransformConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is bool flag && flag))
        {
            return Transform.Identity;
        }
        return new RotateTransform(270.0);
    }

    [MirrorVerified("PtcWpfBooleanToLayoutTransformConverterMechanicalDifferentialTests")]
    public BooleanToLayoutTransformConverter() { }
}
