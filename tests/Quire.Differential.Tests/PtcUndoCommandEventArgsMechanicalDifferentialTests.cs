namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Undo.CommandEventArgs</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Undo.CommandEventArgs.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcUndoCommandEventArgsMechanicalDifferentialTests
{
    [SkippableFact]
    public void IsCommandAlive_get_060006d3() => Mechanical.Compare(typeof(global::Quire.Undo.CommandEventArgs), 0x060006d3);

    [SkippableFact]
    public void ActionType_get_060006d5() => Mechanical.Compare(typeof(global::Quire.Undo.CommandEventArgs), 0x060006d5);
}
