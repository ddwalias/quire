using System;
using System.Collections.Generic;
using System.ComponentModel;
using Quire.Calculation.Core;
using Quire.Controls.Core;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class ChartComponentCalculatableWrapper
{
    private const uint LhsTag = 65535u;

    private BaseComponentEquationType ComponentEquationType { get; set; }

    public void HandleRequestEvaluationAux()
    {
        base.HandleRequestEvaluation(WrappedCalculatable);
    }

    protected override void ClearEquationType()
    {
        ComponentEquationType = BaseComponentEquationType.Unknown;
        ChartWrapperType = ChartWrapperType.Unknown;
    }

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

    private readonly List<IChartTraceCalculatable> _traceCalcs = new List<IChartTraceCalculatable>();

    private string _identifier;

    private uint? _identifierHandle;

    private ChartWrapperType _chartType;

    public ChartWrapperType ChartWrapperType
    {
        get
        {
            return _chartType;
        }
        private set
        {
            if (_chartType == value)
            {
                return;
            }
            if (!_chartType.IsUnknownType())
            {
                PreviousChartType = CustomErrorType == OleWrapperCustomErrorType.Placement ? ChartWrapperType.Invalid : _chartType;
            }
            _chartType = value;
            if (_chartType.IsValidOrUnknownType())
            {
                ClearAllErrors();
            }
            if (ChartTypeChanged != null)
            {
                ChartTypeChanged(this);
            }
            if (!_chartType.IsUnknownType())
            {
                PreviousChartType = ChartWrapperType.Unknown;
            }
            DebugTools.DevAssert(_chartType == value, null);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override EngineResult EngineResult
    {
        get
        {
            return base.EngineResult;
        }
        set
        {
            string text = null;
            if (value.Errors.IsNotEmpty())
            {
                DebugTools.DevAssert(value.Errors.Count == 1, null);
                if (value.SerializationTag != null)
                {
                    DebugTools.DevAssert(ChartWrapperType.IsTraceOrCommonType(), null);
                    text = value.SerializationTag;
                    value.SerializationTag = null;
                }
            }
            base.EngineResult = value;
            if (text != null)
            {
                List<uint> list = new List<uint>();
                string[] array = text.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    if (DebugTools.DevAssert(uint.TryParse(array[i], out var result), null).Passed)
                    {
                        list.Add(result);
                    }
                }
                if (UpdateTraceWithDeserializedError != null)
                {
                    UpdateTraceWithDeserializedError(this, list);
                }
            }
        }
    }

    public string RlIdWithLabel => GetRlIdWithLabel(ChartWrapperType);

    public event Action<IChartComponentCalculatableWrapper> ChartTypeChanged;

    public event Action<IChartComponentCalculatableWrapper, List<uint>> UpdateTraceWithDeserializedError;

    public override Pair<bool, bool> IsFragment => new Pair<bool, bool>
    {
        First = ChartWrapperType.IsUnknownType() ? PreviousChartType.IsTraceOrCommonType() : this.IsTraceOrCommonExpression(),
        Second = !ChartWrapperType.IsUnknownType()
    };

    public override string Rl
    {
        get
        {
            string text = ConstructRl(ChartWrapperType, RhsExpression);
            if (text.IsNotEmpty())
            {
                return text;
            }
            return "-PLACEHOLDER";
        }
    }

    public ChartComponentCalculatableWrapper(ICalculatable wrappedCalculatable)
        : base(wrappedCalculatable)
    {
        if (wrappedCalculatable is IEquationControl equationControl)
        {
            equationControl.OverrideIdLabelByResolvedForDefinitionLhs = true;
        }
    }

    public void UpdateIdentifierLabelsIfNeeded()
    {
        IEquationControl equationControl = WrappedCalculatable as IEquationControl;
        if (_chartType.IsValidType() && IsInputEquation && equationControl != null)
        {
            if (DebugTools.DevAssert(equationControl.OverrideIdLabelByResolvedForDefinitionLhs, null).Failed)
            {
                equationControl.OverrideIdLabelByResolvedForDefinitionLhs = true;
            }
            if (_identifierHandle.HasValue)
            {
                SetIdentifierLabelsAux(new Dictionary<ResolvedId, string[]>
                {
                    {
                        new ResolvedId
                        {
                            Calculatable = WrappedCalculatable,
                            Tag = (int)_identifierHandle.Value,
                            Name = _identifier
                        },
                        new string[1] { IdLabelToStringConverter.Convert(IdLabels.Keyword, null) }
                    }
                });
            }
        }
    }

    public ChartWrapperType ResetChartWrapperType()
    {
        ChartWrapperType = ChartWrapperType.Unknown;
        ChartWrapperType previousChartType = PreviousChartType;
        PreviousChartType = ChartWrapperType.Unknown;
        return previousChartType;
    }

    protected override OleWrapperCustomErrorType GetCustomErrorType(string errorCode)
    {
        OleWrapperCustomErrorType customErrorType = base.GetCustomErrorType(errorCode);
        if (customErrorType != OleWrapperCustomErrorType.None)
        {
            return customErrorType;
        }
        switch (errorCode)
        {
            case "chart_invalid_identifier":
            case "chart_invalid_index":
                return OleWrapperCustomErrorType.Formatting;
            case "chart_duplicate_trace_expression":
            case "chart_duplicate_limit_expression":
                return OleWrapperCustomErrorType.Placement;
            default:
                if (!ChartMinMaxCalculatable.IsLimitsCalculationRelatedCustomError(errorCode))
                {
                    return OleWrapperCustomErrorType.None;
                }
                return OleWrapperCustomErrorType.Calculation;
        }
    }

    private void SetIdentifierLabelsAux(IDictionary<ResolvedId, string[]> idLabelMap)
    {
        base.SetIdentifierLabels(idLabelMap);
    }

    internal bool TraceAffectsCalcData(IChartTraceCalculatable trace)
    {
        if (!trace.ErrorOwners.Contains(this) && trace.SyncStatus == SynchronizationStatus.Synchronized)
        {
            return trace.CalcStatus != CalculatingStatus.Idle;
        }
        return true;
    }
}
