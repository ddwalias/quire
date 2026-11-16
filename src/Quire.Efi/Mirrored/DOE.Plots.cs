#nullable disable
#pragma warning disable

using System;
using Quire.Efi;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class Plots
{
    private Dim[] m_cdim;

    public MatVal BoxPlot(MatVal data)
    {
        Array array = Array.CreateInstance(typeof(double[]), data.Cols);
        m_cdim = new Dim[data.Cols];
        for (int i = 0; i < data.Cols; i++)
        {
            MatVal v = data.Column(i);
            v = stripVector(v);
            if (!v.IsUniformScalar)
            {
                AppException.Raise("mixed units in column", 1u);
            }
            m_cdim[i] = v.m_dim;
            v = MatVal.StripDimension(v);
            double num = (Data.percentile(v, new NumVal(0.25)) as NumVal).asReal();
            double num2 = (Data.percentile(v, new NumVal(0.5)) as NumVal).asReal();
            double num3 = (Data.percentile(v, new NumVal(0.75)) as NumVal).asReal();
            double num4 = num3 - num;
            double num5 = num - 1.5 * num4;
            double num6 = num3 + 1.5 * num4;
            int[] array2 = new int[v.Rows];
            double[] array3 = new double[v.Rows];
            int num7 = 0;
            for (int j = 0; j < v.Rows; j++)
            {
                array2[j] = j;
                int num8 = v.m_offset + j;
                if (v.m_rarr[num8] > num6 || v.m_rarr[num8] < num5)
                {
                    array2[j] = -1;
                    array3[num7++] = v.m_rarr[v.m_offset + j];
                }
            }
            Array.Sort(array3, 0, num7);
            int num9 = -1;
            double num10 = 0.0;
            double num11 = 0.0;
            for (int k = 0; k < v.Rows; k++)
            {
                if (array2[k] != -1)
                {
                    if (num9 == -1)
                    {
                        num9 = v.m_offset + k;
                        num10 = v.m_rarr[num9];
                        num11 = v.m_rarr[num9];
                    }
                    else
                    {
                        num9 = v.m_offset + k;
                        num10 = Math.Min(v.m_rarr[num9], num10);
                        num11 = Math.Max(v.m_rarr[num9], num11);
                    }
                }
            }
            double[] array4 = new double[5 + num7];
            array4[0] = num;
            array4[1] = num2;
            array4[2] = num3;
            array4[3] = num10;
            array4[4] = num11;
            for (int l = 0; l < num7; l++)
            {
                array4[l + 5] = array3[l];
            }
            array.SetValue(array4, i);
        }
        return csResult(array);
    }

    private MatVal csResult(Array rvecs)
    {
        int num = 0;
        for (int i = 0; i < rvecs.Length; i++)
        {
            num = Math.Max(num, (rvecs.GetValue(i) as double[]).Length);
        }
        MatVal matVal = new MatVal(num, rvecs.Length);
        for (int j = 0; j < rvecs.Length; j++)
        {
            double[] array = rvecs.GetValue(j) as double[];
            for (int k = 0; k < array.Length; k++)
            {
                matVal.set_2(k, j, array[k], m_cdim[j]);
            }
            for (int l = array.Length; l < num; l++)
            {
                matVal.set_2(l, j, double.NaN, m_cdim[j]);
            }
        }
        return matVal;
    }

    private MatVal stripVector(MatVal v)
    {
        MatVal matVal = new MatVal(v.Rows, 1, imag: false, v.Rows);
        int num = 0;
        for (int i = 0; i < v.Rows; i++)
        {
            object obj = v.get_1(i);
            if (obj is NumVal)
            {
                NumVal numVal = obj as NumVal;
                if (!double.IsNaN(numVal.Data.rp))
                {
                    matVal.set_1(num++, v.get_real_mixed(i), numVal.m_dim);
                }
            }
            else if (!(obj is StrVal))
            {
                AppException.Raise("invalid argument", 1u);
            }
        }
        if (num == 0)
        {
            AppException.Raise("invalid argument", 1u);
        }
        matVal.Data.m_rows = num;
        return matVal;
    }

    public MatVal ParetoPlot(MatVal data)
    {
        MatVal matVal = Matrix.reverse(Matrix.csort(0, data, new NumVal(1)) as MatVal) as MatVal;
        matVal = matVal.ReAllocate(matVal.Rows, matVal.Cols + 1);
        if (!matVal.Column(1).IsUniformScalar)
        {
            AppException.Raise("mixed units in column", 1u);
        }
        double num = 0.0;
        for (int i = 0; i < matVal.Rows; i++)
        {
            if (!Utl.is_preal(matVal.get_2(i, 1)))
            {
                AppException.Raise("values in 2nd column must be positive", 1u);
            }
            double num2 = matVal.get_real_mixed(i, 1);
            num += num2;
        }
        double num3 = 0.0;
        for (int j = 0; j < matVal.Rows; j++)
        {
            num3 += 100.0 * (matVal.get_real_mixed(j, 1) / num);
            matVal.set_2(j, 2, num3, Dim.identity);
        }
        return matVal;
    }
}
