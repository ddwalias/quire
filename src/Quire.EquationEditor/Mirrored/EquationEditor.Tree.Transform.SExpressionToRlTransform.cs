namespace Quire.EquationEditor.Tree.Transform;

public partial class SExpressionToRlTransform
{
    static SExpressionToRlTransform()
    {
        Default = new SExpressionToRlTransform(
            SExpressionToMathNodeTransform.Default,
            MathNodeToRlTransformer.Default);
    }

    public string TransformIdSubNode(string sExpr)
    {
        MathNode node = global::Quire.EquationEditor.Tree.SExpressionHelperUtilites.a(sExpr);
        return a(node);
    }
}
