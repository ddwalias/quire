namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.DummyConverter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.DummyConverter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfDummyConverterMechanicalDifferentialTests
{
    [SkippableFact]
    public void Convert_06000469() => Mechanical.Compare(typeof(global::Quire.Wpf.DummyConverter), 0x06000469);

    [SkippableFact]
    public void ConvertBack_0600046a() => Mechanical.Compare(typeof(global::Quire.Wpf.DummyConverter), 0x0600046a);

    [SkippableFact]
    public void ctor_0600046b() => Mechanical.Compare(typeof(global::Quire.Wpf.DummyConverter), 0x0600046b);
}
