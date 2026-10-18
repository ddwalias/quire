namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.HasProtectionLevelConverter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.HasProtectionLevelConverter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcHasProtectionLevelConverterMechanicalDifferentialTests
{
    [SkippableFact]
    public void ConvertBack_0600024a() => Mechanical.Compare(typeof(global::Quire.HasProtectionLevelConverter), 0x0600024a);

    [SkippableFact]
    public void ctor_0600024b() => Mechanical.Compare(typeof(global::Quire.HasProtectionLevelConverter), 0x0600024b);
}
