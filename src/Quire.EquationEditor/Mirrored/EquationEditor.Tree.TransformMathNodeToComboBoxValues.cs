namespace Quire.EquationEditor.Tree;

public static partial class TransformMathNodeToComboBoxValues
{
    public static MathNode SExpressionToMathNode(string expr)
    {
        return global::Quire.EquationEditor.Tree.Transform.SExpressionToMathNodeTransform.Default.Transform(expr);
    }
}
