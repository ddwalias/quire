#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Whiteboard;

public partial class WhiteboardViewModel
{
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal WhiteboardViewModel(global::Quire.Controls.Whiteboard.IWhiteboardModel model)
        : this(model, true, null)
    {
    }

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

    public event global::System.Action<global::System.Windows.Size, global::System.Windows.Size> ChildCausedResize = delegate { };

    public event global::System.Action RequestMaximumSize = delegate { };

    public event global::System.Action Activated;

    public event global::System.Action<global::Quire.Controls.Core.IWhiteboardItem> RequestActivatePartContainingChild = delegate { };

    public event global::System.Action<global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem>> RequestBringIntoView;

    public WhiteboardItemsCollection WhiteboardItems
    {
        get
        {
            return _whiteboardModel.WhiteboardItems;
        }
    }

    public bool OverRightMargin { get; set; }

    // Layout requires CurrentDragMode before SetInsertionPoint to match Mathcad member ordinal |53_0 for local function Focus
    public DragMode CurrentDragMode { get; set; }

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

    protected virtual void SetInsertionPoint(Point value)
    {
        _insertionPoint = _alignmentGrid.GetNearestIntersectionPoint(value);
        IWhiteboardItem childItemFromLocation = GetChildItemFromLocation(value);
        if (childItemFromLocation != null && WhiteboardItemMgr.GetCanActivate(childItemFromLocation))
        {
            Point itemLocation = GetItemLocation(childItemFromLocation);
            Point location = new Point(value.X - itemLocation.X, value.Y - (itemLocation.Y + childItemFromLocation.VerticalAlignmentOffset));
            SimulateItemClickAndFireActivated(childItemFromLocation, location, 1);
            return;
        }

        ClearSavedState();
        IsCrosshairVisible = true;
        if (ActiveItem != null)
        {
            WhiteboardItemMgr.SuppressBringIntoViewIfNeededAndExecute(ActiveItem, Focus, ActiveItem != null);
        }
        else
        {
            Focus();
        }

        ClearSelectedItems();
        ActiveItem = null;
        OnPropertyChanged("InsertionPoint");
        FireActivated();

        void Focus()
        {
            IsFocused = true;
        }
    }

    protected void FireActivated()
    {
        Activated?.Invoke();
    }

    protected virtual void SimulateItemClickAndFireActivated(IWhiteboardItem item, Point location, int numberOfClicks)
    {
        item.SimulateMouseClick(location, numberOfClicks);
        FireActivated();
    }

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

    protected virtual void HookupActiveItem(IWhiteboardItem item)
    {
        if (item is IWhiteboardItemContainer)
        {
            item.PropertyChanged += ChildWhiteboardPropertyChangedHandler;
        }
    }

