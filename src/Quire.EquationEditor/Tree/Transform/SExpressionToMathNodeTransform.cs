#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor.Tree.Transform;

public partial class SExpressionToMathNodeTransform
{
    [MirrorVerified("Release IL")]
    internal MathNode f(ParsedSExpression parsedSExpression) => e(parsedSExpression);
}
