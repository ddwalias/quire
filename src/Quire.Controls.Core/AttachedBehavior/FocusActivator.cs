#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

internal partial class FocusActivator
{
    [MirrorVerified("PtcControlsCoreAttachedBehaviorFocusActivatorMechanicalDifferentialTests")]
    private IActivator _activator;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorFocusActivatorMechanicalDifferentialTests")]
    private void ElementGotFocus(object sender, RoutedEventArgs e) => _activator.IsActive = true;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorFocusActivatorMechanicalDifferentialTests")]
    public FocusActivator() { }
}
