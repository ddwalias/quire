namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Services.ApplicationLogger</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Services.ApplicationLogger.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcServicesApplicationLoggerMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetLastApplicationLogPath_06000769() => Mechanical.Compare(typeof(global::Quire.Services.ApplicationLogger), 0x06000769);

    [SkippableFact]
    public void IsConnected_get_06000772() => Mechanical.Compare(typeof(global::Quire.Services.ApplicationLogger), 0x06000772);

    [SkippableFact]
    public void IsConnected_set_06000773() => Mechanical.Compare(typeof(global::Quire.Services.ApplicationLogger), 0x06000773);
}
