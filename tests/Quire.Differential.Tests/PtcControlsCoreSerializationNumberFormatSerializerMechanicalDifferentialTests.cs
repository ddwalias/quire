namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.Serialization.NumberFormatSerializer</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.Serialization.NumberFormatSerializer.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreSerializationNumberFormatSerializerMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetSchema_06000b64() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.NumberFormatSerializer), 0x06000b64);

    [SkippableFact]
    public void NumberFormatWasRead_get_06000b4e() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.NumberFormatSerializer), 0x06000b4e);

    [SkippableFact]
    public void NumberFormatWasRead_set_06000b4d() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.NumberFormatSerializer), 0x06000b4d);
}
