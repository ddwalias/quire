#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Quire.Efi.CalculationAssistant;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

public partial class ContourMain
{
    private Function m_F;

    private MatVal m_mdata;

    private Dim m_Fdim;

    private Dim m_Ldim;

    private glMesh m_mesh;

    private LRect3 m_rect;

    private int m_rows;

    private int m_cols;

    private MatVal clipMatrixData(int origin, MatVal limits, MatVal data)
    {
        int num = 0;
        int num2 = data.Rows - 1;
        int num3 = 0;
        int num4 = data.Cols - 1;
        double num5 = limits.get_real(0);
        if (!double.IsNaN(num5))
        {
            num = (int)num5 - origin - 1;
        }
        num5 = limits.get_real(1);
        if (!double.IsNaN(num5))
        {
            num2 = (int)(Math.Ceiling(num5) + 0.001) - origin + 1;
        }
        num5 = limits.get_real(2);
        if (!double.IsNaN(num5))
        {
            num3 = (int)num5 - origin - 1;
        }
        num5 = limits.get_real(3);
        if (!double.IsNaN(num5))
        {
            num4 = (int)(Math.Ceiling(num5) + 0.001) - origin + 1;
        }
        if (num > data.Rows - 1 || num2 < 0)
        {
            AppException.Raise("out of range clipping limits", 3u);
        }
        if (num3 > data.Cols - 1 || num4 < 0)
        {
            AppException.Raise("out of range clipping limits", 3u);
        }
        if (num < 0)
        {
            num = 0;
        }
        if (num3 < 0)
        {
            num3 = 0;
        }
        if (num2 > data.Rows - 1)
        {
            num2 = data.Rows - 1;
        }
        if (num4 > data.Cols - 1)
        {
            num4 = data.Cols - 1;
        }
        if (num2 <= num || num4 <= num3)
        {
            AppException.Raise("out of range clipping limits", 3u);
        }
        limits.set_1(0, num);
        limits.set_1(1, num2);
        limits.set_1(2, num3);
        limits.set_1(3, num4);
        if (num > 0 || num2 < data.Rows - 1 || num3 > 0 || num4 < data.Cols - 1)
        {
            return data.SubMatrix((uint)num, (uint)(num2 + 1), (uint)num3, (uint)(num4 + 1));
        }
        return data;
    }

