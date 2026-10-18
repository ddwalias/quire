#nullable disable
using System.Windows.Controls.Primitives;
using Quire.Mirror;

namespace Quire.Controls;

public partial class DimensionSelectorButton
{
    [MirrorVerified("PtcControlsDimensionSelectorButtonMechanicalDifferentialTests")]
    public bool IsSelected { get => GetIsSelected(this); set => SetIsSelected(this, value); }

    [MirrorVerified("PtcControlsDimensionSelectorButtonMechanicalDifferentialTests")]
    private UniformGrid _child;

    [MirrorVerified("PtcControlsDimensionSelectorButtonMechanicalDifferentialTests")]
    public object Content => _child;

    [MirrorVerified("PtcControlsDimensionSelectorButtonMechanicalDifferentialTests")]
    public event MouseOverCellHandler MouseOverCell;

    [MirrorVerified("PtcControlsDimensionSelectorButtonMechanicalDifferentialTests")]
    public event Delegate0 Commit;
}
