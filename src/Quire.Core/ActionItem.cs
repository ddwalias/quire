#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.Undo;
using Quire.Undo;

namespace Quire;

public partial class ActionItem
{
    [MirrorVerified("PtcActionItemMechanicalDifferentialTests")]
    public ActionItem(IUndoableCommand command, CommandEventArgs.ActionTypeEnum actionType) : base(command, actionType) { }

    [MirrorVerified("Release IL")]
    public ActionItem(CommandEventArgs commandEventArgs) : base(commandEventArgs.Command, commandEventArgs.ActionType) { }
}
