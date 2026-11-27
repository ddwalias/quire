using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;
using Quire.Core;
using Quire.Undo;

namespace Quire.Controls.Plots;

public partial class MarkersPanel
{
    private partial class DraggingScope : IDisposable
    {
        private readonly double _draggedMarkerHorizontalScreenValue = double.NaN;
        private readonly double _draggedMarkerVerticalScreenValue = double.NaN;
        private readonly FrameworkElement _draggedElemnt;
        private readonly MarkersPanel _markersPanel;
        private readonly CommandScope _commandScope;

        public DraggingScope(FrameworkElement draggedElement, MarkersPanel markersPanel)
        {
            _draggedElemnt = draggedElement;
            _markersPanel = markersPanel;
            double horizontalValue = GetHorizontalValue(_draggedElemnt);
            if (!double.IsNaN(horizontalValue))
            {
                _draggedMarkerHorizontalScreenValue = _markersPanel.HorizontalValueToScreen(horizontalValue);
            }
            double verticalValue = GetVerticalValue(_draggedElemnt);
            if (!double.IsNaN(verticalValue))
            {
                _draggedMarkerVerticalScreenValue = _markersPanel.VerticalValueToScreen(verticalValue);
            }
            _commandScope = new CommandScope("Drag Marker");
        }

        public void HandleDrag(Vector vector)
        {
            if (!double.IsNaN(_draggedMarkerHorizontalScreenValue))
            {
                SetHorizontalValue(_draggedElemnt, _markersPanel.HorizontalScreenToValue(_draggedMarkerHorizontalScreenValue + vector.X));
                UpdateBindingTanget(HorizontalValueProperty);
            }
            if (!double.IsNaN(_draggedMarkerVerticalScreenValue))
            {
                SetVerticalValue(_draggedElemnt, _markersPanel.VerticalScreenToValue(_draggedMarkerVerticalScreenValue + vector.Y));
                UpdateBindingTanget(VerticalValueProperty);
            }
        }

        private void UpdateBindingTanget(DependencyProperty dp)
        {
            BindingExpression bindingExpression = _draggedElemnt.GetBindingExpression(dp);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateTarget();
            }
        }

