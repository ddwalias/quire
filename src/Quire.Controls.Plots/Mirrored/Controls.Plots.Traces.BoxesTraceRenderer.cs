using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Traces;

internal partial class BoxesTraceRenderer : ClustersTraceRenderer
{
    protected override double MaxSize => 40.0;

    public BoxesTraceRenderer(IPlotTrace ownerTrace, HorizontalTraceOrderInfoProvider orderInfoProvider)
        : base(ownerTrace, orderInfoProvider)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        DrawGeometry(drawingContext, CreateBoxes);
    }

    private void CreateBoxes(DrawingContext drawingContext, PointCollection pointCollection)
    {
        if (pointCollection.Count < 5)
        {
            return;
        }
        double start = GetStart(pointCollection[0].X);
        double xMid = start + ActualSize / 2.0;
        DrawBox(drawingContext, new Point(start, pointCollection[0].Y), new Point(start + ActualSize, pointCollection[2].Y));
        DrawMedianLine(drawingContext, new Point(start, pointCollection[1].Y), new Point(start + ActualSize, pointCollection[1].Y));
        DrawWhisker(drawingContext, pointCollection, xMid, ActualSize);
        DrawOutliers(drawingContext, pointCollection, start, ActualSize);
    }

    private void DrawBox(DrawingContext drawingContext, Point point1, Point point2)
    {
        Pen pen = new Pen(Brushes.Black, _ownerTrace.LineWeight * 0.75);
        drawingContext.DrawRectangle(null, pen, new Rect(point1, point2));
    }

    private void DrawMedianLine(DrawingContext drawingContext, Point point1, Point point2)
    {
        Pen pen = new Pen(new SolidColorBrush(_ownerTrace.RenderColor), _ownerTrace.LineWeight);
        drawingContext.DrawLine(pen, point1, point2);
    }

    private void DrawWhisker(DrawingContext drawingContext, PointCollection pointCollection, double xMid, double width)
    {
        Pen pen = new Pen(Brushes.Black, _ownerTrace.LineWeight * 0.75)
        {
            DashStyle = LinePen.DashStyle,
            DashCap = PenLineCap.Round
        };
        Pen whiskerPen = new Pen(Brushes.Black, _ownerTrace.LineWeight * 0.75);
        double num = pointCollection[3].Y;
        double minimumY = pointCollection[0].Y;
        if (num > minimumY)
        {
            drawingContext.DrawLine(pen, new Point(xMid, num), new Point(xMid, minimumY));
            drawingContext.DrawLine(whiskerPen, new Point(xMid - width * 0.75 / 2.0, num), new Point(xMid + width * 0.75 / 2.0, num));
        }
        double maximumY = pointCollection[4].Y;
        double medianY = pointCollection[2].Y;
        if (maximumY < medianY)
        {
            drawingContext.DrawLine(pen, new Point(xMid, medianY), new Point(xMid, maximumY));
            drawingContext.DrawLine(whiskerPen, new Point(xMid - width * 0.75 / 2.0, maximumY), new Point(xMid + width * 0.75 / 2.0, maximumY));
        }
    }

    private void DrawOutliers(DrawingContext drawingContext, PointCollection pointCollection, double xStart, double width)
    {
        TraceSymbol key = ((_ownerTrace.Symbol == TraceSymbol.none) ? TraceSymbol.circle : _ownerTrace.Symbol);
        Symbol symbol = TraceRenderer._GeometryTransformationDictionary[key];
        Dictionary<double, int> dictionary = new Dictionary<double, int>();
        for (int i = 5; i < pointCollection.Count; i++)
        {
            if (dictionary.ContainsKey(pointCollection[i].Y))
            {
                dictionary[pointCollection[i].Y]++;
            }
            else
            {
                dictionary[pointCollection[i].Y] = 1;
            }
        }
        foreach (KeyValuePair<double, int> item in dictionary)
        {
            for (int j = 0; j < item.Value; j++)
            {
                double outlierX = xStart + width / (double)(item.Value + 1) * (double)(j + 1);
                drawingContext.DrawGeometry(null, SymbolPen, symbol.Create(new Point(outlierX, item.Key), _ownerTrace));
            }
        }
    }
}
