#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using Quire.DOE;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

public partial struct Sixsigma
{
    public static MatVal verifyGeneralDMatrixFormat(object arg1)
    {
        MatVal matVal = new MatVal(Utl.as_matrix(arg1, 1u));
        if (matVal.HasImag)
        {
            AppException.Raise("must be real", 1u);
        }
        for (int i = 0; i < matVal.Cols; i++)
        {
            if (!(matVal.get_2(0, i) is StrVal))
            {
                AppException.Raise("matrix format error", 1u);
            }
        }
        if (!matVal.SubMatrix(1u, (uint)matVal.Rows, 0u, 2u).IsDimless)
        {
            AppException.Raise("run and block columns must be dimless", 1u);
        }
        for (int j = 2; j < matVal.Cols; j++)
        {
            if (matVal.SubMatrix(1u, (uint)matVal.Rows, (uint)j, (uint)(j + 1)).IsUniformScalar)
            {
                continue;
            }
            for (int k = 1; k < matVal.Rows; k++)
            {
                if (!(matVal.get_2(k, j) is StrVal))
                {
                    AppException.Raise("levels must be all strings or numeric", 1u);
                }
            }
        }
        return matVal;
    }

    public static MatVal verifyNumericDMatrixFormat(object arg1)
    {
        MatVal matVal = new MatVal(Utl.as_matrix(arg1, 1u));
        if (matVal.HasImag)
        {
            AppException.Raise("must be real", 1u);
        }
        for (int i = 0; i < matVal.Cols; i++)
        {
            if (!(matVal.get_2(0, i) is StrVal))
            {
                AppException.Raise("matrix format error", 1u);
            }
        }
        if (!matVal.SubMatrix(1u, (uint)matVal.Rows, 0u, 2u).IsDimless)
        {
            AppException.Raise("run and block columns must be dimless", 1u);
        }
        for (int j = 2; j < matVal.Cols; j++)
        {
            if (!matVal.SubMatrix(1u, (uint)matVal.Rows, (uint)j, (uint)(j + 1)).IsUniformScalar)
            {
                AppException.Raise("matrix format error", 1u);
            }
        }
        return matVal;
    }

    public static MatVal verify2LCodedDMatrix(object arg1)
    {
        MatVal matVal = verifyNumericDMatrixFormat(arg1);
        for (int i = 2; i < matVal.Cols; i++)
        {
            for (int j = 1; j < matVal.Rows; j++)
            {
                double num = matVal.get_real(j, i);
                if (num != -1.0 && num != 1.0)
                {
                    AppException.Raise("must be 2 level -1/+1 coded matrix", 1u);
                }
            }
        }
        return matVal;
    }

    private static int[] verifyBlocking(MatVal A)
    {
        if (!A.SubMatrix(1u, (uint)A.Rows, 1u, 2u).IsDimless)
        {
            AppException.Raise("invalid blocking", 1u);
        }
        int[] array = new int[A.Rows - 1];
        int num = 0;
        for (int i = 1; i < A.Rows; i++)
        {
            array[i - 1] = (int)A.m_rarr[A.Anr + i + A.Offset];
            if (array[i - 1] <= 0)
            {
                AppException.Raise("invalid blocking", 1u);
            }
            num = Math.Max(num, array[i - 1]);
        }
        int num2 = A.Rows - 1;
        if (num2 % num != 0)
        {
            AppException.Raise("invalid blocking", 1u);
        }
        int num3 = num2 / num;
        for (int j = 1; j <= num; j++)
        {
            for (int k = 0; k < num3; k++)
            {
                int num4 = k + (j - 1) * num3;
                if (array[num4] != j)
                {
                    AppException.Raise("invalid blocking", 1u);
                }
            }
        }
        return new int[2] { num, num3 };
    }

