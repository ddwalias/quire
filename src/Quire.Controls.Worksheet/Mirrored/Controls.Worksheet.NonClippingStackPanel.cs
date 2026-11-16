using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quire.Controls.Worksheet;

public partial class NonClippingStackPanel : StackPanel
{
    protected override Geometry GetLayoutClip(Size layoutSlotSize)
    {
        if (!ClipToBounds)
        {
            return null;
        }
        return base.GetLayoutClip(layoutSlotSize);
    }
}
