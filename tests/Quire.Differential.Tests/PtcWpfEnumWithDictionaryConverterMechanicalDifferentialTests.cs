namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.EnumWithDictionaryConverter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.EnumWithDictionaryConverter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfEnumWithDictionaryConverterMechanicalDifferentialTests
{
    [SkippableFact]
    public void ConvertBack_06000410() => Mechanical.Compare(typeof(global::Quire.Wpf.EnumWithDictionaryConverter), 0x06000410);

    [SkippableFact]
    public void ctor_06000411() => Mechanical.Compare(typeof(global::Quire.Wpf.EnumWithDictionaryConverter), 0x06000411);
}
