namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Core.MessageBoxTraceListener</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Core.MessageBoxTraceListener.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests
{
    [SkippableFact]
    public void Fail_06001009() => Mechanical.Compare(typeof(global::Quire.Core.MessageBoxTraceListener), 0x06001009);

    [SkippableFact]
    public void Write_0600100b() => Mechanical.Compare(typeof(global::Quire.Core.MessageBoxTraceListener), 0x0600100b);

    [SkippableFact]
    public void WriteLine_0600100c() => Mechanical.Compare(typeof(global::Quire.Core.MessageBoxTraceListener), 0x0600100c);

    [SkippableFact]
    public void ctor_0600100d() => Mechanical.Compare(typeof(global::Quire.Core.MessageBoxTraceListener), 0x0600100d);
}
