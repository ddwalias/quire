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
        double value = array[0];
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        dictionary.Add(value.ToString(), 0);
        for (int i = 1; i < m_nruns; i++)
        {
            if (array[i] != value)
            {
                value = array[i];
                dictionary.Add(value.ToString(), ++num);
            }
        }
        effect.m_nlevels = num + 1;
        effect.m_levels = new double[num + 1];
        value = array[0];
        num = 0;
        effect.m_levels[0] = value;
        for (int j = 1; j < m_nruns; j++)
        {
            if (array[j] != value)
            {
                value = array[j];
                effect.m_levels[++num] = value;
            }
        }
        effect.m_runlev = new int[m_nruns];
        for (int k = 0; k < m_nruns; k++)
        {
            value = runs[k];
            num = dictionary[value.ToString()];
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
            for (int index = 0; index < m_nruns; index++)
            {
                effect.m_sumRlev[effect.m_runlev[index]] += m_runInfo[index].m_sum;
                effect.m_avgRlev[effect.m_runlev[index]] += m_runInfo[index].m_avg;
            }
            for (int otherValue = 0; otherValue < effect.m_nlevels; otherValue++)
            {
                effect.m_avgRlev[otherValue] /= effect.m_nrpl[otherValue];
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
        int[] indices = new int[Yij.Rows];
        num = 0;
        for (int l = 0; l < Yij.Rows; l++)
        {
            if (!array[l])
            {
                indices[l] = num++;
            }
        }
        ndata = 0;
        for (int m = 0; m < dups.Count; m++)
        {
            List<int> items = dups[m] as List<int>;
            num = 0;
            double value = 0.0;
            foreach (int item in items)
            {
                for (int n = 0; n < Yij.Cols; n++)
                {
                    double otherValue = Yij.get_real_mixed(item, n);
                    if (!double.IsNaN(otherValue))
                    {
                        num++;
                        value += otherValue;
                        ndata++;
                    }
                }
            }
            if (num == 0)
            {
                AppException.Raise("nan", 2u);
            }
            matVal.set_1(indices[items[0]], value /= (double)num);
        }
        for (int rowIndex = 0; rowIndex < Yij.Rows; rowIndex++)
        {
            if (array[rowIndex])
            {
                continue;
            }
            num = 0;
            double nextValue = 0.0;
            for (int columnIndex = 0; columnIndex < Yij.Cols; columnIndex++)
            {
                double currentValue = Yij.get_real_mixed(rowIndex, columnIndex);
                if (!double.IsNaN(currentValue))
                {
                    num++;
                    nextValue += currentValue;
                    ndata++;
                }
            }
            if (num == 0)
            {
                AppException.Raise("nan", 2u);
            }
            matVal.set_1(indices[rowIndex], nextValue /= (double)num);
        }
        return matVal;
    }

    internal override double[,] getDesignMatrix()
    {
        return null;
    }
}
