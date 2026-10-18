#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Properties;

internal partial class Settings
{
    [MirrorVerified("PtcControlsPropertiesSettingsMechanicalDifferentialTests")]
    public Settings() { }

    [MirrorVerified("PtcControlsPropertiesSettingsMechanicalDifferentialTests")]
    private static Settings defaultInstance;

    [MirrorVerified("PtcControlsPropertiesSettingsMechanicalDifferentialTests")]
    public static Settings Default => defaultInstance;
}
