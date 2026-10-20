#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire;

public partial class HasProtectionLevelConverter
{
    [MirrorVerified("Release IL")]
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (DebugTools.DevAssert(value is ProtectionLevel && parameter is ProtectionLevel, null).Failed)
        {
            return false;
        }
        return ((ProtectionLevel)value).HasFlag((ProtectionLevel)parameter);
    }

    [MirrorVerified("PtcHasProtectionLevelConverterMechanicalDifferentialTests")]
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcHasProtectionLevelConverterMechanicalDifferentialTests")]
    public HasProtectionLevelConverter() { }
}
