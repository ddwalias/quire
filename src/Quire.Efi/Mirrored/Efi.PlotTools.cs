#nullable disable
#pragma warning disable

using System;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

public partial class PlotTools
{
    public MatVal EliminateNearPoints(ref MatVal xs, ref MatVal ys, double dmax)
    {
        int[] array = HeapSort(xs);
        short[] array2 = new short[xs.Rows];
        int num = 0;
        int num2 = 0;
        int num3 = 0;
        do
        {
            double num4 = xs.get_real_mixed(array[num]);
            double num5 = ys.get_real_mixed(array[num]);
            array2[num] = 1;
            num++;
            num2 = -num;
            double num6 = xs.get_real_mixed(array[num]);
            do
            {
                if (!(num6 - num4 < dmax))
                {
                    continue;
                }
                if (array2[num] != -1)
                {
                    double num7 = ys.get_real_mixed(array[num]);
                    if (Math.Abs(num5 - num7) < dmax)
                    {
                        array2[num] = -1;
                        num3++;
                    }
                    else if (num2 < 0)
                    {
                        num2 = num;
                    }
                }
                if (++num == xs.Rows)
                {
                    break;
                }
                num6 = xs.get_real_mixed(array[num]);
            }
            while (num6 - num4 < dmax);
            if (num2 > 0)
            {
                num = num2;
            }
        }
        while (num < xs.Rows - 1);
        int num8 = xs.Rows - num3;
        MatVal matVal = MatVal.reserve(num8);
        MatVal matVal2 = MatVal.reserve(num8);
        for (int i = 0; i < xs.Rows; i++)
        {
            if (array2[i] != -1)
            {
                matVal.PushRaw(xs.get_1(array[i]));
                matVal2.PushRaw(ys.get_1(array[i]));
            }
        }
        matVal.m_dim = xs.m_dim;
        matVal2.m_dim = ys.m_dim;
        MatVal matVal3 = MatVal.reserve(3);
        matVal3.PushRaw(matVal);
        matVal3.PushRaw(matVal2);
        matVal3.PushRaw(num8);
        return matVal3;
    }

    public MatVal EliminateCollinearPoints(ref MatVal xs, ref MatVal ys, double Max_Angle, uint ncycles)
    {
        MatVal matVal = EliminateCollinearPoints(ref xs, ref ys, Max_Angle);
        for (int i = 1; i < ncycles; i++)
        {
            MatVal xs2 = matVal.get_1(0) as MatVal;
            MatVal ys2 = matVal.get_1(1) as MatVal;
            matVal = EliminateCollinearPoints(ref xs2, ref ys2, Max_Angle);
        }
        return matVal;
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

    private void dmin_dmax(MatVal ys, out double dmin, out double dmax)
    {
        double num = double.PositiveInfinity;
        double num2 = double.NegativeInfinity;
        for (int i = 0; i < ys.Rows; i++)
        {
            double num3 = ys.get_real_mixed(i);
            if (num3 > num2)
            {
                num2 = num3;
            }
            if (num3 < num)
            {
                num = num3;
            }
        }
        dmax = 0.2 * (num2 - num);
        dmin = 0.005 * (num2 - num);
    }

}
