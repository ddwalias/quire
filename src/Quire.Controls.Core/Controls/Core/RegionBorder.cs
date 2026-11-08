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

    [MirrorVerified("PtcControlsCoreRegionBorderMechanicalDifferentialTests")]
    public RegionBorder() { }
}
