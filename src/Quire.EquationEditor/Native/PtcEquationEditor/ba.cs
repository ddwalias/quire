#nullable disable
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Scoping;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class ba
{
    [MirrorVerified("Release IL")]
    public LinearSegment c() => a_;

    [MirrorVerified("Release IL")]
    public bool b() => b_;

    [MirrorVerified("Release IL")]
    public void a(bool flag) => b_ = flag;

    [MirrorVerified("Release IL")]
    public NavigatorState a() => c_;

    [MirrorVerified("Release IL")]
    public void a(NavigatorState navigatorState) => c_ = navigatorState;
}
