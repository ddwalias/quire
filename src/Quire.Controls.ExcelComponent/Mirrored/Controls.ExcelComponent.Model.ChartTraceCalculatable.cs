using System.Collections.Generic;
using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class ChartTraceCalculatable
{
    public IChartComponentCalculatableWrapper XCalculatable
    {
        get
        {
            return _xCalculatable;
        }
        set
        {
            if (_xCalculatable == value)
            {
                return;
            }
            UnHookupChild(_xCalculatable);
            _xCalculatable = value;
            HookupChild(_xCalculatable);
        }
    }

    public IChartComponentCalculatableWrapper YCalculatable
    {
        get
        {
            return _yCalculatable;
        }
        set
        {
            if (_yCalculatable == value)
            {
                return;
            }
            UnHookupChild(_yCalculatable);
            _yCalculatable = value;
            HookupChild(_yCalculatable);
        }
    }

    public SynchronizationStatus SyncStatus
    {
        get
        {
            return _syncStatus;
        }
        private set
        {
            if (_syncStatus == value)
            {
                return;
            }
            _syncStatus = value;
            UpdateChildren(ChartTraceDataToUpdate.SynchronizationStatus);
        }
    }

    public CalculatingStatus CalcStatus
    {
        get
        {
            return _calcStatus;
        }
        private set
        {
            if (_calcStatus == value)
            {
                return;
            }
            _calcStatus = value;
            UpdateChildren(ChartTraceDataToUpdate.CalculatingStatus);
        }
    }

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
