#nullable disable
using Quire.Controls.Core;
using Quire.Mirror;
using Quire.Native.PtcEquationEditor;

namespace Quire.EquationEditor.Tree;

public static partial class InputControlNodeCreator
{
    [MirrorVerified("Release IL")]
    public static MathNode Create(InputControlTypes inputControlType, IInputControlData data, MathNode[] childrenNodes = default) => r.a_.a("ScriptableControl", inputControlType, data, childrenNodes);
}
