using System.Windows;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Traces;

internal partial class StemsTraceRenderer : TraceRenderer
{
    public StemsTraceRenderer(IPlotTrace ownerTrace)
        : base(ownerTrace)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        DrawSymbolGeometry(drawingContext, _ownerTrace.Symbol);
        DrawGeometry(drawingContext, DrawStems);
    }

    private void DrawStems(DrawingContext drawingcontext, PointCollection pointcollection)
    {
        foreach (Point current in pointcollection)
        {
            drawingcontext.DrawLine(LinePen, current, new Point(current.X, _ownerTrace.XAxis.Offset));
        }
    }
}
