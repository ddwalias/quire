namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Mpl.Run.declare_keyword</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Mpl.Run.declare_keyword.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MplRundeclare_keywordMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060000be() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.declare_keyword), 0x060000be);

    [SkippableFact]
    public void walk_definition_as_060000c1() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.declare_keyword), 0x060000c1);
}
