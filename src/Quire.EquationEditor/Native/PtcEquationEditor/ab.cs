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
    private void a(int index) => c = index;

    [MirrorVerified("Release IL")]
    private TableCoordinate d;

    [MirrorVerified("Release IL")]
    private void a(TableCoordinate tableCoordinate) => d = tableCoordinate;

    [MirrorVerified("Release IL")]
    private IEagerMatrixResultNode e;

    [MirrorVerified("Release IL")]
    private void a(IEagerMatrixResultNode eagerMatrixResultNode) => e = eagerMatrixResultNode;

    [MirrorVerified("Release IL")]
    public override MathNode bo(IResultTreeFormatter resultTreeFormatter) => resultTreeFormatter.FormatResult(this);

    [MirrorVerified("Release IL")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);
}
