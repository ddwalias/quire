namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.StringFormatConverter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.StringFormatConverter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfStringFormatConverterMechanicalDifferentialTests
{
    [SkippableFact]
    public void ConvertBack_06000439() => Mechanical.Compare(typeof(global::Quire.Wpf.StringFormatConverter), 0x06000439);

    [SkippableFact]
    public void ctor_0600043a() => Mechanical.Compare(typeof(global::Quire.Wpf.StringFormatConverter), 0x0600043a);
}
