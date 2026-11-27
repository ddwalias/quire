#nullable disable
using Quire.EquationEditor;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class k
{
    [MirrorVerified("Release IL")]
    private k() { }

    [MirrorVerified("Release IL")]
    private string c() => a_;

    [MirrorVerified("Release IL")]
    private void a(string text) => a_ = text;

    [MirrorVerified("Release IL")]
    private IEquationModel b() => b_;

    [MirrorVerified("Release IL")]
    private void a(IEquationModel iEquationModel) => b_ = iEquationModel;
}
