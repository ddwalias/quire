using System;
using System.Diagnostics;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;
using Quire.Engine;

namespace Quire.Calculation;

public partial class CalculationController
{
    public static bool PreserveEngineBuilder { get; set; }

    private void CleanupAfterCalculatableRemoved(ICalculatable calculatable)
    {
        if (!_iCalculatableToHandleMap.ContainsKey(calculatable))
        {
            return;
        }
        UnhookFromCalculatableEvents(calculatable);
        UnregisterDataExchange(GetDataExchangeOwnerRemoved(_iCalculatableToHandleMap[calculatable].container, calculatable));
        int engineHandle = _iCalculatableToHandleMap[calculatable].engineHandle;
        _handleToICalculatableMap.Remove(engineHandle);
        _iCalculatableToHandleMap.Remove(calculatable);
        calculatable.SetCalculatingStatus((CalculatingStatus)2);
        if (IsAggregate(calculatable))
        {
            ICalculatableContainer calculatableContainer = (ICalculatableContainer)calculatable;
            foreach (ICalculatable calculatable2 in calculatableContainer.CalculatableChildren)
            {
                CleanupAfterCalculatableRemoved(calculatable2);
            }
            calculatableContainer.CalculatableAdded -= HandleCalculatableAdded;
            calculatableContainer.CalculatableMoved -= HandleCalculatableMoved;
            calculatableContainer.CalculatableRemoved -= HandleCalculatableRemoved;
        }
    }

    private void RegisterDataExchange(ICalculatableWithDataExchange calcWithDataExchange)
    {
        if (calcWithDataExchange == null)
        {
            return;
        }
        IDataExchangeHandler component = HandlersFactory.CreateDataExchangeHandler(
            calcWithDataExchange, _contextThreadDispatcher, HandleEngineNeedsUIThreadMessagesProcessed, ref _engineOptions);
        int value = _engine.RegisterComponent(component);
        calcWithDataExchange.ComponentHandle = value;
    }

    private ICalculatableWithDataExchange GetDataExchangeOwnerAdded(ICalculatableContainer container, ICalculatable arg)
    {
        ICalculatableWithDataExchange calculatableWithDataExchange = arg as ICalculatableWithDataExchange;
        ICalculatableWithDataExchange calculatableWithDataExchange2;
        ICalculatableWrapper calculatableWrapper;
        ICalculatableWithDataExchange calculatableWithDataExchange3;
        if (calculatableWithDataExchange != null &&
            (!calculatableWithDataExchange.ComponentHandle.HasValue || calculatableWithDataExchange.ComponentHandle == 0) &&
            calculatableWithDataExchange.DataExchangeModel != null)
        {
            return calculatableWithDataExchange;
        }
        calculatableWithDataExchange2 = container as ICalculatableWithDataExchange;
        if (calculatableWithDataExchange2 != null &&
            !calculatableWithDataExchange2.ComponentHandle.HasValue &&
            calculatableWithDataExchange2.DataExchangeModel != null)
        {
            return calculatableWithDataExchange2;
        }
        calculatableWrapper = container as ICalculatableWrapper;
        if (calculatableWrapper != null)
        {
            calculatableWithDataExchange3 = calculatableWrapper.WrappedCalculatable as ICalculatableWithDataExchange;
            if (calculatableWithDataExchange3 != null &&
                !calculatableWithDataExchange3.ComponentHandle.HasValue &&
                calculatableWithDataExchange3.DataExchangeModel != null)
            {
                IDataExchangeScriptingModel dataExchangeScriptingModel = calculatableWithDataExchange3.DataExchangeModel as IDataExchangeScriptingModel;
                if (dataExchangeScriptingModel != null)
                {
                    dataExchangeScriptingModel.IsBlockRunningScript = true;
                }
                return calculatableWithDataExchange3;
            }
        }
        return null;
    }

