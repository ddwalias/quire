#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class PrimeToolTip
{
    [MirrorVerified("PtcControlsCorePrimeToolTipMechanicalDifferentialTests")]
    public static readonly DependencyProperty HeaderProperty;

    [MirrorVerified("PtcControlsCorePrimeToolTipMechanicalDifferentialTests")]
    public object Header { get => base.GetValue(HeaderProperty); set => base.SetValue(HeaderProperty, value); }

    [MirrorVerified("PtcControlsCorePrimeToolTipMechanicalDifferentialTests")]
    public static readonly DependencyProperty ContentBodyProperty;

    [MirrorVerified("PtcControlsCorePrimeToolTipMechanicalDifferentialTests")]
    public object ContentBody { get => base.GetValue(ContentBodyProperty); set => base.SetValue(ContentBodyProperty, value); }
}
