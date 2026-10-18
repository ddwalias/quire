namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.EnvironmentVariableConverter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.EnvironmentVariableConverter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfEnvironmentVariableConverterMechanicalDifferentialTests
{
    [SkippableFact]
    public void ConvertBack_060003fd() => Mechanical.Compare(typeof(global::Quire.Wpf.EnvironmentVariableConverter), 0x060003fd);

    [SkippableFact]
    public void ctor_060003fe() => Mechanical.Compare(typeof(global::Quire.Wpf.EnvironmentVariableConverter), 0x060003fe);
}
