#nullable disable
using System.Windows;
using System.Windows.Data;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class PropertyValueKeeper
{
    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueKeeperMechanicalDifferentialTests")]
    public void RemoveBinding(DependencyObject sourceObj) => BindingOperations.ClearBinding(this, PropertyValueBase.PropertyValueProperty);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueKeeperMechanicalDifferentialTests")]
    public PropertyValueKeeper() { }
}
