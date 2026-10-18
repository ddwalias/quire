namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.Serialization.MatrixFormatSerializer</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.Serialization.MatrixFormatSerializer.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreSerializationMatrixFormatSerializerMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetSchema_06000ad8() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.MatrixFormatSerializer), 0x06000ad8);

    [SkippableFact]
    public void MatrixFormatWasRead_get_06000ad6() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.MatrixFormatSerializer), 0x06000ad6);

    [SkippableFact]
    public void MatrixFormatWasRead_set_06000ad5() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Serialization.MatrixFormatSerializer), 0x06000ad5);
}
