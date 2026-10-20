#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire;

public partial class IsProtectedConverter
{
    [MirrorVerified("Release IL")]
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (DebugTools.DevAssert(value is ProtectionLevel, null).Failed)
        {
            return false;
        }
        return ((ProtectionLevel)value).IsProtected();
    }

    [MirrorVerified("PtcIsProtectedConverterMechanicalDifferentialTests")]
    private IsProtectedConverter() { }

    [MirrorVerified("PtcIsProtectedConverterMechanicalDifferentialTests")]
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
