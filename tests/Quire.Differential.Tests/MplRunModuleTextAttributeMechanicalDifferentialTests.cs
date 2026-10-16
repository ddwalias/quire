namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Mpl.Run.ModuleTextAttribute</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Mpl.Run.ModuleTextAttribute.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MplRunModuleTextAttributeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ToString_06000007() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.ModuleTextAttribute), 0x06000007);

    [SkippableFact]
    public void ctor_06000008() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.ModuleTextAttribute), 0x06000008);

    [SkippableFact]
    public void Text_get_06000006() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.ModuleTextAttribute), 0x06000006);
}
