#nullable disable
using Quire.Mirror;

namespace Quire;

public static partial class SystemInfo
{
    [MirrorVerified("Release IL")]
    public static bool OsBeforeXp() => OSVersion < OSVersions.WindowsXP;
}
