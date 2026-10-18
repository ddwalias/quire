#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class BooleansToVisibilityMultiConverter
{
    [MirrorVerified("PtcWpfBooleansToVisibilityMultiConverterMechanicalDifferentialTests")]
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfBooleansToVisibilityMultiConverterMechanicalDifferentialTests")]
    public BooleansToVisibilityMultiConverter() { }

    [MirrorVerified("PtcWpfBooleansToVisibilityMultiConverterMechanicalDifferentialTests")]
    public bool Collapse { get; set; }
}
