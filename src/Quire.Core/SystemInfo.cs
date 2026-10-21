#nullable disable
using Quire.Mirror;

namespace Quire;

public static partial class SystemInfo
{
    [MirrorVerified("Release IL")]
    public static OSVersions OSVersion
    {
        get
        {
            OSVersions? osVersion = _osVersion;
            if (!osVersion.HasValue)
            {
                OSVersions? oSVersions = (_osVersion = IdentifyOSVersion());
                return oSVersions.Value;
            }
            return osVersion.GetValueOrDefault();
        }
    }

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
