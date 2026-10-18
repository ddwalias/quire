#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public static partial class TraceTypeDefinitions
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceTypeDefinitionsCctorDifferentialTests")]
    static TraceTypeDefinitions()
    {
        _traceColumnsRanges = new Dictionary<TraceType, TraceTypeRange>();
        _traceColumnsRanges.Add(TraceType.Bars, new TraceTypeRange { Start = 1, End = 1 });
        _traceColumnsRanges.Add(TraceType.Boxes, new TraceTypeRange { Start = 5, End = int.MaxValue });
        _traceColumnsRanges.Add(TraceType.Columns, new TraceTypeRange { Start = 1, End = 1 });
        _traceColumnsRanges.Add(TraceType.Effects, new TraceTypeRange { Start = 2, End = 2 });
        _traceColumnsRanges.Add(TraceType.Errors, new TraceTypeRange { Start = 2, End = 2 });
        _traceColumnsRanges.Add(TraceType.Lines, new TraceTypeRange { Start = 1, End = 1 });
        _traceColumnsRanges.Add(TraceType.Stems, new TraceTypeRange { Start = 1, End = 1 });
        _traceColumnsRanges.Add(TraceType.Waterfall, new TraceTypeRange { Start = 1, End = int.MaxValue });
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceTypeDefinitionsMechanicalDifferentialTests")]
    public static int GetMinRangeColumns(TraceType type) => 1;
}
