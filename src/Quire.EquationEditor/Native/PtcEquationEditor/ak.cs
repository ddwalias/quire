#nullable disable
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class ak
{
    [MirrorVerified("Release IL")]
    public void h() => c(Direction.Right);

    [MirrorVerified("Release IL")]
    public void c(NavigatorState navigatorState) => a(navigatorState, Direction.Right);

    [MirrorVerified("Release IL")]
    public void b(NavigatorState navigatorState) => a(navigatorState, Direction.Left);

    [MirrorVerified("Release IL")]
    public void g() => c(Direction.Left);

    [MirrorVerified("Release IL")]
    public void f() => e(Direction.Right);

    [MirrorVerified("Release IL")]
    public void e() => e(Direction.Left);

    [MirrorVerified("Release IL")]
    public IMemento d() => new a_(this);

    [MirrorVerified("Release IL")]
    private al b(Direction direction, params MathNode[] mathNodes) => e(direction, mathNodes);

    [MirrorVerified("Release IL")]
    public NavigatorState q() => f_;
}
