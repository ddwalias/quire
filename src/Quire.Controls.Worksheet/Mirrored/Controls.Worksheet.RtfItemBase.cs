#nullable disable
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public abstract partial class RtfItemBase : IRtfItemBase
{
    public int PageIndex { get; private set; }

    public Size TopRectClippingSize { get; private set; }

    public Size FullSize { get; protected set; }

    public Point TopLeft { get; private set; }

    public bool NoLeftOffset { get; protected set; }

    public bool AutoWidth { get; protected set; }

    public void Init(int pageIndex, Rect clippingRect, bool autoWidth = false)
    {
        PageIndex = pageIndex;
        TopLeft = clippingRect.TopLeft;
        TopRectClippingSize = clippingRect.Size;
        AutoWidth = autoWidth;
    }

    protected RtfHelper.LocationFlags GetLocationFlags(bool relativeToCaret)
    {
        RtfHelper.LocationFlags locationFlags = RtfHelper.LocationFlags.None;
        if (relativeToCaret)
        {
            locationFlags |= RtfHelper.LocationFlags.RelativeToCaret;
        }
        if (AutoWidth)
        {
            locationFlags |= RtfHelper.LocationFlags.AutoWidth;
        }
        return locationFlags;
    }

    protected RtfItemBase()
    {
    }
}
