#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class CalculatingStatusChangedEventArgs
{
    [MirrorVerified("PtcCalculationCoreCalculatingStatusChangedEventArgsMechanicalDifferentialTests")]
    private static CalculatingStatusChangedEventArgs _singleton;

    [MirrorVerified("PtcCalculationCoreCalculatingStatusChangedEventArgsMechanicalDifferentialTests")]
    public static CalculatingStatusChangedEventArgs GetArgs() => _singleton;

    [MirrorVerified("PtcCalculationCoreCalculatingStatusChangedEventArgsMechanicalDifferentialTests")]
    public CalculatingStatusChangedEventArgs() : base("CalculatingStatus") { }
}
