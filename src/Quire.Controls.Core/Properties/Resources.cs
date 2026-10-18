#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls.Core.Properties;

internal partial class Resources
{
    [MirrorVerified("PtcControlsCorePropertiesResourcesMechanicalDifferentialTests")]
    internal Resources() { }

    [MirrorVerified("PtcControlsCorePropertiesResourcesMechanicalDifferentialTests")]
    private static CultureInfo resourceCulture;

    [MirrorVerified("PtcControlsCorePropertiesResourcesMechanicalDifferentialTests")]
    internal static CultureInfo Culture { get => resourceCulture; set => resourceCulture = value; }
}
