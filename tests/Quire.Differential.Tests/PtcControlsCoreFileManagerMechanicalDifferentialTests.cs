namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.FileManager</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.FileManager.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreFileManagerMechanicalDifferentialTests
{
    [SkippableFact]
    public void IsPackage_0600024f() => Mechanical.Compare(typeof(global::Quire.Controls.Core.FileManager), 0x0600024f);

    [SkippableFact]
    public void ctor_06000254() => Mechanical.Compare(typeof(global::Quire.Controls.Core.FileManager), 0x06000254);

    [SkippableFact]
    public void ctor_06000f9d() => Mechanical.Compare(typeof(global::Quire.Controls.Core.FileManager), 0x06000f9d);
}
