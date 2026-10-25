#nullable disable
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Core;

public partial class AnnotationArrowAdorner : VisibilityAwareAdorner
{
    public AnnotationArrowAdorner(FrameworkElement element)
        : base(element, trackVisibility: true)
    {
        ToolTip = element.FindResource("AnnotatedTooltip");
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        double height;
        double width;
        RegionBorder regionBorder;
        Size renderSize;
        Rect boundingRect;

        base.OnRender(drawingContext);
        regionBorder = RegionBorder.GetRegionBorder(AdornedElement);
        if (regionBorder == null)
        {
            renderSize = AdornedElement.RenderSize;
            width = renderSize.Width;
            renderSize = AdornedElement.RenderSize;
            height = renderSize.Height;
        }
        else
        {
            boundingRect = regionBorder.GetBoundingRect();
            width = boundingRect.Width;
            height = boundingRect.Height;
        }
        DrawAnnotationArrow(drawingContext, width, height);
    }

    private void DrawAnnotationArrow(DrawingContext drawingContext, double boundingRectWidth, double boundingRectHeight)
    {
        drawingContext.PushTransform(new TranslateTransform(boundingRectWidth, boundingRectHeight / 2.0));
        drawingContext.DrawGeometry(AnnotatedAdornerDefinitions.AdornerBrush, AnnotatedAdornerDefinitions.AdornerPen, AnnotatedAdornerDefinitions.ArrowBase);
        drawingContext.DrawGeometry(AnnotatedAdornerDefinitions.AdornerBrush, AnnotatedAdornerDefinitions.AdornerPen, AnnotatedAdornerDefinitions.ArrowHeadTop);
        drawingContext.DrawGeometry(AnnotatedAdornerDefinitions.AdornerBrush, AnnotatedAdornerDefinitions.AdornerPen, AnnotatedAdornerDefinitions.ArrowHeadBottom);
        drawingContext.Pop();
    }
}
