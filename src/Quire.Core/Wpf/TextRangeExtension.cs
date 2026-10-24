#nullable disable
using System.Windows.Documents;
using System.Windows.Documents;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class TextRangeExtension
{
    [MirrorVerified("Release IL")]
    public static bool IsNullOrEmpty(this global::System.Windows.Documents.TextRange range)
    {
        if (range != null)
        {
            return range.IsEmpty;
        }
        return true;
    }

    [MirrorVerified("PtcWpfTextRangeExtensionMechanicalDifferentialTests")]
    public static IEnumerable<Hyperlink> EnumerateHyperlinksInRange(this global::System.Windows.Documents.TextRange range, bool allowEmpty = false) => EnumerateInlinesInRange<Hyperlink>(range, allowEmpty);

    [MirrorVerified("Release IL")]
    private static FlowDocument GetDocument(this global::System.Windows.Documents.TextRange range) => WpfUtils.TryFindAncestorIncludingSelf<FlowDocument>(range.Start.Parent, false);

    private static bool IntersectsWith(this global::System.Windows.Documents.TextRange range, TextElement element)
    {
        int? contentStartVsContentEnd = null;
        int num = range.Start.CompareTo(element.ContentEnd);
        if (num > 0)
        {
            return false;
        }
        if (num == 0 && IsNotEmptyElement())
        {
            return false;
        }
        int num2 = range.End.CompareTo(element.ContentStart);
        if (num2 < 0)
        {
            return false;
        }
        if (num2 == 0 && IsNotEmptyElement())
        {
            return false;
        }
        return true;

        bool IsNotEmptyElement() => (contentStartVsContentEnd = contentStartVsContentEnd ?? element.ContentStart.CompareTo(element.ContentEnd)).Value != 0;
    }

    [MirrorVerified("Release IL")]
    public static bool IsNotEmpty(this global::System.Windows.Documents.TextRange range) => !IsNullOrEmpty(range);
}
