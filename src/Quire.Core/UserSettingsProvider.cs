#nullable disable
using System;
using System.Configuration;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
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

    public override void Initialize(string name, global::System.Collections.Specialized.NameValueCollection values)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = GetType().Name;
        }
        base.Initialize(name, values);
    }

    public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals)
    {
        Configuration origConfig;
        Configuration modifiedConfig;
        ClientSettingsSection section2;
        GetSectionStatusEnum section = GetSection(context, out section2, out origConfig, out modifiedConfig);
        if (section == GetSectionStatusEnum.NoSection)
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            return;
        }
        foreach (SettingsPropertyValue propval in propvals)
        {
            if (!propval.IsDirty || propval.SerializedValue == null)
            {
                continue;
            }
            if (!IsUserScoped(propval.Property))
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
                continue;
            }
            foreach (SettingElement setting in section2.Settings)
            {
                if (setting.Name != propval.Name)
                {
                    continue;
                }
                object propertyValue = propval.PropertyValue;
                XmlNode valueXml = setting.Value.ValueXml;
                switch (setting.SerializeAs)
                {
                    case SettingsSerializeAs.String:
                        valueXml.InnerText = propertyValue.ToString();
                        break;
                    case SettingsSerializeAs.Xml:
                    {
                        using (StringWriter stringWriter = new StringWriter())
                        {
                            new XmlSerializer(propertyValue.GetType()).Serialize(stringWriter, propertyValue);
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.LoadXml(stringWriter.GetStringBuilder().ToString());
                            if (DebugTools.AssertNotNull(valueXml.OwnerDocument, null).Passed
                                && DebugTools.AssertNotNull(xmlDocument.DocumentElement, null).Passed)
                            {
                                XmlNode xmlNode = valueXml.OwnerDocument.ImportNode(xmlDocument.DocumentElement, deep: true);
                                XmlNode xmlNode2 = valueXml.SelectSingleNode(xmlNode.Name);
                                if (DebugTools.AssertNotNull(xmlNode2, null).Passed)
                                {
                                    valueXml.ReplaceChild(xmlNode, xmlNode2);
                                }
                            }
                        }
                        break;
                    }
                    default:
                        DebugTools.FailAssertion(setting.SerializeAs.ToString());
                        break;
                }
                setting.Value.ValueXml = null;
                setting.Value.ValueXml = valueXml;
            }
        }
        try
        {
            if (section == GetSectionStatusEnum.ModifiedSection)
            {
                modifiedConfig.Save();
            }
            else if (modifiedConfig != null)
            {
                origConfig.SaveAs(modifiedConfig.FilePath);
            }
        }
        catch (Exception ex)
        {
            if (!(ex is UnauthorizedAccessException))
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
        }
    }

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

    protected GetSectionStatusEnum GetSection(SettingsContext context, out ClientSettingsSection section, out Configuration origConfig, out Configuration modifiedConfig)
    {
        GetSectionStatusEnum result = GetSectionStatusEnum.NoSection;
        section = null;
        origConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
        ConfigFolderName = global::Quire.Services.ApplicationVariant.IsEngNbPlus == true ? LocalConfigFolderName : CommonConfigFolderName;
        exeConfigurationFileMap.ExeConfigFilename = Path.Combine(ConfigFolderName, ConfigFilenameFromApplicationConfigPath(origConfig.FilePath));
        ObtainConfigFiles(exeConfigurationFileMap, out modifiedConfig);
        string sectionName = GetSectionName("userSettings", context);
        if (modifiedConfig != null)
        {
            section = modifiedConfig.GetSection(sectionName) as ClientSettingsSection;
            result = GetSectionStatusEnum.ModifiedSection;
        }
        if (section == null)
        {
            section = origConfig.GetSection(sectionName) as ClientSettingsSection;
            result = GetSectionStatusEnum.OriginalSection;
        }
        if (section == null)
        {
            result = GetSectionStatusEnum.NoSection;
        }
        return result;
    }

    public void Reset(SettingsContext context)
    {
        Configuration origConfig = null;
        Configuration modifiedConfig = null;
        ClientSettingsSection section = null;
        if (GetSection(context, out section, out origConfig, out modifiedConfig) == GetSectionStatusEnum.ModifiedSection)
        {
            new FileInfo(modifiedConfig.FilePath).Delete();
        }
    }

    public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
    {
        SettingsPropertyValueCollection settingsPropertyValueCollection = new SettingsPropertyValueCollection();
        if (GetSection(context, out ClientSettingsSection section, out _, out _) == GetSectionStatusEnum.NoSection)
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            return settingsPropertyValueCollection;
        }
        foreach (SettingsProperty prop in props)
        {
            if (!IsUserScoped(prop))
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
                continue;
            }
            SettingsPropertyValue settingsPropertyValue = new SettingsPropertyValue(prop);
            foreach (SettingElement setting in section.Settings)
            {
                if (!(setting.Name != prop.Name))
                {
                    XmlNode valueXml = setting.Value.ValueXml;
                    object serializedValue = null;
                    switch (setting.SerializeAs)
                    {
                        case SettingsSerializeAs.String:
                            serializedValue = valueXml.InnerText;
                            break;
                        case SettingsSerializeAs.Xml:
                            serializedValue = valueXml.InnerXml;
                            break;
                        default:
                            DebugTools.FailAssertion(setting.SerializeAs.ToString());
                            break;
                    }
                    settingsPropertyValue.SerializedValue = serializedValue;
                    break;
                }
            }
            settingsPropertyValueCollection.Add(settingsPropertyValue);
        }
        return settingsPropertyValueCollection;
    }

    [MirrorVerified("Release IL")]
    internal static string ConfigFilenameFromApplicationConfigPath(string applicationConfigPath) => Path.GetFileNameWithoutExtension(applicationConfigPath) + ".V7" + Path.GetExtension(applicationConfigPath);

    [MirrorVerified("Release IL")]
    public static string LocalConfigFolderName => FileUtils.GetLocalConfigFolderPath(UseAssemblyVersionInConfigPath);

    [MirrorVerified("Release IL")]
    public static string CommonConfigFolderName => FileUtils.GetCommonConfigFolderPath(UseAssemblyVersionInConfigPath);
}
