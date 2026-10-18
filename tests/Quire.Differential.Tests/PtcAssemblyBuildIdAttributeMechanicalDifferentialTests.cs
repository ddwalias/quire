namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.AssemblyBuildIdAttribute</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.AssemblyBuildIdAttribute.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcAssemblyBuildIdAttributeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000111() => Mechanical.Compare(typeof(global::Quire.AssemblyBuildIdAttribute), 0x06000111);
}
