#nullable disable
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class RegionBorder
{
    private const string PartLeftDragger = "LeftDragger";
    private const string PartRightDragger = "RightDragger";
    private const string PartBottomDragger = "BottomDragger";
    private const string PartBottomLeftDragger = "BottomLeftDragger";
    private const string PartBottomRightDragger = "BottomRightDragger";
    private const string PartTopDragger = "TopDragger";
    private const string PartTopLeftDragger = "TopLeftDragger";
    private const string PartTopRightDragger = "TopRightDragger";
    private const string PartMoveDragger = "MoveDragger";
    private const string PartBorderGrid = "borderGrid";
    private const string PartRootBorder = "rootBorder";
    private const string PartHeader = "header";
    private const string PartBackground = "background";
    private const string PartForeground = "foreground";
    private const string PartBusyClock = "PartBusyClock";

    private static readonly Type _typeOfThis;

    public static readonly RoutedEvent RequestMoveAndResizeEvent;
    public static readonly DependencyProperty DraggerBackgroundProperty;
    public static readonly DependencyProperty LeftDraggerVisibilityProperty;
    public static readonly DependencyProperty RightDraggerVisibilityProperty;
    public static readonly DependencyProperty BottomDraggerVisibilityProperty;
    public static readonly DependencyProperty BottomLeftDraggerVisibilityProperty;
    public static readonly DependencyProperty BottomRightDraggerVisibilityProperty;
    public static readonly DependencyProperty TopDraggerVisibilityProperty;
    public static readonly DependencyProperty TopLeftDraggerVisibilityProperty;
    public static readonly DependencyProperty TopRightDraggerVisibilityProperty;
    public static readonly DependencyProperty MoveDraggerVisibilityProperty;
    public static readonly DependencyProperty ShowBusyProperty;
    public static readonly DependencyProperty PageBackgroundColorProperty;
    public static readonly DependencyProperty IsActiveProperty;
    public static readonly DependencyProperty IsSelectedProperty;
    public static readonly DependencyProperty HeaderProperty;
    public static readonly DependencyProperty ActiveRegionBackgroundOpacityProperty;
    public static readonly DependencyProperty ForceBorderProperty;
    public static readonly DependencyProperty BorderVisibilityProperty;
    public static readonly DependencyProperty RightEdgeVisibilityProperty;
    public static readonly DependencyProperty BottomEdgeVisibilityProperty;
    public static readonly DependencyProperty CornerRadiusProperty;

    static RegionBorder()
    {
        _typeOfThis = typeof(RegionBorder);
        RequestMoveAndResizeEvent = EventManager.RegisterRoutedEvent("RequestMoveAndResize", RoutingStrategy.Bubble, typeof(RequestMoveAndResizeHandler), _typeOfThis);
        DraggerBackgroundProperty = DependencyProperty.Register("DraggerBackground", typeof(Brush), _typeOfThis, new FrameworkPropertyMetadata(OnDraggerBackgroundChanged));
        LeftDraggerVisibilityProperty = DependencyProperty.Register("LeftDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnLeftDraggerVisibilityChanged));
        RightDraggerVisibilityProperty = DependencyProperty.Register("RightDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnRightDraggerVisibilityChanged));
        BottomDraggerVisibilityProperty = DependencyProperty.Register("BottomDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnBottomDraggerVisibilityChanged));
        BottomLeftDraggerVisibilityProperty = DependencyProperty.Register("BottomLeftDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnBottomLeftDraggerVisibilityChanged));
        BottomRightDraggerVisibilityProperty = DependencyProperty.Register("BottomRightDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnBottomRightDraggerVisibilityChanged));
        TopDraggerVisibilityProperty = DependencyProperty.Register("TopDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnTopDraggerVisibilityChanged));
        TopLeftDraggerVisibilityProperty = DependencyProperty.Register("TopLeftDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnTopLeftDraggerVisibilityChanged));
        TopRightDraggerVisibilityProperty = DependencyProperty.Register("TopRightDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Collapsed, OnTopRightDraggerVisibilityChanged));
        MoveDraggerVisibilityProperty = DependencyProperty.Register("MoveDraggerVisibility", typeof(Visibility), _typeOfThis, new FrameworkPropertyMetadata(Visibility.Visible));
        ShowBusyProperty = DependencyProperty.Register("ShowBusy", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(OnShowBusyChanged));
        PageBackgroundColorProperty = DependencyProperty.Register("PageBackgroundColor", typeof(Color), _typeOfThis);
        IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(OnActiveSelectedChanged));
        IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(OnActiveSelectedChanged));
        HeaderProperty = DependencyProperty.Register("Header", typeof(string), _typeOfThis);
        ActiveRegionBackgroundOpacityProperty = DependencyProperty.Register("ActiveRegionBackgroundOpacity", typeof(double), _typeOfThis, new FrameworkPropertyMetadata(0.7));
        ForceBorderProperty = DependencyProperty.Register("ForceBorder", typeof(ForceBorderType), _typeOfThis, new FrameworkPropertyMetadata(OnForceBorderChanged));
        BorderVisibilityProperty = DependencyProperty.Register("BorderVisibility", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(OnForceBorderChanged));
        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
        Control.BackgroundProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(OnRegionBorderBackgroundChanged));
        Control.BorderBrushProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(OnRegionBorderBrushChanged));
        RightEdgeVisibilityProperty = BorderShape.RightEdgeVisibilityProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(OnRightEdgeVisibilityChanged));
        CornerRadiusProperty = BorderShape.CornerRadiusProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(OnCornerRadiusChanged));
    }

    private static void OnRegionBorderBackgroundChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
        ((RegionBorder)depObj).UpdateBackgroundAndForeground();
    }

    private static void OnRegionBorderBrushChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
        ((RegionBorder)depObj)._regionBorderBrushChanged?.Invoke();
    }

    private static void OnDraggerBackgroundChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
        RegionBorder regionBorder = depObj as RegionBorder;
        if (DebugTools.AssertNotNull(regionBorder, null).Passed)
        {
            regionBorder.UpdateDraggersLook();
        }
    }

    private static void OnLeftDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartLeftDragger, (Visibility)arg.NewValue);
    }

    private static void OnRightDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartRightDragger, (Visibility)arg.NewValue);
    }

    private static void OnBottomDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartBottomDragger, (Visibility)arg.NewValue);
    }

    private static void OnBottomLeftDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartBottomLeftDragger, (Visibility)arg.NewValue);
    }

    private static void OnBottomRightDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartBottomRightDragger, (Visibility)arg.NewValue);
    }

    private static void OnTopDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartTopDragger, (Visibility)arg.NewValue);
    }

    private static void OnTopLeftDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartTopLeftDragger, (Visibility)arg.NewValue);
    }

    private static void OnTopRightDraggerVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        OnDraggerVisibilityChanged(depObj as RegionBorder, PartTopRightDragger, (Visibility)arg.NewValue);
    }

    private static void OnDraggerVisibilityChanged(RegionBorder regionBorder, string name, Visibility newValue)
    {
        if (DebugTools.AssertNotNull(regionBorder, null).Passed && regionBorder.AreDraggersShown)
        {
            BorderDragger borderDragger = regionBorder.FindBorderDraggerByName(name);
            if (borderDragger != null)
            {
                borderDragger.Visibility = newValue;
            }
        }
    }

    private static void OnShowBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RegionBorder regionBorder)
        {
            regionBorder.UpdateBusyState();
            regionBorder.UpdateBorderVisibilityState();
        }
    }

    private static void OnActiveSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RegionBorder regionBorder)
        {
            regionBorder.UpdateActiveState();
            regionBorder.UpdateBorderVisibilityState();
            regionBorder.UpdateZIndex();
        }
    }

    public static void OnForceBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as RegionBorder)?.UpdateBorderVisibilityState();
    }

    private static void OnRightEdgeVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        ((RegionBorder)depObj)._rightEdgeVisibilityChanged?.Invoke();
    }

    private static void OnBottomEdgeVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        ((RegionBorder)depObj)._bottomEdgeVisibilityChanged?.Invoke();
    }

    private static void OnCornerRadiusChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs arg)
    {
        ((RegionBorder)depObj)._cornerRadiusChanged?.Invoke();
    }

    internal bool IsBorderShapeVisible
    {
        get
        {
            if (_border != null)
            {
                return _border.IsVisible;
            }
            return false;
        }
    }

    internal bool IsBorderHeaderVisible
    {
        get
        {
            if (_border != null)
            {
                return _header.IsVisible;
            }
            return false;
        }
    }

    internal Brush BorderShapeColor
    {
        get
        {
            if (_border != null)
            {
                return _border.Stroke;
            }
            return null;
        }
    }

    public Brush DraggerBackground
    {
        get => (Brush)GetValue(DraggerBackgroundProperty);
        set => SetValue(DraggerBackgroundProperty, value);
    }

    public Visibility LeftDraggerVisibility
    {
        get => (Visibility)GetValue(LeftDraggerVisibilityProperty);
        set => SetValue(LeftDraggerVisibilityProperty, value);
    }

    public Visibility RightDraggerVisibility
    {
        get => (Visibility)GetValue(RightDraggerVisibilityProperty);
        set => SetValue(RightDraggerVisibilityProperty, value);
    }

    public Visibility BottomDraggerVisibility
    {
        get => (Visibility)GetValue(BottomDraggerVisibilityProperty);
        set => SetValue(BottomDraggerVisibilityProperty, value);
    }

    public Visibility BottomLeftDraggerVisibility
    {
        get => (Visibility)GetValue(BottomLeftDraggerVisibilityProperty);
        set => SetValue(BottomLeftDraggerVisibilityProperty, value);
    }

    public Visibility BottomRightDraggerVisibility
    {
        get => (Visibility)GetValue(BottomRightDraggerVisibilityProperty);
        set => SetValue(BottomRightDraggerVisibilityProperty, value);
    }

    public Visibility TopDraggerVisibility
    {
        get => (Visibility)GetValue(TopDraggerVisibilityProperty);
        set => SetValue(TopDraggerVisibilityProperty, value);
    }

    public Visibility TopLeftDraggerVisibility
    {
        get => (Visibility)GetValue(TopLeftDraggerVisibilityProperty);
        set => SetValue(TopLeftDraggerVisibilityProperty, value);
    }

    public Visibility TopRightDraggerVisibility
    {
        get => (Visibility)GetValue(TopRightDraggerVisibilityProperty);
        set => SetValue(TopRightDraggerVisibilityProperty, value);
    }

    public Visibility MoveDraggerVisibility
    {
        get => (Visibility)GetValue(MoveDraggerVisibilityProperty);
        set => SetValue(MoveDraggerVisibilityProperty, value);
    }

    [MirrorVerified("PtcControlsCoreRegionBorderMechanicalDifferentialTests")]
    private Shape _busyProgressBar;

    [MirrorVerified("PtcControlsCoreRegionBorderMechanicalDifferentialTests")]
    private void HideBusyStateElements() => _busyProgressBar.Visibility = (Visibility)2;


    private BorderDragger FindBorderDraggerByName(string name)
    {
        if (_draggers != null)
        {
            return _draggers.First((BorderDragger item) => item.Name.Equals(name));
        }
        return null;
    }

    private Grid _borderGrid;
    private BorderShape _border;
    private List<BorderDragger> _draggers;
    private BusyClock _busyClock;
    private Rectangle _marchingAnts;
    private Rectangle _background;
    private Rectangle _foreground;
    private TextBlock _header;
    private Action _regionBorderBrushChanged;
    private Action _rightEdgeVisibilityChanged;
    private Action _bottomEdgeVisibilityChanged;
    private Action _cornerRadiusChanged;
    // Compile-order device: 3 alignment fields to reach Mathcad closure ordinals 160, 166, 167, 168.
    private int _closureAlignment1;
    private int _closureAlignment2;
    private int _closureAlignment3;


    public bool ShowBusy
    {
        get => (bool)GetValue(ShowBusyProperty);
        set => SetValue(ShowBusyProperty, value);
    }

    public Color PageBackgroundColor
    {
        get => (Color)GetValue(PageBackgroundColorProperty);
        set => SetValue(PageBackgroundColorProperty, value);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public double ActiveRegionBackgroundOpacity
    {
        get => (double)GetValue(ActiveRegionBackgroundOpacityProperty);
        set => SetValue(ActiveRegionBackgroundOpacityProperty, value);
    }

    public ForceBorderType ForceBorder
    {
        get => (ForceBorderType)GetValue(ForceBorderProperty);
        set => SetValue(ForceBorderProperty, value);
    }

    public bool BorderVisibility
    {
        get => (bool)GetValue(BorderVisibilityProperty);
        set => SetValue(BorderVisibilityProperty, value);
    }

    public bool RightEdgeVisibility
    {
        get => (bool)GetValue(RightEdgeVisibilityProperty);
        set => SetValue(RightEdgeVisibilityProperty, value);
    }

    public bool BottomEdgeVisibility
    {
        get => (bool)GetValue(BottomEdgeVisibilityProperty);
        set => SetValue(BottomEdgeVisibilityProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }


    public event RequestMoveAndResizeHandler RequestMoveAndResize
    {
        add => AddHandler(RequestMoveAndResizeEvent, value);
        remove => RemoveHandler(RequestMoveAndResizeEvent, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _borderGrid = Template.FindName(PartBorderGrid, this) as Grid;
        _header = Template.FindName(PartHeader, this) as TextBlock;
        _background = Template.FindName(PartBackground, this) as Rectangle;
        _foreground = Template.FindName(PartForeground, this) as Rectangle;
        _busyProgressBar = Template.FindName(PartBusyClock, this) as Shape;
        UpdateActiveState();
        UpdateBusyState();
        UpdateBorderVisibilityState();
        UpdateZIndex();
    }

    public Rect GetBoundingRect()
    {
        return new Rect(0.0, 0.0, ActualWidth, ActualHeight);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (TemplatedParent == null)
        {
            DebugTools.DevAssert(false, null);
        }
        Control originalControlElement;
        DragMode? dragMode = GetDragMode(e, out originalControlElement);
        if (dragMode.HasValue)
        {
            Control control = TemplatedParent as Control;
            RaiseMoveAndResizeEvent(dragMode.Value, control, e.GetPosition(control), originalControlElement, automation: false);
            e.Handled = true;
        }
    }

    public DragMode? GetDragMode(RoutedEventArgs args)
    {
        Control originalControlElement;
        return GetDragMode(args, out originalControlElement);
    }

    private DragMode? GetDragMode(RoutedEventArgs args, out Control originalControlElement)
    {
        DragMode? result = null;
        originalControlElement = null;
        DependencyObject source = args.OriginalSource as DependencyObject;
        if (source == null)
        {
            return null;
        }
        originalControlElement = source.TryFindVisualAncestor<Control>();
        if (originalControlElement == null)
        {
            return null;
        }
        RegionBorder regionBorder = originalControlElement.TryFindAncestorIncludingSelf<RegionBorder>();
        if (originalControlElement != TemplatedParent)
        {
            DebugTools.AssertNotNull(regionBorder, null);
        }
        if (regionBorder != this)
        {
            return null;
        }
        // Release IL keeps the switched name in a local declared before the switch.
        string name = originalControlElement.Name;
        switch (name)
        {
            case PartMoveDragger:
                result = DragMode.Move;
                break;
            case PartLeftDragger:
                result = DragMode.ResizeLeft;
                break;
            case PartRightDragger:
                result = DragMode.ResizeRight;
                break;
            case PartBottomDragger:
                result = DragMode.ResizeBottom;
                break;
            case PartBottomLeftDragger:
                result = DragMode.ResizeBottomLeft;
                break;
            case PartBottomRightDragger:
                result = DragMode.ResizeBottomRight;
                break;
            case PartTopDragger:
                result = DragMode.ResizeTop;
                break;
            case PartTopLeftDragger:
                result = DragMode.ResizeTopLeft;
                break;
            case PartTopRightDragger:
                result = DragMode.ResizeTopRight;
                break;
        }
        return result;
    }

    private IResizer RaiseMoveAndResizeEvent(DragMode mode, FrameworkElement element, Point startPosition, DependencyObject originalEventSource, bool automation)
    {
        ResizeEventMouseArgs args = new ResizeEventMouseArgs(RequestMoveAndResizeEvent, mode, element, startPosition, originalEventSource, automation);
        RaiseEvent(args);
        return args.Resizer;
    }

    public static void ResizeElement(FrameworkElement element, Size newSize, DragMode dragMode, bool automation)
    {
        RegionBorder regionBorder = GetRegionBorder(element);
        double width = double.IsNaN(regionBorder.Width) ? regionBorder.ActualWidth : regionBorder.Width;
        double height = double.IsNaN(regionBorder.Height) ? regionBorder.ActualHeight : regionBorder.Height;
        IResizer resizer = regionBorder.RaiseMoveAndResizeEvent(dragMode, element, new Point(width, height), regionBorder.GetGrip(dragMode), automation);
        resizer.ResizeEnd(resizer.ResizeBy(new Vector(newSize.Width - width, newSize.Height - height)));
    }

    private FrameworkElement GetGrip(DragMode dragMode)
    {
        switch (dragMode)
        {
            case DragMode.Move:
                return FindName(PartMoveDragger) as Control;
            case DragMode.ResizeLeft:
                return FindBorderDraggerByName(PartLeftDragger);
            case DragMode.ResizeRight:
                return FindBorderDraggerByName(PartRightDragger);
            case DragMode.ResizeBottom:
                return FindBorderDraggerByName(PartBottomDragger);
            case DragMode.ResizeBottomLeft:
                return FindBorderDraggerByName(PartBottomLeftDragger);
            case DragMode.ResizeBottomRight:
                return FindBorderDraggerByName(PartBottomRightDragger);
            case DragMode.ResizeTop:
                return FindBorderDraggerByName(PartTopDragger);
            case DragMode.ResizeTopLeft:
                return FindBorderDraggerByName(PartTopLeftDragger);
            case DragMode.ResizeTopRight:
                return FindBorderDraggerByName(PartTopRightDragger);
            default:
                return null;
        }
    }

    public static RegionBorder GetRegionBorder(DependencyObject reference)
    {
        return reference.GetVisualDescendantsBreadthFirstOfType<RegionBorder>().FirstOrDefault();
    }

    private void UpdateBusyState()
    {
        if (ShowBusy)
        {
            ShowBusyStateElements();
        }
        else
        {
            HideBusyStateElements();
        }
    }

    private void UpdateActiveState()
    {
        SetBorderLook();
        AreDraggersShown = IsActive || IsSelected;
        if (!IsActive || !IsSelected)
        {
            UpdateBackgroundAndForeground();
        }
    }

    private void UpdateBackgroundAndForeground()
    {
        if (IsActive)
        {
            SolidColorBrush solidColorBrush = Background as SolidColorBrush;
            if (solidColorBrush == null ? true : solidColorBrush.Color == Colors.Transparent)
            {
                SetBackgroundActiveLook();
            }
            else
            {
                SetBackgroundDefaultLook();
            }
        }
        else if (IsSelected)
        {
            SetBackgroundSelectedLook();
        }
        else
        {
            SetBackgroundDefaultLook();
        }
    }

    private void SetBackgroundSelectedLook()
    {
        if (_foreground != null && _background != null)
        {
            _foreground.Opacity = 0.4;
            _background.Opacity = 0.0;
        }
    }

    private void SetBorderLook()
    {
        if (_borderGrid == null)
        {
            return;
        }
        if (_border == null)
        {
            _border = new BorderShape
            {
                Fill = Brushes.Transparent,
                IsHitTestVisible = false,
                Stroke = BorderBrush,
                StrokeThickness = 0.5,
                CornerRadius = CornerRadius,
                RightEdgeVisibility = RightEdgeVisibility
            };
            _regionBorderBrushChanged = (Action)Delegate.Combine(_regionBorderBrushChanged, (Action)(() =>
            {
                _border.Stroke = BorderBrush;
            }));
            _cornerRadiusChanged = (Action)Delegate.Combine(_cornerRadiusChanged, (Action)(() =>
            {
                _border.CornerRadius = CornerRadius;
            }));
            _rightEdgeVisibilityChanged = (Action)Delegate.Combine(_rightEdgeVisibilityChanged, (Action)(() =>
            {
                _border.RightEdgeVisibility = RightEdgeVisibility;
            }));
            _borderGrid.Children.Add(_border);
        }
        _border.StrokeDashArray = (IsActive ? RegionBorderConstants.BorderLineStrokeDashArray : null);
    }

    private void UpdateBorderVisibilityState()
    {
        Visibility visibility = (Visibility)(TryFindResource("InactiveHeaderBorderVisibility") ?? ((object)Visibility.Hidden));
        if (_border != null && _header != null)
        {
            bool flag;
            bool flag2;
            if (IsActive || IsSelected)
            {
                flag = (flag2 = true);
            }
            else
            {
                flag = false;
                flag2 = ForceBorder == ForceBorderType.HeaderOnly || ForceBorder == ForceBorderType.BorderAndHeader;
            }
            _border.Visibility = (!ShowBusy ? Visibility.Visible : visibility);
            if (!BorderVisibility && !flag)
            {
                _border.Visibility = visibility;
            }
            _header.Visibility = (flag2 ? Visibility.Visible : visibility);
        }
    }

    private bool _areDraggersShown;

    private bool AreDraggersShown
    {
        get => _areDraggersShown;
        set
        {
            _areDraggersShown = value;
            if (_areDraggersShown)
            {
                ShowDraggers();
            }
            else
            {
                HideDraggers();
            }
        }
    }

    private void ShowDraggers()
    {
        if (_borderGrid == null)
        {
            return;
        }
        if (_draggers == null)
        {
            _draggers = new List<BorderDragger>
            {
                new BorderDragger
                {
                    Name = "LeftDragger",
                    Cursor = Cursors.SizeWE,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                },
                new BorderDragger
                {
                    Name = "RightDragger",
                    Cursor = Cursors.SizeWE,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                },
                new BorderDragger
                {
                    Name = "BottomDragger",
                    Cursor = Cursors.SizeNS,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center
                },
                new BorderDragger
                {
                    Name = "TopDragger",
                    Cursor = Cursors.SizeNS,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center
                },
                new BorderDragger
                {
                    Name = "BottomLeftDragger",
                    Cursor = Cursors.SizeNESW,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left
                },
                new BorderDragger
                {
                    Name = "TopLeftDragger",
                    Cursor = Cursors.SizeNWSE,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                },
                new BorderDragger
                {
                    Name = "BottomRightDragger",
                    Cursor = Cursors.SizeNWSE,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Right
                },
                new BorderDragger
                {
                    Name = "TopRightDragger",
                    Cursor = Cursors.SizeNESW,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right
                }
            };
            _draggers.Each((BorderDragger dragger) =>
            {
                _borderGrid.Children.Add(dragger);
            });
            _regionBorderBrushChanged = (Action)Delegate.Combine(_regionBorderBrushChanged, new Action(UpdateDraggersLook));
        }
        _draggers.Each((BorderDragger dragger) =>
        {
            dragger.Visibility = GetDesiredDraggerVisibility(dragger);
        });
        UpdateDraggersLook();
    }

    private void HideDraggers()
    {
        _draggers?.Each((BorderDragger dragger) =>
        {
            dragger.Visibility = Visibility.Collapsed;
        });
    }

    private void UpdateDraggersLook()
    {
        if (_draggers == null)
        {
            return;
        }
        Brush background = (IsSelected ? RegionBorderConstants.SelectedDraggerBackground : DraggerBackground);
        double uniformLength = (IsSelected ? 0.75 : 0.5);
        Thickness borderThickness = new Thickness(uniformLength);
        _draggers.Each((BorderDragger dragger) =>
        {
            dragger.BorderBrush = BorderBrush;
            dragger.Background = background;
            dragger.BorderThickness = borderThickness;
        });
    }

    private void SetBackgroundActiveLook()
    {
        if (_foreground != null && _background != null)
        {
            _foreground.Opacity = 0.0;
            _background.Opacity = ActiveRegionBackgroundOpacity;
        }
    }

    private void SetBackgroundDefaultLook()
    {
        if (_foreground != null && _background != null)
        {
            _foreground.Opacity = 0.0;
            _background.Opacity = 0.0;
        }
    }

    private void ShowBusyStateElements()
    {
        if (_borderGrid == null)
        {
            return;
        }
        _busyProgressBar.Visibility = Visibility.Visible;
    }

    private void UpdateZIndex()
    {
        if (TemplatedParent is UIElement)
        {
            Panel.SetZIndex(TemplatedParent as UIElement, IsActive ? 1 : 0);
        }
    }

    private Visibility GetDesiredDraggerVisibility(BorderDragger dragger)
    {
        switch (dragger.Name)
        {
            case PartLeftDragger:
                return LeftDraggerVisibility;
            case PartRightDragger:
                return RightDraggerVisibility;
            case PartBottomDragger:
                return BottomDraggerVisibility;
            case PartTopDragger:
                return TopDraggerVisibility;
            case PartBottomLeftDragger:
                return BottomLeftDraggerVisibility;
            case PartTopLeftDragger:
                return TopLeftDraggerVisibility;
            case PartBottomRightDragger:
                return BottomRightDraggerVisibility;
            case PartTopRightDragger:
                return TopRightDraggerVisibility;
            default:
                return Visibility.Collapsed;
        }
    }

    [MirrorVerified("PtcControlsCoreRegionBorderMechanicalDifferentialTests")]
    public RegionBorder() { }
}
