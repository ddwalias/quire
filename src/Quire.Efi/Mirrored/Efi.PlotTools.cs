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
        short[] items = new short[xs.Rows];
        int num = 0;
        int value = 0;
        int counter = 0;
        do
        {
            double otherValue = xs.get_real_mixed(array[num]);
            double nextValue = ys.get_real_mixed(array[num]);
            items[num] = 1;
            num++;
            value = -num;
            double currentValue = xs.get_real_mixed(array[num]);
            do
            {
                if (!(currentValue - otherValue < dmax))
                {
                    continue;
                }
                if (items[num] != -1)
                {
                    double calculatedValue = ys.get_real_mixed(array[num]);
                    if (Math.Abs(nextValue - calculatedValue) < dmax)
                    {
                        items[num] = -1;
                        counter++;
                    }
                    else if (value < 0)
                    {
                        value = num;
                    }
                }
                if (++num == xs.Rows)
                {
                    break;
                }
                currentValue = xs.get_real_mixed(array[num]);
            }
            while (currentValue - otherValue < dmax);
            if (value > 0)
            {
                num = value;
            }
        }
        while (num < xs.Rows - 1);
        int rowCount = xs.Rows - counter;
        MatVal matVal = MatVal.reserve(rowCount);
        MatVal matVal2 = MatVal.reserve(rowCount);
        for (int i = 0; i < xs.Rows; i++)
        {
            if (items[i] != -1)
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
        matVal3.PushRaw(rowCount);
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
        double maximum = double.NegativeInfinity;
        for (int i = 0; i < ys.Rows; i++)
        {
            double value = ys.get_real_mixed(i);
            if (value > maximum)
            {
                maximum = value;
            }
            if (value < num)
            {
                num = value;
            }
        }
        dmax = 0.2 * (maximum - num);
        dmin = 0.005 * (maximum - num);
    }

}
