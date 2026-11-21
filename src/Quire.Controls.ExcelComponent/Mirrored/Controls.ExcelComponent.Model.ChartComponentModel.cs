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
