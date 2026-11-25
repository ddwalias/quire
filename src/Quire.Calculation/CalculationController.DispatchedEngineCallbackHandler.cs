using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using Quire.Calculation.Core;
using Quire.Engine;

namespace Quire.Calculation;

public partial class CalculationController
{
    // The members of this class are declared in Mathcad's order. Roslyn numbers the lambdas here (`<Commit>b__19_0`
    // and so on) by the count of members declared before each one, so the order and the count of the members matter.
    private partial class DispatchedEngineCallbackHandler
    {
        protected readonly ICalculatable _encapsulatedCalculatable;
        protected readonly Dispatcher _dispatcher;
        protected EngineOptions _engineOptions;
        private readonly Regex _resolvedIdRegEx = new Regex("^(?<name>[^:]+):0x(?<handle>[0-9a-fA-F]{4})(?<tag>[0-9a-fA-F]{4})$");
        protected readonly Func<ICalculatable, bool> _isCalcInContext;

        public event Action<ICalculatable> RequestEvaluation;
        public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;
        public event Action<ICalculatable> TreatAsUpToDate;

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

        public void Commit()
        {
            Action action = () =>
            {
                // Guard clause: Mathcad's closure branches past the call with brtrue, not brfalse.
                if (!_isCalcInContext(_encapsulatedCalculatable))
                {
                    return;
                }
                _encapsulatedCalculatable.Commit();
            };
            _dispatcher.BeginInvoke(DispatcherPriority.Send, action);
        }

        public void SetStatusSynchronized()
        {
            Action action = () =>
            {
                // Guard clause: Mathcad's closure branches past the call with brtrue, not brfalse.
                if (!_isCalcInContext(_encapsulatedCalculatable))
                {
                    return;
                }
                _encapsulatedCalculatable.SetStatusSynchronized();
            };
            _dispatcher.BeginInvoke(DispatcherPriority.Send, action);
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

        public virtual void SetSynchronizationStatus(SynchronizationStatus syncStatus)
        {
            if (syncStatus == SynchronizationStatus.Pending)
            {
                IncrementComponentCookie();
            }
            Action action = () =>
            {
                DispatchedSetSynchronizationStatus(syncStatus);
            };
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
        }

        public virtual void SetCalculatingStatus(CalculatingStatus computingStatus)
        {
            Action action = () =>
            {
                // Guard clause: Mathcad's closure branches past the call with brtrue, not brfalse.
                if (!_isCalcInContext(_encapsulatedCalculatable))
                {
                    return;
                }
                _encapsulatedCalculatable.SetCalculatingStatus(computingStatus);
            };
            _dispatcher.BeginInvoke(DispatcherPriority.Send, action);
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

        public virtual void SetResult(IResultNode result)
        {
            Action action = () =>
            {
                DispatchedSetResult(result);
            };
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
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

        public virtual void SetError(EngineError error)
        {
            Action action = () =>
            {
                DispatchedSetError(error);
            };
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
        }

        public virtual void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings)
        {
            Action action = () =>
            {
                // Guard clause: Mathcad's closure branches past the call with brtrue, not brfalse.
                if (!_isCalcInContext(_encapsulatedCalculatable))
                {
                    return;
                }
                _encapsulatedCalculatable.SetRedefinitionWarnings(warnings);
            };
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
        }

        public virtual void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
        {
            Action action = () =>
            {
                // Guard clause: Mathcad's closure branches past the call with brtrue, not brfalse.
                if (!_isCalcInContext(_encapsulatedCalculatable))
                {
                    return;
                }
                _encapsulatedCalculatable.SetIdentifierLabels(idLabelMap);
            };
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
        }

        protected void IncrementComponentCookie()
        {
            if (_encapsulatedCalculatable is ICalculatableWithDataExchange { ComponentHandle: not null } calculatableWithDataExchange)
            {
                _cookie = calculatableWithDataExchange.IncrementComponentCookieMultiThreaded(false);
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

        // Declared after SetIdentifierLabels: its position fixes the closure ordinals of the members below.
        private int _cookie;

        public virtual void SetSynchronizedNoResultNoError()
        {
            Action action = () =>
            {
                DispatchedSetSyncResultAndError(_emptyResult, null);
            };
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
        }

        public virtual void SetSynchronizedWithResultNoError(SimpleResultNode result)
        {
            Action action = () =>
            {
                DispatchedSetSyncResultAndError(result, null);
            };
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
        }

        public virtual void SetSynchronizedWithErrorNoResult(EngineError error)
        {
            Action action = () =>
            {
                DispatchedSetSyncResultAndError(_emptyResult, error);
            };
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
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

        public virtual void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap)
        {
            Action action = () =>
            {
                _encapsulatedCalculatable.SetAutoSelectAlgorithm(autoAlgorithmMap);
            };
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
        }

        // Blocked: the Mathcad body calls IEngine handle members that carry modopt(IsLong).
        private IDictionary ParentGlobalErrorResultsDictionary(int handle, IEngine engine, Dictionary<int, ICalculatable> handleToICalculatableMap) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        protected virtual void ErrorHandle(IDictionary resultsIDictionary, Dictionary<int, ICalculatable> handleToICalculatableMap)
        {
            EngineErrorHandler engineErrorHandler = new EngineErrorHandler(handleToICalculatableMap);
            IList list = resultsIDictionary["%errors-new"] as IList;
            engineErrorHandler.HandleErrors(list).Each((EngineError error) =>
            {
                SetSynchronizedWithErrorNoResult(error);
            });
        }

        // Blocked: IEngine handle members (see the checkpoint note in docs/mirror/work/lane-calculation-haiku-20261125.md).
        public virtual void SetResult(int handle, IEngine engine, Dictionary<int, ICalculatable> handleToICalculatableMap) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        protected virtual void ProcessResult(int handle, IEngine engine, IDictionary results, Dictionary<int, ICalculatable> handleToICalculatableMap) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        protected virtual void ProcessIdBoundLabels(int handle, IDictionary results, Dictionary<int, ICalculatable> handleToICalculatableMap) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        protected virtual void ProcessAlgorithms(IDictionary results, Dictionary<int, ICalculatable> handleToICalculatableMap) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        protected virtual void ProcessWarningRedefinition(IDictionary results, Dictionary<int, ICalculatable> handleToICalculatableMap) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }
}
