namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Serialization.TextRegionSerializationHelper</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Serialization.TextRegionSerializationHelper.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcSerializationTextRegionSerializationHelperMechanicalDifferentialTests
{
    [SkippableFact]
    public void SaveTextRangeAsXamlPackage_0600080d() => Mechanical.Compare(typeof(global::Quire.Serialization.TextRegionSerializationHelper), 0x0600080d);

    [SkippableFact]
    public void SelectNodes_06000810() => Mechanical.Compare(typeof(global::Quire.Serialization.TextRegionSerializationHelper), 0x06000810);
}
