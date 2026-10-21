#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class ApplicationSettingsSafe
{
    [MirrorVerified("PtcApplicationSettingsSafeMechanicalDifferentialTests")]
    protected ApplicationSettingsSafe() { }

    [MirrorVerified("PtcApplicationSettingsSafeMechanicalDifferentialTests")]
    private static Dictionary<string, object> FallbackValues { get; }

    public new void Reset()
    {
        try
        {
            base.Reset();
            FallbackValues.Clear();
        }
        catch (global::System.Configuration.ConfigurationErrorsException ex)
        {
            DebugTools.FailAssertion(ex);
        }
    }

    public new void Reload()
    {
        try
        {
            base.Reload();
            FallbackValues.Clear();
        }
        catch (global::System.Configuration.ConfigurationErrorsException ex)
        {
            DebugTools.FailAssertion(ex);
        }
    }
}
