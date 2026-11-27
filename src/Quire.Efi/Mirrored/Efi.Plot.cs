#nullable disable
#pragma warning disable

using System;
using Quire.EfiMatrix;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

public static partial class Plot
{
    public static object cplot(int origin, int ioptions, int ixopt, int iyopt, int iy2opt, int[] yParams, int[] y2Params, object[] yTraces, object[] y2Traces, object[] _xlimits, object[] _ylimits, object[] _y2limits)
    {
        Engine engine = new Engine(new PlotOptions(ioptions), new AxisOptions(ixopt), new AxisOptions(iyopt), origin);
        bool flag = engine.Run(yTraces, yParams, _xlimits, _ylimits);
        Engine engine2 = new Engine(new PlotOptions(ioptions), new AxisOptions(ixopt), new AxisOptions(iy2opt), origin);
        if (y2Traces.Length != 0)
        {
            flag |= engine2.Run(y2Traces, y2Params, _xlimits, _y2limits);
        }
        if (!flag)
        {
            AppException.Raise("NoPointsToPlot");
        }
        return Engine.CreateOutput(engine, engine2);
    }

    public static object maketrace(int type, object x, Function lambdax, Function lambday, int xtag, int ytag)
    {
        return new Trace.Spec((Trace.Spec.TraceType)type, x, lambdax, lambday, xtag, ytag);
    }

    public static object logspace(object min, object max, object n)
    {
        double d = Utl.as_preal(min, 1u);
        double d2 = Utl.as_preal(max, 2u);
        DimVal obj = min as DimVal;
        DimVal dimVal = max as DimVal;
        Dim dim = Dim.Eq(obj.m_dim, dimVal.m_dim);
        uint num = Utl.as_pint(n, 3u);
        MatVal matVal = new MatVal((int)num);
        double value = Math.Log10(d);
        double otherValue = (Math.Log10(d2) - value) / (double)(num - 1);
        for (int i = 0; i < num; i++)
        {
            matVal.set_1(i, Math.Pow(10.0, (double)i * otherValue + value));
        }
        matVal.m_dim = dim;
        return matVal;
    }

    public static object logpts(object min, object decades, object n)
    {
        double num = Utl.as_real(min, 1u);
        uint value = Utl.as_pint(decades, 2u);
        uint otherValue = Utl.as_pint(n, 3u);
        MatVal matVal = new MatVal((int)(otherValue * value));
        for (int i = 0; i < value; i++)
        {
            double power = Math.Pow(10.0, num + (double)i);
            double nextValue = power * 9.0 / (double)otherValue;
            for (int j = 0; j < otherValue; j++)
            {
                matVal.set_1((int)(j + i * otherValue), power + (double)j * nextValue);
            }
        }
        matVal.m_dim = Dim.identity;
        return matVal;
    }

    public static object OpScaleInverse(object x, object y)
    {
        return values.op_scale_inverse(x, y);
    }

    public static Function InverseScaleFunction(Function f, object scale)
    {
        return values.inverse_scale_function(f, scale) as Function;
    }

    public static object plot3d(int origin, object exp, object scale, object limits, object gridn, object param)
    {
        object[] array = null;
        if (param != null)
        {
            MatVal matVal = param as MatVal;
            array = new object[matVal.Rows];
            for (int i = 0; i < matVal.Rows; i++)
            {
                array[i] = matVal.get_1(i);
            }
        }
        return plot3d_(origin, exp, scale, limits, gridn, array);
    }

    public static object ReduceData(object X, object Y, object P, object N)
    {
        MatVal xs = X as MatVal;
        MatVal ys = Y as MatVal;
        double num = Utl.as_nnreal(P, 3u);
        PlotTools plotTools = new PlotTools();
        if (N != null)
        {
            uint ncycles = Utl.as_pint(N, 4u);
            return plotTools.EliminateCollinearPoints(ref xs, ref ys, num, ncycles);
        }
        return plotTools.EliminateNearPoints(ref xs, ref ys, num);
    }

    public static object format_xy_plot(DimVal kind, DimVal xs, DimVal ys)
    {
        Plot2D plot2D = new Plot2D();
        return plot2D.format_xy_plot(kind, xs, ys);
    }

    public static object parametric_plot(DimVal xf, DimVal yf, DimVal points)
    {
        Plot2D plot2D = new Plot2D();
        return plot2D.parametric_plot(xf, yf, points);
    }

    public static object range_eval(Function f, object args)
    {
        Plot2D plot2D = new Plot2D();
        return plot2D.range_eval(f, args);
    }

    public static object contour(int origin, object data, object scale, object grid, object limits, object levels, object[] param)
    {
        Contour contourPlot = new Contour(origin, data, scale, limits, grid, levels, param);
        return contourPlot.plot_Contour();
    }

    public static object plot3d_(int origin, object exp, object scale, object limits, object gridn, object[] param)
    {
        Plot3D plot3D = new Plot3D(origin, exp, scale, limits, gridn, param);
        return plot3D.eval_3d_plot();
    }

}
