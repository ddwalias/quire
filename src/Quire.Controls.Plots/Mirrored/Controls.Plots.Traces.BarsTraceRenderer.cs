using System.Windows;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Traces;

internal partial class BarsTraceRenderer : ClustersTraceRenderer
{
    protected override double MaxSize => double.PositiveInfinity;

    public BarsTraceRenderer(IPlotTrace ownerTrace, VerticalTraceOrderInfoProvider orderInfoProvider)
        : base(ownerTrace, orderInfoProvider)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        DrawGeometry(drawingContext, CreateBars);
    }

    private void CreateBars(DrawingContext drawingContext, PointCollection pointCollection)
    {
        foreach (Point item in pointCollection)
        {
            drawingContext.DrawGeometry(FillBrush, new Pen(FillBrush, 0.75), GetBarGeometry(item));
        }
    }

    private Geometry GetBarGeometry(Point point)
    {
        Point val = new Point(_ownerTrace.YAxis.Offset, GetStart(point.Y));
        PolyLineSegment polyLineSegment = new PolyLineSegment();
        polyLineSegment.Points.Add(val);
        polyLineSegment.Points.Add(new Point(point.X, val.Y));
        polyLineSegment.Points.Add(new Point(point.X, val.Y + ActualSize));
        polyLineSegment.Points.Add(new Point(val.X, val.Y + ActualSize));
        PathFigure value = new PathFigure(val, new PathSegmentCollection { polyLineSegment }, true);
        return new PathGeometry(new PathFigureCollection { value });
    }
}
