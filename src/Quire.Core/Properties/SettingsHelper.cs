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
}
