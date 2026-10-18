namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.WorksheetPropertiesManager</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.WorksheetPropertiesManager.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests
{
    [SkippableFact]
    public void SetWorksheetPropertiesSource_060008fb() => Mechanical.Compare(typeof(global::Quire.Controls.Core.WorksheetPropertiesManager), 0x060008fb);

    [SkippableFact]
    public void SetWorksheetPropertiesChangeInfo_060008fd() => Mechanical.Compare(typeof(global::Quire.Controls.Core.WorksheetPropertiesManager), 0x060008fd);

    [SkippableFact]
    public void WorksheetFrameworkElement_get_06001074() => Mechanical.Compare(typeof(global::Quire.Controls.Core.WorksheetPropertiesManager), 0x06001074);

    [SkippableFact]
    public void WorksheetFrameworkElement_set_06001075() => Mechanical.Compare(typeof(global::Quire.Controls.Core.WorksheetPropertiesManager), 0x06001075);
}
