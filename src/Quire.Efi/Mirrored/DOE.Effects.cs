#nullable disable
#pragma warning disable

using System;
using System.Collections;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class Effects
{
    private MatVal m_T;

    public Effects()
    {
    }

    public Effects(MatVal A, MatVal Y)
    {
        ArrayList dups = FindDuplicates(A);
        m_A = removeDuplicateRuns(A, dups);
        m_Y = averageDuplicateRuns(Y, dups, out m_nobsrv);
        m_nfacts = A.Cols - 2;
        m_nruns = A.Rows - 1;
        if (!analyzeObservations(out var _))
        {
            AppException.Raise("must have at least one observation per run", 2u);
        }
        setupMainFactors(doYstats: true);
    }

    public bool effectsTable(out MatVal table, string effects)
    {
        effects = effects.ToUpper();
        int num = 0;
        if (effects.Contains("BLOCKS"))
        {
            BlocksInfo();
            if (m_nblocks > 2)
            {
                AppException.Raise("this function supports at most 2 blocks", 1u);
            }
            if (m_nblocks == 2)
            {
                num = 1;
            }
            effects = effects.Replace("BLOCKS", "");
        }
        string[] array = effects.Split(new char[5] { ' ', ',', '\t', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
        int num2 = 0;
        string[] array2 = array;
        foreach (string text in array2)
        {
            array[num2++] = text.ToUpper();
        }
        if (!m_utils.verifyGens(m_nfacts, array))
        {
            AppException.Raise("invalid effect", 3u);
        }
        if (!m_utils.verifyUniqueGens(m_nfacts, array))
        {
            AppException.Raise("invalid or non unique effect", 3u);
        }
        table = new MatVal(array.Length + 1 + num, 4);
        table.m_dim = Dim.identity;
        m_T = table;
        effectsTable(doMain: false);
        int idx = interactionsEffects(array);
        if (num == 1)
        {
            addBlocksRow(idx);
        }
        return true;
    }

    public bool effectsTable(out MatVal table, int order)
    {
        if (order > m_nfacts)
        {
            AppException.Raise("invalid interaction order", 3u);
        }
        int num = 0;
        for (int i = 2; i <= order; i++)
        {
            num += Utils.combination(m_nfacts, i);
        }
        BlocksInfo();
        if (m_nblocks > 2)
        {
            AppException.Raise("this function supports at most 2 blocks", 1u);
        }
        int num2 = 0;
        if (m_nblocks == 2)
        {
            num2 = 1;
        }
        table = new MatVal(m_nfacts + num + 1 + num2, 4);
        table.m_dim = Dim.identity;
        m_T = table;
        effectsTable(doMain: true);
        int idx = interactionsEffects(order);
        if (num2 == 1)
        {
            addBlocksRow(idx);
        }
        return true;
    }

    private void addBlocksRow(int idx)
    {
        m_T.set_2(idx, 0, new StrVal("Blocks"));
        m_T.set_2(idx, 1, m_blockmean[0], m_Ydim);
        m_T.set_2(idx, 2, m_blockmean[1], m_Ydim);
        m_T.set_2(idx, 3, m_blockmean[1] - m_blockmean[0], m_Ydim);
    }

    private void effectsTable(bool doMain)
    {
        m_T.set_2(0, 0, new StrVal("Factor"));
        m_T.set_2(0, 1, new StrVal("(-) Avg"));
        m_T.set_2(0, 2, new StrVal("(+) Avg"));
        m_T.set_2(0, 3, new StrVal("(+) - (-)"));
        if (doMain)
        {
            for (int i = 1; i <= m_nfacts; i++)
            {
                FactorInfo factorInfo = (FactorInfo)m_factorInfo.GetValue(i - 1);
                m_T.set_2(i, 0, new StrVal(factorInfo.m_name));
                m_T.set_2(i, 1, factorInfo.m_avgRlev[0], m_Ydim);
                m_T.set_2(i, 2, factorInfo.m_avgRlev[1], m_Ydim);
                m_T.set_2(i, 3, factorInfo.m_avgRlev[1] - factorInfo.m_avgRlev[0], m_Ydim);
            }
        }
    }

    private double[] interactionEffect(int[] f)
    {
        FactorInfo factorInfo = new FactorInfo();
        double[] array = new double[m_nruns];
        for (int i = 0; i < m_nruns; i++)
        {
            array[i] = 1.0;
            for (int j = 0; j < f.Length; j++)
            {
                FactorInfo factorInfo2 = (FactorInfo)m_factorInfo.GetValue(f[j]);
                array[i] *= factorInfo2.m_levels[factorInfo2.m_runlev[i]];
            }
        }
        factorInfo.m_name = "";
        for (int k = 0; k < f.Length; k++)
        {
            FactorInfo factorInfo3 = (FactorInfo)m_factorInfo.GetValue(f[k]);
            factorInfo.m_name += factorInfo3.m_name;
        }
        setupFactor(array, factorInfo, doYstats: true);
        return new double[3]
        {
            factorInfo.m_avgRlev[0],
            factorInfo.m_avgRlev[1],
            factorInfo.m_avgRlev[1] - factorInfo.m_avgRlev[0]
        };
    }

    private int interactionsEffects(string[] effects)
    {
        if (effects.Length == 0)
        {
            return 1;
        }
        int num = 1;
        foreach (string text in effects)
        {
            m_utils.getGenIndices(text, out var findx);
            double[] array = null;
            array = interactionEffect(findx);
            m_T.set_2(num, 0, new StrVal(text));
            m_T.set_2(num, 1, array[0], m_Ydim);
            m_T.set_2(num, 2, array[1], m_Ydim);
            m_T.set_2(num, 3, array[2], m_Ydim);
            num++;
        }
        return num;
    }

    private string FactorLabel(int f)
    {
        string text = ((FactorInfo)m_factorInfo.GetValue(f)).m_name;
        if (text.Contains("="))
        {
            text = text.Substring(0, text.IndexOf("="));
        }
        return text.Replace(" ", "");
    }

    private string FactorLabel(int[] f)
    {
        string text = null;
        for (int i = 0; i < f.Length; i++)
        {
            text += FactorLabel(f[i]);
            if (i < f.Length - 1)
            {
                text += ".";
            }
        }
        return text;
    }

    private string FactorLabel(string factor)
    {
        m_utils.getGenIndices(factor, out var findx);
        return FactorLabel(findx);
    }

    private MatVal general_block_effect()
    {
        BlocksInfo();
        MatVal matVal = new MatVal(m_nblocks + 1, 2);
        matVal.m_dim = Dim.identity;
        matVal.set_2(0, 0, new StrVal("Block"));
        matVal.set_2(0, 1, new StrVal("Effect"));
        for (int i = 0; i < m_nblocks; i++)
        {
            double re = m_blockmean[i] - m_gavg;
            matVal.set_2(i + 1, 0, i + 1);
            matVal.set_2(i + 1, 1, re, m_Ydim);
        }
        return matVal;
    }

    private MatVal general_main_effect(string factor)
    {
        int index = m_utils.charMap[factor];
        FactorInfo factorInfo = (FactorInfo)m_factorInfo.GetValue(index);
        MatVal matVal = new MatVal(factorInfo.m_nlevels + 1, 2);
        matVal.m_dim = Dim.identity;
        matVal.set_2(0, 0, new StrVal("Level of " + factorInfo.m_name));
        matVal.set_2(0, 1, new StrVal("Effect of " + factorInfo.m_name));
        for (int i = 0; i < factorInfo.m_nlevels; i++)
        {
            if (factorInfo.m_strlvls)
            {
                matVal.set_2(i + 1, 0, new StrVal(factorInfo.m_idx2str[i]));
            }
            else
            {
                double re = factorInfo.m_levels[i];
                matVal.set_2(i + 1, 0, re, factorInfo.m_unit);
            }
            double re2 = factorInfo.m_avgRlev[i] - m_gavg;
            matVal.set_2(i + 1, 1, re2, m_Ydim);
        }
        return matVal;
    }

    public MatVal get_general_effect(string factor)
    {
        factor = factor.ToUpper();
        int num = 0;
        if (factor.Contains("BLOCKS"))
        {
            num = 1;
            factor = factor.Replace("BLOCKS", "");
        }
        string[] array = factor.Split(new char[5] { ' ', ',', '\t', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (!m_utils.verifyUniqueGens(m_nfacts, array))
        {
            AppException.Raise("invalid or non unique effect", 3u);
        }
        if (array.Length == 0 && num == 1)
        {
            return general_block_effect();
        }
        if (array.Length == 1 && num == 0)
        {
            return general_interaction_effect(array[0]);
        }
        MatVal matVal = new MatVal(array.Length + num + 1, 2);
        matVal.set_2(0, 0, new StrVal("Factor"));
        matVal.set_2(0, 1, new StrVal("Effects"));
        MatVal matVal2 = null;
        if (num == 1)
        {
            matVal2 = general_block_effect();
            matVal.set_2(1, 0, new StrVal("Blocks"));
            matVal.set_2(1, 1, matVal2);
        }
        int num2 = num + 1;
        string[] array2 = array;
        foreach (string factor2 in array2)
        {
            matVal2 = general_interaction_effect(factor2);
            if (matVal2.Rows != 1)
            {
                matVal.set_2(num2, 0, new StrVal(FactorLabel(factor2)));
                matVal.set_2(num2, 1, matVal2);
                num2++;
            }
        }
        matVal.Data.m_rows = num2;
        return matVal;
    }


    private int interactionsEffects(int order)
    {
        if (order < 2)
        {
            return m_nfacts + 1;
        }
        int[] findx = new int[m_nfacts];
        for (int i = 0; i < m_nfacts; i++)
        {
            findx[i] = i;
        }
        string gen = "";
        m_utils.getGenName(findx, out gen);
        Combinations combinations = new Combinations(gen.GetEnumerator());
        Array allCombinations = combinations.getAllCombinations();
        int num = m_nfacts + 1;
        for (int j = 1; j < order; j++)
        {
            IEnumerator enumerator = ((Array)allCombinations.GetValue(j)).GetEnumerator();
            while (enumerator.MoveNext())
            {
                string[] gen2 = (string[])enumerator.Current;
                string name = m_utils.getName(gen2);
                m_utils.getGenIndices(name, out findx);
                double[] array = null;
                array = interactionEffect(findx);
                m_T.set_2(num, 0, new StrVal(name));
                m_T.set_2(num, 1, array[0], m_Ydim);
                m_T.set_2(num, 2, array[1], m_Ydim);
                m_T.set_2(num, 3, array[2], m_Ydim);
                num++;
            }
        }
        return num;
    }

    public MatVal get_general_effect(int order)
    {
        if (order < 1 || order > m_nfacts)
        {
            AppException.Raise("invalid interaction order", 3u);
        }
        int[] array = new int[m_nfacts];
        for (int i = 0; i < m_nfacts; i++)
        {
            array[i] = i;
        }
        string gen = "";
        m_utils.getGenName(array, out gen);
        Combinations combinations = new Combinations(gen.GetEnumerator());
        Array allCombinations = combinations.getAllCombinations();
        int num = 0;
        for (int j = 1; j <= order; j++)
        {
            num += Utils.combination(m_nfacts, j);
        }
        MatVal matVal = new MatVal(num + 1, 2);
        matVal.set_2(0, 0, new StrVal("Factor"));
        matVal.set_2(0, 1, new StrVal("Effects"));
        MatVal matVal2 = null;
        int num2 = 1;
        for (int k = 0; k < order; k++)
        {
            IEnumerator enumerator = ((Array)allCombinations.GetValue(k)).GetEnumerator();
            while (enumerator.MoveNext())
            {
                string[] gen2 = (string[])enumerator.Current;
                string name = m_utils.getName(gen2);
                matVal2 = general_interaction_effect(name);
                if (matVal2.Rows != 1)
                {
                    matVal.set_2(num2, 0, new StrVal(FactorLabel(name)));
                    matVal.set_2(num2, 1, matVal2);
                    num2++;
                }
            }
        }
        matVal.Data.m_rows = num2;
        return matVal;
    }

}
