#nullable disable
using System;
using System.Configuration;
using System.Configuration;
using System.IO;
using Quire.IO;
using Quire.Mirror;
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

    [MirrorVerified("Release IL")]
    private bool IsApplicationScoped(SettingsProperty prop) => HasSettingScope(prop, typeof(ApplicationScopedSettingAttribute));

    [MirrorVerified("Release IL")]
    private bool IsUserScoped(SettingsProperty prop) => HasSettingScope(prop, typeof(UserScopedSettingAttribute));

    private bool HasSettingScope(SettingsProperty prop, Type attributeType)
    {
        bool result = prop.Attributes[typeof(SettingsManageabilityAttribute)] != null;
        bool flag = prop.Attributes[typeof(ApplicationScopedSettingAttribute)] != null;
        bool flag2 = prop.Attributes[typeof(UserScopedSettingAttribute)] != null;
        if (flag2 & flag)
        {
            DebugTools.FailAssertion((string)null);
        }
        else if (!flag2 && !flag)
        {
            DebugTools.FailAssertion((string)null);
        }
        if (attributeType == typeof(ApplicationScopedSettingAttribute))
        {
            return flag;
        }
        if (attributeType == typeof(UserScopedSettingAttribute))
        {
            return flag2;
        }
        if (attributeType == typeof(SettingsManageabilityAttribute))
        {
            return result;
        }
        DebugTools.FailAssertion((string)null);
        return false;
    }

    [MirrorVerified("Release IL")]
    internal static string ConfigFilenameFromApplicationConfigPath(string applicationConfigPath) => Path.GetFileNameWithoutExtension(applicationConfigPath) + ".V7" + Path.GetExtension(applicationConfigPath);

    [MirrorVerified("Release IL")]
    public static string LocalConfigFolderName => FileUtils.GetLocalConfigFolderPath(UseAssemblyVersionInConfigPath);

    [MirrorVerified("Release IL")]
    public static string CommonConfigFolderName => FileUtils.GetCommonConfigFolderPath(UseAssemblyVersionInConfigPath);
}