    public static bool is_Y_UniformScalar(MatVal Y)
    {
        if (Y.IsUniformScalar)
        {
            return true;
        }
        Dim dim = Dim.identity;
        bool flag = true;
        for (int i = 0; i < Y.Cols; i++)
        {
            for (int j = 0; j < Y.Rows; j++)
            {
                if (double.IsNaN(Y.get_real_mixed(j, i)))
                {
                    continue;
                }
                if (flag)
                {
                    flag = false;
                    dim = Y.get_2_dim(j, i);
                    continue;
                }
                Dim y = Y.get_2_dim(j, i);
                if (!Dim.Eq_b(dim, y))
                {
                    return false;
                }
            }
        }
        Y.m_dim = dim;
        return true;
    }

    public static MatVal randomize(object arg1)
    {
        MatVal matVal = new MatVal(Utl.as_matrix(arg1, 1u));
        if (matVal.HasImag)
        {
            AppException.Raise("must be real", 1u);
        }
        if (matVal.Cols < 3)
        {
            AppException.Raise("must have at least three columns", 1u);
        }
        int[] array = verifyBlocking(matVal);
        int num = array[0];
        int num2 = array[1];
        for (int i = 0; i < num; i++)
        {
            for (int j = 0; j < num2 - 1; j++)
            {
                int num3 = j + (int)(StatisticsExports.uniform() * (double)(num2 - j));
                if (j == num3)
                {
                    continue;
                }
                int i2 = 1 + j + i * num2;
                int i3 = 1 + num3 + i * num2;
                for (int k = 0; k < matVal.Cols; k++)
                {
                    if (k != 1)
                    {
                        object x = matVal.get_2(i2, k);
                        object x2 = matVal.get_2(i3, k);
                        matVal.set_2(i3, k, x);
                        matVal.set_2(i2, k, x2);
                    }
                }
            }
        }
        return matVal;
    }

    private static MatVal fullfact_(object arg1, object arg2, bool keepOldOrder)
    {
        int num = (int)Utl.as_pint(arg1, 1u);
        if (!DimVal.IsDimOf(arg1, Dim.identity))
        {
            AppException.Raise("must be dimless", 1u);
        }
        MatVal dmat;
        if (arg2 != null)
        {
            int[] array = new int[num];
            if (arg2 is NumVal)
            {
                int num2 = (int)Utl.as_pint(arg2, 2u);
                if (!DimVal.IsDimOf(arg2, Dim.identity))
                {
                    AppException.Raise("must be dimless", 2u);
                }
                for (int i = 0; i < num; i++)
                {
                    array[i] = num2;
                }
            }
            else if (arg2 is MatVal)
            {
                MatVal matVal = Utl.as_ivector(arg2, 2u);
                if (!DimVal.IsDimOf(arg2, Dim.identity))
                {
                    AppException.Raise("must be dimless", 2u);
                }
                if (matVal.Rows != num)
                {
                    AppException.Raise("wrong size vector", 2u);
                }
                for (int j = 0; j < num; j++)
                {
                    array[j] = (int)matVal.m_rarr[j + matVal.m_offset];
                }
            }
            else
            {
                AppException.Raise("must be an integer or a vector", 2u);
            }
            new Permutations(num, array, keepOldOrder).getDesignMatrix(out dmat);
        }
        else
        {
            new Permutations(num, keepOldOrder).getDesignMatrix(out dmat);
        }
        return dmat;
    }

