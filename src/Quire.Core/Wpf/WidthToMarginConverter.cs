#nullable disable
using System.Globalization;
using System.Globalization;
using System.Windows;
using Quire.Mirror;
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

    [MirrorVerified("Release IL")]
    public WidthToMarginConverter()
    {
        Multiplier = 1.0;
    }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => new Thickness((double)value * Multiplier + Addition);
}
