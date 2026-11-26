using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class WorksheetControlContent
{

    private global::System.Windows.Point _insertionPoint;

    private global::Quire.Controls.Core.IWhiteboardItem _activeItem;

    public WorksheetControlContent(global::Quire.Controls.Worksheet.IWorksheetModel model, global::Quire.Controls.Core.IRestrictedAreaAlignmentGrid alignmentGrid, global::Quire.Controls.Core.CustomSplitItemRenderSizeDelegate splitItemRenderSizeDelegate = default)
    {
        _model = model;
        _worksheetGrid = alignmentGrid;
        _customSplitItemRenderSizeDelegate = splitItemRenderSizeDelegate;
        _selectedItems = new ObservableRangeCollection<IWhiteboardItem>();
        WhiteboardItems.RangeChanged += HandleItemsCollectionChanged;
        SelectedItems.RangeChanged += HandleSelectedItemsChanged;
    }

    private readonly global::Quire.Controls.Worksheet.IWorksheetModel _model;

    private static IWorksheetItemManager WorksheetItemManager => WorksheetViewModel.ItemManager;

    IContainerItemManager IWhiteboardItemContainer.ContainerItemManager => WorksheetItemManager;

    IWhiteboardItemManager IWhiteboardControl.WhiteboardItemManager => WorksheetItemManager;

    IUIElementArrangeInfoProvider IHasChildArrangeInfoProvider.ChildArrangeInfoProvider => WorksheetItemManager;

    public IEnumerable<IWhiteboardItem> ActiveOrSelectedItems
    {
        get
        {
            if (ActiveItem == null)
            {
                return SelectedItems.ToList();
            }
            return new List<IWhiteboardItem> { ActiveItem };
        }
    }

    public Point GridPosition
    {
        get
        {
            throw new NotImplementedException();
        }

        set => throw new NotImplementedException();
    }

    public Point InsertionPoint
    {
        get => _insertionPoint;
        set => SetInsertionPoint(value, false, GridRestriction.Skip);
    }

    public Point DefaultInsertionPoint => default;

    public bool IsCrosshairVisible
    {
        get => ActiveOrSelectedItems.Count() == 0;
        set => throw new NotImplementedException();
    }

    public IWhiteboardItem ActiveItem
    {
        get => _activeItem;
        set
        {
            if (_activeItem == value)
            {
                return;
            }
            if (_activeItem != null)
            {
                UnhookActiveItem(_activeItem);
                ActiveItem.Activate(false);
                WorksheetItemManager.DeactivateItem(ActiveItem);
                _model.CommitItemIfNeeded(_activeItem);
            }
            IWhiteboardItem activeItem = _activeItem;
            _activeItem = value;
            ActiveItemChanged(activeItem, _activeItem);
            if (_activeItem != null)
            {
                HookupActiveItem(_activeItem);
                ActiveItem.Activate(true);
                WorksheetItemManager.ActivateItem(ActiveItem);
                ClearSelection();
            }
        }
    }

    private void HookupActiveItem(IWhiteboardItem item)
    {
        if (item is IWhiteboardItemContainer)
        {
            item.PropertyChanged += ChildWhiteboardPropertyChangedHandler;
        }
    }

    private void UnhookActiveItem(IWhiteboardItem item)
    {
        if (item is IWhiteboardItemContainer)
        {
            item.PropertyChanged -= ChildWhiteboardPropertyChangedHandler;
        }
    }

    private void ChildWhiteboardPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        if (sender is IWhiteboardControl)
        {
            if (e.PropertyName == "ActiveDescendant")
            {
                ActiveDescendantChanged();
            }
            if (e.PropertyName == "SelectedDescendants")
            {
                SelectedDescendantsChanged();
            }
        }
        else if (e.PropertyName == "ActiveItem")
        {
            ActiveDescendantChanged();
        }
    }

    public IWhiteboardItem ActiveDescendant
    {
        get
        {
            if (ActiveItem == null)
            {
                return null;
            }
            IWhiteboardItemContainer whiteboardItemContainer = ActiveItem as IWhiteboardItemContainer;
            if (whiteboardItemContainer == null)
            {
                return ActiveItem;
            }
            IWhiteboardControl whiteboardControl = ActiveItem as IWhiteboardControl;
            if (whiteboardControl == null)
            {
                return whiteboardItemContainer.ActiveItem ?? ActiveItem;
            }
            return whiteboardControl.ActiveDescendant ?? ActiveItem;
        }
    }

    public IList<IWhiteboardItem> SelectedDescendants
    {
        get
        {
            if (ActiveItem == null)
            {
                return SelectedItems.ToList();
            }
            IWhiteboardControl whiteboardControl = ActiveItem as IWhiteboardControl;
            if (whiteboardControl == null)
            {
                return SelectedItems.ToList();
            }
            return whiteboardControl.SelectedDescendants;
        }
    }

    public WhiteboardItemsCollection WhiteboardItems => _model.WhiteboardItems;

    public ObservableRangeCollection<IWorksheetPageBreak> PageBreaks => _model.PageBreaks;

    public Size GridCellSize
    {
        get => _worksheetGrid.GridCellSize;
        set => throw new NotImplementedException();
    }

    public bool CanAddItems => true;

    public INotifyCollectionRangeChanged<IWhiteboardItem> SelectedItems => _selectedItems;

    public Point GetItemLocation(IWhiteboardItem item) => _model.GetItemWorksheetLocation(item);

    public Rect GetItemRect(IWhiteboardItem item) => WorksheetItemManager.GetItemRect(item);

    public Point GetItemLocation(IWorksheetPageBreak pageBreak) => _model.GetItemWorksheetLocation(pageBreak);

    public void SetItemLocation(IWorksheetPageBreak pageBreak, Point location) => _model.SetItemWorksheetLocation(pageBreak, location);

    public Point CalculateItemWorksheetLocation(IWhiteboardItem item, Point worksheetLocation, GridRestriction restriction)
    {
        if (worksheetLocation.X + WorksheetItemManager.GetRenderSize(item).Width > _worksheetGrid.GridArea.Width && !WorksheetItemManager.GetIgnoreForHorizontalMovementAndWidth(item))
        {
            UpdateWorksheetGridForNonprintableSpace(worksheetLocation.X + WorksheetItemManager.GetRenderSize(item).Width);
        }
        worksheetLocation = _worksheetGrid.GetNearestIntersectionPointDisregardingBounds(worksheetLocation, restriction, Direction.Down);
        if (WorksheetItemManager.GetIgnoreForHorizontalMovementAndWidth(item))
        {
            worksheetLocation = new Point(0.0, worksheetLocation.Y);
        }
        return worksheetLocation;
    }

    public void SetItemLocation(IWhiteboardItem item, Point worksheetLocation)
    {
        GridRestriction restriction = (WorksheetItemManager.GetShouldAssertOnLocation(item) ? GridRestriction.Assert : GridRestriction.Ignore);
        _model.SetItemWorksheetLocation(item, CalculateItemWorksheetLocation(item, worksheetLocation, restriction));
    }

    public Point GetItemGridPosition(IWhiteboardItem item)
    {
        throw new NotImplementedException();
    }

    public void ActivateWhiteboard(bool isActive)
    {
    }

    public void SetInsertionPointWhenLoaded(Point insertionPoint)
    {
        throw new NotImplementedException();
    }

    public void SetItemGridPosition(IWhiteboardItem item, Point location)
    {
        throw new NotImplementedException();
    }

    public void AddItem(IWhiteboardItem item)
    {
        throw new NotImplementedException();
    }

    public void AddItems(Dictionary<IWhiteboardItem, Point> addItemsAtLocation)
    {
        AddItemsCommand command = new AddItemsCommand(this, addItemsAtLocation);
        command.Execute();
    }

    public void RemoveItem(IWhiteboardItem item)
    {
        _model.RemoveItem(item);
    }

    public bool IsTabNavigatable => throw new NotImplementedException();

    public void TabInto(FocusNavigationDirection dir)
    {
        throw new NotImplementedException();
    }

    public void AddItemAtLocation(IWhiteboardItem item, Point location, bool forceAlignment = false)
    {
        GridRestriction restriction = (WorksheetItemManager.GetShouldAssertOnLocation(item) ? GridRestriction.Assert : GridRestriction.Ignore);
        if (forceAlignment)
        {
            location = _worksheetGrid.GetNearestIntersectionPointDisregardingBounds(location, restriction, Direction.Right, Direction.Down);
        }
        if (location.X + WorksheetItemManager.GetRenderSize(item).Width > _worksheetGrid.GridArea.Width)
        {
            UpdateWorksheetGridForNonprintableSpace(location.X + WorksheetItemManager.GetRenderSize(item).Width);
        }
        if (WorksheetItemManager.GetIgnoreForHorizontalMovementAndWidth(item))
        {
            location = new Point(0.0, location.Y);
        }
        _model.AddItemAtLocation(item, location);
    }

    public void AddPageBreakAtLocation(IWorksheetPageBreak pageBreak, Point location, bool forceAlignment = false)
    {
        _model.AddItemAtLocation(pageBreak, location);
    }

    public void CommitItem(IWhiteboardItem item)
    {
        _model.CommitItem(item);
    }

    public void CommitActiveItem()
    {
        if (ActiveItem != null)
        {
            _model.CommitItemIfNeeded(ActiveItem);
        }
    }

    public void ToggleItemSelection(IWhiteboardItem item, bool includeActiveItem)
    {
        if (ActiveItem == item)
        {
            SelectItem(item);
        }
        else if (SelectedItems.Contains(item))
        {
            UnSelectItem(item);
            if (SelectedItems.Count() == 0)
            {
                item.Activate(true);
            }
        }
        else if (includeActiveItem && ActiveItem != null)
        {
            SelectItems(new IWhiteboardItem[2] { ActiveItem, item });
        }
        else
        {
            SelectItem(item);
        }
    }

    public void SelectContiguous(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SelectAllItems()
    {
        SelectItems(WhiteboardItems);
    }

    public void ClearSelection()
    {
        if (_selectedItems.Count() > 0)
        {
            _selectedItems.Clear();
        }
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("closure ordinal anchor; body pending")]
    public void SelectItems(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemsToSelect) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private readonly global::Quire.Controls.Core.IRestrictedAreaAlignmentGrid _worksheetGrid;

    public void UnSelectItems(IEnumerable<IWhiteboardItem> itemsToRemove)
    {
        if (_selectedItems.Any())
        {
            _selectedItems.RemoveRange(itemsToRemove);
        }
    }

    public void SelectItem(IWhiteboardItem item)
    {
        SelectItems(new IWhiteboardItem[1] { item });
    }

    public void UnSelectItem(IWhiteboardItem item)
    {
        UnSelectItems(new IWhiteboardItem[1] { item });
    }

    public bool CopySelection()
    {
        if (_copyWhiteboardItems != null)
        {
            return _copyWhiteboardItems(SelectedItems);
        }
        return false;
    }

    public void CutSelection()
    {
        if (CopySelection())
        {
            DeleteSelection();
        }
    }

    public void PasteItems(IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation)
    {
        PasteItemsCommand command = new PasteItemsCommand(this, pasteItemsAtLocation);
        command.Execute();
    }

    public void SaveVisualState()
    {
        if (ActiveItem == null && SelectedItems.Count() == 0)
        {
            _savedInsertionPoint = InsertionPoint;
        }
        else
        {
            _savedInsertionPoint = null;
        }
        _savedSelectionList = SelectedItems.ToList();
        _savedActiveItem = ActiveDescendant;
    }

    public void RestoreVisualState()
    {
        if (_savedInsertionPoint.HasValue)
        {
            InsertionPoint = _savedInsertionPoint.Value;
            InsertionPointSet();
        }
        else if (_savedActiveItem != null)
        {
            _savedActiveItem.Activate(true);
        }
        else if (_savedSelectionList != null)
        {
            SelectItems(_savedSelectionList);
        }
    }

    public void ClearVisualState()
    {
        ActiveItem = null;
        ClearSelection();
    }

    public void SaveResizeInformation() => throw new NotImplementedException();

    public void RestoreResizeInformation() => throw new NotImplementedException();

    public Point InsertionPointForResize
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public bool ForbidGrowingMovementRight
    {
        set => throw new NotImplementedException();
    }

    public Vector? NavigateInto(Direction direction, Point point)
    {
        throw new NotImplementedException();
    }

    private Size GetItemRenderSize(IWhiteboardItem item)
    {
        Size renderSize;
        if (item is ICustomSplitItem && _customSplitItemRenderSizeDelegate != null)
        {
            renderSize = _customSplitItemRenderSizeDelegate(item as ICustomSplitItem);
        }
        else
        {
            renderSize = WorksheetItemManager.GetRenderSize(item);
        }
        return renderSize;
    }

    public global::System.Windows.Point GetNavigationStartLocation(global::Quire.Direction direction)
    {
        Point location = InsertionPoint;
        if (ActiveItem != null)
        {
            if (ActiveItem is IWhiteboardControl whiteboardControl)
            {
                Point itemLocation = GetItemLocation(ActiveItem);
                location = whiteboardControl.GetNavigationStartLocation(direction);
                Size renderSize = WorksheetItemManager.GetRenderSize(whiteboardControl);
                switch (direction)
                {
                    case global::Quire.Direction.Up:
                        location = new Point(location.X, 0.0);
                        break;
                    case global::Quire.Direction.Left:
                        location = new Point(0.0, location.Y);
                        break;
                    case global::Quire.Direction.Down:
                        location = new Point(location.X, renderSize.Height);
                        break;
                    case global::Quire.Direction.Right:
                        location = new Point(renderSize.Width, location.Y);
                        break;
                }
                location = new TranslateTransform(itemLocation.X, itemLocation.Y).Transform(location);
            }
            else
            {
                Point itemLocation = GetItemLocation(ActiveItem);
                Size itemRenderSize = GetItemRenderSize(ActiveItem);
                location = itemLocation;
                switch (direction)
                {
                    case global::Quire.Direction.Up:
                        location.Offset(0.0, ActiveItem.VerticalAlignmentOffset);
                        break;
                    case global::Quire.Direction.Right:
                        location.Offset(itemRenderSize.Width, 0.0);
                        break;
                    case global::Quire.Direction.Down:
                        location.Offset(0.0, ActiveItem.VerticalAlignmentOffset + itemRenderSize.Height);
                        break;
                }
            }
        }
        return location;
    }

    public event Action<IWhiteboardControl, Rect> RequestScrollTo;

    public event Action<IWhiteboardControl, Size, Size> ChildTriggeredResize;

    public event Action<IWhiteboardControl> RequestMaximumSize;

    public event Action Activated;

    public event Action<IWhiteboardItemContainer, IWhiteboardItem> RequestActivatePartContainingChild = delegate { };

    public void SetCopyWhiteboardItemsDelegate(Func<IEnumerable<IWhiteboardItem>, bool> copyWhiteboardItems)
    {
        _copyWhiteboardItems = copyWhiteboardItems;
    }

    public void UpdateSizeLimits(Size maximumSize)
    {
        throw new NotImplementedException();
    }

    public void InsertVerticalSpace(object sender, ExecutedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void DeleteVerticalSpaceAbove(object sender, ExecutedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void DeleteVerticalSpaceBelow(object sender, ExecutedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void GridCellSizeChangedHandler(Size oldSize, Size newSize)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IWhiteboardItem> RecursivelyEnumerateDescendantItemssBypassWhiteboard() =>
        global::Quire.Controls.Whiteboard.WhiteboardViewModel.RecursivelyEnumerateDescendantsIgnoringTypes(WhiteboardItems);

    public bool CanSeparateOverlappedItems(Dimension dimension)
    {
        throw new NotImplementedException();
    }

    public void SeparateOverlappedItems(Dimension dimension) => throw new NotImplementedException();

    public Point GetDescendantInsertionPoint()
    {
        throw new NotImplementedException();
    }

    public double ContentAreaWidth
    {
        private get => _contentAreaWidth;
        set
        {
            _contentAreaWidth = value;
            _worksheetGrid.GridArea = new Rect(new Size(ContentAreaWidth, _worksheetGrid.GridArea.Height));
        }
    }

    public void UpdateItemLocationsAfterGridSizeChanged(Size oldGridCellSize)
    {
        foreach (IWhiteboardItem item in _model.WhiteboardItems.ToList())
        {
            Point itemLocation = GetItemLocation(item);
            Point value = new Point(itemLocation.X / oldGridCellSize.Width, itemLocation.Y / oldGridCellSize.Height);
            SetItemLocation(item, _worksheetGrid.ConvertGridCoordinatesToDips(value));
        }
    }

    public bool HandleDelete()
    {
        if (SelectedItems.Count() > 0)
        {
            DeleteSelection();
            return true;
        }
        return false;
    }

    private void DeleteSelection()
    {
        WorksheetContentCommand command = new DeleteSelectedItemsCommand(this, "Delete selected items");
        command.Execute();
    }

    public void MoveItemsBy(IEnumerable<IWhiteboardItem> items, Vector offset)
    {
        throw new NotImplementedException("The move command from the worksheet view model should be reviewed and placed here. For now this method cannot be used.");
    }

    public void HandleDirectionalMove(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public event Action InsertionPointSet = () => { };

    public event Action<IWhiteboardItem, IWhiteboardItem> ActiveItemChanged = delegate { };

    public event Action ActiveDescendantChanged = () => { };

    public event Action SelectedDescendantsChanged = () => { };

    public event Action<double> NonprintableSpaceWidthIncrease = delegate { };

    public bool BoundDirectionalMoveByContentWidth { private get; set; }

    public StaticMathFormat MathFormat { get; set; }

    public IDictionary<IdLabels, ExtendedFont> LabeledIdFormat { get; set; }

    public void SetWidthWithNonprintableSpace(double newWidth)
    {
        if (newWidth > _worksheetGrid.GridArea.Width)
        {
            UpdateWorksheetGridForNonprintableSpace(newWidth);
        }
    }

    public IEnumerable<IWhiteboardItem> SortItemsByLocation(IEnumerable<IWhiteboardItem> items, bool descending)
    {
        return WorksheetItemManager.SortItemsByLocation(items, descending);
    }

    public bool CheckPotentialMove(Dictionary<IWhiteboardItem, Point> itemsAtNewLocationsRaw, bool preventRestrictedItemLocations)
    {
        Dictionary<IWhiteboardItem, Point> itemsAtNewLocations = itemsAtNewLocationsRaw.ToDictionary(pair => pair.Key, pair => CalculateItemWorksheetLocation(pair.Key, pair.Value, GridRestriction.Ignore));
        Func<IWhiteboardItem, Point, bool> movesToALocation = (item, loc) =>
        {
            return loc != GetItemLocation(item);
        };
        bool flag = itemsAtNewLocations.All(item => item.Key is ICollapsibleAreaControl || movesToALocation(item.Key, item.Value));
        if (flag)
        {
            Func<IWhiteboardItem, double> getItemTop = item =>
            {
                Point val = itemsAtNewLocations[item];
                return val.Y + item.VerticalAlignmentOffset;
            };
            double itemsTopEdge = itemsAtNewLocations.Min(item => getItemTop(item.Key));
            flag = !(from item in WhiteboardItems.Except(itemsAtNewLocations.Keys)
                     where item is ICollapsibleAreaControl
                     select item).Any(item =>
            {
                Rect itemRect = WorksheetItemManager.GetItemRect(item);
                return itemsTopEdge.IsBetween(itemRect.Top, itemRect.Bottom);
            });
        }

        if (flag & preventRestrictedItemLocations)
        {
            flag = itemsAtNewLocations.Values.All(IsValidInsertionPoint);
        }

        return flag;
    }

    public bool HasRegionsWithConverterAnnotations => this.HasRegionsWithConverterAnnotations();

    public IEnumerable<UIElement> RegionsWithConverterAnnotations => this.RegionsWithConverterAnnotations();

    private void HandleItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                ItemsAdded(args.NewItems.OfType<IWhiteboardItem>().ToList());
                break;
            case NotifyCollectionChangedAction.Remove:
                ItemsRemoved(args.OldItems.OfType<IWhiteboardItem>().ToList());
                break;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", args.Action);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
        }
    }

    private void ItemsRemoved(IList<IWhiteboardItem> items)
    {
        UnSelectItems(items);
        items.GetRegionsOfType<IOleBaseObjectControl>().Each(ole => ole.CloseOle());
        items.GetRegionsOfType<ISpellCheckRegion>().Each(text => text.DisableSpellCheck());
    }

    private readonly global::Quire.Controls.Core.CustomSplitItemRenderSizeDelegate _customSplitItemRenderSizeDelegate;

    private global::System.Nullable<global::System.Windows.Point> _savedInsertionPoint;

    private global::System.Collections.Generic.List<global::Quire.Controls.Core.IWhiteboardItem> _savedSelectionList;

    private global::Quire.Controls.Core.IWhiteboardItem _savedActiveItem;

    private double _contentAreaWidth;

    private global::Quire.Wpf.IObservableRangeCollection<global::Quire.Controls.Core.IWhiteboardItem> _selectedItems;

    private global::System.Func<global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem>, bool> _copyWhiteboardItems;

    private void ItemsAdded(IEnumerable<IWhiteboardItem> items)
    {
    }

    private void HandleSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.Action == NotifyCollectionChangedAction.Add && SelectedItems.Count() == args.NewItems.Count)
        {
            ActiveItem = null;
        }
    }

    public void HandleItemGotFocus(IWhiteboardItem item)
    {
        ActiveItem = item;
    }

    private bool CanMoveToInsertionPoint(Point insertionPoint)
    {
        bool flag = SelectedItems.Count() > 0 || (insertionPoint.X.IsGreaterThanOrEqual(0.0) && insertionPoint.Y.IsGreaterThanOrEqual(0.0));
        if (!BoundDirectionalMoveByContentWidth)
        {
            return flag;
        }
        if (flag)
        {
            return insertionPoint.X.IsLessThanOrEqual(ContentAreaWidth);
        }
        return false;
    }

    public global::Quire.Controls.Core.IWhiteboardItem GetItemAtLocation(global::System.Windows.Point location) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool IsValidInsertionPoint(Point point)
    {
        if (point.X.IsGreaterThanOrEqual(0.0) && point.Y.IsGreaterThanOrEqual(0.0))
        {
            return !_worksheetGrid.IsWithinRestrictedArea(point);
        }
        return false;
    }

    private void UpdateWorksheetGridForNonprintableSpace(double newWidth)
    {
        _worksheetGrid.GridArea = new Rect(0.0, 0.0, newWidth, _worksheetGrid.GridArea.Height);
        NonprintableSpaceWidthIncrease(newWidth);
    }

    public void Dispose()
    {
        StringBuilder stringBuilder = new StringBuilder();
        ClearSelection();
        if (_activeItem != null)
        {
            UnhookActiveItem(_activeItem);
            _activeItem = null;
        }
        foreach (IDisposable item in WhiteboardItems.OfType<IDisposable>())
        {
            try
            {
                item.Dispose();
            }
            catch (Exception ex)
            {
                stringBuilder.AppendFormat("Exception when disposing item {0}) : {1}\n", item.GetType().Name, ex.Message);
            }
        }
        WhiteboardItems.RemoveAll();
        DebugTools.DevAssert(stringBuilder.Length == 0, stringBuilder.ToString());
    }

    private void SetInsertionPoint(Point insertionPoint, bool forceSet, GridRestriction restriction)
    {
        if (insertionPoint.X >= _worksheetGrid.GridArea.Width && !BoundDirectionalMoveByContentWidth)
        {
            UpdateWorksheetGridForNonprintableSpace(insertionPoint.X + _worksheetGrid.GridCellSize.Width);
        }
        Point nearestIntersectionPointDisregardingBounds = _worksheetGrid.GetNearestIntersectionPointDisregardingBounds(insertionPoint, restriction, Direction.Down);
        if (!forceSet && _insertionPoint == nearestIntersectionPointDisregardingBounds && ActiveOrSelectedItems.FirstOrDefault() == null)
        {
            return;
        }
        _insertionPoint = nearestIntersectionPointDisregardingBounds;
        InsertionPointSet();
    }

    public StaticMathFormat TargetMathFormat => MathFormat;

    public IDictionary<IdLabels, ExtendedFont> TargetLabeledIdFormat => LabeledIdFormat;

    private void FireActivated()
    {
        Activated?.Invoke();
    }

    public ISnapToPixelHelper SnappingHelper => null;

    internal abstract partial class WorksheetContentCommand : global::Quire.Undo.IUndoableCommand
    {
        protected WorksheetControlContent WorksheetContent => _worksheetContent as WorksheetControlContent;

        public string Name { get; private set; }

        protected WorksheetContentCommand(IWorksheetContent worksheetContent, string name)
        {
            Name = name;
            _worksheetContent = worksheetContent;
        }

        public bool Execute()
        {
            return global::Quire.Undo.CommandBuffer.Execute(this);
        }

        public abstract bool Do();

        public abstract void Undo();

        public abstract void Redo();
    }

    internal partial class DeleteSelectedItemsCommand
    {
        public override void Redo()
        {
            WorksheetContent.FireActivated();
            Do();
        }
    }

    internal partial class MoveItemsCommand
    {
        public bool AssertOnRestrictedItemLocations
        {
            get => _assertOnRestrictedItemLocations;
            set => _assertOnRestrictedItemLocations = value;
        }

        public override void Redo()
        {
            WorksheetContent.FireActivated();
            Do();
        }

        private void MoveItemsToPositions(Dictionary<IWhiteboardItem, Point> itemsAtNewLocations)
        {
            foreach (KeyValuePair<IWhiteboardItem, Point> itemAtLocation in itemsAtNewLocations)
            {
                IWhiteboardItem item = itemAtLocation.Key;
                Point location = itemAtLocation.Value;
                WorksheetItemManager.SetShouldAssertOnLocation(item, AssertOnRestrictedItemLocations);
                _worksheetContent.SetItemLocation(item, location);
                WorksheetItemManager.ClearShouldAssertOnLocation(item);
            }

            WorksheetItemManager.BlockUntilLoaded();
        }
    }

    internal partial class AddItemsCommand
    {
        public override void Redo()
        {
            WorksheetContent.FireActivated();
            Do();
        }
    }

    internal partial class PasteItemsCommand
    {
        public override void Redo()
        {
            WorksheetContent.FireActivated();
            Do();
        }
    }

    internal partial class ResizeItemCommand
    {
        public override bool Do()
        {
            DoRedo(true);
            return true;
        }

        public override void Undo()
        {
            WorksheetContent.FireActivated();
            if (_resizedItem is IResizableElementWithDataStore resizable)
            {
                resizable.RestoreResizeData(_oldData);
                return;
            }

            WorksheetItemManager.SetSize(_resizedItem, _oldSize, _dragMode);
        }

        public override void Redo()
        {
            WorksheetContent.FireActivated();
            DoRedo(false);
        }

        private void DoRedo(bool doAction)
        {
            var resizable = _resizedItem as IResizableElementWithDataStore;
            Size newSize = _newSize;
            if (resizable != null)
            {
                if (doAction)
                {
                    _oldData = resizable.StoreResizeData(_oldSize, _newSize);
                }
                else
                {
                    newSize = resizable.CoerceNewSize(_newSize, _oldData);
                }
            }

            WorksheetItemManager.SetSize(_resizedItem, newSize, _dragMode);
        }
    }

    internal partial class ResizeItemCommandWithSizeEffects
    {
        public override void Redo()
        {
        }
    }

}

