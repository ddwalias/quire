namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.MatrixFormat</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.MatrixFormat.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreMatrixFormatMechanicalDifferentialTests
{
    [SkippableFact]
    public void FormatChanged_add_0600046c() => Mechanical.Compare(typeof(global::Quire.Controls.Core.MatrixFormat), 0x0600046c);

    [SkippableFact]
    public void FormatChanged_remove_0600046d() => Mechanical.Compare(typeof(global::Quire.Controls.Core.MatrixFormat), 0x0600046d);

    [SkippableFact]
    public void PropertyChanged_add_06000487() => Mechanical.Compare(typeof(global::Quire.Controls.Core.MatrixFormat), 0x06000487);

    [SkippableFact]
    public void PropertyChanged_remove_06000488() => Mechanical.Compare(typeof(global::Quire.Controls.Core.MatrixFormat), 0x06000488);
}
