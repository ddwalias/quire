namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.SyncStatusChangedEventArgs</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.SyncStatusChangedEventArgs.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreSyncStatusChangedEventArgsMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetArgs_060000b6() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SyncStatusChangedEventArgs), 0x060000b6);

    [SkippableFact]
    public void ctor_060000b7() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SyncStatusChangedEventArgs), 0x060000b7);
}
