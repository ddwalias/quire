#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class SetPropertyValueCommandParameter
{
    [MirrorVerified("PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests")]
    public SetPropertyValueCommandParameter() { }

    [MirrorVerified("PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests")]
    protected override Freezable CreateInstanceCore() => new SetPropertyValueCommandParameter();

    [MirrorVerified("PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests")]
    public static DependencyProperty DependencyPropValProperty;

    [MirrorVerified("PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests")]
    public object DependencyPropVal { get => base.GetValue(DependencyPropValProperty); set => base.SetValue(DependencyPropValProperty, value); }

    [MirrorVerified("PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests")]
    public string UserData { get; set; }
}
