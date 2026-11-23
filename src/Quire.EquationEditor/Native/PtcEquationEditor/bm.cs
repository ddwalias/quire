#nullable disable
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class bm
{
    [MirrorVerified("Release IL")]
    private readonly a1 a;

    [MirrorVerified("Release IL")]
    private readonly MathNode b;

    [MirrorVerified("Release IL")]
    private readonly MathNode c;

    [MirrorVerified("Release IL")]
    private readonly a2 d;

    [MirrorVerified("Release IL")]
    protected override NavigatorState bq() => d.a(b, c, a);
}
