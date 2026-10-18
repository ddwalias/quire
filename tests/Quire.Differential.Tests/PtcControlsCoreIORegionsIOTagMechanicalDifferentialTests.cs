namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.IORegions.IOTag</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.IORegions.IOTag.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreIORegionsIOTagMechanicalDifferentialTests
{
    [SkippableFact]
    public void ClearIOTag_06000e80() => Mechanical.Compare(typeof(global::Quire.Controls.Core.IORegions.IOTag), 0x06000e80);

    [SkippableFact]
    public void HasInputTag_06000e84() => Mechanical.Compare(typeof(global::Quire.Controls.Core.IORegions.IOTag), 0x06000e84);

    [SkippableFact]
    public void HasOutputTag_06000e85() => Mechanical.Compare(typeof(global::Quire.Controls.Core.IORegions.IOTag), 0x06000e85);

    [SkippableFact]
    public void IOTagSet_add_06000e7a() => Mechanical.Compare(typeof(global::Quire.Controls.Core.IORegions.IOTag), 0x06000e7a);

    [SkippableFact]
    public void IOTagSet_remove_06000e7b() => Mechanical.Compare(typeof(global::Quire.Controls.Core.IORegions.IOTag), 0x06000e7b);

    [SkippableFact]
    public void IOTagUnSet_add_06000e7c() => Mechanical.Compare(typeof(global::Quire.Controls.Core.IORegions.IOTag), 0x06000e7c);

    [SkippableFact]
    public void IOTagUnSet_remove_06000e7d() => Mechanical.Compare(typeof(global::Quire.Controls.Core.IORegions.IOTag), 0x06000e7d);
}
