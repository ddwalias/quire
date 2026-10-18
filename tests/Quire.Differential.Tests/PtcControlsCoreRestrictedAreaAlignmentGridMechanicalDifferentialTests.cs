namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.RestrictedAreaAlignmentGrid</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.RestrictedAreaAlignmentGrid.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreRestrictedAreaAlignmentGridMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetNextIntersectionIgnoringRestrictedAreas_0600063a() => Mechanical.Compare(typeof(global::Quire.Controls.Core.RestrictedAreaAlignmentGrid), 0x0600063a);

    [SkippableFact]
    public void GetNearestIntersectionInDirectionIgnoringRestrictedAreas_0600063b() => Mechanical.Compare(typeof(global::Quire.Controls.Core.RestrictedAreaAlignmentGrid), 0x0600063b);

    [SkippableFact]
    public void RestrictedAreas_get_06000632() => Mechanical.Compare(typeof(global::Quire.Controls.Core.RestrictedAreaAlignmentGrid), 0x06000632);

    [SkippableFact]
    public void ProtectedAreas_get_06000633() => Mechanical.Compare(typeof(global::Quire.Controls.Core.RestrictedAreaAlignmentGrid), 0x06000633);
}
