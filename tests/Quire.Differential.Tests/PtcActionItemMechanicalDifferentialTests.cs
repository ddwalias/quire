namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.ActionItem</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.ActionItem.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcActionItemMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060001f3() => Mechanical.Compare(typeof(global::Quire.ActionItem), 0x060001f3);
}
