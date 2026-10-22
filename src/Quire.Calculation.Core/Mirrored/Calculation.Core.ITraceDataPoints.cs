#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ITraceDataPoints
{
    global::Quire.Calculation.Core.IVectorListResultNode RangePoints { get; }
    global::Quire.Calculation.Core.IVectorListResultNode ResultPoints { get; }
    bool IsEmpty { get; }
}
