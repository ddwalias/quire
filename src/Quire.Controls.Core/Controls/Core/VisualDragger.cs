#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class VisualDragger
{
    public static readonly DependencyProperty DraggedElementProperty = DependencyProperty.Register("DraggedElement", typeof(FrameworkElement), typeof(VisualDragger));
    private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsDragging", typeof(bool), typeof(VisualDragger), new PropertyMetadata(false));
    public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
    public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(DraggingDirection), typeof(VisualDragger), new PropertyMetadata(DirectionPropertyChanged));
    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(DraggingMode), typeof(VisualDragger), new PropertyMetadata(DraggingMode.Margin));
    public static readonly RoutedEvent DragEndEvent = EventManager.RegisterRoutedEvent("DragEnd", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VisualDragger));
    public static readonly RoutedEvent DragStartEvent = EventManager.RegisterRoutedEvent("DragStart", RoutingStrategy.Bubble, typeof(MouseEventHandler), typeof(VisualDragger));
    public static readonly RoutedEvent DragEvent = EventManager.RegisterRoutedEvent("Drag", RoutingStrategy.Bubble, typeof(DragHandler), typeof(VisualDragger));
    static VisualDragger() => FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(VisualDragger), new FrameworkPropertyMetadata(typeof(VisualDragger)));

    public FrameworkElement DraggedElement
    {
        get => (FrameworkElement)GetValue(DraggedElementProperty);
        set => SetValue(DraggedElementProperty, value);
    }

    public bool IsDragging
    {
        get => (bool)GetValue(IsDraggingProperty);
        private set => SetValue(IsDraggingPropertyKey, value);
    }

    public DraggingDirection Direction
    {
        get => (DraggingDirection)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    public DraggingMode Mode
    {
        get => (DraggingMode)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public IInputElement ReferenceElement
    {
        get => _referenceElement ?? VisualTreeHelper.GetParent(DraggedElement) as IInputElement;
        set => _referenceElement = value;
    }

    public event RoutedEventHandler DragEnd
    {
        add => AddHandler(DragEndEvent, value);
        remove => RemoveHandler(DragEndEvent, value);
    }

    public event MouseEventHandler DragStart
    {
        add => AddHandler(DragStartEvent, value);
        remove => RemoveHandler(DragStartEvent, value);
    }

    public event DragHandler Drag
    {
        add => AddHandler(DragEvent, value);
        remove => RemoveHandler(DragEvent, value);
    }

    public VisualDragger()
    {
        DirectionPropertyChanged(this, new DependencyPropertyChangedEventArgs(DirectionProperty, null, DraggingDirection.Left));
    }

    private static void DirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var dragger = (VisualDragger)d;
        switch (dragger.Direction)
        {
            case DraggingDirection.Left:
            case DraggingDirection.Right:
                dragger.Cursor = Cursors.SizeWE;
                break;
            case DraggingDirection.Up:
            case DraggingDirection.Down:
                dragger.Cursor = Cursors.SizeNS;
                break;
            case DraggingDirection.UpLeft:
            case DraggingDirection.DownRight:
                dragger.Cursor = Cursors.SizeNWSE;
                break;
            case DraggingDirection.UpRight:
            case DraggingDirection.DownLeft:
                dragger.Cursor = Cursors.SizeNESW;
                break;
            case DraggingDirection.Move:
                dragger.Cursor = Cursors.SizeAll;
                break;
        }
    }


    [MirrorVerified("PtcControlsCoreVisualDraggerMechanicalDifferentialTests")]
    protected override void OnContextMenuOpening(ContextMenuEventArgs e) => e.Handled = true;

    [MirrorVerified("PtcControlsCoreVisualDraggerMechanicalDifferentialTests")]
    public void ResetDrag(Point point) => DragStartPoint = point;

    [MirrorVerified("PtcControlsCoreVisualDraggerMechanicalDifferentialTests")]
    public Point DragStartPoint { get; private set; }

    private void StartDrag(MouseEventArgs e)
    {
        if (DraggedElement == null)
        {
            return;
        }
        IInputElement referenceElement = ReferenceElement;
        DebugTools.DevAssert(referenceElement != null, null);
        Point position = e.GetPosition(referenceElement);
        DragStartPoint = position;
        IsDragging = CaptureMouse();
        if (Mode == DraggingMode.Margin)
        {
            _originalMargin = DraggedElement.Margin;
            _originalSize = new Size(DraggedElement.ActualWidth, DraggedElement.ActualHeight);
        }
        RaiseEvent(new MouseEventArgs(e.MouseDevice, e.Timestamp)
        {
            RoutedEvent = DragStartEvent,
            Source = this,
        });
    }

    private void StopDrag()
    {
        if (IsMouseCaptured)
        {
            ReleaseMouseCapture();
        }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (!IsDragging)
        {
            StartDrag(e);
        }
        e.Handled = true;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        StopDrag();
        e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (IsDragging)
        {
            IInputElement referenceElement = ReferenceElement;
            DebugTools.DevAssert(referenceElement != null, null);
            Point position = e.GetPosition(referenceElement);
            DragInternal(position - DragStartPoint);
            e.Handled = true;
        }
    }

    private void DragMoveMargin(Vector vector)
    {
        Thickness margin = _originalMargin;
        margin.Top += vector.Y;
        margin.Bottom -= vector.Y;
        if (margin.Top < 0)
        {
            margin.Bottom = _originalMargin.Bottom + _originalMargin.Top;
        }
        else if (margin.Bottom < 0)
        {
            margin.Top = _originalMargin.Top + _originalMargin.Bottom;
        }
        margin.Left += vector.X;
        margin.Right -= vector.X;
        if (margin.Left < 0)
        {
            margin.Right = _originalMargin.Right + _originalMargin.Left;
        }
        else if (margin.Right < 0)
        {
            margin.Left = _originalMargin.Left + _originalMargin.Right;
        }
        SetMargin(margin);
    }

    private void SetMargin(Thickness newMargin)
    {
        if (newMargin.Top < 0)
        {
            newMargin.Top = 0;
        }
        if (newMargin.Bottom < 0)
        {
            newMargin.Bottom = 0;
        }
        if (newMargin.Left < 0)
        {
            newMargin.Left = 0;
        }
        if (newMargin.Right < 0)
        {
            newMargin.Right = 0;
        }
        DraggedElement.Margin = newMargin;
    }

    private void CancelDrag()
    {
        // Guard clause: the IL tests DraggedElement first and returns early, not with a combined condition.
        if (DraggedElement == null)
        {
            return;
        }
        if (IsDragging)
        {
            if (Mode == DraggingMode.Margin)
            {
                DraggedElement.Margin = _originalMargin;
            }
            StopDrag();
        }
    }

    private void DragInternal(Vector vector)
    {
        switch (Mode)
        {
            case DraggingMode.Margin:
                if (Direction == DraggingDirection.Move)
                {
                    DragMoveMargin(vector);
                }
                else
                {
                    DragInDirection(vector);
                }
                break;
            case DraggingMode.Custom:
                switch (Direction)
                {
                    case DraggingDirection.Left:
                    case DraggingDirection.Right:
                        CustomDragHorizontal(vector);
                        break;
                    case DraggingDirection.Up:
                    case DraggingDirection.Down:
                        CustomDragVertical(vector);
                        break;
                    default:
                        CustomDrag(vector);
                        break;
                }
                break;
            default:
                DebugTools.DevAssert(false, null);
                break;
        }
    }

    private void CustomDrag(Vector vector)
    {
        RaiseEvent(new DragEventArgs(DragEvent, vector));
    }

    private void CustomDragHorizontal(Vector vector)
    {
        // The IL zeroes Y on a copy and still raises the original vector; keep that shape for the Release IL.
        var horizontal = vector;
        horizontal.Y = 0;
        CustomDrag(vector);
    }

    private void CustomDragVertical(Vector vector)
    {
        // The IL zeroes X on a copy and still raises the original vector; keep that shape for the Release IL.
        var vertical = vector;
        vertical.X = 0;
        CustomDrag(vector);
    }
}
