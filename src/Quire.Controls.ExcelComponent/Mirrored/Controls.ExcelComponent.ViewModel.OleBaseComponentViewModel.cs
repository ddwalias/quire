using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Whiteboard;
using Quire.Core;
using Quire.Undo;

using System.Windows.Input;
using Quire.Calculation.Core;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.Mathboard;
using Quire.Wpf;
using Quire.Calculation;
using Quire;
using System.Linq;
using System.Windows.Documents;
using Quire.Controls.Core.Whiteboard;
namespace Quire.Controls.ExcelComponent.ViewModel;

public abstract partial class OleBaseComponentViewModel
{
    public WhiteboardItemsCollection InputItems { get; private set; }

    public WhiteboardItemsCollection OutputItems { get; private set; }

    internal double SectionsMinimumHeight { get; set; }

    private bool IsInternalResizing { get; set; }

    protected internal static IOleBaseComponentItemManager OleComponentItemMgr => global::Quire.Controls.ExcelComponent.Model.OleBaseComponentModel.OleComponentItemMgr;

    public bool ShouldAlignOleContainerItemWithGridLines => OutputSectionExists;

    protected override Size StrictMinimumSize => _strictMinimumSize;

    public virtual void EndInit()
    {
    }

    protected virtual void OnLoadOleContainerItem()
    {
    }

