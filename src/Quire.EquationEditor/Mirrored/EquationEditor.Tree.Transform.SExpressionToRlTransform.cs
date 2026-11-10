namespace Quire.EquationEditor.Tree.Transform;

public partial class SExpressionToRlTransform
{
    public string TransformIdSubNode(string sExpr)
    {
        MathNode node = global::Quire.EquationEditor.Tree.SExpressionHelperUtilites.a(sExpr);
        return a(node);
    }
}
