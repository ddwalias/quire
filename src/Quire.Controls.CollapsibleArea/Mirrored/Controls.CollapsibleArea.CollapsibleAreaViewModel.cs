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
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Protection;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Whiteboard;
using Quire.FunctionalitiesLimitation;
using Quire.PersistentData;
using Quire.Services;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.CollapsibleArea;

public partial class CollapsibleAreaViewModel
{
    internal const string DragBorderName = "PART_DragBorder";

    public static readonly double CollapsedAreaHeight = 1.0;

    private readonly ICollapsibleAreaModel _model;

    private bool _areaHasErrors;

    private double _areaHeight;

    public bool IsCollapsed => _model.IsCollapsed;

    public bool IsBorderVisible
    {
        get
        {
            return _model.IsBorderVisible;
        }
        set
        {
            _model.IsBorderVisible = value;
        }
    }

    public bool AreaHasErrors
    {
        get
        {
            return _areaHasErrors;
        }
        set
        {
            // Release IL returns early on an unchanged value; a block-if compiles to a branch to the exit instead.
            if (_areaHasErrors == value)
            {
                return;
            }
            _areaHasErrors = value;
            OnPropertyChanged("AreaHasErrors");
        }
    }

    public double AreaHeight
    {
        get
        {
            double areaHeight = _areaHeight;
            Size gridCellSize = Grid.GridCellSize;
            return areaHeight * gridCellSize.Height;
        }
        set
        {
            _areaHeight = value / Grid.GridCellSize.Height;
        }
    }

    public double AreaWidth { get; set; }

    public CollapsibleAreaViewModel Clone()
    {
        CollapsibleAreaViewModel collapsibleAreaViewModel = new CollapsibleAreaViewModel(this);
        collapsibleAreaViewModel.WhiteboardItems.Each(collapsibleAreaViewModel.HandleItemAdded);
        if (IsInPrinting && IsCollapsed && DebugTools.DevAssert(PrintViewport.HasValue, null).Passed)
        {
            collapsibleAreaViewModel.AreaHasErrors = AreaHasErrors;
        }
        if (IsInPrinting && !IsBorderVisible && DebugTools.DevAssert(PrintViewport.HasValue, null).Passed)
        {
            collapsibleAreaViewModel.IsBorderVisible = IsBorderVisible;
        }
        return collapsibleAreaViewModel;
    }

    public void SetPersistentData(IAreaType collapsibleArea, Version formatVersion = null)
    {
        _model.SetPersistentData(collapsibleArea, formatVersion);
    }

    public void GetPersistentData(IAreaType collapsibleArea)
    {
        _model.GetPersistentData(collapsibleArea);
    }

    public Func<ProtectionDialogState, bool> ShowProtectionDialogRequest { set; private get; }

    public Func<Func<string, bool>, bool> ShowUnProtectDialogRequest { set; private get; }

    public Action<string> ShowWarningDialogRequest { set; private get; }

    private bool _protectionOperationsEnabled;

    public CollapsibleAreaViewModel(ICollapsibleAreaModel model, Size? gridCellSize = null)
        : base(model, gridCellSize)
    {
        _model = model;
        _model.PropertyChanged += ModelPropertyChangedInner;
        double num = WpfUtils.InchesToDips(0.2);
        _insertionPoint = new Point(num, num);
        UpdateProtectionOperationEnabled();
        FunctionalityServiceWrapper.Instance.FunctionalityServiceLicenseTypeChanged += (object sender, EventArgs e) =>
        {
            UpdateProtectionOperationEnabled();
        };
        BindCommands();
    }

