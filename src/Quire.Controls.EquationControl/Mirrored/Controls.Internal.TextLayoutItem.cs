using System.Windows;

namespace Quire.Controls.Internal;

public partial class TextLayoutItem
{
    public Rect Rect;

    public ItemTypes ItemType { get; set; }

    public Size Size
    {
        get => Rect.Size;
        set => Rect = new Rect(0.0, 0.0, value.Width, value.Height);
    }

    public double TextBaselineOffset { get; set; }

    public double CapsHeight { get; set; }

    public double XHeight { get; set; }

    public override string ToString()
    {
        return string.Format("{0}; BL:{1} CH:{2} XH:{3}", Rect, TextBaselineOffset, CapsHeight, XHeight);
    }

    public TextLayoutItem()
    {
    }
}
