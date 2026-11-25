using System;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using Quire.Calculation.Core;

namespace Quire.Calculation;

public partial class CalculationController
{
    private partial class DispatchedEngineCallbackHandler
    {
        private readonly Regex _resolvedIdRegEx = new Regex("^(?<name>[^:]+):0x(?<handle>[0-9a-fA-F]{4})(?<tag>[0-9a-fA-F]{4})$");

        public DispatchedEngineCallbackHandler(ICalculatable calculatable, Dispatcher dispatcher, Func<ICalculatable, bool> isCalcInContext, EngineOptions engineOptions)
        {
            _encapsulatedCalculatable = calculatable;
            _dispatcher = dispatcher;
            _isCalcInContext = isCalcInContext;
            _engineOptions = engineOptions;
            if (_encapsulatedCalculatable is ICalculatableWithDataExchange { ComponentHandle: not null } calculatableWithDataExchange)
            {
                _cookie = calculatableWithDataExchange.ComponentCookieMultiThreaded;
            }
        }

        protected DispatcherPriority ResultDispatchingPriority
        {
            get
            {
                // Separate `is` and `as` checks: a pattern variable compiles to a different branch shape than Mathcad's IL.
                DispatcherPriority? priority = (_encapsulatedCalculatable is ICalcWithPriority) ? (_encapsulatedCalculatable as ICalcWithPriority).ResultsProcessingPriority : (DispatcherPriority?)null;
                if (!priority.HasValue)
                {
                    return CalcResultsProcessingPriorities.DefaultResultDispatchPriority_Background;
                }
                return priority.Value;
            }
        }

        public string Rl
        {
            get
            {
                if (!_isCalcInContext(_encapsulatedCalculatable))
                {
                    return string.Empty;
                }
                return _encapsulatedCalculatable.Rl;
            }
        }

        protected bool IsComponentCookieUpToDate
        {
            get
            {
                if (_encapsulatedCalculatable is ICalculatableWithDataExchange calculatableWithDataExchange)
                {
                    return _cookie == calculatableWithDataExchange.ComponentCookieMultiThreaded;
                }
                return true;
            }
        }

        protected void IncrementComponentCookie()
        {
            if (_encapsulatedCalculatable is ICalculatableWithDataExchange { ComponentHandle: not null } calculatableWithDataExchange)
            {
                _cookie = calculatableWithDataExchange.IncrementComponentCookieMultiThreaded(false);
            }
        }

        protected void DispatchedSetSynchronizationStatus(SynchronizationStatus status)
        {
            // Guard clauses: Mathcad's IL returns from each failed check instead of branching around one combined condition.
            if (!_isCalcInContext(_encapsulatedCalculatable))
            {
                return;
            }
            if (!IsComponentCookieUpToDate)
            {
                return;
            }
            _encapsulatedCalculatable.SetSynchronizationStatus(status);
            if (_encapsulatedCalculatable is ICalculatableResultOwner { InterceptAndUpdateResult: not false, EngineResult: var engineResult } calculatableResultOwner)
            {
                calculatableResultOwner.EngineResult = new EngineResult(engineResult, status);
            }
        }

        protected virtual void DispatchedSetResult(IResultNode result)
        {
            // Guard clauses: Mathcad's IL returns from each failed check instead of branching around one combined condition.
            if (!_isCalcInContext(_encapsulatedCalculatable))
            {
                return;
            }
            if (!IsComponentCookieUpToDate)
            {
                return;
            }
            _encapsulatedCalculatable.SetResult(result);
            if (_encapsulatedCalculatable is ICalculatableResultOwner { InterceptAndUpdateResult: not false, EngineResult: var engineResult } calculatableResultOwner)
            {
                calculatableResultOwner.EngineResult = new EngineResult(engineResult, result);
            }
        }

        protected virtual void DispatchedSetError(EngineError error)
        {
            // Guard clauses: Mathcad's IL returns from each failed check instead of branching around one combined condition.
            if (!_isCalcInContext(_encapsulatedCalculatable))
            {
                return;
            }
            if (!IsComponentCookieUpToDate)
            {
                return;
            }
            if (_encapsulatedCalculatable is ICalculatableResultOwner { InterceptAndUpdateResult: not false, EngineResult: var engineResult } calculatableResultOwner)
            {
                calculatableResultOwner.EngineResult = new EngineResult(engineResult, CloneError(error));
            }
            _encapsulatedCalculatable.SetError(error);
        }

        protected EngineError CloneError(EngineError error)
        {
            if (error == null)
            {
                return null;
            }
            EngineError engineError = error.Clone();
            if (engineError.ErrorSource == _encapsulatedCalculatable || !(_encapsulatedCalculatable is ICalculatableAggregateWithComponent))
            {
                ResultManager.ErrorHandlesToSerialNumber(engineError, Rl);
            }
            else
            {
                DebugTools.AssertNotNull(engineError.ErrorSource, null);
            }
            return engineError;
        }

        protected virtual void DispatchedSetSyncResultAndError(IResultNode result, EngineError error)
        {
            if (!_isCalcInContext(_encapsulatedCalculatable))
            {
                return;
            }
            if (!IsComponentCookieUpToDate)
            {
                return;
            }
            if (_encapsulatedCalculatable is ICalculatableResultOwner { InterceptAndUpdateResult: not false } calculatableResultOwner)
            {
                EngineError engineError = CloneError(error);
                EngineErrorCollection errorCollection = engineError == null ? null : new EngineErrorCollection { engineError };
                calculatableResultOwner.EngineResult = new EngineResult(result, errorCollection, SynchronizationStatus.Synchronized);
            }
            _encapsulatedCalculatable.SetError(error);
            _encapsulatedCalculatable.SetResult(result);
            _encapsulatedCalculatable.SetSynchronizationStatus(SynchronizationStatus.Synchronized);
        }
    }
}
