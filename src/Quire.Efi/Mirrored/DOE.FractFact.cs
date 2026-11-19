#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class FractFact
{
    private string[] m_gens;

    private int[] m_gens_sgn;

    private int m_q;

    private SortedDictionary<int, SortedDictionary<int, Utils.Gens_level2>> m_fractDefaults;

    private SortedDictionary<int, List<string>> m_sdefRel;

    private List<string> m_defRel;

    private bool m_keepOldOrder;

    public string Generators
    {
        get
        {
            string text = "";
            int num = m_nfacts - m_q;
            for (int i = 0; i < num; i++)
            {
                text += m_utils.icharMap[i];
                text += ", ";
            }
            int num2 = 0;
            string[] gens = m_gens;
            foreach (string text2 in gens)
            {
                text += m_utils.icharMap[num++];
                text += "=";
                if (m_gens_sgn[num2++] == -1)
                {
                    text += "-";
                }
                text += text2;
                text += ", ";
            }
            text = text.Remove(text.Length - 2);
            return text;
        }
    }

    public int nfactors => m_nfacts;

    public int nruns => m_nruns;

    public FractFact(int nfacts, int q, bool keepOldOrder)
    {
        if (nfacts > 26)
        {
            AppException.Raise("maximum allowed number of factors is 26", 1u);
        }
        if (q >= nfacts)
        {
            AppException.Raise("invalid design", 2u);
        }
        if (q == 0)
        {
            AppException.Raise("use fullfact for complete factorial", 2u);
        }
        m_nfacts = nfacts;
        m_nruns = 1 << nfacts - q;
        if (m_nruns < nfacts)
        {
            AppException.Raise("invalid design", 2u);
        }
        m_q = q;
        m_gens = new string[q];
        m_gens_sgn = new int[q];
        for (int i = 0; i < q; i++)
        {
            m_gens_sgn[i] = 1;
        }
        init();
        if (!lookupDefaultGens())
        {
            if (m_nfacts - m_q - 1 == 0)
            {
                AppException.Raise("invalid design", 2u);
            }
            defineDefaultGens();
        }
        m_keepOldOrder = keepOldOrder;
    }

    private void init()
    {
        m_defRel = new List<string>();
        m_sdefRel = new SortedDictionary<int, List<string>>();
        m_fractDefaults = m_utils.fractDefaults;
    }

    private bool lookupDefaultGens()
    {
        if (m_fractDefaults.TryGetValue(m_nfacts, out var value))
        {
            if (value.TryGetValue(m_nruns, out var value2))
            {
                value2.m_gens.CopyTo(m_gens, 0);
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    private void defineDefaultGens()
    {
        string[] array = new string[m_nfacts - m_q];
        for (int i = 0; i < m_nfacts - m_q; i++)
        {
            m_gens[0] += m_utils.icharMap[i];
            array[i] = m_utils.icharMap[i];
        }
        Combinations combinations = new Combinations(array.GetEnumerator());
        int num = 1;
        while (num < m_q)
        {
            combinations.k = m_nfacts - m_q - num;
            combinations.Reset();
            while (combinations.MoveNext() && num < m_q)
            {
                string[] array2 = (string[])combinations.Current;
                m_gens[num] = string.Concat(array2);
                num++;
            }
        }
    }

    public SortedDictionary<int, List<string>> getSortedDefRelation()
    {
        if (m_sdefRel.Count > 0)
        {
            return m_sdefRel;
        }
        if (m_defRel.Count == 0)
        {
            m_defRel = getDefiningRelation();
        }
        foreach (string item in m_defRel)
        {
            if (m_sdefRel.ContainsKey(item.Length))
            {
                m_sdefRel[item.Length].Add(item);
                continue;
            }
            List<string> list = new List<string>();
            list.Add(item);
            m_sdefRel[item.Length] = list;
        }
        for (int i = 2; i < m_sdefRel.Count; i++)
        {
            if (m_sdefRel.ContainsKey(i))
            {
                List<string> list2 = m_sdefRel[i];
                Utils.StringComparer comparer = new Utils.StringComparer();
                list2.Sort(comparer);
            }
            else
            {
                List<string> value = new List<string>();
                m_sdefRel[i] = value;
            }
        }
        return m_sdefRel;
    }

    private int getDefWordSgn(string[] combs)
    {
        int num = 1;
        for (int i = 0; i < combs.Length; i++)
        {
            if (combs[i].Substring(0, 1).Equals("-"))
            {
                num *= -1;
                combs[i] = combs[i].Remove(0, 1);
            }
        }
        return num;
    }

    private string getDefWord(string[] combs)
    {
        int defWordSgn = getDefWordSgn(combs);
        string text = "";
        int[] array = new int[26];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = 0;
        }
        foreach (string text2 in combs)
        {
            m_utils.getGenIndices(text2, out var findx);
            for (int k = 0; k < text2.Length; k++)
            {
                array[findx[k]]++;
            }
        }
        for (int l = 0; l < array.Length; l++)
        {
            if (array[l] % 2 != 0)
            {
                text += m_utils.icharMap[l];
            }
        }
        if (defWordSgn == -1)
        {
            text = text.Insert(0, "-");
        }
        return text;
    }

    public int getResolution()
    {
        if (m_sdefRel.Count == 0)
        {
            getSortedDefRelation();
        }
        foreach (KeyValuePair<int, List<string>> item in m_sdefRel)
        {
            if (item.Value.Count > 0)
            {
                return item.Key;
            }
        }
        return 0;
    }

    public string getAliasing(string effect, int order)
    {
        string[] gens = new string[1] { effect };
        if (!m_utils.verifyGens(m_nfacts, gens))
        {
            AppException.Raise("invalid effect", 2u);
        }
        if (m_defRel.Count == 0)
        {
            m_defRel = getDefiningRelation();
        }
        List<string> list = new List<string>();
        string[] array = new string[2];
        array[0] = effect;
        for (int i = 0; i < m_defRel.Count; i++)
        {
            array[1] = m_defRel[i];
            string defWord = getDefWord(array);
            if (defWord == "")
            {
                continue;
            }
            if (order > 0)
            {
                if (defWord.Length != order || list.Contains(defWord))
                {
                    continue;
                }
                list.Add(defWord);
            }
            if (!list.Contains(defWord))
            {
                list.Add(defWord);
            }
        }
        Utils.StringComparer comparer = new Utils.StringComparer();
        list.Sort(comparer);
        string text = "";
        foreach (string item in list)
        {
            text += item;
            text += ", ";
        }
        if (text.Length > 0)
        {
            text = text.Remove(text.Length - 2);
        }
        return text;
    }

    internal override double[,] getDesignMatrix()
    {
        double[,] array = new double[m_nruns, m_nfacts];
        Permutations permutations = new Permutations(m_nfacts - m_gens.Length, m_keepOldOrder);
        double[,] allPermutations = permutations.getAllPermutations();
        for (int i = 0; i < allPermutations.GetLength(0); i++)
        {
            for (int j = 0; j < allPermutations.GetLength(1); j++)
            {
                array[i, j] = allPermutations[i, j];
            }
        }
        int num = m_nfacts - m_gens.Length;
        int num2 = 0;
        string[] gens = m_gens;
        foreach (string gen in gens)
        {
            m_utils.getGenIndices(gen, out var findx);
            for (int l = 0; l < m_nruns; l++)
            {
                int num3 = 1;
                for (int m = 0; m < findx.Length; m++)
                {
                    num3 *= (int)array[l, findx[m]];
                }
                array[l, num] = m_gens_sgn[num2] * num3;
            }
            num2++;
            num++;
        }
        return array;
    }

    public List<string> getDefiningRelation()
    {
        int num = 1 + m_gens.Length;
        for (int i = 2; i < m_gens.Length; i++)
        {
            num += Utils.combination(m_gens.Length, i);
        }
        m_defRel.Capacity = num;
        for (int j = 0; j < m_gens.Length; j++)
        {
            int key = m_nfacts - m_gens.Length + j;
            string item = ((m_gens_sgn[j] == -1) ? "-" : "") + m_gens[j] + m_utils.icharMap[key];
            m_defRel.Add(item);
        }
        string[] array = new string[m_gens.Length];
        m_defRel.CopyTo(0, array, 0, m_gens.Length);
        Combinations combinations = new Combinations(array.GetEnumerator());
        Array allCombinations = combinations.getAllCombinations();
        for (int k = 1; k < allCombinations.Length; k++)
        {
            IEnumerator enumerator = ((Array)allCombinations.GetValue(k)).GetEnumerator();
            while (enumerator.MoveNext())
            {
                string[] combs = (string[])enumerator.Current;
                m_defRel.Add(getDefWord(combs));
            }
        }
        return m_defRel;
    }

}
