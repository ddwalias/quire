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
        double num2 = OrderFunction(num);
        num = RoundWithOrder(num, 0.5 * num2);
        if (num == 0.0)
        {
            num = 0.5 * num2;
        }
        start = FloorWithOrder(start, 0.5 * num2);
        end = CeilingWithOrder(end, 0.5 * num2);
        n = (int)CeilingWithOrder((end - start) / num, 1.0);
        end = start + (double)n * num;
    }

    private static void CalculateBetterTickmarksWithZeroInRange(ref double start, ref double end, ref int n)
    {
        double num = (-start) / end;
        int val = (int)Math.Round((double)n * num / (1.0 + num));
        val = Math.Max(val, 1);
        val = Math.Min(val, n - 1);
        int num2 = n - val;
        double num3 = OrderFunction((-start) / (double)val);
        double num4 = OrderFunction(end / (double)num2);
        double num5 = Math.Min(num3, num4);
        double val2 = RoundWithOrder((-start) / (double)val, 0.5 * num3);
        double val3 = RoundWithOrder(end / (double)num2, 0.5 * num4);
        double num6 = Math.Max(val2, val3);
        if (num6 == 0.0)
        {
            num6 = 0.5 * num5;
        }
        start = FloorWithOrder(start, 0.5 * num5);
        end = CeilingWithOrder(end, 0.5 * num5);
        val = (int)CeilingWithOrder((-start) / num6, 1.0);
        num2 = (int)CeilingWithOrder(end / num6, 1.0);
        n = val + num2;
        start = (double)(-val) * num6;
        end = (double)num2 * num6;
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
