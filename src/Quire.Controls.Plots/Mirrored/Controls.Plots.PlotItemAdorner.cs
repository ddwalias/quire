using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Quire.Wpf;

namespace Quire.Controls.Plots;

internal partial class PlotItemAdorner : Adorner
{
    private readonly ContentPresenter _plotItemContentPresenter;

    private readonly AdornerLayer _adornerLayer;

    private double _left;

    private double _top;

    protected override int VisualChildrenCount => 1;

    public double Left
    {
        get
        {
            return _left;
        }
        set
        {
            _left = value;
            UpdatePosition();
        }
    }

    public double Top
    {
        get
        {
            return _top;
        }
        set
        {
            _top = value;
            UpdatePosition();
        }
    }

    public PlotItemAdorner(UIElement PlotItemVisualBrushRectangle, UIElement _adorneredElement)
        : base(_adorneredElement)
    {
        _adornerLayer = AdornerLayer.GetAdornerLayer(_adorneredElement);
        _plotItemContentPresenter = new ContentPresenter();
        _plotItemContentPresenter.Content = PlotItemVisualBrushRectangle;
        _plotItemContentPresenter.IsHitTestVisible = false;
    }

    protected override Visual GetVisualChild(int index)
    {
        return _plotItemContentPresenter;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        _plotItemContentPresenter.Measure(constraint);
        return _plotItemContentPresenter.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _plotItemContentPresenter.Arrange(new Rect(finalSize));
        return finalSize;
    }

    private void UpdatePosition()
    {
        if (AdornedElement != null)
        {
            _adornerLayer.Update(AdornedElement);
        }
    }

    public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
    {
        return WpfUtils.GroupTransformsAsFrozen(new TranslateTransform(Left, Top), base.GetDesiredTransform(transform));
    }
}
