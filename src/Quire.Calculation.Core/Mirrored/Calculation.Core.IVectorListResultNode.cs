#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IVectorListResultNode
{
    global::System.Collections.Generic.IList<global::Quire.Calculation.Core.IListResultNode> Vectors { get; }
    global::Quire.Calculation.Core.ResultValueType CommonValuesType { get; }
    int Count { get; }
    int ElementsCount { get; }
    int SingleVectorLength { get; }
    global::Quire.Calculation.Core.VectorsType VectorsType { get; }
    global::Quire.Calculation.Core.IListResultNode this[int index] { get; }
    global::Quire.Calculation.Core.ResultValueType GetVectorValuesType(int index);
}
