#nullable disable
#pragma warning disable

using System.Windows.Documents;

namespace Quire.Controls.Text;

public static partial class HyperlinkExtensions
{
    public static bool HasSimpleTextContent(this Hyperlink link, RichTextWithEmbeddedItem ownerDocument) =>
        ownerDocument.ContainsSimpleText(new System.Windows.Documents.TextRange(link.ElementStart, link.ElementEnd));
}
