#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Properties;

internal partial class Settings
{
    [MirrorVerified("PtcControlsPropertiesSettingsMechanicalDifferentialTests")]
    public Settings() { }

    [MirrorVerified("PtcControlsPropertiesSettingsMechanicalDifferentialTests")]
    private static Settings defaultInstance = (Settings)global::System.Configuration.SettingsBase.Synchronized(new Settings());

    [MirrorVerified("PtcControlsPropertiesSettingsMechanicalDifferentialTests")]
    public static Settings Default => defaultInstance;

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("")]
    public string MicrosoftPdfDriverName
    {
        get
        {
            return ((string)this["MicrosoftPdfDriverName"]) ?? null;
        }
        set
        {
            this["MicrosoftPdfDriverName"] = value;
        }
    }
}
