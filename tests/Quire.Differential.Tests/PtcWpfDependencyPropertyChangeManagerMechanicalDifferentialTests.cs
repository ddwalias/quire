namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.DependencyPropertyChangeManager</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.DependencyPropertyChangeManager.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfDependencyPropertyChangeManagerMechanicalDifferentialTests
{
    [SkippableFact]
    public void SourcePropertyChanged_add_06001184() => Mechanical.Compare(typeof(global::Quire.Wpf.DependencyPropertyChangeManager), 0x06001184);

    [SkippableFact]
    public void SourcePropertyChanged_remove_06001185() => Mechanical.Compare(typeof(global::Quire.Wpf.DependencyPropertyChangeManager), 0x06001185);
}
