namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.DebugTools</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.DebugTools.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcDebugToolsMechanicalDifferentialTests
{
    [SkippableFact]
    public void FailAssertion_06000136() => Mechanical.Compare(typeof(global::Quire.DebugTools), 0x06000136);

    [SkippableFact]
    public void FailAssertionDoNotThrow_06000137() => Mechanical.Compare(typeof(global::Quire.DebugTools), 0x06000137);

    [SkippableFact]
    public void DevAssert_06000139() => Mechanical.Compare(typeof(global::Quire.DebugTools), 0x06000139);

    [SkippableFact]
    public void ReportAssertionFailureOnAnotherThread_0600013b() => Mechanical.Compare(typeof(global::Quire.DebugTools), 0x0600013b);

    [SkippableFact]
    public void LogError_0600013e() => Mechanical.Compare(typeof(global::Quire.DebugTools), 0x0600013e);

    [SkippableFact]
    public void LogMessage_0600013f() => Mechanical.Compare(typeof(global::Quire.DebugTools), 0x0600013f);

    [SkippableFact]
    public void ReplaceAssertionWithException_get_06000131() => Mechanical.Compare(typeof(global::Quire.DebugTools), 0x06000131);

    [SkippableFact]
    public void ReplaceAssertionWithException_set_06000132() => Mechanical.Compare(typeof(global::Quire.DebugTools), 0x06000132);

    [SkippableFact]
    public void IsDebug_get_06000140() => Mechanical.Compare(typeof(global::Quire.DebugTools), 0x06000140);

    [SkippableFact]
    public void DebugVerificationsAllowed_get_06000141() => Mechanical.Compare(typeof(global::Quire.DebugTools), 0x06000141);

    [SkippableFact]
    public void DebugVerificationsAllowed_set_06000142() => Mechanical.Compare(typeof(global::Quire.DebugTools), 0x06000142);
}
