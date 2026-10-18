#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire;

public partial class IsProtectedConverter
{
    [MirrorVerified("PtcIsProtectedConverterMechanicalDifferentialTests")]
    private IsProtectedConverter() { }

    [MirrorVerified("PtcIsProtectedConverterMechanicalDifferentialTests")]
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
