#nullable disable
using Quire.Mirror;

namespace Quire.Undo;

public partial class UndoableCommandWithSideEffects
{
    [MirrorVerified("PtcUndoUndoableCommandWithSideEffectsMechanicalDifferentialTests")]
    public UndoableCommandWithSideEffects(string name, DelegateFunction0<bool> doMethod, Delegate0 undoMethod) : base(name, doMethod, undoMethod) { }

    [MirrorVerified("PtcUndoUndoableCommandWithSideEffectsMechanicalDifferentialTests")]
    public UndoableCommandWithSideEffects(string name, DelegateFunction0<bool> doMethod, Delegate0 undoMethod, Delegate0 redoMethod) : base(name, doMethod, undoMethod, redoMethod) { }
}
