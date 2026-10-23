#nullable disable
#pragma warning disable

namespace Quire;

partial class UserSettingsProvider
{
    private static string GetSectionName(string sectionGroupName, global::System.Configuration.SettingsContext context)
    {
        string text = (string)context["GroupName"];
        string text2 = (string)context["SettingsKey"];
        if (!string.IsNullOrEmpty(text2))
        {
            text = text + "." + text2;
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
