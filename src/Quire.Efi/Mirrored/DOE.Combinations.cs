#nullable disable
#pragma warning disable

using System;
using System.Collections;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class Combinations
{
    private string[] m_factors;

    private string[] m_currentComb;

    private int[] m_curCmbIndices;

    private int m_k;

    private int m_n;

    private bool[] m_init;

    public int k
    {
        get
        {
            return m_k;
        }
        set
        {
            if (value <= 0 || value > m_n)
            {
                AppException.Raise("invalid design", 1u);
            }
            m_k = value;
            initK();
        }
    }

    public object Current
    {
        get
        {
            if (!m_init[0])
            {
                throw new InvalidOperationException("Reset() must be called first");
            }
            for (int i = 0; i < m_k; i++)
            {
                m_currentComb[i] = m_factors[m_curCmbIndices[i]];
            }
            return m_currentComb;
        }
    }

    public Combinations(string factors)
    {
        string[] array = factors.Split(new char[5] { ' ', ',', '\t', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
        initN(array.GetEnumerator());
        m_k = 0;
    }

    public Combinations(string factors, int k)
    {
        string[] array = factors.Split(new char[5] { ' ', ',', '\t', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
        initN(array.GetEnumerator());
        m_k = k;
        initK();
    }

    public Combinations(IEnumerator factors)
    {
        initN(factors);
        m_k = 0;
    }

    public Combinations(IEnumerator factors, int k)
    {
        initN(factors);
        m_k = k;
        initK();
    }

    private void initK()
    {
        m_curCmbIndices = new int[m_k];
        m_currentComb = new string[m_k];
        Reset();
    }

    private void initN(IEnumerator factors)
    {
        factors.Reset();
        int num = 0;
        while (factors.MoveNext())
        {
            num++;
        }
        m_n = num;
        m_factors = new string[num];
        m_init = new bool[2];
        num = 0;
        factors.Reset();
        while (factors.MoveNext())
        {
            if (factors.Current is char)
            {
                m_factors[num++] = ((char)factors.Current).ToString();
            }
            else
            {
                m_factors[num++] = (string)factors.Current;
            }
        }
    }

    private bool nextCombination()
    {
        int num = m_k;
        for (int index = m_k - 1; index >= 0; index--)
        {
            if (m_curCmbIndices[index] < m_n - m_k + index)
            {
                num = index;
                break;
            }
        }
        if (num == m_k)
        {
            return false;
        }
        m_curCmbIndices[num]++;
        for (int i = num + 1; i < m_k; i++)
        {
            m_curCmbIndices[i] = m_curCmbIndices[i - 1] + 1;
        }
        return true;
    }

    public void Reset()
    {
        m_init[0] = false;
        m_init[1] = true;
        for (int i = 0; i < m_k; i++)
        {
            m_curCmbIndices[i] = i;
        }
    }

    public bool MoveNext()
    {
        if (m_init[1])
        {
            m_init[1] = false;
            m_init[0] = true;
            return true;
        }
        m_init[0] = nextCombination();
        return m_init[0];
    }

    public Array getAllCombinations()
    {
        Array array = Array.CreateInstance(typeof(Array), m_n);
        for (m_k = 1; m_k <= m_n; m_k++)
        {
            initK();
            int length = Utils.combination(m_n, m_k);
            Array items = Array.CreateInstance(typeof(string[]), length);
            int num = 0;
            while (MoveNext())
            {
                string[] names = (string[])Current;
                string[] otherValues = new string[m_k];
                names.CopyTo(otherValues, 0);
                items.SetValue(otherValues, num++);
            }
            array.SetValue(items, m_k - 1);
        }
        return array;
    }
}
