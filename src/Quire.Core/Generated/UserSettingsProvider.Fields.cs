#nullable disable

namespace Quire;

public partial class UserSettingsProvider
{
    // Compile order places fields in Generated/ ahead of UserSettingsProvider.cs to preserve Mathcad closure ordinals.
    internal const string ApplicationSettingsGroupName = "applicationSettings";
    internal const string UserSettingsGroupName = "userSettings";
    private const string CompanyName = "Quire";
    private const string UserConfigFileName = "user.config";
    private const string FolderNamePattern = "Quire_Url_[a-zA-Z0-9]{32}";
    private const string SettingsGroupName = "userSettings";
    private static bool _useAssemblyVersionInConfigPath;
}
