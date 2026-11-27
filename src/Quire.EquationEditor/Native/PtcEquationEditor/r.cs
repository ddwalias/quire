#nullable disable
using Quire.Controls.Core;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class r
{
    [MirrorVerified("Release IL")]
    public MathNode b() => a("NaN", IdLabels.Constant);

    [MirrorVerified("Release IL")]
    public MathNode a(FormattedString formattedString) => a(formattedString, null);

    [MirrorVerified("Release IL")]
    public MathNode a(string text, params MathNode[] mathNodes) => b(text, null, mathNodes);

    [MirrorVerified("Release IL")]
    public MathNode a(InputControlTypes inputControlTypes) => InputControlNodeCreator.Create(inputControlTypes, null, null);

    [MirrorVerified("Release IL")]
    public MathNode a(string text, InputControlTypes inputControlTypes) => a(text, inputControlTypes, null, null);

    [MirrorVerified("Release IL")]
    public string d() => c_;
}
