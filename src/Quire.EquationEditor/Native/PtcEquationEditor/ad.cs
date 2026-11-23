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
    private void a(IMatrixValueProxyWrapper A_0) => c = A_0;

    [MirrorVerified("Release IL")]
    private Func<IValueProxyWrapper, IResultNode> d;

    [MirrorVerified("Release IL")]
    private void a(Func<IValueProxyWrapper, IResultNode> A_0) => d = A_0;

    [MirrorVerified("Release IL")]
    public override MathNode bo(IResultTreeFormatter A_0) => A_0.FormatResult(this);

    [MirrorVerified("Release IL")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);
}
