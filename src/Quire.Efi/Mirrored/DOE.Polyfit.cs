#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Quire.Efi;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.DOE;

public partial class Polyfit
{
    public partial class TermInfo
    {
        public string label;

        public int hi_pwr;

        public TermInfo(string t, int n)
        {
            label = t;
            hi_pwr = n;
        }
    }

    public partial class StrngCompare : IComparer<TermInfo>
    {
        public int Compare(TermInfo x_, TermInfo y_)
        {
            string label = x_.label;
            string label2 = y_.label;
            if (label == null && label2 == null)
            {
                return 0;
            }
            if (label == null)
            {
                return -1;
            }
            if (label2 == null)
            {
                return 1;
            }
            if (label.Equals(string.Empty) && label2.Equals(string.Empty))
            {
                return 0;
            }
            if (label.Equals(string.Empty))
            {
                return -1;
            }
            if (label2.Equals(string.Empty))
            {
                return -1;
            }
            if (label.Equals("Intercept") && label2.Equals("Intercept"))
            {
                return 0;
            }
            if (label.Equals("Intercept"))
            {
                return -1;
            }
            if (label2.Equals("Intercept"))
            {
                return 1;
            }
            if (label.Length > label2.Length)
            {
                return 1;
            }
            if (label.Length < label2.Length)
            {
                return -1;
            }
            if (x_.hi_pwr < y_.hi_pwr)
            {
                return -1;
            }
            if (x_.hi_pwr > y_.hi_pwr)
            {
                return 1;
            }
            return string.Compare(label, label2);
        }
    }

    private int m_nterms;

    private int m_hpwr;

    private string[] m_TermLabel;

    private MatVal m_polymat;

    private MatVal m_RHS;

    private MatVal m_LHS;

    private MatVal m_LHS_ns;

    private Dim[] m_Adim;

    private bool m_IsScaleOn;

    private bool m_ConstTerm;

    private Array m_scale;

    private double m_tol;

    private ThreadLocal<double[]> _threadLocalWork;

    private double[] m_work => _threadLocalWork.Value;

    public MatVal polyMat
    {
        get
        {
            unscale_polymat();
            return m_polymat;
        }
    }

    public bool scaling
    {
        get
        {
            return m_IsScaleOn;
        }
        set
        {
            m_IsScaleOn = value;
        }
    }

    public double tol
    {
        get
        {
            return m_tol;
        }
        set
        {
            m_tol = value;
            if (m_tol > 1E-05)
            {
                m_tol = 1E-05;
            }
        }
    }

    private void unscale_polymat()
    {
        if (m_IsScaleOn)
        {
            MatVal matVal = m_polymat.Column(0);
            m_polymat = MatVal.StripDimension(m_polymat);
            UnScale();
            for (int i = 0; i < m_polymat.Rows; i++)
            {
                NumVal numVal = matVal.get_1(i) as NumVal;
                double re = m_polymat.get_real(i, 0);
                m_polymat.set_2(i, 0, re, numVal.m_dim);
            }
        }
    }

    public void fituser(MatVal P, m_fittype ftyp)
    {
        m_polymat = P;
        m_nterms = P.Rows;
        m_ConstTerm = false;
        m_hpwr = 1;
        for (int i = 0; i < m_nterms; i++)
        {
            int num = 0;
            for (int j = 0; j < m_nfacts; j++)
            {
                NumVal numVal = m_polymat.get_2(i, j + 1) as NumVal;
                num += (int)numVal.Data.rp;
            }
            if (num == 0)
            {
                m_ConstTerm = true;
            }
            m_hpwr = Math.Max(m_hpwr, num);
        }
        verify_unique_gens();
        if (m_nterms == NumCumulativeMonomials(m_nfacts, m_hpwr))
        {
            m_IsScaleOn = true;
        }
        dofitting(ftyp);
    }

