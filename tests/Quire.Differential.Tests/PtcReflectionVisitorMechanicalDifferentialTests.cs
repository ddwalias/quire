namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.ReflectionVisitor</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.ReflectionVisitor.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcReflectionVisitorMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_0600037e() => Mechanical.Compare(typeof(global::Quire.ReflectionVisitor), 0x0600037e);
}
