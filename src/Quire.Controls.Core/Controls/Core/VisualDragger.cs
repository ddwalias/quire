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


    [MirrorVerified("PtcControlsCoreVisualDraggerMechanicalDifferentialTests")]
    protected override void OnContextMenuOpening(ContextMenuEventArgs e) => e.Handled = true;

    [MirrorVerified("PtcControlsCoreVisualDraggerMechanicalDifferentialTests")]
    public void ResetDrag(Point point) => DragStartPoint = point;

    [MirrorVerified("PtcControlsCoreVisualDraggerMechanicalDifferentialTests")]
    public Point DragStartPoint { get; private set; }
}
