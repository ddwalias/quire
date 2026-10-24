#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Undo;

public partial class UndoableCommand
{
    [MirrorVerified("Release IL")]
    public UndoableCommand(string name, DelegateFunction0<bool> doMethod, Delegate0 undoMethod, Delegate0 redoMethod)
    {
        _name = name;
        _do = doMethod;
        _undo = undoMethod;
        _redo = redoMethod;
    }

    [MirrorVerified("PtcUndoUndoableCommandMechanicalDifferentialTests")]
    private DelegateFunction0<bool> _do;

    [MirrorVerified("PtcUndoUndoableCommandMechanicalDifferentialTests")]
    public virtual bool Do() => _do.Invoke();

    [MirrorVerified("PtcUndoUndoableCommandMechanicalDifferentialTests")]
    private Delegate0 _undo;

    [MirrorVerified("PtcUndoUndoableCommandMechanicalDifferentialTests")]
    public virtual void Undo() => _undo.Invoke();

    public UndoableCommand(string name, DelegateFunction0<bool> doMethod, Delegate0 undoMethod)
        : this(name, doMethod, undoMethod, () =>
        {
            doMethod();
        })
    {
    }

    [MirrorVerified("PtcUndoUndoableCommandMechanicalDifferentialTests")]
    private Delegate0 _redo;

    [MirrorVerified("PtcUndoUndoableCommandMechanicalDifferentialTests")]
    public virtual void Redo() => _redo.Invoke();

    [MirrorVerified("PtcUndoUndoableCommandMechanicalDifferentialTests")]
    private string _name;

    [MirrorVerified("PtcUndoUndoableCommandMechanicalDifferentialTests")]
    public string Name => _name;

}
