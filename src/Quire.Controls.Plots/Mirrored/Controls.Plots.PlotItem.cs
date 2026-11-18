#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Quire.Controls.Core;

namespace Quire.Controls.Plots;

public partial class PlotItem
{
    private static readonly Type _typeOfThis;
    internal const string DragBorder = "PART_DragBorder";
    protected bool _dragStarted;
    protected RegionBorder _border;
    public static readonly DependencyProperty MoveDraggerVisibilityProperty;
    public static readonly DependencyProperty IsBorderVisibleProperty;

    internal bool IsWidthExplicitlySet { get; set; }

    public Visibility MoveDraggerVisibility
    {
        get
        {
            return (Visibility)GetValue(MoveDraggerVisibilityProperty);
        }
        set
        {
            SetValue(MoveDraggerVisibilityProperty, value);
        }
    }

    public bool IsBorderVisible
    {
        get
        {
            return (bool)GetValue(IsBorderVisibleProperty);
        }
        set
        {
            SetValue(IsBorderVisibleProperty, value);
        }
    }

    public string Header { get; set; }

    public Visibility RightDraggerVisibility { get; set; }

    protected RegionBorder Border
    {
        get
        {
            return _border;
        }
        set
        {
            if (MoveDraggerVisibility == Visibility.Visible)
            {
                RegisterOrUnregisterFromBorderEvents(register: false);
            }
            _border = value;
            if (MoveDraggerVisibility == Visibility.Visible)
            {
                RegisterOrUnregisterFromBorderEvents(register: true);
            }
        }
    }

    internal UIElement VisualBrushRectangle => new Rectangle
    {
        Width = ActualWidth,
        Height = ActualHeight,
        Fill = new VisualBrush(this),
        Opacity = 0.8,
        IsHitTestVisible = false
    };

    public event Action<PlotItem> DragStarted;

    public event Action<PlotItem> DragEnded;

    static PlotItem()
    {
        _typeOfThis = typeof(PlotItem);
        IsBorderVisibleProperty = DependencyProperty.Register("IsBorderVisible", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false));
        MoveDraggerVisibilityProperty = RegionBorder.MoveDraggerVisibilityProperty.AddOwner(_typeOfThis, new PropertyMetadata(OnIsMoveableChanged));
    }

    protected static void OnIsMoveableChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs eArgs)
    {
        ((PlotItem)dObj).RegisterOrUnregisterFromBorderEvents((Visibility)eArgs.NewValue == Visibility.Visible);
    }

    protected void FireDragStarted()
    {
        if (DragStarted != null)
        {
            DragStarted(this);
        }
    }

    protected void FireDragEnded()
    {
        if (DragEnded != null)
        {
            DragEnded(this);
        }
    }

    public override void OnApplyTemplate()
    {
        Border = Template.FindName("PART_DragBorder", this) as RegionBorder;
    }

    protected void BorderMouseUp(object sender, MouseButtonEventArgs e)
    {
        _dragStarted = false;
    }

    protected void RequestMoveAndResize(object sender, ResizeEventMouseArgs resizeEventMouseArgs)
    {
        if (resizeEventMouseArgs.DragMode == DragMode.Move)
        {
            _dragStarted = true;
            resizeEventMouseArgs.Handled = true;
        }
    }

    protected void BorderPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_dragStarted && sender == Border)
        {
            _dragStarted = false;
            DataObject dataObject = new DataObject();
            dataObject.SetData(_typeOfThis.Name, this);
            Mouse.Capture(this);
            FireDragStarted();
            DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Move);
            FireDragEnded();
            Mouse.Capture(null);
        }
        base.OnPreviewMouseMove(e);
    }
}
