#nullable disable
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class ak
{
    [MirrorVerified("Release IL")]
    public void h() => c(Direction.Right);

    [MirrorVerified("Release IL")]
    public void c(NavigatorState A_0) => a(A_0, Direction.Right);

    [MirrorVerified("Release IL")]
    public void b(NavigatorState A_0) => a(A_0, Direction.Left);

    [MirrorVerified("Release IL")]
    public void g() => c(Direction.Left);

    [MirrorVerified("Release IL")]
    public void f() => e(Direction.Right);

    [MirrorVerified("Release IL")]
    public void e() => e(Direction.Left);

    [MirrorVerified("Release IL")]
    public IMemento d() => new a_(this);

    [MirrorVerified("Release IL")]
    private al b(Direction A_0, params MathNode[] A_1) => e(A_0, A_1);
}
