#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Quire.Efi.CalculationAssistant;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

public partial class Plot2D
{
    private partial class RangeEnumerator : IEnumerator<double>, IEnumerator, IDisposable
    {
        private readonly RngVal m_range;

        private readonly int count;

        private int step;

        public double Current => m_range.get(step);

        object IEnumerator.Current => Current;

        public RangeEnumerator(RngVal range)
        {
            m_range = range;
            count = range.Rsize;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            return ++step < count;
        }

        public void Reset()
        {
            step = 0;
        }
    }

    private partial struct arg_desc
    {
        public int m_pos;

        public RngVal m_rng;

        public int rsize;

        public int m_idx;

        public void FetchInto(object[] args, int i)
        {
            args[m_pos] = m_rng.range_get(i);
        }

        public void Restore(object[] args)
        {
            args[m_pos] = m_rng;
        }
    }

    private partial struct args
    {
        public object[] argv;

        public arg_desc[] rngs;

        public int nrngs;

        public int jsize;

        public bool singleton;

        public object arg
        {
            get
            {
                if (!singleton)
                {
                    return argv;
                }
                return argv[0];
            }
        }

        public args(object v)
        {
            argv = v as object[];
            if (argv == null)
            {
                singleton = true;
                argv = new object[1] { v };
            }
            else
            {
                singleton = false;
            }
            int num = argv.Length;
            jsize = 1;
            rngs = null;
            nrngs = 0;
            for (int i = 0; i < num; i++)
            {
                if (argv[i] is RngVal rngVal)
                {
                    if (nrngs == 0)
                    {
                        rngs = new arg_desc[num - i];
                    }
                    rngs[nrngs].m_rng = rngVal;
                    rngs[nrngs].m_pos = i;
                    rngs[nrngs].m_idx = 0;
                    rngs[nrngs].rsize = rngVal.Rsize;
                    jsize *= rngVal.Rsize;
                    nrngs++;
                }
            }
        }

        public bool Incr()
        {
            int num = 0;
            do
            {
                if (++rngs[num].m_idx < rngs[num].rsize)
                {
                    rngs[num].FetchInto(argv, rngs[num].m_idx);
                    return true;
                }
                rngs[num].m_idx = 0;
                rngs[num].FetchInto(argv, 0);
            }
            while (++num < nrngs);
            return false;
        }

        public void Init()
        {
            for (int i = 0; i < nrngs; i++)
            {
                rngs[i].FetchInto(argv, 0);
            }
        }

        public void Restore()
        {
            for (int i = 0; i < nrngs; i++)
            {
                rngs[i].Restore(argv);
            }
        }
    }

    public double no_min_max;

    public double default_lo;

    public double default_hi;

    public int default_n;

    public Plot2D()
    {
        no_min_max = 0.0;
        default_lo = -10.0;
        default_hi = 10.0;
        default_n = 1000;
    }

    public RngVal plot_range(double lo, double hi, int n)
    {
        return RngVal.IncrRange(lo, (hi - lo) / (double)(n - 1), hi, Dim.identity);
    }

    private RngVal plot_range(double lo, double hi)
    {
        return plot_range(lo, hi, default_n);
    }

    private RngVal plot_range()
    {
        return plot_range(default_lo, default_hi, default_n);
    }

    private RngVal plot_range(int n)
    {
        return plot_range(default_lo, default_hi, n);
    }

    public RngVal plot_range(object[] args)
    {
        switch (args.Length)
        {
            case 0:
                return plot_range();
            case 1:
                return plot_range(((DimVal)args[0]).asInt(3u));
            case 2:
                return plot_range(((DimVal)args[0]).asReal(3u), ((DimVal)args[1]).asReal(4u));
            case 3:
                return plot_range(((DimVal)args[0]).asReal(3u), ((DimVal)args[1]).asReal(4u), ((DimVal)args[2]).asInt(5u));
            default:
                AppException.Raise((args.Length == 1) ? "too few arguments" : "too many arguments");
                return null;
        }
    }

    private object parametric_quickplot(object[] args)
    {
        switch (args.Length)
        {
            case 2:
                return parametric_quickplot((DimVal)args[0], (DimVal)args[1]);
            case 3:
                return parametric_plot((DimVal)args[0], (DimVal)args[1], (DimVal)args[2]);
            case 4:
                return parametric_quickplot((DimVal)args[0], (DimVal)args[1], ((DimVal)args[2]).asReal(3u), ((DimVal)args[3]).asReal(4u));
            case 5:
                return parametric_quickplot((DimVal)args[0], (DimVal)args[1], ((DimVal)args[2]).asReal(3u), ((DimVal)args[3]).asReal(4u), ((DimVal)args[4]).asInt(5u));
            default:
                AppException.Raise((args.Length < 2) ? "too few arguments" : "too many arguments");
                return null;
        }
    }

