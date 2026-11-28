#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor.Tree;

public partial class InputScriptableWithInputNodeInfo
{
    [MirrorVerified("Release IL")]
    protected object a() => new InputScriptableWithInputNodeInfo(this);

    [MirrorVerified("Release IL")]
    public void DelegateRequestRestoreInsertionPoint(RequestRestoreInsertionPointEventArgs source) { }

    [MirrorVerified("Release IL")]
    private object[] b;

    [MirrorVerified("Release IL")]
    public object[] InputsToRestore { get => b; set => b = value; }

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
    }

    partial class b_
    {
        [MirrorVerified("Release IL")]
        public b_() { }
    }

    public event global::System.Action<global::Quire.DelegateFunction0<bool>, string> RequestUndoableCommandExecution;
    public event global::System.Action ValueChanged;
    public event global::System.EventHandler<global::Quire.EquationEditor.Tree.RequestRestoreInsertionPointEventArgs> RequestRestoreInsertionPoint;
}
