#nullable disable
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class r
{
    [MirrorVerified("Release IL")]
    public MathNode b() => a("NaN", IdLabels.Constant);

    [MirrorVerified("Release IL")]
    public MathNode a(FormattedString A_0) => a(A_0, null);

    [MirrorVerified("Release IL")]
    public MathNode a(string A_0, params MathNode[] A_1) => b(A_0, null, A_1);

    [MirrorVerified("Release IL")]
    public MathNode a(InputControlTypes A_0) => InputControlNodeCreator.Create(A_0, null, null);

    [MirrorVerified("Release IL")]
    public MathNode a(string A_0, InputControlTypes A_1) => a(A_0, A_1, null, null);
}