    private void UpdateProtectionOperationEnabled()
    {
        ProtectionOperationsEnabled = FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.ToggleProtectStateCommand);
    }

    private CollapsibleAreaViewModel(CollapsibleAreaViewModel original)
        : base(original)
    {
        _model = (ICollapsibleAreaModel)_mathboardModel;
        _insertionPoint = original._insertionPoint;
        _areaHasErrors = original._areaHasErrors;
        _areaHeight = original._areaHeight;
        AreaWidth = original.AreaWidth;
        IsBorderVisible = original.IsBorderVisible;
        if (IsCollapsed)
        {
            HandleCollapsibleStateChanged();
        }
        BindCommands();
    }

    public ProtectionData ProtectionData => _model.ProtectionData;

    object ICloneable.Clone()
    {
        return Clone();
    }

    private void BindCommands()
    {
        CreateCommandBinding(CollapsibleAreaControl.AreaControlToggleCollapseState, (object param) =>
        {
            OnToggleCollapseState();
        }, (object param) => CanToggleCollapseState());
        CreateCommandBinding(RoutedProtectionCommands.ToggleProtectState, OnToggleProtectState, (object param) => CanToggleProtectState());
        CommandBindings.Add(new CommandBinding(CollapsibleAreaControl.BorderVisibilityCommand, (object sender, ExecutedRoutedEventArgs e) =>
        {
            OnBorderVisibilityCommand();
        }));
    }

    private bool CanToggleCollapseState()
    {
        return _model.CanToggleCollapseState;
    }

    private void ToggleCollapseState()
    {
        CommandBuffer.Execute(new ToggleCollapseStateCommand(_model));
    }

    private void BorderVisibilityCommand()
    {
        CommandBuffer.Execute(new ToggleBorderCollapseStateCommand(_model));
    }

    private void OnToggleCollapseState()
    {
        ToggleCollapseState();
    }

    public void OnBorderVisibilityCommand()
    {
        BorderVisibilityCommand();
    }

    public bool ProtectionOperationsEnabled
    {
        get
        {
            return _protectionOperationsEnabled;
        }
        private set
        {
            // Release IL returns early on an unchanged value; a block-if compiles to a branch to the exit instead.
            if (_protectionOperationsEnabled == value)
            {
                return;
            }
            _protectionOperationsEnabled = value;
            OnPropertyChanged("ProtectionOperationsEnabled");
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.CollapsibleArea.CollapsibleAreaViewModel+PropertyNames, Ptc.Controls.CollapsibleArea")]
    public new static partial class PropertyNames
    {
        public const string AreaHasErrors = "AreaHasErrors";

        public const string ProtectionData = "ProtectionData";

        public const string IsCollapsed = "IsCollapsed";

        public const string IsBorderVisible = "IsBorderVisible";

        public const string ProtectionOperationsEnabled = "ProtectionOperationsEnabled";
    }

    private bool CanToggleProtectState()
    {
        return true;
    }

    private void OnToggleProtectState(object arg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override Size StrictMinimumSize
    {
        get
        {
            Size result;
            if (!IsCollapsed)
            {
                double areaWidth = AreaWidth;
                result = Grid.GridCellSize;
                return new Size(areaWidth, result.Height * 2.0);
            }
            result = default;
            return result;
        }
    }

    public override bool CanAddItems
    {
        get
        {
            if (!IsCollapsed && !ProtectionData.ProtectionLevel().IsActivationProtected())
            {
                return base.CanAddItems;
            }
            return false;
        }
    }

    public override bool FindOncePrivate(string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        if (IsCollapsed && CanToggleCollapseState())
        {
            ToggleCollapseState();
        }
        return base.FindOncePrivate(text, startFromCaret, direction, options, subscriptPositions);
    }

    public override bool IsTabNavigatable
    {
        get
        {
            if (!ProtectionData.ProtectionLevel().IsActivationProtected() && !IsCollapsed)
            {
                return base.IsTabNavigatable;
            }
            return false;
        }
    }

    private static bool IsToggleButtonClicked(CollapsibleAreaControl collapsibleAreaControl, object originalSource)
    {
        Canvas canvas = collapsibleAreaControl.Template.FindName("PART_AreaSection", collapsibleAreaControl) as Canvas;
        if (DebugTools.AssertNotNull(canvas, null).Failed)
        {
            return false;
        }
        if (!(originalSource is Visual visual))
        {
            return false;
        }
        if (!((object)canvas).Equals((object)visual))
        {
            return canvas.IsAncestorOf(visual);
        }
        return true;
    }

    public override void PreviewMouseLeftButtonDownHandler(object sender, MouseButtonEventArgs eventArgs)
    {
        if (ShouldBlockLeftMouseClick(sender, eventArgs))
        {
            eventArgs.Handled = true;
        }
        else
        {
            base.PreviewMouseLeftButtonDownHandler(sender, eventArgs);
        }
    }

    public override void PreviewMouseLeftButtonUpHandler(object sender, RoutedEventArgs eventArgs)
    {
        if (ShouldBlockLeftMouseClick(sender, eventArgs))
        {
            eventArgs.Handled = true;
        }
        else
        {
            base.PreviewMouseLeftButtonUpHandler(sender, eventArgs);
        }
    }

    public override void PreviewMouseRightButtonDownHandler(object sender, MouseButtonEventArgs eventArgs)
    {
        if (ShouldBlockRightMouseClick(sender, eventArgs))
        {
            eventArgs.Handled = true;
        }
        else
        {
            base.PreviewMouseRightButtonDownHandler(sender, eventArgs);
        }
    }

    public override void PreviewMouseRightButtonUpHandler(object sender, RoutedEventArgs eventArgs)
    {
        base.PreviewMouseRightButtonUpHandler(sender, eventArgs);
    }

    private bool ShouldBlockRightMouseClick(object sender, RoutedEventArgs eventArgs)
    {
        if (ProtectionData.ProtectionLevel().IsActivationProtected() || IsCollapsed)
        {
            CollapsibleAreaControl collapsibleAreaControl = sender as CollapsibleAreaControl;
            if (DebugTools.AssertNotNull(collapsibleAreaControl, null).Passed && !IsToggleButtonClicked(collapsibleAreaControl, eventArgs.OriginalSource))
            {
                return true;
            }
        }
        return false;
    }

    private bool ShouldBlockLeftMouseClick(object sender, RoutedEventArgs eventArgs)
    {
        if (ProtectionData.ProtectionLevel().IsActivationProtected())
        {
            bool flag = false;
            bool flag2 = false;
            CollapsibleAreaControl collapsibleAreaControl = sender as CollapsibleAreaControl;
            if (DebugTools.AssertNotNull(collapsibleAreaControl, null).Failed)
            {
                return false;
            }
            if (!ProtectionData.ProtectionLevel().IsSelectionProtected())
            {
                RegionBorder regionBorder = collapsibleAreaControl.Template.FindName("PART_DragBorder", collapsibleAreaControl) as RegionBorder;
                if (DebugTools.AssertNotNull(regionBorder, null).Failed)
                {
                    return false;
                }
                flag = regionBorder.GetDragMode(eventArgs).HasValue;
            }
            flag2 = IsToggleButtonClicked(collapsibleAreaControl, eventArgs.OriginalSource);
            if (!(flag | flag2))
            {
                return true;
            }
        }
        return false;
    }

    public override void HandleGridCellSizeChanged(Size oldSize, Size newSize) => base.HandleGridCellSizeChanged(oldSize, newSize);

    protected override void HandleItemAdded(IWhiteboardItem item)
    {
        if (!IsCollapsed)
        {
            base.HandleItemAdded(item);
            return;
        }
        Action action = () =>
        {
            if (item is ICalculatableInfo calculatableInfo)
            {
                calculatableInfo.PropertyChanged += ErrorPropertyChangedHandler;
            }
            else if (item is ICalculatableContainer calculatableContainer && !(calculatableContainer is ICalculatableAggregate))
            {
                calculatableContainer.CalculatableChildren.OfType<ICalculatableInfo>().Each((ICalculatableInfo info) =>
                {
                    info.PropertyChanged += ErrorPropertyChangedHandler;
                });
            }
            base.HandleItemAdded(item);
        };
        WhiteboardViewModel.WhiteboardItemMgr.ExecuteOnLoad(item, action);
    }

    public override void UpdateSizeLimits(Size maximumSize)
    {
        if (!IsCollapsed)
        {
            base.UpdateSizeLimits(maximumSize);
        }
    }

    public override Size ResizeAndNotify(IEnumerable<IWhiteboardItem> list, Size oldSize)
    {
        if (!IsCollapsed)
        {
            return base.ResizeAndNotify(list, oldSize);
        }
        return ContentSize;
    }

    protected override void HandleChildSizeChanged(IWhiteboardItem item, Size oldSize, Size newSize)
    {
        if (!IsCollapsed)
        {
            base.HandleChildSizeChanged(item, oldSize, newSize);
            return;
        }
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
        if (AreaHeight.IsLessThan(contentSize.Height))
        {
            AreaHeight = contentSize.Height;
        }
    }

    public override Vector? NavigateInto(Direction direction, Point point)
    {
        if (ProtectionData.ProtectionLevel().IsActivationProtected())
        {
            DebugTools.FailAssertion((string)null);
            Point nextIntersection = _alignmentGrid.GetNextIntersection(new Point(0.0, (direction == Direction.Down) ? ContentHeight : 0.0), direction);
            return new Vector(nextIntersection.X, nextIntersection.Y - point.Y);
        }
        return base.NavigateInto(direction, point);
    }

    protected override void SetInsertionPoint(Point insertionPoint)
    {
        insertionPoint = Grid.GetNearestIntersectionPoint(insertionPoint);
        if (insertionPoint.Y.IsEqualTo(0.0))
        {
            insertionPoint.Offset(0.0, Grid.GridCellSize.Height);
        }
        if (insertionPoint.Y.IsEqualTo(ContentHeight))
        {
            insertionPoint.Offset(0.0, -Grid.GridCellSize.Height);
        }
        base.SetInsertionPoint(insertionPoint);
    }

    protected override bool BoundsOutsideContentSize(Rect rect)
    {
        if (!(rect.Bottom > ContentHeight))
        {
            return rect.Top < 0.0;
        }
        return true;
    }

    protected override bool CanMove(Direction direction)
    {
        bool flag = base.CanMove(direction);
        if (SelectedItems.Count() != 0)
        {
            return flag;
        }
        if (flag)
        {
            Point nearestIntersectionInDirection = Grid.GetNearestIntersectionInDirection(GetNavigationStartLocation(direction), direction);
            if (nearestIntersectionInDirection.Y.IsEqualTo(0.0) && direction == Direction.Up)
            {
                flag = false;
            }
            if (nearestIntersectionInDirection.Y.IsEqualTo(ContentHeight) && direction == Direction.Down)
            {
                flag = false;
            }
        }
        return flag;
    }

    public override Point DefaultInsertionPoint
    {
        get
        {
            double num = WpfUtils.InchesToDips(0.2);
            return new Point(num, num);
        }
    }

    protected override void SelectedItemsAddNew(IEnumerable<IWhiteboardItem> items)
    {
        if (DebugTools.DevAssert(!IsCollapsed, null).Failed)
        {
            _model.ToggleCollapseState();
        }
        base.SelectedItemsAddNew(items);
    }

    public override void SaveState()
    {
        if (!IsCollapsed)
        {
            base.SaveState();
        }
    }

    public override void ClearSavedState()
    {
        if (!IsCollapsed)
        {
            base.ClearSavedState();
        }
    }

    public override void RestoreSavedState()
    {
        if (!IsCollapsed)
        {
            base.RestoreSavedState();
        }
    }

    public override bool CanDeleteVerticalSpaceBelow
    {
        get
        {
            if (base.CanDeleteVerticalSpaceBelow)
            {
                Point insertionPoint = InsertionPoint;
                return insertionPoint.Y.IsLessThan(ContentHeight - Grid.GridCellSize.Height);
            }
            return false;
        }
    }

    public Rect? PrintViewport
    {
        get
        {
            return _model.PrintViewport;
        }
        set
        {
            _model.PrintViewport = value;
        }
    }

    public override bool HandleMouseDown(IWhiteboardControl sender, Point position, MouseButton button)
    {
        if (IsCollapsed && button != MouseButton.Left)
        {
            return false;
        }
        if (sender is IWhiteboardItem item && !WhiteboardViewModel.WhiteboardItemMgr.GetCanActivate(item))
        {
            return false;
        }
        return base.HandleMouseDown(sender, position, button);
    }

    protected override void HandleChildItemGotFocus(IWhiteboardItem item)
    {
        if (DebugTools.DevAssert(!IsCollapsed, null).Failed)
        {
            _model.ToggleCollapseState();
        }
        base.HandleChildItemGotFocus(item);
    }

    public IResizeData StoreResizeData(Size oldSize, Size newSize)
    {
        return new CollapsibleAreaResizeData(new Size(ContentWidth, ContentHeight));
    }

    public void RestoreResizeData(IResizeData resizeData)
    {
        if (DebugTools.DevAssert(!IsCollapsed, null).Failed)
        {
            _model.ToggleCollapseState();
        }
        CollapsibleAreaResizeData collapsibleAreaResizeData = resizeData as CollapsibleAreaResizeData;
        if (collapsibleAreaResizeData == null)
        {
            ContentSize = new Size(ContentWidth, ContentHeight);
            return;
        }
        Size oldSize = collapsibleAreaResizeData.OldSize;
        ContentSize = oldSize.IsEmpty ? new Size(ContentWidth, ContentHeight) : oldSize;
    }

    private void HandleCollapsibleStateChanged()
    {
        if (IsCollapsed)
        {
            AreaHeight = ContentHeight;
            MinimumHeight = 0.0;
            IsCrosshairVisible = false;
            ContentHeight = CollapsedAreaHeight;
            AreaHasErrors = _model.CalculatableChildren.OfType<ICalculatableInfo>().Any((ICalculatableInfo calcInfo) => calcInfo.ErrorCount != 0);
            _model.CalculatableChildren.OfType<ICalculatableInfo>().Each((ICalculatableInfo calcInfo) =>
            {
                calcInfo.PropertyChanged += ErrorPropertyChangedHandler;
            });
            if (HasSelectedItems)
            {
                ClearSelectedItems();
            }
        }
        else
        {
            MinimumHeight = FindMinimumSize().Height;
            ContentHeight = AreaHeight;
            _model.CalculatableChildren.OfType<ICalculatableInfo>().Each((ICalculatableInfo calcInfo) =>
            {
                calcInfo.PropertyChanged -= ErrorPropertyChangedHandler;
            });
        }
    }

    protected override void ModelPropertyChanged(string propertyName)
    {
        if (propertyName == "IsCollapsed")
        {
            HandleCollapsibleStateChanged();
        }
        FireChanged();
    }

    private void ErrorPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Error")
        {
            AreaHasErrors = _model.CalculatableChildren.OfType<ICalculatableInfo>().Any((ICalculatableInfo calcInfo) => calcInfo.ErrorCount != 0);
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.CollapsibleArea.CollapsibleAreaViewModel+CollapsibleAreaResizeData, Ptc.Controls.CollapsibleArea")]
    internal partial class CollapsibleAreaResizeData : IResizeData
    {
        public Size OldSize { get; private set; }

        public CollapsibleAreaResizeData(Size oldSize)
        {
            OldSize = oldSize;
        }
    }
}
