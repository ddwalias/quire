using System.Windows.Media.Imaging;
using System.Windows;
using Net.Sgoliver.NRtfTree.Core;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class RtfItemImage
{
    public BitmapSource Image { get; private set; }

    public RtfItemImage(BitmapSource image, bool noLeftOffset = false)
    {
        Image = image;
        FullSize = ImageUtils.GetArchitectureIndependentSize(Image);
        NoLeftOffset = noLeftOffset;
    }

    protected override void AddItemToDocumentInternal(RtfTreeNode node, Rect clippingRect, bool relativeToCaret, RtfMerger merger)
    {
        node.AddRegion(Image, clippingRect, GetLocationFlags(relativeToCaret));
    }

}
