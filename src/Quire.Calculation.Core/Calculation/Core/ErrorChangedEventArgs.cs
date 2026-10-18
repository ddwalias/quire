#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class ErrorChangedEventArgs
{
    [MirrorVerified("PtcCalculationCoreErrorChangedEventArgsMechanicalDifferentialTests")]
    private static ErrorChangedEventArgs _singleton;

    [MirrorVerified("PtcCalculationCoreErrorChangedEventArgsMechanicalDifferentialTests")]
    public static ErrorChangedEventArgs GetArgs() => _singleton;

    [MirrorVerified("PtcCalculationCoreErrorChangedEventArgsMechanicalDifferentialTests")]
    public ErrorChangedEventArgs() : base("Error") { }
}
