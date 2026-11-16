using System.Windows;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Traces;

internal partial class ErrorsTraceRenderer : ClustersTraceRenderer
{
    protected override double MaxSize => 10.0;

    public ErrorsTraceRenderer(IPlotTrace ownerTrace, HorizontalTraceOrderInfoProvider orderProvider)
        : base(ownerTrace, orderProvider)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (_ownerTrace.LineStyle != LineStyle.None)
        {
            DrawGeometry(drawingContext, CreateErrors);
        }
        DrawSymbolGeometry(drawingContext, _ownerTrace.Symbol);
    }

    private void CreateErrors(DrawingContext drawingContext, PointCollection pointCollection)
    {
        if (pointCollection.Count < 2)
        {
            return;
        }

        double start = GetStart(pointCollection[0].X);
        double center = start + ActualSize / 2.0;
        DrawHorizontalLine(drawingContext, new Point(start, pointCollection[0].Y));
        drawingContext.DrawLine(LinePen, new Point(center, pointCollection[0].Y), new Point(center, pointCollection[1].Y));
        DrawHorizontalLine(drawingContext, new Point(start, pointCollection[1].Y));
    }

    private void DrawHorizontalLine(DrawingContext drawingContext, Point startPoint)
    {
        Point endPoint = new Point(startPoint.X + ActualSize, startPoint.Y);
        drawingContext.DrawLine(LinePen, startPoint, endPoint);
    }
}
