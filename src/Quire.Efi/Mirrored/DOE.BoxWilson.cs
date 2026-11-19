#nullable disable
#pragma warning disable

using System;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class BoxWilson
{
    private int m_nc;

    private int m_nf;

    private int m_fract;

    private double m_alpha;

    private bool m_keepOldOrder;

    public BoxWilson(int nfacts, bool keepOldOrder)
    {
        switch (nfacts)
        {
            case 2:
            case 3:
            case 4:
                m_fract = 0;
                break;
            case 5:
            case 6:
            case 7:
                m_fract = 1;
                break;
            case 8:
            case 9:
                m_fract = 2;
                break;
            default:
                AppException.Raise("factors are limited to 9", 1u);
                break;
        }
        m_nfacts = nfacts;
        m_nf = 1 << nfacts - m_fract;
        m_nc = (int)Math.Ceiling(4.0 * Math.Sqrt(m_nf + 1) - (double)(2 * m_nfacts));
        m_nruns = m_nf + m_nc + 2 * m_nfacts;
        m_alpha = Math.Pow(m_nf, 0.25);
        m_keepOldOrder = keepOldOrder;
    }

    public BoxWilson(int nfacts, int nc, double alpha, bool keepOldOrder)
        : this(nfacts, keepOldOrder)
    {
        m_nc = nc;
        m_alpha = alpha;
        m_nruns = m_nf + m_nc + 2 * m_nfacts;
        m_keepOldOrder = keepOldOrder;
    }

    public BoxWilson(int nfacts, int fract, int nc, double alpha)
    {
        if (nfacts < 2 || nfacts > 9)
        {
            AppException.Raise("factors are limited to 9", 1u);
        }
        switch (fract)
        {
            case 1:
                if (nfacts < 5)
                {
                    AppException.Raise("impractical design", 2u);
                }
                break;
            case 2:
                if (nfacts < 8)
                {
                    AppException.Raise("impractical design", 2u);
                }
                break;
            default:
                AppException.Raise("invalid fraction power. allowed values 0, 1 or 2");
                break;
            case 0:
                break;
        }
        m_nf = 1 << nfacts - fract;
        m_nfacts = nfacts;
        m_fract = fract;
        m_nc = nc;
        m_alpha = alpha;
        m_nruns = m_nf + m_nc + 2 * m_nfacts;
    }

    private void setNc()
    {
        m_nc = (int)Math.Ceiling(4.0 * (Math.Sqrt(m_nf) + 1.0) - (double)(2 * m_nfacts));
        m_nc = (int)Math.Ceiling(4.0 * Math.Sqrt(m_nf + 1) - (double)(2 * m_nfacts));
    }

    private void setAlpha()
    {
        m_alpha = Math.Sqrt((Math.Sqrt(m_nf * (m_nf + 2 * m_nfacts + m_nc)) - (double)m_nf) * 0.5);
        m_alpha = Math.Pow(m_nf, 0.25);
    }


    internal override double[,] getDesignMatrix()
    {
        double[,] array = new double[m_nruns, m_nfacts];
        if (m_fract == 0)
        {
            Permutations permutations = new Permutations(m_nfacts, m_keepOldOrder);
            double[,] allPermutations = permutations.getAllPermutations();
            for (int i = 0; i < m_nf; i++)
            {
                for (int j = 0; j < m_nfacts; j++)
                {
                    array[i, j] = allPermutations[i, j];
                }
            }
        }
        else
        {
            FractFact fractFact = new FractFact(m_nfacts, m_fract, m_keepOldOrder);
            double[,] designMatrix = fractFact.getDesignMatrix();
            for (int k = 0; k < m_nf; k++)
            {
                for (int l = 0; l < m_nfacts; l++)
                {
                    array[k, l] = designMatrix[k, l];
                }
            }
        }
        for (int m = 0; m < m_nc + 2 * m_nfacts; m++)
        {
            for (int n = 0; n < m_nfacts; n++)
            {
                array[m + m_nf, n] = 0.0;
            }
        }
        for (int num = 0; num < 2 * m_nfacts; num += 2)
        {
            array[num + m_nf + m_nc, num / 2] = m_alpha;
            array[num + m_nf + m_nc + 1, num / 2] = -m_alpha;
        }
        return array;
    }

}
