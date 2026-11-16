#nullable disable
#pragma warning disable

using Quire.Efi;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class MonteCarlo
{
    private Function m_G;

    private int m_nvars;

    private int m_nsamples;

    private NumVal[] m_nomvals;

    private double[] m_nomvalsd;

    private double[] m_lo;

    private double[] m_hi;

    private bool m_udist;

    private bool m_clip;

    private uint m_LArgNo;

    private uint m_DArgNo;

    private double[] m_sigma;

    private Function[] m_distfn;

    private MatVal m_T;

    public MonteCarlo(Function G, int nsamples, MatVal vals)
    {
        m_clip = false;
        m_udist = false;
        m_LArgNo = 0u;
        m_DArgNo = 0u;
        m_G = G;
        m_nvars = vals.Rows;
        m_nsamples = nsamples;
        if (G.Purity != m_nvars)
        {
            AppException.Raise("Wrong number of Arguments", 1u);
        }
        m_nomvals = new NumVal[m_nvars];
        m_nomvalsd = new double[m_nvars];
        m_sigma = new double[m_nvars];
        for (int i = 0; i < m_nvars; i++)
        {
            if (!(vals.get_2(i, 0) is StrVal) && !(vals.get_2(i, 0) is NumVal { IsInt: not 0 }))
            {
                AppException.Raise("invalid label", 3u);
            }
            NumVal numVal2 = vals.get_2(i, 1) as NumVal;
            if (numVal2 == null || numVal2.Data.ip != 0.0)
            {
                AppException.Raise("mean must be real", 3u);
            }
            m_nomvals[i] = new NumVal(numVal2, numVal2.m_dim);
            m_nomvalsd[i] = numVal2.Data.rp;
            NumVal numVal3 = vals.get_2(i, 2) as NumVal;
            if (numVal3 == null || numVal3.Data.ip != 0.0)
            {
                AppException.Raise("stdev must be real", 3u);
            }
            if (numVal3.Data.rp < 0.0)
            {
                AppException.Raise("stdev must be non-negative", 3u);
            }
            if (!Dim.Eq_b(numVal3.m_dim, numVal2.m_dim))
            {
                AppException.Raise("inconsistent units", 3u);
            }
            m_sigma[i] = numVal3.Data.rp;
        }
        m_T = new MatVal(m_nsamples + 1, m_nvars + 1);
        m_T.m_dim = Dim.identity;
        for (int j = 0; j < m_nvars; j++)
        {
            m_T.set_2(0, j, vals.get_2(j, 0));
        }
        m_T.set_2(0, m_nvars, new StrVal("Output"));
    }

    public MonteCarlo(Function G, int nsamples, MatVal vals, MatVal opt1)
        : this(G, nsamples, vals)
    {
        if (opt1.Rows != m_nvars)
        {
            AppException.Raise("argument dimension mismatch", 4u);
        }
        if (opt1.Cols == 2)
        {
            m_LArgNo = 4u;
            readLimits(opt1);
        }
        else if (opt1.Cols == 1)
        {
            m_DArgNo = 4u;
            readDists(opt1);
        }
        else
        {
            AppException.Raise("invalid argument", 4u);
        }
    }

    public MonteCarlo(Function G, int nsamples, MatVal vals, MatVal opt1, MatVal opt2)
        : this(G, nsamples, vals)
    {
        if (opt1.Rows != m_nvars)
        {
            AppException.Raise("argument dimension mismatch", 4u);
        }
        if (opt2.Rows != m_nvars)
        {
            AppException.Raise("argument dimension mismatch", 4u);
        }
        if (opt1.Cols == 2 && opt2.Cols == 1)
        {
            m_LArgNo = 4u;
            m_DArgNo = 5u;
            readLimits(opt1);
            readDists(opt2);
        }
        else if (opt1.Cols == 1 && opt2.Cols == 2)
        {
            m_LArgNo = 5u;
            m_DArgNo = 4u;
            readLimits(opt2);
            readDists(opt1);
        }
        else
        {
            AppException.Raise("invalid argument");
        }
    }

    private void readLimits(MatVal limits)
    {
        m_clip = true;
        m_hi = new double[m_nvars];
        m_lo = new double[m_nvars];
        NumVal numVal = null;
        for (int i = 0; i < m_nvars; i++)
        {
            numVal = limits.get_2(i, 0) as NumVal;
            if (numVal == null)
            {
                AppException.Raise("must be real matrix", m_LArgNo);
            }
            if (!double.IsNaN(numVal.Data.rp))
            {
                if (!Dim.Eq_b(numVal.m_dim, m_nomvals[i].m_dim))
                {
                    AppException.Raise("inconsistent units", m_LArgNo);
                }
                if (numVal.Data.rp >= m_nomvalsd[i])
                {
                    AppException.Raise("lower limit must be smaller than mean", m_LArgNo);
                }
            }
            m_lo[i] = numVal.Data.rp;
            numVal = limits.get_2(i, 1) as NumVal;
            if (numVal == null)
            {
                AppException.Raise("must be real matrix", m_LArgNo);
            }
            if (!double.IsNaN(numVal.Data.rp))
            {
                if (!Dim.Eq_b(numVal.m_dim, m_nomvals[i].m_dim))
                {
                    AppException.Raise("inconsistent units", m_LArgNo);
                }
                if (numVal.Data.rp <= m_nomvalsd[i])
                {
                    AppException.Raise("upper limit must be greater than mean", m_LArgNo);
                }
            }
            m_hi[i] = numVal.Data.rp;
        }
    }

    private void readDists(MatVal D)
    {
        m_udist = true;
        m_distfn = new Function[m_nvars];
        for (int i = 0; i < m_nvars; i++)
        {
            m_distfn[i] = D.get_1(i) as Function;
            if (m_distfn[i] == null)
            {
                if (!(D.get_2(i, 0) is NumVal numVal) || !double.IsNaN(numVal.Data.rp))
                {
                    AppException.Raise("must be vector of functions", m_DArgNo);
                }
            }
            else if (m_distfn[i].Purity != 1)
            {
                AppException.Raise("function must take one argument only", m_DArgNo);
            }
        }
    }

    private bool clip(int vidx, double rval)
    {
        if (!m_clip)
        {
            return false;
        }
        if (!double.IsNaN(m_lo[vidx]) && rval < m_lo[vidx])
        {
            return true;
        }
        if (!double.IsNaN(m_hi[vidx]) && rval > m_hi[vidx])
        {
            return true;
        }
        return false;
    }

    private double sample(int vidx)
    {
        if (m_udist)
        {
            if (m_distfn[vidx] == null)
            {
                return StatisticsExports.normal();
            }
            NumVal numVal = m_distfn[vidx].Apply1(new NumVal(1)) as NumVal;
            if (numVal == null || numVal.Data.ip != 0.0)
            {
                AppException.Raise("function eval not pure real", m_DArgNo);
            }
            return numVal.Data.rp;
        }
        return StatisticsExports.normal();
    }
}
