#nullable disable
using System.Linq;
using System.Windows.Documents;
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

    [MirrorVerified("Release IL")]
    public static bool IsNotEmpty(this global::System.Windows.Documents.TextRange range) => !IsNullOrEmpty(range);

    [MirrorVerified("PtcWpfTextRangeExtensionMechanicalDifferentialTests")]
    public static IEnumerable<Hyperlink> EnumerateHyperlinksInRange(this global::System.Windows.Documents.TextRange range, bool allowEmpty = false) => EnumerateInlinesInRange<Hyperlink>(range, allowEmpty);

    public static IEnumerable<Run> EnumerateRunsInRange(this global::System.Windows.Documents.TextRange range) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static IEnumerable<T> EnumerateInlinesInRange<T>(this global::System.Windows.Documents.TextRange range, bool allowEmpty = false) where T : Inline => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static IEnumerable<T> EnumerateInlinesInRange<T>(this global::System.Windows.Documents.TextRange range, IEnumerable<Block> blocks) where T : Inline => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static IEnumerable<T> EnumerateInlinesInRange<T>(this global::System.Windows.Documents.TextRange range, InlineCollection inlines) where T : Inline => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [MirrorVerified("Release IL")]
    private static FlowDocument GetDocument(this global::System.Windows.Documents.TextRange range) => WpfUtils.TryFindAncestorIncludingSelf<FlowDocument>(range.Start.Parent, false);

    public static object GetFormattingPropertyFromRuns(List<Run> runs, global::System.Windows.DependencyProperty formattingProperty, Func<object, object, bool> comparer = default)
    {
        bool flag = false;
        if (global::Quire.DebugTools.DevAssert(runs.IsNotEmpty(), null).Failed)
        {
            return null;
        }
        IEnumerable<object> source = runs.Select((Run run) =>
        {
            if (((object)formattingProperty).Equals((object?)TextElement.FontSizeProperty) && run is ICustomFontSizeOwner customFontSizeOwner)
            {
                return (object)customFontSizeOwner.FontSizeForReport;
            }
            return ((global::System.Windows.DependencyObject)run).GetValue(formattingProperty);
        });
        object firstValue = source.FirstOrDefault();
        if (firstValue != null)
        {
            flag = source.All((object val) => comparer?.Invoke(firstValue, val) ?? firstValue.Equals(val));
        }
        if (!flag)
        {
            return null;
        }
        return firstValue;
    }

    public static void FixAndCleanImportedHyperlinksData(this global::System.Windows.Documents.TextRange range, bool fixHyperlinkEmptyRuns = true)
    {
        if (!range.EnumerateHyperlinksInRange().IsNotEmpty(out var list))
        {
            return;
        }
        if (fixHyperlinkEmptyRuns)
        {
            list = list.Select((Hyperlink link) => link.RecursiveRemoveEmptyInlinesIncludingSelf()).OfType<Hyperlink>().ToList();
        }
        list.Each((Hyperlink link) =>
        {
            link.ResetLinkStyleProps();
        });
    }

    public static string SerializeRangeContentToXamlString(this global::System.Windows.Documents.TextRange range)
    {
        string result;
        using (global::Quire.MindTouch.IO.ChunkedMemoryStream chunkedMemoryStream = new global::Quire.MindTouch.IO.ChunkedMemoryStream())
        {
            range.Save(chunkedMemoryStream, global::System.Windows.DataFormats.Xaml);
            chunkedMemoryStream.Position = 0L;
            using (global::System.IO.StreamReader streamReader = new global::System.IO.StreamReader(chunkedMemoryStream))
            {
                result = streamReader.ReadToEnd();
            }
        }
        return result;
    }

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

    public static List<(int, int)> GetRangeItemsOffsets<TRun>(this global::System.Windows.Documents.TextRange range) where TRun : Run
    {
        global::System.Windows.Documents.TextPointer documentStart = range.Start.DocumentStart;
        return range.EnumerateRunsInRange().OfType<TRun>().Select((Run run) =>
        {
            int start = documentStart.GetOffsetToPosition(run.ElementStart);
            int end = documentStart.GetOffsetToPosition(run.ElementEnd);
            return (start, end);
        }).ToList();
    }
}
