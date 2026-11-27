#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class Blocking
{
    private string[] m_bgens;

    private double[,] m_dmat;

    public int noBlocks => m_nblocks;

    public Blocking(double[,] dmat, int nblocks)
    {
        SortedDictionary<int, SortedDictionary<int, SortedDictionary<int, Utils.Gens_level2>>> blockDefaults = m_utils.blockDefaults;
        int length = dmat.GetLength(1);
        int length2 = dmat.GetLength(0);
        if (blockDefaults.TryGetValue(length, out var value))
        {
            if (value.TryGetValue(length2, out var otherValue))
            {
                if (otherValue.TryGetValue(nblocks, out var nextValue))
                {
                    m_bgens = new string[nextValue.m_gens.Length];
                    nextValue.m_gens.CopyTo(m_bgens, 0);
                    m_nblocks = nblocks;
                    m_dmat = dmat;
                }
                else
                {
                    AppException.Raise("invalid number of blocks");
                }
            }
            else
            {
                AppException.Raise("Blocking not available");
            }
        }
        else
        {
            AppException.Raise("defaults not available");
        }
        init();
    }

    public Blocking(double[,] dmat, string bgens)
    {
        m_dmat = dmat;
        m_bgens = bgens.Split(new char[5] { ' ', ',', '\t', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
        int num = 0;
        string[] bgens2 = m_bgens;
        foreach (string text in bgens2)
        {
            m_bgens[num++] = text.ToUpper();
        }
        m_nblocks = 1 << m_bgens.Length;
        init();
        if (!m_utils.verifyGens(m_nfacts, m_bgens))
        {
            AppException.Raise("invalid block generators", 2u);
        }
    }

    private void init()
    {
        m_nfacts = m_dmat.GetLength(1);
        m_nruns = m_dmat.GetLength(0);
        m_blocksz = m_nruns / m_nblocks;
    }

    internal override double[,] getDesignMatrix()
    {
        return m_dmat;
    }

    public int[] getBlocking()
    {
        Permutations permutations = new Permutations(m_bgens.Length, keepOldOrder: true);
        double[,] allPermutations = permutations.getAllPermutations();
        int[] array = new int[m_dmat.GetLength(0)];
        for (int i = 0; i < m_dmat.GetLength(0); i++)
        {
            int[] indices = new int[m_bgens.Length];
            for (int j = 0; j < m_bgens.Length; j++)
            {
                m_utils.getGenIndices(m_bgens[j], out var findx);
                int num = 1;
                for (int k = 0; k < findx.Length; k++)
                {
                    num *= (int)m_dmat[i, findx[k]];
                }
                indices[j] = num;
            }
            for (int l = 0; l < allPermutations.GetLength(0); l++)
            {
                int value = 1;
                for (int m = 0; m < allPermutations.GetLength(1); m++)
                {
                    if (allPermutations[l, m] != (double)indices[m])
                    {
                        value = 0;
                        break;
                    }
                }
                if (value == 1)
                {
                    array[i] = l;
                }
            }
        }
        return array;
    }

    public void getBlockMatrix(out MatVal dmat)
    {
        int[] blocking = getBlocking();
        int[] array = new int[m_nblocks];
        dmat = new MatVal(m_nruns + 1, m_nfacts + 2);
        dmat.set_2(0, 0, "Run");
        dmat.set_2(0, 1, "Block");
        for (int i = 0; i < m_nfacts; i++)
        {
            dmat.set_2(0, i + 2, m_utils.icharMap[i]);
        }
        for (int j = 0; j < m_nblocks; j++)
        {
            array[j] = 0;
        }
        for (int k = 0; k < m_nruns; k++)
        {
            int num = m_blocksz * blocking[k] + array[blocking[k]]++;
            dmat.m_rarr[dmat.Anr + (num + 1) + dmat.m_offset] = blocking[k] + 1;
            for (int l = 0; l < m_nfacts; l++)
            {
                dmat.m_rarr[k + 1 + dmat.Offset] = k + 1;
                dmat.m_rarr[(l + 2) * dmat.Anr + (num + 1) + dmat.Offset] = m_dmat[k, l];
            }
        }
    }
}
