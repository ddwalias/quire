#nullable disable
using Quire.Mirror;

namespace Quire.Wpf.Printing;

public partial class PrintDialog
{
    [MirrorVerified("PtcWpfPrintingPrintDialogMechanicalDifferentialTests")]
    public bool PrintToFile => false;

    [MirrorVerified("PtcWpfPrintingPrintDialogMechanicalDifferentialTests")]
    public string FileName => string.Empty;
}
