#nullable disable
using System.Linq;
using Microsoft.Win32;
using Quire.Mirror;

namespace Quire;

public static partial class FrameworkVersion
{
    [MirrorVerified("Release IL")]
    private const string RegistryFrameworkVersionsKey = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP";

    [MirrorVerified("Release IL")]
    private const string RegistryFrameworkVersionKey_4 = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full";

    [MirrorVerified("Release IL")]
    private const string RegistryFrameworkVersionRelease = "Release";

    [MirrorVerified("Release IL")]
    private static MajorVersions _majorVersion;

    [MirrorVerified("Release IL")]
    private static int _versionID;

    [MirrorVerified("Release IL")]
    private static List<Pair<Versions, int>> _registryReleaseKeyLegend;

    [MirrorVerified("Release IL")]
    private static void InitializeRegistryReleaseKeyLegendDictionary()
    {
        _registryReleaseKeyLegend.Add(new Pair<Versions, int>(Versions.v45, 378389));
        _registryReleaseKeyLegend.Add(new Pair<Versions, int>(Versions.v451, 378675));
        _registryReleaseKeyLegend.Add(new Pair<Versions, int>(Versions.v451, 378758));
        _registryReleaseKeyLegend.Add(new Pair<Versions, int>(Versions.v452, 379893));
        _registryReleaseKeyLegend.Add(new Pair<Versions, int>(Versions.v452_patched, 379959));
    }

    private static void IdentifyFrameworkVersion()
    {
        _majorVersion = MajorVersions.Undefined;
        _versionID = -1;
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP");
        if ((from x in registryKey.GetSubKeyNames()
              orderby x descending
              select x).First().Substring(1).StartsWith("4"))
        {
            _majorVersion = MajorVersions.Major4;
            registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full");
            if (registryKey != null)
            {
                object value = registryKey.GetValue("Release");
                if (value != null)
                {
                    _versionID = (int)value;
                }
            }
        }
    }

    static FrameworkVersion()
    {
        _registryReleaseKeyLegend = new List<Pair<Versions, int>>(5);
        InitializeRegistryReleaseKeyLegendDictionary();
        IdentifyFrameworkVersion();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.FrameworkVersion+Versions, Ptc.Core")]
    public enum Versions
    {
        v45 = 0,
        v451 = 1,
        v452 = 2,
        v452_patched = 3,
    }

    [global::Quire.Mirror.MirrorOf("Ptc.FrameworkVersion+MajorVersions, Ptc.Core")]
    public enum MajorVersions
    {
        Major4 = 0,
        Undefined = 1,
    }

    public static bool IsGreaterThan(Versions frameworkVersion)
    {
        if (_majorVersion == MajorVersions.Major4)
        {
            return _registryReleaseKeyLegend.Where((Pair<Versions, int> ver) => ver.First == frameworkVersion).All((Pair<Versions, int> ver) => ver.Second < _versionID);
        }
        return false;
    }

    public static bool IsLesserThan(Versions frameworkVersion)
    {
        if (_majorVersion == MajorVersions.Major4)
        {
            return _registryReleaseKeyLegend.Where((Pair<Versions, int> ver) => ver.First == frameworkVersion).All((Pair<Versions, int> ver) => ver.Second > _versionID);
        }
        return false;
    }

    public static bool IsSameAs(Versions frameworkVersion)
    {
        if (_majorVersion == MajorVersions.Major4)
        {
            return _registryReleaseKeyLegend.Any((Pair<Versions, int> ver) => ver.First == frameworkVersion && ver.Second == _versionID);
        }
        return false;
    }
}
