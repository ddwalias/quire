#nullable disable
#pragma warning disable
using System.Runtime.InteropServices;

namespace Quire.ChartAppLib;

public partial struct TraceData
{
    public ChartAppType m_type;
    public ChartMinMax m_minMax;
    public int m_arrXLength;
    public int m_arrYLength;
    public int m_arrZLength;
    public int m_sectionLength;
    [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_R8)]
    public double[] m_X;
    [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_R8)]
    public double[] m_Y;
    [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_R8)]
    public double[] m_Z;
    public int m_uniqueId;
    [MarshalAs(UnmanagedType.BStr)]
    public string m_xUnits;
    [MarshalAs(UnmanagedType.BStr)]
    public string m_yUnits;
    [MarshalAs(UnmanagedType.BStr)]
    public string m_zUnits;
}
