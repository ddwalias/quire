#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Whiteboard;

public partial class WhiteboardControl : ContentControl, IWhiteboardControl, IWhiteboardItemContainer, IHasRegionsWithConverterAnnotations, IMathFormatDeserializationProvider, IMaybeHasPointThatSnapsToPixel, IHasChildArrangeInfoProvider, IDisposable, ISearchableRegion, IImeClient
{
    public static int DeleteEmptySpaceDefaultLinesCount;

    public static readonly DependencyProperty DeleteIfEmptyProperty;

    public static readonly DependencyProperty DragDropManagerProperty;

    public static readonly DependencyProperty IsActiveProperty;

    public event Action<IWhiteboardItemContainer, IWhiteboardItem> RequestActivatePartContainingChild = delegate
    {
    };

    public IWhiteboardItemManager WhiteboardItemManager => WhiteboardViewModel.WhiteboardItemMgr;

    IContainerItemManager IWhiteboardItemContainer.ContainerItemManager => WhiteboardItemManager;

    IUIElementArrangeInfoProvider IHasChildArrangeInfoProvider.ChildArrangeInfoProvider => WhiteboardItemManager;

    public IEnumerable<IWhiteboardItem> RecursivelyEnumerateDescendantItemssBypassWhiteboard()
    {
        return ViewModel.RecursivelyEnumerateDescendantItemssBypassWhiteboard();
    }

    public bool CanSeparateOverlappedItems(Dimension dimension)
    {
        return ViewModel.CanSeparateOverlappedItems(dimension);
    }

    public void SeparateOverlappedItems(Dimension dimension)
    {
        ViewModel.SeparateOverlappedItems(dimension);
    }

