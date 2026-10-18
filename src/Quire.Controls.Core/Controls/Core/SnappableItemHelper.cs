#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class SnappableItemHelper
{
    [MirrorVerified("PtcControlsCoreSnappableItemHelperMechanicalDifferentialTests")]
    public static SnappableItemHelper Create(UIElement itemElement, bool isPageDescendant) => new SnappableItemHelper(itemElement, isPageDescendant);
}
