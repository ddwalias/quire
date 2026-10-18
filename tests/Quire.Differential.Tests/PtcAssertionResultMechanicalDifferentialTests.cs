namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.AssertionResult</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.AssertionResult.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcAssertionResultMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000122() => Mechanical.Compare(typeof(global::Quire.AssertionResult), 0x06000122);

    [SkippableFact]
    public void Passed_get_06000123() => Mechanical.Compare(typeof(global::Quire.AssertionResult), 0x06000123);
}
