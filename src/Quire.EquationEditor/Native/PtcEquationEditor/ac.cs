#nullable disable
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class ac
{
    [MirrorVerified("Release IL")]
    private int b;

    [MirrorVerified("Release IL")]
    private void a(int A_0) => b = A_0;

    [MirrorVerified("Release IL")]
    public override MathNode bo(IResultTreeFormatter A_0) => A_0.FormatResult(this);

    [MirrorVerified("Release IL")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);
}
