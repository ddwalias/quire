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

    [MirrorVerified("Release IL")]
    public int FirstIndex
    {
        get
        {
            PageRange pageNumRange = PageNumRange;
            return pageNumRange.PageFrom - 1;
        }
    }

    [MirrorVerified("Release IL")]
    public int LastIndex
    {
        get
        {
            PageRange pageNumRange = PageNumRange;
            return pageNumRange.PageTo - 1;
        }
    }
}