    public void fituser(string polyterms, m_fittype ftyp)
    {
        string[] array = polyterms.Split(new char[5] { ' ', ',', '\t', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
        int num = 0;
        string[] names = array;
        foreach (string text in names)
        {
            array[num++] = text.ToUpper();
        }
        if (!m_utils.verifyGens(m_nfacts, array))
        {
            AppException.Raise("invalid term", 3u);
        }
        m_nterms = array.Length + 1;
        m_polymat = new MatVal(m_nterms, m_nfacts + 1);
        Array.Clear(m_polymat.m_rarr, 0, m_polymat.m_rarr.Length);
        m_polymat.set_2(0, 0, 1.0);
        m_hpwr = 1;
        for (num = 1; num < m_nterms; num++)
        {
            m_utils.getGenIndices(array[num - 1], out var findx);
            m_hpwr = Math.Max(m_hpwr, findx.Length);
            for (int j = 0; j < findx.Length; j++)
            {
                double value = m_polymat.get_real(num, findx[j] + 1);
                m_polymat.set_2(num, findx[j] + 1, value + 1.0);
                m_polymat.set_2(num, 0, 1.0);
            }
        }
        verify_unique_gens();
        if (m_nterms == NumCumulativeMonomials(m_nfacts, m_hpwr))
        {
            m_IsScaleOn = true;
        }
        dofitting(ftyp);
    }

    private void verify_unique_gens()
    {
        int[] array = new int[m_nfacts];
        for (int i = 0; i < m_nterms; i++)
        {
            for (int j = 0; j < m_nfacts; j++)
            {
                array[j] = (int)m_polymat.m_rarr[m_polymat.m_offset + i + (j + 1) * m_polymat.Anr];
            }
            for (int k = i + 1; k < m_nterms; k++)
            {
                int num = 0;
                for (int l = 0; l < m_nfacts; l++)
                {
                    if (array[l] == (int)m_polymat.m_rarr[m_polymat.m_offset + k + (l + 1) * m_polymat.Anr])
                    {
                        num++;
                    }
                }
                if (num == m_nfacts)
                {
                    AppException.Raise("duplicate terms not allowed", 3u);
                }
            }
        }
    }

    public void fitfull(int degree, m_fittype ftyp)
    {
        m_nterms = NumCumulativeMonomials(m_nfacts, degree);
        m_hpwr = degree;
        m_polymat = new MatVal(m_nterms, m_nfacts + 1);
        Array.Clear(m_polymat.m_rarr, 0, m_polymat.m_rarr.Length);
        CreateCumulativeMonomials(0, m_nfacts, degree);
        dofitting(ftyp);
    }

    private void CheckDmatType(MatVal A, MatVal Y)
    {
        // Mathcad keeps the observation flag and the run averages in the first two local slots.
        bool hasObservations;
        double[] array;
        if (A.get_2(0, 0) is StrVal)
        {
            m_nfacts = A.Cols - 2;
            m_nruns = A.Rows - 1;
            m_A = A.SubMatrix(1u, (uint)A.Rows, 2u, (uint)A.Cols);
        }
        else
        {
            m_nfacts = A.Cols;
            m_nruns = A.Rows;
            m_A = A;
        }
        if (m_A.IsUniformScalar)
        {
            m_Adim = new Dim[m_A.Cols];
            for (int i = 0; i < m_A.Cols; i++)
            {
                m_Adim[i] = m_A.m_dim;
            }
        }
        else
        {
            m_Adim = m_A.RowDimArray(0);
        }
        if (m_A.Rows != Y.Rows)
        {
            AppException.Raise("must have same number of runs", 2u);
        }
        m_Y = Y;
        if (!analyzeObservations(out hasObservations))
        {
            AppException.Raise("must have at least one observation per run", 2u);
        }
        array = new double[m_nruns];
        for (int j = 0; j < m_nruns; j++)
        {
            array[j] = m_runInfo[j].m_avg;
        }
        m_RHS = new MatVal(array);
        m_RHS.m_dim = m_Ydim;
    }

    private int NumCumulativeMonomials(int nVars, int degree)
    {
        int i;
        double result = 1.0;
        if (1 == degree)
        {
            return nVars + 1;
        }
        for (i = 1; i <= degree; i++)
        {
            result *= i;
        }
        result = 1.0 / result;
        for (i = nVars + degree - 1; i >= nVars; i--)
        {
            result *= i;
        }
        result *= (double)(nVars + degree) / nVars;
        return (int)(result + 0.5);
    }

    private int CreateCumulativeMonomials(int row, int nVars, int degree)
    {
        int count = NumCumulativeMonomials(nVars, degree - 1);
        int i;
        int index;
        if (1 == nVars)
        {
            for (i = 0; i <= degree; i++)
            {
                index = i + row + m_polymat.m_offset + m_polymat.Anr;
                m_polymat.m_rarr[index] += i;
            }
            return 0;
        }
        if (1 == degree)
        {
            for (i = 0; i < nVars; i++)
            {
                index = i + row + m_polymat.m_offset + (i + 1) * m_polymat.Anr;
                m_polymat.m_rarr[index]++;
            }
            return 0;
        }
        for (i = 0; i < count; i++)
        {
            index = i + row + m_polymat.m_offset + nVars * m_polymat.Anr;
            m_polymat.m_rarr[index]++;
        }
        CreateCumulativeMonomials(row, nVars, degree - 1);
        CreateCumulativeMonomials(row + count, nVars - 1, degree);
        return 0;
    }

    private int CoefIndex(int[] R, int[] S, int var_index)
    {
        int j;
        for (int i = 0; i < m_nterms; i++)
        {
            for (j = 0; j < m_nfacts; j++)
            {
                int num = m_polymat.m_offset + i + (j + 1) * m_polymat.Anr;
                if (m_polymat.m_rarr[num] != (double)(R[j] + ((j < var_index) ? S[j] : 0)))
                {
                    break;
                }
            }
            if (j == m_nfacts)
            {
                return i;
            }
        }
        return -1;
    }

    private void dofitting(m_fittype ftyp)
    {
        sortPolyMat(m_polymat);
        if (ftyp == m_fittype.LS)
        {
            if (m_IsScaleOn)
            {
                getScaling();
            }
            constructLHS();
            MatVal dim2 = IntelMKL.lsolve(m_LHS, m_RHS) as MatVal;
            setDim(dim2);
            return;
        }
        if (ftyp == m_fittype.LM)
        {
            m_IsScaleOn = false;
            constructLHS();
            MatVal dim = Fitting.polylmfit(m_tol, m_A, m_RHS, m_polymat);
            setDim(dim);
        }
    }

    private void constructLHS()
    {
        m_LHS = new MatVal(m_nruns, m_nterms);
        m_LHS.m_dim = Dim.identity;
        if (m_IsScaleOn)
        {
            m_LHS_ns = new MatVal(m_nruns, m_nterms);
            m_LHS_ns.m_dim = Dim.identity;
        }
        else
        {
            m_LHS_ns = m_LHS;
        }
        double[] array = new double[m_nfacts];
        for (int i = 0; i < m_nruns; i++)
        {
            int num = 0;
            for (int j = 0; j < m_nfacts; j++)
            {
                NumVal numVal = m_A.get_2(i, j) as NumVal;
                array[num++] = numVal.Data.rp;
            }
            for (int k = 0; k < m_nterms; k++)
            {
                double value = 1.0;
                double otherValue = 1.0;
                for (int l = 0; l < m_nfacts; l++)
                {
                    double rp = (m_polymat.get_2(k, l + 1) as NumVal).Data.rp;
                    if (rp > 0.0)
                    {
                        if (m_IsScaleOn)
                        {
                            double[] values = m_scale.GetValue(l) as double[];
                            value *= Math.Pow((array[l] - values[0]) / values[1], rp);
                            otherValue *= Math.Pow(array[l], rp);
                        }
                        else
                        {
                            value *= Math.Pow(array[l], rp);
                        }
                    }
                }
                m_LHS.set_2(i, k, value);
                if (m_IsScaleOn)
                {
                    m_LHS_ns.set_2(i, k, otherValue);
                }
            }
        }
    }

    private void setDim(MatVal coef)
    {
        int offset = coef.m_offset;
        m_polymat.m_dim = Dim.identity;
        for (int i = 0; i < m_nterms; i++)
        {
            Dim dim = Dim.identity;
            for (int j = 0; j < m_nfacts; j++)
            {
                double rp = (m_polymat.get_2(i, j + 1) as NumVal).Data.rp;
                if (rp > 0.0)
                {
                    dim = Dim.Mult(dim, Dim.Pwr(m_Adim[j], rp));
                }
            }
            dim = Dim.Div(m_Ydim, dim);
            m_polymat.set_2(i, 0, new NumVal(coef.m_rarr[i + offset], 0.0, dim));
        }
    }

    public NumVal evalPoly(object arg)
    {
        if (arg is NumVal numVal)
        {
            if (m_nfacts > 1)
            {
                AppException.Raise("must be real vector", 1u);
            }
            m_work[0] = numVal.asReal();
            if (!Dim.Eq_b(numVal.m_dim, m_Adim[0]))
            {
                AppException.Raise("unexpected dimension", 1u);
            }
        }
        else
        {
            MatVal matVal = Utl.as_vector(arg, 1u);
            if (!matVal.IsRealMatrixMixed)
            {
                AppException.Raise("must be real vector", 1u);
            }
            if (matVal.Rows != m_nfacts)
            {
                AppException.Raise("wrong size vector", 1u);
            }
            Dim[] array = null;
            if (matVal.IsUniformScalar)
            {
                array = new Dim[matVal.Rows];
                for (int i = 0; i < matVal.Rows; i++)
                {
                    array[i] = matVal.m_dim;
                }
            }
            else
            {
                array = matVal.ColDimArray(0);
            }
            for (int j = 0; j < m_nfacts; j++)
            {
                NumVal numVal2 = matVal.get_1(j) as NumVal;
                m_work[j] = numVal2.Data.rp;
                if (!Dim.Eq_b(array[j], m_Adim[j]))
                {
                    AppException.Raise("unexpected dimension", 1u);
                }
            }
        }
        double num = 0.0;
        for (int k = 0; k < m_nterms; k++)
        {
            double value = (m_polymat.get_2(k, 0) as NumVal).Data.rp;
            for (int l = 0; l < m_nfacts; l++)
            {
                double rp = (m_polymat.get_2(k, l + 1) as NumVal).Data.rp;
                if (rp > 0.0)
                {
                    if (m_IsScaleOn)
                    {
                        double[] values = m_scale.GetValue(l) as double[];
                        value *= Math.Pow((m_work[l] - values[0]) / values[1], rp);
                    }
                    else
                    {
                        value *= Math.Pow(m_work[l], rp);
                    }
                }
            }
            num += value;
        }
        return new NumVal(num, 0.0, m_Ydim);
    }

    private void getScaling()
    {
        m_scale = Array.CreateInstance(typeof(double[]), m_nfacts);
        NumVal[] array = new NumVal[m_nruns];
        for (int i = 0; i < m_nfacts; i++)
        {
            double num = 0.0;
            for (int j = 0; j < m_nruns; j++)
            {
                array[j] = m_A.get_2(j, i) as NumVal;
                if (array[j] == null)
                {
                    AppException.Raise("must be real matrix", 1u);
                }
                num += array[j].Data.rp;
            }
            double value = num / (double)m_nruns;
            double otherValue = 0.0;
            for (int k = 0; k < m_nruns; k++)
            {
                double nextValue = array[k].Data.rp - value;
                otherValue += nextValue * nextValue;
            }
            otherValue = Math.Sqrt(otherValue / (double)m_nruns);
            if (otherValue < 0.01)
            {
                otherValue = 0.01;
            }
            m_scale.SetValue(new double[2] { value, otherValue }, i);
        }
    }

    private double SumTail(int[] R, int[] S, int deg_remainder, int var_index)
    {
        if (deg_remainder == 0 || var_index >= m_nfacts)
        {
            int num = CoefIndex(R, S, var_index);
            if (num == -1)
            {
                AppException.Raise("internal error");
            }
            double value = m_polymat.m_rarr[m_polymat.m_offset + num];
            for (int i = 0; i < var_index; i++)
            {
                double[] array = m_scale.GetValue(i) as double[];
                double otherValue = -array[0] / array[1];
                for (int j = 1; j <= S[i]; j++)
                {
                    value *= otherValue * (double)(R[i] + j) / (double)j;
                }
            }
            return value;
        }
        double nextValue = 0.0;
        for (int k = 0; k <= deg_remainder; k++)
        {
            S[var_index] = k;
            nextValue += SumTail(R, S, deg_remainder - k, var_index + 1);
        }
        return nextValue;
    }

    private void UnScale()
    {
        double[] array = new double[m_nterms];
        int[] indices = new int[m_nfacts];
        int[] otherValues = new int[m_nfacts];
        for (int i = 0; i < m_nfacts; i++)
        {
            indices[i] = 0;
        }
        for (int j = 0; j < m_nterms; j++)
        {
            for (int k = 0; k < m_nfacts; k++)
            {
                int num = m_polymat.m_offset + j + (k + 1) * m_polymat.Anr;
                otherValues[k] = (int)m_polymat.m_rarr[num];
            }
            int value = 0;
            double otherValue = 1.0;
            for (int l = 0; l < m_nfacts; l++)
            {
                double[] values = m_scale.GetValue(l) as double[];
                double nextValue = 1.0 / values[1];
                int currentValue = otherValues[l];
                value += currentValue;
                for (int m = 0; m < currentValue; m++)
                {
                    otherValue *= nextValue;
                }
            }
            array[j] = otherValue * SumTail(otherValues, indices, m_hpwr - value, 0);
        }
        for (int n = 0; n < m_nterms; n++)
        {
            m_polymat.m_rarr[m_polymat.m_offset + n] = array[n];
        }
    }

    private void sortPolyMat(MatVal P)
    {
        List<TermInfo> list = new List<TermInfo>();
        m_TermLabel = new string[P.Rows];
        int rows = P.Rows;
        int num = P.Cols - 1;
        for (int i = 0; i < rows; i++)
        {
            string text = "";
            int value = 0;
            for (int j = 0; j < num; j++)
            {
                int otherValue = (P.get_2(i, j + 1) as NumVal).asInt();
                value = Math.Max(value, otherValue);
                if (otherValue > 0)
                {
                    string label = m_utils.icharMap[j];
                    for (int k = 0; k < otherValue; k++)
                    {
                        text += label;
                    }
                }
            }
            if (text == "")
            {
                text = "Intercept";
            }
            list.Add(new TermInfo(text, value));
            m_TermLabel[i] = text;
        }
        StrngCompare comparer = new StrngCompare();
        list.Sort(comparer);
        for (int l = 0; l < rows; l++)
        {
            for (int m = l; m < rows; m++)
            {
                if (m_TermLabel[m].Equals(list[l].label) && l != m)
                {
                    string name = m_TermLabel[m];
                    m_TermLabel[m] = m_TermLabel[l];
                    m_TermLabel[l] = name;
                    for (int n = 0; n < P.Cols; n++)
                    {
                        object x = P.get_2(l, n);
                        P.set_2(l, n, P.get_2(m, n));
                        P.set_2(m, n, x);
                    }
                    break;
                }
            }
        }
    }

    public MatVal getFitc(double cip, bool unscale)
    {
        if (unscale)
        {
            unscale_polymat();
        }
        MatVal matVal = new MatVal(m_nterms + 1, 8);
        matVal.m_dim = Dim.identity;
        matVal.set_2(0, 0, new StrVal("Term"));
        matVal.set_2(0, 1, new StrVal("Coefficient"));
        matVal.set_2(0, 2, new StrVal("Std Error"));
        matVal.set_2(0, 3, new StrVal(cip * 100.0 + "% CI Low"));
        matVal.set_2(0, 4, new StrVal(cip * 100.0 + "% CI High"));
        matVal.set_2(0, 5, new StrVal("VIF"));
        matVal.set_2(0, 6, new StrVal("T"));
        matVal.set_2(0, 7, new StrVal("P"));
        for (int i = 0; i < m_nterms; i++)
        {
            matVal.set_2(i + 1, 0, new StrVal(m_TermLabel[i]));
        }
        for (int j = 0; j < m_nterms; j++)
        {
            matVal.set_2(j + 1, 1, m_polymat.get_2(j, 0));
        }
        for (int k = 0; k < m_nterms; k++)
        {
            for (int l = 2; l < 8; l++)
            {
                matVal.set_2(k + 1, l, double.NaN);
            }
        }
        return matVal;
    }

    private void getResiduals(double[] y_pred, double[] res)
    {
        MatVal matVal = new MatVal(m_nfacts);
        for (int i = 0; i < m_nruns; i++)
        {
            for (int j = 0; j < m_nfacts; j++)
            {
                matVal.set_1(j, m_A.get_2(i, j));
            }
            y_pred[i] = evalPoly(matVal).asReal();
            res[i] = m_RHS.get_real_mixed(i) - y_pred[i];
        }
    }

    private double get_SS_PE(out int dof_pe)
    {
        ArrayList arrayList = FindDuplicates(m_A);
        double num = 0.0;
        dof_pe = 0;
        if (arrayList.Count == 0 && m_Y.Cols == 1)
        {
            return num;
        }
        bool[] array = new bool[m_nruns];
        for (int i = 0; i < m_nruns; i++)
        {
            array[i] = false;
        }
        int value = 0;
        if (arrayList.Count > 0)
        {
            for (int j = 0; j < arrayList.Count; j++)
            {
                List<int> list = arrayList[j] as List<int>;
                value += list.Count;
                double otherValue = 0.0;
                foreach (int item in list)
                {
                    array[item] = true;
                    otherValue += m_RHS.get_real_mixed(item);
                }
                otherValue /= (double)list.Count;
                foreach (int nextValue in list)
                {
                    for (int k = 0; k < m_Y.Cols; k++)
                    {
                        double currentValue = m_Y.get_real_mixed(nextValue, k);
                        if (!double.IsNaN(currentValue))
                        {
                            num += (currentValue - otherValue) * (currentValue - otherValue);
                        }
                    }
                }
            }
        }
        int count = m_nruns - value + arrayList.Count;
        dof_pe = m_nobsrv - count;
        if (m_Y.Cols > 1)
        {
            for (int l = 0; l < m_nruns; l++)
            {
                if (array[l])
                {
                    continue;
                }
                for (int m = 0; m < m_Y.Cols; m++)
                {
                    double calculatedValue = m_Y.get_real_mixed(l, m);
                    if (!double.IsNaN(calculatedValue))
                    {
                        double sampleValue = m_RHS.get_real_mixed(l);
                        num += (calculatedValue - sampleValue) * (calculatedValue - sampleValue);
                    }
                }
            }
        }
        return num;
    }
}
