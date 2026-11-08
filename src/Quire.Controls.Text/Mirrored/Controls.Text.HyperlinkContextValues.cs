#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using DocumentTextRange = System.Windows.Documents.TextRange;
using Quire.IO;
using Quire.Wpf;

namespace Quire.Controls.Text;

public partial class HyperlinkContextValues
{
    private Hyperlink _contextLink;

    public HyperlinkContextValues(RichTextWithEmbeddedItem parent)
    {
        if (parent.Selection.IsEmpty)
        {
            ContextLink = parent.GetHyperlinkAtCaretPositionIncludingEdges();
            ConvertToLinkRange = ContextLink != null
                ? new DocumentTextRange(ContextLink.ElementStart, ContextLink.ElementEnd)
                : parent.GetWordRange();
        }
        else
        {
            ConvertToLinkRange = new DocumentTextRange(parent.Selection.Start, parent.Selection.End);
            List<Hyperlink> links = parent.GetHyperlinksInRange(ConvertToLinkRange, false);
            if (EnumerableExtensions.IsNotEmpty(links) && DebugTools.DevAssert(links.Count == 1).Passed)
            {
                Hyperlink link = links.First();
                ContextLink = link;
                TextPointer start = ConvertToLinkRange.Start.CompareTo(link.ElementStart) > 0
                    ? link.ElementStart
                    : ConvertToLinkRange.Start;
                TextPointer end = ConvertToLinkRange.End.CompareTo(link.ElementEnd) < 0
                    ? link.ElementEnd
                    : ConvertToLinkRange.End;
                if (ConvertToLinkRange.Start.CompareTo(start) != 0 || ConvertToLinkRange.End.CompareTo(end) != 0)
                {
                    ConvertToLinkRange = new DocumentTextRange(start, end);
                }
            }
        }

        if (TextRangeExtension.IsNotEmpty(ConvertToLinkRange) && parent.ContainsSimpleText(ConvertToLinkRange))
        {
            string text = RichTextBase.GetTextOnlyFromRuns(ConvertToLinkRange);
            if (EnumerableExtensions.IsNotEmpty(text))
            {
                ContextText = text;
            }
            else
            {
                ConvertToLinkRange = null;
            }
        }
        UserCanModifyText = TextRangeExtension.IsNullOrEmpty(ConvertToLinkRange) || EnumerableExtensions.IsNotEmpty(ContextText);
    }

    public DocumentTextRange ConvertToLinkRange { get; }

    public string ContextText { get; }

    public string ContextLinkAddress { get; private set; }

    public Hyperlink ContextLink
    {
        get => _contextLink;
        private set
        {
            _contextLink = value;
            string address = null;
            if (_contextLink != null)
            {
                address = UrlUtils.GetStringAddressFromUri(ContextLink.NavigateUri);
                DebugTools.DevAssert(EnumerableExtensions.IsNotEmpty(address));
            }
            ContextLinkAddress = address;
        }
    }

    public bool UserCanModifyText { get; }
}
