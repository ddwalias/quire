namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.CheckBoxData</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.CheckBoxData.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfCheckBoxDataMechanicalDifferentialTests
{
    [SkippableFact]
    public void Message_get_0600051b() => Mechanical.Compare(typeof(global::Quire.Wpf.CheckBoxData), 0x0600051b);

    [SkippableFact]
    public void Message_set_0600051c() => Mechanical.Compare(typeof(global::Quire.Wpf.CheckBoxData), 0x0600051c);

    [SkippableFact]
    public void PropertyChanged_add_06000520() => Mechanical.Compare(typeof(global::Quire.Wpf.CheckBoxData), 0x06000520);

    [SkippableFact]
    public void PropertyChanged_remove_06000521() => Mechanical.Compare(typeof(global::Quire.Wpf.CheckBoxData), 0x06000521);
}
