using System;
using System.Collections;
using Quire.AutomationPrivate;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Automation;

public partial class ValueFactory : IValueFactory
{
    public static ValueFactory Instance = new ValueFactory();

    internal static MatVal RangeToVector(RngVal range)
    {
        MatVal matVal = range.asRealVector();
        matVal.IsRange = true;
        matVal.Immutable = true;
        return matVal;
    }

    internal static object TransposeArray(double[] values, int rows, int cols)
    {
        if (values == null)
        {
            return null;
        }
        if (rows == 1)
        {
            return values.Clone();
        }
        double[,] array = new double[rows, cols];
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                array[j, i] = values[i * rows + j];
            }
        }
        return array;
    }

    internal static bool GetDimensions(object o, out int rows, out int cols)
    {
        rows = (cols = 0);
        if (!(o is Array array) || array.Rank < 1 || array.Rank > 2)
        {
            return false;
        }
        rows = array.GetUpperBound(0) - array.GetLowerBound(0) + 1;
        if (array.Rank == 2)
        {
            cols = array.GetUpperBound(1) - array.GetLowerBound(1) + 1;
        }
        else
        {
            cols = 1;
        }
        if (rows >= 1)
        {
            return cols >= 1;
        }
        return false;
    }

    internal static object FirstValue(Array arr)
    {
        IEnumerator enumerator = arr.GetEnumerator();
        if (enumerator.MoveNext())
        {
            return enumerator.Current;
        }
        return null;
    }

    internal static object VariantToEngine(object real, object imag)
    {
        if (real is string)
        {
            return real;
        }
        if (real is Array || imag is Array)
        {
            int rows = 0;
            int cols = 0;
            if (real != null && imag != null)
            {
                if (!GetDimensions(real, out rows, out cols))
                {
                    throw new ArgumentException();
                }
                if (!GetDimensions(imag, out var rows2, out var cols2))
                {
                    throw new ArgumentException();
                }
                if (rows != rows2 || cols != cols2)
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                if (real != null && !GetDimensions(real, out rows, out cols))
                {
                    throw new ArgumentException();
                }
                if (imag != null && !GetDimensions(imag, out rows, out cols))
                {
                    throw new ArgumentException();
                }
            }
            MatVal matVal = new MatVal(rows, cols, imag != null);
            IEnumerator enumerator = ((real != null) ? (real as Array).GetEnumerator() : null);
            IEnumerator imaginaryEnumerator = ((imag != null) ? (imag as Array).GetEnumerator() : null);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (real != null)
                    {
                        enumerator.MoveNext();
                    }
                    if (imag != null)
                    {
                        imaginaryEnumerator.MoveNext();
                    }
                    matVal.set_2(i, j, VariantToEngine((real != null) ? enumerator.Current : ((object)0), (imag != null) ? imaginaryEnumerator.Current : ((object)0)));
                }
            }
            return matVal;
        }
        return new NumVal(Convert.ToDouble(real), Convert.ToDouble(imag));
    }

    public static object ToAutomation(object o, string sig)
    {
        return ToAutomation(o, sig, RuntimeOptions.SaveThreadOptions());
    }

    public static object ToAutomation(object o, string sig, RuntimeOptions options)
    {
        try
        {
            DimVal dimVal = o as DimVal;
            if (dimVal != null && dimVal.m_dim == null)
            {
                ((DimVal)o).m_dim = Dim.Parse(sig);
            }
        }
        catch (NonCriticalException ex)
        {
            if (!ex.Message.Contains(NonCriticalException.ArgumentIsNull))
            {
                throw new NonCriticalException(NonCriticalException.Unexpected);
            }
        }
        catch
        {
        }
        return Instance.EngineToAutomation(o, options);
    }

    public static object FromAutomation(object o)
    {
        return Instance.AutomationToEngine(o);
    }

    public virtual object EngineToAutomation(object eng)
    {
        return EngineToAutomation(eng, RuntimeOptions.SaveThreadOptions());
    }

    public virtual object EngineToAutomation(object eng, RuntimeOptions options)
    {
        if (eng == null)
        {
            return null;
        }
        NumVal num = eng as NumVal;
        if (num != null)
        {
            return new NumericValue(num, options);
        }
        MatVal matVal = eng as MatVal;
        if (matVal != null)
        {
            matVal.Immutable = true;
            return new MatrixValue(matVal, options);
        }
        RngVal range = eng as RngVal;
        if (range != null)
        {
            matVal = RangeToVector(range);
            return new MatrixValue(matVal, options);
        }
        StrVal strVal = eng as StrVal;
        if (strVal != null)
        {
            return new StringValue(strVal.m_string, options);
        }
        if (eng is NotAVal)
        {
            return new NumericValue(NumVal.mcNaN, options);
        }
        string s = eng as string;
        if (s != null)
        {
            return new StringValue(s, options);
        }
        Function f = eng as Function;
        if (f != null)
        {
            return new FunctionValue(f, options);
        }
        return new SymbolicValue(eng, options);
    }

    public virtual object AutomationToEngine(object auto)
    {
        if (auto == null)
        {
            return null;
        }
        if (auto is Value value)
        {
            return value.Core;
        }
        throw new InvalidCastException();
    }

    public virtual object VariantToAutomation(object real, object imag)
    {
        return EngineToAutomation(VariantToEngine(real, imag));
    }

    public virtual void AutomationToVariant(object auto, out object real, out object imag)
    {
        if (auto is NumericValue numericValue)
        {
            NumVal number = numericValue.Number;
            real = number.Data.rp;
            imag = number.Data.ip;
            return;
        }
        if (auto is MatrixValue matrixValue)
        {
            MatVal matrix = matrixValue.Matrix;
            real = TransposeArray(matrix.m_rarr, matrix.Rows, matrix.Cols);
            imag = TransposeArray(matrix.m_iarr, matrix.Rows, matrix.Cols);
            return;
        }
        if (auto is StringValue stringValue)
        {
            real = stringValue.String;
            imag = null;
            return;
        }
        throw new ArgumentException();
    }

    public virtual object WrapperToAutomation(object wrap)
    {
        throw new NotImplementedException();
    }

    public virtual object AutomationToWrapper(object auto)
    {
        throw new NotImplementedException();
    }

    public virtual object VSDataToAutomation(object vs)
    {
        throw new NotImplementedException();
    }

    public virtual object AutomationToVSData(object auto)
    {
        throw new NotImplementedException();
    }

    public ValueFactory()
    {
    }
}
