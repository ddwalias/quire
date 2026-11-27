#nullable disable
#pragma warning disable

using System.Collections.Generic;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

internal partial class Engine
{
    private static readonly double AXIS_FUZZ = 0.001;

    public readonly int origin;

    public readonly PlotOptions options;

    public readonly AxisOptions X;

    public readonly AxisOptions Y;

    private Trace[] traces = new Trace[0];

    private AxisStats xStats = new AxisStats();

    private YAxisStats yStats = new YAxisStats();

    private bool bHasGoodPoint;

    public Engine(PlotOptions popt, AxisOptions xopt, AxisOptions yopt, int orig)
    {
        X = xopt;
        Y = yopt;
        options = popt;
        origin = orig;
    }

    private void ScalePlot()
    {
        FixOutliers();
        if (!options.isPolar())
        {
            ScaleAxis(X, xStats, equalScales: false);
        }
        if (options.isPolar() || !options.equalScales())
        {
            ScaleAxis(Y, yStats, options.equalScales());
        }
    }

    private bool ProcessPoint(double x, double y, int nCurrentTrace)
    {
        if (!traces[nCurrentTrace].AddPoint(x, y, X, Y))
        {
            return false;
        }
        int nCurrentPoint = traces[nCurrentTrace].Points.Count - 1;
        xStats.Adjust(x, y, nCurrentTrace, nCurrentPoint);
        yStats.Adjust(x, y, nCurrentTrace, nCurrentPoint);
        return true;
    }

    private void GetCoordinates(int nCurrentTrace, Pair<object> args, out double x, out double y)
    {
        Trace trace = traces[nCurrentTrace];
        int count = trace.Points.Count;
        if (X.isCategory())
        {
            if (nCurrentTrace == 0)
            {
                if (xStats.labels == null)
                {
                    xStats.labels = new string[trace.Size];
                }
                xStats.labels[count] = args.x.ToString();
            }
            x = origin + count;
        }
        else
        {
            x = trace.CheckValueX(args.x);
        }
        if (Y.isCategory())
        {
            if (nCurrentTrace == 0)
            {
                if (yStats.labels == null)
                {
                    yStats.labels = new string[trace.Size];
                }
                yStats.labels[count] = args.y.ToString();
            }
            y = origin + count;
        }
        else
        {
            y = trace.CheckValueY(args.y);
        }
    }

    public int ProcessTrace(Trace trace, int nCurrentTrace)
    {
        int num = 0;
        foreach (Pair<object> item in trace)
        {
            GetCoordinates(nCurrentTrace, item, out var x, out var y);
            if (ProcessPoint(x, y, nCurrentTrace))
            {
                num++;
            }
        }
        bHasGoodPoint |= num > 0;
        return num;
    }

    public bool Run(object[] yTraces, int[] args, object[] xlimits, object[] ylimits)
    {
        Dim dim = null;
        X.ll = Util.CheckLimit(xlimits[0], X.isLLAuto(), ref dim);
        X.ul = Util.CheckLimit(xlimits[1], X.isULAuto(), ref dim);
        xStats.marker1 = Util.CheckLimit(xlimits[2], X.isM1Auto(), ref dim);
        xStats.marker2 = Util.CheckLimit(xlimits[3], X.isM2Auto(), ref dim);
        Dim dim2 = null;
        Y.ll = Util.CheckLimit(ylimits[0], Y.isLLAuto(), ref dim2);
        Y.ul = Util.CheckLimit(ylimits[1], Y.isULAuto(), ref dim2);
        yStats.marker1 = Util.CheckLimit(ylimits[2], Y.isM1Auto(), ref dim2);
        yStats.marker2 = Util.CheckLimit(ylimits[3], Y.isM2Auto(), ref dim2);
        int num = yTraces.Length;
        yStats.traces = num;
        traces = new Trace[num];
        bHasGoodPoint = false;
        for (int i = 0; i < num; i++)
        {
            traces[i] = Trace.Create((Trace.Spec)yTraces[i], args[i], dim, dim2);
            traces[i].Process(this, i);
        }
        if (!bHasGoodPoint)
        {
            AppException.Raise("NoPointsToPlot");
        }
        ScalePlot();
        if (X.ll != Util.MARKER)
        {
            xStats.lowerLimit = X.ll;
        }
        if (X.ul != Util.MARKER)
        {
            xStats.upperLimit = X.ul;
        }
        if (Y.ll != Util.MARKER)
        {
            yStats.lowerLimit = Y.ll;
        }
        if (Y.ul != Util.MARKER)
        {
            yStats.upperLimit = Y.ul;
        }
        return bHasGoodPoint;
    }

    private static MatVal CreatePointsVector(List<Pair<double>> points)
    {
        double[] array = new double[points.Count * 2];
        for (int i = 0; i < points.Count; i++)
        {
            array[2 * i] = points[i].x;
            array[2 * i + 1] = points[i].y;
        }
        return new MatVal(array);
    }

    private static MatVal CreateTraceVector(Trace trace)
    {
        MatVal matVal = new MatVal(4);
        if (trace is QuickTrace quickTrace)
        {
            matVal.set_1(0, quickTrace.Name);
        }
        else
        {
            matVal.set_1(0, 0.0);
        }
        matVal.set_1(1, new NumVal().dv(trace.DimX));
        matVal.set_1(2, new NumVal().dv(trace.DimY));
        matVal.set_1(3, CreatePointsVector(trace.Points));
        return matVal;
    }

    public static MatVal CreateOutput(Engine yPlot, Engine y2Plot)
    {
        int num = yPlot.traces.Length;
        int length = y2Plot.traces.Length;
        int value = num + length;
        MatVal matVal = new MatVal(value);
        for (int i = 0; i < value; i++)
        {
            Trace trace = ((i < num) ? yPlot.traces[i] : y2Plot.traces[i - num]);
            matVal.set_1(i, CreateTraceVector(trace));
        }
        AxisStats stats = yPlot.xStats;
        if (y2Plot.traces.Length != 0)
        {
            stats = AxisStats.Merge(yPlot.xStats, y2Plot.xStats);
        }
        MatVal x = CreateStatsVector(stats);
        MatVal x2 = CreateStatsVector(yPlot.yStats);
        MatVal x3 = CreateStatsVector(y2Plot.yStats);
        MatVal matVal2 = new MatVal(4);
        matVal2.set_1(0, x);
        matVal2.set_1(1, x2);
        matVal2.set_1(2, x3);
        matVal2.set_1(3, matVal);
        return matVal2;
    }
}
