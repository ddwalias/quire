#nullable disable
#pragma warning disable

namespace Quire;

partial class UserSettingsProvider
{
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
