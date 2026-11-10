using Quire.Calculation.Core;
using Quire.Controls.Core;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class ChartComponentCalculatableWrapper
{
    private BaseComponentEquationType ComponentEquationType { get; set; }

    public static string ConstructRl(ChartWrapperType type, string rhsExpression)
    {
        string rlIdWithLabel = GetRlIdWithLabel(type);
        if (rlIdWithLabel == null)
        {
            return null;
        }
        return string.Format("(define {0}:0x{1:x8} {2})", rlIdWithLabel, LhsTag, rhsExpression);
    }

    public static string ConstructAxisExpression(ChartWrapperType chartType, uint? index)
    {
        bool hasValue = index.HasValue;
        if (DebugTools.DevAssert(ChartComponentConstants.TypeToIdentifier.TryGetValue(chartType, out var value) || hasValue == value.Second, null).Failed)
        {
            return null;
        }
        string text = ChartTypeToIdentifier(chartType);
        if (DebugTools.AssertNotNull(text, null).Failed)
        {
            return null;
        }
        if (!hasValue)
        {
            return string.Format("(:= (@LABEL VARIABLE {0}) @PLACEHOLDER)", text);
        }
        return string.Format("(:= (@INDEX (@LABEL VARIABLE {0}) {1}) @PLACEHOLDER)", text, index);
    }

    public bool HasAssociatedTraces()
    {
        return _traceCalcs.Count != 0;
    }

    public void AddAssociatedTrace(IChartTraceCalculatable trace)
    {
        if (DebugTools.DevAssert(!_traceCalcs.Contains(trace), null).Failed)
        {
            return;
        }
        DebugTools.DevAssert(!IsConnectedToTheEngine, null);
        _traceCalcs.Add(trace);
        if (TraceAffectsCalcData(trace))
        {
            UpdateAssociatedTraceData(ChartTraceDataToUpdate.All);
        }
    }

    public void RemoveAssociatedTrace(IChartTraceCalculatable trace)
    {
        if (DebugTools.DevAssert(_traceCalcs.Contains(trace), null).Failed)
        {
            return;
        }
        _traceCalcs.Remove(trace);
        if (TraceAffectsCalcData(trace))
        {
            UpdateAssociatedTraceData(ChartTraceDataToUpdate.All);
        }
    }

    internal static string ChartTypeToIdentifier(ChartWrapperType chartType)
    {
        if (!ChartComponentConstants.TypeToIdentifier.TryGetValue(chartType, out var value))
        {
            return null;
        }
        return value.First;
    }

    private static string GetRlIdWithLabel(ChartWrapperType type)
    {
        string text = ChartTypeToIdentifier(type);
        if (text == null)
        {
            return null;
        }
        return "ui_chart_" + text.ToLower() + IdLabelsExtensions.EmptyLabelRl;
    }

    public global::System.Nullable<uint> TraceId { get; private set; }

    public ChartWrapperType PreviousChartType { get; private set; }

    public override bool IsInputEquation => ComponentEquationType == BaseComponentEquationType.Input;

    public override bool IsUnknownEquation => ComponentEquationType == BaseComponentEquationType.Unknown;

    public override bool IsOutputEquation => ComponentEquationType == BaseComponentEquationType.Output;

    public override bool IsInvalidEquation => ComponentEquationType == BaseComponentEquationType.Invalid;
}
