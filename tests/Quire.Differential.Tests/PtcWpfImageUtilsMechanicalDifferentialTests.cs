namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.ImageUtils</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.ImageUtils.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfImageUtilsMechanicalDifferentialTests
{
    [SkippableFact]
    public void CreateBitmapFromVisual_060004f7() => Mechanical.Compare(typeof(global::Quire.Wpf.ImageUtils), 0x060004f7);

    [SkippableFact]
    public void EncodePngImage_06000500() => Mechanical.Compare(typeof(global::Quire.Wpf.ImageUtils), 0x06000500);

    [SkippableFact]
    public void ctor_06000514() => Mechanical.Compare(typeof(global::Quire.Wpf.ImageUtils), 0x06000514);
}
