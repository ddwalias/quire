#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class DoeAnlyz
{
    public partial class FactorInfo
    {
        public string m_name;

        public Dim m_unit;

        public bool m_strlvls;

        public int m_nlevels;

        public int[] m_nrpl;

        public int[] m_runlev;

        public double[] m_levels;

        public double[] m_avgRlev;

        public double[] m_sumRlev;

        public Dictionary<string, int> m_str2idx;

        public string[] m_idx2str;

        public FactorInfo()
        {
            m_str2idx = null;
            m_idx2str = null;
        }
    }

    public partial class RunInfo
    {
        public int m_ndups;

        public double m_sum;

        public double m_avg;

        public RunInfo()
        {
            m_ndups = 0;
            m_sum = 0.0;
            m_avg = 0.0;
        }
    }

    protected MatVal m_A;

    protected MatVal m_Y;

    protected Array m_factorInfo;

    protected RunInfo[] m_runInfo;

    protected Dim m_Ydim;

    protected Dim m_Ydim2;

    protected int m_nobsrv;

    protected double m_gsum;

    protected double m_gavg;

    protected double[] m_blockmean;

    protected virtual void setupMainFactors(bool doYstats)
    {
        m_factorInfo = Array.CreateInstance(typeof(FactorInfo), m_nfacts);
        double[] array = new double[m_nruns];
        for (int i = 0; i < m_nfacts; i++)
        {
            FactorInfo factorInfo = new FactorInfo();
            factorInfo.m_name = (m_A.get_2(0, i + 2) as StrVal).m_string;
            factorInfo.m_unit = m_A.get_2_dim(1, i + 2);
            if (m_A.get_2(1, i + 2) is StrVal)
            {
                factorInfo.m_strlvls = true;
                factorInfo.m_str2idx = new Dictionary<string, int>(m_nruns);
                int num = 0;
                for (int j = 0; j < m_nruns; j++)
                {
                    StrVal strVal = m_A.get_2(j + 1, i + 2) as StrVal;
                    if (factorInfo.m_str2idx.ContainsKey(strVal.m_string))
                    {
                        array[j] = factorInfo.m_str2idx[strVal.m_string];
                        continue;
                    }
                    array[j] = num;
                    factorInfo.m_str2idx.Add(strVal.m_string, num++);
                }
                factorInfo.m_idx2str = new string[num];
                factorInfo.m_str2idx.Keys.CopyTo(factorInfo.m_idx2str, 0);
            }
            else
            {
                factorInfo.m_strlvls = false;
                for (int k = 0; k < m_nruns; k++)
                {
                    array[k] = m_A.get_real_mixed(k + 1, i + 2);
                }
            }
            setupFactor(array, factorInfo, doYstats);
            m_factorInfo.SetValue(factorInfo, i);
        }
    }

    protected virtual void setupFactor(double[] runs, FactorInfo effect, bool doYstats)
    {
        double[] array = new double[m_nruns];
        Array.Copy(runs, array, m_nruns);
        Array.Sort(array);
        int num = 0;
        double num2 = array[0];
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        dictionary.Add(num2.ToString(), 0);
        for (int i = 1; i < m_nruns; i++)
        {
            if (array[i] != num2)
            {
                num2 = array[i];
                dictionary.Add(num2.ToString(), ++num);
            }
        }
        effect.m_nlevels = num + 1;
        effect.m_levels = new double[num + 1];
        num2 = array[0];
        num = 0;
        effect.m_levels[0] = num2;
        for (int j = 1; j < m_nruns; j++)
        {
            if (array[j] != num2)
            {
                num2 = array[j];
                effect.m_levels[++num] = num2;
            }
        }
        effect.m_runlev = new int[m_nruns];
        for (int k = 0; k < m_nruns; k++)
        {
            num2 = runs[k];
            num = dictionary[num2.ToString()];
            effect.m_runlev[k] = num;
        }
        effect.m_nrpl = new int[effect.m_nlevels];
        for (int l = 0; l < effect.m_nlevels; l++)
        {
            effect.m_nrpl[l] = 0;
        }
        for (int m = 0; m < m_nruns; m++)
        {
            effect.m_nrpl[effect.m_runlev[m]]++;
        }
        if (doYstats)
        {
            effect.m_avgRlev = new double[effect.m_nlevels];
            effect.m_sumRlev = new double[effect.m_nlevels];
            for (int n = 0; n < effect.m_nlevels; n++)
            {
                effect.m_avgRlev[n] = 0.0;
                effect.m_sumRlev[n] = 0.0;
            }
            for (int num3 = 0; num3 < m_nruns; num3++)
            {
                effect.m_sumRlev[effect.m_runlev[num3]] += m_runInfo[num3].m_sum;
                effect.m_avgRlev[effect.m_runlev[num3]] += m_runInfo[num3].m_avg;
            }
            for (int num4 = 0; num4 < effect.m_nlevels; num4++)
            {
                effect.m_avgRlev[num4] /= effect.m_nrpl[num4];
            }
        }
    }

    protected virtual bool analyzeObservations(out bool hasNaN)
    {
        m_Ydim = m_Y.m_dim;
        m_Ydim2 = Dim.Pwr(m_Ydim, 2.0);
        m_gavg = 0.0;
        m_gsum = 0.0;
        m_nobsrv = 0;
        hasNaN = false;
        m_runInfo = new RunInfo[m_nruns];
        for (int i = 0; i < m_nruns; i++)
        {
            m_runInfo[i] = new RunInfo();
            for (int j = 0; j < m_Y.Cols; j++)
            {
                double num = m_Y.get_real_mixed(i, j);
                if (!double.IsNaN(num))
                {
                    m_runInfo[i].m_ndups++;
                    m_runInfo[i].m_sum += num;
                }
                else
                {
                    hasNaN = true;
                }
            }
            if (m_runInfo[i].m_ndups == 0)
            {
                return false;
            }
            m_runInfo[i].m_avg = m_runInfo[i].m_sum / (double)m_runInfo[i].m_ndups;
            m_nobsrv += m_runInfo[i].m_ndups;
            m_gsum += m_runInfo[i].m_sum;
        }
        m_gavg = m_gsum / (double)m_nobsrv;
        return true;
    }

    public MatVal averageDuplicateRuns(MatVal Yij, ArrayList dups, out int ndata)
    {
        bool[] array = new bool[Yij.Rows];
        for (int i = 0; i < Yij.Rows; i++)
        {
            array[i] = false;
        }
        int num = 0;
        for (int j = 0; j < dups.Count; j++)
        {
            List<int> list = dups[j] as List<int>;
            num += list.Count;
            for (int k = 1; k < list.Count; k++)
            {
                array[list[k]] = true;
            }
        }
        MatVal matVal = new MatVal(Yij.Rows - num + dups.Count);
        matVal.m_dim = Yij.m_dim;
        int[] array2 = new int[Yij.Rows];
        num = 0;
        for (int l = 0; l < Yij.Rows; l++)
        {
            if (!array[l])
            {
                array2[l] = num++;
            }
        }
        ndata = 0;
        for (int m = 0; m < dups.Count; m++)
        {
            List<int> list2 = dups[m] as List<int>;
            num = 0;
            double num2 = 0.0;
            foreach (int item in list2)
            {
                for (int n = 0; n < Yij.Cols; n++)
                {
                    double num3 = Yij.get_real_mixed(item, n);
                    if (!double.IsNaN(num3))
                    {
                        num++;
                        num2 += num3;
                        ndata++;
                    }
                }
            }
            if (num == 0)
            {
                AppException.Raise("nan", 2u);
            }
            matVal.set_1(array2[list2[0]], num2 /= (double)num);
        }
        for (int num4 = 0; num4 < Yij.Rows; num4++)
        {
            if (array[num4])
            {
                continue;
            }
            num = 0;
            double num5 = 0.0;
            for (int num6 = 0; num6 < Yij.Cols; num6++)
            {
                double num7 = Yij.get_real_mixed(num4, num6);
                if (!double.IsNaN(num7))
                {
                    num++;
                    num5 += num7;
                    ndata++;
                }
            }
            if (num == 0)
            {
                AppException.Raise("nan", 2u);
            }
            matVal.set_1(array2[num4], num5 /= (double)num);
        }
        return matVal;
    }

    internal override double[,] getDesignMatrix()
    {
        return null;
    }
}
