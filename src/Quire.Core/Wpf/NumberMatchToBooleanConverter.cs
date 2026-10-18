#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class NumberMatchToBooleanConverter
{
    [MirrorVerified("PtcWpfNumberMatchToBooleanConverterMechanicalDifferentialTests")]
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfNumberMatchToBooleanConverterMechanicalDifferentialTests")]
    public NumberMatchToBooleanConverter() { }
}
