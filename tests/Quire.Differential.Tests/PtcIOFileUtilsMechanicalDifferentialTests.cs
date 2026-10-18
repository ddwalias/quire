namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.IO.FileUtils</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.IO.FileUtils.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcIOFileUtilsMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetLocalConfigFolderPath_06000f9d() => Mechanical.Compare(typeof(global::Quire.IO.FileUtils), 0x06000f9d);

    [SkippableFact]
    public void GetCommonConfigFolderPath_06000f9e() => Mechanical.Compare(typeof(global::Quire.IO.FileUtils), 0x06000f9e);

    [SkippableFact]
    public void ctor_06000fa7() => Mechanical.Compare(typeof(global::Quire.IO.FileUtils), 0x06000fa7);
}
