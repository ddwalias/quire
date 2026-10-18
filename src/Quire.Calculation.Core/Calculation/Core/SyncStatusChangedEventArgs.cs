#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class SyncStatusChangedEventArgs
{
    [MirrorVerified("PtcCalculationCoreSyncStatusChangedEventArgsMechanicalDifferentialTests")]
    private static SyncStatusChangedEventArgs _singleton;

    [MirrorVerified("PtcCalculationCoreSingletonCctorsDifferentialTests")]
    static SyncStatusChangedEventArgs() => _singleton = new SyncStatusChangedEventArgs();

    [MirrorVerified("PtcCalculationCoreSyncStatusChangedEventArgsMechanicalDifferentialTests")]
    public static SyncStatusChangedEventArgs GetArgs() => _singleton;

    [MirrorVerified("PtcCalculationCoreSyncStatusChangedEventArgsMechanicalDifferentialTests")]
    public SyncStatusChangedEventArgs() : base("SynchronizationStatus") { }
}
