namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.CommonDragger</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.CommonDragger.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreCommonDraggerMechanicalDifferentialTests
{
    [SkippableFact]
    public void OffsetTo_06000193() => Mechanical.Compare(typeof(global::Quire.Controls.Core.CommonDragger), 0x06000193);

    [SkippableFact]
    public void MouseUp_ResizeRightMode_06000194() => Mechanical.Compare(typeof(global::Quire.Controls.Core.CommonDragger), 0x06000194);

    [SkippableFact]
    public void MouseUp_ResizeBottomMode_06000196() => Mechanical.Compare(typeof(global::Quire.Controls.Core.CommonDragger), 0x06000196);
}
