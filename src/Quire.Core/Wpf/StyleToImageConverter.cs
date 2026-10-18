#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class StyleToImageConverter
{
    [MirrorVerified("PtcWpfStyleToImageConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

    [MirrorVerified("PtcWpfStyleToImageConverterMechanicalDifferentialTests")]
    public StyleToImageConverter() { }
}
