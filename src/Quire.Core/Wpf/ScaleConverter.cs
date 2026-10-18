#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ScaleConverter
{
    [MirrorVerified("PtcWpfScaleConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfScaleConverterMechanicalDifferentialTests")]
    public ScaleConverter() { }
}
