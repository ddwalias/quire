namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Services.ApplicationBehaviorOptions</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Services.ApplicationBehaviorOptions.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcServicesApplicationBehaviorOptionsMechanicalDifferentialTests
{
    [SkippableFact]
    public void CollectInputOutputStatesTimeout_get_06000746() => Mechanical.Compare(typeof(global::Quire.Services.ApplicationBehaviorOptions), 0x06000746);

    [SkippableFact]
    public void CollectInputOutputStatesTimeout_set_06000747() => Mechanical.Compare(typeof(global::Quire.Services.ApplicationBehaviorOptions), 0x06000747);

    [SkippableFact]
    public void IsConnected_get_06000748() => Mechanical.Compare(typeof(global::Quire.Services.ApplicationBehaviorOptions), 0x06000748);

    [SkippableFact]
    public void IsConnected_set_06000749() => Mechanical.Compare(typeof(global::Quire.Services.ApplicationBehaviorOptions), 0x06000749);
}
