#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core.Trace2DResults;

partial interface IMatrixToVectorsConverter
{
    bool ReadAsColumns { get; }
    global::System.Collections.Generic.IList<global::Quire.Calculation.Core.IListResultNode> ReadVectors(int maxCount);
    global::Quire.Calculation.Core.ResultValueType GetCommonType();
}
