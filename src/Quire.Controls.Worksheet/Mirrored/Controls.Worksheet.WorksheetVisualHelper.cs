using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

internal partial class WorksheetVisualHelper
{
    public bool ShouldToggleSelect
    {
        get
        {
            return ((int)Keyboard.Modifiers & 2) > 0;
        }
    }

    public bool ShouldContiguousSelect
    {
        get
        {
            return ((int)Keyboard.Modifiers & 4) > 0;
        }
    }

    IWhiteboardItem IWorksheetVisualHelper.FindWhiteboardItemAncestor(object child)
    {
        return FindWhiteboardItemAncestor(child);
    }

    internal static IWhiteboardItem FindWhiteboardItemAncestor(object child)
    {
        return (child as DependencyObject).TryFindVisualAncestorLimited<IWhiteboardItem, IWorksheetControl>();
    }

    IWhiteboardItem IWorksheetVisualHelper.FindWhiteboardItemAncestorIncludingSelf(object child)
    {
        return FindWhiteboardItemAncestorIncludingSelf(child);
    }

    internal static IWhiteboardItem FindWhiteboardItemAncestorIncludingSelf(object child)
    {
        return (child as DependencyObject).TryFindAncestorIncludingSelfLimited<IWhiteboardItem, IWorksheetControl>();
    }

    IWhiteboardItem IWorksheetVisualHelper.FindTopWhiteboardItemAncestorIncludingSelf(object source)
    {
        return FindTopWhiteboardItemAncestorIncludingSelf(source);
    }

    internal static IWhiteboardItem FindTopWhiteboardItemAncestorIncludingSelf(object source)
    {
        IWhiteboardItem result = null;
        for (IWhiteboardItem whiteboardItem = FindWhiteboardItemAncestorIncludingSelf(source); whiteboardItem != null; whiteboardItem = FindWhiteboardItemAncestor(whiteboardItem))
        {
            result = whiteboardItem;
        }
        return result;
    }

    public IWhiteboardItemContainer FindContainerAncestor(IWhiteboardItem item)
    {
        return (item as DependencyObject).TryFindVisualAncestor<IWhiteboardItemContainer>();
    }

    public IWorksheetPage FindWorksheetPageAncestor(object child)
    {
        return (child as DependencyObject).TryFindAncestorIncludingSelfLimited<IWorksheetPage, IWorksheetControl>();
    }

    public UIElement FindUIElementAncestorOrSelf(object element)
    {
        return (element as DependencyObject).TryFindAncestorIncludingSelf<UIElement>();
    }

    public IWorksheetControl FindWorksheetControlAncestor(object child)
    {
        return (child as DependencyObject).TryFindAncestorIncludingSelf<IWorksheetControl>();
    }

    public object FindOneOfPageRelatedAncestorsIncludingSelf(object child)
    {
        return WpfUtils.TryFindFirstAncestorIncludingSelfLimited<IWorksheetControl>(child as DependencyObject, new List<Type>
        {
            typeof(SwitchToDraftModeButton),
            typeof(IWorksheetPage),
            typeof(IWorksheetPageSection)
        });
    }

    public Point TranslatePoint(Point point, object transformFrom, object transformTo)
    {
        UIElement uIElement = transformFrom as UIElement;
        UIElement uIElement2 = transformTo as UIElement;
        if (uIElement == null || uIElement2 == null)
        {
            return point;
        }
        return uIElement.TranslatePoint(point, uIElement2);
    }

    public bool IsSignificantDistance(Vector vector)
    {
        return WpfUtils.IsSignificantMouseMove(vector);
    }

    public void BringIntoView(IWhiteboardItem item)
    {
        if (item is FrameworkElement frameworkElement)
        {
            if (frameworkElement is ICollapsibleAreaControl)
            {
                frameworkElement.BringIntoView(new Rect(new Size(0.0, frameworkElement.ActualHeight)));
            }
            else
            {
                frameworkElement.BringIntoView();
            }
        }
        else
        {
            DebugTools.FailAssertion("The given item should be a FrameworkElement.");
        }
    }

    public void BringIntoView(IWhiteboardItem item, Rect targetRect)
    {
        if (item is FrameworkElement frameworkElement)
        {
            frameworkElement.BringIntoView(targetRect);
        }
        else
        {
            DebugTools.FailAssertion("The given item should be a FrameworkElement.");
        }
    }

    public void SuppressBringIntoViewAndExecute(IWorksheetPageSection pageSection, Action action)
    {
        AttachedProperties.SuppressBringIntoViewIfNeededAndExecute(pageSection as DependencyObject, action, suppressBringToView: true);
    }

    public IScrollInfo GetScrollViewerRect(object child, out Rect mainScrollViewerBounds)
    {
        IScrollInfo scrollInfo = (child as DependencyObject).TryFindVisualAncestor<IScrollInfo>();
        while (scrollInfo != null && scrollInfo.ScrollOwner == null)
        {
            scrollInfo = (scrollInfo as DependencyObject).TryFindVisualAncestor<IScrollInfo>();
        }
        if (scrollInfo != null)
        {
            mainScrollViewerBounds = new Rect(scrollInfo.HorizontalOffset, scrollInfo.VerticalOffset, scrollInfo.ViewportWidth, scrollInfo.ViewportHeight);
            return scrollInfo;
        }
        mainScrollViewerBounds = default;
        return null;
    }

    private double GetEngineeringDocumentZoomValue(IWhiteboardItem item)
    {
        return (item as DependencyObject).TryFindVisualAncestor<IEngineeringDocument>().Zoom;
    }

    public Point GetVisualLocation(IWhiteboardItem item)
    {
        WpfUtils.DoEvents(Dispatcher.CurrentDispatcher, (DispatcherPriority)6);
        FrameworkElement worksheetFrameworkElement = WorksheetPropertiesManager.GetWorksheetFrameworkElement((Visual)item);
        return TranslatePoint(default, item, worksheetFrameworkElement);
    }
}
