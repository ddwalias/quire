#nullable disable
using System;
using System.Windows;
using System.Windows.Input;
using Quire.Core;

namespace Quire.Controls.Core.Whiteboard;

public partial interface IContainerItemManager
{
    void SelectItem(IWhiteboardItem item);
    void DeselectItem(IWhiteboardItem item);
    bool IsItemSelected(IWhiteboardItem item);
    void ActivateItem(IWhiteboardItem item);
    void DeactivateItem(IWhiteboardItem item);
    bool IsItemActivated(IWhiteboardItem item);
    void SetIsTabNavigatable(IWhiteboardItem item, bool isTabStop);
    bool GetIsTabNavigatable(IWhiteboardItem item);
    bool IsTabNavigatable(IWhiteboardItem item);
    void TabInto(IWhiteboardItem item, FocusNavigationDirection dir);
    void BringIntoView(IWhiteboardItem item, Rect areaOfInterest);
    void SetIsBringToView(IWhiteboardItem item, bool value);
    bool GetIsBringToView(IWhiteboardItem item);
    bool IsSuppressBringIntoView(IWhiteboardItem item);
    void SuppressBringIntoViewIfNeededAndExecute(IWhiteboardItem item, Action action, bool suppressBringToView);
    WhiteboardItemSizeChangedEventHandler HookupSizeChanged(IWhiteboardItem item, Action<IWhiteboardItem, Size> sizeChangedHandler);
    WhiteboardItemSizeChangedEventHandler HookupSizeChanged(IWhiteboardItem item, Action<IWhiteboardItem, Size, Size> sizeChangedHandler);
    void HookupSizeChanged(IWhiteboardItem item, WhiteboardItemSizeChangedEventHandler sizeChangedHandler);
    void UnhookSizeChanged(IWhiteboardItem item, WhiteboardItemSizeChangedEventHandler sizeChangedHandler);
    void HookupMouseEnter(IWhiteboardItem item, Action<IWhiteboardItem> mouseEnterHandler);
    void UnhookMouseEnter(IWhiteboardItem item);
    void HookupDragEnter(IWhiteboardItem item, Action<IWhiteboardItem> dragEnterHandler);
    void UnhookDragEnter(IWhiteboardItem item);
    void ProhibitSizeChangedEventHandler(IWhiteboardItem item, bool prohibit);
    bool IsSizeChangedEventHandlerProhibited(IWhiteboardItem item);
    bool GetItemHandlesContextMenus(IWhiteboardItem item);
    void SetItemHandlesContextMenus(IWhiteboardItem item, bool handlesContextMenus);
    void ClearItemHandlesContextMenus(IWhiteboardItem item);
    bool GetCanActivate(IWhiteboardItem item);
    void SetCanActivate(IWhiteboardItem item, bool value);
    bool GetItemIsInCopy(IWhiteboardItem item);
    void SetItemIsInCopy(IWhiteboardItem item, bool value);
    void ClearItemIsInCopy(IWhiteboardItem item);
    void HookupLeftMouseButtonDown(IWhiteboardItem item, Action<IWhiteboardItem> handler);
    void SetItemSupportsInputsOutputs(IWhiteboardItem item, bool value);
    bool GetItemSupportsInputsOutputs(IWhiteboardItem item);
    void ClearItemSupportsInputsOutputs(IWhiteboardItem item);
    void SetItemIsOnVisiblePage(IWhiteboardItem item, bool visible);
    bool GetItemIsOnVisiblePage(IWhiteboardItem item);
    void ClearItemIsOnVisiblePage(IWhiteboardItem item);
    void SetElementIsOnVisiblePage(IMaybeHasPointThatSnapsToPixel item, bool isPageVisible);
    bool GetElementIsOnVisiblePage(IMaybeHasPointThatSnapsToPixel item);
    void AddItemIsOnVisiblePageChangedHandler(IWhiteboardItem item, DependencyPropertyChangedEventHandler handler);
    void RemoveItemIsOnVisiblePageChangedHandler(IWhiteboardItem item, DependencyPropertyChangedEventHandler handler);
    void SetContainedInCollapsedWhiteboard(IWhiteboardItem item, bool isContainedInCollapsedWhiteboard);
    bool GetContainedInCollapsedWhiteboard(IWhiteboardItem item);
    void ClearContainedInCollapsedWhiteboard(IWhiteboardItem item);
}
