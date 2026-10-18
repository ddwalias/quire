#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class BooleanOperatorMultiConverter
{
    [MirrorVerified("PtcWpfBooleanOperatorMultiConverterMechanicalDifferentialTests")]
    private BooleanOperatorMultiConverter() { }

    [MirrorVerified("PtcWpfBooleanOperatorMultiConverterMechanicalDifferentialTests")]
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