    private ICalculatableWithDataExchange GetDataExchangeOwnerRemoved(ICalculatableContainer container, ICalculatable arg)
    {
        ICalculatableWithDataExchange calculatableWithDataExchange = container as ICalculatableWithDataExchange;
        ICalculatableWithDataExchange calculatableWithDataExchange2;
        ICalculatableWrapper calculatableWrapper;
        ICalculatableWithDataExchange calculatableWithDataExchange3;
        if (calculatableWithDataExchange != null &&
            calculatableWithDataExchange.DataExchangeModel == null &&
            calculatableWithDataExchange.ComponentHandle.HasValue)
        {
            return calculatableWithDataExchange;
        }
        calculatableWithDataExchange2 = arg as ICalculatableWithDataExchange;
        if (calculatableWithDataExchange2 != null &&
            calculatableWithDataExchange2.ComponentHandle.HasValue)
        {
            return calculatableWithDataExchange2;
        }
        calculatableWrapper = arg as ICalculatableWrapper;
        if (calculatableWrapper != null)
        {
            calculatableWithDataExchange3 = calculatableWrapper.WrappedCalculatable as ICalculatableWithDataExchange;
            if (calculatableWithDataExchange3 != null &&
                calculatableWithDataExchange3.ComponentHandle.HasValue)
            {
                return calculatableWithDataExchange3;
            }
        }
        return null;
    }

    private void HandleEngineNeedsUIThreadMessagesProcessed()
    {
        _interruptWaitForCalc.Set();
    }

    private void UnregisterDataExchange(ICalculatableWithDataExchange calcWithDataExchange)
    {
        if (calcWithDataExchange == null)
        {
            return;
        }
        _engine.UnregisterComponent(calcWithDataExchange.ComponentHandle.Value);
        calcWithDataExchange.ComponentHandle = null;
    }

    private static bool IsAggregate(ICalculatable calculatable) => calculatable is ICalculatableAggregate;

    private static bool IsAggregate(ICalculatableContainer container) => container is ICalculatableAggregate;

    private object HandleGetEngineOption(string value)
    {
        return _engine.GetOption(value);
    }

    private void HookupCalculatableEvents(ICalculatable calc)
    {
        calc.RequestEvaluation += HandleCalculatableUpdated;
        calc.GetValueOfIdentifierBefore += HandleGetValueOfIdentifierBefore;
        calc.TreatAsUpToDate += HandleTreatCalculatableAsUpToDate;
    }

    private void UnhookFromCalculatableEvents(ICalculatable calc)
    {
        calc.RequestEvaluation -= HandleCalculatableUpdated;
        calc.GetValueOfIdentifierBefore -= HandleGetValueOfIdentifierBefore;
        calc.TreatAsUpToDate -= HandleTreatCalculatableAsUpToDate;
    }

    private bool IsCalculatableInContext(ICalculatable calc)
    {
        return _iCalculatableToHandleMap.ContainsKey(calc);
    }

    [Conditional("DEBUG")]
    private static void DebugEnter(string methodName, string methodInfo)
    {
        if (!_enableDebugWrites)
        {
            return;
        }
        if (!string.IsNullOrEmpty(methodInfo) && methodInfo.Length < 40)
        {
            methodInfo = null;
        }
        string.IsNullOrEmpty(methodInfo);
    }

    [Conditional("DEBUG")]
    private static void DebugExit(string methodName, string methodInfo)
    {
        if (!_enableDebugWrites)
        {
            return;
        }
        string.IsNullOrEmpty(methodInfo);
    }

    private void HandleEngineNodeCallback(int handle, NodeStatus status, int clientCookie, int triggerHandle, int triggerCookie)
    {
        _handleToICalculatableMap.TryGetValue(handle, out var calculatable);
        if (calculatable != null)
        {
            DebugTools.LogData(string.Format("HandleEngineNodeCallback: {0}: {1}", handle, status));
            var handler = HandlersFactory.CreateEngineCallbackHandler(
                calculatable, _contextThreadDispatcher, IsCalculatableInContext, _engineOptions);
            switch (status)
            {
                case NodeStatus.NODE_COMPUTING:
                case NodeStatus.NODE_NOT_COMPUTING:
                    handler.SetCalculatingStatus((CalculatingStatus)status);
                    break;
                case NodeStatus.NODE_INVALID:
                    handler.SetSynchronizationStatus(SynchronizationStatus.Pending);
                    break;
                case NodeStatus.NODE_TREATED_READY:
                    // The original Synchronized value is 3; the legacy enum still declares it as 1.
                    handler.SetSynchronizationStatus((SynchronizationStatus)3);
                    break;
                case NodeStatus.NODE_READY:
                    handler.SetResult(handle, _engine, _handleToICalculatableMap);
                    break;
            }
        }
    }
}
