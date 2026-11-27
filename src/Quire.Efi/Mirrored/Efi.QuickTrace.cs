#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

internal partial class QuickTrace
{
    public static readonly int QUICK_PLOT_POINTS = 1001;

    private double delta;

    private readonly bool parametric;

    private readonly string name;

    private static readonly int SAMPLE_POINTS = 4;

    private static readonly int SAMPLE_DEPTH = 3;

    public string Name => name;

    private bool IsParametric => parametric;

    public QuickTrace(Spec spec, Dim xDim, Dim yDim, bool parametric)
        : base(spec, xDim, yDim)
    {
        this.parametric = parametric;
        name = spec.reserved as string;
        Size = QUICK_PLOT_POINTS;
    }

    public override void Process(Engine engine, int currentTrace)
    {
        enumerator = PointEnumerator(engine);
        engine.ProcessTrace(this, currentTrace);
    }

    private bool IsValidAt(double t)
    {
        if (Util.IsBad(Util.Evaluate(lambdaX, t), log: false))
        {
            return false;
        }
        if (Util.IsBad(Util.Evaluate(lambdaY, t), log: false))
        {
            return false;
        }
        return true;
    }

    private int GetFirstValidX(double start, double inc, out double t)
    {
        for (int i = 0; i < Size; i++)
        {
            t = start + (double)i * inc;
            if (IsValidAt(t))
            {
                return i;
            }
        }
        t = 0.0;
        return -1;
    }

    private void CalculateDomain(Engine plot, out double xLow, out double xHigh)
    {
        bool flag = plot.options.isPolar();
        double num;
        if (plot.X.isLog())
        {
            num = (flag ? 0.1 : 1.0);
        }
        else
        {
            num = (flag ? 0.0 : (-10.0));
        }
        double value = (flag ? (Math.PI * 2.0) : 10.0);
        double otherValue = num;
        if (plot.X.ll != Util.MARKER)
        {
            otherValue = plot.X.ll;
        }
        double nextValue = value;
        if (plot.X.ul != Util.MARKER)
        {
            nextValue = plot.X.ul;
        }
        xLow = otherValue;
        xHigh = nextValue;
        if (flag || IsParametric)
        {
            if (otherValue > nextValue)
            {
                xLow = Math.Min(num, nextValue);
                xHigh = Math.Max(otherValue, value);
            }
            else
            {
                xLow = Math.Min(num, otherValue);
                xHigh = Math.Max(nextValue, value);
            }
        }
        AdjustDomain(plot.X, ref xLow, ref xHigh);
    }

    private static bool HasSingularity(ArraySegment<Pair<double>> points, Function lambda, int depth, ref double xsing)
    {
        xsing = (points.Array[points.Offset + 1].x + points.Array[points.Offset + 2].x) / 2.0;
        if (depth == 0)
        {
            return true;
        }
        double num = points.Array[points.Offset + 1].y - points.Array[points.Offset].y;
        double offset = points.Array[points.Offset + 2].y - points.Array[points.Offset + 1].y;
        double otherValue = points.Array[points.Offset + 3].y - points.Array[points.Offset + 2].y;
        if (otherValue == 0.0 || offset == 0.0 || num == 0.0 || !Util.SameSign(otherValue, num) || Util.SameSign(otherValue, offset))
        {
            return false;
        }
        NumVal a_ = new NumVal(2);
        try
        {
            double nextValue = Util.ToDouble(Calculus.NthDerivative(lambda, a_, new NumVal(points.Array[points.Offset + 2].x)));
            double b = Util.ToDouble(Calculus.NthDerivative(lambda, a_, new NumVal(points.Array[points.Offset + 1].x)));
            if (Util.SameSign(nextValue, b))
            {
                return false;
            }
            if (!Util.SameSign(num, b) || Util.SameSign(otherValue, nextValue))
            {
                return false;
            }
        }
        catch
        {
            return true;
        }
        int sAMPLE_POINTS = SAMPLE_POINTS;
        double currentValue = (points.Array[points.Offset + 2].x - points.Array[points.Offset + 1].x) / (double)(sAMPLE_POINTS + 1);
        int value = sAMPLE_POINTS + 4;
        Pair<double>[] array = new Pair<double>[value];
        array[0] = points.Array[points.Offset];
        array[1] = points.Array[points.Offset + 1];
        for (int i = 2; i < sAMPLE_POINTS + 2; i++)
        {
            double calculatedValue = (xsing = array[i - 1].x + currentValue);
            double sampleValue = Util.Evaluate(lambda, calculatedValue);
            if (sampleValue == Util.MARKER)
            {
                return true;
            }
            array[i] = new Pair<double>(calculatedValue, sampleValue);
            if (i > 2 && HasSingularity(new ArraySegment<Pair<double>>(array, i - 3, 4), lambda, depth - 1, ref xsing))
            {
                return true;
            }
        }
        array[value - 2] = points.Array[points.Offset + 2];
        if (HasSingularity(new ArraySegment<Pair<double>>(array, value - 5, 4), lambda, depth - 1, ref xsing))
        {
            return true;
        }
        array[value - 1] = points.Array[points.Offset + 3];
        if (HasSingularity(new ArraySegment<Pair<double>>(array, value - 4, 4), lambda, depth - 1, ref xsing))
        {
            return true;
        }
        return false;
    }

    public bool VerifySingularityY(double x)
    {
        Pair<double>[] array = new Pair<double>[4]
        {
            new Pair<double>(x - delta, Util.MARKER),
            new Pair<double>(x - delta * 0.5, Util.MARKER),
            new Pair<double>(x + delta * 0.5, Util.MARKER),
            new Pair<double>(x + delta, Util.MARKER)
        };
        if ((array[0].y = Util.Evaluate(lambdaY, array[0].x)) == Util.NEG_MARKER)
        {
            return false;
        }
        if ((array[1].y = Util.Evaluate(lambdaY, array[1].x)) == Util.NEG_MARKER)
        {
            return false;
        }
        if ((array[2].y = Util.Evaluate(lambdaY, array[2].x)) == Util.NEG_MARKER)
        {
            return false;
        }
        if ((array[3].y = Util.Evaluate(lambdaY, array[3].x)) == Util.NEG_MARKER)
        {
            return false;
        }
        double xsing = Util.MARKER;
        return HasSingularityY(new List<Pair<double>>(array), 3, ref xsing);
    }

    public bool VerifySingularityX(double y)
    {
        Pair<double>[] array = new Pair<double>[4]
        {
            new Pair<double>(Util.MARKER, y - delta),
            new Pair<double>(Util.MARKER, y - delta * 0.5),
            new Pair<double>(Util.MARKER, y + delta * 0.5),
            new Pair<double>(Util.MARKER, y + delta)
        };
        if ((array[0].x = Util.Evaluate(lambdaX, array[0].y)) == Util.NEG_MARKER)
        {
            return false;
        }
        if ((array[1].x = Util.Evaluate(lambdaX, array[1].y)) == Util.NEG_MARKER)
        {
            return false;
        }
        if ((array[2].x = Util.Evaluate(lambdaX, array[2].y)) == Util.NEG_MARKER)
        {
            return false;
        }
        if ((array[3].x = Util.Evaluate(lambdaX, array[3].y)) == Util.NEG_MARKER)
        {
            return false;
        }
        double ysing = Util.MARKER;
        return HasSingularityX(new List<Pair<double>>(array), 3, ref ysing);
    }
}
