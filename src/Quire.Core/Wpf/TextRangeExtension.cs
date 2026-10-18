#nullable disable
using System.Windows.Documents;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class TextRangeExtension
{
    [MirrorVerified("PtcWpfTextRangeExtensionMechanicalDifferentialTests")]
    public static IEnumerable<Hyperlink> EnumerateHyperlinksInRange(this global::System.Windows.Documents.TextRange range, bool allowEmpty = false) => EnumerateInlinesInRange<Hyperlink>(range, allowEmpty);
}
