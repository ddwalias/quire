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
        string[] array2 = array;
        foreach (string text in array2)
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
                double num2 = m_polymat.get_real(num, findx[j] + 1);
                m_polymat.set_2(num, findx[j] + 1, num2 + 1.0);
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
                double num2 = 1.0;
                double num3 = 1.0;
                for (int l = 0; l < m_nfacts; l++)
                {
                    double rp = (m_polymat.get_2(k, l + 1) as NumVal).Data.rp;
                    if (rp > 0.0)
                    {
                        if (m_IsScaleOn)
                        {
                            double[] array2 = m_scale.GetValue(l) as double[];
                            num2 *= Math.Pow((array[l] - array2[0]) / array2[1], rp);
                            num3 *= Math.Pow(array[l], rp);
                        }
                        else
                        {
                            num2 *= Math.Pow(array[l], rp);
                        }
                    }
                }
                m_LHS.set_2(i, k, num2);
                if (m_IsScaleOn)
                {
                    m_LHS_ns.set_2(i, k, num3);
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
            double num2 = (m_polymat.get_2(k, 0) as NumVal).Data.rp;
            for (int l = 0; l < m_nfacts; l++)
            {
                double rp = (m_polymat.get_2(k, l + 1) as NumVal).Data.rp;
                if (rp > 0.0)
                {
                    if (m_IsScaleOn)
                    {
                        double[] array2 = m_scale.GetValue(l) as double[];
                        num2 *= Math.Pow((m_work[l] - array2[0]) / array2[1], rp);
                    }
                    else
                    {
                        num2 *= Math.Pow(m_work[l], rp);
                    }
                }
            }
            num += num2;
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
            double num2 = num / (double)m_nruns;
            double num3 = 0.0;
            for (int k = 0; k < m_nruns; k++)
            {
                double num4 = array[k].Data.rp - num2;
                num3 += num4 * num4;
            }
            num3 = Math.Sqrt(num3 / (double)m_nruns);
            if (num3 < 0.01)
            {
                num3 = 0.01;
            }
            m_scale.SetValue(new double[2] { num2, num3 }, i);
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
            double num2 = m_polymat.m_rarr[m_polymat.m_offset + num];
            for (int i = 0; i < var_index; i++)
            {
                double[] array = m_scale.GetValue(i) as double[];
                double num3 = -array[0] / array[1];
                for (int j = 1; j <= S[i]; j++)
                {
                    num2 *= num3 * (double)(R[i] + j) / (double)j;
                }
            }
            return num2;
        }
        double num4 = 0.0;
        for (int k = 0; k <= deg_remainder; k++)
        {
            S[var_index] = k;
            num4 += SumTail(R, S, deg_remainder - k, var_index + 1);
        }
        return num4;
    }

    private void UnScale()
    {
        double[] array = new double[m_nterms];
        int[] array2 = new int[m_nfacts];
        int[] array3 = new int[m_nfacts];
        for (int i = 0; i < m_nfacts; i++)
        {
            array2[i] = 0;
        }
        for (int j = 0; j < m_nterms; j++)
        {
            for (int k = 0; k < m_nfacts; k++)
            {
                int num = m_polymat.m_offset + j + (k + 1) * m_polymat.Anr;
                array3[k] = (int)m_polymat.m_rarr[num];
            }
            int num2 = 0;
            double num3 = 1.0;
            for (int l = 0; l < m_nfacts; l++)
            {
                double[] array4 = m_scale.GetValue(l) as double[];
                double num4 = 1.0 / array4[1];
                int num5 = array3[l];
                num2 += num5;
                for (int m = 0; m < num5; m++)
                {
                    num3 *= num4;
                }
            }
            array[j] = num3 * SumTail(array3, array2, m_hpwr - num2, 0);
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
            int num2 = 0;
            for (int j = 0; j < num; j++)
            {
                int num3 = (P.get_2(i, j + 1) as NumVal).asInt();
                num2 = Math.Max(num2, num3);
                if (num3 > 0)
                {
                    string text2 = m_utils.icharMap[j];
                    for (int k = 0; k < num3; k++)
                    {
                        text += text2;
                    }
                }
            }
            if (text == "")
            {
                text = "Intercept";
            }
            list.Add(new TermInfo(text, num2));
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
                    string text3 = m_TermLabel[m];
                    m_TermLabel[m] = m_TermLabel[l];
                    m_TermLabel[l] = text3;
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
        int num2 = 0;
        if (arrayList.Count > 0)
        {
            for (int j = 0; j < arrayList.Count; j++)
            {
                List<int> list = arrayList[j] as List<int>;
                num2 += list.Count;
                double num3 = 0.0;
                foreach (int item in list)
                {
                    array[item] = true;
                    num3 += m_RHS.get_real_mixed(item);
                }
                num3 /= (double)list.Count;
                foreach (int item2 in list)
                {
                    for (int k = 0; k < m_Y.Cols; k++)
                    {
                        double num4 = m_Y.get_real_mixed(item2, k);
                        if (!double.IsNaN(num4))
                        {
                            num += (num4 - num3) * (num4 - num3);
                        }
                    }
                }
            }
        }
        int num5 = m_nruns - num2 + arrayList.Count;
        dof_pe = m_nobsrv - num5;
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
                    double num6 = m_Y.get_real_mixed(l, m);
                    if (!double.IsNaN(num6))
                    {
                        double num7 = m_RHS.get_real_mixed(l);
                        num += (num6 - num7) * (num6 - num7);
                    }
                }
            }
        }
        return num;
    }
}
