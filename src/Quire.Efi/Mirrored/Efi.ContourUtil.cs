#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

public partial class ContourUtil
{
    public ContourUtil()
    {
    }

    public int ContourGradient(List<shpPoint3D> cntr, glMesh grid, Function F)
    {
        for (int i = 0; i < cntr.Count - 2; i++)
        {
            shpPoint3D shpPoint3D2 = cntr[i];
            shpPoint3D shpPoint3D3 = cntr[i + 1];
            shpPoint3D shpPoint3D4 = cntr[i + 2];
            double num = shpPoint3D3.X() - shpPoint3D2.X();
            double value = shpPoint3D3.Y() - shpPoint3D2.Y();
            if (Math.Abs(num) < 1E-07 && Math.Abs(value) < 1E-07)
            {
                continue;
            }
            double otherValue = shpPoint3D4.X() - shpPoint3D3.X();
            double nextValue = shpPoint3D4.Y() - shpPoint3D3.Y();
            if ((!(Math.Abs(otherValue) < 1E-07) || !(Math.Abs(nextValue) < 1E-07)) && num * nextValue - value * otherValue > 1E-05)
            {
                get_bisectorX(shpPoint3D2, shpPoint3D3, shpPoint3D4, out var x, out var y);
                double x2 = shpPoint3D3.X() + 0.1 * (x - shpPoint3D3.X());
                double y2 = shpPoint3D3.Y() + 0.1 * (y - shpPoint3D3.Y());
                int currentValue = i - 1;
                if (currentValue < 0)
                {
                    currentValue += cntr.Count;
                }
                shpPoint3D p = cntr[currentValue];
                currentValue = i + 3;
                if (currentValue > cntr.Count - 1)
                {
                    currentValue -= cntr.Count;
                }
                shpPoint3D p2 = cntr[currentValue];
                double calculatedValue = get_nearest_in_vertex_value(p, shpPoint3D2, shpPoint3D3, shpPoint3D4, p2, new shpPoint3D(x2, y2, 0.0), grid);
                double sampleValue = shpPoint3D3.Z();
                if (double.IsNaN(calculatedValue))
                {
                    return -2;
                }
                if (sampleValue > calculatedValue)
                {
                    return -1;
                }
                if (sampleValue < calculatedValue)
                {
                    return 1;
                }
            }
        }
        return -2;
    }

    private void get_bisectorX(shpPoint3D p1, shpPoint3D p2, shpPoint3D p3, out double x, out double y)
    {
        double num = Math.Sqrt((p2.X() - p1.X()) * (p2.X() - p1.X()) + (p2.Y() - p1.Y()) * (p2.Y() - p1.Y()));
        double distance = Math.Sqrt((p3.X() - p2.X()) * (p3.X() - p2.X()) + (p3.Y() - p2.Y()) * (p3.Y() - p2.Y()));
        double otherValue = Math.Sqrt((p3.X() - p1.X()) * (p3.X() - p1.X()) + (p3.Y() - p1.Y()) * (p3.Y() - p1.Y()));
        double value = otherValue / (1.0 + num / distance);
        double nextValue = otherValue - value;
        x = (p3.X() - p1.X()) * nextValue / otherValue + p1.X();
        y = (p3.Y() - p1.Y()) * nextValue / otherValue + p1.Y();
    }

    private int segXseg(shpPoint3D p11, shpPoint3D p12, shpPoint3D p21, shpPoint3D p22, out double h1, out double h2, out double xs, out double ys)
    {
        double num = p12.X() - p11.X();
        double value = p12.Y() - p11.Y();
        double otherValue = p22.X() - p21.X();
        double nextValue = p22.Y() - p21.Y();
        double currentValue = p21.X() - p11.X();
        double calculatedValue = p21.Y() - p11.Y();
        double sampleValue = otherValue * value - nextValue * num;
        h1 = Math.Sqrt(num * num + value * value);
        h2 = Math.Sqrt(otherValue * otherValue + nextValue * nextValue);
        xs = double.NaN;
        ys = double.NaN;
        if (Math.Abs(sampleValue) == 0.0)
        {
            return 0;
        }
        double limitValue = 1.0 / sampleValue;
        double coordinate = (otherValue * calculatedValue - nextValue * currentValue) * limitValue;
        double measurement = (num * calculatedValue - value * currentValue) * limitValue;
        if (coordinate < 0.0 || coordinate > 1.0 || measurement < 0.0 || measurement > 1.0)
        {
            return 0;
        }
        xs = p11.X() + coordinate * num;
        ys = p11.Y() + coordinate * value;
        return 1;
    }

}
