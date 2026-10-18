namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.IO.CollectionWithSyncStatus</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.IO.CollectionWithSyncStatus.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsIOCollectionWithSyncStatusMechanicalDifferentialTests
{
    [SkippableFact]
    public void Synchronized_add_0600020e() => Mechanical.Compare(typeof(global::Quire.Controls.IO.CollectionWithSyncStatus), 0x0600020e);

    [SkippableFact]
    public void Synchronized_remove_0600020f() => Mechanical.Compare(typeof(global::Quire.Controls.IO.CollectionWithSyncStatus), 0x0600020f);
}
