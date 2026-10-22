#nullable disable
using Quire.Mirror;

namespace Quire.Core;

public partial class SynchFontSizeOnScriptRunsWrapperCmd
{
    [MirrorVerified("Release IL")]
    private global::System.Windows.Documents.TextRange GetRange()
    {
        return new global::System.Windows.Documents.TextRange(_document.ContentStart.GetPositionAtOffset(_rangeStart.Item1, _rangeStart.Item2), _document.ContentEnd.GetPositionAtOffset(_rangeEnd.Item1, _rangeEnd.Item2));
    }

    [MirrorVerified("Release IL")]
    private global::System.Windows.Documents.TextRange GetExtendedRange()
    {
        global::System.Windows.Documents.TextPointer textPointer = _document.ContentStart.GetPositionAtOffset(_rangeStart.Item1, _rangeStart.Item2);
        global::System.Windows.Documents.TextPointer textPointer2 = _document.ContentEnd.GetPositionAtOffset(_rangeEnd.Item1, _rangeEnd.Item2);
        if (textPointer?.GetAdjacentElement(global::System.Windows.Documents.LogicalDirection.Backward) is global::System.Windows.Documents.Run run)
        {
            textPointer = run.ContentEnd;
        }
        if (textPointer2?.GetAdjacentElement(global::System.Windows.Documents.LogicalDirection.Forward) is global::System.Windows.Documents.Run run2)
        {
            textPointer2 = run2.ContentStart;
        }
        return new global::System.Windows.Documents.TextRange(textPointer, textPointer2);
    }

    [MirrorVerified("PtcCoreSynchFontSizeOnScriptRunsWrapperCmdMechanicalDifferentialTests")]
    public string Name { get; }
}
