#nullable disable
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;

namespace Quire.EquationEditor.Tree.Scoping;

public partial class TypeoverState
{
    [MirrorVerified("Release IL")]
    public TypeoverState(LinearSegment segment, NavigatorState navState) : this(segment, navState, false) { }

    [MirrorVerified("Release IL")]
    public TypeoverState(NavigatorState left, NavigatorState right, NavigatorState navState) : this(left, right, navState, false) { }

    [MirrorVerified("Release IL")]
    public TypeoverState Clone() => new TypeoverState(this);

    [MirrorVerified("Release IL")]
    public IEnumerable<MathNode> ExcludedNodes => b();

    [MirrorVerified("Release IL")]
    public IEnumerable<MathNode> ExcludedOperands => a();

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
    }

    partial class b_
    {
        [MirrorVerified("Release IL")]
        public b_() { }
    }
}
