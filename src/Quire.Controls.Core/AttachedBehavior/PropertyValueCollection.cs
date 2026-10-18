#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class PropertyValueCollection
{
    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueCollectionMechanicalDifferentialTests")]
    protected virtual void OnLoaded(FrameworkElement element) { }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueCollectionMechanicalDifferentialTests")]
    protected virtual void OnUnloaded(FrameworkElement element) { }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueCollectionMechanicalDifferentialTests")]
    public PropertyValueCollection() { }
}
