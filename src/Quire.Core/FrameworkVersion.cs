#nullable disable
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
}
