using System.Collections.Generic;
using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class ChartTraceCalculatable
{
    public EngineError Error => CurrentError;

    public static int TraceIdForChartApp(uint traceId)
    {
        return (int)traceId - 1;
    }

    public void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        SyncStatus = syncStatus;
    }

    public void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
        CalcStatus = computingStatus;
    }

    public void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap)
    {
    }
}
