namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.IsNullConverter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.IsNullConverter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfIsNullConverterMechanicalDifferentialTests
{
    [SkippableFact]
    public void ConvertBack_06000470() => Mechanical.Compare(typeof(global::Quire.Wpf.IsNullConverter), 0x06000470);

    [SkippableFact]
    public void ctor_06000471() => Mechanical.Compare(typeof(global::Quire.Wpf.IsNullConverter), 0x06000471);
}
