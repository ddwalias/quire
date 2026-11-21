using System;
using System.Collections;
using System.IO;
using System.Xml;
using Quire.AutomationPrivate;
using Quire.Mcd;

namespace Quire.Automation;

public partial class MatrixValue : Value, IMatrixValue, IValue, IMatrixValuePrivate
{
    private int size = -1;

    public MatrixValue()
        : base(new MatVal())
    {
    }

    public MatrixValue(MatVal mat)
        : base(mat, RuntimeOptions.SaveThreadOptions())
    {
    }

    public MatrixValue(MatVal mat, RuntimeOptions o)
        : base(mat, o)
    {
    }

    public MatrixValue(double[] rarr, int r, int c)
        : base(new MatVal(rarr, null, r, c))
    {
    }

    public MatrixValue(double[] rarr, double[] iarr, int r, int c)
        : base(new MatVal(rarr, iarr, r, c))
    {
    }

    public MatrixValue(int r, int c)
        : base(new MatVal(r, c))
    {
    }

    public MatVal Matrix => (MatVal)core;

    internal override void WriteValueXml(XmlWriter xml, UnitAction action, double scaleFactor)
    {
        size = 0;
        Value.WriteMathElementStart(xml, "matrix");
        xml.WriteAttributeString("rows", Rows.ToString(Value.NumFmt));
        xml.WriteAttributeString("cols", Cols.ToString(Value.NumFmt));
        for (int i = 0; i < Matrix.Cols; i++)
        {
            for (int j = 0; j < Matrix.Rows; j++)
            {
                Value value = (Value)GetElement(j, i);
                value.WriteXml(xml, action, scaleFactor);
                size += value.Size;
            }
        }
        xml.WriteEndElement();
    }

    internal override void WriteValueRL(TextWriter rl, UnitAction action)
    {
        rl.Write("(op_matrix (unboxed {0}) (unboxed {1}) (list ", Rows.ToString(Value.NumFmt), Cols.ToString(Value.NumFmt));
        size = 0;
        for (int i = 0; i < Matrix.Cols; i++)
        {
            for (int j = 0; j < Matrix.Rows; j++)
            {
                Value value = (Value)GetElement(j, i);
                rl.Write(" ");
                value.WriteRL(rl, action);
                size += value.Size;
            }
        }
        rl.Write("))");
    }

    private int CountElements(MatVal m)
    {
        int num = m.Rows * m.Cols;
        if (m.m_iarr != null)
        {
            num += num / 2;
        }
        if (m.m_parr != null)
        {
            foreach (DictionaryEntry item in m.m_parr)
            {
                MatVal m2 = item.Value as MatVal;
                if (m2 != null)
                {
                    num += CountElements(m2);
                }
            }
        }
        return num;
    }

    public override int Size
    {
        get
        {
            if (size == -1)
            {
                size = CountElements(Matrix);
            }
            return size;
        }
    }

    public bool IsComplex
    {
        get
        {
            return Matrix.m_iarr != null;
        }
        set
        {
            if (value != IsComplex)
            {
                if (Matrix.Immutable)
                {
                    core = new MatVal(Rows, Cols, value);
                }
                else if (value)
                {
                    Matrix.AllocImaginary();
                }
                else
                {
                    Matrix.m_iarr = null;
                }
            }
        }
    }

    public int Rows => Matrix.Rows;

    public int Cols => Matrix.Cols;

    public bool IsUniform => Matrix.IsUniform;

    public bool IsScalarMatrix => Matrix.IsScalarMatrix;

    public bool IsUniformScalar => Matrix.IsUniformScalar;

    public bool IsRealMatrix => Matrix.IsRealMatrix;

    public bool IsRealMatrixMixed => Matrix.IsRealMatrixMixed;

    public virtual void SetElement(int row, int col, object obj)
    {
        if (obj == this)
        {
            throw new ArgumentException();
        }
        object v;
        if (!(obj is Value))
        {
            v = ValueFactory.VariantToEngine(obj, null);
        }
        else
        {
            v = ValueFactory.Instance.AutomationToEngine(obj);
        }
        core = Matrix.mcd_set_2(row, col, v);
    }

    public virtual object GetElement(int row, int col)
    {
        object eng = Matrix.get_2(row, col);
        return ValueFactory.Instance.EngineToAutomation(eng, m_options);
    }

    public override string Type => "Matrix";

    public override string AsString => string.Format("Matrix({0},{1})", Rows.ToString(Value.NumFmt), Cols.ToString(Value.NumFmt));

    public bool IsRange => Matrix.IsRange;
}
