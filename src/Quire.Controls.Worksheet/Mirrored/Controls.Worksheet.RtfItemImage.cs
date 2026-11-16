using System.Windows.Media.Imaging;
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
}
