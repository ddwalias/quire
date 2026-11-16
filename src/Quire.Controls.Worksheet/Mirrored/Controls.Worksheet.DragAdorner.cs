using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

internal partial class DragAdorner : Adorner
{
    private readonly UIElement _child;

    private Vector _borderOffset;

    public DragAdorner(UIElement owner, UIElement adornElement)
        : base(owner)
    {
        Rectangle rectangle = new Rectangle();
        Rect descendantBounds = WorksheetViewModel.ItemManager.GetDescendantBounds(adornElement);
        rectangle.Width = descendantBounds.Width;
        rectangle.Height = descendantBounds.Height;
        Size desiredSize = adornElement.DesiredSize;
        ICollapsibleAreaControl collapsibleAreaControl = adornElement as ICollapsibleAreaControl;
        if (collapsibleAreaControl != null)
        {
            if (collapsibleAreaControl.IsCollapsed)
            {
                desiredSize.Height = collapsibleAreaControl.AreaHeight;
            }

            descendantBounds.Width = desiredSize.Width;
        }

        _borderOffset = new Vector(descendantBounds.Left, descendantBounds.Top);
        try
        {
            VisualBrush brush = new VisualBrush(adornElement);
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)descendantBounds.Width, (int)descendantBounds.Height, 96.0, 96.0, PixelFormats.Pbgra32);
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(brush, null, new Rect(default(Point), descendantBounds.Size));
            }

            renderTargetBitmap.Render(drawingVisual);
            ImageBrush fill = new ImageBrush(renderTargetBitmap);
            rectangle.Fill = fill;
        }
        catch (Exception)
        {
            rectangle.Stroke = Brushes.LightGray;
        }

        _child = rectangle;
    }

    protected override Visual GetVisualChild(int index)
    {
        return _child;
    }

    protected override int VisualChildrenCount => 1;

    protected override Size MeasureOverride(Size finalSize)
    {
        _child.Measure(finalSize);
        return _child.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _child.Arrange(new Rect(_child.DesiredSize));
        return finalSize;
    }

    public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
    {
        return WpfUtils.GroupTransformsAsFrozen(new TranslateTransform(_borderOffset.X, _borderOffset.Y), base.GetDesiredTransform(transform));
    }
}
