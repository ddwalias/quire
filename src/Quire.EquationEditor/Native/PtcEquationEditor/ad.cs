#nullable disable
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class ad
{
    [MirrorVerified("Release IL")]
    private IMatrixValueProxyWrapper c;

    [MirrorVerified("Release IL")]
    private void a(IMatrixValueProxyWrapper matrixValueProxyWrapper) => c = matrixValueProxyWrapper;

    [MirrorVerified("Release IL")]
    private Func<IValueProxyWrapper, IResultNode> d;

    [MirrorVerified("Release IL")]
    private void a(Func<IValueProxyWrapper, IResultNode> callback) => d = callback;

    [MirrorVerified("Release IL")]
    public override MathNode bo(IResultTreeFormatter resultTreeFormatter) => resultTreeFormatter.FormatResult(this);

    [MirrorVerified("Release IL")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);
}
