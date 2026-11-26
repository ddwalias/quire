using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class WorksheetItemManager
{
    private static readonly global::System.Type _typeOfThis;
    public static global::System.Windows.DependencyProperty WorksheetItemSizeChangedHandlerProperty;
    public static global::System.Windows.DependencyProperty SizeChangedEventArgsListProperty;
    public static readonly global::System.Windows.DependencyProperty AddMoveRemoveTimestampProperty;
    public static readonly global::System.Windows.DependencyProperty SplitCheckIsRequiredProperty;
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
    public override Rect GetItemRect(IWhiteboardItem item)
    {
        Point itemWorksheetLocation = GetItemWorksheetLocation(item);
        itemWorksheetLocation.Offset(0.0, item.VerticalAlignmentOffset);
        return new Rect(itemWorksheetLocation, GetRenderSize(item));
    }
    public override global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> SortItemsByLocation(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items, bool descending) => base.SortItemsByLocation(items, descending);
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
    public override Rect GetDescendantBounds(object obj)
    {
        return base.GetDescendantBounds(obj);
    }
    protected override WhiteboardItemSizeChangedEventHandler GetItemSizeChangedHandler(IWhiteboardItem item)
    {
        return GetWorksheetItemSizeChangedHandler(FrameworkElementFacade.GetFrameworkElement(item));
    }
    protected override void SetItemSizeChangedHandler(IWhiteboardItem item, WhiteboardItemSizeChangedEventHandler handler)
    {
        SetWorksheetItemSizeChangedHandler(FrameworkElementFacade.GetFrameworkElement(item), handler);
    }
    protected override void ClearItemSizeChangedHandler(IWhiteboardItem item)
    {
        FrameworkElement frameworkElement = FrameworkElementFacade.GetFrameworkElement(item);
        ClearWorksheetItemSizeChangedHandler(frameworkElement);
        ClearSizeChangedEventArgsList(frameworkElement);
        RemoveSizeChangedItem(item, frameworkElement, "clear SizeChanged handler");
    }
    private global::System.Windows.Threading.DispatcherOperation _delayedSizeChangedNotification;
    private global::Quire.Controls.Worksheet.SizeChangeNotificationDebugData _sizeChangedDebugData;
    private readonly global::System.Collections.Generic.IList<global::System.WeakReference> _sizeChangedItemRefs = new global::System.Collections.Generic.List<global::System.WeakReference>();
    private void ClearSizeChangedItemRefsIfNeeded(string operation)
    {
        if (_sizeChangedItemRefs.Any((WeakReference weakRef) => weakRef.IsAlive))
        {
            return;
        }
        if (_delayedSizeChangedNotification != null && _delayedSizeChangedNotification.Status == DispatcherOperationStatus.Pending)
        {
            _sizeChangedDebugData.AbortedOperations++;
            _delayedSizeChangedNotification.Abort();
        }
        _sizeChangedDebugData.DeadItemsCount += _sizeChangedItemRefs.Count;
        try
        {
            if (_sizeChangedDebugData.UnexpectedNotifyAttemptsCount > 0)
            {
                FailAssertionReportingSizeChangeProcessingHistory(string.Format("Finished during {0}!", operation));
            }
        }
        finally
        {
            _sizeChangedDebugData = default;
            _sizeChangedItemRefs.Clear();
        }
    }
    protected override void OnItemSizeChanged(object sender, SizeChangedEventArgs args)
    {
        FrameworkElement frameworkElement = sender as FrameworkElement;
        IWhiteboardItem whiteboardItem = sender as IWhiteboardItem;
        if (DebugTools.AssertNotNull(frameworkElement, "elem").Failed || DebugTools.AssertNotNull(whiteboardItem, "item").Failed)
        {
            return;
        }
        if (IsSizeChangedEventHandlerProhibited(whiteboardItem))
        {
            return;
        }
        IList<WhiteboardItemSizeChangedEventArgs> sizeChangedEventArgsList = GetSizeChangedEventArgsList(frameworkElement);
        WhiteboardItemSizeChangedEventArgs item = new WhiteboardItemSizeChangedEventArgs(args);
        if (sizeChangedEventArgsList == null)
        {
            sizeChangedEventArgsList = new List<WhiteboardItemSizeChangedEventArgs> { item };
            SetSizeChangedEventArgsList(frameworkElement, sizeChangedEventArgsList);
        }
        else
        {
            sizeChangedEventArgsList.Add(item);
        }
        AddSizeChangedItem(whiteboardItem, frameworkElement);
        DispatchSizeChangeNotificationsAction(((DispatcherObject)frameworkElement).Dispatcher);
    }
    private void DispatchSizeChangeNotificationsAction(Dispatcher dispatcher)
    {
        if (_delayedSizeChangedNotification != null && _delayedSizeChangedNotification.Status == DispatcherOperationStatus.Pending)
        {
            return;
        }
        bool dispatchedFromUndoRedo = EnvironmentScope.IsCurrentlyInsideUndoOrRedo;
        Action action = () =>
        {
            if (_dispatchingSizeChangeNotifications)
            {
                _sizeChangedDebugData.SkippedOperations++;
            }
            else
            {
                string text = string.Format("dispatch (from undo/redo: {0})", dispatchedFromUndoRedo);
                if (_sizeChangedItemRefs.IsNullOrEmpty())
                {
                    DebugTools.FailAssertion("Shouldn't get here with no items!");
                }
                else
                {
                    ClearSizeChangedItemRefsIfNeeded(text);
                }
                if (_sizeChangedItemRefs.IsNotEmpty())
                {
                    try
                    {
                        if (dispatchedFromUndoRedo)
                        {
                            FailAssertionReportingSizeChangeProcessingHistory(string.Format("Starting unexpected {0}!", text));
                        }
                    }
                    finally
                    {
                        if (dispatchedFromUndoRedo)
                        {
                            _sizeChangedDebugData.UnexpectedNotifyAttemptsCount++;
                            using (new CommandScope(new CompoundCommand()))
                            {
                                ProcessDispatchedSizeChangeNotifications(text);
                            }
                        }
                        else
                        {
                            ProcessDispatchedSizeChangeNotifications(text);
                        }
                    }
                }
            }
        };
        _delayedSizeChangedNotification = dispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate)action);
    }
    private bool _dispatchingSizeChangeNotifications;
    private void FailAssertionReportingSizeChangeProcessingHistory(string title)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(title);
        stringBuilder.Append("in dispatch: ").Append(_dispatchingSizeChangeNotifications).Append(", ");
        stringBuilder.Append("items count: ").Append((_sizeChangedItemRefs == null) ? (-1) : _sizeChangedItemRefs.Count).AppendLine();
        stringBuilder.Append("overall attempts: ").Append(_sizeChangedDebugData.NotifyAttemptsCount).Append(", ");
        stringBuilder.Append("unexpected attempts: ").Append(_sizeChangedDebugData.UnexpectedNotifyAttemptsCount).AppendLine();
        stringBuilder.Append("aborted operations: ").Append(_sizeChangedDebugData.AbortedOperations).Append(", ");
        stringBuilder.Append("skipped operations: ").Append(_sizeChangedDebugData.SkippedOperations).AppendLine();
        stringBuilder.Append("added: ").Append(_sizeChangedDebugData.AddedItemsCount).Append(", ");
        stringBuilder.Append("removed: ").Append(_sizeChangedDebugData.RemovedItemsCount).Append(", ");
        stringBuilder.Append("dead: ").Append(_sizeChangedDebugData.DeadItemsCount).AppendLine();
        if (_sizeChangedItemRefs.IsNotEmpty() && _sizeChangedItemRefs.First().Target is FrameworkElement frameworkElement)
        {
            stringBuilder.Append("first item: ").Append(frameworkElement.GetType().Name).Append(", ");
            stringBuilder.Append("location: ").Append(GetItemWorksheetLocation(frameworkElement as IWhiteboardItem)).AppendLine();
            stringBuilder.Append("loaded: ").Append(frameworkElement.IsLoaded).Append(", ");
            stringBuilder.Append("visible: ").Append(frameworkElement.IsVisible).Append(", ");
            stringBuilder.Append("measured: ").Append(frameworkElement.IsMeasureValid).Append(", ");
            stringBuilder.Append("arranged: ").Append(frameworkElement.IsArrangeValid).AppendLine();
        }
        DebugTools.FailAssertion(stringBuilder.ToString());
    }
    private void OnSizeChangedItemLayoutUpdated(object sender, EventArgs e)
    {
        ProcessDispatchedSizeChangeNotifications("layout update");
    }
    private void ProcessDispatchedSizeChangeNotifications(string operation)
    {
        if (_dispatchingSizeChangeNotifications)
        {
            return;
        }
        if (DebugTools.DevAssert(_sizeChangedItemRefs.IsNotEmpty(), null).Failed)
        {
            return;
        }
        try
        {
            _dispatchingSizeChangeNotifications = true;
            _sizeChangedDebugData.NotifyAttemptsCount++;
            List<IWhiteboardItem> list;
            while (!(list = CollectItemsForSizeChangedNotification()).IsNullOrEmpty())
            {
                foreach (IWhiteboardItem item in list)
                {
                    if (!IsItemArrangeValid(item))
                    {
                        continue;
                    }
                    if (item is ICustomSplitItem)
                    {
                        (item as ICustomSplitItem).ValidateLayout();
                    }
                    FrameworkElement frameworkElement = FrameworkElementFacade.GetFrameworkElement(item);
                    IList<WhiteboardItemSizeChangedEventArgs> sizeChangedEventArgsList = GetSizeChangedEventArgsList(frameworkElement);
                    if (!DebugTools.DevAssert(!sizeChangedEventArgsList.IsNullOrEmpty(), null).Failed)
                    {
                        ClearSizeChangedEventArgsList(frameworkElement);
                        RemoveSizeChangedItem(item, frameworkElement, "middle of " + operation, attemptToClearList: false);
                        WhiteboardItemSizeChangedEventHandler worksheetItemSizeChangedHandler = GetWorksheetItemSizeChangedHandler(frameworkElement);
                        if (DebugTools.AssertNotNull(worksheetItemSizeChangedHandler, null).Passed)
                        {
                            worksheetItemSizeChangedHandler(item, sizeChangedEventArgsList);
                        }
                    }
                }
            }
            ClearSizeChangedItemRefsIfNeeded("end of " + operation);
        }
        finally
        {
            _dispatchingSizeChangeNotifications = false;
        }
    }
    private List<IWhiteboardItem> CollectItemsForSizeChangedNotification()
    {
        IEnumerable<IWhiteboardItem> items = from weakRef in _sizeChangedItemRefs
            select weakRef.Target as IWhiteboardItem into item
            where item != null && IsItemArrangeValid(item)
            select item;
        return SortItemsByLocation(items, descending: false).ToList();
    }
    private void AddSizeChangedItem(IWhiteboardItem item, FrameworkElement elem)
    {
        DebugTools.DevAssert(item == elem, null);
        if (_sizeChangedItemRefs.FirstOrDefault((WeakReference weakRef) => weakRef.Target == item) == null)
        {
            _sizeChangedItemRefs.Add(new WeakReference(item));
            elem.LayoutUpdated += OnSizeChangedItemLayoutUpdated;
            _sizeChangedDebugData.AddedItemsCount++;
        }
    }
    private void RemoveSizeChangedItem(IWhiteboardItem item, FrameworkElement elem, string operation, bool attemptToClearList = true)
    {
        DebugTools.DevAssert(item == elem, null);
        WeakReference weakReference = _sizeChangedItemRefs.FirstOrDefault((WeakReference weakRef) => weakRef.Target == item);
        if (weakReference != null)
        {
            elem.LayoutUpdated -= OnSizeChangedItemLayoutUpdated;
            _sizeChangedItemRefs.Remove(weakReference);
            _sizeChangedDebugData.RemovedItemsCount++;
            if (attemptToClearList)
            {
                ClearSizeChangedItemRefsIfNeeded(operation);
            }
        }
    }
    public IWhiteboardItem GetItemPrintView(IWhiteboardItem item)
    {
        return AttachedProperties.GetPrintView(FrameworkElementFacade.GetFrameworkElement(item));
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
    public IEnumerable<IWhiteboardItem> SmartDescendantsSatisfyingCondition(IWhiteboardItem startItem, Predicate<IWhiteboardItem> condition) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void HookupAreaBorderChanged(IWhiteboardItem item)
    {
        if (item is ICollapsibleAreaControl collapsibleAreaControl)
        {
            collapsibleAreaControl.BorderStateChanged += OnAreaBorderStateChanged;
        }
    }
    private void OnAreaBorderStateChanged(ICollapsibleAreaControl area)
    {
        (((area as DependencyObject))?.TryFindVisualAncestor<IWorksheetControl>()).UpdateIsRegionBorderShownState();
    }
    public WorksheetItemManager() { }
}
