#nullable disable
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class bq
{
    [MirrorVerified("Release IL")]
    public bq(MathNode A_0) : this(A_0, Direction.Indeterminate) { }

    [MirrorVerified("Release IL")]
    public bq c(Direction A_0)
    {
        b(A_0);
        return this;
    }

    [MirrorVerified("Release IL")]
    public MathNode a(Direction A_0) => b(A_0).Current;

    [MirrorVerified("Release IL")]
    public static bool c(MathNode A_0) => a(Direction.Right, A_0);

    [MirrorVerified("Release IL")]
    public static bool b(MathNode A_0) => a(Direction.Left, A_0);

    [MirrorVerified("Release IL")]
    public static bool c(NavigatorState A_0) => a(Direction.Right, A_0);

    [MirrorVerified("Release IL")]
    public static bool b(NavigatorState A_0) => a(Direction.Left, A_0);
}
