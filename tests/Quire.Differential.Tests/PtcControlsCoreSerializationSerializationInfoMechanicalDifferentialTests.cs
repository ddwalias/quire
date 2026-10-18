namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.Serialization.SerializationInfo</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.Serialization.SerializationInfo.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreSerializationSerializationInfoMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000b97() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.SerializationInfo), 0x06000b97);

    [SkippableFact]
    public void Flags_get_06000b93() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.SerializationInfo), 0x06000b93);

    [SkippableFact]
    public void Flags_set_06000b94() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.SerializationInfo), 0x06000b94);

    [SkippableFact]
    public void DefaultValue_get_06000b95() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.SerializationInfo), 0x06000b95);

    [SkippableFact]
    public void DefaultValue_set_06000b96() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.SerializationInfo), 0x06000b96);
}
