#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core.Trace2DResults;

partial interface ITrace2DResultOwner
{
    bool IsTypeDefault { get; }
    global::Quire.Calculation.Core.Trace2DResults.TraceType Type { get; }
}
