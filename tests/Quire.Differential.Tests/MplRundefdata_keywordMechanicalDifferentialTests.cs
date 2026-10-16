namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Mpl.Run.defdata_keyword</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Mpl.Run.defdata_keyword.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MplRundefdata_keywordMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060000a1() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.defdata_keyword), 0x060000a1);

    [SkippableFact]
    public void walk_declaration_as_060000a3() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.defdata_keyword), 0x060000a3);

    [SkippableFact]
    public void walk_definition_as_060000a4() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.defdata_keyword), 0x060000a4);
}
