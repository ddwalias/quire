using System.Windows.Media;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Traces;

internal partial class LinesTraceRenderer : TraceRenderer
{
    public LinesTraceRenderer(IPlotTrace ownerTrace)
        : base(ownerTrace)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        if (ScreenPoints == null || TotalPointCount < 1)
        {
            return;
        }

        bool hasAxes = _ownerTrace.XAxis != null && _ownerTrace.YAxis != null;
        if (hasAxes)
        {
            drawingContext.PushClip(_ownerTrace.ClippingGeometry);
        }

        if (_ownerTrace.LineStyle != LineStyle.None)
        {
            DrawGeometry(drawingContext, CreateLines);
        }

        bool flag = TotalPointCount == 1;
        TraceSymbol symbol = _ownerTrace.Symbol;
        if (flag && symbol == TraceSymbol.none)
        {
            symbol = TraceSymbol.x;
        }

        DrawSymbolGeometry(drawingContext, symbol);
        if (hasAxes)
        {
            drawingContext.Pop();
        }
    }

    private void CreateLines(DrawingContext drawingContext, PointCollection pointCollection)
    {
        PolyLineSegment polyLineSegment = new PolyLineSegment
        {
            Points = pointCollection
        };
        PathFigure figure = new PathFigure(polyLineSegment.Points[0], new PathSegmentCollection { polyLineSegment }, false);
        drawingContext.DrawGeometry(null, LinePen, new PathGeometry(new PathFigureCollection { figure }));
    }
}
