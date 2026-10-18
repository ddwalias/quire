namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.UnsafeNativeMethods</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.UnsafeNativeMethods.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfUnsafeNativeMethodsMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060005c3() => Mechanical.Compare(typeof(global::Quire.Wpf.UnsafeNativeMethods), 0x060005c3);
}
