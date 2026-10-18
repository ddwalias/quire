namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.AssemblyStreamAttribute</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.AssemblyStreamAttribute.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcAssemblyStreamAttributeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000112() => Mechanical.Compare(typeof(global::Quire.AssemblyStreamAttribute), 0x06000112);
}
