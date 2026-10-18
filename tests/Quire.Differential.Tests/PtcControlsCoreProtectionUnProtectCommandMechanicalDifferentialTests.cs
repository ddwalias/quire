namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.Protection.UnProtectCommand</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.Protection.UnProtectCommand.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreProtectionUnProtectCommandMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000c2d() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Protection.UnProtectCommand), 0x06000c2d);

    [SkippableFact]
    public void Undo_06000c2f() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Protection.UnProtectCommand), 0x06000c2f);

    [SkippableFact]
    public void Redo_06000c30() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Protection.UnProtectCommand), 0x06000c30);

    [SkippableFact]
    public void Name_get_06000c31() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Protection.UnProtectCommand), 0x06000c31);
}
