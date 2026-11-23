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
        internal string b(MathNode mathNode) => mathNode.SExpression;

        [MirrorVerified("Release IL")]
        internal string a(MathNode mathNode) => mathNode.SExpression;

        [MirrorVerified("Release IL")]
        internal bool a(string text) => EnumerableExtensions.IsNullOrEmpty<char>(text);
    }

    partial class b
    {
        [MirrorVerified("Release IL")]
        public b() { }
    }
}
