#nullable disable
using System.Windows.Documents;
using System.Windows.Documents;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class TextRangeExtension
{
    [MirrorVerified("PtcWpfTextRangeExtensionMechanicalDifferentialTests")]
    public static IEnumerable<Hyperlink> EnumerateHyperlinksInRange(this global::System.Windows.Documents.TextRange range, bool allowEmpty = false) => EnumerateInlinesInRange<Hyperlink>(range, allowEmpty);

    [MirrorVerified("Release IL")]
    public static bool IsNotEmpty(this global::System.Windows.Documents.TextRange range) => !IsNullOrEmpty(range);

    [MirrorVerified("Release IL")]
    private static FlowDocument GetDocument(this global::System.Windows.Documents.TextRange range) => WpfUtils.TryFindAncestorIncludingSelf<FlowDocument>(range.Start.Parent, false);
}
