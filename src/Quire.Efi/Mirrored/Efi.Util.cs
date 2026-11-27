#nullable disable
#pragma warning disable

using System;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

internal static partial class Util
{

    internal static bool FuzzyEquals(double val, double tst)
    {
        if (!(tst < 1.0))
        {
            return Math.Abs(val - tst) / tst < 1E-323;
        }
        return Math.Abs(val - tst) < 1E-323;
    }

    internal static bool SameSign(double a, double b)
    {
        if (a != 0.0 && b != 0.0 && (!(a > 0.0) || !(b > 0.0)))
        {
            if (a < 0.0)
            {
                return b < 0.0;
            }
            return false;
        }
        return true;
    }

    internal static double Round(double val, double delta)
    {
        double floored = Math.Floor(val);
        if (val != floored)
        {
            if (val - floored < delta / 2.0)
            {
                return floored;
            }
            if (++floored - val < delta / 2.0)
            {
                return floored;
            }
        }
        return val;
    }

    internal static double ToDouble(object obj)
    {
        return ToDouble(obj, -1);
    }

    internal static object Evaluate(Function lambda)
    {
        try
        {
            return lambda.Call_0<object>();
        }
        catch
        {
            return VAL_NEG_MARKER;
        }
    }

    internal static object Evaluate(Function lambda, object arg)
    {
        try
        {
            return lambda.Call(arg);
        }
        catch
        {
            return VAL_NEG_MARKER;
        }
    }

    internal static object Evaluate(Function lambda, object[] args)
    {
        try
        {
            return lambda.Apply(args);
        }
        catch
        {
            return VAL_NEG_MARKER;
        }
    }

    internal static double Evaluate(Function lambda, double arg)
    {
        return ToDouble(Evaluate(lambda, new NumVal(arg)));
    }

    internal static double CheckValue(object val, int tag, ref Dim dim)
    {
        double result = NEG_MARKER;
        if (!(val is NumVal numVal))
        {
            if (val is MatVal)
            {
                AppException.Raise("must be scalar", (uint)tag);
            }
        }
        else if (IsPureReal(numVal))
        {
            result = numVal.Real;
        }
        CheckDim(val, tag, ref dim);
        return result;
    }

    internal static double CheckLimit(object o, bool isAuto, ref Dim dim)
    {
        if (isAuto)
        {
            return MARKER;
        }
        object[] array = o as object[];
        if (array == null || array.Length != 2)
        {
            AppException.Raise("unknown error");
        }
        int tag = (int)array[1];
        object limit = array[0];
        double result = Utl.as_real(limit, (uint)tag);
        CheckDim(limit, tag, ref dim);
        return result;
    }

    internal static bool IsFinite(double x)
    {
        if (!MatVal.IsPtrNan(ref x) && !double.IsInfinity(x))
        {
            return !double.IsNaN(x);
        }
        return false;
    }

    internal static bool IsBad(double real, bool log)
    {
        if ((!log || !(real <= 0.0)) && real != NEG_MARKER)
        {
            return !IsFinite(real);
        }
        return true;
    }

    internal static double Median(double[] values, int n)
    {
        Array.Sort(values, 0, n);
        return (values[(n + 1) / 2 - 1] + values[(n + 2) / 2 - 1]) / 2.0;
    }

    internal static void FixOutliers(double[] values, int n, ref double lowerBound, ref double upperBound)
    {
        FixOutliers(values, n, ref lowerBound, ref upperBound, out var _);
    }
}