    private void OnResizeOleContainerItem()
    {
        SetRestrictedArea();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.ViewModel.OleBaseComponentViewModel+ToggleSectionVisibilityCommand, Ptc.Controls.ExcelComponent")]
    public partial class ToggleSectionVisibilityCommand : IUndoableCommand
    {
        private readonly OleBaseComponentViewModel _oleBaseComponentViewModel;
        private readonly Action<bool> _setVisibility;
        private readonly bool _originalVisibility;
        private readonly string _name;

        public ToggleSectionVisibilityCommand(OleBaseComponentViewModel oleBaseComponentViewModel, bool isInputSection)
        {
            _oleBaseComponentViewModel = oleBaseComponentViewModel;
            _name = string.Format("Toggle {0} Section State", isInputSection ? "Input" : "Output");
            _originalVisibility = isInputSection ? _oleBaseComponentViewModel.IsInputSectionVisible : _oleBaseComponentViewModel.IsOutputSectionVisible;
            _setVisibility = isInputSection
                ? visibility => _oleBaseComponentViewModel.IsInputSectionVisible = visibility
                : visibility => _oleBaseComponentViewModel.IsOutputSectionVisible = visibility;
        }

        public string Name => _name;

        public bool Do()
        {
            _setVisibility(!_originalVisibility);
            return true;
        }

        public void Undo()
        {
            _setVisibility(_originalVisibility);
        }

        public void Redo()
        {
            Do();
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.ViewModel.OleBaseComponentViewModel+PasteOleComponentItemsCommand, Ptc.Controls.ExcelComponent")]
    public partial class PasteOleComponentItemsCommand : PasteMathboardItemsCommand
    {
        public PasteOleComponentItemsCommand(IOleBaseComponentViewModel viewModel, string commandName, IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation)
            : base(viewModel, commandName, pasteItemsAtLocation)
        {
        }

        private OleBaseComponentViewModel OleBaseComponentViewModel => _viewModel as OleBaseComponentViewModel;

        protected override bool DoWithResize()
        {
            double inputsHeight;
            Point insertionPoint = OleBaseComponentViewModel.InsertionPoint;
            if (insertionPoint.Y < OleBaseComponentViewModel.OleContainerItemTop)
            {
                inputsHeight = OleBaseComponentViewModel.InputSectionActualHeight;
                _itemsAtLocation.Values.Each(itemLocation =>
                {
                    inputsHeight = Math.Max(inputsHeight, itemLocation.Y + 1 + OleBaseComponentViewModel.Grid.GridCellSize.Height / 2);
                });
                if (inputsHeight > OleBaseComponentViewModel.InputSectionActualHeight)
                {
                    OleBaseComponentViewModel.OleContainerItemTop = inputsHeight;
                    inputsHeight = OleBaseComponentViewModel.OleContainerItemTop;
                    double inputsHeightDelta = inputsHeight - OleBaseComponentViewModel.InputSectionActualHeight;
                    OleBaseComponentViewModel.InputSectionActualHeight = inputsHeight;
                    Size newSize = new Size(OleBaseComponentViewModel.ContentWidth, OleBaseComponentViewModel.ContentHeight + inputsHeightDelta);
                    OleBaseComponentViewModel.FireChildCausedResize(OleBaseComponentViewModel.ContentSize, newSize);
                    CommandBuffer.ExecuteAndAttach(new WhiteboardViewModel.MoveItemsCommand(ViewModel, new Vector(0.0, inputsHeightDelta), true, OleBaseComponentViewModel.OutputItems));
                }
            }
            return base.DoWithResize();
        }
    }

    public EngineError Error => _model.Error;

    public IOleContainerItem OleContainerItem => _model.OleContainerItem;

    private IOleContainerItemViewModel OleContainerItemViewModel
    {
        get
        {
            if (!DebugTools.AssertNotNull(OleContainerItem, null).Failed)
            {
                return OleContainerItem.ViewModel;
            }
            return null;
        }
    }

    internal double OleContainerItemTop
    {
        get
        {
            if (OleContainerItem != null)
            {
                Point itemOleComponentLocation = OleComponentItemMgr.GetItemOleComponentLocation(OleContainerItem);
                return itemOleComponentLocation.Y;
            }
            return double.NaN;
        }
        set
        {
            if (OleContainerItem != null)
            {
                ClearRestrictedArea();
                Point nearestIntersectionPointDisregardingBounds = _alignmentGrid.GetNearestIntersectionPointDisregardingBounds(new Point(0.0, value), GridRestriction.Assert, Direction.Down);
                OleComponentItemMgr.SetItemOleComponentLocation(OleContainerItem, nearestIntersectionPointDisregardingBounds);
                base.SetItemLocation(OleContainerItem, default);
                SetRestrictedArea();
            }
        }
    }

    internal Size OleContainerItemSize
    {
        get
        {
            if (OleContainerItem != null)
            {
                return OleComponentItemMgr.GetSize(OleContainerItem);
            }
            return Size.Empty;
        }
    }

    protected double OleContainerItemHeight
    {
        get
        {
            Size oleContainerItemSize = OleContainerItemSize;
            return oleContainerItemSize.Height;
        }
    }

    protected double OleContainerItemWidth
    {
        get
        {
            Size oleContainerItemSize = OleContainerItemSize;
            return oleContainerItemSize.Width;
        }
    }

    public bool IsInputSectionVisible
    {
        get
        {
            return _isInputSectionVisible;
        }
        set
        {
            SetInputSectionVisibilityState(value);
        }
    }

    private double InputSectionActualTop
    {
        get
        {
            if (!IsInputSectionVisible)
            {
                return 0.0;
            }
            return InputSectionIndicator.Top;
        }
    }

    public double InputSectionActualHeight
    {
        get
        {
            if (!IsInputSectionVisible)
            {
                return 0.0;
            }
            return InputSectionIndicator.Height;
        }
        set
        {
            if (IsInputSectionVisible || InputSectionIndicator.Height == 0.0)
            {
                InputSectionIndicator.Height = value;
            }
        }
    }

    public bool IsOutputSectionVisible
    {
        get
        {
            if (OutputSectionExists)
            {
                return _isOutputSectionVisible;
            }
            return false;
        }
        set
        {
            SetOutputSectionVisibilityState(value);
        }
    }

    private double OutputSectionActualTop => InputSectionActualHeight + OleContainerItemHeight;

    public double OutputSectionActualHeight
    {
        get
        {
            if (!IsOutputSectionVisible)
            {
                return 0.0;
            }
            return OutputSectionIndicator.Height;
        }
        set
        {
            if (IsOutputSectionVisible || OutputSectionIndicator.Height == 0.0)
            {
                OutputSectionIndicator.Height = value;
            }
        }
    }

    public SectionIndicatorInfo InputSectionIndicator
    {
        get
        {
            return _sectionIndicators[0];
        }
        set
        {
            _sectionIndicators[0] = value;
        }
    }

    public SectionIndicatorInfo OutputSectionIndicator
    {
        get
        {
            return _sectionIndicators[1];
        }
        set
        {
            _sectionIndicators[1] = value;
        }
    }

    public override bool IsCrosshairVisible
    {
        get
        {
            if (base.IsCrosshairVisible)
            {
                if (!IsInputSectionVisible)
                {
                    return IsOutputSectionVisible;
                }
                return true;
            }
            return false;
        }
        set
        {
            base.IsCrosshairVisible = value && (IsInputSectionVisible || IsOutputSectionVisible);
        }
    }

    public override bool CanAddItems
    {
        get
        {
            if (base.CanAddItems)
            {
                return IsCrosshairVisible;
            }
            return false;
        }
    }

    protected double DefaultSectionsMinimumHeight
    {
        get
        {
            Size gridCellSize = Grid.GridCellSize;
            return gridCellSize.Height * 3.0;
        }
    }

    protected Size OleContainerItemInitialSize
    {
        get
        {
            Size result = OleContainerItemViewModel.EstablishInitialSize();
            result.Height = AlignHeightWithGridLines(result.Height, isOleContainerItem: true);
            return result;
        }
    }

    internal bool IsResizing
    {
        get
        {
            return OleContainerItem.IsResizing;
        }
        set
        {
            OleContainerItem.IsResizing = value;
            if (value)
            {
                DebugTools.DevAssert(_previousResizeData == null, null);
                return;
            }
            AdjustOleContainerItemSize(OleContainerItemSize);
            ReconcileOutputItemsLocations();
            MinimumSize = FindMinimumSize();
            _previousResizeData = null;
        }
    }

    public bool CanInsertOutputExpression
    {
        get
        {
            bool result = false;
            if (IsCrosshairVisible)
            {
                result = new Rect(0.0, OutputSectionActualTop, OleContainerItemWidth, OutputSectionActualHeight).ContainsAccountingForMeasurementError(InsertionPoint);
            }
            return result;
        }
    }

    public bool CanInsertInputExpression
    {
        get
        {
            bool result = false;
            if (IsCrosshairVisible)
            {
                result = new Rect(0.0, InputSectionActualTop, OleContainerItemWidth, InputSectionActualHeight).ContainsAccountingForMeasurementError(InsertionPoint);
            }
            return result;
        }
    }

    private void ClearRestrictedArea()
    {
        if (OleContainerItem != null)
        {
            ((IRestrictedAreaAlignmentGrid)_alignmentGrid).RestrictedAreas.Remove(OleContainerItem);
        }
    }

    private void SetRestrictedArea()
    {
        if (OleContainerItem != null)
        {
            Pair<double, double> value = new Pair<double, double>(InputSectionActualHeight, InputSectionActualHeight + OleContainerItemHeight);
            Dictionary<object, Pair<double, double>> restrictedAreas = ((IRestrictedAreaAlignmentGrid)_alignmentGrid).RestrictedAreas;
            if (restrictedAreas.ContainsKey(OleContainerItem))
            {
                restrictedAreas[OleContainerItem] = value;
            }
            else
            {
                restrictedAreas.Add(OleContainerItem, value);
            }
        }
    }

    protected void SetInputSectionVisibilityState(bool bVisible, bool bMoveInsertionPoint = true)
    {
        if (IsInputSectionVisible != bVisible)
        {
            _isInputSectionVisible = bVisible;
            OnPropertyChanged("IsInputSectionVisible");
            DeselectItems();
            UpdateMinimumHeight();
            if (IsInputSectionVisible)
            {
                ContentHeight += InputSectionIndicator.Height;
            }
            else
            {
                ContentHeight -= InputSectionIndicator.Height;
            }
            SetRestrictedArea();
            if ((OleContainerItem != null && WhiteboardViewModel.WhiteboardItemMgr.IsItemLoaded(OleContainerItem)) & bMoveInsertionPoint)
            {
                if (IsInputSectionVisible)
                {
                    if (IsOutputSectionVisible)
                    {
                        Point insertionPoint = InsertionPoint;
                        insertionPoint.Offset(0.0, InputSectionActualHeight - _alignmentGrid.GridCellSize.Height);
                        InsertionPoint = _alignmentGrid.GetNearestIntersectionInDirection(insertionPoint, Direction.Down);
                    }
                    else
                    {
                        InsertionPoint = _alignmentGrid.GetNearestIntersectionInDirection(new Point(_alignmentGrid.GridCellSize.Width, InputSectionActualTop), Direction.Down);
                    }
                }
                else if (IsOutputSectionVisible)
                {
                    Rect val = new Rect(0.0, OutputSectionIndicator.Top, OleContainerItemWidth, OutputSectionIndicator.Height);
                    if (!val.Contains(InsertionPoint))
                    {
                        InsertionPoint = _alignmentGrid.GetNearestIntersectionInDirection(new Point(_alignmentGrid.GridCellSize.Width, OutputSectionActualTop), Direction.Down);
                    }
                    else
                    {
                        InsertionPoint = new Point(InsertionPoint.X, InsertionPoint.Y - InputSectionIndicator.Height);
                    }
                }
                else
                {
                    IsCrosshairVisible = false;
                }
            }
            IndicateHiddenErrors();
        }
    }

    protected void SetOutputSectionVisibilityState(bool bVisible, bool bMoveInsertionPoint = true)
    {
        if (IsOutputSectionVisible != bVisible)
        {
            _isOutputSectionVisible = bVisible;
            OnPropertyChanged("IsOutputSectionVisible");
            UpdateMinimumHeight();
            DeselectItems();
            bool flag = (OleContainerItem != null && WhiteboardViewModel.WhiteboardItemMgr.IsItemLoaded(OleContainerItem)) & bMoveInsertionPoint;
            if (IsOutputSectionVisible)
            {
                ContentHeight += OutputSectionIndicator.Height;
                if (flag && !IsInputSectionVisible)
                {
                    InsertionPoint = _alignmentGrid.GetNearestIntersectionInDirection(new Point(_alignmentGrid.GridCellSize.Width, OutputSectionActualTop), Direction.Down);
                }
            }
            else
            {
                if (flag)
                {
                    if (IsInputSectionVisible)
                    {
                        Rect val = new Rect(0.0, InputSectionIndicator.Top, OleContainerItemWidth, InputSectionIndicator.Height);
                        if (!val.Contains(InsertionPoint))
                        {
                            InsertionPoint = _alignmentGrid.GetNearestIntersectionInDirection(new Point(_alignmentGrid.GridCellSize.Width, InputSectionActualTop), Direction.Down);
                        }
                    }
                    else
                    {
                        IsCrosshairVisible = false;
                    }
                }
                ContentHeight -= OutputSectionIndicator.Height;
            }
            IndicateHiddenErrors();
        }
    }

    private void UpdateSectionHeights()
    {
        SetRestrictedArea();
        SetIndicatorsDimensions();
        OutputSectionActualHeight = ContentSize.Height - OutputSectionActualTop;
    }

    private bool ExpandSectionIfNeeded(IWhiteboardItem item)
    {
        return ExpandSectionIfNeeded(new IWhiteboardItem[1] { item });
    }

    private bool ExpandSectionIfNeeded(IEnumerable<IWhiteboardItem> items)
    {
        bool result = false;
        if (!IsInputSectionVisible && InputItems.Any(items.Contains<IWhiteboardItem>))
        {
            SetInputSectionVisibilityState(bVisible: true, bMoveInsertionPoint: false);
            result = true;
        }
        if (!IsOutputSectionVisible && OutputItems.Any(items.Contains<IWhiteboardItem>))
        {
            SetOutputSectionVisibilityState(bVisible: true, bMoveInsertionPoint: false);
            result = true;
        }
        return result;
    }

    private void DeselectItems()
    {
        if (HasSelectedItems)
        {
            List<IWhiteboardItem> list = new List<IWhiteboardItem>();
            IEnumerable<IWhiteboardItem> enumerable = InputItems.Intersect(SelectedItems);
            IEnumerable<IWhiteboardItem> enumerable2 = OutputItems.Intersect(SelectedItems);
            bool flag = !enumerable.IsNullOrEmpty();
            bool flag2 = !enumerable2.IsNullOrEmpty();
            bool flag3 = flag2 & flag;
            if ((!IsInputSectionVisible & flag3) || (!IsOutputSectionVisible & flag3))
            {
                list.AddRange(SelectedItems);
            }
            else
            {
                if (!IsInputSectionVisible & flag)
                {
                    list.AddRange(enumerable);
                }
                if (!IsOutputSectionVisible & flag2)
                {
                    list.AddRange(enumerable2);
                }
            }
            UnSelectItems(list);
        }
        if (ActiveItem != null && ((!IsInputSectionVisible && InputItems.Contains(ActiveItem)) || (!IsOutputSectionVisible && OutputItems.Contains(ActiveItem))))
        {
            ActiveItem = null;
        }
    }

    protected override void SetupCommandBindings()
    {
        base.SetupCommandBindings();
        CreateCommandBinding(ToggleInputSectionVisibility, ToggleInputSectionVisibilityCommand.Execute);
        CreateCommandBinding(ToggleOutputSectionVisibility, ToggleOutputSectionVisibilityCommand.Execute);
    }

    protected override void HandleChildItemGotFocus(IWhiteboardItem item)
    {
        ExpandSectionIfNeeded(item);
        base.HandleChildItemGotFocus(item);
    }

    protected override SeparateOverlappedItemsCommand CreateSeparateOverlappedItemsCommand(IEnumerable<IWhiteboardItem> itemsToSeparate, Dimension dimension)
    {
        return new SeparateOverlappedItemsCommand(this, itemsToSeparate, dimension, allowGrowthDownOrRight: true, OleComponentItemMgr);
    }

    protected override bool IsValidInsertionPoint(Point pt)
    {
        return !((IRestrictedAreaAlignmentGrid)_alignmentGrid).IsWithinRestrictedArea(pt);
    }

    protected override Point GetNextIntersectionForMove(Point currentLocation, Direction direction)
    {
        return ((IRestrictedAreaAlignmentGrid)_alignmentGrid).GetNextIntersectionIgnoringRestrictedAreas(currentLocation, direction);
    }

    public override void FireChildCausedResize(Size oldSize, Size newSize)
    {
        base.FireChildCausedResize(oldSize, newSize);
        UpdateSectionHeights();
    }

    protected override void HandleItemRemoved(IWhiteboardItem item)
    {
        if (InputItems.Contains(item))
        {
            InputItems.Remove(item);
        }
        else if (OutputItems.Contains(item))
        {
            OutputItems.Remove(item);
        }
        base.HandleItemRemoved(item);
    }

    protected override bool CanTraverse(FocusNavigationDirection direction)
    {
        bool result = false;
        IWhiteboardItem whiteboardItem = PredictFocus(direction);
        if (whiteboardItem != null)
        {
            result = !(whiteboardItem is FrameworkElement frameworkElement) || frameworkElement.IsVisible;
        }
        return result;
    }

    public override Point GetItemLocation(IWhiteboardItem item)
    {
        return OleComponentItemMgr.GetItemOleComponentLocation(item);
    }

    public override void SetItemLocation(IWhiteboardItem item, Point location)
    {
        Point location2 = location;
        if (item == OleContainerItem)
        {
            OleComponentItemMgr.SetItemOleComponentLocation(item, location);
            SetRestrictedArea();
            return;
        }
        if (OutputItems.Contains(item))
        {
            location2.Offset(0.0, -(InputSectionIndicator.Height + OleContainerItemHeight));
        }
        if (location2.Y.IsGreaterThanOrEqual(0.0) && location2.X.IsGreaterThanOrEqual(0.0))
        {
            OleComponentItemMgr.SetItemOleComponentLocation(item, location);
            base.SetItemLocation(item, location2);
        }
        _model.UpdateCustomErrorBasedOnPlacement(item);
    }

    public override void AddItemAtLocation(IWhiteboardItem item, Point location)
    {
        if (!IsInputSectionVisible)
        {
            location.Offset(0.0, InputSectionIndicator.Height);
        }
        base.AddItemAtLocation(item, location);
    }

    private void AddItemToAppropriateSection(IWhiteboardItem item)
    {
        Point itemOleComponentLocation = OleComponentItemMgr.GetItemOleComponentLocation(item);
        Point itemOleComponentLocation2 = OleComponentItemMgr.GetItemOleComponentLocation(OleContainerItem);
        double num = itemOleComponentLocation2.Y + OleContainerItemHeight;
        if (itemOleComponentLocation.Y < num)
        {
            if (OutputItems.Contains(item))
            {
                OutputItems.Remove(item);
            }
            WhiteboardViewModel.WhiteboardItemMgr.SetItemLocation(item, itemOleComponentLocation);
            if (!InputItems.Contains(item))
            {
                InputItems.Add(item);
            }
        }
        else
        {
            Point point = itemOleComponentLocation;
            point.Y -= num;
            if (InputItems.Contains(item))
            {
                InputItems.Remove(item);
            }
            WhiteboardViewModel.WhiteboardItemMgr.SetItemLocation(item, point);
            if (!OutputItems.Contains(item))
            {
                OutputItems.Add(item);
            }
        }
        IndicateHiddenErrors();
    }

    public override IEnumerable<IWhiteboardItem> RecursivelyEnumerateDescendantItemssBypassWhiteboard()
    {
        return WhiteboardViewModel.RecursivelyEnumerateDescendantsIgnoringTypes(WhiteboardItems, typeof(IOleContainerItem));
    }

    protected override bool ItemsOverlapped(IEnumerable<IWhiteboardItem> items)
    {
        return OleComponentItemMgr.ItemsOverlapped(items);
    }

    public override Rect GetItemRect(IWhiteboardItem item)
    {
        Rect itemRect = OleComponentItemMgr.GetItemRect(item);
        if (!IsInputSectionVisible)
        {
            itemRect.Offset(0.0, -InputSectionIndicator.Height);
        }
        return itemRect;
    }

    public override IEnumerable<IWhiteboardItem> SortItemsByLocation(IEnumerable<IWhiteboardItem> items, bool descending)
    {
        return OleComponentItemMgr.SortItemsByLocation(items, descending);
    }

    public override bool HandleMouseDown(IWhiteboardControl sender, Point position, MouseButton button)
    {
        if (button == MouseButton.Right && sender is IOleBaseComponentControl)
        {
            Rect itemRect = WhiteboardViewModel.WhiteboardItemMgr.GetItemRect(OleContainerItem);
            itemRect.Offset(0.0, OleContainerItemTop);
            if (itemRect.Contains(position))
            {
                return true;
            }
        }
        return base.HandleMouseDown(sender, position, button);
    }

    public override void RestoreSavedState()
    {
        if (IsInputSectionVisible || IsOutputSectionVisible)
        {
            base.RestoreSavedState();
        }
    }

    private Size GetOleContainerItemMinSize(bool useDefault = false)
    {
        Size result = useDefault ? OleContainerItemViewModel.DefaultMinSize : OleContainerItemViewModel.MinSize;
        result.Height = AlignHeightWithGridLines(result.Height, isOleContainerItem: true);
        return result;
    }

    protected void ForceOleComponentResizeDueToOleContainerItemResize(bool setExplicitMinHeight)
    {
        bool isInternalResizing = IsInternalResizing;
        IsInternalResizing = true;
        Size oleContainerItemMinSize = GetOleContainerItemMinSize();
        Size oleContainerItemSize = OleContainerItemSize;
        double num = Math.Max(oleContainerItemMinSize.Width, oleContainerItemSize.Width);
        double num2 = setExplicitMinHeight ? oleContainerItemMinSize.Height : Math.Max(oleContainerItemMinSize.Height, oleContainerItemSize.Height);
        Size newSize = new Size(num, num2);
        SetItemSize(OleContainerItem, newSize);
        OnResizeOleContainerItem();
        IsInternalResizing = isInternalResizing;
    }

    private double GetMinimumContentHeight(double containerHeight)
    {
        double num = 0.0;
        if (IsInputSectionVisible)
        {
            num = SectionsMinimumHeight;
        }
        if (IsOutputSectionVisible)
        {
            num += SectionsMinimumHeight;
        }
        return num + containerHeight;
    }

    private double GetMinimumContentHeight()
    {
        if (DebugTools.AssertNotNull(OleContainerItemViewModel, null).Passed)
        {
            return GetMinimumContentHeight(GetOleContainerItemMinSize().Height);
        }
        return 0.0;
    }

    private void UpdateMinimumHeight()
    {
        _strictMinimumSize.Height = GetMinimumContentHeight();
        MinimumHeight = _strictMinimumSize.Height;
    }

    protected Size FindMinimumSize(bool useDefault)
    {
        if (DebugTools.AssertNotNull(OleContainerItemViewModel, null).Passed)
        {
            return _strictMinimumSize = FindMinimumSize(GetOleContainerItemMinSize(useDefault));
        }
        return default;
    }

    private Size AdjustOleContainerItemSize(Size oleContainerItemSize)
    {
        double num = AlignHeightWithGridLines(oleContainerItemSize.Height, isOleContainerItem: true);
        double num2 = oleContainerItemSize.Height - num;
        ContentSize = new Size(ContentSize.Width, ContentSize.Height - num2);
        oleContainerItemSize.Height = num;
        OleComponentItemMgr.SetSize(OleContainerItem, oleContainerItemSize);
        OnResizeOleContainerItem();
        return oleContainerItemSize;
    }

    public override void UpdateSizeLimits(Size maximumSize)
    {
        SectionsMinimumHeight = DefaultSectionsMinimumHeight;
        base.UpdateSizeLimits(MaximumSize);
    }

    public override Vector? NavigateInto(Direction direction, Point point)
    {
        Vector? val = null;
        IOleContainerItem oleContainerItem = GetChildItemFromLocation(point) as IOleContainerItem;
        switch (direction)
        {
            case Direction.Left:
                if (oleContainerItem != null)
                {
                    val = new Vector(-OleContainerItemWidth - _alignmentGrid.GridCellSize.Width, 0.0);
                }
                break;
            case Direction.Right:
                if (oleContainerItem != null)
                {
                    val = new Vector(OleContainerItemWidth + _alignmentGrid.GridCellSize.Width, 0.0);
                }
                break;
            case Direction.Up:
                if (oleContainerItem != null || (!IsInputSectionVisible && !IsOutputSectionVisible))
                {
                    val = new Vector(0.0, -OleContainerItemHeight - _alignmentGrid.GridCellSize.Height);
                }
                break;
            case Direction.Down:
                if (oleContainerItem != null || (!IsInputSectionVisible && !IsOutputSectionVisible))
                {
                    val = new Vector(0.0, OleContainerItemHeight + _alignmentGrid.GridCellSize.Height);
                }
                break;
        }
        return val ?? base.NavigateInto(direction, point);
    }

    private Size FindMinimumSize(Size oleContainerItemMinimalSize)
    {
        double num;
        double num2;
        if (IsResizing || IsInternalResizing)
        {
            num = Math.Max(oleContainerItemMinimalSize.Width, GetRightMostItemEdge(InputItems.Union(OutputItems)));
            num2 = AlignHeightWithGridLines(oleContainerItemMinimalSize.Height, isOleContainerItem: true);
            if (IsInputSectionVisible)
            {
                num2 += Math.Max(InputSectionActualHeight, GetLowestItemEdge(InputItems));
            }
            if (IsOutputSectionVisible)
            {
                num2 += Math.Max(OutputSectionActualHeight, GetLowestItemEdge(OutputItems) - OutputSectionActualTop);
            }
        }
        else
        {
            num = oleContainerItemMinimalSize.Width;
            num2 = GetMinimumContentHeight(AlignHeightWithGridLines(oleContainerItemMinimalSize.Height, isOleContainerItem: true));
        }
        return new Size(num, num2);
    }

    private bool IsPointInDeadArea(Point point, double oleContainerItemTop)
    {
        Rect val = new Rect(new Point(0.0, oleContainerItemTop), OleContainerItemSize);
        return val.Contains(point);
    }

    private void ReconcileOutputItemsLocations()
    {
        UpdateItemsLocation(isOutput: true);
    }

    protected override bool CanDeleteVerticalSpace(Direction direction)
    {
        Point insertionPoint = InsertionPoint;
        if (insertionPoint.Y <= InputSectionActualHeight ? CheckCanDeleteInInput() : CheckCanDeleteInOutput(direction))
        {
            return base.CanDeleteVerticalSpace(direction);
        }
        return false;
    }

    public override void DeleteVerticalSpaceAbove(int numberOfLines)
    {
        DeleteSpaceCommandForOleComponent command = new DeleteSpaceCommandForOleComponent(this, numberOfLines, Direction.Up);
        command.Execute();
    }

    public override void DeleteVerticalSpaceBelow(int numberOfLines)
    {
        DeleteSpaceCommandForOleComponent command = new DeleteSpaceCommandForOleComponent(this, numberOfLines, Direction.Down);
        command.Execute();
    }

    protected override void SetIndicatorsDimensions()
    {
        if (OutputSectionActualTop.IsValid())
        {
            OutputSectionIndicator.Top = OutputSectionActualTop;
        }
    }

    public override PasteItemsCommand GetPasteItemsCommand(string commandName, IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation)
    {
        return new PasteOleComponentItemsCommand(this, commandName, pasteItemsAtLocation);
    }

    protected override Size FindMinimumSize()
    {
        return FindMinimumSize(useDefault: false);
    }

    private void OleContainerMinSizeChanged(bool allowResize)
    {
        MinimumSize = FindMinimumSize();
        Size oleContainerItemMinSize = GetOleContainerItemMinSize();
        if (allowResize && (OleContainerItemSize.Height < oleContainerItemMinSize.Height || OleContainerItemSize.Width < oleContainerItemMinSize.Width))
        {
            ForceOleComponentResizeDueToOleContainerItemResize(setExplicitMinHeight: false);
        }
    }
}
