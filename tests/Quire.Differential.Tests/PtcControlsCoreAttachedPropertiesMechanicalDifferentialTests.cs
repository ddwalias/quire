namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.AttachedProperties</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.AttachedProperties.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreAttachedPropertiesMechanicalDifferentialTests
{
    [SkippableFact]
    public void ClearCanActivate_0600015c() => Mechanical.Compare(typeof(global::Quire.Controls.Core.AttachedProperties), 0x0600015c);

    [SkippableFact]
    public void ClearHandlesContextMenu_0600015f() => Mechanical.Compare(typeof(global::Quire.Controls.Core.AttachedProperties), 0x0600015f);
}
