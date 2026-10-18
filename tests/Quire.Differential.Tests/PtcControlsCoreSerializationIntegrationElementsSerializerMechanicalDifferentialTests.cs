namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.Serialization.IntegrationElementsSerializer</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.Serialization.IntegrationElementsSerializer.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreSerializationIntegrationElementsSerializerMechanicalDifferentialTests
{
    [SkippableFact]
    public void RepairXDocument_06000aab() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.IntegrationElementsSerializer), 0x06000aab);

    [SkippableFact]
    public void ShouldValidateOnWrite_get_06000aa9() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.IntegrationElementsSerializer), 0x06000aa9);

    [SkippableFact]
    public void ShouldValidateOnRead_get_06000aaa() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.IntegrationElementsSerializer), 0x06000aaa);
}