    public static MatVal foldover(object arg1, object arg2)
    {
        MatVal matVal = verify2LCodedDMatrix(arg1);
        int num = matVal.Cols - 2;
        int num2 = matVal.Rows - 1;
        double num3 = Math.Log(Math.Pow(2.0, num) / (double)num2) / Math.Log(2.0);
        int num4 = 1 << num - (int)num3;
        int num5 = 1 << num - (int)(num3 + 0.5);
        if (num3 == 0.0 || (num2 != num4 && num2 != num5))
        {
            AppException.Raise("must be a fractional factorial design", 1u);
        }
        if (verifyBlocking(matVal)[0] != 1)
        {
            AppException.Raise("must be unblocked 2 level coded fractfact design", 1u);
        }
        MatVal matVal2 = new MatVal(2 * matVal.Rows - 1, matVal.Cols);
        for (int i = 0; i < matVal.Cols; i++)
        {
            matVal2.set_2(0, i, matVal.get_2(0, i));
        }
        int[] array = new int[matVal.Cols - 2];
        for (int j = 0; j < matVal.Cols - 2; j++)
        {
            array[j] = -1;
        }
        if (arg2 != null)
        {
            if (!(arg2 is StrVal))
            {
                AppException.Raise("must be string", 2u);
            }
            string text = (arg2 as StrVal).m_string;
            if (text.Equals(""))
            {
                AppException.Raise("invalid argument", 2u);
            }
            string[] array2 = text.Split(new char[5] { ' ', ',', '\t', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
            int num6 = 0;
            string[] array3 = array2;
            foreach (string text2 in array3)
            {
                array2[num6++] = text2.ToUpper();
            }
            Utils utils = new Utils();
            for (int l = 0; l < array2.Length; l++)
            {
                if (!utils.getGenIndices(array2[l], out var findx) || findx.Length > 1 || findx[0] >= matVal.Cols - 2)
                {
                    AppException.Raise("invalid factor", 2u);
                }
                array[findx[0]] = 1;
            }
            for (int m = 0; m < matVal.Cols - 2; m++)
            {
                array[m] = -array[m];
            }
        }
        for (int n = 1; n < matVal.Rows; n++)
        {
            matVal2.m_rarr[n] = n;
            matVal2.m_rarr[n + num2] = n + num2;
            matVal2.m_rarr[n + matVal2.Anr] = 1.0;
            matVal2.m_rarr[n + num2 + matVal2.Anr] = 2.0;
        }
        for (int num7 = 2; num7 < matVal.Cols; num7++)
        {
            for (int num8 = 1; num8 < matVal.Rows; num8++)
            {
                matVal2.m_rarr[num8 + num7 * matVal2.Anr] = matVal.get_real(num8, num7);
                matVal2.m_rarr[num8 + num2 + num7 * matVal2.Anr] = (double)array[num7 - 2] * matVal2.m_rarr[num8 + num7 * matVal2.Anr];
            }
        }
        return matVal2;
    }

    public static MatVal fractalias(object arg1, object arg2, object arg3)
    {
        if (!(arg1 is StrVal))
        {
            AppException.Raise("must be string", 1u);
        }
        if (!(arg2 is StrVal))
        {
            AppException.Raise("must be string", 2u);
        }
        int order = 0;
        if (arg3 == null)
        {
            order = -1;
        }
        else if (arg3 is NumVal)
        {
            order = (int)Utl.as_pint(arg3, 3u);
            if (!DimVal.IsDimOf(arg3, Dim.identity))
            {
                AppException.Raise("must be dimless", 3u);
            }
        }
        else
        {
            AppException.Raise("invalid argument", 2u);
        }
        string text = (arg1 as StrVal).m_string;
        if (text.Equals(""))
        {
            AppException.Raise("invalid argument", 1u);
        }
        FractFact fractFact = new FractFact(text, keepOldOrder: true);
        string text2 = (arg2 as StrVal).m_string;
        if (text2.Equals(""))
        {
            AppException.Raise("invalid argument", 2u);
        }
        string[] array = text2.Split(new char[5] { ' ', ',', '\t', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
        MatVal matVal = new MatVal(array.Length, 2);
        for (int i = 0; i < array.Length; i++)
        {
            string aliasing = fractFact.getAliasing(array[i].ToUpper(), order);
            matVal.set_2(i, 0, new StrVal(array[i]));
            matVal.set_2(i, 1, new StrVal(aliasing));
        }
        return matVal;
    }

    public static MatVal BoxBehnken(object arg1)
    {
        uint nfacts = Utl.as_pint(arg1, 1u);
        if (!DimVal.IsDimOf(arg1, Dim.identity))
        {
            AppException.Raise("must be dimless", 1u);
        }
        new BoxBehnken((int)nfacts).getDesignMatrix(out var dmat);
        return dmat;
    }

    private static MatVal BoxWilson_(object arg1, object arg2, object arg3, bool keepOldOrder)
    {
        if (!(arg1 is NumVal))
        {
            AppException.Raise("must be positive integer", 1u);
        }
        int num = (int)Utl.as_pint(arg1, 1u);
        if (!DimVal.IsDimOf(arg1, Dim.identity))
        {
            AppException.Raise("must be dimless", 1u);
        }
        if (num < 2)
        {
            AppException.Raise("must have at least 2 factors", 1u);
        }
        if (arg2 == null && arg3 == null)
        {
            new BoxWilson(num, keepOldOrder).getDesignMatrix(out var dmat);
            return dmat;
        }
        if (arg2 is NumVal && arg3 is NumVal)
        {
            int nc = (int)Utl.as_pint(arg2, 2u);
            if (!DimVal.IsDimOf(arg2, Dim.identity))
            {
                AppException.Raise("must be dimless", 2u);
            }
            double alpha = Utl.as_real(arg3, 3u);
            if (!DimVal.IsDimOf(arg3, Dim.identity))
            {
                AppException.Raise("must be dimless", 3u);
            }
            new BoxWilson(num, nc, alpha, keepOldOrder).getDesignMatrix(out var dmat2);
            return dmat2;
        }
        AppException.Raise("inconsistent arguments");
        return null;
    }

    public static MatVal PlackettBurman(object arg1)
    {
        uint nruns = Utl.as_pint(arg1, 1u);
        if (!DimVal.IsDimOf(arg1, Dim.identity))
        {
            AppException.Raise("must be dimless", 1u);
        }
        MatVal dmat = new MatVal();
        new PlackettBurman((int)nruns).getDesignMatrix(out dmat);
        return dmat;
    }

    public static MatVal boxplot(object arg1)
    {
        Plots plots = new Plots();
        MatVal matVal = Utl.as_matrix(arg1, 1u);
        if (!matVal.IsRealMatrixMixed)
        {
            AppException.Raise("must be real", 1u);
        }
        return plots.BoxPlot(matVal);
    }

    public static MatVal effectsgraph(object arg1)
    {
        MatVal matVal = Utl.as_matrix(arg1, 1u);
        if (matVal.Cols != 4)
        {
            AppException.Raise("invalid argument", 1u);
        }
        if (!(matVal.get_2(0, 3) is StrVal strVal) || !strVal.asString().Equals("(+) - (-)"))
        {
            AppException.Raise("invalid argument", 1u);
        }
        MatVal x = matVal.SubMatrix(1u, (uint)matVal.Rows, 0u, 1u);
        MatVal x2 = matVal.SubMatrix(1u, (uint)matVal.Rows, 1u, 3u);
        MatVal matVal2 = new MatVal(2);
        matVal2.set_1(0, x);
        matVal2.set_1(1, x2);
        return matVal2;
    }

    public static MatVal Taguchi(object arg1, object arg2)
    {
        uint nfacts = Utl.as_pint(arg1, 1u);
        if (!DimVal.IsDimOf(arg1, Dim.identity))
        {
            AppException.Raise("must be dimless", 1u);
        }
        int nlevels = (int)Utl.as_pint(arg2, 2u);
        if (!DimVal.IsDimOf(arg2, Dim.identity))
        {
            AppException.Raise("must be dimless", 2u);
        }
        new Taguchi((int)nfacts, nlevels).getDesignMatrix(out var dmat);
        return dmat;
    }

    public static MatVal MixedTaguchi(object arg1, object arg2, object arg3, object arg4)
    {
        uint nfacts = Utl.as_pint(arg1, 1u);
        if (!DimVal.IsDimOf(arg1, Dim.identity))
        {
            AppException.Raise("must be dimless", 1u);
        }
        int nlevels = (int)Utl.as_pint(arg2, 2u);
        if (!DimVal.IsDimOf(arg2, Dim.identity))
        {
            AppException.Raise("must be dimless", 2u);
        }
        int nfacts2 = (int)Utl.as_pint(arg3, 3u);
        if (!DimVal.IsDimOf(arg3, Dim.identity))
        {
            AppException.Raise("must be dimless", 3u);
        }
        int nlevels2 = (int)Utl.as_pint(arg4, 4u);
        if (!DimVal.IsDimOf(arg4, Dim.identity))
        {
            AppException.Raise("must be dimless", 4u);
        }
        new Taguchi((int)nfacts, nlevels, nfacts2, nlevels2).getDesignMatrix(out var dmat);
        return dmat;
    }

    public static MatVal Anova(object arg1, object arg2, object arg3)
    {
        MatVal matVal = verifyNumericDMatrixFormat(arg1);
        verifyBlocking(matVal);
        if (!matVal.SubMatrix(1u, (uint)matVal.Rows, 0u, (uint)matVal.Cols).IsDimless)
        {
            AppException.Raise("must be dimless", 1u);
        }
        MatVal matVal2 = Utl.as_rmatrix(arg2, 2u);
        if (!matVal2.IsUniformScalar)
        {
            AppException.Raise("must be of uniform dimension", 2u);
        }
        if (matVal2.Rows != matVal.Rows - 1)
        {
            AppException.Raise("invalid matrix size", 2u);
        }
        if (matVal2.Cols < 2)
        {
            AppException.Raise("at least 2 replicas are required to compute Anova", 2u);
        }
        Anova anova = new Anova(matVal, matVal2);
        MatVal table = null;
        if (arg3 == null)
        {
            anova.anovaTable(out table, -1);
        }
        else if (arg3 is NumVal)
        {
            int order = (int)Utl.as_pint(arg3, 3u);
            if (!DimVal.IsDimOf(arg3, Dim.identity))
            {
                AppException.Raise("must be dimless", 3u);
            }
            anova.anovaTable(out table, order);
        }
        else if (arg3 is StrVal)
        {
            string text = (arg3 as StrVal).m_string;
            if (text.Equals(""))
            {
                AppException.Raise("invalid argument", 3u);
            }
            anova.anovaTable(out table, text);
        }
        else if (arg3 != null)
        {
            AppException.Raise("invalid argument", 3u);
        }
        return table;
    }

    public static MatVal QuickScreen(object arg1, object arg2, object arg3)
    {
        MatVal matVal = verify2LCodedDMatrix(arg1);
        verifyBlocking(matVal);
        MatVal matVal2 = Utl.as_matrix(arg2, 2u);
        if (!matVal2.IsRealMatrixMixed)
        {
            AppException.Raise("must be real", 2u);
        }
        if (!is_Y_UniformScalar(matVal2))
        {
            AppException.Raise("must be of uniform dimension", 2u);
        }
        if (matVal2.Rows != matVal.Rows - 1)
        {
            AppException.Raise("invalid matrix size", 2u);
        }
        Effects effects = new Effects(matVal, matVal2);
        MatVal table = null;
        if (arg3 == null)
        {
            effects.effectsTable(out table, -1);
        }
        else if (arg3 is NumVal)
        {
            int order = (int)Utl.as_pint(arg3, 3u);
            if (!DimVal.IsDimOf(arg3, Dim.identity))
            {
                AppException.Raise("must be dimless", 3u);
            }
            effects.effectsTable(out table, order);
        }
        else if (arg3 is StrVal)
        {
            string text = (arg3 as StrVal).m_string;
            if (text.Equals(""))
            {
                AppException.Raise("invalid argument", 3u);
            }
            effects.effectsTable(out table, text);
        }
        else if (arg3 != null)
        {
            AppException.Raise("invalid argument", 3u);
        }
        return table;
    }

    public static MatVal Effects(object arg1, object arg2, object arg3)
    {
        MatVal matVal = verifyGeneralDMatrixFormat(arg1);
        verifyBlocking(matVal);
        MatVal matVal2 = Utl.as_matrix(arg2, 2u);
        if (!matVal2.IsRealMatrixMixed)
        {
            AppException.Raise("must be real", 2u);
        }
        if (!is_Y_UniformScalar(matVal2))
        {
            AppException.Raise("must be of uniform dimension", 2u);
        }
        if (matVal2.Rows != matVal.Rows - 1)
        {
            AppException.Raise("invalid matrix size", 2u);
        }
        Effects effects = new Effects(matVal, matVal2);
        if (arg3 == null)
        {
            return effects.get_general_effect(1);
        }
        if (arg3 is NumVal)
        {
            int order = (int)Utl.as_pint(arg3, 3u);
            if (!DimVal.IsDimOf(arg3, Dim.identity))
            {
                AppException.Raise("must be dimless", 3u);
            }
            return effects.get_general_effect(order);
        }
        if (arg3 is StrVal)
        {
            string text = (arg3 as StrVal).m_string;
            if (text.Equals(""))
            {
                AppException.Raise("invalid argument", 3u);
            }
            return effects.get_general_effect(text);
        }
        if (arg3 != null)
        {
            AppException.Raise("invalid argument", 3u);
        }
        return null;
    }

    public static Function Polyfit(double tol, object arg1, object arg2, object arg3)
    {
        MatVal matVal = Utl.as_matrix(arg1, 1u);
        if (matVal.get_2(0, 0) is StrVal)
        {
            matVal = verifyNumericDMatrixFormat(arg1);
        }
        else
        {
            if (!matVal.IsRealMatrixMixed)
            {
                AppException.Raise("must be real", 1u);
            }
            for (int i = 0; i < matVal.Cols; i++)
            {
                if (!matVal.Column(i).IsUniformScalar)
                {
                    AppException.Raise("mixed units in column", 1u);
                }
            }
        }
        MatVal matVal2 = Utl.as_matrix(arg2, 2u);
        if (!matVal2.IsRealMatrixMixed)
        {
            AppException.Raise("must be real", 2u);
        }
        if (!is_Y_UniformScalar(matVal2))
        {
            AppException.Raise("must be of uniform dimension", 2u);
        }
        Polyfit polyfit = new Polyfit(matVal, matVal2);
        polyfit.scaling = true;
        if (arg3 is NumVal)
        {
            int degree = (int)Utl.as_pint(arg3, 3u);
            if (!DimVal.IsDimOf(arg3, Dim.identity))
            {
                AppException.Raise("must be dimless", 3u);
            }
            polyfit.fitfull(degree, Quire.DOE.Polyfit.m_fittype.LS);
        }
        else if (arg3 is MatVal)
        {
            polyfit.fituser(arg3 as MatVal, Quire.DOE.Polyfit.m_fittype.LS);
        }
        else if (arg3 is StrVal)
        {
            string text = (arg3 as StrVal).m_string;
            if (text.Equals(""))
            {
                AppException.Raise("invalid argument", 3u);
            }
            polyfit.fituser(text, Quire.DOE.Polyfit.m_fittype.LS);
        }
        else
        {
            AppException.Raise("invalid argument", 3u);
        }
        return DelegateFunction.make(new polyfit_del(polyfit.evalPoly), 1);
    }

    public static MatVal Polyfitc(double tol, object arg1, object arg2, object arg3, object arg4)
    {
        MatVal matVal = Utl.as_matrix(arg1, 1u);
        if (matVal.get_2(0, 0) is StrVal)
        {
            matVal = verifyNumericDMatrixFormat(arg1);
        }
        else
        {
            if (!matVal.IsRealMatrixMixed)
            {
                AppException.Raise("must be real", 1u);
            }
            for (int i = 0; i < matVal.Cols; i++)
            {
                if (!matVal.Column(i).IsUniformScalar)
                {
                    AppException.Raise("mixed units in column", 1u);
                }
            }
        }
        MatVal matVal2 = Utl.as_matrix(arg2, 2u);
        if (!matVal2.IsRealMatrixMixed)
        {
            AppException.Raise("must be real", 2u);
        }
        if (!is_Y_UniformScalar(matVal2))
        {
            AppException.Raise("must be of uniform dimension", 2u);
        }
        Polyfit polyfit = new Polyfit(matVal, matVal2);
        if (arg3 is NumVal)
        {
            polyfit.scaling = true;
            int degree = (int)Utl.as_pint(arg3, 3u);
            if (!DimVal.IsDimOf(arg3, Dim.identity))
            {
                AppException.Raise("must be dimless", 3u);
            }
            polyfit.fitfull(degree, Quire.DOE.Polyfit.m_fittype.LS);
        }
        else if (arg3 is MatVal)
        {
            polyfit.scaling = false;
            polyfit.fituser(arg3 as MatVal, Quire.DOE.Polyfit.m_fittype.LS);
        }
        else if (arg3 is StrVal)
        {
            polyfit.scaling = false;
            string text = (arg3 as StrVal).m_string;
            if (text.Equals(""))
            {
                AppException.Raise("invalid argument", 3u);
            }
            polyfit.fituser(text, Quire.DOE.Polyfit.m_fittype.LS);
        }
        else
        {
            AppException.Raise("invalid argument", 3u);
        }
        double cip = 0.95;
        if (arg4 != null)
        {
            cip = Utl.as_preal(arg4, 4u, "this value must be a real between 0 and 1");
        }
        return polyfit.getFitStatistics(cip, coeffOnly: true);
    }

    public static MatVal PolyfitStat(double tol, object arg1, object arg2, object arg3, object arg4)
    {
        MatVal matVal = Utl.as_matrix(arg1, 1u);
        if (matVal.get_2(0, 0) is StrVal)
        {
            matVal = verifyNumericDMatrixFormat(arg1);
        }
        else
        {
            if (!matVal.IsRealMatrixMixed)
            {
                AppException.Raise("must be real", 1u);
            }
            for (int i = 0; i < matVal.Cols; i++)
            {
                if (!matVal.Column(i).IsUniformScalar)
                {
                    AppException.Raise("mixed units in column", 1u);
                }
            }
        }
        MatVal matVal2 = Utl.as_matrix(arg2, 2u);
        if (!matVal2.IsRealMatrixMixed)
        {
            AppException.Raise("must be real", 2u);
        }
        if (!is_Y_UniformScalar(matVal2))
        {
            AppException.Raise("must be of uniform dimension", 2u);
        }
        Polyfit polyfit = new Polyfit(matVal, matVal2);
        if (arg3 is NumVal)
        {
            polyfit.scaling = true;
            int degree = (int)Utl.as_pint(arg3, 3u);
            if (!DimVal.IsDimOf(arg3, Dim.identity))
            {
                AppException.Raise("must be dimless", 3u);
            }
            polyfit.fitfull(degree, Quire.DOE.Polyfit.m_fittype.LS);
        }
        else if (arg3 is MatVal)
        {
            polyfit.scaling = false;
            polyfit.fituser(arg3 as MatVal, Quire.DOE.Polyfit.m_fittype.LS);
        }
        else if (arg3 is StrVal)
        {
            polyfit.scaling = false;
            string text = (arg3 as StrVal).m_string;
            if (text.Equals(""))
            {
                AppException.Raise("invalid argument", 3u);
            }
            polyfit.fituser(text, Quire.DOE.Polyfit.m_fittype.LS);
        }
        else
        {
            AppException.Raise("invalid argument", 3u);
        }
        double cip = 0.95;
        if (arg4 != null)
        {
            cip = Utl.as_preal(arg4, 4u, "this value must be a real between 0 and 1");
        }
        return polyfit.getFitStatistics(cip, coeffOnly: false);
    }

    public static Function PolyLMfit(double tol, object arg1, object arg2, object arg3)
    {
        MatVal a = Utl.as_matrix(arg1, 1u);
        MatVal y = Utl.as_matrix(arg2, 2u);
        MatVal p = Utl.as_matrix(arg3, 3u);
        if (!is_Y_UniformScalar(y))
        {
            AppException.Raise("must be of uniform dimension", 2u);
        }
        Polyfit polyfit = new Polyfit(a, y);
        polyfit.scaling = false;
        polyfit.fituser(p, Quire.DOE.Polyfit.m_fittype.LM);
        return DelegateFunction.make(new polyfit_del(polyfit.evalPoly), 1);
    }

    public static MatVal PolyLMfitc(double tol, object arg1, object arg2, object arg3)
    {
        MatVal a = Utl.as_matrix(arg1, 1u);
        MatVal y = Utl.as_matrix(arg2, 2u);
        MatVal p = Utl.as_matrix(arg3, 3u);
        if (!is_Y_UniformScalar(y))
        {
            AppException.Raise("must be of uniform dimension", 2u);
        }
        Polyfit polyfit = new Polyfit(a, y);
        polyfit.scaling = false;
        polyfit.tol = tol;
        polyfit.fituser(p, Quire.DOE.Polyfit.m_fittype.LM);
        return polyfit.polyMat;
    }

    public static MatVal Multidfit(double tol, object arg1, object arg2, object arg3, object arg4)
    {
        MatVal matVal = Utl.as_matrix(arg1, 1u);
        if (matVal.get_2(0, 0) is StrVal)
        {
            matVal = verifyNumericDMatrixFormat(arg1);
        }
        else
        {
            if (!matVal.IsRealMatrixMixed)
            {
                AppException.Raise("must be real", 1u);
            }
            for (int i = 0; i < matVal.Cols; i++)
            {
                if (!matVal.Column(i).IsUniformScalar)
                {
                    AppException.Raise("mixed units in column", 1u);
                }
            }
        }
        MatVal matVal2 = Utl.as_matrix(arg2, 2u);
        if (!matVal2.IsRealMatrixMixed)
        {
            AppException.Raise("must be real", 2u);
        }
        if (!is_Y_UniformScalar(matVal2))
        {
            AppException.Raise("must be of uniform dimension", 2u);
        }
        MatVal matVal3 = Utl.as_matrix(arg3, 3u);
        if (!matVal3.IsRealMatrixMixed)
        {
            AppException.Raise("must be real", 3u);
        }
        Function function = arg4 as Function;
        if (function == null)
        {
            AppException.Raise("must be function", 4u);
        }
        return new Polyfit(matVal, matVal2)
        {
            tol = tol
        }.Multidfit(matVal3, function);
    }

    public static MatVal fullfact(object arg1, object arg2)
    {
        return fullfact_(arg1, arg2, keepOldOrder: false);
    }

    public static MatVal fullfact2(object arg1, object arg2)
    {
        return fullfact_(arg1, arg2, keepOldOrder: true);
    }

    public static MatVal fractfact(object arg1, object arg2)
    {
        return fractfact_(arg1, arg2, keepOldOrder: false);
    }

    public static MatVal fractfact2(object arg1, object arg2)
    {
        return fractfact_(arg1, arg2, keepOldOrder: true);
    }

    public static MatVal BoxWilson(object arg1, object arg2, object arg3, bool keepOldOrder)
    {
        return BoxWilson_(arg1, arg2, arg3, keepOldOrder: false);
    }

    public static MatVal BoxWilson2(object arg1, object arg2, object arg3, bool keepOldOrder)
    {
        return BoxWilson_(arg1, arg2, arg3, keepOldOrder: true);
    }
}
