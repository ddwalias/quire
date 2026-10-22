#nullable disable
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Linq;
using Quire.IO;
using Quire.Mirror;

namespace Quire.Properties;

public static partial class SettingsHelper
{

    [MirrorVerified("Release IL")]
    public static string GetPerUserMathcadPrimeConfigFile(bool copyIfNeeded)
    {
        string path = Path.Combine(
            UserSettingsProvider.LocalConfigFolderName,
            UserSettingsProvider.ConfigFilenameFromApplicationConfigPath(MathcadPrimeConfigFilePath));
        if (!File.Exists(path))
        {
            if (copyIfNeeded)
            {
                File.Copy(MathcadPrimeConfigFilePath, path);
            }
            else
            {
                path = null;
            }
        }
        return path;
    }

    [MirrorVerified("Release IL")]
    private static System.Collections.Generic.IDictionary<string, string> GetMathcadPrimeLicenseConfigValues(params string[] keys)
    {
        try
        {
            System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap
                {
                    ExeConfigFilename = GetAllUsersMathcadPrimeConfigFile(false) ?? MathcadPrimeConfigFilePath
                },
                ConfigurationUserLevel.None);
            ConfigurationSectionGroup userSettings = configuration.GetSectionGroup("userSettings");
            ClientSettingsSection ptcProperties = (ClientSettingsSection)userSettings.Sections[LicenseSettingsNamespace];
            return keys.ToDictionary(key => key, key => ptcProperties.Settings.Get(key).Value.ValueXml.InnerText);
        }
        catch
        {
            return null;
        }
    }

    [MirrorVerified("Release IL")]
    private static string MathcadPrimeConfigFilePath => Path.Combine(FileUtils.GetAssemblyFolder(Assembly.GetExecutingAssembly()), "MathcadPrime.dll.config");



    [MirrorVerified("Release IL")]
    public static string GetAllUsersMathcadPrimeConfigFile(bool copyIfNeeded)
    {
        string path = Path.Combine(
            UserSettingsProvider.CommonConfigFolderName,
            UserSettingsProvider.ConfigFilenameFromApplicationConfigPath(MathcadPrimeConfigFilePath));
        if (!File.Exists(path))
        {
            if (copyIfNeeded)
            {
                File.Copy(MathcadPrimeConfigFilePath, path);
            }
            else
            {
                path = null;
            }
        }
        return path;
    }

}
