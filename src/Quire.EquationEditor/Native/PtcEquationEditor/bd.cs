#nullable disable
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class bd
{
    [MirrorVerified("Release IL")]
    private NavigatorState c;

    [MirrorVerified("Release IL")]
    public NavigatorState a() => c;

    [MirrorVerified("Release IL")]
    public void a(NavigatorState navigatorState) => c = navigatorState;

    [MirrorVerified("Release IL")]
    public bool b() => b_;

    [MirrorVerified("Release IL")]
    public void a(bool flag) => b_ = flag;
}
