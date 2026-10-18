namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.WorksheetPropertyNames</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.WorksheetPropertyNames.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreWorksheetPropertyNamesMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetNames_060008b5() => Mechanical.Compare(typeof(global::Quire.Controls.Core.WorksheetPropertyNames), 0x060008b5);

    [SkippableFact]
    public void GetTypeOfWorksheetProperties_060008b6() => Mechanical.Compare(typeof(global::Quire.Controls.Core.WorksheetPropertyNames), 0x060008b6);
}
