#nullable disable
using Quire.Mirror;
using Quire.Undo;

namespace Quire;

public partial class ActionItem
{
    [MirrorVerified("PtcActionItemMechanicalDifferentialTests")]
    public ActionItem(IUndoableCommand command, CommandEventArgs.ActionTypeEnum actionType) : base(command, actionType) { }
}