    private object parametric_quickplot(DimVal xf, DimVal yf)
    {
        return parametric_plot(xf, yf, plot_range());
    }

    private object parametric_quickplot(DimVal xf, DimVal yf, double lo, double hi)
    {
        return parametric_plot(xf, yf, plot_range(lo, hi));
    }

    private object parametric_quickplot(DimVal xf, DimVal yf, double lo, double hi, int n)
    {
        return parametric_plot(xf, yf, plot_range(lo, hi, n));
    }

    public object format_xy_plot(DimVal kind, DimVal xs, DimVal ys)
    {
        MatVal x = xs.asMatrix(1u);
        MatVal y = ys.asMatrix(2u);
        nan_fill(ref x, ref y);
        int rows = x.Rows;
        int cols = x.Cols;
        int cols2 = y.Cols;
        double num = double.PositiveInfinity;
        double num2 = double.NegativeInfinity;
        double num3 = double.PositiveInfinity;
        double num4 = double.NegativeInfinity;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                double r = x.get_real(i, j);
                if (r > num2)
                {
                    num2 = r;
                }
                if (r < num)
                {
                    num = r;
                }
                if (MatVal.IsPtrNan(ref r))
                {
                    object obj = x.get_2(i, j);
                    if (obj is RngVal)
                    {
                        x.set_2(i, j, MatVal.Singleton(NumVal.ZeroId));
                    }
                    else if (obj is MatVal)
                    {
                        AppException.Raise("may not be nested");
                    }
                }
            }
            for (int k = 0; k < cols2; k++)
            {
                double r2 = y.get_real(i, k);
                if (r2 > num4)
                {
                    num4 = r2;
                }
                if (r2 < num3)
                {
                    num3 = r2;
                }
                if (MatVal.IsPtrNan(ref r2))
                {
                    object obj2 = y.get_2(i, k);
                    if (obj2 is RngVal)
                    {
                        y.set_2(i, k, MatVal.Singleton(NumVal.ZeroId));
                    }
                    else if (obj2 is MatVal)
                    {
                        AppException.Raise("may not be nested");
                    }
                }
            }
        }
        MatVal matVal = new MatVal(9);
        matVal.m_dim = Dim.identity;
        Dim d = x.DivideByDefaultDimension();
        Dim d2 = y.DivideByDefaultDimension();
        matVal.set_ptrs_1(0, x);
        matVal.set_ptrs_1(1, y);
        matVal.set_1(2, (num == double.PositiveInfinity) ? no_min_max : num);
        matVal.set_1(3, (num2 == double.NegativeInfinity) ? no_min_max : num2);
        matVal.set_1(4, 1.0, d);
        matVal.set_1(5, (num3 == double.PositiveInfinity) ? no_min_max : num3);
        matVal.set_1(6, (num4 == double.NegativeInfinity) ? no_min_max : num4);
        matVal.set_1(7, 1.0, d2);
        matVal.set_1(8, kind);
        return matVal;
    }

    private void eval(Function fn, DimVal p, int i, MatVal xres, ref double xmin, ref double xmax)
    {
        try
        {
            object x = fn.DynCall(p);
            xres.set_1(i, x);
            double num = xres.get_real(i);
            if (num > xmax)
            {
                xmax = num;
            }
            if (num < xmin)
            {
                xmin = num;
            }
        }
        catch (Exception)
        {
            xres.set_1(i, NumVal.NaN);
        }
    }

    private object ev(Function f, DimVal p)
    {
        try
        {
            return f.DynCall(p);
        }
        catch (Exception)
        {
            return NumVal.mcNaN;
        }
    }

    public object eval_parametric_plot(uint x_tag, DimVal xf, DimVal yf, DimVal points)
    {
        Function function = xf.asFunctionN(1, 1u);
        Function function2 = yf.asFunctionN(1, 2u);
        int isize = points.Isize;
        MatVal xres = MatVal.reserve(isize);
        MatVal yres = MatVal.reserve(isize);
        if (!try_calc_mt_values(function, function2, 0, isize, points, ref xres, ref yres))
        {
            if (Rts.IsCancellationRequested())
            {
                throw new AbortRequestedException();
            }
            for (int i = 0; i < isize; i++)
            {
                DimVal p = points.iterator_get(i);
                xres.PushRaw(ev(function, p));
                yres.PushRaw(ev(function2, p));
            }
        }
        if (isIdentityFunction(function) && !isIdentityFunction(function2))
        {
            massageData(x_tag, ref yres, ref xres, function2);
        }
        else if (isIdentityFunction(function2) && !isIdentityFunction(function))
        {
            massageData(x_tag, ref xres, ref yres, function);
        }
        else
        {
            massageData(x_tag, ref yres, ref xres, function2);
            massageData(x_tag, ref xres, ref yres, function);
        }
        return MatVal.Triple(new StrVal("parametric"), xres, yres);
    }

    private object[] BuildArguments(IEnumerator<double>[] arguments)
    {
        object[] array = new object[arguments.Length];
        for (int i = 0; i < arguments.Length; i++)
        {
            array[i] = new NumVal(arguments[i].Current);
        }
        return array;
    }

    private IEnumerator<object[]> ArgumentEnumerator(object[] ranges)
    {
        System.Diagnostics.Trace.Assert(ranges != null, "ranges != null");
        IEnumerator<double>[] array = new IEnumerator<double>[ranges.Length];
        int num = 1;
        for (int i = 0; i < ranges.Length; i++)
        {
            RngVal rngVal = ranges[i] as RngVal;
            if (rngVal == null)
            {
                AppException.Raise("too many range variables");
            }
            num *= rngVal.Rsize;
            array[i] = new RangeEnumerator(rngVal);
        }
        return MultiRangeEnumerator(array);
    }

    private void nan_fill(ref MatVal x, ref MatVal y)
    {
        int rows = x.Rows;
        int cols = x.Cols;
        int rows2 = y.Rows;
        int cols2 = y.Cols;
        if (rows == rows2)
        {
            return;
        }
        if (rows2 < rows)
        {
            y = y.ReAllocate(rows, cols2);
            for (int i = 0; i < cols2; i++)
            {
                for (int j = rows2; j < rows; j++)
                {
                    y.set_2(j, i, double.NaN);
                }
            }
            return;
        }
        x = x.ReAllocate(rows2, cols);
        for (int k = 0; k < cols; k++)
        {
            for (int l = rows; l < rows2; l++)
            {
                x.set_2(l, k, double.NaN);
            }
        }
    }

    public bool isIdentityFunction(Function fn)
    {
        try
        {
            if ((ev(fn, new NumVal(-123.4)) as NumVal).asReal() != -123.4)
            {
                return false;
            }
            if ((ev(fn, new NumVal(0)) as NumVal).asReal() != 0.0)
            {
                return false;
            }
            if ((ev(fn, new NumVal(567.8)) as NumVal).asReal() != 567.8)
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    public object rangeize(Function f, object[] args)
    {
        int nrngs;
        return rangeize(f, args, out nrngs);
    }

    public object rangeize(Function f, object[] args, out int nrngs)
    {
        int num = args.Length;
        arg_desc[] array = null;
        nrngs = 0;
        for (int i = 0; i < num; i++)
        {
            if (args[i] is RngVal rngVal)
            {
                if (nrngs == 0)
                {
                    array = new arg_desc[num - i];
                }
                else if (rngVal != array[0].m_rng)
                {
                    AppException.Raise("too many range variables");
                }
                array[nrngs].m_rng = rngVal;
                array[nrngs].m_pos = i;
                array[nrngs].m_idx = 0;
                array[nrngs].rsize = rngVal.Rsize;
                nrngs++;
            }
        }
        if (nrngs == 0)
        {
            return f.Apply(args);
        }
        MatVal matVal = MatVal.reserve(array[0].rsize);
        for (int j = 0; j < array[0].rsize; j++)
        {
            for (int k = 0; k < nrngs; k++)
            {
                array[k].FetchInto(args, j);
            }
            object obj = null;
            try
            {
                obj = f.Apply(args);
            }
            catch (Exception)
            {
                obj = NumVal.mcNaN;
            }
            matVal.PushRaw1(obj);
        }
        for (int l = 0; l < nrngs; l++)
        {
            array[l].Restore(args);
        }
        return matVal;
    }

    public object range_eval(Function f, object args)
    {
        f.AssertFullApp(1);
        if (args == null)
        {
            return f.Call(args);
        }
        args args2 = new args(args);
        if (args2.nrngs == 0)
        {
            return f.Call(args);
        }
        MatVal matVal = MatVal.reserve(args2.jsize);
        args2.Init();
        do
        {
            matVal.PushRaw(f.Call(args2.arg));
        }
        while (args2.Incr());
        args2.Restore();
        return matVal;
    }
}
