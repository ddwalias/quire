#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class MessagesDecorator
{
    [MirrorVerified("PtcControlsCoreAttachedBehaviorMessagesDecoratorMechanicalDifferentialTests")]
    private readonly List<NodeMessageAdorner> _adornerList;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorMessagesDecoratorMechanicalDifferentialTests")]
    public IEnumerable<NodeMessageAdorner> AdornerList => _adornerList;
}
