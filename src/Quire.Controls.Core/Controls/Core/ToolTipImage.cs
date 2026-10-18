#nullable disable
using Quire.Core;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ToolTipImage
{
    [MirrorVerified("PtcControlsCoreToolTipImageMechanicalDifferentialTests")]
    private readonly SnappableItemHelper _snappableHelper;

    [MirrorVerified("PtcControlsCoreToolTipImageMechanicalDifferentialTests")]
    public ISnapToPixelHelper SnappingHelper => _snappableHelper;
}
