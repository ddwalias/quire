using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;

namespace Quire.Controls.Worksheet;

public partial class WorksheetItemManager
{
    public static WhiteboardItemSizeChangedEventHandler GetWorksheetItemSizeChangedHandler(DependencyObject depObj)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return null;
        }
        return (WhiteboardItemSizeChangedEventHandler)depObj.GetValue(WorksheetItemSizeChangedHandlerProperty);
    }

    public static void SetWorksheetItemSizeChangedHandler(DependencyObject depObj, WhiteboardItemSizeChangedEventHandler handler)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return;
        }
        depObj.SetValue(WorksheetItemSizeChangedHandlerProperty, handler);
    }

    public static void ClearWorksheetItemSizeChangedHandler(DependencyObject depObj)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return;
        }
        depObj.ClearValue(WorksheetItemSizeChangedHandlerProperty);
    }

    public static IList<WhiteboardItemSizeChangedEventArgs> GetSizeChangedEventArgsList(DependencyObject depObj)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return null;
        }
        return (IList<WhiteboardItemSizeChangedEventArgs>)depObj.GetValue(SizeChangedEventArgsListProperty);
    }

    public static void SetSizeChangedEventArgsList(DependencyObject depObj, IList<WhiteboardItemSizeChangedEventArgs> argsList)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return;
        }
        depObj.SetValue(SizeChangedEventArgsListProperty, argsList);
    }

    public static void ClearSizeChangedEventArgsList(DependencyObject depObj)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return;
        }
        depObj.ClearValue(SizeChangedEventArgsListProperty);
    }

    public static long GetAddMoveRemoveTimestamp(DependencyObject depObj)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return 0L;
        }
        return (long)depObj.GetValue(AddMoveRemoveTimestampProperty);
    }

    public static void SetAddMoveRemoveTimestamp(DependencyObject depObj, long stamp)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return;
        }
        depObj.SetValue(AddMoveRemoveTimestampProperty, stamp);
    }

    public static bool GetSplitCheckIsRequired(DependencyObject depObj)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return false;
        }
        return (bool)depObj.GetValue(SplitCheckIsRequiredProperty);
    }

    public static void SetSplitCheckIsRequired(DependencyObject depObj, bool required)
    {
        if (DebugTools.AssertNotNull(depObj, null).Failed)
        {
            return;
        }
        depObj.SetValue(SplitCheckIsRequiredProperty, required);
    }

    public bool ItemHasPendingSizeChangedEvents(IWhiteboardItem item)
    {
        return !GetSizeChangedEventArgsList(FrameworkElementFacade.GetFrameworkElement(item)).IsNullOrEmpty();
    }

    public Point GetItemWorksheetLocation(IWhiteboardItem item)
    {
        return GetItemLocation(item, WorksheetControl.GetWorksheetLeft, WorksheetControl.GetWorksheetTop);
    }

    public Point GetItemWorksheetLocation(IWorksheetPageBreak pageBreak)
    {
        return GetItemLocation(pageBreak, WorksheetControl.GetWorksheetLeft, WorksheetControl.GetWorksheetTop);
    }

    private void SetItemWorksheetLocationForObject(object obj, Point location)
    {
        DependencyObject val = obj as DependencyObject;
        if (val != null)
        {
            WorksheetControl.SetWorksheetLeft(val, location.X);
            WorksheetControl.SetWorksheetTop(val, location.Y);
        }
    }

    public void SetItemWorksheetLocation(IWhiteboardItem item, Point location)
    {
        SetItemWorksheetLocationForObject(item, location);
    }

    public void SetItemWorksheetLocation(IWorksheetPageBreak pageBreak, Point location)
    {
        SetItemWorksheetLocationForObject(pageBreak, location);
    }

    public void SetContainingPage(IWhiteboardItem item, IWorksheetPage containingPage)
    {
        DependencyObject val = item as DependencyObject;
        if (val == null)
        {
            return;
        }
        if (containingPage == null)
        {
            val.ClearValue(WorksheetItemSplitter.ContainingPageProperty);
        }
        else
        {
            val.SetValue(WorksheetItemSplitter.ContainingPageProperty, containingPage);
        }
    }

    public IWorksheetPage GetContainingPage(IWhiteboardItem item)
    {
        DependencyObject val = item as DependencyObject;
        if (val == null)
        {
            return null;
        }
        return (IWorksheetPage)val.GetValue(WorksheetItemSplitter.ContainingPageProperty);
    }

    public void SetItemClipGeometry(IWhiteboardItem item, Geometry clip)
    {
        if (clip != null)
        {
            ((Freezable)clip).Freeze();
        }
        UIElement uIElement = UIElementFacade.GetUIElement(item);
        IWhiteboardItemPresenter itemPresenter = GetItemPresenter(item);
        if (itemPresenter != null)
        {
            DebugTools.DevAssert(uIElement.Clip == null, null);
            itemPresenter.Clip = clip;
        }
        else
        {
            uIElement.Clip = clip;
        }
    }

    public override Rect GetItemRect(IWhiteboardItem item)
    {
        Point itemWorksheetLocation = GetItemWorksheetLocation(item);
        itemWorksheetLocation.Offset(0.0, item.VerticalAlignmentOffset);
        return new Rect(itemWorksheetLocation, GetRenderSize(item));
    }

    public long GetItemAddMoveRemoveTimestamp(IWhiteboardItem item)
    {
        return GetAddMoveRemoveTimestamp(FrameworkElementFacade.GetFrameworkElement(item));
    }

    public void SetItemAddMoveRemoveTimestamp(IWhiteboardItem item, long stamp)
    {
        SetAddMoveRemoveTimestamp(FrameworkElementFacade.GetFrameworkElement(item), stamp);
    }

    public bool GetItemSplitCheckIsRequired(IWhiteboardItem item)
    {
        return GetSplitCheckIsRequired(FrameworkElementFacade.GetFrameworkElement(item));
    }

    public void SetItemSplitCheckIsRequired(IWhiteboardItem item, bool required)
    {
        SetSplitCheckIsRequired(FrameworkElementFacade.GetFrameworkElement(item), required);
    }

    public bool GetShouldAssertOnLocation(IWhiteboardItem item)
    {
        return AttachedProperties.GetShouldAssertOnLocation(FrameworkElementFacade.GetFrameworkElement(item));
    }

    public void SetShouldAssertOnLocation(IWhiteboardItem item, bool value)
    {
        AttachedProperties.SetShouldAssertOnLocation(FrameworkElementFacade.GetFrameworkElement(item), value);
    }

    public void ClearShouldAssertOnLocation(IWhiteboardItem item)
    {
        AttachedProperties.ClearShouldAssertOnLocation(FrameworkElementFacade.GetFrameworkElement(item));
    }

    public IWhiteboardItem GetItemPrintView(IWhiteboardItem item)
    {
        return AttachedProperties.GetPrintView(FrameworkElementFacade.GetFrameworkElement(item));
    }

    public void HookupAreaBorderChanged(IWhiteboardItem item)
    {
        if (item is ICollapsibleAreaControl collapsibleAreaControl)
        {
            collapsibleAreaControl.BorderStateChanged += OnAreaBorderStateChanged;
        }
    }

    public Geometry GetItemClipGeometry(IWhiteboardItem item)
    {
        UIElement uIElement = UIElementFacade.GetUIElement(item);
        IWhiteboardItemPresenter itemPresenter = GetItemPresenter(item);
        Geometry clip;
        // Single exit through one local: Release IL shares one return slot across both branches.
        if (itemPresenter != null)
        {
            DebugTools.DevAssert(uIElement.Clip == null, null);
            clip = itemPresenter.Clip;
        }
        else
        {
            clip = uIElement.Clip;
        }
        return clip;
    }
}
