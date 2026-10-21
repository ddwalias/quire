#nullable disable
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire;

public static partial class SystemInfo
{
    [MirrorVerified("Release IL")]
    private static OSVersions? _osVersion;

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
    private static OSVersions IdentifyOSVersion()
    {
        Dictionary<PlatformID, Dictionary<(int, int), OSVersions>> dictionary = new Dictionary<PlatformID, Dictionary<(int, int), OSVersions>>
        {
            {
                PlatformID.Win32Windows,
                new Dictionary<(int, int), OSVersions>
                {
                    { (-1, -1), OSVersions.WindowsBeforeXP }
                }
            },
            {
                PlatformID.Win32NT,
                new Dictionary<(int, int), OSVersions>
                {
                    { (5, 0), OSVersions.WindowsBeforeXP },
                    { (5, 1), OSVersions.WindowsXP },
                    { (5, 2), OSVersions.WindowsServer2003 },
                    { (6, 0), OSVersions.WindowsVistaOrServer2008 },
                    { (6, 1), OSVersions.Windows7 },
                    { (6, 2), OSVersions.Windows8 },
                    { (6, 3), OSVersions.Windows8Point1 },
                    { (10, -1), OSVersions.Windows10 }
                }
            }
        };
        OperatingSystem oSVersion = Environment.OSVersion;
        int major = oSVersion.Version.Major;
        int minor = oSVersion.Version.Minor;
        OSVersions result = OSVersions.Unknown;
        if (dictionary.ContainsKey(oSVersion.Platform))
        {
            Dictionary<(int, int), OSVersions> dictionary2 = dictionary[oSVersion.Platform];
            if (dictionary2.ContainsKey((major, minor)))
            {
                result = dictionary2[(major, minor)];
            }
            else if (dictionary2.ContainsKey((major, -1)))
            {
                result = dictionary2[(major, -1)];
            }
            else if (dictionary2.ContainsKey((-1, -1)))
            {
                result = dictionary2[(major, -1)];
            }
        }
        return result;
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
