namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.WorksheetSectionDeserializationStrategy</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.WorksheetSectionDeserializationStrategy.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsWorksheetSectionDeserializationStrategyMechanicalDifferentialTests
{
    [SkippableFact]
    public void AddRegion_0600015f() => Mechanical.Compare(typeof(global::Quire.Controls.WorksheetSectionDeserializationStrategy), 0x0600015f);

    [SkippableFact]
    public void DeleteRegion_06000160() => Mechanical.Compare(typeof(global::Quire.Controls.WorksheetSectionDeserializationStrategy), 0x06000160);

    [SkippableFact]
    public void Regions_get_0600015e() => Mechanical.Compare(typeof(global::Quire.Controls.WorksheetSectionDeserializationStrategy), 0x0600015e);
}
