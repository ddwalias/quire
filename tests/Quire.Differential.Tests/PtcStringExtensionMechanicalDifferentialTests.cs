namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.StringExtension</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.StringExtension.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcStringExtensionMechanicalDifferentialTests
{
    [SkippableFact]
    public void StripInvalidXmlChars_06000273() => Mechanical.Compare(typeof(global::Quire.StringExtension), 0x06000273);

    [SkippableFact]
    public void IsValidXmlString_06000274() => Mechanical.Compare(typeof(global::Quire.StringExtension), 0x06000274);
}
