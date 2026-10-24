#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf.Printing;

public partial class PrintDialog
{
    [MirrorVerified("Release IL")]
    public PrintDialog(RangeSelectionOptions rangeSelectionOptions)
    {
        CurrentPageEnabled = rangeSelectionOptions.HasFlag(RangeSelectionOptions.CurrentPageEnabled);
        SelectedPagesEnabled = rangeSelectionOptions.HasFlag(RangeSelectionOptions.SelectedPagesEnabled);
        UserPageRangeEnabled = rangeSelectionOptions.HasFlag(RangeSelectionOptions.UserPageRangeEnabled);
    }

    [MirrorVerified("PtcWpfPrintingPrintDialogMechanicalDifferentialTests")]
    public bool PrintToFile => false;

    [MirrorVerified("PtcWpfPrintingPrintDialogMechanicalDifferentialTests")]
    public string FileName => string.Empty;

    [MirrorVerified("Release IL")]
    public PageIndexRange PageRange => new PageIndexRange(base.PageRange);
}
