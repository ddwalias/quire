#nullable disable
#pragma warning disable

using System;
using System.Collections;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class Permutations
{
    private int[] m_currentPermut;

    private bool[] m_init;

    private int[] m_levels;

    private bool m_mixed;

    private bool m_2Level;

    private bool m_keepOldOrder;

    public int nruns => m_nruns;

    public object Current
    {
        get
        {
            if (!m_init[0])
            {
                throw new InvalidOperationException("Reset() must be called first");
            }
            return m_currentPermut;
        }
    }

    public Permutations(int numfactors, bool keepOldOrder)
    {
        m_nfacts = numfactors;
        m_currentPermut = new int[m_nfacts];
        m_init = new bool[2];
        m_nruns = 1 << m_nfacts;
        m_levels = new int[m_nfacts];
        for (int i = 0; i < m_nfacts; i++)
        {
            m_levels[i] = 2;
        }
        m_2Level = true;
        m_mixed = false;
        m_keepOldOrder = keepOldOrder;
        Reset();
    }

    public Permutations(int numfactors, int[] levels, bool keepOldOrder)
        : this(numfactors, keepOldOrder)
    {
        if (levels.Length < m_nfacts)
        {
            AppException.Raise("too few levels", 2u);
        }
        m_2Level = true;
        m_mixed = false;
        m_keepOldOrder = keepOldOrder;
        m_nruns = 1;
        for (int i = 0; i < m_nfacts; i++)
        {
            if (levels[i] <= 0)
            {
                AppException.Raise("levels must be positive", 2u);
            }
            m_nruns *= levels[i];
            if (levels[i] != 2)
            {
                m_2Level = false;
            }
            if (levels[i] != levels[0])
            {
                m_mixed = true;
            }
        }
        m_levels = levels;
    }

    private bool nextPermutation_2()
    {
        int num = m_nfacts;
        for (int index = m_nfacts - 1; index >= 0; index--)
        {
            if (m_currentPermut[index] == 0)
            {
                num = index;
                break;
            }
        }
        if (num == m_nfacts)
        {
            return false;
        }
        m_currentPermut[num] = 1;
        for (int i = num + 1; i < m_nfacts; i++)
        {
            m_currentPermut[i] = 0;
        }
        return true;
    }

    private bool nextPermutation()
    {
        int num = m_nfacts;
        for (int index = m_nfacts - 1; index >= 0; index--)
        {
            if (m_currentPermut[index] < m_levels[index] - 1)
            {
                num = index;
                break;
            }
        }
        if (num == m_nfacts)
        {
            return false;
        }
        m_currentPermut[num]++;
        for (int i = num + 1; i < m_nfacts; i++)
        {
            m_currentPermut[i] = 0;
        }
        return true;
    }

    public void Reset()
    {
        m_init[0] = false;
        m_init[1] = true;
        for (int i = 0; i < m_nfacts; i++)
        {
            m_currentPermut[i] = 0;
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
        m_init[0] = nextPermutation();
        return m_init[0];
    }

    public double[,] getAllPermutations()
    {
        if (m_mixed && !m_keepOldOrder)
        {
            return mixed_factorial_std();
        }
        double[,] array = new double[m_nruns, m_nfacts];
        Reset();
        int num = 0;
        while (MoveNext())
        {
            int[] indices = (int[])Current;
            for (int i = 0; i < m_nfacts; i++)
            {
                array[num, i] = indices[i];
                if (array[num, i] == 0.0 && m_2Level)
                {
                    array[num, i] = -1.0;
                }
            }
            num++;
        }
        if (m_keepOldOrder)
        {
            return array;
        }
        double[,] values = new double[m_nruns, m_nfacts];
        for (int j = 0; j < m_nfacts; j++)
        {
            for (num = 0; num < m_nruns; num++)
            {
                values[num, j] = array[num, m_nfacts - 1 - j];
            }
        }
        return values;
    }

    internal override double[,] getDesignMatrix()
    {
        return getAllPermutations();
    }

    private double[,] mixed_factorial_std()
    {
        double[,] array = new double[m_nruns, m_nfacts];
        for (int i = 0; i < m_nfacts; i++)
        {
            int num = 1;
            for (int j = 0; j < i; j++)
            {
                num *= m_levels[j];
            }
            int counter = 0;
            while (counter < m_nruns)
            {
                for (int k = 0; k < m_levels[i]; k++)
                {
                    for (int l = 0; l < num; l++)
                    {
                        array[counter++, i] = k;
                    }
                }
            }
        }
        return array;
    }
}
