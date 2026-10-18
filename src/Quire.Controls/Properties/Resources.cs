#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls.Properties;

public partial class Resources
{
    [MirrorVerified("PtcControlsPropertiesResourcesMechanicalDifferentialTests")]
    internal Resources() { }

    [MirrorVerified("PtcControlsPropertiesResourcesMechanicalDifferentialTests")]
    private static CultureInfo resourceCulture;

    [MirrorVerified("PtcControlsPropertiesResourcesMechanicalDifferentialTests")]
    public static CultureInfo Culture { get => resourceCulture; set => resourceCulture = value; }
}
