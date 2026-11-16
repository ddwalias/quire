using System.Windows;

namespace Quire.Controls.Internal;

public partial struct LayoutItem
{
    public Rect Rect;
    public double TextBaselineOffset;

    public LayoutItem(Size itemSize)
        : this(itemSize, 0.0)
    {
    }

    public LayoutItem(Size itemSize, double textBaseline)
    {
        Rect = new Rect(0.0, 0.0, itemSize.Width, itemSize.Height);
        TextBaselineOffset = textBaseline;
    }

    public override string ToString()
    {
        return string.Format("{0}; BL:{1}", Rect, TextBaselineOffset);
    }
}
