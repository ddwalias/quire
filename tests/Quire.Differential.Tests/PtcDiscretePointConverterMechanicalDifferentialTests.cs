namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.DiscretePointConverter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.DiscretePointConverter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcDiscretePointConverterMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060001b7() => Mechanical.Compare(typeof(global::Quire.DiscretePointConverter), 0x060001b7);
}
