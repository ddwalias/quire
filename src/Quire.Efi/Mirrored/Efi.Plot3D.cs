#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Quire.Efi.CalculationAssistant;
using Quire.EfiMatrix;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

public partial class Plot3D
{
    protected Function m_F;

    protected Dim m_Fdim;

    protected MatVal m_mdata;

    protected MatVal m_data;

    protected MatVal m_limits;

    protected MatVal m_gridn;

    protected object m_scale;

    protected object m_expr;

    protected ArrayList m_rngIdx;

    protected RngVal m_u;

    protected RngVal m_v;

    protected object[] m_param;

    protected int m_plotType;

    protected int m_origin;

    private int m_fullyConstrained;

    protected int m_gridNDefault;

    protected double m_rangeIncDefault;

    protected double m_limitMaxDefault;

    protected double m_limitMinDefault;

    public Plot3D(int origin, object exp, object scale, object limits_, object gridn, object[] param)
    {
        m_origin = origin;
        m_expr = exp;
        m_F = exp as Function;
        m_mdata = null;
        m_scale = scale;
        m_fullyConstrained = 0;
        m_limitMaxDefault = 10.0;
        m_limitMinDefault = -10.0;
        m_gridNDefault = 41;
        if (limits_ != null)
        {
            MatVal matVal = Utl.as_rvector(limits_, 3u);
            if (matVal.Rows < 4)
            {
                AppException.Raise("plot3D invalid limits vector", 3u);
            }
            if (matVal.Rows < 6)
            {
                m_limits = new MatVal(6);
                m_limits.set_1(0, matVal.get_1(0));
                m_limits.set_1(1, matVal.get_1(1));
                m_limits.set_1(2, matVal.get_1(2));
                m_limits.set_1(3, matVal.get_1(3));
            }
            else
            {
                m_limits = matVal;
            }
        }
        else
        {
            m_limits = new MatVal(6);
            m_limits.set_1(0, m_limitMinDefault);
            m_limits.set_1(1, m_limitMaxDefault);
            m_limits.set_1(2, m_limitMinDefault);
            m_limits.set_1(3, m_limitMaxDefault);
        }
        m_gridn = null;
        if (gridn != null)
        {
            m_gridn = Utl.as_ivector(gridn, 4u);
            if (m_gridn.Rows != 2)
            {
                AppException.Raise("plot3D invalid grid vector", 4u);
            }
            double num = m_gridn.get_real_mixed(0);
            if (!double.IsNaN(num))
            {
                m_gridNDefault = (int)num;
            }
            else
            {
                num = m_gridn.get_real_mixed(1);
                if (!double.IsNaN(num))
                {
                    m_gridNDefault = (int)num;
                }
            }
            if (m_gridNDefault <= 1)
            {
                AppException.Raise("plot3D invalid grid vector", 4u);
            }
        }
        else
        {
            m_gridn = new MatVal(2);
            m_gridn.set_1(0, m_gridNDefault);
            m_gridn.set_1(1, m_gridNDefault);
        }
        m_rangeIncDefault = (m_limitMaxDefault - m_limitMinDefault) / (double)(m_gridNDefault - 1);
        m_Fdim = Dim.identity;
        m_plotType = -100;
        m_u = null;
        m_v = null;
        m_param = null;
        m_rngIdx = new ArrayList(2);
        if (param != null)
        {
            m_param = param;
            int counter = 0;
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] is RngVal)
                {
                    if (counter == 0)
                    {
                        m_u = param[i] as RngVal;
                    }
                    else
                    {
                        m_v = param[i] as RngVal;
                    }
                    m_rngIdx.Add(i);
                    counter++;
                }
                if (counter > 2)
                {
                    AppException.Raise("plot3D more than 2 ranges found", 1u);
                }
            }
        }
        verifyExpression();
    }

    private NumVal getSamplePoint(int nmb)
    {
        double num;
        double value;
        NumVal result;
        if (nmb == 0)
        {
            num = m_limits.get_real_mixed(0);
            value = m_limits.get_real_mixed(1);
            result = new NumVal(-Math.PI / 7.0);
        }
        else
        {
            num = m_limits.get_real_mixed(2);
            value = m_limits.get_real_mixed(3);
            result = new NumVal(Math.PI / 7.0);
        }
        if (!double.IsNaN(num))
        {
            if (!double.IsNaN(value))
            {
                return new NumVal(num + Math.PI / 7.0 * (value - num));
            }
            return new NumVal(num + 1.0);
        }
        if (!double.IsNaN(value))
        {
            return new NumVal(value - 1.0);
        }
        return result;
    }

    private void testMatrixData()
    {
        if (m_mdata.Cols == 1)
        {
            if (m_mdata.Rows < 3)
            {
                m_plotType = 3;
                return;
            }
            object obj = m_mdata.get_1(0);
            object value = m_mdata.get_1(1);
            object otherValue = m_mdata.get_1(2);
            if (obj is MatVal && value is MatVal && otherValue is MatVal)
            {
                MatVal matVal = obj as MatVal;
                MatVal matVal2 = value as MatVal;
                MatVal matVal3 = otherValue as MatVal;
                if (matVal.Cols != matVal2.Cols || matVal.Rows != matVal2.Rows)
                {
                    AppException.Raise("plot3D inconsistent Create format", 1u);
                }
                if (matVal.Cols != matVal3.Cols || matVal.Rows != matVal3.Rows)
                {
                    AppException.Raise("plot3D inconsistent Create format", 1u);
                }
                if (m_mdata.Rows != 3)
                {
                    AppException.Raise("plot3D unsupported data format", 1u);
                }
                if (!matVal.IsUniformScalar || !matVal2.IsUniformScalar || !matVal3.IsUniformScalar)
                {
                    AppException.Raise("plot3D non uniform data", 1u);
                }
                if (matVal.Cols == 1)
                {
                    m_plotType = 5;
                    return;
                }
                if (matVal.Rows < 2)
                {
                    AppException.Raise("plot3D inconsistent Create format", 1u);
                }
                m_plotType = 6;
            }
            else if (obj is Function || value is Function || otherValue is Function)
            {
                m_F = Function.VectorFunction(m_mdata);
                testSimpleFunction();
            }
            else if (obj is NumVal && value is NumVal && otherValue is NumVal)
            {
                m_plotType = 3;
            }
            else
            {
                AppException.Raise("plot3D unsupported data format", 1u);
            }
        }
        else if (m_mdata.Cols == 3)
        {
            m_plotType = 4;
        }
        else
        {
            if (!m_mdata.IsUniformScalar)
            {
                AppException.Raise("plot3D non uniform data", 1u);
            }
            m_plotType = 3;
        }
    }

    public MatVal eval_3d_plot()
    {
        switch (m_plotType)
        {
            case 0:
                return paramCurve();
            case 1:
                return parametricSurface();
            case 2:
                return explicitSurface();
            case 3:
                m_fullyConstrained = 1;
                return MappedMatrixData();
            case 4:
                m_fullyConstrained = 1;
                return ScatteredData();
            case 5:
            case 6:
                m_fullyConstrained = 1;
                return CreateData();
            case -1:
                m_plotType = 0;
                return explicitCurve2D();
            case -2:
                m_plotType = 2;
                return explicitSurface_rangized();
            default:
                AppException.Raise("plot3D unsupported data format", 1u);
                return null;
        }
    }

    private void setLimits(MatVal Z, int idx)
    {
        double num = double.PositiveInfinity;
        double maximum = double.NegativeInfinity;
        Dim dim = Z.m_dim;
        for (int i = 0; i < Z.Cols; i++)
        {
            for (int j = 0; j < Z.Rows; j++)
            {
                double value = Z.get_real_mixed(j, i);
                if (value < num)
                {
                    num = value;
                }
                if (value > maximum)
                {
                    maximum = value;
                }
            }
        }
        m_limits.set_1(idx, num, dim);
        m_limits.set_1(idx + 1, maximum, dim);
    }

    protected MatVal explicitSurface_rangized()
    {
        MatVal matVal = new MatVal(m_u.Isize, m_v.Isize);
        MatVal matVal2 = new MatVal(m_u.Isize, m_v.Isize);
        MatVal matVal3 = new MatVal(m_u.Isize, m_v.Isize);
        int num = 0;
        for (int i = 0; i < m_v.Isize; i++)
        {
            NumVal numVal = m_v.range_get(i) as NumVal;
            m_param[(int)m_rngIdx[1]] = numVal;
            for (int j = 0; j < m_u.Isize; j++)
            {
                NumVal numVal2 = m_u.range_get(j) as NumVal;
                m_param[(int)m_rngIdx[0]] = numVal2;
                NumVal numVal3 = m_F.Apply(m_param) as NumVal;
                if (numVal3 == null)
                {
                    AppException.Raise("plot3D function must return scalar or 3 element vector", 1u);
                }
                if (!numVal2.IsDimless || !matVal2.IsDimless)
                {
                    AppException.Raise("plot3D inconsistent units", 1u);
                }
                if (m_scale != null)
                {
                    numVal3 = values.op_scale_inverse(numVal3, m_scale) as NumVal;
                }
                m_Fdim = numVal3.m_dim;
                matVal.set_1(num, numVal2);
                matVal2.set_1(num, numVal);
                matVal3.set_1(num, numVal3);
                num++;
            }
        }
        return packResult(matVal, matVal2, matVal3);
    }

    protected MatVal explicitSurface_rangized(bool try_multithreading)
    {
        MatVal X = new MatVal(m_u.Isize, m_v.Isize);
        MatVal Y = new MatVal(m_u.Isize, m_v.Isize);
        MatVal Z = new MatVal(m_u.Isize, m_v.Isize);
        if (!try_multithreading || m_v.Isize < CalculationAssistantAdHoc.Threads || m_u.Isize < CalculationAssistantAdHoc.Threads || !try_calc_mt_explicitSurface_range(ref X, ref Y, ref Z))
        {
            if (Rts.IsCancellationRequested())
            {
                throw new AbortRequestedException();
            }
            explicitSurfaceRangized_loop_function(ref X, ref Y, ref Z, ref m_param, 0, m_v.Isize - 1, 0, m_u.Isize - 1, 0, ref m_Fdim, CancellationToken.None);
        }
        return packResult(X, Y, Z);
    }

    private MatVal MappedMatrixData()
    {
        if (m_scale != null)
        {
            m_mdata = values.op_scale_inverse(m_mdata, m_scale) as MatVal;
        }
        m_Fdim = m_mdata.m_dim;
        if (!m_mdata.IsUniformScalar)
        {
            AppException.Raise("plot3D non uniform data", 1u);
        }
        MatVal matVal = new MatVal(m_mdata.Rows, m_mdata.Cols);
        MatVal matVal2 = new MatVal(m_mdata.Rows, m_mdata.Cols);
        MatVal matVal3 = new MatVal(m_mdata);
        matVal.m_dim = Dim.identity;
        matVal2.m_dim = Dim.identity;
        for (int i = 0; i < m_mdata.Cols; i++)
        {
            for (int j = 0; j < m_mdata.Rows; j++)
            {
                matVal.set_2(j, i, j + m_origin);
                matVal2.set_2(j, i, i + m_origin);
            }
        }
        if (m_mdata.Cols == 1 && m_mdata.Rows == 1)
        {
            m_plotType = 4;
        }
        else if (m_mdata.Cols == 1 || m_mdata.Rows == 1)
        {
            m_plotType = 0;
            if (m_mdata.Rows == 1)
            {
                matVal = matVal.Transpose();
                matVal2 = matVal2.Transpose();
                matVal3 = matVal3.Transpose();
            }
        }
        m_data = new MatVal(3);
        m_data.set_1(0, matVal);
        m_data.set_1(1, matVal2);
        m_data.set_1(2, matVal3);
        m_limits.set_1(0, m_origin);
        m_limits.set_1(1, m_mdata.Rows - 1 + m_origin);
        m_limits.set_1(2, m_origin);
        m_limits.set_1(3, m_mdata.Cols - 1 + m_origin);
        setLimits(m_mdata, 4);
        MatVal matVal4 = new MatVal(4);
        matVal4.set_1(0, m_plotType, Dim.identity);
        matVal4.set_1(1, m_data);
        matVal4.set_1(2, m_limits);
        matVal4.set_1(3, m_fullyConstrained, Dim.identity);
        return matVal4;
    }

    private MatVal ScatteredData()
    {
        MatVal matVal = new MatVal(m_mdata.Column(0));
        MatVal matVal2 = new MatVal(m_mdata.Column(1));
        MatVal matVal3 = new MatVal(m_mdata.Column(2));
        if (!matVal.IsUniformScalar || !matVal2.IsUniformScalar || !matVal3.IsUniformScalar)
        {
            AppException.Raise("plot3D non uniform data", 1u);
        }
        if (!matVal.IsDimless || !matVal2.IsDimless)
        {
            AppException.Raise("plot3D inconsistent units", 1u);
        }
        if (m_scale != null)
        {
            matVal3 = values.op_scale_inverse(matVal3, m_scale) as MatVal;
        }
        m_Fdim = matVal3.m_dim;
        return packResult(matVal, matVal2, matVal3);
    }

    private MatVal CreateData()
    {
        MatVal matVal = m_mdata.get_1(0) as MatVal;
        MatVal matVal2 = m_mdata.get_1(1) as MatVal;
        MatVal matVal3 = m_mdata.get_1(2) as MatVal;
        if (!matVal.IsUniformScalar || !matVal2.IsUniformScalar || !matVal3.IsUniformScalar)
        {
            AppException.Raise("plot3D non uniform data", 1u);
        }
        if (!matVal.IsDimless || !matVal2.IsDimless)
        {
            AppException.Raise("plot3D inconsistent units", 1u);
        }
        if (m_scale != null)
        {
            matVal3 = values.op_scale_inverse(matVal3, m_scale) as MatVal;
        }
        m_Fdim = matVal3.m_dim;
        return packResult(matVal, matVal2, matVal3);
    }

    private MatVal paramCurve()
    {
        MatVal matVal = new MatVal(m_u.Isize);
        MatVal matVal2 = new MatVal(m_u.Isize);
        MatVal matVal3 = new MatVal(m_u.Isize);
        for (int i = 0; i < m_u.Isize; i++)
        {
            m_param[(int)m_rngIdx[0]] = m_u.range_get(i);
            MatVal matVal4 = m_F.Apply(m_param) as MatVal;
            if (matVal4 == null || matVal4.Cols != 1 || matVal4.Rows != 3)
            {
                AppException.Raise("plot3D function must return scalar or 3 element vector", 1u);
            }
            NumVal numVal = matVal4.get_1(0) as NumVal;
            NumVal numVal2 = matVal4.get_1(1) as NumVal;
            NumVal numVal3 = matVal4.get_1(2) as NumVal;
            if (numVal == null || numVal2 == null || numVal3 == null)
            {
                AppException.Raise("plot3D unsupported data format", 1u);
            }
            if (!numVal.IsDimless || !matVal2.IsDimless)
            {
                AppException.Raise("plot3D inconsistent units", 1u);
            }
            if (m_scale != null)
            {
                numVal3 = values.op_scale_inverse(numVal3, m_scale) as NumVal;
            }
            m_Fdim = numVal3.m_dim;
            matVal.set_1(i, numVal);
            matVal2.set_1(i, numVal2);
            matVal3.set_1(i, numVal3);
        }
        return packResult(matVal, matVal2, matVal3);
    }

    private MatVal explicitCurve2D()
    {
        MatVal matVal = new MatVal(m_u.Isize);
        MatVal matVal2 = new MatVal(m_u.Isize);
        MatVal matVal3 = new MatVal(m_u.Isize);
        for (int i = 0; i < m_u.Isize; i++)
        {
            NumVal numVal = m_u.range_get(i) as NumVal;
            m_param[(int)m_rngIdx[0]] = numVal;
            NumVal numVal2 = m_F.Apply(m_param) as NumVal;
            if (numVal2 == null)
            {
                AppException.Raise("plot3D function must return scalar or 3 element vector", 1u);
            }
            if (!numVal.IsDimless)
            {
                AppException.Raise("plot3D inconsistent units", 1u);
            }
            if (m_scale != null)
            {
                numVal2 = values.op_scale_inverse(numVal2, m_scale) as NumVal;
            }
            m_Fdim = numVal2.m_dim;
            matVal.set_1(i, numVal);
            matVal2.set_1(i, 0.0);
            matVal3.set_1(i, numVal2);
        }
        return packResult(matVal, matVal2, matVal3);
    }

    protected MatVal parametricSurface()
    {
        MatVal matVal = new MatVal(m_u.Isize, m_v.Isize);
        MatVal matVal2 = new MatVal(m_u.Isize, m_v.Isize);
        MatVal matVal3 = new MatVal(m_u.Isize, m_v.Isize);
        int num = 0;
        for (int i = 0; i < m_v.Isize; i++)
        {
            m_param[(int)m_rngIdx[1]] = m_v.range_get(i);
            for (int j = 0; j < m_u.Isize; j++)
            {
                m_param[(int)m_rngIdx[0]] = m_u.range_get(j);
                MatVal matVal4 = m_F.Apply(m_param) as MatVal;
                if (matVal4 == null || matVal4.Cols != 1 || matVal4.Rows != 3)
                {
                    AppException.Raise("plot3D function must return scalar or 3 element vector", 1u);
                }
                NumVal numVal = matVal4.get_1(0) as NumVal;
                NumVal numVal2 = matVal4.get_1(1) as NumVal;
                NumVal numVal3 = matVal4.get_1(2) as NumVal;
                if (numVal == null || numVal2 == null || numVal3 == null)
                {
                    AppException.Raise("plot3D unsupported data format", 1u);
                }
                if (!numVal.IsDimless || !matVal2.IsDimless)
                {
                    AppException.Raise("plot3D inconsistent units", 1u);
                }
                if (m_scale != null)
                {
                    numVal3 = values.op_scale_inverse(numVal3, m_scale) as NumVal;
                }
                m_Fdim = numVal3.m_dim;
                matVal.set_1(num, numVal);
                matVal2.set_1(num, numVal2);
                matVal3.set_1(num, numVal3);
                num++;
            }
        }
        return packResult(matVal, matVal2, matVal3);
    }

    private MatVal packResult(MatVal X, MatVal Y, MatVal Z)
    {
        X.m_dim = Dim.identity;
        Y.m_dim = Dim.identity;
        m_data = new MatVal(3);
        m_data.set_1(0, X);
        m_data.set_1(1, Y);
        m_data.set_1(2, Z);
        setLimits(X, 0);
        setLimits(Y, 2);
        setLimits(Z, 4);
        MatVal matVal = new MatVal(4);
        matVal.set_1(0, m_plotType, Dim.identity);
        matVal.set_1(1, m_data);
        matVal.set_1(2, m_limits);
        matVal.set_1(3, m_fullyConstrained, Dim.identity);
        return matVal;
    }
}
