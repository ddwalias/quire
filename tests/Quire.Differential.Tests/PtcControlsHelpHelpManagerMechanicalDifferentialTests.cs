namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Help.HelpManager</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Help.HelpManager.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsHelpHelpManagerMechanicalDifferentialTests
{
    [SkippableFact]
    public void ActiveToolTipUid_get_060002be() => Mechanical.Compare(typeof(global::Quire.Controls.Help.HelpManager), 0x060002be);

    [SkippableFact]
    public void ActiveToolTipUid_set_060002bf() => Mechanical.Compare(typeof(global::Quire.Controls.Help.HelpManager), 0x060002bf);

    [SkippableFact]
    public void PropertyChanged_add_060002c9() => Mechanical.Compare(typeof(global::Quire.Controls.Help.HelpManager), 0x060002c9);

    [SkippableFact]
    public void PropertyChanged_remove_060002ca() => Mechanical.Compare(typeof(global::Quire.Controls.Help.HelpManager), 0x060002ca);
}
