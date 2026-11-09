#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Whiteboard;

public partial class WhiteboardViewModel
{
    internal WhiteboardViewModel(global::Quire.Controls.Whiteboard.IWhiteboardModel model) { }

    internal void HandleChildSizeChangedUnitTestWrapper(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Size oldSize, global::System.Windows.Size newSize)
    {
        HandleChildSizeChanged(item, oldSize, newSize);
    }

    private global::Quire.Controls.Core.RelayCommand _moveDownCommand;

    private global::Quire.Controls.Core.RelayCommand _moveUpCommand;

    private global::Quire.Controls.Core.RelayCommand _moveLeftCommand;

    private global::Quire.Controls.Core.RelayCommand _moveRightCommand;

    private global::Quire.Controls.Core.RelayCommand _carriageReturnCommand;

    private global::Quire.Controls.Core.RelayCommand _traverseForwardCommand;

    private global::Quire.Controls.Core.RelayCommand _traverseBackwardCommand;

    public ICommand MoveDownCommand
    {
        get
        {
            if (_moveDownCommand == null)
            {
                _moveDownCommand = new RelayCommand(_ => DirectionalMoveHandler(Direction.Down), _ => CanMove(Direction.Down));
            }

            return _moveDownCommand;
        }
    }

    public ICommand MoveUpCommand
    {
        get
        {
            if (_moveUpCommand == null)
            {
                _moveUpCommand = new RelayCommand(_ => DirectionalMoveHandler(Direction.Up), _ => CanMove(Direction.Up));
            }

            return _moveUpCommand;
        }
    }

    public ICommand MoveRightCommand
    {
        get
        {
            if (_moveRightCommand == null)
            {
                _moveRightCommand = new RelayCommand(_ => DirectionalMoveHandler(Direction.Right), _ => CanMove(Direction.Right));
            }

            return _moveRightCommand;
        }
    }

    public ICommand MoveLeftCommand
    {
        get
        {
            if (_moveLeftCommand == null)
            {
                _moveLeftCommand = new RelayCommand(_ => DirectionalMoveHandler(Direction.Left), _ => CanMove(Direction.Left));
            }

            return _moveLeftCommand;
        }
    }

    public ICommand CarriageReturnCommand
    {
        get
        {
            if (_carriageReturnCommand == null)
            {
                _carriageReturnCommand = new RelayCommand(_ => HandleCarriageReturn());
            }

            return _carriageReturnCommand;
        }
    }

    public ICommand TraverseForwardCommand
    {
        get
        {
            if (_traverseForwardCommand == null)
            {
                _traverseForwardCommand = new RelayCommand(_ => HandleTraverse(FocusNavigationDirection.Next), _ => CanTraverse(FocusNavigationDirection.Next));
            }

            return _traverseForwardCommand;
        }
    }

    public ICommand TraverseBackwardCommand
    {
        get
        {
            if (_traverseBackwardCommand == null)
            {
                _traverseBackwardCommand = new RelayCommand(_ => HandleTraverse(FocusNavigationDirection.Previous), _ => CanTraverse(FocusNavigationDirection.Previous));
            }

            return _traverseBackwardCommand;
        }
    }

    public static IWhiteboardItemManager WhiteboardItemMgr
    {
        get
        {
            if (_whiteboardItemMgr == null || !_whiteboardItemMgr.IsConnected)
            {
                return _whiteboardItemMgr = AppServiceRegistry.GetService<IWhiteboardItemManager>();
            }

            return _whiteboardItemMgr;
        }
    }

    public event global::System.Action<global::System.Windows.Size, global::System.Windows.Size> ChildCausedResize;

    public event global::System.Action RequestMaximumSize;

    public event global::System.Action Activated;

    public event global::System.Action<global::Quire.Controls.Core.IWhiteboardItem> RequestActivatePartContainingChild;

    public event global::System.Action<global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem>> RequestBringIntoView;

    public WhiteboardItemsCollection WhiteboardItems
    {
        get
        {
            return _whiteboardModel.WhiteboardItems;
        }
    }

    public bool OverRightMargin { get; set; }

    public Point InsertionPoint
    {
        get
        {
            return _insertionPoint;
        }
        set
        {
            SetInsertionPoint(value);
        }
    }

