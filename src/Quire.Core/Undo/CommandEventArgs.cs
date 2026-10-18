#nullable disable
using Quire.Mirror;

namespace Quire.Undo;

public partial class CommandEventArgs
{
    [MirrorVerified("PtcUndoCommandEventArgsMechanicalDifferentialTests")]
    protected readonly WeakReference _commandReference;

    [MirrorVerified("PtcUndoCommandEventArgsMechanicalDifferentialTests")]
    public bool IsCommandAlive => _commandReference.IsAlive;

    [MirrorVerified("PtcUndoCommandEventArgsMechanicalDifferentialTests")]
    protected readonly ActionTypeEnum _actionType;

    [MirrorVerified("PtcUndoCommandEventArgsMechanicalDifferentialTests")]
    public ActionTypeEnum ActionType => _actionType;
}
