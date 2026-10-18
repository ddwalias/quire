namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.ScaleConverter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.ScaleConverter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfScaleConverterMechanicalDifferentialTests
{
    [SkippableFact]
    public void ConvertBack_06000473() => Mechanical.Compare(typeof(global::Quire.Wpf.ScaleConverter), 0x06000473);

    [SkippableFact]
    public void ctor_06000475() => Mechanical.Compare(typeof(global::Quire.Wpf.ScaleConverter), 0x06000475);
}
