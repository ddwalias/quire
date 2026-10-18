#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class IsNotNullOrEmptyToVisibilityConverter
{
    [MirrorVerified("PtcWpfIsNotNullOrEmptyToVisibilityConverterMechanicalDifferentialTests")]
    private readonly bool _collapsed;

    [MirrorVerified("PtcWpfIsNotNullOrEmptyToVisibilityConverterMechanicalDifferentialTests")]
    private IsNotNullOrEmptyToVisibilityConverter(bool collapsed)
    {
        _collapsed = collapsed;
    }

    [MirrorVerified("PtcWpfIsNotNullOrEmptyToVisibilityConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
