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

    [MirrorVerified("PtcCoreSynchFontSizeOnScriptRunsWrapperCmdMechanicalDifferentialTests")]
    public string Name { get; }
}
