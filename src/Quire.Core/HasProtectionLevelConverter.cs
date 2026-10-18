#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire;

public partial class HasProtectionLevelConverter
{
    [MirrorVerified("PtcHasProtectionLevelConverterMechanicalDifferentialTests")]
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcHasProtectionLevelConverterMechanicalDifferentialTests")]
    public HasProtectionLevelConverter() { }
}
