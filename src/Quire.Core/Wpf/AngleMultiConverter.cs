#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class AngleMultiConverter
{
    [MirrorVerified("PtcWpfAngleMultiConverterMechanicalDifferentialTests")]
    private AngleMultiConverter() { }

    [MirrorVerified("PtcWpfAngleMultiConverterMechanicalDifferentialTests")]
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
