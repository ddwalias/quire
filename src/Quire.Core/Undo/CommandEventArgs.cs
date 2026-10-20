#nullable disable
using Quire.Mirror;
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

    [MirrorVerified("Release IL")]
    public CommandEventArgs(IUndoableCommand command, CommandEventArgs.ActionTypeEnum actionType)
    {
        _commandReference = new WeakReference(command);
        _actionType = actionType;
    }

    [MirrorVerified("Release IL")]
    public IUndoableCommand Command
    {
        get
        {
            IUndoableCommand obj = _commandReference.Target as IUndoableCommand;
            DebugTools.DevAssert(obj != null, null);
            return obj;
        }
    }
}
