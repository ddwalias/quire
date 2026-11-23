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
    public void a(bool A_0) => c = A_0;

    [MirrorVerified("Release IL")]
    private NavigatorState d;

    [MirrorVerified("Release IL")]
    public NavigatorState a() => d;

    [MirrorVerified("Release IL")]
    public void a(NavigatorState A_0) => d = A_0;
}
