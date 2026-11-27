#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;

namespace Quire.DOE;

public partial class Utils
{
    public partial class Gens_level2
    {
        public string[] m_gens;

        public int m_resl;

        public Gens_level2(string[] gens, int resl)
        {
            m_gens = gens;
            m_resl = resl;
        }
    }

    public partial class StringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            if (x.Equals(string.Empty) && y.Equals(string.Empty))
            {
                return 0;
            }
            if (x.Equals(string.Empty))
            {
                return -1;
            }
            if (y.Equals(string.Empty))
            {
                return 1;
            }
            if (x.Length > y.Length)
            {
                return 1;
            }
            if (x.Length < y.Length)
            {
                return -1;
            }
            return string.Compare(x, y);
        }
    }

    private SortedDictionary<string, int> m_alphbetMap;

    private SortedDictionary<int, string> m_ialphbetMap;

    private SortedDictionary<int, SortedDictionary<int, Gens_level2>> m_fractDefaults;

    private SortedDictionary<int, SortedDictionary<int, SortedDictionary<int, Gens_level2>>> m_blockDefaults;

    public SortedDictionary<string, int> charMap => m_alphbetMap;

    public SortedDictionary<int, string> icharMap => m_ialphbetMap;

    public SortedDictionary<int, SortedDictionary<int, Gens_level2>> fractDefaults => m_fractDefaults;

    public SortedDictionary<int, SortedDictionary<int, SortedDictionary<int, Gens_level2>>> blockDefaults => m_blockDefaults;

    private static int recProduct(int n, int len)
    {
        if (1 < len)
        {
            int num = len >> 1;
            return recProduct(n, num) * recProduct(n + num, len - num);
        }
        return n;
    }

    public static int factorial(int n)
    {
        if (n == 0)
        {
            return 1;
        }
        return recProduct(1, n);
    }

    public static int combination(int n, int k)
    {
        if (k <= 0 || k > n)
        {
            throw new ArgumentOutOfRangeException("k");
        }
        return factorial(n) / (factorial(n - k) * factorial(k));
    }

    public bool getGenIndices(string gen, out int[] findx)
    {
        IEnumerator enumerator = gen.GetEnumerator();
        int num = 0;
        findx = new int[gen.Length];
        enumerator.Reset();
        while (enumerator.MoveNext())
        {
            string key = ((char)enumerator.Current).ToString();
            if (!m_alphbetMap.TryGetValue(key, out findx[num++]))
            {
                return false;
            }
        }
        return true;
    }

    public bool getGenIndices(string[] gen, out int[] findx)
    {
        IEnumerator enumerator = gen.GetEnumerator();
        int num = 0;
        findx = new int[gen.Length];
        enumerator.Reset();
        while (enumerator.MoveNext())
        {
            string key = (string)enumerator.Current;
            if (!m_alphbetMap.TryGetValue(key, out findx[num++]))
            {
                return false;
            }
        }
        return true;
    }

    public bool getGenName(int[] findx, out string gen)
    {
        gen = "";
        for (int i = 0; i < findx.Length; i++)
        {
            if (!m_ialphbetMap.TryGetValue(findx[i], out var value))
            {
                return false;
            }
            gen += value;
        }
        return true;
    }

    public string getName(string[] gen)
    {
        IEnumerator enumerator = gen.GetEnumerator();
        string text = "";
        enumerator.Reset();
        while (enumerator.MoveNext())
        {
            text += (string)enumerator.Current;
        }
        return text;
    }

    public bool verifyGens(int nfacts, string[] gens)
    {
        for (int i = 0; i < gens.Length; i++)
        {
            if (!getGenIndices(gens[i], out var findx))
            {
                return false;
            }
            for (int j = 0; j < findx.Length; j++)
            {
                if (findx[j] >= nfacts)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool verifyUniqueGens(int nfacts, string[] gens)
    {
        Array array = Array.CreateInstance(typeof(int[]), gens.Length);
        for (int i = 0; i < gens.Length; i++)
        {
            if (!getGenIndices(gens[i], out var findx))
            {
                return false;
            }
            for (int j = 0; j < findx.Length; j++)
            {
                if (findx[j] >= nfacts)
                {
                    return false;
                }
            }
            Array.Sort(findx);
            array.SetValue(findx, i);
        }
        for (int k = 0; k < gens.Length - 1; k++)
        {
            int[] indices = array.GetValue(k) as int[];
            for (int l = k + 1; l < gens.Length; l++)
            {
                int[] otherValues = array.GetValue(l) as int[];
                if (indices.Length != otherValues.Length)
                {
                    continue;
                }
                bool flag = true;
                for (int m = 0; m < indices.Length; m++)
                {
                    if (indices[m] != otherValues[m])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
