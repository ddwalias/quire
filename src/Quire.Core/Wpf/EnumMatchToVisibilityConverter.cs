#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class EnumMatchToVisibilityConverter
{
    [MirrorVerified("PtcWpfEnumMatchToVisibilityConverterMechanicalDifferentialTests")]
    private EnumMatchToVisibilityConverter() { }

    [MirrorVerified("PtcWpfEnumMatchToVisibilityConverterMechanicalDifferentialTests")]
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
