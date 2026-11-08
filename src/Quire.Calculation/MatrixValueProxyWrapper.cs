#nullable disable
using System.Xml;
using Quire.Automation;
using Quire.Calculation.Core;
using Quire.Mirror;

namespace Quire.Calculation;

internal partial class MatrixValueProxyWrapper : IMatrixValueProxyWrapper
{
    private static readonly PlainDataConversionOptions _conversionOptions;

    private IMatrixValueProxy Proxy { get; set; }

    public bool IsUniform => Proxy.IsUniform;

    public uint Rows => Proxy.Rows;

    public uint Cols => Proxy.Cols;

    public string Xml => Proxy.Xml;

    public MatrixValueProxyWrapper(IMatrixValueProxy proxy)
    {
        Proxy = proxy;
    }

    public IValueProxyWrapper Get(uint rowIndex, uint columnIndex)
    {
        return new ValueProxyWrapper(Proxy.Get(rowIndex, columnIndex));
    }

    public void WriteXml(XmlWriter writer)
    {
        Proxy.WriteXml(writer);
    }

    public IMatrixValueDataAccessorWrapper GetPlainData()
    {
        return new MatrixValueDataAccessorWrapper(Proxy.GetPlainData(_conversionOptions));
    }

    [MirrorVerified("PtcCalculationMatrixValueProxyWrapperCctorMechanicalDifferentialTests")]
    static MatrixValueProxyWrapper()
    {
        var options = new PlainDataConversionOptions { ImaginaryThreshold = double.NaN };
        _conversionOptions = options;
    }
}
