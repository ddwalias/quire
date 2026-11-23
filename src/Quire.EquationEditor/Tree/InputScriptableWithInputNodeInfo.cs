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
}
