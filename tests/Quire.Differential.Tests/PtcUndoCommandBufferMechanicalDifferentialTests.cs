namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Undo.CommandBuffer</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Undo.CommandBuffer.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcUndoCommandBufferMechanicalDifferentialTests
{
    [SkippableFact]
    public void CurrentInstance_get_060006bb() => Mechanical.Compare(typeof(global::Quire.Undo.CommandBuffer), 0x060006bb);

    [SkippableFact]
    public void CurrentInstance_set_060006bc() => Mechanical.Compare(typeof(global::Quire.Undo.CommandBuffer), 0x060006bc);

    [SkippableFact]
    public void UndoStackDepth_get_060006c6() => Mechanical.Compare(typeof(global::Quire.Undo.CommandBuffer), 0x060006c6);

    [SkippableFact]
    public void BeforeCommand_add_060006c9() => Mechanical.Compare(typeof(global::Quire.Undo.CommandBuffer), 0x060006c9);

    [SkippableFact]
    public void BeforeCommand_remove_060006ca() => Mechanical.Compare(typeof(global::Quire.Undo.CommandBuffer), 0x060006ca);

    [SkippableFact]
    public void AfterCommand_add_060006cb() => Mechanical.Compare(typeof(global::Quire.Undo.CommandBuffer), 0x060006cb);

    [SkippableFact]
    public void AfterCommand_remove_060006cc() => Mechanical.Compare(typeof(global::Quire.Undo.CommandBuffer), 0x060006cc);

    [SkippableFact]
    public void Do_0600131f() => Mechanical.Compare(typeof(global::Quire.Undo.CommandBuffer), 0x0600131f);

    [SkippableFact]
    public void Top_get_06001322() => Mechanical.Compare(typeof(global::Quire.Undo.CommandBuffer), 0x06001322);
}
