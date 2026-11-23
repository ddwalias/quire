#nullable disable
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class bd
{
    [MirrorVerified("Release IL")]
    private NavigatorState c;

    [MirrorVerified("Release IL")]
    public NavigatorState a() => c;

    [MirrorVerified("Release IL")]
    public void a(NavigatorState A_0) => c = A_0;
}
