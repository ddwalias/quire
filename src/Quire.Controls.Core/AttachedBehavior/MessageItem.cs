#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class MessageItem
{
    [MirrorVerified("PtcControlsCoreAttachedBehaviorMessageItemMechanicalDifferentialTests")]
    public MessageItem() { }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorMessageItemMechanicalDifferentialTests")]
    public NodeMessageAdornerType MessageType { get; set; }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorMessageItemMechanicalDifferentialTests")]
    public MessageCategory MessageCategory { get; set; }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorMessageItemMechanicalDifferentialTests")]
    public string MessageResourceKey { get; set; }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorMessageItemMechanicalDifferentialTests")]
    public string Message { get; set; }
}
