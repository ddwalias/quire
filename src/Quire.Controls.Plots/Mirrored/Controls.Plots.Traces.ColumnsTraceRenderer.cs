using System.Windows;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Traces;

internal partial class ColumnsTraceRenderer : ClustersTraceRenderer
{
    protected override double MaxSize => double.PositiveInfinity;

    public ColumnsTraceRenderer(IPlotTrace ownerTrace, HorizontalTraceOrderInfoProvider orderInfoProvider)
        : base(ownerTrace, orderInfoProvider)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        DrawGeometry(drawingContext, CreateColumns);
    }

    private void CreateColumns(DrawingContext drawingContext, PointCollection pointCollection)
    {
        foreach (Point item in pointCollection)
        {
            drawingContext.DrawGeometry(FillBrush, LinePen, GetColumnGeometry(item));
        }
    }

    private Geometry GetColumnGeometry(Point point)
    {
        Point val = new Point(GetStart(point.X), _ownerTrace.XAxis.Offset);
        PolyLineSegment polyLineSegment = new PolyLineSegment();
        polyLineSegment.Points.Add(val);
        polyLineSegment.Points.Add(new Point(val.X, point.Y));
        polyLineSegment.Points.Add(new Point(val.X + ActualSize, point.Y));
        polyLineSegment.Points.Add(new Point(val.X + ActualSize, val.Y));
        PathFigure value = new PathFigure(val, new PathSegmentCollection { polyLineSegment }, true);
        return new PathGeometry(new PathFigureCollection { value });
    }
}
