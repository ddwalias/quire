using System;
using System.Collections.Generic;
using Quire.Calculation.Core;
using Quire.Controls.Core;

namespace Quire.Controls.ExcelComponent.Model;

internal partial class ChartTraceCalculatable
{
    public static int TraceIdForChartApp(uint traceId)
    {
        return (int)traceId - 1;
    }

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

    public uint TraceId
    {
        get
        {
            uint? traceId;
            switch (YCalculatable.ChartWrapperType)
            {
                case ChartWrapperType.YCommon:
                    traceId = XCalculatable.TraceId ?? 1;
                    break;
                case ChartWrapperType.YTrace:
                    traceId = YCalculatable.TraceId;
                    break;
                default:
                    traceId = null;
                    break;
            }
            if (!traceId.HasValue)
            {
                DebugTools.FailAssertion(XCalculatable.WrappedCalculatable.Rl + " (" + XCalculatable.ChartWrapperType.ToString() + "), " + YCalculatable.WrappedCalculatable.Rl + " (" + YCalculatable.ChartWrapperType.ToString() + ")");
                traceId = 1u;
            }
            return traceId.Value;
        }
    }

    public void Commit()
    {
        if (RequestEvaluation != null)
        {
            RequestEvaluation(this);
        }
    }

    public void SetStatusSynchronized()
    {
        if (TreatAsUpToDate != null)
        {
            TreatAsUpToDate(this);
        }
    }

    public void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        SyncStatus = syncStatus;
    }

    public void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
        CalcStatus = computingStatus;
    }

    public void SetResult(IResultNode result)
    {
        if (DebugTools.DevAssert(result is IEmptyResultNode, null).Failed)
        {
            return;
        }
        XCalculatable.SetResult(result);
        YCalculatable.SetResult(result);
    }

    public void SetError(EngineError error)
    {
        if (error != null)
        {
            if (_nodeHandleMapper.GetMappedEquation(error.NodeHandle) is IChartComponentCalculatableWrapper errorSource)
            {
                error.ErrorSource = errorSource;
                error.NodeHandle = _nodeHandleMapper.GetMappedNodeNandle(error.NodeHandle);
            }
            else if (error.NodeHandle == _xMinScaleHandle || error.NodeHandle == _xMaxScaleHandle)
            {
                IChartComponentCalculatableWrapper limit = _minMax.GetLimit((error.NodeHandle == _xMinScaleHandle) ? ChartWrapperType.XMin : ChartWrapperType.XMax);
                if (DebugTools.AssertNotNull(limit, null).Passed && DebugTools.AssertNotNull(limit.Error, null).Passed && (error.OriginalError == null || DebugTools.DevAssert(error.OriginalError.IsEquivalentTo(limit.Error), null).Failed))
                {
                    error.OriginalError = limit.Error;
                }
            }
            else if (error.ErrorSource == this)
            {
                DebugTools.DevAssert(error.NodeHandle <= _xyplotHandle, null);
                if (error.ErrorCode == "illegal_range")
                {
                    IChartComponentCalculatableWrapper limit = _minMax.GetLimit(ChartWrapperType.XMax) ?? _minMax.GetLimit(ChartWrapperType.XMin);
                    if (DebugTools.AssertNotNull(limit, null).Passed)
                    {
                        DebugTools.DevAssert(limit.Error == null, null);
                        limit.SetCustomError("chart_limits_too_close", error);
                    }
                }
                else
                {
                    error.NodeHandle = 0u;
                    error.Argument = 0u;
                }
            }
            else
            {
                DebugTools.FailAssertion(error.Message);
            }
        }
        CurrentError = error;
    }

    public void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap)
    {
    }

    public void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings)
    {
        if (warnings.IsNotEmpty())
        {
            throw new NotImplementedException();
        }
    }

    public Pair<bool, bool> IsFragment
    {
        get
        {
            if (DebugTools.DevAssert(XCalculatable != null && YCalculatable != null, null).Failed)
            {
                return new Pair<bool, bool>
                {
                    First = true,
                    Second = false
                };
            }
            Pair<bool, bool> xIsFragment = XCalculatable.IsFragment;
            Pair<bool, bool> yIsFragment = YCalculatable.IsFragment;
            DebugTools.DevAssert(xIsFragment.First && yIsFragment.First, null);
            return new Pair<bool, bool>
            {
                First = true,
                Second = xIsFragment.Second && yIsFragment.Second
            };
        }
    }

    public bool IsConnectedToTheEngine
    {
        get
        {
            return _isConnectedToTheEngine;
        }
        set
        {
            _isConnectedToTheEngine = value;
            DebugTools.DevAssert(!_isConnectedToTheEngine || (XCalculatable != null && YCalculatable != null), null);
        }
    }

    public EngineError Error => CurrentError;

    public bool ContainsError(EngineError error)
    {
        return CurrentError == error;
    }

    public EngineError CurrentError
    {
        get
        {
            return _currentError;
        }
        private set
        {
            if (value != null && _currentError == value)
            {
                return;
            }
            _currentError = value;
            DebugTools.DevAssert(_currentError == null || _currentError.ErrorSource == this || _currentError.ErrorSource is IChartComponentCalculatableWrapper, null);
            UpdateChildren(ChartTraceDataToUpdate.Error);
        }
    }

    public void Dispose()
    {
        XCalculatable = null;
        YCalculatable = null;
        DebugTools.DevAssert(!IsConnectedToTheEngine, null);
    }

    private void HookupChild(IChartComponentCalculatableWrapper child)
    {
        if (child == null)
        {
            return;
        }
        child.AddAssociatedTrace(this);
        child.UpdateTraceWithDeserializedError += OnUpdateTraceWithDeserializedError;
    }

    private void UnHookupChild(IChartComponentCalculatableWrapper child)
    {
        if (child == null)
        {
            return;
        }
        child.UpdateTraceWithDeserializedError -= OnUpdateTraceWithDeserializedError;
        child.RemoveAssociatedTrace(this);
    }

    private void UpdateChildren(ChartTraceDataToUpdate dataToUpdate)
    {
        IChartComponentCalculatableWrapper[] children = new IChartComponentCalculatableWrapper[] { XCalculatable, YCalculatable };
        foreach (IChartComponentCalculatableWrapper child in children)
        {
            if (DebugTools.AssertNotNull(child, null).Passed)
            {
                child.UpdateAssociatedTraceData(dataToUpdate);
            }
        }
    }
}
