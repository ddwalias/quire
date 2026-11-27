#nullable disable
#pragma warning disable

using System.Collections.Generic;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class PlackettBurman
{
    private SortedDictionary<int, int[]> m_genvecs;

    private double[,] m_pb28;

    public PlackettBurman(int nruns)
    {
        if (nruns % 4 != 0)
        {
            AppException.Raise("number of runs must be a multiple of 4", 1u);
        }
        if (nruns < 8 || nruns > 48)
        {
            AppException.Raise("number of runs must be greater than 7 and less than 49", 1u);
        }
        m_nfacts = nruns - 1;
        m_nruns = nruns;
        init();
    }

    internal override double[,] getDesignMatrix()
    {
        if (m_nruns == 28)
        {
            special_28();
            return m_pb28;
        }
        double[,] array = new double[m_nruns, m_nfacts];
        int[] indices = m_genvecs[m_nruns];
        for (int i = 0; i < m_nruns - 1; i++)
        {
            array[i, 0] = indices[i];
        }
        array[m_nruns - 1, 0] = -1.0;
        for (int j = 1; j < m_nfacts; j++)
        {
            array[0, j] = array[m_nruns - 2, j - 1];
            for (int k = 1; k < m_nruns - 1; k++)
            {
                array[k, j] = array[k - 1, j - 1];
            }
            array[m_nruns - 1, j] = -1.0;
        }
        return array;
    }

    private void init()
    {
        m_genvecs = new SortedDictionary<int, int[]>();
        int[] genvec = new int[7] { 1, 1, 1, -1, 1, -1, -1 };
        m_genvecs.Add(8, genvec);
        genvec = new int[11]
        {
            1, 1, -1, 1, 1, 1, -1, -1, -1, 1,
            -1
        };
        m_genvecs.Add(12, genvec);
        genvec = new int[15]
        {
            1, 1, 1, 1, -1, 1, -1, 1, 1, -1,
            -1, 1, -1, -1, -1
        };
        m_genvecs.Add(16, genvec);
        genvec = new int[19]
        {
            1, 1, -1, -1, 1, 1, 1, 1, -1, 1,
            -1, 1, -1, -1, -1, -1, 1, 1, -1
        };
        m_genvecs.Add(20, genvec);
        genvec = new int[23]
        {
            1, 1, 1, 1, 1, -1, 1, -1, 1, 1,
            -1, -1, 1, 1, -1, -1, 1, -1, 1, -1,
            -1, -1, -1
        };
        m_genvecs.Add(24, genvec);
        genvec = new int[27]
        {
            1, 1, -1, -1, -1, -1, 1, 1, 1, 1,
            -1, 1, 1, 1, -1, 1, 1, -1, -1, -1,
            1, -1, 1, -1, -1, 1, -1
        };
        m_genvecs.Add(28, genvec);
        genvec = new int[31]
        {
            -1, -1, -1, -1, 1, -1, 1, -1, 1, 1,
            1, -1, 1, 1, -1, -1, -1, 1, 1, 1,
            1, 1, -1, -1, 1, 1, -1, 1, -1, -1,
            1
        };
        m_genvecs.Add(32, genvec);
        genvec = new int[35]
        {
            -1, 1, -1, 1, 1, 1, -1, -1, -1, 1,
            1, 1, 1, 1, -1, 1, 1, 1, -1, -1,
            1, -1, -1, -1, -1, 1, -1, 1, -1, 1,
            1, -1, -1, 1, -1
        };
        m_genvecs.Add(36, genvec);
        genvec = new int[39]
        {
            1, 1, -1, -1, 1, 1, 1, 1, -1, 1,
            -1, 1, -1, -1, -1, -1, 1, 1, -1, -1,
            1, 1, -1, -1, 1, 1, 1, 1, -1, 1,
            -1, 1, -1, -1, -1, -1, 1, 1, -1
        };
        m_genvecs.Add(40, genvec);
        genvec = new int[43]
        {
            1, 1, -1, -1, 1, -1, 1, -1, -1, 1,
            1, 1, -1, 1, 1, 1, 1, 1, -1, -1,
            -1, 1, -1, 1, 1, 1, -1, -1, -1, -1,
            -1, 1, -1, -1, -1, 1, 1, -1, 1, -1,
            1, 1, -1
        };
        m_genvecs.Add(44, genvec);
        genvec = new int[47]
        {
            1, 1, 1, 1, 1, -1, 1, 1, 1, 1,
            -1, -1, 1, -1, 1, -1, 1, 1, 1, -1,
            -1, 1, -1, -1, 1, 1, -1, 1, 1, -1,
            -1, -1, 1, -1, 1, -1, 1, 1, -1, -1,
            -1, -1, 1, -1, -1, -1, -1
        };
        m_genvecs.Add(48, genvec);
    }

    private void special_28()
    {
        double[,] blockA = new double[9, 9]
        {
            { 1.0, -1.0, 1.0, 1.0, 1.0, 1.0, -1.0, -1.0, -1.0 },
            { 1.0, 1.0, -1.0, 1.0, 1.0, 1.0, -1.0, -1.0, -1.0 },
            { -1.0, 1.0, 1.0, 1.0, 1.0, 1.0, -1.0, -1.0, -1.0 },
            { -1.0, -1.0, -1.0, 1.0, -1.0, 1.0, 1.0, 1.0, 1.0 },
            { -1.0, -1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, 1.0 },
            { -1.0, -1.0, -1.0, -1.0, 1.0, 1.0, 1.0, 1.0, 1.0 },
            { 1.0, 1.0, 1.0, -1.0, -1.0, -1.0, 1.0, -1.0, 1.0 },
            { 1.0, 1.0, 1.0, -1.0, -1.0, -1.0, 1.0, 1.0, -1.0 },
            { 1.0, 1.0, 1.0, -1.0, -1.0, -1.0, -1.0, 1.0, 1.0 }
        };
        double[,] blockB = new double[9, 9]
        {
            { -1.0, 1.0, -1.0, -1.0, -1.0, 1.0, -1.0, -1.0, 1.0 },
            { -1.0, -1.0, 1.0, 1.0, -1.0, -1.0, 1.0, -1.0, -1.0 },
            { 1.0, -1.0, -1.0, -1.0, 1.0, -1.0, -1.0, 1.0, -1.0 },
            { -1.0, -1.0, 1.0, -1.0, 1.0, -1.0, -1.0, -1.0, 1.0 },
            { 1.0, -1.0, -1.0, -1.0, -1.0, 1.0, 1.0, -1.0, -1.0 },
            { -1.0, 1.0, -1.0, 1.0, -1.0, -1.0, -1.0, 1.0, -1.0 },
            { -1.0, -1.0, 1.0, -1.0, -1.0, 1.0, -1.0, 1.0, -1.0 },
            { 1.0, -1.0, -1.0, 1.0, -1.0, -1.0, -1.0, -1.0, 1.0 },
            { -1.0, 1.0, -1.0, -1.0, 1.0, -1.0, 1.0, -1.0, -1.0 }
        };
        double[,] blockC = new double[9, 9]
        {
            { 1.0, 1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0 },
            { -1.0, 1.0, 1.0, 1.0, 1.0, -1.0, 1.0, 1.0, -1.0 },
            { 1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0 },
            { 1.0, -1.0, 1.0, 1.0, 1.0, -1.0, 1.0, -1.0, 1.0 },
            { 1.0, 1.0, -1.0, -1.0, 1.0, 1.0, 1.0, 1.0, -1.0 },
            { -1.0, 1.0, 1.0, 1.0, -1.0, 1.0, -1.0, 1.0, 1.0 },
            { 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, 1.0, -1.0 },
            { 1.0, 1.0, -1.0, 1.0, 1.0, -1.0, -1.0, 1.0, 1.0 },
            { -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, 1.0, -1.0, 1.0 }
        };
        m_pb28 = new double[28, 27];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                m_pb28[i, j] = blockA[i, j];
                m_pb28[i, j + 9] = blockB[i, j];
                m_pb28[i, j + 18] = blockC[i, j];
                m_pb28[i + 9, j] = blockC[i, j];
                m_pb28[i + 9, j + 9] = blockA[i, j];
                m_pb28[i + 9, j + 18] = blockB[i, j];
                m_pb28[i + 18, j] = blockB[i, j];
                m_pb28[i + 18, j + 9] = blockC[i, j];
                m_pb28[i + 18, j + 18] = blockA[i, j];
            }
        }
        for (int k = 0; k < 27; k++)
        {
            m_pb28[27, k] = -1.0;
        }
    }
}
