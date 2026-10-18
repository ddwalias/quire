namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.IO.InputOutputItemsConflicts</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.IO.InputOutputItemsConflicts.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsIOInputOutputItemsConflictsMechanicalDifferentialTests
{
    [SkippableFact]
    public void GeneralWarnings_get_06000240() => Mechanical.Compare(typeof(global::Quire.Controls.IO.InputOutputItemsConflicts), 0x06000240);

    [SkippableFact]
    public void GeneralErrors_get_06000241() => Mechanical.Compare(typeof(global::Quire.Controls.IO.InputOutputItemsConflicts), 0x06000241);

    [SkippableFact]
    public void ItemWarnings_get_06000242() => Mechanical.Compare(typeof(global::Quire.Controls.IO.InputOutputItemsConflicts), 0x06000242);

    [SkippableFact]
    public void ItemErrors_get_06000243() => Mechanical.Compare(typeof(global::Quire.Controls.IO.InputOutputItemsConflicts), 0x06000243);
}