    protected virtual void UnhookActiveItem(IWhiteboardItem item)
    {
        if (item is IWhiteboardItemContainer)
        {
            item.PropertyChanged -= ChildWhiteboardPropertyChangedHandler;
        }
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == "ActiveItem")
        {
            OnPropertyChanged("ActiveDescendant");
            OnPropertyChanged("SelectedDescendants");
        }
        if (propertyName == "SelectedItems")
        {
            OnPropertyChanged("SelectedDescendants");
        }
    }

    private void ChildWhiteboardPropertyChangedHandler(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is IWhiteboardControl)
        {
            if (e.PropertyName == "ActiveDescendant" || e.PropertyName == "SelectedDescendants")
            {
                OnPropertyChanged(e.PropertyName);
            }
        }
        else if (e.PropertyName == "ActiveItem")
        {
            OnPropertyChanged("ActiveDescendant");
        }
    }

    private void DeleteItemIfNeeded(global::Quire.Controls.Core.IWhiteboardItem item)
    {
        if (!HandleUserInput)
        {
            return;
        }
        if (item == null || !WhiteboardItems.Contains(item))
        {
            return;
        }
        if (!WhiteboardItemMgr.ShouldItemBeDeletedIfEmpty(item) || SelectedItems.Contains(item))
        {
            return;
        }
        if (item.IsEmpty)
        {
            RemoveItemsCommand removeItemsCommand = GetRemoveItemsCommand("Remove Empty Item", GetItemAtLocation(ActiveItem));
            removeItemsCommand.SetInsertionPointToFirstItemsLocation = false;
            removeItemsCommand.Execute();
        }
    }

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

    protected readonly global::Quire.Controls.Whiteboard.IWhiteboardModel _whiteboardModel;

    private static global::Quire.Controls.Core.Whiteboard.IWhiteboardItemManager _whiteboardItemMgr;

    protected global::System.Windows.Point _insertionPoint;

    private global::Quire.Controls.Core.IWhiteboardItem _activeItem;

    private global::Quire.Wpf.IObservableRangeCollection<global::Quire.Controls.Core.IWhiteboardItem> _selectedItems;

    protected global::Quire.Controls.Core.IAlignmentGrid _alignmentGrid;

    private bool _isFocused;

    private bool _isCrosshairVisible;

    private global::System.Nullable<global::System.Windows.Point> _savedInsertionPoint;

    private global::Quire.Controls.Core.IWhiteboardItem _savedActiveItem;

    private global::System.Nullable<bool> _savedIsFocused;

    public virtual void HandleGridCellSizeChanged(global::System.Windows.Size oldSize, global::System.Windows.Size newSize)
    {
        Grid.GridCellSize = newSize;
        foreach (IWhiteboardItem item in WhiteboardItems.ToList())
        {
            Point itemLocation = GetItemLocation(item);
            Point value = new Point(itemLocation.X / oldSize.Width, itemLocation.Y / oldSize.Height);
            SetItemLocation(item, Grid.ConvertGridCoordinatesToDips(value));
        }

        if (IsCrosshairVisible)
        {
            Point oldInsertionPoint = new Point(InsertionPoint.X / oldSize.Width, InsertionPoint.Y / oldSize.Height);
            WhiteboardItemMgr.ExecuteInBackground(() =>
            {
                InsertionPoint = Grid.ConvertGridCoordinatesToDips(oldInsertionPoint);
            }, asynch: true);
        }
    }

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

    protected virtual global::System.Windows.Size FindMinimumSize()
    {
        double height = StrictMinimumSize.Height;
        double width = StrictMinimumSize.Width;
        if (WhiteboardItems != null)
        {
            foreach (global::Quire.Controls.Core.IWhiteboardItem item in WhiteboardItems)
            {
                if (!WhiteboardItemMgr.GetIgnoreForHorizontalMovementAndWidth(item))
                {
                    width = global::System.Math.Max(width, GetItemLocation(item).X + GetItemDesiredSize(item).Width);
                }
                height = global::System.Math.Max(height, GetItemLocation(item).Y + GetItemDesiredSize(item).Height + item.VerticalAlignmentOffset);
            }
        }
        return new global::System.Windows.Size(width, height);
    }

    private global::System.Collections.Generic.List<global::Quire.Controls.Core.IWhiteboardItem> _savedSelectionList = new global::System.Collections.Generic.List<global::Quire.Controls.Core.IWhiteboardItem>();

    private readonly global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::Quire.Controls.Core.Whiteboard.WhiteboardItemSizeChangedEventHandler> _itemSizeChangedHandlerMap = new global::System.Collections.Generic.Dictionary<global::Quire.Controls.Core.IWhiteboardItem, global::Quire.Controls.Core.Whiteboard.WhiteboardItemSizeChangedEventHandler>();

    public event global::System.Action<global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem>> CopyWhiteboardItems = delegate { };

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

    private global::System.Windows.Size _savedMaxSize;

    private global::System.Windows.Size _savedMinSize;

    private double _contentHeight;

    private double _contentWidth;

    private double _maximumWidth;

    private double _minimumHeight;

    private double _minimumWidth;

    private bool _enteredRestoreSavedState;

    protected WhiteboardViewModel(global::Quire.Controls.Whiteboard.IWhiteboardModel model, bool handleUserInput, global::System.Nullable<global::System.Windows.Size> gridCellSize = default)
    {
        HandleUserInput = handleUserInput;
        _whiteboardModel = model;
        _alignmentGrid = new global::Quire.Controls.Core.AlignmentGrid(gridCellSize);
        Initialize();
    }

    protected WhiteboardViewModel(global::Quire.Controls.Whiteboard.WhiteboardViewModel original)
    {
        _whiteboardModel = (global::Quire.Controls.Whiteboard.IWhiteboardModel)original._whiteboardModel.Clone();
        _alignmentGrid = new global::Quire.Controls.Core.AlignmentGrid(original._alignmentGrid.GridCellSize);
        HandleUserInput = original.HandleUserInput;
        OverRightMargin = original.OverRightMargin;
        _activeItem = null;
        _savedActiveItem = null;
        _isFocused = false;
        _savedIsFocused = false;
        _isCrosshairVisible = false;
        _savedInsertionPoint = original._savedInsertionPoint;
        ForbidGrowingMovementRight = original.ForbidGrowingMovementRight;
        _contentHeight = original._contentHeight;
        _contentWidth = original._contentWidth;
        _maximumHeight = original._maximumHeight;
        _maximumWidth = original._maximumWidth;
        _minimumHeight = original._minimumHeight;
        _minimumWidth = original._minimumWidth;
        Initialize();
    }

    protected internal WhiteboardViewModel(global::Quire.Controls.Whiteboard.IWhiteboardModel model, global::Quire.Controls.Core.IAlignmentGrid grid)
    {
        _whiteboardModel = model;
        _alignmentGrid = grid;
        HandleUserInput = true;
        Initialize();
    }

    protected void Initialize()
    {
        _insertionPoint = Grid.ConvertGridCoordinatesToDips(DefaultInsertionPoint);
        _selectedItems = new ObservableRangeCollection<IWhiteboardItem>();
        _selectedItems.RangeChanged += SelectedItemsChangedHandler;
        _whiteboardModel.WhiteboardItems.RangeChanging += (object sender, NotifyCollectionChangedEventArgs args) =>
        {
            HandleItemsListChanging(args);
        };
        _whiteboardModel.WhiteboardItems.RangeChanged += (object sender, NotifyCollectionChangedEventArgs args) =>
        {
            HandleItemsListChanged(args);
        };
        if (HandleUserInput)
        {
            SetupCommandBindings();
        }
    }

    private void HandleItemsListChanging(NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Move:
                args.OldItems.OfType<IWhiteboardItem>().Each((IWhiteboardItem item) =>
                {
                    WhiteboardItemMgr.SetHandleChangesEnabled(item, false);
                });
                break;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", args.Action);
                break;
            case NotifyCollectionChangedAction.Add:
                break;
        }
    }

    private void HandleItemsListChanged(NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                e.NewItems.Cast<IWhiteboardItem>().Each(HandleItemAdded);
                break;
            case NotifyCollectionChangedAction.Remove:
                e.OldItems.Cast<IWhiteboardItem>().Each(HandleItemRemoved);
                break;
            case NotifyCollectionChangedAction.Move:
                e.OldItems.Cast<IWhiteboardItem>().Each(HandleItemMoved);
                break;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                break;
        }
    }

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

    public virtual void AddItemAndActivate(IWhiteboardItem item)
    {
        Commands.WhiteboardCommand command = new AddItemsCommand(this, "Add Whiteboard Items", item);
        command.Execute();
    }

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

    public void AddItems(Dictionary<IWhiteboardItem, Point> addItemsAtLocation)
    {
        Commands.WhiteboardCommand command = new AddItemsCommand(this, "Add Items", addItemsAtLocation);
        command.Execute();
    }

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

    public void SetItemGridPosition(IWhiteboardItem item, Point location)
    {
        Point point = Grid.ConvertGridCoordinatesToDips(location);
        Commands.WhiteboardCommand command = new MoveItemsCommand(this, item, point);
        command.Execute();
    }

    public virtual Point GetItemGridPosition(IWhiteboardItem item)
    {
        return Grid.ConvertDipsToGridCoordinates(GetItemLocation(item));
    }

    public bool GetItemIgnoreForHorizontalMovementAndWidth(IWhiteboardItem item)
    {
        return WhiteboardItemMgr.GetIgnoreForHorizontalMovementAndWidth(item);
    }

    protected virtual void SelectedItemsAddNew(IEnumerable<IWhiteboardItem> items)
    {
        _selectedItems.AddRange(items);
    }

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

    public void SelectContiguous(IWhiteboardItem item)
    {
        if (item == ActiveItem)
        {
            SelectItem(ActiveItem);
            return;
        }
        int num = WhiteboardItems.IndexOf(item);
        List<IWhiteboardItem> itemsToSelect = new List<IWhiteboardItem>();
        if (ActiveItem == null && SelectedItems.IsEmpty())
        {
            0.UpTo(num, (int i) =>
            {
                itemsToSelect.Add(WhiteboardItems[i]);
            });
        }
        else
        {
            int num2 = WhiteboardItems.IndexOf(FirstSelectedItem ?? ActiveItem);
            if (num2 < num)
            {
                num2.UpTo(num, (int i) =>
                {
                    itemsToSelect.Add(WhiteboardItems[i]);
                });
            }
            else
            {
                num2.DownTo(num, (int i) =>
                {
                    itemsToSelect.Add(WhiteboardItems[i]);
                });
            }
        }
        ClearSelectedItems();
        SelectItems(itemsToSelect);
    }

    public void SelectRegionsInBounds(Rect rect)
    {
        List<IWhiteboardItem> list = new List<IWhiteboardItem>();
        List<IWhiteboardItem> list2 = new List<IWhiteboardItem>();
        foreach (IWhiteboardItem whiteboardItem in _whiteboardModel.WhiteboardItems)
        {
            if (rect.IntersectsWith(GetItemRect(whiteboardItem)))
            {
                list.Add(whiteboardItem);
            }
            else if (SelectedItems.Contains(whiteboardItem))
            {
                list2.Add(whiteboardItem);
            }
        }

        UnSelectItems(list2);
        SelectItems(list);
    }

    public void SelectAllItems()
    {
        SelectItems(WhiteboardItems);
    }

    public void HandleSelectAll()
    {
        SelectAllItems();
    }

    public void TrySelectRegionsInBounds(Rect rect, WhiteboardControl wbc)
    {
        if (BoundsOutsideContentSize(rect))
        {
            wbc.OnDragSelectBeyondBorder(rect);
        }
        else
        {
            SelectRegionsInBounds(rect);
        }
    }

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

    public virtual bool CopySelection()
    {
        CopyWhiteboardItems(SelectedItems);
        return true;
    }

    public void CutSelection()
    {
        if (CopySelection())
        {
            DeleteSelectedItems();
        }
    }

    public virtual void Paste(IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation)
    {
        if (pasteItemsAtLocation != null)
        {
            GetPasteItemsCommand("PasteSelectedItems", pasteItemsAtLocation).Execute();
        }
    }

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

    public virtual void RestoreSavedState()
    {
        bool flag = false;
        try
        {
            if (!_enteredRestoreSavedState)
            {
                _enteredRestoreSavedState = true;
                flag = true;
            }
            else if (_savedInsertionPoint.HasValue || _savedActiveItem != null || _savedSelectionList.IsNotEmpty())
            {
                DebugTools.FailAssertion("Reentrancy to RestoreSavedState for object " + GetType().Name + " while there is a saved state.");
                ClearSavedState();
                return;
            }

            if (_savedInsertionPoint.HasValue)
            {
                InsertionPoint = _savedInsertionPoint.Value;
                IsCrosshairVisible = true;
            }

            if (_savedSelectionList.Any(item => !SelectedItems.Contains(item)))
            {
                ClearSelectedItems();
                SelectItems(_savedSelectionList);
            }

            if (_savedActiveItem != null)
            {
                if (WhiteboardItems.Contains(_savedActiveItem))
                {
                    ActivateItem(_savedActiveItem);
                }
                else
                {
                    InsertionPoint = _savedInsertionPoint ?? DefaultInsertionPoint;
                    _savedIsFocused = true;
                }
            }

            if (_savedIsFocused.HasValue)
            {
                IsFocused = _savedIsFocused.Value;
            }
        }
        finally
        {
            if (flag)
            {
                _enteredRestoreSavedState = false;
            }
        }
    }

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

    public virtual Vector? NavigateInto(Direction direction, Point point)
    {
        Vector? result = null;
        IWhiteboardItem childItemFromLocation = GetChildItemFromLocation(point);
        if (childItemFromLocation == null)
        {
            point = _alignmentGrid.GetNearestIntersectionPoint(point);
            childItemFromLocation = GetChildItemFromLocation(point);
        }
        if (childItemFromLocation == null)
        {
            InsertionPoint = point;
        }
        else
        {
            IWhiteboardControl whiteboardControl = childItemFromLocation as IWhiteboardControl;
            if (whiteboardControl != null)
            {
                Point itemLocation = GetItemLocation(childItemFromLocation);
                Point point2 = new TranslateTransform(-itemLocation.X, -itemLocation.Y).Transform(point);
                result = whiteboardControl.NavigateInto(direction, point2);
            }
            else
            {
                result = childItemFromLocation.NavigateIntoItem(direction);
            }
        }
        return result;
    }

    public Point GetNavigationStartLocation(Direction direction)
    {
        Point val = InsertionPoint;
        if (ActiveItem == null)
        {
            return val;
        }

        if (ActiveItem is IWhiteboardControl whiteboardControl)
        {
            Point itemLocation = GetItemLocation(ActiveItem);
            val = whiteboardControl.GetNavigationStartLocation(direction);
            Size renderSize = WhiteboardItemMgr.GetRenderSize(whiteboardControl);
            switch (direction)
            {
                case Direction.Up:
                    val = new Point(val.X, 0.0);
                    break;
                case Direction.Left:
                    val = new Point(0.0, val.Y);
                    break;
                case Direction.Down:
                    val = new Point(val.X, renderSize.Height);
                    break;
                case Direction.Right:
                    val = new Point(renderSize.Width, val.Y);
                    break;
            }

            val = new TranslateTransform(itemLocation.X, itemLocation.Y).Transform(val);
        }
        else
        {
            Rect itemRect = GetItemRect(ActiveItem);
            val = itemRect.TopLeft;
            switch (direction)
            {
                case Direction.Left:
                    // Mathcad IL negates the offset (neg) rather than subtracting from a 0.0 constant.
                    val.Offset(0.0, -ActiveItem.VerticalAlignmentOffset);
                    break;
                case Direction.Right:
                    val.Offset(itemRect.Width, -ActiveItem.VerticalAlignmentOffset);
                    break;
                case Direction.Down:
                    val.Offset(0.0, itemRect.Height);
                    break;
            }
        }

        return val;
    }

    public virtual void OnRequestMoveAndResize(global::Quire.Controls.Core.Whiteboard.IWhiteboardControl whiteboardControl, global::Quire.Controls.Core.ResizeEventMouseArgs args)
    {
        IWhiteboardItem item = args.SourceElement as IWhiteboardItem;
        if (item != null && WhiteboardItems.Contains(item))
        {
            CurrentDragMode = args.DragMode;
            switch (CurrentDragMode)
            {
                case DragMode.ResizeLeft:
                case DragMode.ResizeRight:
                case DragMode.ResizeBottom:
                case DragMode.ResizeBottomRight:
                    if (whiteboardControl is WhiteboardControl whiteboardControl2)
                    {
                        Point startPosition = args.SourceElement.TranslatePoint(args.StartPoint, whiteboardControl2);
                        bool preserveAspectRatio;
                        FrameworkElement resizableElement = FrameworkElementFacade.GetResizableElement(args.SourceElement, CurrentDragMode, out preserveAspectRatio);
                        WhiteboardItemResizer whiteboardItemResizer = new WhiteboardItemResizer(whiteboardControl2, startPosition, CurrentDragMode, resizableElement, preserveAspectRatio, args.Automation);
                        if (args.SourceElement is IWhiteboardControl whiteBoard)
                        {
                            UpdateSizeLimit(whiteBoard);
                        }

                        whiteboardItemResizer.DragDone += (Size oldSize, Size newSize, bool widthIsAuto, bool heightIsAuto, DragMode dragMode) =>
                        {
                            ResizeItemCommand resizeItemCommand = new ResizeItemCommand(this, item, oldSize, newSize, widthIsAuto, heightIsAuto, dragMode);
                            resizeItemCommand.Execute();
                        };
                        args.Resizer = whiteboardItemResizer;
                    }
                    break;
                case DragMode.Move:
                {
                    bool flag = WhiteboardItemMgr.IsItemSelected(item);
                    if (WhiteboardItemMgr.IsItemActivated(item))
                    {
                        SelectItem(item);
                    }

                    WhiteboardDragger.MouseDown(whiteboardControl);
                    if (flag)
                    {
                        WhiteboardDragger.Default.OnNoDrag += ActivateItem;
                    }
                    break;
                }
            }

            args.Handled = true;

            void ActivateItem()
            {
                this.ActivateItem(item);
                WhiteboardDragger.Default.OnNoDrag -= ActivateItem;
            }
        }
    }

    public void SelectItems(IEnumerable<IWhiteboardItem> itemsToSelect)
    {
        if (SelectedItems.Any())
        {
            itemsToSelect = itemsToSelect.Where(item => !SelectedItems.Contains(item)).ToList();
        }

        if (itemsToSelect.Any())
        {
            SelectedItemsAddNew(itemsToSelect);
        }
    }

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

    private void UpdateSizeLimit(IWhiteboardControl whiteBoard)
    {
        Size maximumSize = FindSizeLimit((IWhiteboardItem)whiteBoard);
        whiteBoard.UpdateSizeLimits(maximumSize);
    }

    private Size FindSizeLimit(IWhiteboardItem whiteboardItem)
    {
        List<IWhiteboardItem> list = GetOverlappingItems(whiteboardItem).ToList();
        WhiteboardItems.Where(item => item is Quire.Controls.Text.IParagraphControl).Each(list.Add);
        return FindSizeLimit(whiteboardItem, list);
    }

    private Size FindSizeLimit(IWhiteboardItem item, IEnumerable<IWhiteboardItem> itemsToIgnore)
    {
        double num = double.PositiveInfinity;
        double num2 = double.PositiveInfinity;
        Point itemLocation = GetItemLocation(item);
        Size renderSize = WhiteboardItemMgr.GetRenderSize(item);
        foreach (IWhiteboardItem whiteboardItem in WhiteboardItems)
        {
            if (item != whiteboardItem && !itemsToIgnore.Contains(whiteboardItem))
            {
                Point itemLocation2 = GetItemLocation(whiteboardItem);
                itemLocation2.Offset(0.0, whiteboardItem.VerticalAlignmentOffset);
                Size renderSize2 = WhiteboardItemMgr.GetRenderSize(whiteboardItem);
                if (renderSize2.Width + itemLocation2.X > itemLocation.X && itemLocation2.X < itemLocation.X + renderSize.Width && itemLocation2.Y + renderSize2.Height > itemLocation.Y)
                {
                    num2 = Math.Min(num2, Math.Max(0.0, itemLocation2.Y - itemLocation.Y));
                }
                if (renderSize2.Height + itemLocation2.Y > itemLocation.Y && itemLocation2.Y < itemLocation.Y + renderSize.Height && itemLocation2.X + renderSize2.Width > itemLocation.X)
                {
                    num = Math.Min(num, itemLocation2.X - itemLocation.X);
                }
                if (itemLocation.X + renderSize.Width == itemLocation2.X && itemLocation.Y + renderSize.Height == itemLocation2.Y)
                {
                    num2 = Math.Min(num2, Math.Max(0.0, itemLocation2.Y - itemLocation.Y));
                }
            }
        }
        return new Size(num, num2);
    }

    private IEnumerable<IWhiteboardItem> GetOverlappingItems(IWhiteboardItem item)
    {
        Rect itemRect = GetItemRect(item);
        return WhiteboardItems.Where(otherItem => otherItem != item && itemRect.IntersectsWith(GetItemRect(otherItem)));
    }

    public virtual bool FindOncePrivate(string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        if (WhiteboardItems == null)
        {
            return false;
        }

        IWhiteboardItem whiteboardItem = ActiveOrSelectedItems.FirstOrDefault();
        Point startPoint = (whiteboardItem != null)
            ? GetItemLocation(whiteboardItem)
            : ((direction == LogicalDirection.Forward)
                ? new Point(double.NegativeInfinity, double.NegativeInfinity)
                : new Point(double.PositiveInfinity, double.PositiveInfinity));

        IEnumerable<WhiteboardItemSearchRestrictor> enumerable = ((direction == LogicalDirection.Forward)
            ? WhiteboardItems.Where(item => _whiteboardModel.PositionComparer.Compare(startPoint, GetItemLocation(item)) <= 0)
            : WhiteboardItems.Where(item => _whiteboardModel.PositionComparer.Compare(GetItemLocation(item), startPoint) <= 0).Reverse())
            .Select(WhiteboardItemSearchRestrictor.TryCreateFindRestrictor)
            .Where(restrictor => restrictor != null);

        bool flag = false;
        foreach (WhiteboardItemSearchRestrictor item in enumerable)
        {
            if (flag)
            {
                break;
            }

            flag = item.FindOnce(text, startFromCaret, direction, options, subscriptPositions);
            startFromCaret = false;
        }

        return flag;
    }

    public void FireRequestMaximumSize()
    {
        RequestMaximumSize();
    }

    public virtual void FireChildCausedResize(Size oldSize, Size newSize)
    {
        ChildCausedResize(oldSize, newSize);
    }

    public virtual RemoveItemsCommand GetRemoveItemsCommand(string commandName, IDictionary<IWhiteboardItem, Point> itemsAtLocations)
    {
        return new RemoveItemsCommand(this, "Remove Items", itemsAtLocations);
    }

    public void ClearSelectedItems()
    {
        if (_selectedItems.Any())
        {
            _selectedItems.Clear();
        }
    }

    public virtual Size ArrangeOverride(Size size)
    {
        // Mathcad IL enumerates through IEnumerable<T>, so the copy is viewed through the interface.
        foreach (IWhiteboardItem item in (IEnumerable<IWhiteboardItem>)new List<IWhiteboardItem>(WhiteboardItems))
        {
            WhiteboardItemMgr.ClearHandleChangesEnabled(item);
            Point itemLocation = WhiteboardItemMgr.GetItemLocation(item);
            itemLocation.Y += item.VerticalAlignmentOffset;
            WhiteboardItemMgr.Arrange(item, new Rect(itemLocation, WhiteboardItemMgr.GetDesiredSize(item)));
        }

        return size;
    }

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

    public virtual PasteItemsCommand GetPasteItemsCommand(string commandName, IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation)
    {
        return new PasteItemsCommand(this, commandName, pasteItemsAtLocation);
    }

    public virtual bool IsTabNavigatable => WhiteboardItems.Any(item => WhiteboardItemMgr.IsTabNavigatable(item));

    public virtual void TabInto(global::System.Windows.Input.FocusNavigationDirection dir)
    {
        WhiteboardItemMgr.TabInto((dir == FocusNavigationDirection.Next) ? WhiteboardItems.First(item => WhiteboardItemMgr.IsTabNavigatable(item)) : WhiteboardItems.Last(item => WhiteboardItemMgr.IsTabNavigatable(item)), dir);
    }

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

        public virtual IEnumerable<IWhiteboardItem> SortItemsByLocation(IEnumerable<IWhiteboardItem> items, bool descending)
    {
        return WhiteboardItemMgr.SortItemsByLocation(items, descending);
    }

    public event global::System.EventHandler Changed;

    public virtual bool HandleMouseDown(global::Quire.Controls.Core.Whiteboard.IWhiteboardControl sender, global::System.Windows.Point position, global::System.Windows.Input.MouseButton button)
    {
        bool result = false;
        if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0 && (Keyboard.Modifiers & ModifierKeys.Control) == 0)
        {
            if (IsValidInsertionPoint(position))
            {
                InsertionPoint = position;
            }

            if (ActiveItem != null)
            {
                return false;
            }

            WhiteboardControl whiteboardControl = sender as WhiteboardControl;
            if (whiteboardControl != null)
            {
                WhiteboardSelectionDragger whiteboardSelectionDragger = new WhiteboardSelectionDragger(whiteboardControl, position);
                whiteboardSelectionDragger.DragDone += rect =>
                {
                    TrySelectRegionsInBounds(rect, whiteboardControl);
                };
                result = true;
            }
        }

        return result;
    }

    public virtual void PreviewMouseLeftButtonDownHandler(object sender, MouseButtonEventArgs eventArgs)
    {
        object originalSource = eventArgs.OriginalSource;
        IWhiteboardItem whiteboardItem = (originalSource as DependencyObject).TryFindAncestorIncludingSelf<IWhiteboardItem>();
        if (whiteboardItem == null)
        {
            return;
        }

        IWhiteboardItem whiteboardItem2 = whiteboardItem;
        while (!WhiteboardItems.Contains(whiteboardItem2) && whiteboardItem2 != sender)
        {
            whiteboardItem2 = (whiteboardItem2 as DependencyObject).TryFindVisualAncestor<IWhiteboardItem>();
            if (whiteboardItem2 is IWhiteboardControl)
            {
                return;
            }
        }

        if (!WhiteboardItems.Contains(whiteboardItem2))
        {
            return;
        }

        if (((int)Keyboard.Modifiers & 2) > 0)
        {
            if ((whiteboardItem is IRichTextWithEmbeddedItem richTextWithEmbeddedItem && richTextWithEmbeddedItem.HasHyperlinkInMouseContext(eventArgs.OriginalSource)) || whiteboardItem is Quire.Controls.Include.IIncludeWorksheetControl { IncludedFilePath: not null })
            {
                eventArgs.Handled = false;
                return;
            }

            ToggleItemSelection(whiteboardItem2, includeActiveItem: true);
            eventArgs.Handled = true;
        }
        else if (((int)Keyboard.Modifiers & 4) > 0)
        {
            SelectContiguous(whiteboardItem2);
            eventArgs.Handled = true;
        }
        else
        {
            eventArgs.Handled = false;
        }
    }

    public virtual void PreviewMouseRightButtonDownHandler(object sender, MouseButtonEventArgs eventArgs)
    {
    }

    public virtual void PreviewMouseRightButtonUpHandler(object sender, RoutedEventArgs eventArgs)
    {
    }

    public virtual void PreviewMouseLeftButtonUpHandler(object sender, RoutedEventArgs eventArgs)
    {
    }

    public virtual bool HandleMouseUp(IWhiteboardControl sender, Point position, MouseButton button)
    {
        CurrentDragMode = DragMode.None;
        return false;
    }

    public virtual IEnumerable<IWhiteboardItem> RecursivelyEnumerateDescendantItemssBypassWhiteboard()
    {
        return RecursivelyEnumerateDescendantsIgnoringTypes(WhiteboardItems);
    }

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

    public void HandleCaretLeavesTextArea(CommonRoutedEvents.CaretLeavesTextRegionEventArgs args)
    {
        Direction direction = Direction.Down;
        switch (args.LeavingDirection)
        {
            case CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Down:
                direction = Direction.Down;
                break;
            case CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Up:
                direction = Direction.Up;
                break;
            case CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Left:
                direction = Direction.Left;
                break;
            case CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Right:
                direction = Direction.Right;
                break;
        }

        Point navigationStartLocation = GetNavigationStartLocation(direction);
        Point nearestIntersectionInDirection = _alignmentGrid.GetNearestIntersectionInDirection(navigationStartLocation, direction);
        bool flag = false;
        switch (direction)
        {
            case Direction.Down:
                flag = nearestIntersectionInDirection.Y.IsGreaterThanOrEqual(ContentHeight);
                break;
            case Direction.Up:
                flag = nearestIntersectionInDirection.Y.IsLessThanOrEqual(0.0);
                break;
            case Direction.Left:
                flag = nearestIntersectionInDirection.X.IsLessThanOrEqual(0.0);
                break;
            case Direction.Right:
                flag = nearestIntersectionInDirection.X.IsGreaterThanOrEqual(ContentWidth);
                break;
        }

        // Mathcad IL returns early when the caret stays inside the grid (brfalse to the body; ret).
        if (flag)
        {
            return;
        }

        DirectionalMoveHandler(direction);
        args.Handled = true;
    }

    public static IEnumerable<IWhiteboardItem> RecursivelyEnumerateDescendantsIgnoringTypes(IEnumerable<IWhiteboardItem> items, params Type[] typesToIgnore)
    {
        foreach (IWhiteboardItem whiteboardItem in items)
        {
            if (!(whiteboardItem is IWhiteboardControl whiteboardControl))
            {
                if (typesToIgnore.All((Type ignoreType) => !ignoreType.IsInstanceOfType(whiteboardItem)))
                {
                    yield return whiteboardItem;
                }

                continue;
            }

            foreach (IWhiteboardItem descendantWhiteboardItem in whiteboardControl.RecursivelyEnumerateDescendantItemssBypassWhiteboard())
            {
                if (typesToIgnore.All((Type ignoreType) => !ignoreType.IsInstanceOfType(descendantWhiteboardItem)))
                {
                    yield return descendantWhiteboardItem;
                }
            }
        }
    }

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

    protected virtual void HandleCarriageReturn()
    {
        if (EnumerableExtensions.IsNullOrEmpty(ActiveOrSelectedItems))
        {
            InsertVerticalSpace(1, pushInsertionPoint: true, ignoreItemsOnCurrentLine: false);
            return;
        }

        Point insertionPoint = GetInsertionPointAfterCarriageReturn();
        InsertionPoint = insertionPoint;
        Point nearestIntersectionInDirection = Grid.GetNearestIntersectionInDirection(insertionPoint, Direction.Below);
        if (DoubleExtension.IsGreaterThanOrEqual(nearestIntersectionInDirection.Y, ContentHeight, double.NaN))
        {
            Commands.WhiteboardCommand command = new InsertSpaceCommand(this, 1, pushInsertionPoint: true, ignoreItemsOnCurrentLine: true, Array.Empty<IWhiteboardItem>());
            command.Execute();
        }
    }

    protected Point GetInsertionPointAfterCarriageReturn()
    {
        double lowestItemEdge = GetLowestItemEdge(ActiveOrSelectedItems);
        return new Point(ActiveOrSelectedItems.Min((IWhiteboardItem item) => GetItemLocation(item).X), lowestItemEdge + Grid.GridCellSize.Height / 2.0);
    }

    protected double GetRightMostItemEdge(IEnumerable<IWhiteboardItem> items)
    {
        if (EnumerableExtensions.IsNullOrEmpty(items))
        {
            return 0.0;
        }

        return items.Max((IWhiteboardItem item) => GetItemRightEdge(item));
    }

    protected double GetLowestItemEdge(IEnumerable<IWhiteboardItem> items)
    {
        if (EnumerableExtensions.IsNullOrEmpty(items))
        {
            return 0.0;
        }

        return items.Max((IWhiteboardItem item) => GetItemBottom(item));
    }

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

    protected void DirectionalMoveHandler(Direction direction, bool allowGrowthDownOrRight)
    {
        Point point;
        if (HasSelectedItems)
        {
            Point itemLocation = GetItemLocation(SelectedItems.First());
            Vector vector = GetNextIntersectionForMove(itemLocation, direction) - itemLocation;
            Commands.WhiteboardCommand command = new MoveItemsCommand(this, vector, allowGrowthDownOrRight, SelectedItems.ToArray());
            if (command.Execute())
            {
                RequestBringIntoView?.Invoke(SelectedItems);
            }
            return;
        }

        point = _alignmentGrid.GetNearestIntersectionInDirection(GetNavigationStartLocation(direction), direction);
        NavigateInto(direction, point);
    }

    protected virtual Point GetNextIntersectionForMove(Point currentLocation, Direction direction)
    {
        return _alignmentGrid.GetNextIntersection(currentLocation, direction);
    }

    protected virtual bool IsValidInsertionPoint(Point pt)
    {
        return true;
    }

    protected virtual void HandleTraverse(FocusNavigationDirection direction)
    {
        IWhiteboardItem whiteboardItem = PredictFocus(direction);
        if (whiteboardItem != null)
        {
            WhiteboardItemMgr.TabInto(whiteboardItem, direction);
        }
    }

    private void DeleteActiveItem()
    {
        if (ActiveItem != null)
        {
            GetRemoveItemsCommand("Remove Active Item", GetItemAtLocation(ActiveItem)).Execute();
        }
    }

    protected void DeleteSelectedItems()
    {
        Commands.WhiteboardCommand command = new RemoveSelectedItemsCommand(this, "Remove Selected Items");
        command.Execute();
    }

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

    protected IWhiteboardItem PredictFocus(FocusNavigationDirection direction)
    {
        if (WhiteboardItems.Count == 0)
        {
            return null;
        }

        IEnumerable<IWhiteboardItem> whiteboardItems;
        IEnumerable<IWhiteboardItem> enumerable = (direction == FocusNavigationDirection.Previous)
            ? WhiteboardItems.Reverse()
            : (whiteboardItems = WhiteboardItems);

        return GetItemsAccordingTabNavigateOrder(enumerable, direction, ActiveItem).FirstOrDefault(item => WhiteboardItemMgr.IsTabNavigatable(item));
    }

    protected virtual IEnumerable<IWhiteboardItem> GetItemsAccordingTabNavigateOrder(IEnumerable<IWhiteboardItem> itemsAccordingTabDirection, FocusNavigationDirection direction, IWhiteboardItem startItem)
    {
        if (startItem == null)
        {
            return (direction == FocusNavigationDirection.Previous)
                ? itemsAccordingTabDirection.SkipWhile(item => _whiteboardModel.PositionComparer.Compare(InsertionPoint, GetItemLocation(item)) < 0)
                : itemsAccordingTabDirection.SkipWhile(item => _whiteboardModel.PositionComparer.Compare(InsertionPoint, GetItemLocation(item)) > 0);
        }

        return itemsAccordingTabDirection.Skip(itemsAccordingTabDirection.IndexOf(startItem) + 1);
    }

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

    public virtual bool CanSeparateOverlappedItems(Dimension dimension)
    {
        if (ActiveItem != null)
        {
            return false;
        }

        INotifyCollectionRangeChanged<IWhiteboardItem> whiteboardItems;
        IEnumerable<IWhiteboardItem> items = SelectedItems.Any()
            ? SelectedItems
            : (whiteboardItems = WhiteboardItems);

        if (items.OfType<IWhiteboardControl>().Any((IWhiteboardControl wb) => wb.CanSeparateOverlappedItems(dimension)))
        {
            return true;
        }

        if (dimension == Dimension.Horizontal)
        {
            items = items.Where((IWhiteboardItem item) => !GetItemIgnoreForHorizontalMovementAndWidth(item));
        }

        return ItemsOverlapped(items);
    }

    public void SeparateOverlappedItems(Dimension dimension)
    {
        INotifyCollectionRangeChanged<IWhiteboardItem> whiteboardItems;
        IEnumerable<IWhiteboardItem> items = SelectedItems.Any()
            ? SelectedItems
            : (whiteboardItems = WhiteboardItems);

        IEnumerable<IWhiteboardItem> itemsToSeparate = items;
        if (dimension == Dimension.Horizontal)
        {
            itemsToSeparate = itemsToSeparate.Where((IWhiteboardItem item) => !GetItemIgnoreForHorizontalMovementAndWidth(item));
        }

        using (new global::Quire.Undo.CommandScope("Separate Overlapped Items"))
        {
            items.OfType<IWhiteboardControl>()
                .Where((IWhiteboardControl wb) => wb.CanSeparateOverlappedItems(dimension))
                .ToList()
                .Each((IWhiteboardControl wb) => wb.SeparateOverlappedItems(dimension));

            CreateSeparateOverlappedItemsCommand(itemsToSeparate, dimension).Execute();
        }
    }

        protected virtual SeparateOverlappedItemsCommand CreateSeparateOverlappedItemsCommand(IEnumerable<IWhiteboardItem> itemsToSeparate, Dimension dimension)
    {
        return new SeparateOverlappedItemsCommand(this, itemsToSeparate, dimension);
    }

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

    protected override void OnDispose()
    {
        ClearSelectedItems();
        if (_activeItem != null)
        {
            UnhookActiveItem(_activeItem);
            _activeItem = null;
        }
        ClearSavedState();
        StringBuilder stringBuilder = new StringBuilder();
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
        DebugTools.DevAssert(stringBuilder.Length == 0, stringBuilder.ToString());
        _whiteboardModel.Dispose();
        _alignmentGrid.Dispose();
    }

    protected virtual IWhiteboardItem GetChildItemFromLocation(Point point)
    {
        if (WhiteboardItems == null || WhiteboardItems.Count == 0)
        {
            return null;
        }

        return WhiteboardItems.Reverse().FirstOrDefault((IWhiteboardItem whiteboardItem) => GetItemRect(whiteboardItem).ContainsAccountingForMeasurementError(point));
    }

    protected virtual void SelectedItemsChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (SelectedItems.Any() && FirstSelectedItem != SelectedItems.First())
        {
            FirstSelectedItem = SelectedItems.First();
        }
        else if (SelectedItems.IsEmpty())
        {
            FirstSelectedItem = null;
        }

        if (SelectedItems.Any() && ActiveItem != null)
        {
            ActiveItem = null;
        }

        if (SelectedItems.Any())
        {
            ClearSavedState();
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (SelectedItems.Count() == e.NewItems.Count)
                {
                    RequestActivatePartContainingChild(SelectedItems.First());
                    Activate(isActive: true);
                }

                IsFocused = true;
            }

            IsCrosshairVisible = false;
        }

        if (HandleUserInput)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.Cast<IWhiteboardItem>().Each(WhiteboardItemMgr.SelectItem);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.Cast<IWhiteboardItem>().Each(WhiteboardItemMgr.DeselectItem);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    WhiteboardItems.Each(WhiteboardItemMgr.DeselectItem);
                    break;
                default:
                    DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                    break;
            }
        }

        OnPropertyChanged("SelectedItems");
    }

    protected virtual void HandleItemAdded(IWhiteboardItem item)
    {
        if (HandleUserInput)
        {
            item.OnGotFocusEvent += HandleChildItemGotFocus;
            item.RequestRemove += HandleChildRequestRemove;
            WhiteboardItemSizeChangedEventHandler handler = WhiteboardItemMgr.HookupSizeChanged(item, HandleChildSizeChanged);
            _itemSizeChangedHandlerMap.Add(item, handler);
            IEquationControl eq = item as IEquationControl;
            if (eq != null)
            {
                eq.RootNodeChanged += () => SetEqResizable(eq);
            }

            if (item is IWhiteboardControl whiteboardControl)
            {
                whiteboardControl.ChildTriggeredResize += ChildTriggeredResizeHandler;
            }
        }

        FireChanged();
    }

    protected virtual void HandleItemRemoved(IWhiteboardItem item)
    {
        if (HandleUserInput)
        {
            item.OnGotFocusEvent -= HandleChildItemGotFocus;
            item.GetRegionsOfType<IOleBaseObjectControl>().Each((IOleBaseObjectControl ole) => ole.CloseOle());
            item.GetRegionsOfType<ISpellCheckRegion>().Each((ISpellCheckRegion text) => text.DisableSpellCheck());
            item.RequestRemove -= HandleChildRequestRemove;
            _itemSizeChangedHandlerMap.TryGetValue(item, out WhiteboardItemSizeChangedEventHandler handler);
            if (handler != null)
            {
                WhiteboardItemMgr.UnhookSizeChanged(item, handler);
            }

            _itemSizeChangedHandlerMap.Remove(item);
            if (item is IWhiteboardControl whiteboardControl)
            {
                whiteboardControl.ChildTriggeredResize -= ChildTriggeredResizeHandler;
            }
        }

        FireChanged();
    }

    private void ChildTriggeredResizeHandler(IWhiteboardControl whiteboard, Size oldSize, Size newSize)
    {
        IWhiteboardItem item = whiteboard as IWhiteboardItem;
        if (item == null)
        {
            return;
        }

        Rect itemRect = WhiteboardItemMgr.GetItemRect(item);
        global::Quire.Undo.CommandBuffer.ExecuteAndAttach(new ResizeItemCommandWithSideEffects(this, item, oldSize, newSize, false, false));
        if (newSize.Height.IsLessThanOrEqual(oldSize.Height))
        {
            return;
        }

        double heightIncreased = newSize.Height - oldSize.Height;
        double num = 0.0;
        itemRect.Height += heightIncreased;
        double itemsToPushDownTop = itemRect.Bottom;
        IEnumerable<IWhiteboardItem> source = WhiteboardItems.Where(other =>
        {
            if (other == item)
            {
                return false;
            }

            Rect otherRect = GetItemRect(other);
            return otherRect.Right.IsGreaterThan(itemRect.Left) &&
                   otherRect.Left.IsLessThan(itemRect.Right) &&
                   otherRect.Top.IsLessThanOrEqual(itemRect.Bottom) &&
                   otherRect.Top.IsGreaterThanOrEqual(itemRect.Bottom - heightIncreased);
        });

        if (source.Any())
        {
            itemsToPushDownTop = source.Min(i => GetItemRect(i).Top);
            num = itemRect.Bottom - itemsToPushDownTop;
        }

        if (num > 0.0)
        {
            num += Grid.GridCellSize.Height * 0.5;
            global::Quire.Undo.CommandBuffer.ExecuteAndAttach(new MoveItemsCommandWithSideEffects(this, new Vector(0.0, num), true, WhiteboardItems.Where(other =>
            {
                if (other != item)
                {
                    Rect otherRect = GetItemRect(other);
                    return otherRect.Top.IsGreaterThanOrEqual(itemsToPushDownTop);
                }

                return false;
            }).ToArray()));
        }
    }

    protected virtual void HandleItemMoved(IWhiteboardItem item)
    {
    }

    protected virtual void HandleChildSizeChanged(IWhiteboardItem item, Size oldSize, Size newSize)
    {
        SetEqResizable(item);
        Rect itemRect = GetItemRect(item);
        Size contentSize = ContentSize;
        if (newSize.Width.IsGreaterThan(oldSize.Width) && itemRect.Right.IsGreaterThan(ContentWidth))
        {
            contentSize.Width = itemRect.Right;
        }

        if (newSize.Height.IsGreaterThan(oldSize.Height) && itemRect.Bottom.IsGreaterThan(ContentHeight))
        {
            contentSize.Height = itemRect.Bottom;
        }

        if (contentSize != ContentSize)
        {
            ChildCausedResize(ContentSize, contentSize);
        }
    }

    protected void SetEqResizable(IWhiteboardItem item)
    {
        IEquationControl eq = item as IEquationControl;
        if (eq != null)
        {
            Action action = () =>
            {
                WhiteboardDependencyProperties.SetShowResizeIndicator(eq as DependencyObject, eq.IsShrunk);
                WhiteboardDependencyProperties.SetUserResizable(eq as DependencyObject, eq.IsResizable);
            };
            eq.ExecuteOnRootLoaded(action);
        }
    }

    protected virtual bool ItemsOverlapped(IEnumerable<IWhiteboardItem> items)
    {
        return WhiteboardItemMgr.ItemsOverlapped(items);
    }

    public virtual Rect GetItemRect(IWhiteboardItem item)
    {
        return WhiteboardItemMgr.GetItemRect(item);
    }

    protected virtual void SetupCommandBindings()
    {
        CreateCommandBinding(EditingCommands.MoveDownByLine, MoveDownCommand.Execute, MoveDownCommand.CanExecute);
        CreateCommandBinding(EditingCommands.MoveUpByLine, MoveUpCommand.Execute, MoveUpCommand.CanExecute);
        CreateCommandBinding(EditingCommands.MoveLeftByCharacter, MoveLeftCommand.Execute, MoveLeftCommand.CanExecute);
        CreateCommandBinding(EditingCommands.MoveRightByCharacter, MoveRightCommand.Execute, MoveRightCommand.CanExecute);
        CreateCommandBinding(EditingCommands.EnterParagraphBreak, CarriageReturnCommand.Execute);
        CreateCommandBinding(EditingCommands.TabForward, TraverseForwardCommand.Execute, TraverseForwardCommand.CanExecute);
        CreateCommandBinding(EditingCommands.TabBackward, TraverseBackwardCommand.Execute, TraverseBackwardCommand.CanExecute);
        CreateCommandBinding(EditingCommands.Delete, (object param) =>
        {
            HandleDelete();
        }, (object param) => CanDelete());
        CreateCommandBinding(EditingCommands.Backspace, (object param) =>
        {
            HandleBackspace();
        }, (object param) => CanBackspace());
        CreateCommandBinding(WhiteboardCommands.DeleteActiveItem, (object param) =>
        {
            DeleteActiveItem();
        }, (object param) => CanDeleteActiveItem());
        CreateCommandBinding(WhiteboardCommands.DeleteSelectedItems, (object param) =>
        {
            DeleteSelectedItems();
        }, (object param) => CanDeleteSelectedItems());
        CreateCommandBinding(WhiteboardCommands.DeleteActiveOrSelectedItems, (object param) =>
        {
            DeleteActiveOrSelectedItems();
        }, (object param) => CanDeleteActiveOrSelectedItems());
        CreateCommandBinding(ApplicationCommands.Copy, (object param) =>
        {
            CopySelection();
        }, (object param) => CanCopySelection());
        CreateCommandBinding(ApplicationCommands.Cut, (object param) =>
        {
            CutSelection();
        }, (object param) => CanCutSelection());
        CreateCommandBinding(WhiteboardCommands.SeparateOverlappedItemsVertically, (object param) =>
        {
            SeparateOverlappedItems(Dimension.Vertical);
        }, (object param) => CanSeparateOverlappedItems(Dimension.Vertical));
        CreateCommandBinding(WhiteboardCommands.SeparateOverlappedItemsHorizontally, (object param) =>
        {
            SeparateOverlappedItems(Dimension.Horizontal);
        }, (object param) => CanSeparateOverlappedItems(Dimension.Horizontal));
        CreateCommandBinding(WhiteboardCommands.SelectAllRegions, (object param) =>
        {
            HandleSelectAll();
        });
    }

    public virtual bool CanInsertVerticalSpace
    {
        get
        {
            return ActiveOrSelectedItems.Count() == 0;
        }
    }

    public virtual bool CanDeleteVerticalSpaceAbove => CanDeleteVerticalSpace(Direction.Above);

    public virtual bool CanDeleteVerticalSpaceBelow => CanDeleteVerticalSpace(Direction.Below);

    public virtual void InsertVerticalSpace(int numberOfLines, bool pushInsertionPoint, bool ignoreItemsOnCurrentLine)
    {
        Commands.WhiteboardCommand command = new InsertSpaceCommand(this, numberOfLines, pushInsertionPoint, Array.Empty<IWhiteboardItem>());
        command.Execute();
    }

    public virtual void DeleteVerticalSpaceAbove(int numberOfLines)
    {
        Commands.WhiteboardCommand command = new DeleteSpaceCommand(this, numberOfLines, Direction.Above);
        command.Execute();
    }

    public virtual void DeleteVerticalSpaceBelow(int numberOfLines)
    {
        Commands.WhiteboardCommand command = new DeleteSpaceCommand(this, numberOfLines, Direction.Below);
        command.Execute();
    }

    public double LimitOffsetBasedOnItemsToMove(IEnumerable<IWhiteboardItem> itemsToMove, double offsetY, Direction direction, Point insertionPointLocation)
    {
        double height = Grid.GridCellSize.Height;
        double val = -offsetY;
        double? num = itemsToMove.Any() ? new double?(itemsToMove.Min(item => GetItemTop(item))) : null;
        IEnumerable<IWhiteboardItem> source = WhiteboardItems.Except(itemsToMove);
        double? num2 = source.Any() ? new double?(source.Max(item => GetItemBottom(item))) : null;
        double num4;
        double num3;
        if (direction == Direction.Down)
        {
            num3 = num2.HasValue ? Math.Max(insertionPointLocation.Y, num2.Value) : insertionPointLocation.Y;
            num4 = num.HasValue ? num.Value : ContentHeight;
        }
        else
        {
            num3 = num2.HasValue ? num2.Value : 0.0;
            num4 = num.HasValue ? Math.Min(insertionPointLocation.Y, num.Value) : insertionPointLocation.Y;
        }
        val = Math.Min(val, num4 - num3);
        if (val.IsLessThan(0.0))
        {
            val = 0.0;
        }
        return -((val / height).Floor() * height);
    }

    protected virtual bool CanDeleteVerticalSpace(Direction direction)
    {
        double cellHeight = Grid.GridCellSize.Height;
        DoubleComparer defaultDoubleComparer = DoubleComparer.DefaultDoubleComparer;
        if (ActiveOrSelectedItems.Any())
        {
            return false;
        }
        if (defaultDoubleComparer.Compare(ContentHeight - cellHeight, StrictMinimumSize.Height) < 0)
        {
            return false;
        }
        if (direction == Direction.Above)
        {
            return !WhiteboardItems.Any(item =>
            {
                Rect itemRect = GetItemRect(item);
                itemRect.Y -= item.VerticalAlignmentOffset;
                if (itemRect.Y.IsEqualTo(InsertionPoint.Y))
                {
                    if (itemRect.Left.IsLessThan(InsertionPoint.X))
                    {
                        // IL requires goto to match single-exit brtrue.s jump to final ldc.i4.1; ret sequence
                        goto ReturnTrue;
                    }
                }
                if (itemRect.Y.IsEqualTo(InsertionPoint.Y - cellHeight))
                {
                    return itemRect.Right.IsGreaterThanOrEqual(InsertionPoint.X);
                }
                return false;
            ReturnTrue:
                return true;
            });
        }
        return !WhiteboardItems.Any(item =>
        {
            Rect itemRect = GetItemRect(item);
            itemRect.Y -= item.VerticalAlignmentOffset;
            if (itemRect.Y.IsEqualTo(InsertionPoint.Y))
            {
                if (itemRect.Left.IsGreaterThanOrEqual(InsertionPoint.X))
                {
                    // IL requires goto to match single-exit brtrue.s jump to final ldc.i4.1; ret sequence
                    goto ReturnTrue;
                }
            }
            if (itemRect.Y.IsEqualTo(InsertionPoint.Y + cellHeight))
            {
                return itemRect.Right.IsLessThan(InsertionPoint.X);
            }
            return false;
        ReturnTrue:
            return true;
        });
    }

        public virtual Size ResizeAndNotify(IEnumerable<IWhiteboardItem> list, Size oldSize)
    {
        Size val = oldSize;
        foreach (IWhiteboardItem item in list)
        {
            Point itemLocation = GetItemLocation(item);
            Size itemDesiredSize = GetItemDesiredSize(item);
            val.Width = Math.Max(val.Width, itemDesiredSize.Width + itemLocation.X);
            val.Height = Math.Max(val.Height, itemDesiredSize.Height + itemLocation.Y + item.VerticalAlignmentOffset);
        }

        if (val != oldSize)
        {
            FireChildCausedResize(oldSize, val);
        }

        return val;
    }

    public virtual bool IsValidLocation(IWhiteboardItem item, IDictionary<IWhiteboardItem, Point> newLocations, bool allowGrowthDownOrRight)
    {
        Point point = newLocations[item];
        if (GetItemIgnoreForHorizontalMovementAndWidth(item))
        {
            if (!allowGrowthDownOrRight)
            {
                if (point.Y.IsGreaterThanOrEqual(0.0))
                {
                    return point.Y.IsLessThanOrEqual(ContentHeight);
                }
                return false;
            }
            return point.Y.IsGreaterThanOrEqual(0.0);
        }
        if (!allowGrowthDownOrRight)
        {
            if (point.X.IsGreaterThanOrEqual(0.0) && point.Y.IsGreaterThanOrEqual(0.0) && point.X.IsLessThanOrEqual(ContentWidth))
            {
                return point.Y.IsLessThanOrEqual(ContentHeight);
            }
            return false;
        }
        if (point.X.IsGreaterThanOrEqual(0.0))
        {
            return point.Y.IsGreaterThanOrEqual(0.0);
        }
        return false;
    }

    CommandBindingCollection IWhiteboardViewModel.CommandBindings
    {
        get
        {
            return base.CommandBindings;
        }
    }



    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+WhiteboardResizingCommand, Ptc.Controls.Whiteboard")]
    public abstract partial class WhiteboardResizingCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        protected global::System.Windows.Size _oldSize;
        protected global::Quire.Controls.Core.IResizeData _oldData;
        protected WhiteboardResizingCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, string commandName)
            : base(viewModel, commandName)
        {
        }
        [global::Quire.Mirror.MirrorStub]
        protected abstract bool DoWithResize();
        protected void BackupTheSize()
        {
            Size size = _viewModel.ContentSize;
            if (_viewModel is IResizableElementViewModelWithDataStore resizable)
            {
                _oldData = resizable.StoreResizeData(size, size);
            }

            _oldSize = size;
        }

        public override bool Do()
        {
            BackupTheSize();
            return DoWithResize();
        }

        public void UndoResize()
        {
            if (_viewModel is IResizableElementViewModelWithDataStore resizable)
            {
                resizable.RestoreResizeData(_oldData);
                return;
            }

            _viewModel.MinimumSize = new Size(0.0, 0.0);
            _viewModel.MaximumSize = WpfUtils.SizeInfinity;
            _viewModel.ContentSize = _oldSize;
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+AddItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class AddItemsCommand : global::Quire.Controls.Whiteboard.WhiteboardViewModel.WhiteboardResizingCommand
    {
        private readonly IDictionary<IWhiteboardItem, Point> _itemsAtLocations;
        private Point _insertionPoint;
        private readonly bool _shouldActivate;
        public AddItemsCommand(IWhiteboardViewModel viewModel, string commandName, IWhiteboardItem item)
            : this(viewModel, commandName, new Dictionary<IWhiteboardItem, Point> { { item, viewModel.InsertionPoint } })
        {
            _shouldActivate = true;
        }
        public AddItemsCommand(IWhiteboardViewModel viewModel, string commandName, IDictionary<IWhiteboardItem, Point> itemsAtLocations)
            : base(viewModel, commandName)
        {
            _itemsAtLocations = itemsAtLocations;
            _insertionPoint = viewModel.InsertionPoint;
        }
        protected override bool DoWithResize()
        {
            if (_itemsAtLocations.IsNullOrEmpty())
            {
                return false;
            }
            _insertionPoint = _viewModel.InsertionPoint;
            _itemsAtLocations.Each((KeyValuePair<IWhiteboardItem, Point> itemAtLocation) =>
            {
                _viewModel.AddItemAtLocation(itemAtLocation.Key, itemAtLocation.Value);
            });
            WhiteboardItemMgr.ExecuteInBackground(() =>
            {
                _viewModel.ResizeAndNotify(_itemsAtLocations.Keys, _viewModel.ContentSize);
            }, true);
            if (_shouldActivate)
            {
                Dictionary<IWhiteboardItem, bool> shouldItemBeDeletedIfEmptyCache = new Dictionary<IWhiteboardItem, bool>(_itemsAtLocations.Count);
                _itemsAtLocations.Each((KeyValuePair<IWhiteboardItem, Point> itemAtLocation) =>
                {
                    IWhiteboardItem key2 = itemAtLocation.Key;
                    shouldItemBeDeletedIfEmptyCache.Add(key2, WhiteboardItemMgr.ShouldItemBeDeletedIfEmpty(key2));
                    WhiteboardItemMgr.SetShouldItemBeDeletedIfEmpty(itemAtLocation.Key, false);
                });
                IWhiteboardItem key = _itemsAtLocations.First().Key;
                if (_viewModel.ActiveItem == key)
                {
                    _viewModel.ActivateActiveItem();
                }
                else
                {
                    _viewModel.ActiveItem = key;
                }
                shouldItemBeDeletedIfEmptyCache.Each((KeyValuePair<IWhiteboardItem, bool> itemAtLocation) =>
                {
                    IWhiteboardItem key2 = itemAtLocation.Key;
                    WhiteboardItemMgr.ClearShouldItemBeDeletedIfEmpty(key2);
                    if (WhiteboardItemMgr.ShouldItemBeDeletedIfEmpty(key2) != itemAtLocation.Value)
                    {
                        WhiteboardItemMgr.SetShouldItemBeDeletedIfEmpty(key2, itemAtLocation.Value);
                    }
                });
            }
            return true;
        }
        public override void Undo()
        {
            _viewModel.GetRemoveItemsCommand("Remove Items", _itemsAtLocations).Do();
            _viewModel.InsertionPoint = _insertionPoint;
            UndoResize();
            _viewModel.IsFocused = true;
            _viewModel.IsCrosshairVisible = true;
        }

        public override void Redo()
        {
            Do();
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+RemoveItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class RemoveItemsCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        protected readonly global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> _itemsAtLocations;
        private readonly global::Quire.Controls.Core.IWhiteboardItem _activeItemPriorToRemoval;
        private readonly global::System.Windows.Point _insertionPoint;
        public bool SetInsertionPointToFirstItemsLocation { set; private get; }
        public RemoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, string commandName, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> itemsAtLocations)
            : base(viewModel, commandName)
        {
            if (itemsAtLocations.Count == 1)
            {
                IWhiteboardItem whiteboardItem = itemsAtLocations.Keys.First();
                if (whiteboardItem == viewModel.ActiveItem)
                {
                    _activeItemPriorToRemoval = whiteboardItem;
                }
            }
            SetInsertionPointToFirstItemsLocation = true;
            _insertionPoint = _viewModel.InsertionPoint;
            _itemsAtLocations = itemsAtLocations;
        }
        public override bool Do()
        {
            ViewModel.UnSelectItems(_itemsAtLocations.Keys);
            _itemsAtLocations.Each((KeyValuePair<IWhiteboardItem, Point> itemAtLocation) =>
            {
                IWhiteboardItem key = itemAtLocation.Key;
                WhiteboardItemMgr.DeselectItem(key);
                ViewModel._whiteboardModel.RemoveItem(key);
                if (ViewModel._activeItem == key)
                {
                    bool flag = WhiteboardItemMgr.ShouldItemBeDeletedIfEmpty(key);
                    WhiteboardItemMgr.SetShouldItemBeDeletedIfEmpty(key, false);
                    ViewModel.ActiveItem = null;
                    WhiteboardItemMgr.ClearShouldItemBeDeletedIfEmpty(key);
                    if (WhiteboardItemMgr.ShouldItemBeDeletedIfEmpty(key) != flag)
                    {
                        WhiteboardItemMgr.SetShouldItemBeDeletedIfEmpty(key, flag);
                    }
                }
            });
            if (ViewModel.HandleUserInput)
            {
                if (SetInsertionPointToFirstItemsLocation)
                {
                    ViewModel.InsertionPoint = _itemsAtLocations.First().Value;
                }
                else if (!ViewModel.IsSelected)
                {
                    ViewModel.InsertionPoint = _insertionPoint;
                }
            }
            return true;
        }
        public override void Undo()
        {
            Commands.WhiteboardCommand command = new AddItemsCommand(_viewModel, "Add Item", _itemsAtLocations);
            command.Do();
            if (_activeItemPriorToRemoval != null)
            {
                _activeItemPriorToRemoval.Activate(true);
            }
        }
        public override void Redo()
        {
            Do();
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+RemoveSelectedItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class RemoveSelectedItemsCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        protected readonly global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> _itemsAtLocations;
        private readonly global::Quire.Undo.CompoundCommand _nestedCommand = new global::Quire.Undo.CompoundCommand();

        public RemoveSelectedItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, string commandName)
            : base(viewModel, commandName)
        {
            _itemsAtLocations = new global::System.Collections.Generic.Dictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point>();
            _viewModel.SelectedItems.Each((global::Quire.Controls.Core.IWhiteboardItem item) =>
            {
                _itemsAtLocations.Add(item, _viewModel.GetItemLocation(item));
            });
        }
        public override bool Do()
        {
            bool result;
            using (var scope = new global::Quire.Undo.CommandScope(_nestedCommand))
            {
                result = _viewModel.GetRemoveItemsCommand("Remove Selected Items", _itemsAtLocations).Execute();
            }

            if (result)
            {
                ViewModel.IsFocused = true;
                ViewModel.IsCrosshairVisible = true;
            }

            return result;
        }

        public override void Undo()
        {
            _nestedCommand.Undo();
            ViewModel.SelectItems(_itemsAtLocations.Keys);
        }

        public override void Redo()
        {
            _nestedCommand.Redo();
            ViewModel.IsFocused = true;
            ViewModel.IsCrosshairVisible = true;
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+PasteItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class PasteItemsCommand : global::Quire.Controls.Whiteboard.WhiteboardViewModel.WhiteboardResizingCommand
    {
        protected readonly global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> _itemsAtLocation;
        private readonly global::Quire.Controls.Whiteboard.WhiteboardViewModel.AddItemsCommand _addItemsCommand;
        private readonly global::Quire.Controls.Core.IWhiteboardItem _itemToActivate;

        public PasteItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, string commandName, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> pasteItemsAtLocation)
            : base(viewModel, commandName)
        {
            _itemsAtLocation = new global::System.Collections.Generic.Dictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point>(pasteItemsAtLocation);
            _itemToActivate = _itemsAtLocation.Keys.FirstOrDefault((global::Quire.Controls.Core.IWhiteboardItem item) => WhiteboardItemMgr.IsItemActivated(item));
            _addItemsCommand = new AddItemsCommand(_viewModel, "Add Whiteboard Items", _itemsAtLocation);
        }
        protected override bool DoWithResize()
        {
            if (!_addItemsCommand.Do())
            {
                return false;
            }

            WhiteboardItemMgr.BlockUntilLoaded();
            _viewModel.ClearSelectedItems();
            if (_itemToActivate != null)
            {
                _itemToActivate.Activate(true);
            }
            else
            {
                ViewModel.SelectItems(_itemsAtLocation.Keys);
            }

            return true;
        }

        public override void Undo()
        {
            _addItemsCommand.Undo();
            UndoResize();
        }

        public override void Redo()
        {
            Do();
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+ResizeItemCommand, Ptc.Controls.Whiteboard")]
    public partial class ResizeItemCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        private readonly global::System.Windows.Size _oldSize;
        private readonly global::System.Windows.Size _newSize;
        private readonly global::Quire.Controls.Core.IWhiteboardItem _item;
        private readonly global::Quire.Controls.Core.DragMode _dragMode;
        private bool _isActiveItem;
        private readonly bool _widthIsAuto;
        private readonly bool _heightIsAuto;
        private global::Quire.Controls.Core.IResizeData _oldData;
        public ResizeItemCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Size oldSize, global::System.Windows.Size newSize, bool widthIsAuto, bool heightIsAuto, global::Quire.Controls.Core.DragMode dragMode)
            : base(whiteboard, "Resize Item")
        {
            _item = item;
            _oldSize = oldSize;
            _newSize = newSize;
            _widthIsAuto = widthIsAuto;
            _heightIsAuto = heightIsAuto;
            _dragMode = dragMode;
        }
        public override bool Do()
        {
            DoRedo(true);
            return true;
        }

        public override void Undo()
        {
            Size size = _oldSize;
            if (_widthIsAuto)
            {
                size.Width = double.NaN;
            }

            if (_heightIsAuto)
            {
                size.Height = double.NaN;
            }

            if (_item is IResizableElementWithDataStore resizable)
            {
                resizable.RestoreResizeData(_oldData);
            }
            else
            {
                _viewModel.SetItemSize(_item, size, _dragMode);
            }

            if (_isActiveItem)
            {
                _item.Activate(true);
            }
        }

        public override void Redo()
        {
            DoRedo(false);
        }

        private void DoRedo(bool doAction)
        {
            _isActiveItem = _viewModel.ActiveItem == _item;
            IResizableElementWithDataStore resizable = _item as IResizableElementWithDataStore;
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

            _viewModel.SetItemSize(_item, newSize, _dragMode);
        }

    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+ResizeItemCommandWithSideEffects, Ptc.Controls.Whiteboard")]
    public partial class ResizeItemCommandWithSideEffects : global::Quire.Controls.Whiteboard.WhiteboardViewModel.ResizeItemCommand, global::Quire.Undo.IUndoableCommandWithSideEffects, global::Quire.Undo.IUndoableCommand
    {
        public ResizeItemCommandWithSideEffects(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Size oldSize, global::System.Windows.Size newSize, bool widthIsAuto, bool heightIsAuto)
            : base(whiteboard, item, oldSize, newSize, widthIsAuto, heightIsAuto, global::Quire.Controls.Core.DragMode.None)
        {
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+MoveItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class MoveItemsCommand : global::Quire.Controls.Whiteboard.WhiteboardViewModel.WhiteboardResizingCommand
    {
        private readonly global::System.Collections.Generic.List<global::Quire.Controls.Core.IWhiteboardItem> _list;
        private readonly global::System.Collections.Generic.Dictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> _initialLocations = new global::System.Collections.Generic.Dictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point>();
        private readonly global::System.Collections.Generic.Dictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point> _finalLocations = new global::System.Collections.Generic.Dictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Point>();
        private global::Quire.Controls.Core.IWhiteboardItem[] _selectedItems;
        private global::Quire.Controls.Core.IWhiteboardItem _activeItem;
        private readonly bool _allowGrowthDownOrRight;
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::Quire.Controls.Core.IWhiteboardItem item, bool allowGrowthDownOrRight, global::System.Windows.Point newLocation)
            : base(whiteboard, "Move Item")
        {
            _list = new List<IWhiteboardItem> { item };
            _initialLocations[item] = _viewModel.GetItemLocation(item);
            _finalLocations[item] = newLocation;
            _allowGrowthDownOrRight = allowGrowthDownOrRight;
        }
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Windows.Vector offset, bool allowGrowthDownOrRight, params global::Quire.Controls.Core.IWhiteboardItem[] elements)
            : this(whiteboard, offset, allowGrowthDownOrRight, (global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem>)elements)
        {
        }
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Windows.Vector offset, bool allowGrowthDownOrRight, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> elements)
            : base(whiteboard, "Move Items")
        {
            IEnumerable<IWhiteboardItem> collection = _viewModel.SortItemsByLocation(elements, offset.Y > 0.0 || (offset.Y == 0.0 && offset.X > 0.0));
            _list = new List<IWhiteboardItem>(collection);
            _list.Each((IWhiteboardItem item) =>
            {
                _initialLocations[item] = _viewModel.GetItemLocation(item);
                _finalLocations[item] = _initialLocations[item] + offset;
            });
            _allowGrowthDownOrRight = allowGrowthDownOrRight;
        }
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Vector> elements, bool allowGrowthDownOrRight)
            : base(whiteboard, "Move Items")
        {
            IEnumerable<IWhiteboardItem> keys = elements.Keys;
            Vector value = elements.ElementAt(0).Value;
            keys = _viewModel.SortItemsByLocation(keys, value.Y > 0.0 || (value.Y == 0.0 && value.X > 0.0));
            _list = new List<IWhiteboardItem>(keys);
            _list.Each((IWhiteboardItem item) =>
            {
                _initialLocations[item] = _viewModel.GetItemLocation(item);
                _finalLocations[item] = _initialLocations[item] + elements[item];
            });
            _allowGrowthDownOrRight = allowGrowthDownOrRight;
        }
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Point newLocation)
            : this(whiteboard, item, false, newLocation)
        {
        }
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Windows.Vector offset, params global::Quire.Controls.Core.IWhiteboardItem[] elements)
            : this(whiteboard, offset, false, elements)
        {
        }
        public MoveItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Windows.Vector> elements)
            : this(whiteboard, elements, false)
        {
        }
        protected override bool DoWithResize()
        {
            _selectedItems = _viewModel.SelectedItems.ToArray();
            _activeItem = _viewModel.ActiveItem;
            if (!CheckPotentialMove(_finalLocations))
            {
                return false;
            }

            bool moved = MoveItems();
            if (moved)
            {
                _viewModel.ResizeAndNotify(_list, _viewModel.ContentSize);
            }

            return moved;
        }

        private bool CheckPotentialMove(IDictionary<IWhiteboardItem, Point> newLocations)
        {
            if (!_allowGrowthDownOrRight)
            {
                return CheckMovementWithoutGrowth(newLocations);
            }

            return CheckMovementWithGrowth(newLocations);
        }

        private bool CheckMovementWithGrowth(IDictionary<IWhiteboardItem, Point> newLocations)
        {
            _viewModel.FireRequestMaximumSize();
            foreach (IWhiteboardItem item in _list)
            {
                bool ignoreHorizontal = _viewModel.GetItemIgnoreForHorizontalMovementAndWidth(item);
                if (!_viewModel.IsValidLocation(item, newLocations, true))
                {
                    return false;
                }

                Direction[] directions = new Direction[2] { Direction.Right, Direction.Down };
                double limit = _viewModel.ForbidGrowingMovementRight ? _viewModel.ContentWidth : _viewModel.MaximumSize.Width;
                if (!ignoreHorizontal)
                {
                    Point point = _viewModel.Grid.GetNearestIntersectionPointDisregardingBounds(newLocations[item], GridRestriction.Ignore, directions);
                    if (point.X + _viewModel.GetItemDesiredSize(item).Width > limit)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CheckMovementWithoutGrowth(IDictionary<IWhiteboardItem, Point> newLocations)
        {
            return _list.All((IWhiteboardItem item) => _viewModel.IsValidLocation(item, newLocations, false));
        }
        private bool MoveItems()
        {
            bool moved = false;
            foreach (IWhiteboardItem item in _list)
            {
                if (_allowGrowthDownOrRight)
                {
                    _viewModel.SetItemLocation(item, _finalLocations[item]);
                }
                else
                {
                    _viewModel.SetItemLocationToNearestGridIntersection(item, _finalLocations[item]);
                }

                if (_viewModel.GetItemLocation(item) != _initialLocations[item])
                {
                    moved = true;
                }
            }

            return moved;
        }

        public override void Undo()
        {
            UndoResize();
            _list.Reverse<IWhiteboardItem>().Each((IWhiteboardItem item) =>
            {
                _viewModel.SetItemLocation(item, _initialLocations[item]);
            });
            _viewModel.SelectItems(_selectedItems);
            if (_activeItem != null)
            {
                _viewModel.ActiveItem = _activeItem;
            }
            _viewModel.ResizeAndNotify(_list, _viewModel.ContentSize);
            _viewModel.UpdateSizeLimits(_viewModel.MaximumSize);
        }
        public override void Redo()
        {
            BackupTheSize();
            if (MoveItems())
            {
                _viewModel.ResizeAndNotify(_list, _viewModel.ContentSize);
            }

            _viewModel.SelectItems(_selectedItems);
        }

    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+MoveItemsCommandWithSideEffects, Ptc.Controls.Whiteboard")]
    public partial class MoveItemsCommandWithSideEffects : global::Quire.Controls.Whiteboard.WhiteboardViewModel.MoveItemsCommand, global::Quire.Undo.IUndoableCommandWithSideEffects, global::Quire.Undo.IUndoableCommand
    {
        public MoveItemsCommandWithSideEffects(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Windows.Vector offset, bool allowGrowthDownOrRight, params global::Quire.Controls.Core.IWhiteboardItem[] elements)
            : base(whiteboard, offset, allowGrowthDownOrRight, elements)
        {
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+PushItemsDownCommand, Ptc.Controls.Whiteboard")]
    public partial class PushItemsDownCommand : global::Quire.Controls.Whiteboard.WhiteboardViewModel.WhiteboardResizingCommand, global::Quire.Undo.IUndoableCommandWithSideEffects, global::Quire.Undo.IUndoableCommand
    {
        private int _numberOfLines;
        private readonly global::Quire.Controls.Whiteboard.WhiteboardViewModel.MoveItemsCommand _moveCommand;
        private global::System.Windows.Vector _offset;
        public PushItemsDownCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel viewModel, int numberOfLines, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemsToMove)
            : base(viewModel, "PushItemsDown")
        {
            _numberOfLines = numberOfLines;
            _offset = new Vector(0.0, (double)numberOfLines * _viewModel.Grid.GridCellSize.Height);
            if (itemsToMove.Count() > 0)
            {
                _moveCommand = new MoveItemsCommand(_viewModel, _offset, true, itemsToMove.ToArray());
            }
        }
        protected override bool DoWithResize()
        {
            Size size = _viewModel.ContentSize;
            _viewModel.ContentHeight += _offset.Y;
            if (_moveCommand != null)
            {
                _moveCommand.Execute();
            }

            ViewModel.FireChildCausedResize(new Size(_viewModel.ContentWidth, size.Height), _viewModel.ContentSize);
            return true;
        }

        public override void Undo()
        {
            if (_moveCommand != null)
            {
                _moveCommand.Undo();
            }

            Size size = _viewModel.ContentSize;
            UndoResize();
            ViewModel.FireChildCausedResize(size, _viewModel.ContentSize);
        }

        public override void Redo()
        {
            Do();
        }

    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+InsertSpaceCommand, Ptc.Controls.Whiteboard")]
    public partial class InsertSpaceCommand : global::Quire.Controls.Whiteboard.WhiteboardViewModel.WhiteboardResizingCommand
    {
        private readonly global::Quire.Controls.Whiteboard.WhiteboardViewModel.MoveItemsCommand _moveCommand;
        private global::System.Windows.Vector _offset;
        private readonly global::System.Windows.Point _insertionPoint;
        private readonly bool _pushInsertionPoint;
        public InsertSpaceCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, int numberOfLines, bool pushInsertionPoint, params global::Quire.Controls.Core.IWhiteboardItem[] checkVerticalOnlyItems)
            : this(whiteboard, numberOfLines, pushInsertionPoint, false, checkVerticalOnlyItems)
        {
        }
        public InsertSpaceCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, int numberOfLines, bool pushInsertionPoint, bool ignoreItemsOnCurrentLine, params global::Quire.Controls.Core.IWhiteboardItem[] checkVerticalOnlyItems)
            : base(whiteboard, "Insert Space")
        {
            _offset = new Vector(0.0, (double)numberOfLines * _viewModel.Grid.GridCellSize.Height);
            _insertionPoint = _viewModel.InsertionPoint;
            IEnumerable<IWhiteboardItem> source = _viewModel.WhiteboardItems.SkipWhile((IWhiteboardItem item) =>
            {
                Rect itemRect = ViewModel.GetItemRect(item);
                itemRect.Y -= item.VerticalAlignmentOffset;
                double y = itemRect.Y;
                Point insertionPoint = _insertionPoint;
                if (!y.IsLessThan(insertionPoint.Y))
                {
                    if (checkVerticalOnlyItems != null && !checkVerticalOnlyItems.Contains(item, null))
                    {
                        double y2 = itemRect.Y;
                        insertionPoint = _insertionPoint;
                        if (y2.IsEqualTo(insertionPoint.Y))
                        {
                            double x = itemRect.X;
                            insertionPoint = _insertionPoint;
                            return x.IsLessThan(insertionPoint.X);
                        }
                    }
                    return false;
                }
                return true;
            });
            if (ignoreItemsOnCurrentLine)
            {
                source = source.SkipWhile((IWhiteboardItem item) =>
                {
                    Rect itemRect = ViewModel.GetItemRect(item);
                    double value = itemRect.Top - item.VerticalAlignmentOffset;
                    Point insertionPoint = _insertionPoint;
                    return value.IsEqualTo(insertionPoint.Y);
                });
            }
            if (source.Count() > 0)
            {
                _moveCommand = new MoveItemsCommand(_viewModel, _offset, true, source.ToArray());
            }
            _pushInsertionPoint = pushInsertionPoint;
        }
        protected override bool DoWithResize()
        {
            _viewModel.FireRequestMaximumSize();
            _viewModel.ContentHeight += _offset.Y;
            if (_moveCommand != null)
            {
                _moveCommand.Execute();
            }

            if (_pushInsertionPoint)
            {
                _viewModel.InsertionPoint = new Point(_viewModel.InsertionPoint.X, _viewModel.InsertionPoint.Y + _offset.Y);
            }

            ViewModel.FireChildCausedResize(_oldSize, _viewModel.ContentSize);
            return true;
        }

        public override void Undo()
        {
            if (_moveCommand != null)
            {
                _moveCommand.Undo();
            }

            if (_pushInsertionPoint)
            {
                _viewModel.InsertionPoint = _insertionPoint;
            }

            Size size = _viewModel.ContentSize;
            UndoResize();
            ViewModel.FireChildCausedResize(size, _viewModel.ContentSize);
        }

        public override void Redo()
        {
            Do();
        }

    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+DeleteSpaceCommand, Ptc.Controls.Whiteboard")]
    public partial class DeleteSpaceCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        private readonly global::Quire.Direction _direction;
        private readonly global::Quire.Controls.Whiteboard.WhiteboardViewModel.MoveItemsCommand _moveCommand;
        private global::System.Windows.Vector _offset;
        private readonly double _oldHeight;
        private global::System.Windows.Point _insertionPoint;
        public DeleteSpaceCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, int numberOfLines, global::Quire.Direction direction)
            : base(whiteboard, "Delete Space")
        {
            _direction = direction;
            _oldHeight = _viewModel.ContentHeight;
            _insertionPoint = _viewModel.InsertionPoint;
            double height = _viewModel.Grid.GridCellSize.Height;
            DoubleComparer comparer = DoubleComparer.DefaultDoubleComparer;
            WhiteboardItemsCollection whiteboardItems = _viewModel.WhiteboardItems;
            IEnumerable<IWhiteboardItem> enumerable = ((_direction == Direction.Down) ? whiteboardItems.Where((IWhiteboardItem item) =>
            {
                DoubleComparer doubleComparer = comparer;
                Point itemLocation = _viewModel.GetItemLocation(item);
                return doubleComparer.Compare(itemLocation.Y, _insertionPoint.Y) == 1;
            }) : whiteboardItems.Where((IWhiteboardItem item) =>
            {
                DoubleComparer doubleComparer = comparer;
                Point itemLocation = _viewModel.GetItemLocation(item);
                return doubleComparer.Compare(itemLocation.Y, _insertionPoint.Y) >= 0;
            }));
            _offset = new Vector(0.0, -((double)numberOfLines * height));
            if (numberOfLines > 1)
            {
                _offset.Y = ((enumerable.Count() > 0) ? ((direction == Direction.Down) ? (_offset.Y = _viewModel.LimitOffsetBasedOnItemsToMove(enumerable, _offset.Y, direction, _viewModel.InsertionPoint)) : _offset.Y) : Math.Max(_offset.Y, LimitOffsetBasedOnMinimumHeight()));
                double num2 = ((_direction == Direction.Down) ? (_viewModel.ContentHeight - _insertionPoint.Y) : _insertionPoint.Y);
                _offset.Y = Math.Max(-num2, _offset.Y);
            }
            if (enumerable.Count() > 0 && _offset.Y != 0.0)
            {
                _moveCommand = new MoveItemsCommand(_viewModel, _offset, true, enumerable.ToArray());
            }
        }
        public override bool Do()
        {
            if (_insertionPoint.Y < _viewModel.Grid.GridCellSize.Height && _direction == Direction.Above)
            {
                return false;
            }

            _viewModel.UpdateSizeLimits(_viewModel.MaximumSize);
            if (_moveCommand != null)
            {
                _moveCommand.Execute();
            }

            if (_direction == Direction.Above)
            {
                _viewModel.InsertionPoint = new Point(_viewModel.InsertionPoint.X, _viewModel.InsertionPoint.Y + _offset.Y);
            }

            if (_moveCommand == null)
            {
                _viewModel.ContentHeight += _offset.Y;
            }

            return true;
        }

        public override void Undo()
        {
            _viewModel.ContentHeight = _oldHeight;
            if (_moveCommand != null)
            {
                _moveCommand.Undo();
            }

            if (_direction == Direction.Above)
            {
                _viewModel.InsertionPoint = _insertionPoint;
            }
        }

        public override void Redo()
        {
            Do();
        }

        private double LimitOffsetBasedOnMinimumHeight()
        {
            double gridHeight = _viewModel.Grid.GridCellSize.Height;
            return -(((_viewModel.ContentHeight - _viewModel.MinimumSize.Height) / gridHeight).Floor(double.NaN) * gridHeight);
        }

    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Whiteboard.WhiteboardViewModel+SeparateOverlappedItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class SeparateOverlappedItemsCommand : global::Quire.Controls.Whiteboard.Commands.WhiteboardCommand
    {
        private readonly global::Quire.Controls.Whiteboard.WhiteboardViewModel.MoveItemsCommand _moveItemsCommand;
        public SeparateOverlappedItemsCommand(global::Quire.Controls.Whiteboard.IWhiteboardViewModel whiteboard, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemsToSeparate, global::Quire.Dimension dimension, bool allowGrowthDownOrRight = false, global::Quire.Controls.Core.Whiteboard.IWhiteboardItemManager itemManager = default)
            : base(whiteboard, "SeparateOverlappedRegions")
        {
            IWhiteboardItemsSeparator whiteboardItemsSeparator = new WhiteboardItemsSeparator(itemsToSeparate, dimension, whiteboard.Grid, itemManager ?? WhiteboardItemMgr);
            if (!whiteboardItemsSeparator.CanSeparate())
            {
                return;
            }

            _moveItemsCommand = new MoveItemsCommand(_viewModel, whiteboardItemsSeparator.Separate(), allowGrowthDownOrRight);
        }
        public override bool Do()
        {
            if (_moveItemsCommand == null)
            {
                return false;
            }

            return _moveItemsCommand.Do();
        }

        public override void Undo()
        {
            _moveItemsCommand.Undo();
        }

        public override void Redo()
        {
            _moveItemsCommand.Redo();
        }

    }

}
