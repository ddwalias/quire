namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.EnumerableExtensions</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.EnumerableExtensions.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcEnumerableExtensionsMechanicalDifferentialTests
{
    [SkippableFact]
    public void Collect_060001df() => Mechanical.Compare(typeof(global::Quire.EnumerableExtensions), 0x060001df);
}
