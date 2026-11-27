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
            double value = (Data.percentile(v, new NumVal(0.5)) as NumVal).asReal();
            double otherValue = (Data.percentile(v, new NumVal(0.75)) as NumVal).asReal();
            double nextValue = otherValue - num;
            double currentValue = num - 1.5 * nextValue;
            double calculatedValue = otherValue + 1.5 * nextValue;
            int[] indices = new int[v.Rows];
            double[] values = new double[v.Rows];
            int counter = 0;
            for (int j = 0; j < v.Rows; j++)
            {
                indices[j] = j;
                int sampleValue = v.m_offset + j;
                if (v.m_rarr[sampleValue] > calculatedValue || v.m_rarr[sampleValue] < currentValue)
                {
                    indices[j] = -1;
                    values[counter++] = v.m_rarr[v.m_offset + j];
                }
            }
            Array.Sort(values, 0, counter);
            int limitValue = -1;
            double coordinate = 0.0;
            double measurement = 0.0;
            for (int k = 0; k < v.Rows; k++)
            {
                if (indices[k] != -1)
                {
                    if (limitValue == -1)
                    {
                        limitValue = v.m_offset + k;
                        coordinate = v.m_rarr[limitValue];
                        measurement = v.m_rarr[limitValue];
                    }
                    else
                    {
                        limitValue = v.m_offset + k;
                        coordinate = Math.Min(v.m_rarr[limitValue], coordinate);
                        measurement = Math.Max(v.m_rarr[limitValue], measurement);
                    }
                }
            }
            double[] otherValues = new double[5 + counter];
            otherValues[0] = num;
            otherValues[1] = value;
            otherValues[2] = otherValue;
            otherValues[3] = coordinate;
            otherValues[4] = measurement;
            for (int l = 0; l < counter; l++)
            {
                otherValues[l + 5] = values[l];
            }
            array.SetValue(otherValues, i);
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
            double value = matVal.get_real_mixed(i, 1);
            num += value;
        }
        double otherValue = 0.0;
        for (int j = 0; j < matVal.Rows; j++)
        {
            otherValue += 100.0 * (matVal.get_real_mixed(j, 1) / num);
            matVal.set_2(j, 2, otherValue, Dim.identity);
        }
        return matVal;
    }
}
