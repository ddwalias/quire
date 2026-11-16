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
            double num2 = shpPoint3D3.Y() - shpPoint3D2.Y();
            if (Math.Abs(num) < 1E-07 && Math.Abs(num2) < 1E-07)
            {
                continue;
            }
            double num3 = shpPoint3D4.X() - shpPoint3D3.X();
            double num4 = shpPoint3D4.Y() - shpPoint3D3.Y();
            if ((!(Math.Abs(num3) < 1E-07) || !(Math.Abs(num4) < 1E-07)) && num * num4 - num2 * num3 > 1E-05)
            {
                get_bisectorX(shpPoint3D2, shpPoint3D3, shpPoint3D4, out var x, out var y);
                double x2 = shpPoint3D3.X() + 0.1 * (x - shpPoint3D3.X());
                double y2 = shpPoint3D3.Y() + 0.1 * (y - shpPoint3D3.Y());
                int num5 = i - 1;
                if (num5 < 0)
                {
                    num5 += cntr.Count;
                }
                shpPoint3D p = cntr[num5];
                num5 = i + 3;
                if (num5 > cntr.Count - 1)
                {
                    num5 -= cntr.Count;
                }
                shpPoint3D p2 = cntr[num5];
                double num6 = get_nearest_in_vertex_value(p, shpPoint3D2, shpPoint3D3, shpPoint3D4, p2, new shpPoint3D(x2, y2, 0.0), grid);
                double num7 = shpPoint3D3.Z();
                if (double.IsNaN(num6))
                {
                    return -2;
                }
                if (num7 > num6)
                {
                    return -1;
                }
                if (num7 < num6)
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
        double num2 = Math.Sqrt((p3.X() - p2.X()) * (p3.X() - p2.X()) + (p3.Y() - p2.Y()) * (p3.Y() - p2.Y()));
        double num3 = Math.Sqrt((p3.X() - p1.X()) * (p3.X() - p1.X()) + (p3.Y() - p1.Y()) * (p3.Y() - p1.Y()));
        double num4 = num3 / (1.0 + num / num2);
        double num5 = num3 - num4;
        x = (p3.X() - p1.X()) * num5 / num3 + p1.X();
        y = (p3.Y() - p1.Y()) * num5 / num3 + p1.Y();
    }

    private int segXseg(shpPoint3D p11, shpPoint3D p12, shpPoint3D p21, shpPoint3D p22, out double h1, out double h2, out double xs, out double ys)
    {
        double num = p12.X() - p11.X();
        double num2 = p12.Y() - p11.Y();
        double num3 = p22.X() - p21.X();
        double num4 = p22.Y() - p21.Y();
        double num5 = p21.X() - p11.X();
        double num6 = p21.Y() - p11.Y();
        double num7 = num3 * num2 - num4 * num;
        h1 = Math.Sqrt(num * num + num2 * num2);
        h2 = Math.Sqrt(num3 * num3 + num4 * num4);
        xs = double.NaN;
        ys = double.NaN;
        if (Math.Abs(num7) == 0.0)
        {
            return 0;
        }
        double num8 = 1.0 / num7;
        double num9 = (num3 * num6 - num4 * num5) * num8;
        double num10 = (num * num6 - num2 * num5) * num8;
        if (num9 < 0.0 || num9 > 1.0 || num10 < 0.0 || num10 > 1.0)
        {
            return 0;
        }
        xs = p11.X() + num9 * num;
        ys = p11.Y() + num9 * num2;
        return 1;
    }

}
