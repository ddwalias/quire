namespace Quire.EquationEditor.Tree.Transform;

public partial class ParsedSExpressionToInlineTransform
{
    private global::System.Windows.Documents.Run b(ParsedSExpression parsed)
    {
        return new global::System.Windows.Documents.Run(parsed.Value);
    }

    public global::System.Windows.Documents.Span Transform(ParsedSExpression parsed)
    {
        return d(parsed);
    }
}
