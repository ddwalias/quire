namespace Quire.EquationEditor.Tree.Transform;

public partial class ParsedSExpressionToInlineTransform
{
    private global::System.Collections.Generic.IList<global::System.Windows.Documents.Inline> a(
        global::System.Collections.Generic.List<ParsedSExpression> parsed)
    {
        return parsed.ConvertAll<global::System.Windows.Documents.Inline>(c);
    }

    private global::System.Windows.Documents.Inline c(ParsedSExpression parsed)
    {
        if (!parsed.IsValid)
        {
            throw new InvalidSExpression("Logic Error: ParsedSExpression is Invalid");
        }

        global::System.Windows.Documents.Inline inline;
        // Mathcad branches on HasValue to the Run path before the list path.
        if (parsed.HasValue)
        {
            inline = b(parsed);
        }
        else
        {
            inline = a(parsed);
        }
        if (inline == null)
        {
            return null;
        }

        if (!global::System.String.IsNullOrEmpty(parsed.Label))
        {
            throw new InvalidSExpression(string.Format(
                "Labels are not supported in Inline s-expressions: {0}",
                SExpressionParser.UnParse(parsed)));
        }

        return inline;
    }

    private global::System.Windows.Documents.Run b(ParsedSExpression parsed)
    {
        return new global::System.Windows.Documents.Run(parsed.Value);
    }

    public global::System.Windows.Documents.Span Transform(ParsedSExpression parsed)
    {
        return d(parsed);
    }
}
