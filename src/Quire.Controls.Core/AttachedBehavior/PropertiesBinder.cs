#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class PropertiesBinder
{
    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests")]
    public static readonly DependencyProperty SourceValueConnectorProperty;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests")]
    public object SourceValueConnector { get => base.GetValue(SourceValueConnectorProperty); set => base.SetValue(SourceValueConnectorProperty, value); }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests")]
    public static readonly DependencyProperty TargetValueConnectorProperty;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests")]
    public object TargetValueConnector { get => base.GetValue(TargetValueConnectorProperty); set => base.SetValue(TargetValueConnectorProperty, value); }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests")]
    public bool BlockUndoWhenBindingToTarget { get; set; }
}
