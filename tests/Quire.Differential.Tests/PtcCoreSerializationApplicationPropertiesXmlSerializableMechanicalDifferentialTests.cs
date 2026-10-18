namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Core.Serialization.ApplicationPropertiesXmlSerializable</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Core.Serialization.ApplicationPropertiesXmlSerializable.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCoreSerializationApplicationPropertiesXmlSerializableMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_0600108b() => Mechanical.Compare(typeof(global::Quire.Core.Serialization.ApplicationPropertiesXmlSerializable), 0x0600108b);

    [SkippableFact]
    public void IsFormatVersionCompatible_06001092() => Mechanical.Compare(typeof(global::Quire.Core.Serialization.ApplicationPropertiesXmlSerializable), 0x06001092);

    [SkippableFact]
    public void ShouldValidateOnWrite_get_0600108f() => Mechanical.Compare(typeof(global::Quire.Core.Serialization.ApplicationPropertiesXmlSerializable), 0x0600108f);

    [SkippableFact]
    public void ShouldValidateOnRead_get_06001090() => Mechanical.Compare(typeof(global::Quire.Core.Serialization.ApplicationPropertiesXmlSerializable), 0x06001090);
}
