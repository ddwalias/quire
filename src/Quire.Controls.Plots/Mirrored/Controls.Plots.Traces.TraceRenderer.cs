using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Plots.Presenter;
using Quire.Wpf;

namespace Quire.Controls.Plots.Traces;

internal abstract partial class TraceRenderer
{
    protected static readonly Dictionary<TraceSymbol, Symbol> _GeometryTransformationDictionary = CreateSymbolsGeometryDictionary();

    internal static readonly Dictionary<LineStyle, DashStyle> _TraceLineDictionary = CreateTraceLineDictionary();

    protected readonly IPlotTrace _ownerTrace;

    private const int StartNewCollectionAfter = 500;

    protected TraceRenderer(IPlotTrace ownerTrace)
    {
        _ownerTrace = ownerTrace;
    }

    public void Render(DrawingContext drawingContext)
    {
        OnRender(drawingContext);
    }

    protected abstract void OnRender(DrawingContext drawingContext);

    protected void DrawGeometry(DrawingContext drawingContext, DrawGeometryDelegate drawGeometryDelegate)
    {
        // The positive null test matches Mathcad's brfalse skip over the loop.
        if (ScreenPoints != null)
        {
            foreach (PointCollection screenPoint in ScreenPoints)
            {
                if (screenPoint != null && screenPoint.Count != 0)
                {
                    drawGeometryDelegate(drawingContext, screenPoint);
                }
            }
        }
    }

    private void CreateSymbols(DrawingContext drawingContext, PointCollection pointCollecion, TraceSymbol symbol)
    {
        Symbol symbolGeometry = _GeometryTransformationDictionary[symbol];
        if (symbolGeometry != null)
        {
            Pen symbolPen = SymbolPen;
            foreach (Point item in pointCollecion)
            {
                Brush brush = symbolGeometry.IsFilled ? FillBrush : null;
                drawingContext.DrawGeometry(brush, symbolPen, symbolGeometry.Create(item, _ownerTrace));
            }
        }
    }

    protected void DrawSymbolGeometry(DrawingContext drawingContext, TraceSymbol symbol)
    {
        DrawGeometryDelegate drawGeometryDelegate = (DrawingContext dc, PointCollection pointCollection) =>
        {
            CreateSymbols(dc, pointCollection, symbol);
        };
        DrawGeometry(drawingContext, drawGeometryDelegate);
    }

    private static Dictionary<TraceSymbol, Symbol> CreateSymbolsGeometryDictionary()
    {
        return new Dictionary<TraceSymbol, Symbol>
        {
            { TraceSymbol.rectangle, new RectangleSymbol() },
            { TraceSymbol.rhombus, new RhombusSymbol() },
            { TraceSymbol.star, new StarSymbol() },
            { TraceSymbol.circle, new CircleSymbol() },
            { TraceSymbol.x, new XSymbol() },
            { TraceSymbol.none, null },
            { TraceSymbol.point, new PointSymbol() },
            { TraceSymbol.plus, new PlusSymbol() },
            { TraceSymbol.filledRectangle, new FilledRectangleSymbol() },
            { TraceSymbol.filledRhombus, new FilledRhombusSymbol() },
            { TraceSymbol.triangle, new TriangleSymbol() },
            { TraceSymbol.filledTriangle, new FilledTriangleSymbol() },
            { TraceSymbol.filledStar, new FilledStarSymbol() }
        };
    }

    private static Dictionary<LineStyle, DashStyle> CreateTraceLineDictionary()
    {
        return new Dictionary<LineStyle, DashStyle>
        {
            { LineStyle.None, null },
            { LineStyle.Solid, DashStyles.Solid },
            { LineStyle.Dot, DashStyles.Dot },
            { LineStyle.Dash, DashStyles.Dash },
            { LineStyle.DashDot, DashStyles.DashDot },
            { LineStyle.DashDotDot, DashStyles.DashDotDot }
        };
    }

    protected Brush FillBrush => new SolidColorBrush(_ownerTrace.RenderColor);

    protected Pen LinePen => new Pen
    {
        Brush = FillBrush,
        Thickness = _ownerTrace.LineWeight,
        DashCap = PenLineCap.Round,
        DashStyle = _TraceLineDictionary[_ownerTrace.LineStyle],
        LineJoin = PenLineJoin.Bevel
    };

    protected Pen SymbolPen => new Pen(FillBrush, _ownerTrace.LineWeight);

    public IList<PointCollection> ScreenPoints { get; private set; }

    public bool HasPoints { get; private set; }

    private PointCollection LastScreenPointCollection => ScreenPoints[ScreenPoints.Count - 1];

    private void AddNewScreenPointCollection()
    {
        ScreenPoints.Add(new PointCollection());
    }

    public void SetScreenPoints()
    {
        HasPoints = false;
        IList<Point> plottingPoints = _ownerTrace.PlottingPoints;
        if (plottingPoints == null)
        {
            return;
        }
        ScreenPoints = new List<PointCollection>();
        AddNewScreenPointCollection();
        PlotGraphAxis.Point2ScreenHelper xPoint2ScreenHelper = _ownerTrace.XPoint2ScreenHelper;
        PlotGraphAxis.Point2ScreenHelper yPoint2ScreenHelper = _ownerTrace.YPoint2ScreenHelper;
        int count = plottingPoints.Count;
        for (int num = 0; num < count; num++)
        {
            Point point = plottingPoints[num];
            if (point.IsNaN())
            {
                if (LastScreenPointCollection.Count > 0)
                {
                    AddNewScreenPointCollection();
                }
            }
            else
            {
                Point val = new Point(xPoint2ScreenHelper.Point2Screen(point.X), yPoint2ScreenHelper.Point2Screen(point.Y));
                LastScreenPointCollection.Add(val);
                HasPoints = true;
                BreakPolylineIfItIsTooLong(plottingPoints, count, num, val);
            }
        }
    }

    private void BreakPolylineIfItIsTooLong(IList<Point> plottingPoints, int plottingPointsCount, int index, Point screenPoint)
    {
        if (LastScreenPointCollection.Count >= 500 && index + 1 < plottingPointsCount && !plottingPoints[index + 1].IsNaN())
        {
            AddNewScreenPointCollection();
            LastScreenPointCollection.Add(screenPoint);
        }
        TotalPointCount = ScreenPoints.Sum((PointCollection pc) => pc.Count);
    }

    protected int TotalPointCount { get; private set; }
}
