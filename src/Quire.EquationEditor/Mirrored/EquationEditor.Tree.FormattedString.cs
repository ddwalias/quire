using System.Text;
using System.Windows.Documents;

namespace Quire.EquationEditor.Tree;

public partial class FormattedString
{
    private string g;
    private string h;
    private int j;

    public int Length
    {
        // Mathcad clears initlocals on this getter despite its fat method header.
        [System.Runtime.CompilerServices.SkipLocalsInit]
        get
        {
            if (!IsEditable)
                return j;
            if (j == 0 && ((Span)WpfTextBlock.Inlines.FirstInline).Inlines.Count != 0)
                j = Text.Length;
            return j;
        }
    }

    public bool IsSimpleString
    {
        get
        {
            if (!IsEditable)
                return g != null && h == null;
            Span content = (Span)WpfTextBlock.Inlines.FirstInline;
            return content.Inlines.Count == 0 ||
                (content.Inlines.Count == 1 && content.Inlines.FirstInline.GetType() == typeof(Run));
        }
    }

    public override int GetHashCode() => ToString().GetHashCode();

    public override string ToString()
    {
        if (!IsEditable)
        {
            if (!DebugTools.AssertNotNull(g, "ToString() method shouldn't be used on read-only, non-simple formatted strings, as the result isn't defined.").Passed)
                return string.Empty;
            return g;
        }
        StringBuilder text = new StringBuilder();
        DebugTools.DevAssert(WpfTextBlock.Inlines.Count < 2, null);
        a(WpfTextBlock.Inlines.FirstInline, text, true);
        return text.ToString();
    }
}
