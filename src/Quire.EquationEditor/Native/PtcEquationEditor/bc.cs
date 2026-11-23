#nullable disable
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class bc
{
    [MirrorVerified("Release IL")]
    private bool c;

    [MirrorVerified("Release IL")]
    public bool b() => c;

    [MirrorVerified("Release IL")]
    public void a(bool flag) => c = flag;

    [MirrorVerified("Release IL")]
    private NavigatorState d;

    [MirrorVerified("Release IL")]
    public NavigatorState a() => d;

    [MirrorVerified("Release IL")]
    public void a(NavigatorState navigatorState) => d = navigatorState;
}
