namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.DoubleExtension</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.DoubleExtension.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcDoubleExtensionMechanicalDifferentialTests
{
    [SkippableFact]
    public void Round_0600026c() => Mechanical.Compare(typeof(global::Quire.DoubleExtension), 0x0600026c);
}
