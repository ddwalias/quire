#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class PropertyValueBase
{
    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueBaseMechanicalDifferentialTests")]
    public PropertyValueBase() { }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueBaseMechanicalDifferentialTests")]
    public static readonly DependencyProperty PropertyValueProperty;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueBaseMechanicalDifferentialTests")]
    public object PropertyValue { get => base.GetValue(PropertyValueProperty); set => base.SetValue(PropertyValueProperty, value); }
}
