#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Core;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ToolTipImage
{
    [MirrorVerified("PtcControlsCoreToolTipImageMechanicalDifferentialTests")]
    private readonly SnappableItemHelper _snappableHelper;

    [MirrorVerified("PtcControlsCoreToolTipImageMechanicalDifferentialTests")]
    public ISnapToPixelHelper SnappingHelper => _snappableHelper;

    public ToolTipImage()
    {
        Stretch = Stretch.None;
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
        _snappableHelper = SnappableItemHelper.Create(this, isPageDescendant: false);
        _snappableHelper.RenderTarget = this;
    }
}
