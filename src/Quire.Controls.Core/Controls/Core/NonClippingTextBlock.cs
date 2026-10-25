#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class NonClippingTextBlock : TextBlock
{
    [MirrorVerified("PtcControlsCoreNonClippingTextBlockMechanicalDifferentialTests")]
    public NonClippingTextBlock() { }

    protected override Geometry GetLayoutClip(Size layoutSlotSize)
    {
        Geometry layoutClip = base.GetLayoutClip(layoutSlotSize);
        if (!ClipToBounds)
        {
            return null;
        }
        return layoutClip;
    }
}
