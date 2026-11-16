#nullable disable
#pragma warning disable

using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.DOE;

internal partial class DoeLabel
{
    public DoeLabel(MatVal A)
    {
        m_A = A;
        m_nfacts = A.Cols - 2;
        m_nruns = A.Rows - 1;
        setupMainFactors(doYstats: false);
    }

    public MatVal coded2Real(MatVal Vals)
    {
        check_units(Vals);
        MatVal matVal = new MatVal(m_A);
        if (m_nfacts > Vals.Rows)
        {
            AppException.Raise("missing factor label", 2u);
        }
        for (int i = 0; i < m_nfacts; i++)
        {
            FactorInfo factorInfo = (FactorInfo)m_factorInfo.GetValue(i);
            StrVal strVal = Vals.get_2(i, 0) as StrVal;
            if (strVal == null)
            {
                AppException.Raise("factor label must be a string", 2u);
            }
            matVal.set_2(0, i + 2, strVal);
            for (int j = 0; j < m_nruns; j++)
            {
                int num = factorInfo.m_runlev[j];
                if (num + 1 >= Vals.Cols)
                {
                    AppException.Raise("missing level value", 2u);
                }
                object obj = Vals.get_2(i, num + 1);
                if (obj is NumVal numVal)
                {
                    if (double.IsNaN(numVal.Data.rp) || double.IsNaN(numVal.Data.ip))
                    {
                        AppException.Raise("missing or invalid level value", 2u);
                    }
                }
                else if (!(obj is StrVal))
                {
                    AppException.Raise("invalid level value", 2u);
                }
                matVal.set_2(j + 1, i + 2, obj);
            }
        }
        return matVal;
    }

    private void check_units(MatVal Vals)
    {
        if (m_nfacts > Vals.Rows)
        {
            AppException.Raise("missing factor label", 2u);
        }
        for (int i = 0; i < m_nfacts; i++)
        {
            FactorInfo factorInfo = (FactorInfo)m_factorInfo.GetValue(i);
            Dim x = null;
            if (Vals.Cols <= factorInfo.m_nlevels)
            {
                AppException.Raise("missing level value", 2u);
            }
            for (int j = 0; j < factorInfo.m_nlevels; j++)
            {
                object obj = Vals.get_2(i, j + 1);
                if (obj is NumVal numVal)
                {
                    if (j == 0)
                    {
                        x = numVal.m_dim;
                    }
                    else
                    {
                        Dim.Eq(x, numVal.m_dim);
                    }
                }
                else if (!(obj is StrVal))
                {
                    AppException.Raise("invalid level value", 2u);
                }
            }
        }
    }
}
