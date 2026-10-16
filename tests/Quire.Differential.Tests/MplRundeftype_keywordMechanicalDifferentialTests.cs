namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Mpl.Run.deftype_keyword</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Mpl.Run.deftype_keyword.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MplRundeftype_keywordMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060000a6() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.deftype_keyword), 0x060000a6);

    [SkippableFact]
    public void walk_declaration_as_060000a8() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.deftype_keyword), 0x060000a8);

    [SkippableFact]
    public void walk_definition_as_060000a9() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.deftype_keyword), 0x060000a9);
}
