#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class BooleanToLayoutTransformConverter
{
    [MirrorVerified("PtcWpfBooleanToLayoutTransformConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfBooleanToLayoutTransformConverterMechanicalDifferentialTests")]
    public BooleanToLayoutTransformConverter() { }
}
