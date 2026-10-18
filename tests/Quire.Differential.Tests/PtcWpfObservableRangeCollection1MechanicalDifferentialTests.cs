namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.ObservableRangeCollection`1</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.ObservableRangeCollection`1.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfObservableRangeCollection1MechanicalDifferentialTests
{
    [SkippableFact]
    public void RangeChanging_add_06000550() => Mechanical.Compare(typeof(global::Quire.Wpf.ObservableRangeCollection<>), 0x06000550);

    [SkippableFact]
    public void RangeChanging_remove_06000551() => Mechanical.Compare(typeof(global::Quire.Wpf.ObservableRangeCollection<>), 0x06000551);

    [SkippableFact]
    public void RangeChanged_add_06000552() => Mechanical.Compare(typeof(global::Quire.Wpf.ObservableRangeCollection<>), 0x06000552);

    [SkippableFact]
    public void RangeChanged_remove_06000553() => Mechanical.Compare(typeof(global::Quire.Wpf.ObservableRangeCollection<>), 0x06000553);
}
