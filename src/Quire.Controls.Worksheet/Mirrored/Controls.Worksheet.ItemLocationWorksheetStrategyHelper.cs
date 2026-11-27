using System;
using System.Windows;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public static partial class ItemLocationWorksheetStrategyHelper
{
    public static Rect? GetItemContentClipping(
        IWhiteboardItem whiteboardItem,
        Rect contentPartRect,
        int partCount,
        out Rect? stickingPartRect,
        Func<IWhiteboardItem, int, Rect?> getItemBoundsAndRelativePartLocation)
    {
        stickingPartRect = null;
        Rect? result = getItemBoundsAndRelativePartLocation(whiteboardItem, 0);
        if (result.HasValue)
        {
            Rect value = Rect.Intersect(contentPartRect, result.Value);
            DebugTools.DevAssert(!value.IsEmpty || partCount > 1, null);
            if (partCount > 1 && (value.IsEmpty || value.Height < WpfUtils.CmToDips(0.5)))
            {
                Rect? val = getItemBoundsAndRelativePartLocation(whiteboardItem, 1);
                if (val.HasValue)
                {
                    Rect intersection = Rect.Intersect(contentPartRect, val.Value);
                    if (!intersection.IsEmpty && (value.IsEmpty || intersection.Height > value.Height))
                    {
                        result = val;
                        stickingPartRect = value;
                    }
                }
            }
        }
        return result;
    }
}
