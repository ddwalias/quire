using System;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.ExcelComponent.View;

namespace Quire.Controls.ExcelComponent.Model;

public partial class ChartComponentModel
{
    private ChartContainer ChartContainerItem => (ChartContainer)OleContainerItem;

    internal void CommitPost()
    {
        _chartMinMaxCalc.Commit();
    }

    protected override void FireCalculatableAdded(ICalculatable calculatable)
    {
        IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = calculatable as IChartComponentCalculatableWrapper;
        if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Failed)
        {
            return;
        }
        if (DebugTools.DevAssert(chartComponentCalculatableWrapper.ChartWrapperType.IsUnknownType(), null).Failed)
        {
            chartComponentCalculatableWrapper.ResetChartWrapperType();
        }
    }

    protected override void FireCalculatableRemoved(ICalculatable calculatable)
    {
        IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = calculatable as IChartComponentCalculatableWrapper;
        if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Failed)
        {
            return;
        }
        if (!IsDisposed)
        {
            ChartWrapperType value = chartComponentCalculatableWrapper.ResetChartWrapperType();
            HandleChildWrapperRemovedOrChartTypeChanged(chartComponentCalculatableWrapper, value);
        }
    }

    public override string Rl
    {
        get
        {
            ComponentNodeHandle = 1u;
            return string.Format("(eval (named-component:0x{0:x8} {1} {2} %@ 0))", ComponentNodeHandle, ChartTraceCalculatable.ChartVariableWithLabel, ComponentId);
        }
    }

    protected override void FireCalculatableMoved(ICalculatable calculatable)
    {
        IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = calculatable as IChartComponentCalculatableWrapper;
        if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Failed)
        {
            return;
        }
        if (chartComponentCalculatableWrapper.IsLimitExpression())
        {
            UpdateChartMinMaxCalculatable();
        }
        if (chartComponentCalculatableWrapper.IsTraceOrCommonExpression())
        {
            UpdateAllTraces(null);
        }
        base.FireCalculatableMoved(calculatable);
    }

    protected override void SetErrorByNodeHandle(EngineError error)
    {
        DebugTools.FailAssertion(error.NodeHandle.ToString());
        SetErrorFallback(error);
    }

    protected override void SetChildError(EngineError error)
    {
        if (error != null && error.ErrorSource is IChartMinMaxCommon)
        {
            _chartMinMaxCalc.ProcessComponentError(error);
            error = null;
        }
        base.SetChildError(error);
    }

    protected override void UpdateCustomErrorBasedOnPlacement(IWhiteboardItem item, bool placedAfterOleContainer)
    {
        ICalculatable calculatable = item as ICalculatable;
        if (DebugTools.AssertNotNull(calculatable, null).Failed)
        {
            return;
        }
        DebugTools.DevAssert(!GetWrapper(calculatable).IsOutputEquation && !placedAfterOleContainer, null);
    }

    protected override ICalculatable LookupCalculatableWrapper(ICalculatable calc)
    {
        if (calc is IChartCalculatable)
        {
            return calc;
        }
        return base.LookupCalculatableWrapper(calc);
    }

    protected override ICalculatable CreateCalculatableWrapper(ICalculatable calc, bool arg2)
    {
        if (calc is IChartCalculatable)
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            return calc;
        }
        if (CalculatableWrapperMap.ContainsKey(calc))
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            return CalculatableWrapperMap[calc];
        }
        ChartComponentCalculatableWrapper chartComponentCalculatableWrapper = new ChartComponentCalculatableWrapper(calc);
        CalculatableWrapperMap[calc] = chartComponentCalculatableWrapper;
        chartComponentCalculatableWrapper.ChartTypeChanged += HandleChildWrapperChartTypeChanged;
        chartComponentCalculatableWrapper.ErrorChanged += HandleChildWrapperErrorChanged;
        return chartComponentCalculatableWrapper;
    }

    protected override void RemoveCalculatableWrapper(ICalculatable calc)
    {
        if (calc is IChartCalculatable)
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            return;
        }
        IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = GetWrapper(calc) as IChartComponentCalculatableWrapper;
        if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Passed)
        {
            chartComponentCalculatableWrapper.ChartTypeChanged -= HandleChildWrapperChartTypeChanged;
            chartComponentCalculatableWrapper.ErrorChanged -= HandleChildWrapperErrorChanged;
            chartComponentCalculatableWrapper.Dispose();
        }
        CalculatableWrapperMap.Remove(calc);
        UpdateErrorAccordingToChildren();
    }

    public override object Clone()
    {
        ChartComponentModel chartComponentModel = new ChartComponentModel();
        CopyStateTo(chartComponentModel);
        return chartComponentModel;
    }

    private void HandleChildWrapperChartTypeChanged(IChartComponentCalculatableWrapper wrapper)
    {
        HandleChildWrapperRemovedOrChartTypeChanged(wrapper, null);
    }

    private void HandleChildWrapperRemovedOrChartTypeChanged(IChartComponentCalculatableWrapper wrapper, ChartWrapperType? prevTypeWhenRemoving)
    {
        ChartWrapperType type = prevTypeWhenRemoving ?? wrapper.PreviousChartType;
        ChartWrapperType chartWrapperType = wrapper.ChartWrapperType;
        if (chartWrapperType.IsUnknownType() && !prevTypeWhenRemoving.HasValue)
        {
            return;
        }
        DebugTools.DevAssert(!prevTypeWhenRemoving.HasValue || (wrapper.ChartWrapperType == ChartWrapperType.Unknown && wrapper.PreviousChartType == ChartWrapperType.Unknown), null);
        if (type.IsLimitType() || chartWrapperType.IsLimitType())
        {
            if (wrapper.IsConnectedToTheEngine && !chartWrapperType.IsLimitType())
            {
                base.FireCalculatableRemoved(wrapper);
            }
            UpdateChartMinMaxCalculatable();
        }
        if (type.IsTraceOrCommonType() || chartWrapperType.IsTraceOrCommonType())
        {
            UpdateAllTraces(wrapper);
        }
        if (RequestUpdateIdentifierLabels != null)
        {
            RequestUpdateIdentifierLabels();
        }
    }

    private void InternalAddCalculatableAndEvaluate(IOleBaseComponentCalculatableChild calc)
    {
        base.FireCalculatableAdded(calc);
        if (!(calc is IChartComponentCalculatableWrapper chartComponentCalculatableWrapper))
        {
            calc.Commit();
        }
        else
        {
            chartComponentCalculatableWrapper.HandleRequestEvaluationAux();
        }
    }
}
