#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

internal abstract partial class Trace
{
    protected readonly int tagX;

    protected readonly int tagY;

    protected readonly Function lambdaX;

    protected readonly Function lambdaY;

    protected Dim dimX;

    protected Dim dimY;

    protected IEnumerator<Pair<object>> enumerator;

    private int count;

    public List<Pair<double>> Points = new List<Pair<double>>();

    public Dim DimX => dimX;

    public Dim DimY => dimY;

    public int Size
    {
        get
        {
            return count;
        }
        protected set
        {
            count = value;
            Points.Capacity = count + 10;
        }
    }

    public Trace(Spec spec, Dim xDim, Dim yDim)
    {
        lambdaX = spec.lambdaX;
        lambdaY = spec.lambdaY;
        tagX = spec.tagX;
        tagY = spec.tagY;
        if (Dim.Id_b(xDim))
        {
            xDim = null;
        }
        if (Dim.Id_b(yDim))
        {
            yDim = null;
        }
        dimX = xDim;
        dimY = yDim;
    }

    public static Trace Create(Spec spec, int traceOptions, Dim xDim, Dim yDim)
    {
        switch (spec.type)
        {
            case Spec.TraceType.Range:
                return new RangeTrace(spec, xDim, yDim);
            case Spec.TraceType.Vector:
                return new VectorTrace(spec, xDim, yDim);
            case Spec.TraceType.Quick:
                return new QuickTrace(spec, xDim, yDim, traceOptions != 0);
            default:
                AppException.Raise(new ArgumentException(), 0u);
                return null;
        }
    }

    public abstract void Process(Engine engine, int currentTrace);

    public virtual bool AddPoint(double x, double y, AxisOptions X, AxisOptions Y)
    {
        bool flag = Util.IsBad(x, X.isLog());
        bool flag2 = Util.IsBad(y, Y.isLog());
        x = (flag ? Util.NEG_MARKER : x);
        y = (flag2 ? Util.NEG_MARKER : y);
        Points.Add(new Pair<double>(x, y));
        if (!flag)
        {
            return !flag2;
        }
        return false;
    }

    public IEnumerator<Pair<object>> GetEnumerator()
    {
        return enumerator;
    }

    public double CheckValueX(object val)
    {
        return Util.CheckValue(val, tagX, ref dimX);
    }

    public double CheckValueY(object val)
    {
        return Util.CheckValue(val, tagY, ref dimY);
    }

    public partial struct Spec
    {
        public readonly object reserved;

        public readonly TraceType type;

        public readonly Function lambdaX;

        public readonly Function lambdaY;

        public readonly int tagX;

        public readonly int tagY;

        public Spec(TraceType t, object r, Function lx, Function ly, int tx, int ty)
        {
            reserved = r;
            type = t;
            lambdaX = lx;
            lambdaY = ly;
            tagX = tx;
            tagY = ty;
        }
    }
}
