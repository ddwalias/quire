namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.IO.IOItemCollection</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.IO.IOItemCollection.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsIOIOItemCollectionMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetItem_06000253() => Mechanical.Compare(typeof(global::Quire.Controls.IO.IOItemCollection), 0x06000253);

    [SkippableFact]
    public void HandleTaggedRegionAdded_06000255() => Mechanical.Compare(typeof(global::Quire.Controls.IO.IOItemCollection), 0x06000255);

    [SkippableFact]
    public void AliasChanged_add_0600024f() => Mechanical.Compare(typeof(global::Quire.Controls.IO.IOItemCollection), 0x0600024f);

    [SkippableFact]
    public void AliasChanged_remove_06000250() => Mechanical.Compare(typeof(global::Quire.Controls.IO.IOItemCollection), 0x06000250);
}
