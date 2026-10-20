#nullable disable
using System;
using System.Collections.Generic;
using Quire;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public static partial class TraceTypeDefinitions
{
    private static readonly Dictionary<TraceType, TraceTypeRange> _traceColumnsRanges;

    public const TraceType AutoTypeForMultipleColumns = TraceType.Waterfall;
    public const TraceType AutoTypeForSingleColumn = TraceType.Lines;
    public const TraceType ReadAsRowsType = TraceType.Boxes;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceTypeDefinitionsCctorMechanicalDifferentialTests")]
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

    public static int GetMaxRangeColumns(TraceType type)
    {
        if (type != TraceType.Waterfall)
        {
            return 1;
        }
        return int.MaxValue;
    }

    public static int GetMinResultColumns(TraceType type)
    {
        int result = -1;
        if (_traceColumnsRanges.ContainsKey(type))
        {
            result = _traceColumnsRanges[type].Start;
        }
        else
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
        }
        return result;
    }

    public static int GetMaxResultColumns(TraceType type)
    {
        int result = -1;
        if (_traceColumnsRanges.ContainsKey(type))
        {
            result = _traceColumnsRanges[type].End;
        }
        else
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
        }
        return result;
    }
}
