#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class SnappableItemBaseHelper
{
    [MirrorVerified("PtcControlsCoreSnappableItemBaseHelperMechanicalDifferentialTests")]
    public static bool IsClearImageEnabled { get; set; }

    [MirrorVerified("PtcControlsCoreSnappableItemBaseHelperMechanicalDifferentialTests")]
    public bool IsPageDescendant { get; private set; }

    [MirrorVerified("PtcControlsCoreSnappableItemBaseHelperMechanicalDifferentialTests")]
    public UIElement ItemElement { get; private set; }
}
