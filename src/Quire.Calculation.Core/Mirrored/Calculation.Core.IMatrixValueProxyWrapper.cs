#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IMatrixValueProxyWrapper
{
    bool IsUniform { get; }
    uint Rows { get; }
    uint Cols { get; }
    string Xml { get; }
    global::Quire.Calculation.Core.IValueProxyWrapper Get(uint rowIndex, uint columnIndex);
    void WriteXml(global::System.Xml.XmlWriter writer);
    global::Quire.Calculation.Core.IMatrixValueDataAccessorWrapper GetPlainData();
}
