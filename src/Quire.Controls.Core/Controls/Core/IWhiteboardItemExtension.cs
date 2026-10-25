#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Protection;
using Quire.Controls.Core.Whiteboard;

namespace Quire.Controls.Core;

public static partial class IWhiteboardItemExtension
{
    public static IEnumerable<T> GetRegionsOfType<T>(this IEnumerable<IWhiteboardItem> items, Func<T, bool> filter = null) where T : class
    {
        return items.SelectMany((IWhiteboardItem item) => item.GetRegionsOfType(filter));
    }

    public static IEnumerable<T> GetRegionsOfType<T>(this IWhiteboardItem item, Func<T, bool> filter = null) where T : class
    {
        if (item is T val && (filter == null || filter(val)))
        {
            yield return val;
        }
        if (!(item is IWhiteboardItemContainer whiteboardItemContainer))
        {
            yield break;
        }
        foreach (T item2 in whiteboardItemContainer.WhiteboardItems.GetRegionsOfType(filter))
        {
            yield return item2;
        }
    }

    public static bool GetCanSelect(this IWhiteboardItem item)
    {
        if (!(item is IProtectableControl protectableControl))
        {
            return true;
        }
        return !protectableControl.ProtectionLevel.IsSelectionProtected();
    }

    public static bool IsSupportingRegionBorder(this IWhiteboardItem item)
    {
        if (!(item is IMathboardControl) || item is ICollapsibleAreaControl)
        {
            IEquationControl equationControl = item as IEquationControl;
            if (equationControl != null)
            {
                DependencyObject dependencyObject = equationControl as DependencyObject;
                return !IOTag.IsIOTagged(dependencyObject);
            }
            return true;
        }
        return false;
    }
}
