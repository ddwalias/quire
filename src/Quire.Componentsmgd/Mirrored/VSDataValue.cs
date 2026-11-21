using System;
using System.Runtime.InteropServices;
using Quire.Automation;
using Quire.Mcd;
using Quire.VSINTERFACESLib;

namespace Quire.Components;

public partial class VSDataValue : MatrixValue, IVSDataType, IVSUnitData, IPersistStream, IPersist, IVSPersistStream
{
    private static readonly byte[] MatrixMarker = new byte[7] { 255, 255, 255, 255, 255, 255, 255 };
    private GCHandle handleR;
    private GCHandle handleI;

    public bool IsScalar
    {
        get
        {
            if (Matrix.Rows == 1)
            {
                return Matrix.Cols == 1;
            }
            return false;
        }
    }

    public VSDATATYPE DataType
    {
        get
        {
            if (IsScalar && IsString(0u, 0u))
            {
                return VSDATATYPE.VSDATATYPE_STRING;
            }
            if (IsComplex)
            {
                return VSDATATYPE.VSDATATYPE_COMPLEX;
            }
            return VSDATATYPE.VSDATATYPE_REAL;
        }
    }

    public uint Dimensions
    {
        get
        {
            if (!IsScalar)
            {
                return 2u;
            }
            return 1u;
        }
    }

    public VSDATABOUNDS Bounds
    {
        get
        {
            return new VSDATABOUNDS
            {
                Rows = Matrix.Rows,
                Cols = Matrix.Cols
            };
        }
        set
        {
            if (value.Rows < Rows || value.Cols < Cols)
            {
                MatVal matVal = Matrix;
                if (Rows > value.Rows || Cols > value.Cols)
                {
                    matVal = matVal.SubMatrix(0u, (uint)Math.Min(Rows, value.Rows), 0u, (uint)Math.Min(Cols, value.Cols));
                }
                core = new MatVal(matVal);
            }
            else if (value.Rows > Rows || value.Cols > Cols)
            {
                core = Matrix.mcd_set_2(value.Rows - 1, value.Cols - 1, new NumVal(0));
            }
        }
    }

    public virtual bool IsDimensionless => Dim.Id_b(Dim);

    public Guid Clsid
    {
        get
        {
            if (IsComplex)
            {
                return new Guid("{6DB86EC5-0A1A-11E0-B0C0-11F8E2A8C36C}");
            }
            return new Guid("{6DA95394-0A1A-11E0-8D8A-11F8E2A8C36C}");
        }
    }

    public int IsDirty
    {
        get
        {
            if (!Matrix.Immutable)
            {
                return 0;
            }
            return -1;
        }
    }

    public long MaxSize
    {
        get
        {
            long num = 36 + Matrix.Rows * Matrix.Cols * 8;
            if (Matrix.HasPtrs)
            {
                foreach (int item in Matrix.m_parr)
                {
                    int i = Matrix.RowOf(item);
                    int j = Matrix.ColOf(item);
                    object obj = Matrix.get_2(i, j);
                    if (obj is string text)
                    {
                        num += text.Length * 2;
                    }
                    else
                    {
                        num += ((IPersistStream)obj).MaxSize + 7;
                    }
                    num += 7;
                }
            }
            return num;
        }
    }

    public VSDataValue()
    {
    }

    public VSDataValue(MatVal mat)
        : base(mat)
    {
    }

    private void SetPowers(double[] powers)
    {
        DimVal dimVal = (DimVal)core;
        int[] global = RuntimeOptions.the_unit_system.ChangeBasis(powers);
        Mcd.Type.Product dimensions = RuntimeOptions.the_unit_system.GetDimensions(global);
        dimVal.m_dim = Dim.New(dimensions);
    }

    private double[] GetPowers()
    {
        int[] global = ((Dim == null) ? new int[RuntimeOptions.the_unit_system.Rank] : Dim.Coordinates);
        return RuntimeOptions.the_unit_system.ChangeBasis(global);
    }

    public void InitNew(uint rows, uint cols, bool bImag)
    {
        core = new MatVal((int)rows, (int)cols, bImag);
    }

    public IVSDataType Clone()
    {
        return new VSDataValue(new MatVal(Matrix));
    }

    public int Lock(bool writeLock)
    {
        lock (this)
        {
            if (handleR.IsAllocated || Matrix.Rows == 0 || Matrix.Cols == 0)
            {
                throw HRESULT.S_FALSE;
            }
            if (!Matrix.Packed || (writeLock && Matrix.Immutable))
            {
                core = new MatVal(Matrix);
            }
            handleR = GCHandle.Alloc(Matrix.m_rarr, GCHandleType.Pinned);
            return ((IntPtr)handleR.AddrOfPinnedObject()).ToInt32();
        }
    }

    public int LockI(bool writeLock)
    {
        lock (this)
        {
            if (handleI.IsAllocated || !IsComplex || Matrix.Rows == 0 || Matrix.Cols == 0)
            {
                throw HRESULT.S_FALSE;
            }
            if (!Matrix.Packed || (writeLock && Matrix.Immutable))
            {
                core = new MatVal(Matrix);
            }
            handleI = GCHandle.Alloc(Matrix.m_iarr, GCHandleType.Pinned);
            return ((IntPtr)handleI.AddrOfPinnedObject()).ToInt32();
        }
    }

    public void Unlock()
    {
        lock (this)
        {
            handleR.Free();
        }
    }

    public void UnlockI()
    {
        lock (this)
        {
            handleI.Free();
        }
    }

    public bool IsString(uint row, uint col)
    {
        return Matrix.mcd_get_2((int)row, (int)col) is string;
    }

    public bool IsMatrix(uint row, uint col)
    {
        return Matrix.mcd_get_2((int)row, (int)col) is MatVal;
    }

    public bool IsObject(uint row, uint col)
    {
        return !(Matrix.mcd_get_2((int)row, (int)col) is NumVal);
    }

    public string GetStringValue(uint row, uint col)
    {
        return ((DimVal)Matrix.mcd_get_2((int)row, (int)col)).IsString;
    }

    public void SetStringValue(uint row, uint col, string str)
    {
        core = Matrix.mcd_set_2((int)row, (int)col, new StrVal(str));
    }

    public void SetUnitSystem(int unit_sys)
    {
        if (unit_sys != 4)
        {
            throw new InvalidOperationException("deprecated method");
        }
    }

    public void GetUnitSystem(ref int unit_sys)
    {
        unit_sys = 4;
    }

    public void GetNumBaseUnits(ref int res)
    {
        res = RuntimeOptions.the_unit_system.Rank;
    }

    public void SetUnitValue(int index, double val)
    {
        if (index >= RuntimeOptions.the_unit_system.Rank || index < 0)
        {
            throw new ArgumentException();
        }
        double[] powers = GetPowers();
        powers[index] = val;
        SetPowers(powers);
    }

    public void GetUnitValue(int index, ref double result)
    {
        if (index >= RuntimeOptions.the_unit_system.Rank || index < 0)
        {
            throw new ArgumentException();
        }
        result = GetPowers()[index];
    }

    public void GetUnitName(int index, ref string result)
    {
        switch (index)
        {
        case 0:
            result = "Mass";
            break;
        case 1:
            result = "Length";
            break;
        case 2:
            result = "Time";
            break;
        case 3:
            result = "Current";
            break;
        case 4:
            result = "Temperature";
            break;
        case 5:
            result = "Luminosity";
            break;
        case 6:
            result = "Substance";
            break;
        case 7:
            result = "Money";
            break;
        default:
            throw new ArgumentException();
        }
    }
}
