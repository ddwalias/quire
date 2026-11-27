#nullable disable
#pragma warning disable

using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.efiio_csharp;

public partial class Descriptor
{
    protected int m_Left;

    protected int m_Top;

    protected int m_Right;

    protected int m_Bottom;

    protected object m_Fill;

    protected int m_blank;

    protected bool m_init_from_rowcol;

    protected int m_r;

    protected int m_nr;

    protected int m_c;

    protected int m_nc;

    private int m_Width;

    private int m_Height;

    private bool m_IsRange;

    protected string m_SheetName;

    protected int m_SheetIndex = -1;

    protected string m_Dimension;

    private string m_A1ref;

    private string m_R1C1ref;

    public int Left => m_Left;

    public int Top => m_Top;

    public int Right => m_Right;

    public int Bottom => m_Bottom;

    public object Fill
    {
        get
        {
            return m_Fill;
        }
        set
        {
            m_Fill = ((value == null) ? new NumVal(NumVal.NaN) : value);
        }
    }

    public int BlankRows => m_blank;

    public int Width
    {
        get
        {
            return m_Width;
        }
        set
        {
            m_Width = value;
        }
    }

    public int Height
    {
        get
        {
            return m_Height;
        }
        set
        {
            m_Height = value;
        }
    }

    public bool IsRange
    {
        get
        {
            return m_IsRange;
        }
        set
        {
            m_IsRange = value;
        }
    }

    public string SheetName => m_SheetName;

    public bool HasSheetName => m_SheetName != null;

    public bool HasSheetIndex => m_SheetIndex >= 0;

    public int SheetIndex => m_SheetIndex;

    public bool HasDimension => m_Dimension != null;

    public string A1ref
    {
        get
        {
            if (m_A1ref == null)
            {
                string text = MakeA1Reference(m_Top, m_Left);
                string label = MakeA1Reference(m_Bottom, m_Right);
                m_A1ref = ((text == label) ? text : string.Format("{0}:{1}", text, label));
            }
            return m_A1ref;
        }
    }

    public string R1C1ref
    {
        get
        {
            if (m_R1C1ref == null)
            {
                string text = MakeR1C1Reference(m_Top, m_Left);
                string label = MakeR1C1Reference(m_Bottom, m_Right);
                m_R1C1ref = ((text == label) ? text : string.Format("{0}:{1}", text, label));
            }
            return m_R1C1ref;
        }
    }

    public Descriptor(string descriptor)
        : this(descriptor, 2u)
    {
    }

    public Descriptor(string descriptor, uint sheetarg)
    {
        m_init_from_rowcol = false;
        m_r = 0;
        m_c = 0;
        m_nr = 0;
        m_nc = 0;
        m_Fill = new NumVal(NumVal.NaN);
        if (string.IsNullOrEmpty(descriptor))
        {
            return;
        }
        int num = descriptor.IndexOf('[');
        if (num >= 0)
        {
            int value = descriptor.IndexOf(']');
            if (value >= 0)
            {
                string s = descriptor.Substring(num + 1, value - num - 1);
                m_SheetIndex = int.Parse(s);
                if (m_SheetIndex > 0)
                {
                    m_SheetIndex--;
                }
                else
                {
                    AppException.Raise(string.Format("bad sheet index", default(ReadOnlySpan<object>)), sheetarg);
                }
                descriptor = descriptor.Substring(value + 1);
            }
            else
            {
                AppException.Raise(string.Format("bad sheet index", default(ReadOnlySpan<object>)), sheetarg);
            }
        }
        else
        {
            int otherValue = descriptor.LastIndexOf('!');
            if (otherValue >= 0)
            {
                m_SheetName = descriptor.Substring(0, otherValue);
                descriptor = descriptor.Substring(otherValue + 1);
            }
            else
            {
                m_SheetName = null;
            }
        }
        Dimension = descriptor;
    }

    private int ParseBlankParameter(object blank)
    {
        if (blank == null)
        {
            return 2;
        }
        string text = blank as string;
        if (text == null)
        {
            AppException.Raise("must be string", 4u);
        }
        if (text.Equals("skip"))
        {
            return 1;
        }
        if (text.Equals("read"))
        {
            return 2;
        }
        if (text.StartsWith("stop"))
        {
            return 3;
        }
        AppException.Raise("must be \"skip\", \"read\", or \"stop reading\"", 4u);
        return 0;
    }

    public int SetBlank(object blank)
    {
        m_blank = ParseBlankParameter(blank);
        return m_blank;
    }

    public Descriptor(string descriptor, object fill, object blank)
        : this(descriptor)
    {
        Fill = fill;
        SetBlank(blank);
    }

    public Descriptor(string descriptor, object fill)
        : this(descriptor)
    {
        Fill = fill;
    }

    public Descriptor(string descriptor, int matrix_rows, int matrix_cols, object fill)
        : this(descriptor, matrix_rows, matrix_cols)
    {
        Fill = fill;
    }

    public Descriptor(int r, int c, int matrix_rows, int matrix_cols, object fill)
        : this(r, c, matrix_rows, matrix_cols)
    {
        Fill = fill;
    }

    public Descriptor(int r, int c, int matrix_rows, int matrix_cols, object fill, object blankrows)
        : this(r, c, matrix_rows, matrix_cols)
    {
        Fill = fill;
        SetBlank(blankrows);
    }

    public static string MakeA1Reference(int row, int col)
    {
        return string.Format("{0}{1}", ExcelSupport.intToCol(col), row);
    }

    public static string MakeR1C1Reference(int row, int col)
    {
        return string.Format("R{0}C{1}", row, col);
    }
}
