namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.ImagePackagingInformation</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.ImagePackagingInformation.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfImagePackagingInformationMechanicalDifferentialTests
{
    [SkippableFact]
    public void ImageNameFormat_get_06000516() => Mechanical.Compare(typeof(global::Quire.Wpf.ImagePackagingInformation), 0x06000516);

    [SkippableFact]
    public void ImageNameFormat_set_06000515() => Mechanical.Compare(typeof(global::Quire.Wpf.ImagePackagingInformation), 0x06000515);

    [SkippableFact]
    public void ImageId_get_06000518() => Mechanical.Compare(typeof(global::Quire.Wpf.ImagePackagingInformation), 0x06000518);

    [SkippableFact]
    public void ImageId_set_06000517() => Mechanical.Compare(typeof(global::Quire.Wpf.ImagePackagingInformation), 0x06000517);
}
