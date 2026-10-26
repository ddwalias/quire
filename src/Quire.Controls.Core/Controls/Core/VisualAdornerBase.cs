#nullable disable
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Quire.Controls.Core;

public abstract partial class VisualAdornerBase : VisibilityAwareAdorner
{
    private static readonly Type _typeOfThis;

    public static readonly RoutedEvent AdornerAddedEvent;

    private bool _removed;

    private readonly TransformGroup _renderTransformGroup = new TransformGroup();

    protected AdornerLayer AdornedElementAdornerLayer => AdornerLayer.GetAdornerLayer(AdornedElement);

    protected AdornerLayer ParentAdornerLayer => VisualParent as AdornerLayer;

    protected FrameworkElement AdornedFrameworkElement => AdornedElement as FrameworkElement;

    public static void AddToAdornerLayer(AdornerDecorator adornerDecorator, Adorner adorner)
    {
        AdornerLayer adornerLayer = adornerDecorator.AdornerLayer;
        if (adornerLayer == null)
        {
            return;
        }
        if (adorner.Parent is AdornerLayer adornerLayer2)
        {
            if (adornerLayer2 == adornerLayer)
            {
                return;
            }
            adornerLayer2.Remove(adorner);
        }
        adornerLayer.Add(adorner);
    }

    static VisualAdornerBase()
    {
        _typeOfThis = typeof(VisualAdornerBase);
        AdornerAddedEvent = EventManager.RegisterRoutedEvent("AdornerAdded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), _typeOfThis);
        UIElement.RenderTransformProperty.OverrideMetadata(_typeOfThis, new PropertyMetadata(UIElement.RenderTransformProperty.DefaultMetadata.DefaultValue, (PropertyChangedCallback)null, (CoerceValueCallback)CoerceRenderTransformPropertyValue));
    }

    private static object CoerceRenderTransformPropertyValue(DependencyObject d, object basevalue)
    {
        if (!(d is VisualAdornerBase visualAdornerBase) || !(basevalue is Transform transform))
        {
            return basevalue;
        }
        return visualAdornerBase.UpdateRenderTransformGroup(transform);
    }

    protected VisualAdornerBase(UIElement adornedElement, bool trackVisibility)
        : base(adornedElement, trackVisibility)
    {
        RenderTransform = UpdateRenderTransformGroup(RenderTransform);
    }

    private Transform UpdateRenderTransformGroup(Transform transform)
    {
        if (transform == _renderTransformGroup)
        {
            return transform;
        }
        _renderTransformGroup.Children.Clear();
        _renderTransformGroup.Children.Add(transform);
        return _renderTransformGroup;
    }

    protected void FireAdornerAdded()
    {
        AdornedElement.RaiseEvent(new RoutedEventArgs(AdornerAddedEvent, this));
    }

    public virtual void Remove()
    {
        _removed = true;
        if (AdornedFrameworkElement != null)
        {
            AdornedFrameworkElement.Loaded -= AdornedElementLoadedHandler;
        }
        LayoutUpdated -= OnLayoutUpdated;
        if (ParentAdornerLayer != null)
        {
            ParentAdornerLayer.Remove(this);
        }
    }

    protected virtual void Add()
    {
        if (AdornedFrameworkElement == null || AdornedFrameworkElement.IsLoaded)
        {
            AddToAdornerLayer();
        }
        if (AdornedFrameworkElement != null)
        {
            AdornedFrameworkElement.Loaded += AdornedElementLoadedHandler;
        }
        LayoutUpdated += OnLayoutUpdated;
    }

    protected virtual void AddToAdornerLayer()
    {
        if (AdornedElementAdornerLayer == null || AdornedElementAdornerLayer == ParentAdornerLayer)
        {
            return;
        }
        if (ParentAdornerLayer != null)
        {
            ParentAdornerLayer.Remove(this);
        }
        AdornedElementAdornerLayer.Add(this);
    }

    private void AdornedElementLoadedHandler(object sender, RoutedEventArgs e)
    {
        if (_removed)
        {
            return;
        }
        AddToAdornerLayer();
    }

    private void OnLayoutUpdated(object sender, EventArgs args)
    {
        if (IsVisible && !IsArrangeValid && DebugTools.AssertNotNull(ParentAdornerLayer, null).Passed)
        {
            ParentAdornerLayer.InvalidateArrange();
        }
    }
}
