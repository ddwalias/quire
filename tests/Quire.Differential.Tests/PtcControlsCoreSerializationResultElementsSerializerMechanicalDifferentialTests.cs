namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.Serialization.ResultElementsSerializer</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.Serialization.ResultElementsSerializer.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreSerializationResultElementsSerializerMechanicalDifferentialTests
{
    [SkippableFact]
    public void RepairXDocument_06000bb1() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.ResultElementsSerializer), 0x06000bb1);

    [SkippableFact]
    public void ShouldValidateOnWrite_get_06000baf() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.ResultElementsSerializer), 0x06000baf);

    [SkippableFact]
    public void ShouldValidateOnRead_get_06000bb0() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.ResultElementsSerializer), 0x06000bb0);
}
