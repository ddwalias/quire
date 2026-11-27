#nullable disable
#pragma warning disable

using System;

namespace Quire.Efi;

internal partial class LevelFormat
{
    public LevelFormat()
    {
    }

    internal void CalculateBetterTickmarks(ref double start, ref double end, ref int num_levels)
    {
        int n = num_levels - 1;
        if (start < 0.0 && end > 0.0)
        {
            CalculateBetterTickmarksWithZeroInRange(ref start, ref end, ref n);
        }
        else
        {
            CalculateBetterTickmarksWhenZeroIsNotInRange(ref start, ref end, ref n);
        }
        num_levels = n + 1;
    }

    private static void CalculateBetterTickmarksWhenZeroIsNotInRange(ref double start, ref double end, ref int n)
    {
        double num = (end - start) / (double)n;
        double value = OrderFunction(num);
        num = RoundWithOrder(num, 0.5 * value);
        if (num == 0.0)
        {
            num = 0.5 * value;
        }
        start = FloorWithOrder(start, 0.5 * value);
        end = CeilingWithOrder(end, 0.5 * value);
        n = (int)CeilingWithOrder((end - start) / num, 1.0);
        end = start + (double)n * num;
    }

    private static void CalculateBetterTickmarksWithZeroInRange(ref double start, ref double end, ref int n)
    {
        double num = (-start) / end;
        int val = (int)Math.Round((double)n * num / (1.0 + num));
        val = Math.Max(val, 1);
        val = Math.Min(val, n - 1);
        int value = n - val;
        double otherValue = OrderFunction((-start) / (double)val);
        double nextValue = OrderFunction(end / (double)value);
        double currentValue = Math.Min(otherValue, nextValue);
        double calculatedValue = RoundWithOrder((-start) / (double)val, 0.5 * otherValue);
        double sampleValue = RoundWithOrder(end / (double)value, 0.5 * nextValue);
        double limitValue = Math.Max(calculatedValue, sampleValue);
        if (limitValue == 0.0)
        {
            limitValue = 0.5 * currentValue;
        }
        start = FloorWithOrder(start, 0.5 * currentValue);
        end = CeilingWithOrder(end, 0.5 * currentValue);
        val = (int)CeilingWithOrder((-start) / limitValue, 1.0);
        value = (int)CeilingWithOrder(end / limitValue, 1.0);
        n = val + value;
        start = (double)(-val) * limitValue;
        end = (double)value * limitValue;
    }

    private static double OrderFunction(double step)
    {
        return Math.Pow(10.0, Math.Floor(Math.Log10(step)));
    }

    private static double RoundWithOrder(double exp, double order)
    {
        return Math.Round(exp / order) * order;
    }

    private static double CeilingWithOrder(double exp, double order)
    {
        return Math.Ceiling(exp / order) * order;
    }

    private static double FloorWithOrder(double exp, double order)
    {
        return Math.Floor(exp / order) * order;
    }
}
