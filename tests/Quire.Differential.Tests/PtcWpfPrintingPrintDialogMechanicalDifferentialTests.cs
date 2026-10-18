namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.Printing.PrintDialog</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.Printing.PrintDialog.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfPrintingPrintDialogMechanicalDifferentialTests
{
    [SkippableFact]
    public void PrintToFile_get_0600064e() => Mechanical.Compare(typeof(global::Quire.Wpf.Printing.PrintDialog), 0x0600064e);

    [SkippableFact]
    public void FileName_get_0600064f() => Mechanical.Compare(typeof(global::Quire.Wpf.Printing.PrintDialog), 0x0600064f);
}
