namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.AlignmentGrid</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.AlignmentGrid.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreAlignmentGridMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000126() => Mechanical.Compare(typeof(global::Quire.Controls.Core.AlignmentGrid), 0x06000126);

    [SkippableFact]
    public void Dispose_06000137() => Mechanical.Compare(typeof(global::Quire.Controls.Core.AlignmentGrid), 0x06000137);
}
