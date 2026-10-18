#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class WidthToMarginConverter
{
    [MirrorVerified("PtcWpfWidthToMarginConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfWidthToMarginConverterMechanicalDifferentialTests")]
    public double Multiplier { get; set; }

    [MirrorVerified("PtcWpfWidthToMarginConverterMechanicalDifferentialTests")]
    public double Addition { get; set; }
}
