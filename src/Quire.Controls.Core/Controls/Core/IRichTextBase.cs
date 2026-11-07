#nullable disable
using System;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using Quire.Core;
using Quire.Serialization;

namespace Quire.Controls.Core;

public partial interface IRichTextBase
{
    TextRangeExtended TextRange { get; }
    FlowDocument Document { get; set; }
    int CaretOffset { get; set; }
    int SelectionStart { get; }
    int SelectionEnd { get; }
    bool IsOnDrop { get; }
    void SetCaretOffsetAtContentEnd();
    void SetFormattedContent(string content, string dataFormat, XmlLanguage proofingLanguage, TextFormat textFormat = default);
    TextFormat GetTextFormatForWholeDocument();
    void SetSelection(int fromOffset, int toOffset);
    void SelectTo(int toOffset);
    void SaveWholeTextToRtf(Stream rtfStream);
    double? DequeueSplitFontSizeData();
}
