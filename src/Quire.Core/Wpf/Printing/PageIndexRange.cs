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

    [MirrorVerified("Release IL")]
    public PageIndexRange(int firstIndex, int? lastIndex = null)
    {
        PageNumRange = new PageRange(firstIndex + 1, lastIndex.GetValueOrDefault(firstIndex) + 1);
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
