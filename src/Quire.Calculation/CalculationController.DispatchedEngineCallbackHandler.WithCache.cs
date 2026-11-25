using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Threading;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;
using Quire.Engine;

namespace Quire.Calculation;

public partial class CalculationController
{
    private partial class DispatchedEngineCallbackHandlerHideableChildWithCache
    {
        public DispatchedEngineCallbackHandlerHideableChildWithCache(ICalculatable calculatable, Dispatcher dispatcher, Func<ICalculatable, bool> isCalcInContext, EngineOptions engineOptions)
            : base(calculatable, dispatcher, isCalcInContext, engineOptions)
        {
        }

        public override void SetSynchronizedWithResultNoError(SimpleResultNode result)
        {
            Action action;
            if (_encapsulatedCalculatable is ICalcWithCache { Result: not null } calcWithCache && calcWithCache.Result.Equals(result) && calcWithCache.Error == null)
            {
                action = () => DispatchedSetSynchronizationStatus(SynchronizationStatus.Synchronized);
            }
            else
            {
                action = () => DispatchedSetSyncResultAndError(result, null);
            }
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
        }

        public override void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings)
        {
            if (_encapsulatedCalculatable is ICalcWithCache calcWithCache && SimpleCalculatable.IsEngineWarningsEquals(calcWithCache.EngineWarnings, warnings))
            {
                return;
            }
            base.SetRedefinitionWarnings(warnings);
        }

        protected override void ProcessAlgorithms(IDictionary results, Dictionary<int, ICalculatable> handleToICalculatableMap)
        {
        }
    }

    private partial class DispatchedEngineCallbackHandlerChildWithCache
    {
        public DispatchedEngineCallbackHandlerChildWithCache(ICalculatable calculatable, Dispatcher dispatcher, Func<ICalculatable, bool> isCalcInContext, EngineOptions engineOptions)
            : base(calculatable, dispatcher, isCalcInContext, engineOptions)
        {
        }

        public override void SetSynchronizedWithResultNoError(SimpleResultNode result)
        {
            // The local slots follow Mathcad's order (action, simpleCalculatable, num): declared in that order.
            Action action;
            ISimpleCalculatable simpleCalculatable = _encapsulatedCalculatable as ISimpleCalculatable;
            double num = Convert.ToDouble(_engineOptions.SystemVariable["mc_ORIGIN"]);
            if (simpleCalculatable.Result != null && simpleCalculatable.Result.Equals(result) && simpleCalculatable.Error == null && simpleCalculatable.SystemOrigin.Value.Equals(num))
            {
                action = () => DispatchedSetSynchronizationStatus(SynchronizationStatus.Synchronized);
            }
            else
            {
                simpleCalculatable.SystemOrigin.Value = num;
                action = () => DispatchedSetSyncResultAndError(result, null);
            }
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
        }

        public override void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings)
        {
            if (_encapsulatedCalculatable is ICalcWithCache calcWithCache && SimpleCalculatable.IsEngineWarningsEquals(calcWithCache.EngineWarnings, warnings))
            {
                return;
            }
            base.SetRedefinitionWarnings(warnings);
        }
    }

    private partial class DispatchedEngineCallbackHandlerAggregateWithCache
    {
        public DispatchedEngineCallbackHandlerAggregateWithCache(ICalculatable calculatable, Dispatcher dispatcher, Func<ICalculatable, bool> isCalcInContext, EngineOptions engineOptions)
            : base(calculatable, dispatcher, isCalcInContext, engineOptions)
        {
        }

        public override void SetSynchronizedWithResultNoError(SimpleResultNode result)
        {
            Action action;
            if (_encapsulatedCalculatable is ICalcWithCache { Error: null })
            {
                action = () => DispatchedSetSynchronizationStatus(SynchronizationStatus.Synchronized);
            }
            else
            {
                action = () => DispatchedSetSyncResultAndError(result, null);
            }
            _dispatcher.BeginInvoke(ResultDispatchingPriority, action);
        }

        public override void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings)
        {
            if (_encapsulatedCalculatable is ICalcWithCache calcWithCache && SimpleCalculatable.IsEngineWarningsEquals(calcWithCache.EngineWarnings, warnings))
            {
                return;
            }
            base.SetRedefinitionWarnings(warnings);
        }
    }
}
