#nullable disable
#pragma warning disable

namespace Quire;

partial class UserSettingsProvider
{
    private static string GetSectionName(string sectionGroupName, global::System.Configuration.SettingsContext context)
    {
        string text = (string)context["GroupName"];
        string settingsKey = (string)context["SettingsKey"];
        if (!string.IsNullOrEmpty(settingsKey))
        {
            text = text + "." + settingsKey;
        }
        return sectionGroupName + "/" + global::System.Xml.XmlConvert.EncodeLocalName(text);
    }

    public static bool UseAssemblyVersionInConfigPath
    {
        get => _useAssemblyVersionInConfigPath;
        set
        {
            if (_useAssemblyVersionInConfigPath != value)
            {
                _useAssemblyVersionInConfigPath = value;
                ConfigFolderName = null;
            }
        }
    }
}
