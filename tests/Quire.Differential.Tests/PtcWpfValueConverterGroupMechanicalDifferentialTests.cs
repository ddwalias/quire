namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.ValueConverterGroup</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.ValueConverterGroup.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfValueConverterGroupMechanicalDifferentialTests
{
    [SkippableFact]
    public void ConvertBack_06000490() => Mechanical.Compare(typeof(global::Quire.Wpf.ValueConverterGroup), 0x06000490);

    [SkippableFact]
    public void ctor_06000491() => Mechanical.Compare(typeof(global::Quire.Wpf.ValueConverterGroup), 0x06000491);
}
