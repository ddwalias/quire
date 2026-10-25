#nullable disable
using System.Configuration;
using System.Diagnostics;
using Quire.Mirror;

namespace Quire.Controls.Core.Properties;

internal sealed partial class Settings : ApplicationSettingsSafe
{
    [MirrorVerified("PtcControlsCorePropertiesSettingsMechanicalDifferentialTests")]
    private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

    [MirrorVerified("PtcControlsCorePropertiesSettingsMechanicalDifferentialTests")]
    public static Settings Default => defaultInstance;

    [MirrorVerified("PtcControlsCorePropertiesSettingsMechanicalDifferentialTests")]
    public Settings() { }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("True")]
    public bool HandleFocusForIme
    {
        get => ((bool?)this["HandleFocusForIme"]).GetValueOrDefault();
        set => this["HandleFocusForIme"] = value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("True")]
    public bool EnableFontCaching
    {
        get => ((bool?)this["EnableFontCaching"]).GetValueOrDefault();
        set => this["EnableFontCaching"] = value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("2000000")]
    public int MaxNumberOf2dPoints
    {
        get => ((int?)this["MaxNumberOf2dPoints"]).GetValueOrDefault();
        set => this["MaxNumberOf2dPoints"] = value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("40000")]
    public int MaxNumberOf3dPoints
    {
        get => ((int?)this["MaxNumberOf3dPoints"]).GetValueOrDefault();
        set => this["MaxNumberOf3dPoints"] = value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("5000")]
    public int MinPointsForDensity
    {
        get => ((int?)this["MinPointsForDensity"]).GetValueOrDefault();
        set => this["MinPointsForDensity"] = value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("50000")]
    public int MinPointsForForceDensity
    {
        get => ((int?)this["MinPointsForForceDensity"]).GetValueOrDefault();
        set => this["MinPointsForForceDensity"] = value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("120")]
    public int DensityPointsPrintDPI
    {
        get => ((int?)this["DensityPointsPrintDPI"]).GetValueOrDefault();
        set => this["DensityPointsPrintDPI"] = value;
    }
}
