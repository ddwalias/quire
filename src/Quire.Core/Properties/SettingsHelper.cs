#nullable disable
using System.IO;
using System.Reflection;
using Quire.IO;
using Quire.Mirror;

namespace Quire.Properties;

public static partial class SettingsHelper
{
    [MirrorVerified("Release IL")]
    private static string MathcadPrimeConfigFilePath => Path.Combine(FileUtils.GetAssemblyFolder(Assembly.GetExecutingAssembly()), "MathcadPrime.dll.config");

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
