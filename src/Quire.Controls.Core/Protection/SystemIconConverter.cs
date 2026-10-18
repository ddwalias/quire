#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls.Core.Protection;

public partial class SystemIconConverter
{
    [MirrorVerified("PtcControlsCoreProtectionSystemIconConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type type, object parameter, CultureInfo culture) => throw new NotSupportedException();

    [MirrorVerified("PtcControlsCoreProtectionSystemIconConverterMechanicalDifferentialTests")]
    public SystemIconConverter() { }
}
