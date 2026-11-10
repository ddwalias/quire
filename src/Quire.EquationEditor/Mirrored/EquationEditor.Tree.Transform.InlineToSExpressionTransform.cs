namespace Quire.EquationEditor.Tree.Transform;

public partial class InlineToSExpressionTransform
{
    private string a(
        global::System.Windows.Documents.Span span,
        ref int inlineCount,
        ref int styledSpanCount)
    {
        var builder = new global::System.Text.StringBuilder();
        e.TryGetValue(span.GetType(), out string value);
        if (!global::System.String.IsNullOrEmpty(value))
        {
            styledSpanCount++;
            builder.Append('(').Append(value);
        }

        foreach (global::System.Windows.Documents.Inline inline in span.Inlines)
        {
            string transformed = a(inline, ref inlineCount, ref styledSpanCount);
            if (!global::System.String.IsNullOrEmpty(transformed))
            {
                builder.Append(' ').Append(transformed);
            }
        }

        if (!global::System.String.IsNullOrEmpty(value))
        {
            builder.Append(')');
        }
        else if (builder.Length > 0 && builder[0] == ' ')
        {
            builder.Remove(0, 1);
        }

        return builder.ToString();
    }

    private string a(global::System.Windows.Documents.Run run, ref int inlineCount)
    {
        inlineCount++;
        e.TryGetValue(run.GetType(), out string value);
        if (global::System.String.IsNullOrEmpty(value))
        {
            return SExpressionUtility.EscapeIdValue(run.Text);
        }

        return "(" + value + " " + SExpressionUtility.EscapeIdValue(run.Text) + ")";
    }

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
