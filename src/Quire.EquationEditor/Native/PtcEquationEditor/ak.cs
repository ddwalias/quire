#nullable disable
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using System.Collections.Generic;
using Quire.Core;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class ak
{
    [MirrorVerified("Release IL")]
    private al a(Direction direction, params MathNode[] nodes) => a(direction, (IList<MathNode>)nodes);

    [MirrorVerified("Release IL")]
    private al a(Direction direction, IList<MathNode> nodes)
    {
        MathNode first = nodes[0];
        MathNode last = nodes[nodes.Count - 1];
        MathNode leftmost = bq.c(Direction.Left, first).e();
        if (leftmost != first && first.IsContainerSlot(0))
        {
            leftmost = first;
        }
        MathNode rightmost = bq.c(Direction.Right, last).e();
        if (rightmost != last && last.IsContainerSlot(last.ChildCount - 1))
        {
            rightmost = last;
        }
        return new al(leftmost, rightmost, DirectionOps.Opposite(direction));
    }

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