    protected virtual void SetInsertionPoint(global::System.Windows.Point value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected void FireActivated()
    {
        Activated?.Invoke();
    }

    protected virtual void SimulateItemClickAndFireActivated(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Point location, int numberOfClicks) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual IWhiteboardItem ActiveItem
    {
        get
        {
            return _activeItem;
        }
        set
        {
            if (_activeItem == value)
            {
                return;
            }

            if (value != null && !WhiteboardItems.Contains(value))
            {
                throw new ArgumentException("Only a child can be set as an ActiveItem");
            }

            if (_activeItem != null)
            {
                UnhookActiveItem(_activeItem);
                if (HandleUserInput)
                {
                    DeactivateActiveItem();
                }
            }

            DeleteItemIfNeeded(_activeItem);
            _activeItem = value;
            if (_activeItem != null)
            {
                HookupActiveItem(_activeItem);
                ClearSavedState();
                ClearSelectedItems();
                Rect itemRect = GetItemRect(_activeItem);
                if (!itemRect.Contains(_insertionPoint))
                {
                    _insertionPoint = GetItemLocation(_activeItem);
                }

                IsCrosshairVisible = false;
                if (HandleUserInput)
                {
                    ActivateActiveItem();
                }

                FireActivated();
            }

            OnPropertyChanged("ActiveItem");
        }
    }

    protected virtual void HookupActiveItem(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void UnhookActiveItem(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void OnPropertyChanged(string propertyName) => base.OnPropertyChanged(propertyName);

    private void ChildWhiteboardPropertyChangedHandler(object sender, global::System.ComponentModel.PropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DeleteItemIfNeeded(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual INotifyCollectionRangeChanged<IWhiteboardItem> SelectedItems
    {
        get
        {
            return _selectedItems;
        }
    }

    protected IWhiteboardItem FirstSelectedItem { get; set; }

    protected bool HasSelectedItems
    {
        get
        {
            return SelectedItems.Any();
        }
    }

    public IEnumerable<IWhiteboardItem> ActiveOrSelectedItems
    {
        get
        {
            if (ActiveItem != null)
            {
                return new List<IWhiteboardItem> { ActiveItem };
            }

            return SelectedItems;
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

            IWhiteboardItemContainer container = ActiveItem as IWhiteboardItemContainer;
            if (container == null)
            {
                return ActiveItem;
            }

            IWhiteboardControl control = ActiveItem as IWhiteboardControl;
            if (control == null)
            {
                return container.ActiveItem ?? ActiveItem;
            }

            return control.ActiveDescendant ?? ActiveItem;
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

            IWhiteboardControl control = ActiveItem as IWhiteboardControl;
            if (control == null)
            {
                return SelectedItems.ToList();
            }

            return control.SelectedDescendants;
        }
    }

    public IAlignmentGrid Grid
    {
        get
        {
            return _alignmentGrid;
        }
    }

    public bool IsFocused
    {
        get
        {
            return _isFocused;
        }
        set
        {
            _isFocused = value;
            OnPropertyChanged("IsFocused");
        }
    }

    public virtual bool IsCrosshairVisible
    {
        get
        {
            return _isCrosshairVisible;
        }
        set
        {
            _isCrosshairVisible = value;
            OnPropertyChanged("IsCrosshairVisible");
        }
    }

    public Point InsertionPointForResize { get; private set; }

    public bool ForbidGrowingMovementRight { get; set; }

    public virtual void HandleGridCellSizeChanged(global::System.Windows.Size oldSize, global::System.Windows.Size newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool IsSelected { private get; set; }

    public virtual Point DefaultInsertionPoint
    {
        get
        {
            return new Point(1.0, 1.0);
        }
    }

    public Size ContentSize
    {
        get
        {
            return new Size(ContentWidth, ContentHeight);
        }
        set
        {
            ContentWidth = value.Width;
            ContentHeight = value.Height;
        }
    }

    public double ContentHeight
    {
        get
        {
            return _contentHeight;
        }
        set
        {
            SetContentHeight(value);
        }
    }

    public double ContentWidth
    {
        get
        {
            return _contentWidth;
        }
        set
        {
            SetContentWidth(value);
        }
    }

    protected virtual void SetContentHeight(double height)
    {
        double contentHeight = Math.Max(MinimumHeight, height);
        if (_contentHeight.IsEqualTo(contentHeight))
        {
            return;
        }

        _contentHeight = contentHeight;
        _alignmentGrid.GridArea = new Rect(new Size(ContentWidth, ContentHeight));
        OnPropertyChanged("ContentHeight");
    }

    protected virtual void SetContentWidth(double width)
    {
        double contentWidth = Math.Max(MinimumWidth, width);
        if (_contentWidth.IsEqualTo(contentWidth))
        {
            return;
        }

        _contentWidth = contentWidth;
        _alignmentGrid.GridArea = new Rect(new Size(ContentWidth, ContentHeight));
        OnPropertyChanged("ContentWidth");
    }

    public Size MinimumSize
    {
        get
        {
            return new Size(MinimumWidth, MinimumHeight);
        }
        set
        {
            MinimumWidth = value.Width;
            MinimumHeight = value.Height;
        }
    }

    public double MinimumWidth
    {
        get
        {
            return _minimumWidth;
        }
        set
        {
            double minimumWidth = Math.Max(value > MaximumSize.Width ? MaximumSize.Width : value, StrictMinimumSize.Width);
            if (MinimumWidth.IsEqualTo(minimumWidth))
            {
                return;
            }

            _minimumWidth = minimumWidth;
            OnPropertyChanged("MinimumWidth");
        }
    }

    public double MinimumHeight
    {
        get
        {
            return _minimumHeight;
        }
        set
        {
            double minimumHeight = Math.Max(value, StrictMinimumSize.Height);
            if (MinimumHeight.IsEqualTo(minimumHeight))
            {
                return;
            }

            _minimumHeight = minimumHeight;
            OnPropertyChanged("MinimumHeight");
        }
    }

    protected virtual Size StrictMinimumSize
    {
        get
        {
            return new Size(100.0, 100.0);
        }
    }

    public virtual Size MaximumSize
    {
        get
        {
            return new Size(MaximumWidth, MaximumHeight);
        }
        set
        {
            MaximumWidth = value.Width;
            MaximumHeight = value.Height;
        }
    }

    public virtual double MaximumHeight
    {
        get
        {
            return _maximumHeight;
        }
        set
        {
            if (_maximumHeight.IsEqualTo(value))
            {
                return;
            }

            _maximumHeight = value;
            OnPropertyChanged("MaximumHeight");
        }
    }

    public virtual double MaximumWidth
    {
        get
        {
            return _maximumWidth;
        }
        set
        {
            if (_maximumWidth.IsEqualTo(value))
            {
                return;
            }

            _maximumWidth = value;
            OnPropertyChanged("MaximumWidth");
        }
    }

    public virtual void UpdateSizeLimits(Size maximumSize)
    {
        MaximumSize = maximumSize;
        MinimumSize = FindMinimumSize();
    }

    protected virtual global::System.Windows.Size FindMinimumSize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public DragMode CurrentDragMode { get; set; }

    public event global::System.Action<global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem>> CopyWhiteboardItems;

    protected readonly global::Quire.Controls.Whiteboard.IWhiteboardModel _whiteboardModel;

    private static global::Quire.Controls.Core.Whiteboard.IWhiteboardItemManager _whiteboardItemMgr;

    protected global::System.Windows.Point _insertionPoint;

    private global::Quire.Controls.Core.IWhiteboardItem _activeItem;

    private global::Quire.Wpf.IObservableRangeCollection<global::Quire.Controls.Core.IWhiteboardItem> _selectedItems;

    protected global::Quire.Controls.Core.IAlignmentGrid _alignmentGrid;

    private bool _isFocused;

    private bool _isCrosshairVisible;

    private global::System.Nullable<global::System.Windows.Point> _savedInsertionPoint;

    private global::System.Collections.Generic.List<global::Quire.Controls.Core.IWhiteboardItem> _savedSelectionList;

    private global::Quire.Controls.Core.IWhiteboardItem _savedActiveItem;

    private global::System.Nullable<bool> _savedIsFocused;

    private global::System.Windows.Size _savedMaxSize;

    private global::System.Windows.Size _savedMinSize;

    private double _contentHeight;

    private double _contentWidth;

    protected WhiteboardViewModel(global::Quire.Controls.Whiteboard.IWhiteboardModel model, bool handleUserInput, global::System.Nullable<global::System.Windows.Size> gridCellSize = default) { }

    protected WhiteboardViewModel(global::Quire.Controls.Whiteboard.WhiteboardViewModel original) { }

    protected internal WhiteboardViewModel(global::Quire.Controls.Whiteboard.IWhiteboardModel model, global::Quire.Controls.Core.IAlignmentGrid grid) { }

    protected void Initialize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleItemsListChanging(global::System.Collections.Specialized.NotifyCollectionChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleItemsListChanged(global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public Point GridPosition
    {
        get
        {
            return Grid.ConvertDipsToGridCoordinates(InsertionPoint);
        }
        set
        {
            InsertionPoint = Grid.ConvertGridCoordinatesToDips(value);
        }
    }

    public virtual void AddItemAndActivate(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void RemoveWhiteboardItem(global::Quire.Controls.Core.IWhiteboardItem item)
    {
        GetRemoveItemsCommand("Remove Whiteboard Item", GetItemAtLocation(item)).Execute();
    }

    private IDictionary<IWhiteboardItem, Point> GetItemAtLocation(IWhiteboardItem item)
    {
        return new Dictionary<IWhiteboardItem, Point> { { item, GetItemLocation(item) } };
    }

    public virtual void AddItemAtLocation(IWhiteboardItem item, Point location)
    {
        _whiteboardModel.AddItemAtLocation(item, location);
    }

    public void AddItems(global::System.Collections.Generic.Dictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> addItemsAtLocation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual Point GetItemLocation(IWhiteboardItem item)
    {
        return WhiteboardItemMgr.GetItemLocation(item);
    }

    public Size GetItemDesiredSize(IWhiteboardItem item)
    {
        return WhiteboardItemMgr.GetDesiredSize(item);
    }

    public void SetItemSize(IWhiteboardItem item, Size size, DragMode dragMode = default)
    {
        WhiteboardItemMgr.SetSize(item, size, dragMode);
        FireChanged();
    }

    public bool GetItemIsInPasting(IWhiteboardItem item)
    {
        return WhiteboardItemMgr.GetItemIsInPasting(item);
    }

    public virtual void SetItemLocation(IWhiteboardItem item, Point location)
    {
        _whiteboardModel.SetItemLocation(item, location);
        FireChanged();
    }

    public virtual void SetItemLocationToNearestGridIntersection(IWhiteboardItem item, Point location)
    {
        Point nearestIntersection = _alignmentGrid.GetNearestIntersectionPoint(location);
        SetItemLocation(item, nearestIntersection);
    }

    public void SetItemGridPosition(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Point location) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual Point GetItemGridPosition(IWhiteboardItem item)
    {
        return Grid.ConvertDipsToGridCoordinates(GetItemLocation(item));
    }

    public virtual bool GetItemIgnoreForHorizontalMovementAndWidth(IWhiteboardItem item)
    {
        return WhiteboardItemMgr.GetIgnoreForHorizontalMovementAndWidth(item);
    }

    protected virtual void SelectedItemsAddNew(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void ToggleItemSelection(IWhiteboardItem item, bool includeActiveItem)
    {
        if (item == ActiveItem)
        {
            SelectItem(item);
            return;
        }

        if (SelectedItems.Contains(item))
        {
            UnSelectItem(item);
            if (SelectedItems.IsEmpty())
            {
                ActivateItem(item);
            }
        }
        else if (includeActiveItem && ActiveItem != null)
        {
            SelectItems(new IWhiteboardItem[] { ActiveItem, item });
        }
        else
        {
            SelectItem(item);
        }
    }

    public void SelectContiguous(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SelectRegionsInBounds(global::System.Windows.Rect rect) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SelectAllItems()
    {
        SelectItems(WhiteboardItems);
    }

    public void HandleSelectAll()
    {
        SelectAllItems();
    }

    public void TrySelectRegionsInBounds(global::System.Windows.Rect rect, global::Quire.Controls.Whiteboard.WhiteboardControl wbc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual bool BoundsOutsideContentSize(Rect rect)
    {
        if (rect.Bottom > ContentHeight || rect.Top < 0.0 || rect.Left < 0.0 || rect.Right > ContentWidth)
        {
            return true;
        }

        return false;
    }

    public bool CanDelete()
    {
        return SelectedItems.Any() || CanDeleteVerticalSpaceBelow;
    }

    public bool CanBackspace()
    {
        return SelectedItems.Any() || CanDeleteVerticalSpaceAbove;
    }

    public void HandleBackspace()
    {
        if (SelectedItems.Any())
        {
            DeleteSelectedItems();
            return;
        }

        DeleteVerticalSpaceAbove(1);
    }

    public void HandleDelete()
    {
        if (SelectedItems.Any())
        {
            DeleteSelectedItems();
            return;
        }

        DeleteVerticalSpaceBelow(1);
    }

    public bool CanCopySelection()
    {
        return SelectedItems.Any();
    }

    public virtual bool CanAddItems
    {
        get
        {
            if (ActiveItem == null)
            {
                return SelectedItems.Count() == 0;
            }

            if (ActiveDescendant is IWhiteboardItemContainer whiteboardItemContainer)
            {
                return whiteboardItemContainer.CanAddItems;
            }

            return false;
        }
    }

    public bool CanCutSelection()
    {
        return CanCopySelection();
    }

    public bool CanPasteSelection
    {
        get
        {
            return _activeItem == null;
        }
    }

    public virtual bool CopySelection() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void CutSelection()
    {
        if (CopySelection())
        {
            DeleteSelectedItems();
        }
    }

    public virtual void Paste(global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> pasteItemsAtLocation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual void SaveState()
    {
        _savedInsertionPoint = null;
        if (IsCrosshairVisible)
        {
            _savedInsertionPoint = InsertionPoint;
        }

        if (SelectedItems.Any())
        {
            _savedSelectionList = SelectedItems.ToList();
        }

        if (ActiveItem != null)
        {
            _savedActiveItem = ActiveItem;
        }
    }

    public virtual void ClearState()
    {
        if (HandleUserInput)
        {
            ClearSelectedItems();
        }

        ActiveItem = null;
        IsCrosshairVisible = false;
    }

    public virtual void ClearSavedState()
    {
        _savedInsertionPoint = null;
        _savedSelectionList.Clear();
        _savedActiveItem = null;
    }

    private double _maximumHeight;

    public virtual void RestoreSavedState() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SaveResizeInformation()
    {
        _savedMaxSize = MaximumSize;
        _savedMinSize = MinimumSize;
        InsertionPointForResize = InsertionPoint;
    }

    public void RestoreResizeInformation()
    {
        MaximumSize = _savedMaxSize;
        MinimumSize = _savedMinSize;
    }

    public virtual global::System.Nullable<global::System.Windows.Vector> NavigateInto(global::Quire.Direction direction, global::System.Windows.Point point) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::System.Windows.Point GetNavigationStartLocation(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual void OnRequestMoveAndResize(global::Quire.Controls.Core.Whiteboard.IWhiteboardControl whiteboardControl, global::Quire.Controls.Core.ResizeEventMouseArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SelectItems(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemsToSelect) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void UnSelectItems(IEnumerable<IWhiteboardItem> itemsToDeselect)
    {
        if (itemsToDeselect.Any())
        {
            _selectedItems.RemoveRange(itemsToDeselect);
        }
    }

    public void SelectItem(IWhiteboardItem item)
    {
        SelectItems(new IWhiteboardItem[] { item });
    }

    public void UnSelectItem(IWhiteboardItem item)
    {
        UnSelectItems(new IWhiteboardItem[] { item });
    }

    private void UpdateSizeLimit(global::Quire.Controls.Core.Whiteboard.IWhiteboardControl whiteBoard) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::System.Windows.Size FindSizeLimit(global::Quire.Controls.Core.IWhiteboardItem whiteboardItem) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::System.Windows.Size FindSizeLimit(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemsToIgnore) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> GetOverlappingItems(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual bool FindOncePrivate(string text, bool startFromCaret, global::System.Windows.Documents.LogicalDirection direction, global::Quire.Controls.Core.FindOptions options, global::System.Collections.Generic.List<global::Quire.Pair<int, int>> subscriptPositions) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void FireRequestMaximumSize()
    {
        RequestMaximumSize();
    }

    public virtual void FireChildCausedResize(Size oldSize, Size newSize)
    {
        ChildCausedResize(oldSize, newSize);
    }

    public virtual global::Quire.Controls.Whiteboard.WhiteboardViewModel.RemoveItemsCommand GetRemoveItemsCommand(string commandName, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> itemsAtLocations) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void ClearSelectedItems()
    {
        if (_selectedItems.Any())
        {
            _selectedItems.Clear();
        }
    }

    public virtual global::System.Windows.Size ArrangeOverride(global::System.Windows.Size size) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool HandleUserInput { get; protected set; }

    public void ActivateActiveItem()
    {
        // Mathcad IL branches past a guard return (brtrue; ret) and reads ActiveItem again for each call.
        if (ActiveItem == null)
        {
            return;
        }

        ActivateItem(ActiveItem);
        WhiteboardItemMgr.ActivateItem(ActiveItem);
    }

    protected virtual void ActivateItem(IWhiteboardItem item)
    {
        item.Activate(true);
    }

    public virtual void Activate(bool isActive)
    {
        if (isActive)
        {
            RestoreSavedState();
            ClearSavedState();
            if (ActiveItem == null && SelectedItems.IsEmpty())
            {
                IsCrosshairVisible = true;
            }
            else
            {
                if (ActiveItem != null)
                {
                    ActivateActiveItem();
                }

                IsCrosshairVisible = false;
            }

            FireActivated();
        }
        else
        {
            SaveState();
            ClearState();
        }
    }

    public virtual global::Quire.Controls.Whiteboard.WhiteboardViewModel.PasteItemsCommand GetPasteItemsCommand(string commandName, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> pasteItemsAtLocation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual bool IsTabNavigatable { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public virtual void TabInto(global::System.Windows.Input.FocusNavigationDirection dir) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void HandleTabOutOfText(CustomRoutedEventArgs<FocusNavigationDirection> eventArgs)
    {
        // Mathcad IL returns early on a failed CanTraverse (brtrue; ret) instead of nesting the body.
        if (!CanTraverse(eventArgs.Parameter))
        {
            return;
        }

        eventArgs.Handled = true;
        HandleTraverse(eventArgs.Parameter);
    }

    public virtual global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> SortItemsByLocation(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items, bool descending) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public event global::System.EventHandler Changed;

    public virtual bool HandleMouseDown(global::Quire.Controls.Core.Whiteboard.IWhiteboardControl sender, global::System.Windows.Point position, global::System.Windows.Input.MouseButton button) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual void PreviewMouseLeftButtonDownHandler(object sender, global::System.Windows.Input.MouseButtonEventArgs eventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual void PreviewMouseRightButtonDownHandler(object sender, global::System.Windows.Input.MouseButtonEventArgs eventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual void PreviewMouseRightButtonUpHandler(object sender, global::System.Windows.RoutedEventArgs eventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual void PreviewMouseLeftButtonUpHandler(object sender, global::System.Windows.RoutedEventArgs eventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual bool HandleMouseUp(global::Quire.Controls.Core.Whiteboard.IWhiteboardControl sender, global::System.Windows.Point position, global::System.Windows.Input.MouseButton button) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> RecursivelyEnumerateDescendantItemssBypassWhiteboard() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public Point GetDescendantInsertionPoint()
    {
        if (SelectedItems.Any())
        {
            return default;
        }

        if (ActiveItem is IWhiteboardControl whiteboardControl)
        {
            Point descendantInsertionPoint = whiteboardControl.GetDescendantInsertionPoint();
            Point itemLocation = WhiteboardItemMgr.GetItemLocation(ActiveItem);
            return new Point(itemLocation.X + descendantInsertionPoint.X, itemLocation.Y + descendantInsertionPoint.Y);
        }

        if (ActiveItem != null)
        {
            return WhiteboardItemMgr.GetItemLocation(ActiveItem);
        }

        return InsertionPoint;
    }

    public void HandleCaretLeavesTextArea(global::Quire.Controls.Core.CommonRoutedEvents.CaretLeavesTextRegionEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> RecursivelyEnumerateDescendantsIgnoringTypes(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items, params global::System.Type[] typesToIgnore) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void DeactivateActiveItem()
    {
        if (ActiveItem == null)
        {
            return;
        }

        ActiveItem.Activate(false);
        WhiteboardItemMgr.DeactivateItem(ActiveItem);
    }

    protected virtual void HandleChildItemGotFocus(IWhiteboardItem item)
    {
        if (item == null || WhiteboardItems.Contains(item))
        {
            ActiveItem = item;
        }
    }

    protected virtual void HandleChildRequestRemove(IWhiteboardItem itemToRemove, Direction direction)
    {
        RemoveWhiteboardItem(itemToRemove);
    }

    protected virtual void HandleCarriageReturn() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected global::System.Windows.Point GetInsertionPointAfterCarriageReturn() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected double GetRightMostItemEdge(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected double GetLowestItemEdge(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected double GetItemBottom(IWhiteboardItem item)
    {
        Rect rect = GetItemRect(item);
        return rect.Bottom;
    }

    public double GetItemTop(IWhiteboardItem item)
    {
        Rect rect = GetItemRect(item);
        return rect.Top;
    }

    protected double GetItemRightEdge(IWhiteboardItem item)
    {
        return GetItemLocation(item).X + WhiteboardItemMgr.GetDesiredSize(item).Width;
    }

    protected virtual void DirectionalMoveHandler(Direction direction)
    {
        DirectionalMoveHandler(direction, allowGrowthDownOrRight: true);
    }

    protected void DirectionalMoveHandler(global::Quire.Direction direction, bool allowGrowthDownOrRight) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual global::System.Windows.Point GetNextIntersectionForMove(global::System.Windows.Point currentLocation, global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual bool IsValidInsertionPoint(Point pt)
    {
        return true;
    }

    protected virtual void HandleTraverse(global::System.Windows.Input.FocusNavigationDirection direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DeleteActiveItem()
    {
        if (ActiveItem != null)
        {
            GetRemoveItemsCommand("Remove Active Item", GetItemAtLocation(ActiveItem)).Execute();
        }
    }

    protected void DeleteSelectedItems() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DeleteActiveOrSelectedItems()
    {
        if (ActiveItem != null)
        {
            DeleteActiveItem();
            return;
        }

        if (!EnumerableExtensions.IsNullOrEmpty(SelectedItems))
        {
            DeleteSelectedItems();
        }
    }

    protected global::Quire.Controls.Core.IWhiteboardItem PredictFocus(global::System.Windows.Input.FocusNavigationDirection direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> GetItemsAccordingTabNavigateOrder(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemsAccordingTabDirection, global::System.Windows.Input.FocusNavigationDirection direction, global::Quire.Controls.Core.IWhiteboardItem startItem) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual bool CanTraverse(FocusNavigationDirection direction)
    {
        if (PredictFocus(direction) != null)
        {
            return true;
        }

        return false;
    }

    protected virtual bool CanMove(Direction direction)
    {
        if (!HandleUserInput)
        {
            return false;
        }

        Point nearestIntersectionInDirection = _alignmentGrid.GetNearestIntersectionInDirection(GetNavigationStartLocation(direction), direction);
        if (SelectedItems.Count() <= 0)
        {
            if (nearestIntersectionInDirection.X.IsBetween(0.0, ContentWidth))
            {
                return nearestIntersectionInDirection.Y.IsBetween(0.0, ContentHeight);
            }

            return false;
        }

        return true;
    }

    private bool CanDeleteActiveItem()
    {
        return ActiveItem != null;
    }

    private bool CanDeleteSelectedItems()
    {
        return !EnumerableExtensions.IsNullOrEmpty(SelectedItems);
    }

    private bool CanDeleteActiveOrSelectedItems()
    {
        return !EnumerableExtensions.IsNullOrEmpty(ActiveOrSelectedItems);
    }

    public virtual bool CanSeparateOverlappedItems(global::Quire.Dimension dimension) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SeparateOverlappedItems(global::Quire.Dimension dimension) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual global::Quire.Controls.Whiteboard.WhiteboardViewModel.SeparateOverlappedItemsCommand CreateSeparateOverlappedItemsCommand(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemsToSeparate, global::Quire.Dimension dimension) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected void FireChanged()
    {
        if (!HandleUserInput)
        {
            return;
        }

        if (Changed != null)
        {
            Changed(this, EventArgs.Empty);
        }
    }

    protected override void OnDispose() => base.OnDispose();

    protected virtual global::Quire.Controls.Core.IWhiteboardItem GetChildItemFromLocation(global::System.Windows.Point point) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void SelectedItemsChangedHandler(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void HandleItemAdded(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void HandleItemRemoved(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ChildTriggeredResizeHandler(global::Quire.Controls.Core.Whiteboard.IWhiteboardControl whiteboard, global::System.Windows.Size oldSize, global::System.Windows.Size newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void HandleItemMoved(IWhiteboardItem item)
    {
    }

    protected virtual void HandleChildSizeChanged(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Size oldSize, global::System.Windows.Size newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected void SetEqResizable(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual bool ItemsOverlapped(IEnumerable<IWhiteboardItem> items)
    {
        return WhiteboardItemMgr.ItemsOverlapped(items);
    }

    public virtual Rect GetItemRect(IWhiteboardItem item)
    {
        return WhiteboardItemMgr.GetItemRect(item);
    }

    protected virtual void SetupCommandBindings() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual bool CanInsertVerticalSpace
    {
        get
        {
            return ActiveOrSelectedItems.Count() == 0;
        }
    }

    public virtual bool CanDeleteVerticalSpaceAbove { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public virtual bool CanDeleteVerticalSpaceBelow { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public virtual void InsertVerticalSpace(int numberOfLines, bool pushInsertionPoint, bool ignoreItemsOnCurrentLine) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual void DeleteVerticalSpaceAbove(int numberOfLines) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual void DeleteVerticalSpaceBelow(int numberOfLines) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public double LimitOffsetBasedOnItemsToMove(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemsToMove, double offsetY, global::Quire.Direction direction, global::System.Windows.Point insertionPointLocation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual bool CanDeleteVerticalSpace(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual global::System.Windows.Size ResizeAndNotify(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> list, global::System.Windows.Size oldSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public virtual bool IsValidLocation(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> newLocations, bool allowGrowthDownOrRight) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    CommandBindingCollection IWhiteboardViewModel.CommandBindings
    {
        get
        {
            return base.CommandBindings;
        }
    }

    private double _maximumWidth;

    private double _minimumHeight;

    private double _minimumWidth;

    private readonly global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::Quire.Controls.Core.Whiteboard.WhiteboardItemSizeChangedEventHandler> _itemSizeChangedHandlerMap;

    private bool _enteredRestoreSavedState;

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+PropertyNames, Ptc.Controls.Whiteboard")]
    public static partial class PropertyNames
    {
        [global::Quire.Mirror.MirrorStub]
        public const string IsFocused = "IsFocused";
        [global::Quire.Mirror.MirrorStub]
        public const string InsertionPoint = "InsertionPoint";
        [global::Quire.Mirror.MirrorStub]
        public const string IsCrosshairVisible = "IsCrosshairVisible";
        [global::Quire.Mirror.MirrorStub]
        public const string ContentWidth = "ContentWidth";
        [global::Quire.Mirror.MirrorStub]
        public const string ContentHeight = "ContentHeight";
        [global::Quire.Mirror.MirrorStub]
        public const string MinimumWidth = "MinimumWidth";
        [global::Quire.Mirror.MirrorStub]
        public const string MinimumHeight = "MinimumHeight";
        [global::Quire.Mirror.MirrorStub]
        public const string MaximumWidth = "MaximumWidth";
        [global::Quire.Mirror.MirrorStub]
        public const string MaximumHeight = "MaximumHeight";
        [global::Quire.Mirror.MirrorStub]
        public const string ActiveItem = "ActiveItem";
        [global::Quire.Mirror.MirrorStub]
        public const string SelectedItems = "SelectedItems";
        [global::Quire.Mirror.MirrorStub]
        public const string ActiveDescendant = "ActiveDescendant";
        [global::Quire.Mirror.MirrorStub]
        public const string SelectedDescendants = "SelectedDescendants";
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+WhiteboardResizingCommand, Ptc.Controls.Whiteboard")]
    public abstract partial class WhiteboardResizingCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        [global::Quire.Mirror.MirrorStub]
        protected global::System.Windows.Size _oldSize;
        [global::Quire.Mirror.MirrorStub]
        protected global::Quire.Controls.Core.IResizeData _oldData;
        [global::Quire.Mirror.MirrorStub]
        protected WhiteboardResizingCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, string commandName) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        protected abstract bool DoWithResize();
        [global::Quire.Mirror.MirrorStub]
        protected void BackupTheSize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override bool Do() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public void UndoResize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+AddItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class AddItemsCommand : global::Quire.Controls.Whiteboard.WhiteboardViewModel.WhiteboardResizingCommand
    {
        [global::Quire.Mirror.MirrorStub]
        private readonly global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> _itemsAtLocations;
        [global::Quire.Mirror.MirrorStub]
        private global::System.Windows.Point _insertionPoint;
        [global::Quire.Mirror.MirrorStub]
        private readonly bool _shouldActivate;
        [global::Quire.Mirror.MirrorStub]
        public AddItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, string commandName, global::Quire.Controls.Core.IWhiteboardItem item) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public AddItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, string commandName, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> itemsAtLocations) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        protected override bool DoWithResize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+RemoveItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class RemoveItemsCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        [global::Quire.Mirror.MirrorStub]
        protected readonly global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> _itemsAtLocations;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Core.IWhiteboardItem _activeItemPriorToRemoval;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::System.Windows.Point _insertionPoint;
        [global::Quire.Mirror.MirrorStub]
        public bool SetInsertionPointToFirstItemsLocation { private get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
        [global::Quire.Mirror.MirrorStub]
        public RemoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, string commandName, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> itemsAtLocations) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public override bool Do() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+RemoveSelectedItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class RemoveSelectedItemsCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        [global::Quire.Mirror.MirrorStub]
        protected readonly global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> _itemsAtLocations;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Undo.CompoundCommand _nestedCommand;
        [global::Quire.Mirror.MirrorStub]
        public RemoveSelectedItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, string commandName) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public override bool Do() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+PasteItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class PasteItemsCommand : global::Quire.Controls.Whiteboard.WhiteboardViewModel.WhiteboardResizingCommand
    {
        [global::Quire.Mirror.MirrorStub]
        protected readonly global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> _itemsAtLocation;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Whiteboard.WhiteboardViewModel.AddItemsCommand _addItemsCommand;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Core.IWhiteboardItem _itemToActivate;
        [global::Quire.Mirror.MirrorStub]
        public PasteItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, string commandName, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> pasteItemsAtLocation) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        protected override bool DoWithResize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+ResizeItemCommand, Ptc.Controls.Whiteboard")]
    public partial class ResizeItemCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        [global::Quire.Mirror.MirrorStub]
        private readonly global::System.Windows.Size _oldSize;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::System.Windows.Size _newSize;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Core.IWhiteboardItem _item;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Core.DragMode _dragMode;
        [global::Quire.Mirror.MirrorStub]
        private bool _isActiveItem;
        [global::Quire.Mirror.MirrorStub]
        private readonly bool _widthIsAuto;
        [global::Quire.Mirror.MirrorStub]
        private readonly bool _heightIsAuto;
        [global::Quire.Mirror.MirrorStub]
        private global::Quire.Controls.Core.IResizeData _oldData;
        [global::Quire.Mirror.MirrorStub]
        public ResizeItemCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Size oldSize, global::System.Windows.Size newSize, bool widthIsAuto, bool heightIsAuto, global::Quire.Controls.Core.DragMode dragMode) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public override bool Do() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        private void DoRedo(bool doAction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+ResizeItemCommandWithSideEffects, Ptc.Controls.Whiteboard")]
    public partial class ResizeItemCommandWithSideEffects : global::Quire.Controls.Whiteboard.WhiteboardViewModel.ResizeItemCommand, global::Quire.Undo.IUndoableCommandWithSideEffects, global::Quire.Undo.IUndoableCommand
    {
        [global::Quire.Mirror.MirrorStub]
        public ResizeItemCommandWithSideEffects(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Size oldSize, global::System.Windows.Size newSize, bool widthIsAuto, bool heightIsAuto) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(global::Quire.Controls.Core.IWhiteboardItem), default(global::System.Windows.Size), default(global::System.Windows.Size), default(bool), default(bool), default(global::Quire.Controls.Core.DragMode)) { }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+MoveItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class MoveItemsCommand : global::Quire.Controls.Whiteboard.WhiteboardViewModel.WhiteboardResizingCommand
    {
        [global::Quire.Mirror.MirrorStub]
        private readonly global::System.Collections.Generic.List<global::Quire.Controls.Core.IWhiteboardItem> _list;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::System.Collections.Generic.Dictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> _initialLocations;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::System.Collections.Generic.Dictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> _finalLocations;
        [global::Quire.Mirror.MirrorStub]
        private global::Quire.Controls.Core.IWhiteboardItem[] _selectedItems;
        [global::Quire.Mirror.MirrorStub]
        private global::Quire.Controls.Core.IWhiteboardItem _activeItem;
        [global::Quire.Mirror.MirrorStub]
        private readonly bool _allowGrowthDownOrRight;
        [global::Quire.Mirror.MirrorStub]
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::Quire.Controls.Core.IWhiteboardItem item, bool allowGrowthDownOrRight, global::System.Windows.Point newLocation) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Windows.Vector offset, bool allowGrowthDownOrRight, params global::Quire.Controls.Core.IWhiteboardItem[] elements) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Windows.Vector offset, bool allowGrowthDownOrRight, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> elements) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Vector> elements, bool allowGrowthDownOrRight) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Point newLocation) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Windows.Vector offset, params global::Quire.Controls.Core.IWhiteboardItem[] elements) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Vector> elements) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        protected override bool DoWithResize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        private bool CheckPotentialMove(global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> newLocations) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        private bool CheckMovementWithGrowth(global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> newLocations) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        private bool CheckMovementWithoutGrowth(global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> newLocations) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        private bool MoveItems() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+MoveItemsCommandWithSideEffects, Ptc.Controls.Whiteboard")]
    public partial class MoveItemsCommandWithSideEffects : global::Quire.Controls.Whiteboard.WhiteboardViewModel.MoveItemsCommand, global::Quire.Undo.IUndoableCommandWithSideEffects, global::Quire.Undo.IUndoableCommand
    {
        [global::Quire.Mirror.MirrorStub]
        public MoveItemsCommandWithSideEffects(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Windows.Vector offset, bool allowGrowthDownOrRight, params global::Quire.Controls.Core.IWhiteboardItem[] elements) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Vector>)) { }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+PushItemsDownCommand, Ptc.Controls.Whiteboard")]
    public partial class PushItemsDownCommand : global::Quire.Controls.Whiteboard.WhiteboardViewModel.WhiteboardResizingCommand, global::Quire.Undo.IUndoableCommandWithSideEffects, global::Quire.Undo.IUndoableCommand
    {
        [global::Quire.Mirror.MirrorStub]
        private int _numberOfLines;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Whiteboard.WhiteboardViewModel.MoveItemsCommand _moveCommand;
        [global::Quire.Mirror.MirrorStub]
        private global::System.Windows.Vector _offset;
        [global::Quire.Mirror.MirrorStub]
        public PushItemsDownCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, int numberOfLines, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemsToMove) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        protected override bool DoWithResize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+InsertSpaceCommand, Ptc.Controls.Whiteboard")]
    public partial class InsertSpaceCommand : global::Quire.Controls.Whiteboard.WhiteboardViewModel.WhiteboardResizingCommand
    {
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Whiteboard.WhiteboardViewModel.MoveItemsCommand _moveCommand;
        [global::Quire.Mirror.MirrorStub]
        private global::System.Windows.Vector _offset;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::System.Windows.Point _insertionPoint;
        [global::Quire.Mirror.MirrorStub]
        private readonly bool _pushInsertionPoint;
        [global::Quire.Mirror.MirrorStub]
        public InsertSpaceCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, int numberOfLines, bool pushInsertionPoint, params global::Quire.Controls.Core.IWhiteboardItem[] checkVerticalOnlyItems) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public InsertSpaceCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, int numberOfLines, bool pushInsertionPoint, bool ignoreItemsOnCurrentLine, params global::Quire.Controls.Core.IWhiteboardItem[] checkVerticalOnlyItems) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        protected override bool DoWithResize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+DeleteSpaceCommand, Ptc.Controls.Whiteboard")]
    public partial class DeleteSpaceCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Direction _direction;
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Whiteboard.WhiteboardViewModel.MoveItemsCommand _moveCommand;
        [global::Quire.Mirror.MirrorStub]
        private global::System.Windows.Vector _offset;
        [global::Quire.Mirror.MirrorStub]
        private readonly double _oldHeight;
        [global::Quire.Mirror.MirrorStub]
        private global::System.Windows.Point _insertionPoint;
        [global::Quire.Mirror.MirrorStub]
        public DeleteSpaceCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, int numberOfLines, global::Quire.Direction direction) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public override bool Do() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        private double LimitOffsetBasedOnMinimumHeight() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+SeparateOverlappedItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class SeparateOverlappedItemsCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        [global::Quire.Mirror.MirrorStub]
        private readonly global::Quire.Controls.Whiteboard.WhiteboardViewModel.MoveItemsCommand _moveItemsCommand;
        [global::Quire.Mirror.MirrorStub]
        public SeparateOverlappedItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemsToSeparate, global::Quire.Dimension dimension, bool allowGrowthDownOrRight = false, global::Quire.Controls.Core.Whiteboard.IWhiteboardItemManager itemManager = default) : base(default(global::Quire.Controls.Whiteboard.IWhiteboardViewModel), default(string)) { }
        [global::Quire.Mirror.MirrorStub]
        public override bool Do() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Undo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        [global::Quire.Mirror.MirrorStub]
        public override void Redo() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

}
