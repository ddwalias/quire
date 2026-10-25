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

    public static IEnumerable<Run> EnumerateRunsInRange(this global::System.Windows.Documents.TextRange range)
    {
        // Match Mathcad's Release IL local slots and Run/Span branch layout.
        IEnumerable<Run> runs;
        Hyperlink hyperlink;
        LogicalDirection direction;
        bool isForward;
        Stack<Inline> pendingInlines;
        Run run;
        Span span;
        Inline inline;
        IEnumerable<Inline> inlines;

        runs = range.EnumerateInlinesInRange<Run>(allowEmpty: true);
        if (runs.IsNullOrEmpty() && range.IsEmpty)
        {
            global::Quire.DebugTools.DevAssert(range.Start.LogicalDirection == range.End.LogicalDirection, null);
            hyperlink = range.Start.GetHyperlinkAdjacentToTextPointer(out direction);
            if (hyperlink != null)
            {
                isForward = direction == LogicalDirection.Forward;
                Inline firstInline = isForward ? hyperlink.Inlines.FirstInline : hyperlink.Inlines.LastInline;
                pendingInlines = new Stack<Inline>(firstInline.YieldOrEmpty());
                while (pendingInlines.IsNotEmpty())
                {
                    inline = pendingInlines.Pop();
                    run = inline as Run;
                    if (run != null)
                    {
                        goto FoundRun;
                    }

                    span = inline as Span;
                    if (span == null)
                    {
                        goto NonRunOrSpan;
                    }
                    goto FoundSpan;

                FoundRun:
                    runs = run.Yield();
                    pendingInlines = null;
                    goto ContinueTraversal;

                FoundSpan:
                    (isForward ? (inlines = span.Inlines) : span.Inlines.Reverse()).Each(pendingInlines.Push);
                    goto ContinueTraversal;

                NonRunOrSpan:
                    pendingInlines = null;

                ContinueTraversal:
                    ;
                }
            }
        }
        return runs;
    }

    public static IEnumerable<T> EnumerateInlinesInRange<T>(this global::System.Windows.Documents.TextRange range, bool allowEmpty = false) where T : Inline
    {
        if (global::Quire.DebugTools.AssertNotNull(range, null).Failed || global::Quire.DebugTools.DevAssert(typeof(T) != typeof(Span), null).Failed)
        {
            return Enumerable.Empty<T>();
        }
        if (range.IsEmpty)
        {
            if (!allowEmpty)
            {
                return Enumerable.Empty<T>();
            }
            return range.Start.GetTextElementFromTextPointer<T>().YieldOrEmpty();
        }
        FlowDocument document = range.GetDocument();
        if (document != null)
        {
            return range.EnumerateInlinesInRange<T>(document.Blocks);
        }
        global::Quire.DebugTools.FailAssertion(null, global::System.Array.Empty<object>());
        return Enumerable.Empty<T>();
    }

    private static IEnumerable<T> EnumerateInlinesInRange<T>(this global::System.Windows.Documents.TextRange range, IEnumerable<Block> blocks) where T : Inline => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static IEnumerable<T> EnumerateInlinesInRange<T>(this global::System.Windows.Documents.TextRange range, InlineCollection inlines) where T : Inline
    {
        List<Inline> intersectingInlines = inlines.Where(range.IntersectsWith).ToList();
        foreach (Inline inline in intersectingInlines)
        {
            // Keep both yield sites in Mathcad's Release state-machine order.
            if (!(inline is T matchingInline))
            {
                goto CheckSpan;
            }
            goto YieldMatchingInline;

        CheckSpan:
            if (!(inline is Span span))
            {
                goto NextInline;
            }
            goto EnumerateSpan;

        YieldMatchingInline:
            yield return matchingInline;
            goto NextInline;

        EnumerateSpan:
            foreach (T nestedInline in range.EnumerateInlinesInRange<T>(span.Inlines))
            {
                yield return nestedInline;
            }

        NextInline:
            ;
        }
    }

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
