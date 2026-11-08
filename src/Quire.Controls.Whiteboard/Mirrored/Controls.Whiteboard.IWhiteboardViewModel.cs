#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.Wpf;

namespace Quire.Controls.Whiteboard;

public partial interface IWhiteboardViewModel
{
    WhiteboardItemsCollection WhiteboardItems { get; }

    Point InsertionPoint { get; set; }

    IWhiteboardItem ActiveDescendant { get; }

    IList<IWhiteboardItem> SelectedDescendants { get; }

    IAlignmentGrid Grid { get; }

    bool IsFocused { get; set; }

    Size ContentSize { get; set; }

    double ContentWidth { get; set; }

    double ContentHeight { get; set; }

    Size MinimumSize { get; set; }

    Size MaximumSize { get; set; }

    Point GridPosition { get; set; }

    CommandBindingCollection CommandBindings { get; }

    DragMode CurrentDragMode { get; set; }

    INotifyCollectionRangeChanged<IWhiteboardItem> SelectedItems { get; }

    IEnumerable<IWhiteboardItem> ActiveOrSelectedItems { get; }

    bool CanAddItems { get; }

    bool HandleUserInput { get; }

    bool IsCrosshairVisible { get; set; }

    Point InsertionPointForResize { get; }

    bool ForbidGrowingMovementRight { get; set; }

    bool CanInsertVerticalSpace { get; }

    bool CanDeleteVerticalSpaceAbove { get; }

    bool CanDeleteVerticalSpaceBelow { get; }

    bool IsSelected { set; }

    Point DefaultInsertionPoint { get; }

    bool IsTabNavigatable { get; }

    event Action<Size, Size> ChildCausedResize;

    event Action RequestMaximumSize;

    event Action Activated;

    event Action<IWhiteboardItem> RequestActivatePartContainingChild;

    event EventHandler Changed;

    event Action<IEnumerable<IWhiteboardItem>> CopyWhiteboardItems;

    event Action<IEnumerable<IWhiteboardItem>> RequestBringIntoView;

    void AddItemAndActivate(IWhiteboardItem item);

    void RemoveWhiteboardItem(IWhiteboardItem item);

    void AddItemAtLocation(IWhiteboardItem item, Point location);

    void AddItems(Dictionary<IWhiteboardItem, Point> addItemsAtLocation);

    Point GetItemLocation(IWhiteboardItem item);

    void SetItemLocation(IWhiteboardItem item, Point location);

    void SetItemLocationToNearestGridIntersection(IWhiteboardItem item, Point location);

    void SetItemGridPosition(IWhiteboardItem item, Point location);

    Point GetItemGridPosition(IWhiteboardItem item);

    Size GetItemDesiredSize(IWhiteboardItem item);

    void SetItemSize(IWhiteboardItem item, Size size, DragMode dragMode = DragMode.None);

    void OnRequestMoveAndResize(IWhiteboardControl whiteboardControl, ResizeEventMouseArgs e);

    void SelectItems(IEnumerable<IWhiteboardItem> itemsToSelect);

    void UnSelectItems(IEnumerable<IWhiteboardItem> itemsToDeselect);

    void SelectItem(IWhiteboardItem item);

    void UnSelectItem(IWhiteboardItem item);

    void SelectAllItems();

    void ToggleItemSelection(IWhiteboardItem item, bool includeActiveItem);

    bool GetItemIgnoreForHorizontalMovementAndWidth(IWhiteboardItem item);

    void SelectContiguous(IWhiteboardItem item);

    void SelectRegionsInBounds(Rect rect);

    void TrySelectRegionsInBounds(Rect rect, WhiteboardControl wbc);

    bool CopySelection();

    void CutSelection();

    void Paste(IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation);

    void SaveState();

    void ClearSavedState();

    void RestoreSavedState();

    void ClearState();

    void SaveResizeInformation();

    void RestoreResizeInformation();

    Vector? NavigateInto(Direction direction, Point point);

    Point GetNavigationStartLocation(Direction direction);

    void FireRequestMaximumSize();

    void FireChildCausedResize(Size oldSize, Size newSize);

    void UpdateSizeLimits(Size maximumSize);

    bool FindOncePrivate(string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions);

    void InsertVerticalSpace(int numberOfLines, bool pushInsertionPoint, bool ignoreItemsOnCurrentLine);

    void DeleteVerticalSpaceAbove(int numberOfLines);

    void DeleteVerticalSpaceBelow(int numberOfLines);

    double LimitOffsetBasedOnItemsToMove(IEnumerable<IWhiteboardItem> itemsToMove, double offsetY, Direction direction, Point insertionPointLocation);

    WhiteboardViewModel.RemoveItemsCommand GetRemoveItemsCommand(string commandName, IDictionary<IWhiteboardItem, Point> itemsAtLocations);

    void ClearSelectedItems();

    void ActivateActiveItem();

    void Activate(bool isActive);

    void HandleGridCellSizeChanged(Size oldSize, Size newSize);

    IEnumerable<IWhiteboardItem> SortItemsByLocation(IEnumerable<IWhiteboardItem> items, bool descending);

    Size ResizeAndNotify(IEnumerable<IWhiteboardItem> list, Size oldSize);

    bool IsValidLocation(IWhiteboardItem item, IDictionary<IWhiteboardItem, Point> newLocations, bool allowGrowthDownOrRight);

    bool HandleMouseDown(IWhiteboardControl sender, Point position, MouseButton button);

    void PreviewMouseLeftButtonDownHandler(object sender, MouseButtonEventArgs eventArgs);

    void PreviewMouseRightButtonDownHandler(object sender, MouseButtonEventArgs eventArgs);

    void PreviewMouseLeftButtonUpHandler(object sender, RoutedEventArgs eventArgs);

    void PreviewMouseRightButtonUpHandler(object sender, RoutedEventArgs eventArgs);

    bool HandleMouseUp(IWhiteboardControl sender, Point position, MouseButton button);

    IEnumerable<IWhiteboardItem> RecursivelyEnumerateDescendantItemssBypassWhiteboard();

    bool CanSeparateOverlappedItems(Dimension dimension);

    void SeparateOverlappedItems(Dimension dimension);

    Point GetDescendantInsertionPoint();

    void HandleCaretLeavesTextArea(CommonRoutedEvents.CaretLeavesTextRegionEventArgs args);

    void TabInto(FocusNavigationDirection dir);

    void HandleTabOutOfText(CustomRoutedEventArgs<FocusNavigationDirection> eventArgs);

    Rect GetItemRect(IWhiteboardItem item);
}
