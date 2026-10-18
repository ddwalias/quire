#nullable disable
using Quire.Mirror;

namespace Quire;

public static partial class DevelopmentEnvironmentState
{
    [MirrorVerified("PtcDevelopmentEnvironmentStateMechanicalDifferentialTests")]
    private static readonly Dictionary<string, bool> RtStateCache;

    [MirrorVerified("PtcDevelopmentEnvironmentStateMechanicalDifferentialTests")]
    public static void ClearRtStateCache() => RtStateCache.Clear();

    [MirrorVerified("PtcDevelopmentEnvironmentStateMechanicalDifferentialTests")]
    public static void EnableRtInCache(string env) => RtStateCache[env] = true;

    [MirrorVerified("PtcDevelopmentEnvironmentStateMechanicalDifferentialTests")]
    public static List<string> FelinesRtEnvs { get; set; }
}
