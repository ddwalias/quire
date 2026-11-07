#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Quire.Wpf;

namespace Quire.Controls.Core.Whiteboard;

public partial interface IWhiteboardControl
{
    Point GridPosition { get; set; }
    Point InsertionPoint { get; set; }
    Point DefaultInsertionPoint { get; }
    bool IsCrosshairVisible { get; set; }
    IEnumerable<IWhiteboardItem> ActiveOrSelectedItems { get; }
    IWhiteboardItem ActiveDescendant { get; }
    IList<IWhiteboardItem> SelectedDescendants { get; }
    Size GridCellSize { get; set; }
    INotifyCollectionRangeChanged<IWhiteboardItem> SelectedItems { get; }
    Point InsertionPointForResize { get; }
    bool ForbidGrowingMovementRight { set; }
    IWhiteboardItemManager WhiteboardItemManager { get; }
    event Action<IWhiteboardControl, Rect> RequestScrollTo;
    event Action<IWhiteboardControl, Size, Size> ChildTriggeredResize;
    event Action<IWhiteboardControl> RequestMaximumSize;
    event Action Activated;
    event Action<IWhiteboardItemContainer, IWhiteboardItem> RequestActivatePartContainingChild;
    Point GetItemLocation(IWhiteboardItem item);
    Rect GetItemRect(IWhiteboardItem item);
    void SetItemLocation(IWhiteboardItem item, Point location);
    Point GetItemGridPosition(IWhiteboardItem item);
    void ActivateWhiteboard(bool isActive);
    void SetInsertionPointWhenLoaded(Point insertionPoint);
    void SetItemGridPosition(IWhiteboardItem item, Point location);
    void AddItems(Dictionary<IWhiteboardItem, Point> addItemsAtLocation);
    void AddItemAtLocation(IWhiteboardItem item, Point location, bool forceAlignment = false);
    void PasteItems(IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation);
    void ToggleItemSelection(IWhiteboardItem item, bool includeActiveItem);
    void SelectContiguous(IWhiteboardItem item);
    void SelectAllItems();
    void ClearSelection();
    void SelectItems(IEnumerable<IWhiteboardItem> itemsToSelect);
    void UnSelectItems(IEnumerable<IWhiteboardItem> itemsToRemove);
    void SelectItem(IWhiteboardItem item);
    void UnSelectItem(IWhiteboardItem item);
    bool CopySelection();
    void CutSelection();
    void SaveVisualState();
    void RestoreVisualState();
    void ClearVisualState();
    void SaveResizeInformation();
    void RestoreResizeInformation();
    Vector? NavigateInto(Direction direction, Point point);
    Point GetNavigationStartLocation(Direction direction);
    void UpdateSizeLimits(Size maximumSize);
    void InsertVerticalSpace(object sender, ExecutedRoutedEventArgs e);
    void DeleteVerticalSpaceAbove(object sender, ExecutedRoutedEventArgs e);
    void DeleteVerticalSpaceBelow(object sender, ExecutedRoutedEventArgs e);
    void GridCellSizeChangedHandler(Size oldSize, Size newSize);
    IEnumerable<IWhiteboardItem> RecursivelyEnumerateDescendantItemssBypassWhiteboard();
    bool CanSeparateOverlappedItems(Dimension dimension);
    void SeparateOverlappedItems(Dimension dimension);
    Point GetDescendantInsertionPoint();
}
