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

    [MirrorVerified("Release IL")]
    public static bool operator ==(PageIndexRange range1, PageIndexRange range2) => range1.PageNumRange == range2.PageNumRange;

    [MirrorVerified("Release IL")]
    public static bool operator !=(PageIndexRange range1, PageIndexRange range2) => range1.PageNumRange != range2.PageNumRange;

    [MirrorVerified("Release IL")]
    public bool Equals(PageIndexRange other)
    {
        PageRange pageNumRange = PageNumRange;
        return pageNumRange.Equals(other.PageNumRange);
    }

    [MirrorVerified("Release IL")]
    public override bool Equals(object obj)
    {
        if (obj is PageIndexRange other)
        {
            return Equals(other);
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    public override int GetHashCode()
    {
        PageRange pageNumRange = PageNumRange;
        return pageNumRange.GetHashCode();
    }
}
