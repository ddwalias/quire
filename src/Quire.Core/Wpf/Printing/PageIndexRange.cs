#nullable disable
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.Wpf.Printing;

public partial struct PageIndexRange
{
    [MirrorVerified("PtcWpfPrintingPageIndexRangeMechanicalDifferentialTests")]
    public PageIndexRange(PageRange pageNumRange)
    {
        PageNumRange = pageNumRange;
    }

    [MirrorVerified("PtcWpfPrintingPageIndexRangeMechanicalDifferentialTests")]
    public PageRange PageNumRange { get; }
}
