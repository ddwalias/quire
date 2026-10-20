#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.IO;

public partial class FileUtils
{
    [MirrorVerified("PtcIOFileUtilsMechanicalDifferentialTests")]
    public static string GetLocalConfigFolderPath(bool useAssemblyVersionInConfigPath = false) => GetLocalApplicationDataPath(useAssemblyVersionInConfigPath);

    [MirrorVerified("PtcIOFileUtilsMechanicalDifferentialTests")]
    public static string GetCommonConfigFolderPath(bool useAssemblyVersionInConfigPath = false) => GetCommonApplicationDataPath(useAssemblyVersionInConfigPath);

    [MirrorVerified("PtcIOFileUtilsMechanicalDifferentialTests")]
    public FileUtils() { }

    [MirrorVerified("Release IL")]
    public static string GetApplicationDataPath() => GetAppDataPathWithBase(Environment.GetFolderPath((Environment.SpecialFolder)26), false);

    [MirrorVerified("Release IL")]
    public static string GetLocalApplicationDataPath(bool useAssemblyVersionInConfigPath = false) => GetAppDataPathWithBase(Environment.GetFolderPath((Environment.SpecialFolder)28), useAssemblyVersionInConfigPath);

    [MirrorVerified("Release IL")]
    public static string GetCommonApplicationDataPath(bool useAssemblyVersionInConfigPath = false) => GetAppDataPathWithBase(Environment.GetFolderPath((Environment.SpecialFolder)35), useAssemblyVersionInConfigPath);
}
