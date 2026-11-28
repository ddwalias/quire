#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor.Tree;

public partial class ComboBoxControlNodeInfo
{
    [MirrorVerified("Release IL")]
    protected override object bk() => new ComboBoxControlNodeInfo(this);

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
    }

    public event global::System.Action<global::Quire.DelegateFunction0<bool>, string> RequestUndoableCommandExecution;
    public event global::System.Action ValueChanged;
    public event global::System.EventHandler<global::Quire.EquationEditor.Tree.RequestRestoreInsertionPointEventArgs> RequestRestoreInsertionPoint;
}
