#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using Quire.Efi;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class Anova
{
    private partial class FactorAnova : FactorInfo
    {
        public int m_df;

        public double m_msb;

        public double m_ssb;
    }

    public partial class vectorComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] - y[i] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(int[] x)
        {
            int num = 0;
            for (int i = 0; i < x.Length; i++)
            {
                num += (i + 1) * x[i];
            }
            return num;
        }
    }

    private int m_blocks_df;

    private double m_blocks_ssb;

    private double m_blocks_msb;

    private int m_dfe;

    private double m_sse;

    private double m_mse;

    private int m_dft;

    private double m_sst;

    private MatVal m_T;

    private int m_neffects;

    private int m_nYdups;

    private bool m_is_2level;

    private bool m_is_fullfact;

    private Dictionary<int[], double> m_effectSS;

    protected Anova()
    {
    }

    public Anova(MatVal A, MatVal Y)
    {
        m_A = A;
        m_Y = Y;
        m_nfacts = A.Cols - 2;
        m_nruns = A.Rows - 1;
        analyzeObservations(out var hasNaN);
        if (hasNaN)
        {
            AppException.Raise("missing observations not supported", 2u);
        }
        setupMainFactors(doYstats: true);
        analizeDesignMarix();
        BlocksInfo();
    }

    protected override void setupMainFactors(bool doYstats)
    {
        m_factorInfo = Array.CreateInstance(typeof(FactorAnova), m_nfacts);
        double[] array = new double[m_nruns];
        for (int i = 0; i < m_nfacts; i++)
        {
            FactorAnova factorAnova = new FactorAnova();
            factorAnova.m_name = (m_A.get_2(0, i + 2) as StrVal).m_string;
            factorAnova.m_unit = m_A.get_2_dim(1, i + 2);
            for (int j = 0; j < m_nruns; j++)
            {
                array[j] = m_A.get_real_mixed(j + 1, i + 2);
            }
            setupFactor(array, factorAnova, doYstats);
            m_factorInfo.SetValue(factorAnova, i);
        }
    }

    private void doERROR()
    {
        m_sse = 0.0;
        m_dfe = m_dft;
        for (int i = 1; i < m_T.Rows - 2; i++)
        {
            m_sse += (m_T.get_2(i, 1) as NumVal).Data.rp;
            m_dfe -= (int)(m_T.get_2(i, 2) as NumVal).Data.rp;
        }
        m_sse = m_sst - m_sse;
        m_mse = m_sse / (double)m_dfe;
        if (m_sse < 0.0)
        {
            AppException.Raise("Anova can not be computed. Unsupported design or insufficient resolution", 3u);
        }
        for (int j = 1; j < m_T.Rows - 2; j++)
        {
            double rp = (m_T.get_2(j, 3) as NumVal).Data.rp;
            double rp2 = (m_T.get_2(j, 2) as NumVal).Data.rp;
            if (rp2 <= 0.0)
            {
                AppException.Raise("Anova can not be computed. Unsupported design or insufficient resolution", 3u);
            }
            double r = rp / m_mse;
            double r2 = 1.0 - (Statistics.PF(new NumVal(r), new NumVal(rp2), new NumVal(m_dfe)) as NumVal).asReal();
            m_T.set_2(j, 4, r);
            m_T.set_2(j, 5, r2);
        }
    }

    public bool anovaTable(out MatVal table, string effects)
    {
        if (!m_is_2level && !m_is_fullfact)
        {
            AppException.Raise("interactions can not be computed", 3u);
        }
        string[] array = effects.Split(new char[5] { ' ', ',', '\t', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
        int num = 0;
        string[] array2 = array;
        foreach (string text in array2)
        {
            array[num++] = text.ToUpper();
        }
        if (!m_utils.verifyUniqueGens(m_nfacts, array))
        {
            AppException.Raise("invalid or non unique effect", 3u);
        }
        anovaMain();
        m_neffects = array.Length;
        int r = m_neffects + 3 + ((m_nblocks > 1) ? 1 : 0);
        table = new MatVal(r, 6);
        table.m_dim = Dim.identity;
        m_T = table;
        anovaTable(-1);
        anova_interactions(array);
        anovaTable(0);
        return true;
    }

    public bool anovaTable(out MatVal table, int order)
    {
        if (order > m_nfacts)
        {
            AppException.Raise("invalid interaction order", 3u);
        }
        if (order >= 2 && !m_is_2level && !m_is_fullfact)
        {
            AppException.Raise("interactions can not be computed", 3u);
        }
        m_neffects = 0;
        for (int i = 2; i <= order; i++)
        {
            m_neffects += Utils.combination(m_nfacts, i);
        }
        anovaMain();
        int r = m_nfacts + m_neffects + 3 + ((m_nblocks > 1) ? 1 : 0);
        table = new MatVal(r, 6);
        table.m_dim = Dim.identity;
        m_T = table;
        anovaTable(-1);
        anovaTable(-2);
        anova_interactions(order);
        anovaTable(1);
        return true;
    }

    protected void anovaMain()
    {
        m_nYdups = m_Y.Cols;
        m_sst = 0.0;
        m_dft = m_nYdups * m_nruns - 1;
        for (int i = 0; i < m_nruns; i++)
        {
            for (int j = 0; j < m_nYdups; j++)
            {
                double num = m_Y.get_real_mixed(i, j);
                m_sst += (num - m_gavg) * (num - m_gavg);
            }
        }
        m_effectSS = new Dictionary<int[], double>(new vectorComparer());
        for (int k = 0; k < m_nfacts; k++)
        {
            FactorAnova factorAnova = (FactorAnova)m_factorInfo.GetValue(k);
            calcFactorAnova(factorAnova);
            m_effectSS.Add(new int[1] { k }, factorAnova.m_ssb);
        }
    }

    private void calcFactorAnova(FactorAnova effect)
    {
        effect.m_df = effect.m_nlevels - 1;
        effect.m_ssb = 0.0;
        for (int i = 0; i < effect.m_nlevels; i++)
        {
            effect.m_ssb += effect.m_sumRlev[i] * effect.m_sumRlev[i];
        }
        effect.m_ssb *= effect.m_nlevels;
        effect.m_ssb -= m_gsum * m_gsum;
        effect.m_ssb /= m_nYdups * m_nruns;
        effect.m_msb = effect.m_ssb / (double)effect.m_df;
    }

    private double[] anova_effect(int[] f)
    {
        int num = 1;
        for (int i = 0; i < f.Length; i++)
        {
            FactorAnova factorAnova = (FactorAnova)m_factorInfo.GetValue(f[i]);
            num *= factorAnova.m_nlevels - 1;
        }
        double num2 = effect_ssb(f);
        double num3 = num2 / (double)num;
        return new double[3] { num2, num, num3 };
    }

    private double[] anovaEffect(int[] f)
    {
        FactorAnova factorAnova = new FactorAnova();
        double[] array = new double[m_nruns];
        for (int i = 0; i < m_nruns; i++)
        {
            array[i] = 1.0;
            for (int j = 0; j < f.Length; j++)
            {
                FactorAnova factorAnova2 = (FactorAnova)m_factorInfo.GetValue(f[j]);
                array[i] *= factorAnova2.m_levels[factorAnova2.m_runlev[i]];
            }
        }
        factorAnova.m_name = "";
        for (int k = 0; k < f.Length; k++)
        {
            FactorAnova factorAnova3 = (FactorAnova)m_factorInfo.GetValue(f[k]);
            factorAnova.m_name += factorAnova3.m_name;
        }
        setupFactor(array, factorAnova, doYstats: true);
        calcFactorAnova(factorAnova);
        return new double[3] { factorAnova.m_ssb, factorAnova.m_df, factorAnova.m_msb };
    }

    private bool doBLOCKS()
    {
        m_blocks_df = m_nblocks - 1;
        if (m_nblocks == 1)
        {
            m_blocks_ssb = 0.0;
            m_blocks_msb = 0.0;
            return false;
        }
        m_blocks_ssb = 1.0;
        for (int i = 0; i < m_nfacts; i++)
        {
            FactorAnova factorAnova = (FactorAnova)m_factorInfo.GetValue(i);
            m_blocks_ssb *= factorAnova.m_nlevels;
        }
        double num = 0.0;
        for (int j = 0; j < m_nblocks; j++)
        {
            num = (m_blockmean[j] - m_gavg) * (m_blockmean[j] - m_gavg);
        }
        m_blocks_ssb *= num;
        m_blocks_msb = m_blocks_ssb / (double)m_blocks_df;
        return true;
    }

    private double runsmean(int[] runs)
    {
        double num = m_runInfo[runs[0]].m_avg;
        for (int i = 1; i < runs.Length; i++)
        {
            num += m_runInfo[runs[i]].m_avg;
        }
        return num / (double)runs.Length;
    }

}
