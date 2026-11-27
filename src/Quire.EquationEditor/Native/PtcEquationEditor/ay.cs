#nullable disable
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class ay
{
    [MirrorVerified("Release IL")]
    public int d() => a_.Count;

    [MirrorVerified("Release IL")]
    public void b() => a_.Clear();

    [MirrorVerified("Release IL")]
    public void a(NavigatorState navigatorState) => a_.Add(navigatorState);

    [MirrorVerified("Release IL")]
    public void a(int valueValue) => a_.RemoveAt(valueValue);

    [MirrorVerified("Release IL")]
    public void a(int valueValue, NavigatorState navigatorState) => a_.Insert(valueValue, navigatorState);
}
