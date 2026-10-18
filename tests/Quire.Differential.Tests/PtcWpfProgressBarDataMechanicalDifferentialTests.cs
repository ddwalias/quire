namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.ProgressBarData</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.ProgressBarData.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfProgressBarDataMechanicalDifferentialTests
{
    [SkippableFact]
    public void MessageFormat_get_0600052c() => Mechanical.Compare(typeof(global::Quire.Wpf.ProgressBarData), 0x0600052c);

    [SkippableFact]
    public void MessageFormat_set_0600052d() => Mechanical.Compare(typeof(global::Quire.Wpf.ProgressBarData), 0x0600052d);

    [SkippableFact]
    public void ProgressNotifier_get_0600052e() => Mechanical.Compare(typeof(global::Quire.Wpf.ProgressBarData), 0x0600052e);

    [SkippableFact]
    public void ProgressNotifier_set_0600052f() => Mechanical.Compare(typeof(global::Quire.Wpf.ProgressBarData), 0x0600052f);
}
