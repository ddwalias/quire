namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Win32.User32</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Win32.User32.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWin32User32MechanicalDifferentialTests
{
    [SkippableFact]
    public void op_Implicit_06001318() => Mechanical.Compare(typeof(global::Quire.Win32.User32), 0x06001318);
}