    private static void HandleIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WhiteboardControl whiteboardControl)
        {
            bool flag = (bool)e.NewValue;
            whiteboardControl.SetIsSelected(flag);
            if (flag)
            {
                whiteboardControl.ActiveItem = null;
            }
        }
    }

    private void SetIsSelected(bool value)
    {
        ViewModel.IsSelected = value;
    }

    public static bool GetIsSelected(UIElement item)
    {
        if (item != null)
        {
            return WhiteboardDependencyProperties.GetIsSelected(item);
        }
        return false;
    }

    public static void SetIsSelected(UIElement item, bool value)
    {
        if (item != null)
        {
            WhiteboardDependencyProperties.SetIsSelected(item, value);
        }
    }

    public static bool GetIsActive(UIElement item)
    {
        if (item != null)
        {
            return (bool)((DependencyObject)item).GetValue(IsActiveProperty);
        }
        return false;
    }

    public static void SetIsActive(UIElement item, bool value)
    {
        if (item != null)
        {
            ((DependencyObject)item).SetValue(IsActiveProperty, (object)value);
        }
    }

    public event Action<IWhiteboardControl, Rect> RequestScrollTo = delegate
    {
    };

    public static event Action<Rect, WhiteboardControl> DragSelectBeyondBorder;

    public event Action<IWhiteboardControl, Size, Size> ChildTriggeredResize = delegate
    {
    };

    public event Action<IWhiteboardControl> RequestMaximumSize = delegate
    {
    };

    protected IWhiteboardViewModel ViewModel { get; set; }

    public event Action Activated;

    static WhiteboardControl()
    {
        DeleteEmptySpaceDefaultLinesCount = 1000;
        DeleteIfEmptyProperty = DependencyProperty.RegisterAttached("DeleteIfEmpty", typeof(bool), typeof(WhiteboardControl), new PropertyMetadata(false));
        DragDropManagerProperty = DependencyProperty.RegisterAttached("DragDropManger", typeof(FrameworkElement), typeof(IWhiteboardControl));
        DragSelectBeyondBorder = delegate
        {
        };
        WhiteboardDependencyProperties.IsSelectedProperty.OverrideMetadata(typeof(WhiteboardControl), new FrameworkPropertyMetadata(HandleIsSelectedPropertyChanged));
        AttachedProperties.PageElementXamlHelperProperty.OverrideMetadata(typeof(WhiteboardControl), new FrameworkPropertyMetadata(new WhiteboardElementXamlHelper()));
        AppServiceRegistry.AddService<IWhiteboardItemManager>(new WhiteboardItemManager());
        IsActiveProperty = WhiteboardDependencyProperties.IsActiveProperty.AddOwner(typeof(WhiteboardControl));
        DocumentAttachedProperties.IsInCopyProperty.OverrideMetadata(typeof(WhiteboardControl), new FrameworkPropertyMetadata(OnIsInCopyPropertyChanged));
        ChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(typeof(WhiteboardControl));
    }

    private static void OnIsInCopyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        WhiteboardControl whiteboardControl = d as WhiteboardControl;
        if (whiteboardControl == null)
        {
            return;
        }
        bool value = DocumentAttachedProperties.GetIsInCopyNonSerializable(whiteboardControl);
        if (value)
        {
            whiteboardControl.WhiteboardItems.Each((IWhiteboardItem item) =>
            {
                whiteboardControl.WhiteboardItemManager.SetItemIsInCopy(item, value);
            });
        }
        else
        {
            whiteboardControl.WhiteboardItems.Each((IWhiteboardItem item) =>
            {
                whiteboardControl.WhiteboardItemManager.ClearItemIsInCopy(item);
            });
        }
    }

    protected WhiteboardControl(IWhiteboardViewModel viewModel)
    {
        ImeHandler.SetIsInputMethodSuspended(this);
        ViewModel = viewModel;
        DataContext = ViewModel;
        SetupBindings();
        ViewModel.PropertyChanged += ViewModelPropertyChangedHandler;
        HookupCustomEventHandlers();
    }

    private void HookupCustomEventHandlers()
    {
        ViewModel.ChildCausedResize += ChildCausedResizeHandler;
        ViewModel.RequestMaximumSize += RequestMaximumSizeHandler;
        ViewModel.Activated += () =>
        {
            Activated?.Invoke();
        };
        ViewModel.RequestActivatePartContainingChild += (IWhiteboardItem item) =>
        {
            RequestActivatePartContainingChild(this, item);
        };
        ViewModel.RequestBringIntoView += BringIntoView;
        ViewModel.Changed += FireChanged;
        ViewModel.CopyWhiteboardItems += FireCopyWhiteboardItems;
        RequestScrollTo += HandleRequestScrollTo;
        AddHandler(CommonRoutedEvents.TraversedFocusEvent, new CustomRoutedEventHandler<FocusNavigationDirection>(OnTraversedFocusEvent), handledEventsToo: true);
    }

    private void BringIntoView(IEnumerable<IWhiteboardItem> items)
    {
        if (items.IsNullOrEmpty())
        {
            return;
        }
        Rect targetRectangle = items.Skip(1).Select(WhiteboardItemManager.GetItemRect).Aggregate(WhiteboardItemManager.GetItemRect(items.First()), Rect.Union);
        BringIntoView(targetRectangle);
    }

    private void FireCopyWhiteboardItems(IEnumerable<IWhiteboardItem> whiteboardItems)
    {
        RaiseEvent(new CustomRoutedEventArgs<IEnumerable<IWhiteboardItem>>(CommonRoutedEvents.CopyWhiteboardItemsEvent, whiteboardItems));
    }

    private void HandleRequestScrollTo(IWhiteboardControl control, Rect areaOfInterest)
    {
        IWhiteboardItem item = control as IWhiteboardItem;
        if (item == null)
        {
            return;
        }
        WhiteboardItemManager.BringIntoView(item, areaOfInterest);
    }

    public static readonly RoutedEvent ChangedEvent;

    private void FireChanged(object sender, EventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ChangedEvent));
    }

    private void ChildCausedResizeHandler(Size oldSize, Size newSize)
    {
        SaveResizeInformation();
        if (IsLoaded || SerializationHelperUtils.IsConvertedFromMathcad14(this))
        {
            ChildTriggeredResize(this, oldSize, newSize);
        }
    }

    private void RequestMaximumSizeHandler()
    {
        RequestMaximumSize(this);
    }

    private void OnTraversedFocusEvent(object sender, CustomRoutedEventArgs<FocusNavigationDirection> e)
    {
        if ((int)e.Parameter == 0)
        {
            EditingCommands.TabForward.Execute(null, this);
            return;
        }
        DebugTools.DevAssert((int)e.Parameter == 1, null);
        EditingCommands.TabBackward.Execute(null, this);
    }

    private void SetupBindings()
    {
        AttachedProperties.SetRegisterCommandBindings(this, ViewModel.CommandBindings);
        WhiteboardBehavior.SetUIEventsHandler(this, ViewModel);
        SetValue(WhiteboardDragger.IsDragSourceProperty, (object)true);
        SetValue(WhiteboardDragger.IsDropTargetProperty, (object)true);
    }

    public bool CopySelection()
    {
        return ViewModel.CopySelection();
    }

    public void CutSelection()
    {
        ViewModel.CutSelection();
    }

    public bool IsTabNavigatable => ViewModel.IsTabNavigatable;

    public void TabInto(FocusNavigationDirection dir)
    {
        ViewModel.TabInto(dir);
    }

    public void PasteItems(IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation)
    {
        ViewModel.Paste(pasteItemsAtLocation);
    }

    public void OnDragSelectBeyondBorder(Rect rect)
    {
        DragSelectBeyondBorder(rect, this);
    }

    protected virtual void ViewModelPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsFocused")
        {
            if (ViewModel.IsFocused)
            {
                Focus();
            }
        }
        else if (e.PropertyName == "InsertionPoint")
        {
            RequestScrollTo(this, this.GetInsertionPointRect());
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Point GridPosition
    {
        get
        {
            return ViewModel.GridPosition;
        }
        set
        {
            ViewModel.GridPosition = value;
        }
    }

    private const string InsertionPointPropertyName = "InsertionPoint";

    [DependsOn("WhiteboardItemsForXamlSerialization")]
    public Point InsertionPoint
    {
        get
        {
            return ViewModel.InsertionPoint;
        }
        set
        {
            ViewModel.InsertionPoint = value;
        }
    }

    public Point DefaultInsertionPoint
    {
        get
        {
            return ViewModel.DefaultInsertionPoint;
        }
    }

    [DependsOn("InsertionPoint")]
    public bool IsCrosshairVisible
    {
        get
        {
            return ViewModel.IsCrosshairVisible;
        }
        set
        {
            ViewModel.IsCrosshairVisible = value;
        }
    }

    [DependsOn("InsertionPoint")]
    public IWhiteboardItem ActiveItem
    {
        get
        {
            return ViewModel.ActiveItem;
        }
        set
        {
            ViewModel.ActiveItem = value;
        }
    }

    public IEnumerable<IWhiteboardItem> ActiveOrSelectedItems => ViewModel.ActiveOrSelectedItems;

    public IWhiteboardItem ActiveDescendant => ViewModel.ActiveDescendant;

    public IList<IWhiteboardItem> SelectedDescendants => ViewModel.SelectedDescendants;

    public WhiteboardItemsCollection WhiteboardItems => ViewModel.WhiteboardItems;

    private const string WhiteboardItemsForXamlSerializationPropertyName = "WhiteboardItemsForXamlSerialization";

    public virtual IWhiteboardItem[] WhiteboardItemsForXamlSerialization
    {
        get
        {
            return WhiteboardItems.ToArray();
        }
        set
        {
            if (DebugTools.DevAssert(WhiteboardItems.Count == 0, null).Failed)
            {
                WhiteboardItems.Clear();
            }
            WhiteboardItems.AddRange(value);
        }
    }

    public Size GridCellSize
    {
        get
        {
            return ViewModel.Grid.GridCellSize;
        }
        set
        {
            ViewModel.Grid.GridCellSize = value;
        }
    }

    public bool CanAddItems => ViewModel.CanAddItems;

    public INotifyCollectionRangeChanged<IWhiteboardItem> SelectedItems => ViewModel.SelectedItems;

    public Rect GetItemRect(IWhiteboardItem item)
    {
        return ViewModel.GetItemRect(item);
    }

    public Point GetItemLocation(IWhiteboardItem item)
    {
        return ViewModel.GetItemLocation(item);
    }

    public void SetItemLocation(IWhiteboardItem item, Point location)
    {
        ViewModel.SetItemLocationToNearestGridIntersection(item, location);
    }

    public Point GetItemGridPosition(IWhiteboardItem item)
    {
        return ViewModel.GetItemGridPosition(item);
    }

    public void ActivateWhiteboard(bool isActive)
    {
        if (isActive)
        {
            WhiteboardViewModel.WhiteboardItemMgr.ExecuteOnLoad(this, () =>
            {
                Focus();
            });
        }
        ViewModel.Activate(isActive);
    }

    public void SetInsertionPointWhenLoaded(Point insertionPoint)
    {
        WhiteboardViewModel.WhiteboardItemMgr.ExecuteOnLoad(this, () =>
        {
            InsertionPoint = insertionPoint;
        });
    }

    public void SetItemGridPosition(IWhiteboardItem item, Point location)
    {
        ViewModel.SetItemGridPosition(item, location);
    }

    public void AddItem(IWhiteboardItem item)
    {
        if (ActiveItem is IWhiteboardItemContainer whiteboardItemContainer)
        {
            whiteboardItemContainer.AddItem(item);
        }
        else
        {
            ViewModel.AddItemAndActivate(item);
        }
    }

    public void AddItems(Dictionary<IWhiteboardItem, Point> addItemsAtLocation)
    {
        ViewModel.AddItems(addItemsAtLocation);
    }

    public void RemoveItem(IWhiteboardItem item)
    {
        ViewModel.RemoveWhiteboardItem(item);
    }

    public void AddItemAtLocation(IWhiteboardItem item, Point location, bool forceAlignment = false)
    {
        ViewModel.AddItemAtLocation(item, location);
    }

    public void ToggleItemSelection(IWhiteboardItem item, bool includeActiveItem)
    {
        ViewModel.ToggleItemSelection(item, includeActiveItem);
    }

    public void SelectContiguous(IWhiteboardItem item)
    {
        ViewModel.SelectContiguous(item);
    }

    public void ClearSelection()
    {
        ViewModel.ClearSelectedItems();
    }

    public void SelectItems(IEnumerable<IWhiteboardItem> itemsToSelect)
    {
        ViewModel.SelectItems(itemsToSelect);
    }

    public void UnSelectItems(IEnumerable<IWhiteboardItem> itemsToRemove)
    {
        ViewModel.UnSelectItems(itemsToRemove);
    }

    public void SelectItem(IWhiteboardItem item)
    {
        ViewModel.SelectItem(item);
    }

    public void UnSelectItem(IWhiteboardItem item)
    {
        ViewModel.UnSelectItem(item);
    }

    public void SaveVisualState()
    {
        ViewModel.SaveState();
    }

    public void RestoreVisualState()
    {
        ViewModel.RestoreSavedState();
    }

    public void ClearVisualState()
    {
        ViewModel.ClearState();
    }

    public void SaveResizeInformation()
    {
        ViewModel.SaveResizeInformation();
    }

    public void RestoreResizeInformation()
    {
        ViewModel.RestoreResizeInformation();
    }

    public Point InsertionPointForResize
    {
        get
        {
            return ViewModel.InsertionPointForResize;
        }
    }

    public bool ForbidGrowingMovementRight
    {
        set
        {
            ViewModel.ForbidGrowingMovementRight = value;
        }
    }

    public virtual void GridCellSizeChangedHandler(Size oldSize, Size newSize)
    {
        AttachedProperties.SuppressBringIntoViewIfNeededAndExecute(this, () =>
        {
            ViewModel.HandleGridCellSizeChanged(oldSize, newSize);
        }, suppressBringToView: true);
    }

    public Vector? NavigateInto(Direction direction, Point point)
    {
        return ViewModel.NavigateInto(direction, point);
    }

    public void UpdateSizeLimits(Size maximumSize)
    {
        ViewModel.UpdateSizeLimits(maximumSize);
    }

    public Point GetNavigationStartLocation(Direction direction)
    {
        return ViewModel.GetNavigationStartLocation(direction);
    }

    public void SelectAllItems()
    {
        ViewModel.SelectAllItems();
    }

    public Point GetDescendantInsertionPoint()
    {
        return ViewModel.GetDescendantInsertionPoint();
    }

    public bool FindOncePrivate(string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        return ViewModel.FindOncePrivate(text, startFromCaret, direction, options, subscriptPositions);
    }

    public bool HighlightAll(string text, FindOptions options)
    {
        throw new NotImplementedException();
    }

    public virtual void CanInsertVerticalSpace(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ViewModel.CanInsertVerticalSpace;
    }

    public virtual void CanDeleteVerticalSpaceAbove(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ViewModel.CanDeleteVerticalSpaceAbove;
    }

    public virtual void CanDeleteVerticalSpaceBelow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ViewModel.CanDeleteVerticalSpaceBelow;
    }

    public virtual void InsertVerticalSpace(object sender, ExecutedRoutedEventArgs e)
    {
        int numberOfLines = (e.Parameter as int?) ?? 1;
        ViewModel.InsertVerticalSpace(numberOfLines, pushInsertionPoint: false, ignoreItemsOnCurrentLine: false);
    }

    public virtual void DeleteVerticalSpaceAbove(object sender, ExecutedRoutedEventArgs e)
    {
        int numberOfLines = (e.Parameter as int?) ?? 1;
        ViewModel.DeleteVerticalSpaceAbove(numberOfLines);
    }

    public virtual void DeleteVerticalSpaceBelow(object sender, ExecutedRoutedEventArgs e)
    {
        int numberOfLines = (e.Parameter as int?) ?? DeleteEmptySpaceDefaultLinesCount;
        ViewModel.DeleteVerticalSpaceBelow(numberOfLines);
    }

    public Point GetImeWindowPosition()
    {
        return InsertionPoint;
    }

    public virtual void Dispose()
    {
        ViewModel.Dispose();
    }

    public bool HasRegionsWithConverterAnnotations => this.HasRegionsWithConverterAnnotations();

    public IEnumerable<UIElement> RegionsWithConverterAnnotations => this.RegionsWithConverterAnnotations();

    public StaticMathFormat TargetMathFormat => new StaticMathFormat(new MathFormatDobjController(this));

    public IDictionary<IdLabels, ExtendedFont> TargetLabeledIdFormat => (IDictionary<IdLabels, ExtendedFont>)GetValue(AttachedProperties.LabeledIdFormatProperty);

    private ISnapToPixelHelper _snappingHelper;

    public ISnapToPixelHelper SnappingHelper => _snappingHelper ?? (_snappingHelper = SnapToPixelWhiteboardHelper.Create(this));
}
