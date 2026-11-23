#nullable disable
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class ab
{
    [MirrorVerified("Release IL")]
    private int c;

    [MirrorVerified("Release IL")]
    private void a(int A_0) => c = A_0;

    [MirrorVerified("Release IL")]
    private TableCoordinate d;

    [MirrorVerified("Release IL")]
    private void a(TableCoordinate A_0) => d = A_0;

    [MirrorVerified("Release IL")]
    private IEagerMatrixResultNode e;

    [MirrorVerified("Release IL")]
    private void a(IEagerMatrixResultNode A_0) => e = A_0;

    [MirrorVerified("Release IL")]
    public override MathNode bo(IResultTreeFormatter A_0) => A_0.FormatResult(this);

    [MirrorVerified("Release IL")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);
}
