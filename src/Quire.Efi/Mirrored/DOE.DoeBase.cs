#nullable disable
#pragma warning disable

using System;
using Quire.Mcd;

namespace Quire.DOE;

public abstract partial class DoeBase
{
    protected int m_nfacts;
    protected int m_nruns;
    protected int m_nblocks;
    protected int m_blocksz;
    protected Utils m_utils;

    protected DoeBase()
    {
        m_utils = new Utils();
    }

    internal abstract double[,] getDesignMatrix();

    public virtual void getDesignMatrix(out MatVal dmat)
    {
        double[,] designMatrix = getDesignMatrix();
        dmat = new MatVal(m_nruns + 1, m_nfacts + 2);
        dmat.m_dim = Dim.identity;
        dmat.set_2(0, 0, new StrVal("Run"));
        dmat.set_2(0, 1, new StrVal("Block"));
        if (this is FractFact)
        {
            string[] array = (this as FractFact).Generators.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < m_nfacts; i++)
            {
                dmat.set_2(0, i + 2, new StrVal(array[i].Replace(" ", "")));
            }
        }
        else
        {
            for (int factor = 0; factor < m_nfacts; factor++)
            {
                dmat.set_2(0, factor + 2, new StrVal(m_utils.icharMap[factor]));
            }
        }
        for (int run = 0; run < m_nruns; run++)
        {
            dmat.m_rarr[run + 1 + dmat.Offset] = run + 1;
            if (this is BoxBehnken || this is Taguchi)
            {
                dmat.m_rarr[dmat.Anr + (run + 1) + dmat.Offset] = designMatrix[run, 0];
            }
            else
            {
                dmat.m_rarr[dmat.Anr + (run + 1) + dmat.Offset] = 1.0;
            }
        }
        for (int factor = 0; factor < m_nfacts; factor++)
        {
            for (int row = 0; row < m_nruns; row++)
            {
                if (this is BoxBehnken || this is Taguchi)
                {
                    dmat.m_rarr[(factor + 2) * dmat.Anr + (row + 1) + dmat.Offset] = designMatrix[row, factor + 1];
                }
                else
                {
                    dmat.m_rarr[(factor + 2) * dmat.Anr + (row + 1) + dmat.Offset] = designMatrix[row, factor];
                }
            }
        }
    }
}
