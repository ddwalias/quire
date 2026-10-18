#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class RoutedEventBinding
{
    [MirrorVerified("PtcControlsCoreAttachedBehaviorRoutedEventBindingMechanicalDifferentialTests")]
    private readonly RoutedEvent _routedEvent;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorRoutedEventBindingMechanicalDifferentialTests")]
    public RoutedEvent RoutedEvent => _routedEvent;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorRoutedEventBindingMechanicalDifferentialTests")]
    private readonly RoutedEventHandler _handler;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorRoutedEventBindingMechanicalDifferentialTests")]
    public RoutedEventHandler Handler => _handler;
}
