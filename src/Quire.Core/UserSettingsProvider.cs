#nullable disable
using System.Configuration;
using Quire.Mirror;

namespace Quire;

public partial class UserSettingsProvider
{
    [MirrorVerified("PtcUserSettingsProviderMechanicalDifferentialTests")]
    public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property) => throw new NotImplementedException();

    [MirrorVerified("PtcUserSettingsProviderMechanicalDifferentialTests")]
    public void Upgrade(SettingsContext context, SettingsPropertyCollection properties) { }

    [MirrorVerified("PtcUserSettingsProviderMechanicalDifferentialTests")]
    public UserSettingsProvider() { }

    [MirrorVerified("PtcUserSettingsProviderMechanicalDifferentialTests")]
    public static string ConfigFolderName { get; set; }

    [MirrorVerified("PtcUserSettingsProviderMechanicalDifferentialTests")]
    public static bool IsConfigCopiedForMOR { get; set; }

    [MirrorVerified("PtcUserSettingsProviderMechanicalDifferentialTests")]
    public override string ApplicationName { get; set; }
}
