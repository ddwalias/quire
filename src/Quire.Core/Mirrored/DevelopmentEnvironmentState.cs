#nullable disable
#pragma warning disable

namespace Quire;

static partial class DevelopmentEnvironmentState
{
    [global::Quire.Mirror.MirrorVerified("PtcDevelopmentEnvironmentStateMechanicalDifferentialTests")]
    private static readonly global::System.Collections.Generic.Dictionary<string, bool> RtStateCache = new();
    public const string EnableRtEnvValue = "true";
    public static readonly global::System.Collections.Generic.Dictionary<string, global::System.Func<bool>> RtNameToWrapperFuncMap = new();
    public static bool CheckRtEnvState(string env)
    {
        try
        {
            string environmentVariable = global::System.Environment.GetEnvironmentVariable(env, global::System.EnvironmentVariableTarget.Process);
            return !string.IsNullOrEmpty(environmentVariable) && environmentVariable.Equals("true", global::System.StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
        }

        return false;
    }
    public static bool CheckRtEnvStateCached(string env)
    {
        if (!RtStateCache.ContainsKey(env))
        {
            RtStateCache[env] = CheckRtEnvState(env);
        }

        return RtStateCache[env];
    }
}