        public void Dispose()
        {
            _commandScope.Dispose();
        }
    }

    public const string PartMarkerEquationContainer = "PART_MarkerEquationContainer";

    private DraggingScope _draggingScope;

    public static readonly DependencyProperty HorizontalRangeHelperProperty = DependencyProperty.Register("HorizontalRangeHelper", typeof(IValueToScreen), typeof(MarkersPanel), new PropertyMetadata(HandleRangeHelperChanged));

    public static readonly DependencyProperty VerticalRangeHelperProperty = DependencyProperty.Register("VerticalRangeHelper", typeof(IValueToScreen), typeof(MarkersPanel), new PropertyMetadata(HandleRangeHelperChanged));

    public static readonly DependencyProperty VerticalValueProperty = DependencyProperty.RegisterAttached("VerticalValue", typeof(double), typeof(MarkersPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty HorizontalValueProperty = DependencyProperty.RegisterAttached("HorizontalValue", typeof(double), typeof(MarkersPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty EquationControlTransformProperty = DependencyProperty.RegisterAttached("EquationControlTransform", typeof(EquationControlWithTransform), typeof(MarkersPanel), new PropertyMetadata(EquationControlTransformChanged));

    public IValueToScreen HorizontalRangeHelper
    {
        get => (IValueToScreen)GetValue(HorizontalRangeHelperProperty);
        set => SetValue(HorizontalRangeHelperProperty, value);
    }

    public IValueToScreen VerticalRangeHelper
    {
        get => (IValueToScreen)GetValue(VerticalRangeHelperProperty);
        set => SetValue(VerticalRangeHelperProperty, value);
    }

    public MarkersPanel()
    {
        AddDragEventHandlers();
    }

    private void AddDragEventHandlers()
    {
        AddHandler(VisualDragger.DragStartEvent, new MouseEventHandler(OnDragStart));
        AddHandler(VisualDragger.DragEvent, new VisualDragger.DragHandler(OnDrag));
        AddHandler(VisualDragger.DragEndEvent, new RoutedEventHandler(OnDragEnd));
    }

    private void OnDragStart(object sender, MouseEventArgs e)
    {
        VisualDragger visualDragger = e.Source as VisualDragger;
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(visualDragger != null))
        {
            return;
        }
        DebugTools.DevAssert(_draggingScope == null);
        _draggingScope = new DraggingScope(visualDragger.DraggedElement, this);
    }

    private void OnDrag(object sender, Quire.Controls.Core.DragEventArgs e)
    {
        DebugTools.DevAssert(_draggingScope != null);
        _draggingScope.HandleDrag(e.DraggingVector);
    }

    private void OnDragEnd(object sender, RoutedEventArgs e)
    {
        DebugTools.DevAssert(_draggingScope != null);
        _draggingScope.Dispose();
        _draggingScope = null;
    }

    private double HorizontalValueToScreen(double value)
    {
        return HorizontalRangeHelper.CoerceScreenValue(HorizontalRangeHelper.ValueToScreen(value), toEdges: true);
    }

    private double VerticalValueToScreen(double value)
    {
        return VerticalRangeHelper.CoerceScreenValue(VerticalRangeHelper.ValueToScreen(value), toEdges: true);
    }

    private double HorizontalScreenToValue(double value)
    {
        return HorizontalRangeHelper.ScreenToValue(HorizontalRangeHelper.CoerceScreenValue(value, toEdges: false));
    }

    private double VerticalScreenToValue(double value)
    {
        return VerticalRangeHelper.ScreenToValue(VerticalRangeHelper.CoerceScreenValue(value, toEdges: false));
    }

    private static void HandleRangeHelperChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        MarkersPanel markersPanel = d as MarkersPanel;
        if (DebugTools.AssertNotNull(markersPanel, "Dependency object should be of type MarkersPanel").Failed)
        {
            return;
        }
        if (e.OldValue is IValueToScreen valueToScreen)
        {
            valueToScreen.ValueToScreenChanged -= markersPanel.HandleValueToScreenChanged;
        }
        if (e.NewValue is IValueToScreen newValueToScreen)
        {
            newValueToScreen.ValueToScreenChanged += markersPanel.HandleValueToScreenChanged;
        }
    }

    private void HandleValueToScreenChanged(object obj)
    {
        InvalidateArrange();
    }

    public static double GetVerticalValue(UIElement uiElement)
    {
        return (double)uiElement.GetValue(VerticalValueProperty);
    }

    public static void SetVerticalValue(UIElement uiElement, double verticalValue)
    {
        uiElement.SetValue(VerticalValueProperty, verticalValue);
    }

    public static double GetHorizontalValue(UIElement uiElement)
    {
        return (double)uiElement.GetValue(HorizontalValueProperty);
    }

    public static void SetHorizontalValue(UIElement uiElement, double horizontalValue)
    {
        uiElement.SetValue(HorizontalValueProperty, horizontalValue);
    }

    private static void EquationControlTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        EquationControl equationControl = d as EquationControl;
        EquationControlWithTransform equationControlWithTransform = e.NewValue as EquationControlWithTransform;
        if ((AssertionE)DebugTools.DevAssert(equationControlWithTransform != null && equationControl != null) == AssertionE.Passed)
        {
            equationControlWithTransform.Control = equationControl;
        }
    }

    public static EquationControlWithTransform GetEquationControlTransform(DependencyObject depObj)
    {
        return (EquationControlWithTransform)depObj.GetValue(EquationControlTransformProperty);
    }

    public static void SetEquationControlTransform(DependencyObject depObj, EquationControlWithTransform value)
    {
        depObj.SetValue(EquationControlTransformProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (UIElement child in Children)
        {
            if (child is FrameworkElement && !((FrameworkElement)child).IsLoaded)
            {
                using (new CommandScope(new CompoundCommand()))
                {
                    child.Measure(availableSize);
                }
            }
            else
            {
                child.Measure(availableSize);
            }
        }
        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (FrameworkElement child in Children)
        {
            double horizontalValue = GetHorizontalValue(child);
            double verticalValue = GetVerticalValue(child);
            bool flag = !double.IsNaN(horizontalValue);
            bool hasVerticalValue = !double.IsNaN(verticalValue);
            DebugTools.DevAssert(flag | hasVerticalValue);
            double num = GetActualX(horizontalValue, flag, child.HorizontalAlignment, child.DesiredSize);
            double verticalPosition = GetActualY(verticalValue, hasVerticalValue, child.VerticalAlignment, child.DesiredSize);
            Size val = child.DesiredSize;
            bool valid = num.IsValid() && verticalPosition.IsValid() && child.DesiredSize.Height.IsValid() && child.DesiredSize.Width.IsValid();
            if (DebugTools.DevAssert(valid).Failed)
            {
                num = 0.0;
                verticalPosition = 0.0;
                val = finalSize;
            }
            child.Arrange(new Rect(new Point(num, verticalPosition), val));
        }
        return base.ArrangeOverride(finalSize);
    }

    private double GetActualX(double horizontalValue, bool valueSet, HorizontalAlignment horizontalAlignment, Size desiredSize)
    {
        double result = 0.0;
        if (!valueSet || HorizontalRangeHelper == null)
        {
            return result;
        }
        double num = HorizontalValueToScreen(horizontalValue);
        switch (horizontalAlignment)
        {
            case HorizontalAlignment.Left:
                result = num - desiredSize.Width;
                break;
            case HorizontalAlignment.Center:
                result = num - desiredSize.Width / 2.0;
                break;
            case HorizontalAlignment.Right:
                result = num;
                break;
            default:
                DebugTools.DevAssert(false);
                break;
        }
        return result;
    }

    private double GetActualY(double verticalValue, bool valueSet, VerticalAlignment verticalAlignment, Size desiredSize)
    {
        double result = 0.0;
        if (!valueSet || VerticalRangeHelper == null)
        {
            return result;
        }
        double num = VerticalValueToScreen(verticalValue);
        switch (verticalAlignment)
        {
            case VerticalAlignment.Top:
                result = num - desiredSize.Height;
                break;
            case VerticalAlignment.Center:
                result = num - desiredSize.Height / 2.0;
                break;
            case VerticalAlignment.Bottom:
                result = num;
                break;
            default:
                DebugTools.DevAssert(false);
                break;
        }
        return result;
    }
}
