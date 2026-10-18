#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class ApplicationSettingsSafe
{
    [MirrorVerified("PtcApplicationSettingsSafeMechanicalDifferentialTests")]
    protected ApplicationSettingsSafe() { }

    [MirrorVerified("PtcApplicationSettingsSafeMechanicalDifferentialTests")]
    private static Dictionary<string, object> FallbackValues { get; }
}
