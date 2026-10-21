#nullable disable
using Quire.Mirror;

namespace Quire;

public static partial class SystemInfo
{
    [MirrorVerified("Release IL")]
    public static bool OsBeforeXp() => OSVersion < OSVersions.WindowsXP;

    [MirrorVerified("Release IL")]
    public static bool IsWindows8Point1orGreater()
    {
        if (OSVersion == OSVersions.Unknown)
        {
            return false;
        }
        return OSVersion >= OSVersions.Windows8Point1;
    }
}
