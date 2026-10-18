#nullable disable
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
}
