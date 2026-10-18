#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class BooleanToVisibilityConverter
{
    [MirrorVerified("PtcWpfBooleanToVisibilityConverterMechanicalDifferentialTests")]
    public BooleanToVisibilityConverter() : this(true) { }

    [MirrorVerified("PtcWpfBooleanToVisibilityConverterMechanicalDifferentialTests")]
    private readonly bool _valueForVisible;

    [MirrorVerified("PtcWpfBooleanToVisibilityConverterMechanicalDifferentialTests")]
    private BooleanToVisibilityConverter(bool valueForVisible)
    {
        _valueForVisible = valueForVisible;
    }

    [MirrorVerified("PtcWpfBooleanToVisibilityConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
