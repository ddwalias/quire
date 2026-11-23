using Quire.Mirror;

namespace Quire.EquationEditor.Tree;

public static partial class TransformMathNodeToComboBoxValues
{
    public static MathNode SExpressionToMathNode(string expr)
    {
        return global::Quire.EquationEditor.Tree.Transform.SExpressionToMathNodeTransform.Default.Transform(expr);
    }

    partial class a
    {
        [MirrorVerified("Release IL")]
        public a() { }

        [MirrorVerified("Release IL")]
        internal string b(MathNode A_0) => MathNodeToStringTransformer.TransformToValueViewString(A_0);
    }
}
