#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor.Tree.Transform;

public static partial class ComboBoxInputControlDataToSExpressionTransform
{
    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }

        [MirrorVerified("Release IL")]
        internal string b(MathNode A_0) => A_0.SExpression;

        [MirrorVerified("Release IL")]
        internal string a(MathNode A_0) => A_0.SExpression;

        [MirrorVerified("Release IL")]
        internal bool a(string A_0) => EnumerableExtensions.IsNullOrEmpty<char>(A_0);
    }

    partial class b
    {
        [MirrorVerified("Release IL")]
        public b() { }
    }
}
