namespace Quire.EquationEditor.Tree.Transform;

public partial class InlineToSExpressionTransform
{
    private string a(
        global::System.Windows.Documents.Inline inline,
        ref int inlineCount,
        ref int styledSpanCount)
    {
        if (inline is global::System.Windows.Documents.Run run)
        {
            return a(run, ref inlineCount);
        }

        if (inline is global::System.Windows.Documents.Span span)
        {
            return a(span, ref inlineCount, ref styledSpanCount);
        }

        throw new global::System.ArgumentException("Can't handle Inline which doesn't derive from Run or Span.", "inline");
    }

    public string Transform(global::System.Windows.Documents.Inline inline)
    {
        int inlineCount = 0;
        int styledSpanCount = 0;
        string text = a(inline, ref inlineCount, ref styledSpanCount);
        if (inlineCount > 1 || styledSpanCount > 0)
        {
            text = string.Format(a_, text);
        }

        return text;
    }

    static InlineToSExpressionTransform()
    {
        Default = new InlineToSExpressionTransform();
    }
}
