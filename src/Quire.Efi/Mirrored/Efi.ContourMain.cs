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
        int rowCount = data.Rows - 1;
        int value = 0;
        int columnCount = data.Cols - 1;
        double otherValue = limits.get_real(0);
        if (!double.IsNaN(otherValue))
        {
            num = (int)otherValue - origin - 1;
        }
        otherValue = limits.get_real(1);
        if (!double.IsNaN(otherValue))
        {
            rowCount = (int)(Math.Ceiling(otherValue) + 0.001) - origin + 1;
        }
        otherValue = limits.get_real(2);
        if (!double.IsNaN(otherValue))
        {
            value = (int)otherValue - origin - 1;
        }
        otherValue = limits.get_real(3);
        if (!double.IsNaN(otherValue))
        {
            columnCount = (int)(Math.Ceiling(otherValue) + 0.001) - origin + 1;
        }
        if (num > data.Rows - 1 || rowCount < 0)
        {
            AppException.Raise("out of range clipping limits", 3u);
        }
        if (value > data.Cols - 1 || columnCount < 0)
        {
            AppException.Raise("out of range clipping limits", 3u);
        }
        if (num < 0)
        {
            num = 0;
        }
        if (value < 0)
        {
            value = 0;
        }
        if (rowCount > data.Rows - 1)
        {
            rowCount = data.Rows - 1;
        }
        if (columnCount > data.Cols - 1)
        {
            columnCount = data.Cols - 1;
        }
        if (rowCount <= num || columnCount <= value)
        {
            AppException.Raise("out of range clipping limits", 3u);
        }
        limits.set_1(0, num);
        limits.set_1(1, rowCount);
        limits.set_1(2, value);
        limits.set_1(3, columnCount);
        if (num > 0 || rowCount < data.Rows - 1 || value > 0 || columnCount < data.Cols - 1)
        {
            return data.SubMatrix((uint)num, (uint)(rowCount + 1), (uint)value, (uint)(columnCount + 1));
        }
        return data;
    }

    public ContourMain(object data_, int origin, MatVal limits, object scaleFactor)
    {
        m_F = null;
        MatVal matVal = Utl.as_matrix(data_, 1u);
        int num = 0;
        double minimum = double.MaxValue;
        double maximum = double.MinValue;
        double otherValue = double.MaxValue;
        double nextValue = double.MinValue;
        double currentValue = double.MaxValue;
        double calculatedValue = double.MinValue;
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
                    double value = matVal2.get_real(i, 0);
                    for (int j = 1; j < matVal2.Cols; j++)
                    {
                        if (Math.Abs(matVal2.get_real(i, j) - value) > 1E-12)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                for (int k = 0; k < matVal3.Cols; k++)
                {
                    double sampleValue = matVal3.get_real(0, k);
                    for (int l = 1; l < matVal3.Rows; l++)
                    {
                        if (Math.Abs(matVal3.get_real(l, k) - sampleValue) > 1E-12)
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
            double limitValue = limits.get_real(0);
            double coordinate = limits.get_real(1);
            double measurement = limits.get_real(2);
            double resultValue = limits.get_real(3);
            double difference = Math.Abs(matVal2.get_real(1, 0) - matVal2.get_real(0, 0));
            double adjustedValue = Math.Abs(matVal3.get_real(0, 1) - matVal3.get_real(0, 0));
            limitValue -= difference;
            coordinate += difference;
            measurement -= adjustedValue;
            resultValue += adjustedValue;
            bool isValid = true;
            if (double.IsNaN(limitValue))
            {
                limitValue = double.MinValue;
            }
            else
            {
                isValid = false;
            }
            if (double.IsNaN(coordinate))
            {
                coordinate = double.MaxValue;
            }
            else
            {
                isValid = false;
            }
            if (double.IsNaN(measurement))
            {
                measurement = double.MinValue;
            }
            else
            {
                isValid = false;
            }
            if (double.IsNaN(resultValue))
            {
                resultValue = double.MaxValue;
            }
            else
            {
                isValid = false;
            }
            if (!isValid)
            {
                for (int m = 0; m < matVal2.Rows; m++)
                {
                    double derivedValue = matVal2.get_real(m, 0);
                    if (derivedValue < limitValue || derivedValue > coordinate)
                    {
                        m_cols--;
                    }
                }
                for (int n = 0; n < matVal3.Cols; n++)
                {
                    double remainingValue = matVal3.get_real(0, n);
                    if (remainingValue < measurement || remainingValue > resultValue)
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
            for (int index = 0; index < m_rows; index++)
            {
                for (int alternateValue = 0; alternateValue < m_cols; alternateValue++)
                {
                    double selectedValue = matVal3.get_real(alternateValue, index);
                    if (!(selectedValue < measurement) && !(selectedValue > resultValue))
                    {
                        double originalValue = matVal2.get_real(alternateValue, index);
                        if (!(originalValue < limitValue) && !(originalValue > coordinate))
                        {
                            otherValue = ((otherValue < selectedValue) ? otherValue : selectedValue);
                            nextValue = ((nextValue > selectedValue) ? nextValue : selectedValue);
                            minimum = ((minimum < originalValue) ? minimum : originalValue);
                            maximum = ((maximum > originalValue) ? maximum : originalValue);
                            double finalValue = matVal4.get_real(alternateValue, index);
                            currentValue = ((currentValue < finalValue) ? currentValue : finalValue);
                            calculatedValue = ((calculatedValue > finalValue) ? calculatedValue : finalValue);
                            array[num] = new shpPoint3D(originalValue, selectedValue, finalValue);
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
            minimum = limits.get_real(0) + (double)origin;
            maximum = limits.get_real(1) + (double)origin;
            otherValue = limits.get_real(2) + (double)origin;
            nextValue = limits.get_real(3) + (double)origin;
            currentValue = double.MaxValue;
            calculatedValue = double.MinValue;
            double boundaryValue = 1.0;
            double total = 1.0;
            double average = otherValue;
            array = new shpPoint3D[m_rows * m_cols];
            for (int columnIndex = 0; columnIndex < m_mdata.Cols; columnIndex++)
            {
                double additionalValue = minimum;
                for (int rowIndex = 0; rowIndex < m_mdata.Rows; rowIndex++)
                {
                    double inputValue = m_mdata.get_real_mixed(rowIndex, columnIndex);
                    currentValue = ((currentValue < inputValue) ? currentValue : inputValue);
                    calculatedValue = ((calculatedValue > inputValue) ? calculatedValue : inputValue);
                    array[num] = new shpPoint3D(additionalValue, average, inputValue);
                    additionalValue += boundaryValue;
                    num++;
                }
                average += total;
            }
        }
        m_rect = new LRect3(minimum, maximum, otherValue, nextValue, currentValue, calculatedValue);
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
        double value = limits.get_real(1);
        double otherValue = limits.get_real(2);
        double nextValue = limits.get_real(3);
        if (double.IsNaN(num))
        {
            num = -10.0;
        }
        if (double.IsNaN(value))
        {
            value = 10.0;
        }
        if (double.IsNaN(otherValue))
        {
            otherValue = -10.0;
        }
        if (double.IsNaN(nextValue))
        {
            nextValue = 10.0;
        }
        if (num >= value || otherValue >= nextValue)
        {
            AppException.Raise("plot3D invalid limits vector", 3u);
        }
        int ntries = 0;
        shpPoint3D[] vertices = null;
        double minZ = 0.0;
        double maxZ = 0.0;
        NumVal aa = new NumVal(num, 0.0, Dim.identity);
        NumVal bb = new NumVal(otherValue, 0.0, Dim.identity);
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
        double currentValue = 1.0;
        if (!Dim.IsDimensioned(dim) && scaleFactor != null && !Dim.IsDimensioned(Utl.as_scalar(scaleFactor).m_dim))
        {
            currentValue = Utl.as_scalar(scaleFactor).Real;
            if (currentValue == 0.0)
            {
                currentValue = 1.0;
            }
        }
        if (!try_calc_mt_vertices(num, value, otherValue, nextValue, dim, currentValue, int.MaxValue, ref ntries, ref minZ, ref maxZ, ref vertices))
        {
            if (Rts.IsCancellationRequested())
            {
                throw new AbortRequestedException();
            }
            calculate_vertices_attempt(num, value, otherValue, nextValue, dim, currentValue, int.MaxValue, ref ntries, ref minZ, ref maxZ, ref vertices);
        }
        m_rect = new LRect3(num, value, otherValue, nextValue, minZ, maxZ);
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
        double value = (ymax - ymin) / (double)(rows - 1);
        minZ = double.MaxValue;
        maxZ = double.MinValue;
        NumVal numVal = new NumVal(xmin, 0.0, Dim.identity);
        NumVal numVal2 = new NumVal(ymin, 0.0, Dim.identity);
        int counter = startRow * cols;
        double otherValue = ymin + (double)startRow * value;
        int nextValue = startRow;
        while (nextValue <= endRow && !token.IsCancellationRequested)
        {
            double currentValue = xmin;
            int calculatedValue = 0;
            while (calculatedValue < cols && !token.IsCancellationRequested)
            {
                numVal.Data.rp = currentValue;
                numVal2.Data.rp = otherValue;
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
                double sampleValue = Utl.as_real(numVal3) / pws;
                minZ = ((minZ < sampleValue) ? minZ : sampleValue);
                maxZ = ((maxZ > sampleValue) ? maxZ : sampleValue);
                vertices[counter++] = new shpPoint3D(currentValue, otherValue, sampleValue);
                calculatedValue++;
                currentValue += num;
            }
            nextValue++;
            otherValue += value;
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
        int otherValue = allcontours.Count;
        double[] array = new double[otherValue * num];
        double[] values = new double[otherValue * num];
        double[] otherValues = new double[otherValue];
        int counter = 0;
        int nextValue = 0;
        foreach (List<shpPoint3D> allcontour2 in allcontours)
        {
            if (allcontour2.Count < 3)
            {
                otherValue--;
                continue;
            }
            otherValues[nextValue++] = allcontour2[1].Z();
            foreach (shpPoint3D item in allcontour2)
            {
                array[counter] = item.X();
                values[counter] = item.Y();
                counter++;
            }
            for (int i = 0; i < num - allcontour2.Count; i++)
            {
                array[counter] = double.NaN;
                values[counter] = double.NaN;
                counter++;
            }
        }
        MatVal x = new MatVal(array, null, num, otherValue, num);
        MatVal x2 = new MatVal(values, null, num, otherValue, num);
        MatVal x3 = new MatVal(otherValues);
        MatVal matVal = new MatVal(3);
        matVal.set_1(0, x);
        matVal.set_1(1, x2);
        matVal.set_1(2, x3);
        return matVal;
    }

    private MatVal storeContours2(List<List<shpPoint3D>> allcontours)
    {
        double num = 1E-09;
        int count = allcontours.Count;
        foreach (List<shpPoint3D> allcontour in allcontours)
        {
            if (allcontour.Count < 3 || allcontour[0].Z() < num)
            {
                count--;
            }
        }
        if (count <= 0)
        {
            AppException.Raise("no contours found", 1u);
        }
        MatVal matVal = new MatVal(count + 3);
        MatVal matVal2 = new MatVal(count);
        MatVal matVal3 = new MatVal(count);
        ContourUtil contourUtil = new ContourUtil();
        int counter = 0;
        foreach (List<shpPoint3D> allcontour2 in allcontours)
        {
            if (allcontour2.Count < 3 || allcontour2[0].Z() < num)
            {
                continue;
            }
            double[] array = new double[2 * allcontour2.Count];
            int otherValue = 0;
            foreach (shpPoint3D item in allcontour2)
            {
                array[otherValue] = item.X();
                array[otherValue + allcontour2.Count] = item.Y();
                otherValue++;
            }
            MatVal x = new MatVal(array, null, allcontour2.Count, 2, allcontour2.Count);
            matVal.set_1(counter, x);
            matVal2.set_1(counter, allcontour2[1].Z());
            matVal3.set_1(counter, contourUtil.ContourGradient(allcontour2, m_mesh, m_F));
            counter++;
        }
        if (!Dim.Eq_b(m_Fdim, m_Ldim))
        {
            matVal2.m_dim = Dim.Div(m_Fdim, m_Ldim);
        }
        else
        {
            matVal2.m_dim = m_Fdim;
        }
        matVal.set_1(counter++, matVal2);
        matVal.set_1(counter++, matVal3);
        MatVal matVal4 = new MatVal(4);
        matVal4.set_1(0, m_rect.xmin);
        matVal4.set_1(1, m_rect.xmax);
        matVal4.set_1(2, m_rect.ymin);
        matVal4.set_1(3, m_rect.ymax);
        matVal.set_1(counter++, matVal4);
        return matVal;
    }
}
