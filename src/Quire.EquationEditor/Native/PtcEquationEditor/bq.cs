#nullable disable
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class bq
{
    [MirrorVerified("Release IL")]
    public bq(MathNode mathNode) : this(mathNode, Direction.Indeterminate) { }

    [MirrorVerified("Release IL")]
    public bq c(Direction direction)
    {
        b(direction);
        return this;
    }

    [MirrorVerified("Release IL")]
    public MathNode a(Direction direction) => b(direction).Current;

    [MirrorVerified("Release IL")]
    public static bool c(MathNode mathNode) => a(Direction.Right, mathNode);

    [MirrorVerified("Release IL")]
    public static bool b(MathNode mathNode) => a(Direction.Left, mathNode);

    [MirrorVerified("Release IL")]
    public static bool c(NavigatorState navigatorState) => a(Direction.Right, navigatorState);

    [MirrorVerified("Release IL")]
    public static bool b(NavigatorState navigatorState) => a(Direction.Left, navigatorState);

    [MirrorVerified("Release IL")]
    public NavigatorState f() => b_;
}