    public ContourMain(object data_, int origin, MatVal limits, object scaleFactor)
    {
        m_F = null;
        MatVal matVal = Utl.as_matrix(data_, 1u);
        int num = 0;
        double num2 = double.MaxValue;
        double num3 = double.MinValue;
        double num4 = double.MaxValue;
        double num5 = double.MinValue;
        double num6 = double.MaxValue;
        double num7 = double.MinValue;
        shpPoint3D[] array = null;
        if (matVal.Cols == 1 || (matVal.Cols == 3 && matVal.Rows == 1))
        {
            MatVal matVal2 = null;
            MatVal matVal3 = null;
            bool flag = false;
            if (matVal.Cols == 3)
            {
                matVal = matVal.Transpose();
            }
            if (matVal.Cols == 1)
            {
                if (matVal.Rows != 3)
                {
                    AppException.Raise("invalid argument", 1u);
                }
                m_mdata = Utl.as_matrix(matVal.get_1(2));
                if (!m_mdata.IsUniform)
                {
                    AppException.Raise("inconsistent units", 1u);
                }
                m_rows = m_mdata.Cols;
                m_cols = m_mdata.Rows;
                matVal2 = Utl.as_rmatrix(matVal.get_1(0));
                matVal3 = Utl.as_rmatrix(matVal.get_1(1));
                if (matVal2.Cols != m_mdata.Cols || matVal2.Rows != m_mdata.Rows || matVal3.Cols != m_mdata.Cols || matVal3.Rows != m_mdata.Rows)
                {
                    AppException.Raise("invalid argument", 1u);
                }
                for (int i = 0; i < matVal2.Rows; i++)
                {
                    double num8 = matVal2.get_real(i, 0);
                    for (int j = 1; j < matVal2.Cols; j++)
                    {
                        if (Math.Abs(matVal2.get_real(i, j) - num8) > 1E-12)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                for (int k = 0; k < matVal3.Cols; k++)
                {
                    double num9 = matVal3.get_real(0, k);
                    for (int l = 1; l < matVal3.Rows; l++)
                    {
                        if (Math.Abs(matVal3.get_real(l, k) - num9) > 1E-12)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
            }
            else if (matVal.Cols == 3 && matVal.Rows > 2)
            {
                if (matVal.Rows < 2)
                {
                    AppException.Raise("invalid argument", 1u);
                }
                flag = true;
                matVal2 = matVal.Column(0);
                matVal3 = matVal.Column(1);
                m_rows = 41;
                m_cols = 41;
                m_mdata = matVal.Column(2);
                if (!matVal2.IsUniform)
                {
                    AppException.Raise("inconsistent units", 1u);
                }
                if (!matVal3.IsUniform)
                {
                    AppException.Raise("inconsistent units", 1u);
                }
                if (!m_mdata.IsUniform)
                {
                    AppException.Raise("inconsistent units", 1u);
                }
            }
            if (scaleFactor != null)
            {
                m_mdata = Plot.OpScaleInverse(m_mdata, scaleFactor) as MatVal;
            }
            m_Fdim = m_mdata.get_2_dim(0, 0);
            MatVal matVal4 = m_mdata.Canonicalize();
            if (flag)
            {
                MatVal obj = Graphics.map2Dpoints(matVal2, matVal3, matVal4, new NumVal(m_cols), new NumVal(m_rows)) as MatVal;
                if (obj == null)
                {
                    AppException.Raise("no contours found", 1u);
                }
                matVal2 = Utl.as_rmatrix(obj.get_1(0));
                matVal3 = Utl.as_rmatrix(obj.get_1(1));
                matVal4 = Utl.as_rmatrix(obj.get_1(2));
            }
            Dim dim = m_mdata.m_dim;
            m_mdata = matVal4;
            m_mdata.m_dim = dim;
            double num10 = limits.get_real(0);
            double num11 = limits.get_real(1);
            double num12 = limits.get_real(2);
            double num13 = limits.get_real(3);
            double num14 = Math.Abs(matVal2.get_real(1, 0) - matVal2.get_real(0, 0));
            double num15 = Math.Abs(matVal3.get_real(0, 1) - matVal3.get_real(0, 0));
            num10 -= num14;
            num11 += num14;
            num12 -= num15;
            num13 += num15;
            bool flag2 = true;
            if (double.IsNaN(num10))
            {
                num10 = double.MinValue;
            }
            else
            {
                flag2 = false;
            }
            if (double.IsNaN(num11))
            {
                num11 = double.MaxValue;
            }
            else
            {
                flag2 = false;
            }
            if (double.IsNaN(num12))
            {
                num12 = double.MinValue;
            }
            else
            {
                flag2 = false;
            }
            if (double.IsNaN(num13))
            {
                num13 = double.MaxValue;
            }
            else
            {
                flag2 = false;
            }
            if (!flag2)
            {
                for (int m = 0; m < matVal2.Rows; m++)
                {
                    double num16 = matVal2.get_real(m, 0);
                    if (num16 < num10 || num16 > num11)
                    {
                        m_cols--;
                    }
                }
                for (int n = 0; n < matVal3.Cols; n++)
                {
                    double num17 = matVal3.get_real(0, n);
                    if (num17 < num12 || num17 > num13)
                    {
                        m_rows--;
                    }
                }
            }
            if (m_rows == 0 || m_cols == 0)
            {
                AppException.Raise("out of range clipping limits", 3u);
            }
            array = new shpPoint3D[m_rows * m_cols];
            for (int num18 = 0; num18 < m_rows; num18++)
            {
                for (int num19 = 0; num19 < m_cols; num19++)
                {
                    double num20 = matVal3.get_real(num19, num18);
                    if (!(num20 < num12) && !(num20 > num13))
                    {
                        double num21 = matVal2.get_real(num19, num18);
                        if (!(num21 < num10) && !(num21 > num11))
                        {
                            num4 = ((num4 < num20) ? num4 : num20);
                            num5 = ((num5 > num20) ? num5 : num20);
                            num2 = ((num2 < num21) ? num2 : num21);
                            num3 = ((num3 > num21) ? num3 : num21);
                            double num22 = matVal4.get_real(num19, num18);
                            num6 = ((num6 < num22) ? num6 : num22);
                            num7 = ((num7 > num22) ? num7 : num22);
                            array[num] = new shpPoint3D(num21, num20, num22);
                            num++;
                        }
                    }
                }
            }
            if (num != m_rows * m_cols)
            {
                AppException.Raise("plot3D inconsistent Create format", 1u);
            }
        }
        else
        {
            m_mdata = clipMatrixData(origin, limits, matVal);
            if (scaleFactor != null)
            {
                m_mdata = Plot.OpScaleInverse(m_mdata, scaleFactor) as MatVal;
            }
            if (!m_mdata.IsUniform)
            {
                AppException.Raise("inconsistent units", 1u);
            }
            m_rows = m_mdata.Cols;
            m_cols = m_mdata.Rows;
            if (m_rows < 2 || m_cols < 2)
            {
                AppException.Raise("invalid argument", 1u);
            }
            num2 = limits.get_real(0) + (double)origin;
            num3 = limits.get_real(1) + (double)origin;
            num4 = limits.get_real(2) + (double)origin;
            num5 = limits.get_real(3) + (double)origin;
            num6 = double.MaxValue;
            num7 = double.MinValue;
            double num23 = 1.0;
            double num24 = 1.0;
            double num25 = num4;
            array = new shpPoint3D[m_rows * m_cols];
            for (int num26 = 0; num26 < m_mdata.Cols; num26++)
            {
                double num27 = num2;
                for (int num28 = 0; num28 < m_mdata.Rows; num28++)
                {
                    double num29 = m_mdata.get_real_mixed(num28, num26);
                    num6 = ((num6 < num29) ? num6 : num29);
                    num7 = ((num7 > num29) ? num7 : num29);
                    array[num] = new shpPoint3D(num27, num25, num29);
                    num27 += num23;
                    num++;
                }
                num25 += num24;
            }
        }
        m_rect = new LRect3(num2, num3, num4, num5, num6, num7);
        m_mesh = new glMesh(m_rect, m_rows, m_cols, array);
    }

    public ContourMain(object F_, object grid_, MatVal limits, object scaleFactor)
    {
        m_mdata = null;
        Function f = F_ as Function;
        m_F = f;
        MatVal matVal = Utl.as_ivector(grid_);
        m_cols = (int)matVal.get_real(0);
        m_rows = (int)matVal.get_real(1);
        double num = limits.get_real(0);
        double num2 = limits.get_real(1);
        double num3 = limits.get_real(2);
        double num4 = limits.get_real(3);
        if (double.IsNaN(num))
        {
            num = -10.0;
        }
        if (double.IsNaN(num2))
        {
            num2 = 10.0;
        }
        if (double.IsNaN(num3))
        {
            num3 = -10.0;
        }
        if (double.IsNaN(num4))
        {
            num4 = 10.0;
        }
        if (num >= num2 || num3 >= num4)
        {
            AppException.Raise("plot3D invalid limits vector", 3u);
        }
        int ntries = 0;
        shpPoint3D[] vertices = null;
        double minZ = 0.0;
        double maxZ = 0.0;
        NumVal aa = new NumVal(num, 0.0, Dim.identity);
        NumVal bb = new NumVal(num3, 0.0, Dim.identity);
        object obj = m_F.Call(aa, bb);
        if (!(obj is NumVal))
        {
            AppException.Raise("invalid argument", 1u);
        }
        Dim dim = Utl.as_scalar(obj).m_dim;
        if (scaleFactor != null)
        {
            obj = Plot.OpScaleInverse(obj, scaleFactor) as NumVal;
        }
        m_Fdim = Utl.as_scalar(obj).m_dim;
        double num5 = 1.0;
        if (!Dim.IsDimensioned(dim) && scaleFactor != null && !Dim.IsDimensioned(Utl.as_scalar(scaleFactor).m_dim))
        {
            num5 = Utl.as_scalar(scaleFactor).Real;
            if (num5 == 0.0)
            {
                num5 = 1.0;
            }
        }
        if (!try_calc_mt_vertices(num, num2, num3, num4, dim, num5, int.MaxValue, ref ntries, ref minZ, ref maxZ, ref vertices))
        {
            if (Rts.IsCancellationRequested())
            {
                throw new AbortRequestedException();
            }
            calculate_vertices_attempt(num, num2, num3, num4, dim, num5, int.MaxValue, ref ntries, ref minZ, ref maxZ, ref vertices);
        }
        m_rect = new LRect3(num, num2, num3, num4, minZ, maxZ);
        m_mesh = new glMesh(m_rect, m_rows, m_cols, vertices);
    }

    private void calculate_vertices_attempt(double xmin, double xmax, double ymin, double ymax, Dim fdim, double pws, int max_tries, ref int ntries, ref double minZ, ref double maxZ, ref shpPoint3D[] vertices)
    {
        bool retVal = false;
        Exception outExc = null;
        do
        {
            if (Rts.IsCancellationRequested())
            {
                throw new AbortRequestedException();
            }
            vertices = new shpPoint3D[m_rows * m_cols];
            calculateVertices(m_rows, m_cols, 0, m_rows - 1, CancellationToken.None, xmin, xmax, ymin, ymax, fdim, pws, m_F, ref minZ, ref maxZ, ref vertices, ref retVal, out outExc);
            if (!retVal)
            {
                if (++ntries > max_tries)
                {
                    throw outExc;
                }
                m_rows++;
                m_cols++;
            }
        }
        while (!retVal);
    }

    private static void calculateVertices(int rows, int cols, int startRow, int endRow, CancellationToken token, double xmin, double xmax, double ymin, double ymax, Dim fdim, double pws, Function F, ref double minZ, ref double maxZ, ref shpPoint3D[] vertices, ref bool retVal, out Exception outExc)
    {
        outExc = null;
        retVal = false;
        double num = (xmax - xmin) / (double)(cols - 1);
        double num2 = (ymax - ymin) / (double)(rows - 1);
        minZ = double.MaxValue;
        maxZ = double.MinValue;
        NumVal numVal = new NumVal(xmin, 0.0, Dim.identity);
        NumVal numVal2 = new NumVal(ymin, 0.0, Dim.identity);
        int num3 = startRow * cols;
        double num4 = ymin + (double)startRow * num2;
        int num5 = startRow;
        while (num5 <= endRow && !token.IsCancellationRequested)
        {
            double num6 = xmin;
            int num7 = 0;
            while (num7 < cols && !token.IsCancellationRequested)
            {
                numVal.Data.rp = num6;
                numVal2.Data.rp = num4;
                NumVal numVal3;
                try
                {
                    numVal3 = Utl.as_scalar(F.Call(numVal, numVal2));
                    if (numVal3 == null)
                    {
                        AppException.Raise("plot3D function must return scalar or 3 element vector", 1u);
                    }
                }
                catch (Exception ex)
                {
                    outExc = ex;
                    return;
                }
                if (!Dim.Eq_b(fdim, numVal3.m_dim))
                {
                    AppException.Raise("inconsistent units", 1u);
                }
                double num8 = Utl.as_real(numVal3) / pws;
                minZ = ((minZ < num8) ? minZ : num8);
                maxZ = ((maxZ > num8) ? maxZ : num8);
                vertices[num3++] = new shpPoint3D(num6, num4, num8);
                num7++;
                num6 += num;
            }
            num5++;
            num4 += num2;
        }
        retVal = true;
    }

    public MatVal getContours(object levels_)
    {
        double[] array = null;
        int num_levels = 0;
        if (levels_ is MatVal)
        {
            MatVal matVal = Utl.as_rvector(levels_);
            array = new double[matVal.Rows];
            for (int i = 0; i < matVal.Rows; i++)
            {
                array[i] = matVal.get_real(i);
            }
            num_levels = array.Length;
            m_Ldim = matVal.m_dim;
        }
        else
        {
            if (levels_ is NumVal)
            {
                num_levels = (int)Utl.as_pint(levels_, 4u);
                if (num_levels < 3)
                {
                    num_levels = 3;
                }
            }
            else if (levels_ == null)
            {
                num_levels = 15;
            }
            LevelFormat levelFormat = new LevelFormat();
            double start = m_rect.zmin;
            double end = m_rect.zmax;
            levelFormat.CalculateBetterTickmarks(ref start, ref end, ref num_levels);
            array = new double[num_levels];
            double num = (end - start) / (double)(num_levels - 1);
            for (int j = 0; j < num_levels; j++)
            {
                array[j] = start + (double)j * num;
            }
        }
        pltContour pltContour2 = new pltContour(array, num_levels, m_cols, m_rows, m_mesh);
        if (!pltContour2.drawConts())
        {
            AppException.Raise("no contours found");
        }
        sortContours(pltContour2.m_allcontours);
        return storeContours2(pltContour2.m_allcontours);
    }

    private void sortContours(List<List<shpPoint3D>> allcontours)
    {
        ContourUtil contourUtil = new ContourUtil();
        foreach (List<shpPoint3D> allcontour in allcontours)
        {
            if (allcontour.Count >= 2)
            {
                contourUtil.MakeBLoop(m_rect, allcontour);
                double num = contourUtil.polyArea(allcontour);
                if (num < 0.0)
                {
                    allcontour.Reverse();
                    num *= -1.0;
                }
                allcontour[0].Z(num);
            }
        }
        allcontours.Sort(new CntrComparer());
        allcontours.Reverse();
    }

    private MatVal storeContours1(List<List<shpPoint3D>> allcontours)
    {
        int num = 0;
        foreach (List<shpPoint3D> allcontour in allcontours)
        {
            int count = allcontour.Count;
            num = ((num > count) ? num : count);
        }
        int num2 = allcontours.Count;
        double[] array = new double[num2 * num];
        double[] array2 = new double[num2 * num];
        double[] array3 = new double[num2];
        int num3 = 0;
        int num4 = 0;
        foreach (List<shpPoint3D> allcontour2 in allcontours)
        {
            if (allcontour2.Count < 3)
            {
                num2--;
                continue;
            }
            array3[num4++] = allcontour2[1].Z();
            foreach (shpPoint3D item in allcontour2)
            {
                array[num3] = item.X();
                array2[num3] = item.Y();
                num3++;
            }
            for (int i = 0; i < num - allcontour2.Count; i++)
            {
                array[num3] = double.NaN;
                array2[num3] = double.NaN;
                num3++;
            }
        }
        MatVal x = new MatVal(array, null, num, num2, num);
        MatVal x2 = new MatVal(array2, null, num, num2, num);
        MatVal x3 = new MatVal(array3);
        MatVal matVal = new MatVal(3);
        matVal.set_1(0, x);
        matVal.set_1(1, x2);
        matVal.set_1(2, x3);
        return matVal;
    }

    private MatVal storeContours2(List<List<shpPoint3D>> allcontours)
    {
        double num = 1E-09;
        int num2 = allcontours.Count;
        foreach (List<shpPoint3D> allcontour in allcontours)
        {
            if (allcontour.Count < 3 || allcontour[0].Z() < num)
            {
                num2--;
            }
        }
        if (num2 <= 0)
        {
            AppException.Raise("no contours found", 1u);
        }
        MatVal matVal = new MatVal(num2 + 3);
        MatVal matVal2 = new MatVal(num2);
        MatVal matVal3 = new MatVal(num2);
        ContourUtil contourUtil = new ContourUtil();
        int num3 = 0;
        foreach (List<shpPoint3D> allcontour2 in allcontours)
        {
            if (allcontour2.Count < 3 || allcontour2[0].Z() < num)
            {
                continue;
            }
            double[] array = new double[2 * allcontour2.Count];
            int num4 = 0;
            foreach (shpPoint3D item in allcontour2)
            {
                array[num4] = item.X();
                array[num4 + allcontour2.Count] = item.Y();
                num4++;
            }
            MatVal x = new MatVal(array, null, allcontour2.Count, 2, allcontour2.Count);
            matVal.set_1(num3, x);
            matVal2.set_1(num3, allcontour2[1].Z());
            matVal3.set_1(num3, contourUtil.ContourGradient(allcontour2, m_mesh, m_F));
            num3++;
        }
        if (!Dim.Eq_b(m_Fdim, m_Ldim))
        {
            matVal2.m_dim = Dim.Div(m_Fdim, m_Ldim);
        }
        else
        {
            matVal2.m_dim = m_Fdim;
        }
        matVal.set_1(num3++, matVal2);
        matVal.set_1(num3++, matVal3);
        MatVal matVal4 = new MatVal(4);
        matVal4.set_1(0, m_rect.xmin);
        matVal4.set_1(1, m_rect.xmax);
        matVal4.set_1(2, m_rect.ymin);
        matVal4.set_1(3, m_rect.ymax);
        matVal.set_1(num3++, matVal4);
        return matVal;
    }
}
