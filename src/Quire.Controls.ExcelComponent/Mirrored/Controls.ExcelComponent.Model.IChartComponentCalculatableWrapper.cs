#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;

namespace Quire.Controls.ExcelComponent.Model;

internal partial interface IChartComponentCalculatableWrapper
{
    ChartWrapperType ChartWrapperType { get; }
    ChartWrapperType PreviousChartType { get; }
    string RlIdWithLabel { get; }
    uint? TraceId { get; }
    event Action<IChartComponentCalculatableWrapper> ChartTypeChanged;
    event Action<IChartComponentCalculatableWrapper, List<uint>> UpdateTraceWithDeserializedError;
    void AddAssociatedTrace(IChartTraceCalculatable trace);
    bool HasAssociatedTraces();
    void RemoveAssociatedTrace(IChartTraceCalculatable trace);
    void UpdateAssociatedTraceData(ChartTraceDataToUpdate dataToUpdate);
    ChartWrapperType ResetChartWrapperType();
    void UpdateIdentifierLabelsIfNeeded();
    void HandleRequestEvaluationAux();
}
