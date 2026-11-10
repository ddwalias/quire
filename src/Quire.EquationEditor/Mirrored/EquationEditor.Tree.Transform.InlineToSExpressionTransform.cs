namespace Quire.EquationEditor.Tree.Transform;

public partial class InlineToSExpressionTransform
{
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
