namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.DragEventArgs</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.DragEventArgs.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreDragEventArgsMechanicalDifferentialTests
{
    [SkippableFact]
    public void DraggingVector_get_060007ad() => Mechanical.Compare(typeof(global::Quire.Controls.Core.DragEventArgs), 0x060007ad);

    [SkippableFact]
    public void DraggingVector_set_060007ae() => Mechanical.Compare(typeof(global::Quire.Controls.Core.DragEventArgs), 0x060007ae);
}
