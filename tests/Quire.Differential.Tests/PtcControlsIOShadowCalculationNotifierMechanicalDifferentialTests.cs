namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.IO.ShadowCalculationNotifier</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.IO.ShadowCalculationNotifier.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsIOShadowCalculationNotifierMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060002b3() => Mechanical.Compare(typeof(global::Quire.Controls.IO.ShadowCalculationNotifier), 0x060002b3);

    [SkippableFact]
    public void CollectionChanged_add_060002b1() => Mechanical.Compare(typeof(global::Quire.Controls.IO.ShadowCalculationNotifier), 0x060002b1);

    [SkippableFact]
    public void CollectionChanged_remove_060002b2() => Mechanical.Compare(typeof(global::Quire.Controls.IO.ShadowCalculationNotifier), 0x060002b2);
}
