#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core.Trace2DResults;

partial interface ITrace2DConverterHelper
{
    bool IsTypeDefault { get; }
    global::Quire.Calculation.Core.Trace2DResults.TraceType CurrentType { get; }
    int MaxNumberOf2DPoints { get; }
    bool RangeUnitsAllowed { get; }
    bool DiscreteValuesAllowed { get; }
    string AssertValuesTypeMatchTraceType(bool xIsDiscrete, bool yIsDiscrete, global::Quire.Calculation.Core.Trace2DResults.TraceType traceTypePreset);
    string ValidateColumnsAndUnits(int numberOfPointsInResult, int yResultColumns, global::System.Collections.Generic.IList<global::Quire.Calculation.Core.Unit> rangeUnits, global::Quire.Calculation.Core.Trace2DResults.TraceType traceType);
    string ValidateNumberOfPointsLimit(int numberOfPointsInResult, global::Quire.Calculation.Core.Trace2DResults.TraceType traceType);
}
