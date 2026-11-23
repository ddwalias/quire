#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor.Tree.Transform;

public partial class MathNodeToSExpressionTransform
{
    [MirrorVerified("Release IL")]
    public string Transform(MathNode node) => Transform(node, null);
}
