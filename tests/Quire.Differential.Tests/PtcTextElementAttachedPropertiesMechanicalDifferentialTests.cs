namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.TextElementAttachedProperties</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.TextElementAttachedProperties.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcTextElementAttachedPropertiesMechanicalDifferentialTests
{
    [SkippableFact]
    public void ClearTextStyleData_06000335() => Mechanical.Compare(typeof(global::Quire.TextElementAttachedProperties), 0x06000335);
}
