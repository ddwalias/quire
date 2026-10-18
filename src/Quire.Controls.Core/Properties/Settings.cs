#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Properties;

internal partial class Settings
{
    [MirrorVerified("PtcControlsCorePropertiesSettingsMechanicalDifferentialTests")]
    public Settings() { }

    [MirrorVerified("PtcControlsCorePropertiesSettingsMechanicalDifferentialTests")]
    private static Settings defaultInstance;

    [MirrorVerified("PtcControlsCorePropertiesSettingsMechanicalDifferentialTests")]
    public static Settings Default => defaultInstance;
}
