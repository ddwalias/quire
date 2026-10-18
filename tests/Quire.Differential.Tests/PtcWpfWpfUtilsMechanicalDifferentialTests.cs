namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.WpfUtils</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.WpfUtils.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfWpfUtilsMechanicalDifferentialTests
{
    [SkippableFact]
    public void QueueBackgroundTask_060005ef() => Mechanical.Compare(typeof(global::Quire.Wpf.WpfUtils), 0x060005ef);

    [SkippableFact]
    public void Stall_06000605() => Mechanical.Compare(typeof(global::Quire.Wpf.WpfUtils), 0x06000605);

    [SkippableFact]
    public void GetVisualParent_06000614() => Mechanical.Compare(typeof(global::Quire.Wpf.WpfUtils), 0x06000614);
}
