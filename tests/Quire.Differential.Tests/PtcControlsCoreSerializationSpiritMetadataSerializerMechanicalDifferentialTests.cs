namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.Serialization.SpiritMetadataSerializer</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.Serialization.SpiritMetadataSerializer.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreSerializationSpiritMetadataSerializerMechanicalDifferentialTests
{
    [SkippableFact]
    public void CreateSchemas_06000c07() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.SpiritMetadataSerializer), 0x06000c07);

    [SkippableFact]
    public void ctor_06000c0a() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.SpiritMetadataSerializer), 0x06000c0a);
}
