using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;
using Quire.Engine;

namespace Quire.Calculation;

public partial class CalculationController
{
    private partial class EngineCallbackHandlerNoOP : DispatchedEngineCallbackHandler
    {
        public EngineCallbackHandlerNoOP(ICalculatable calculatable, Dispatcher dispatcher, Func<ICalculatable, bool> isCalcInContext, EngineOptions engineOptions)
            : base(calculatable, dispatcher, isCalcInContext, engineOptions)
        {
        }

        public override void SetCalculatingStatus(CalculatingStatus computingStatus)
        {
        }

        public override void SetSynchronizationStatus(SynchronizationStatus syncStatus)
        {
        }

        public override void SetResult(int handle, IEngine engine, Dictionary<int, ICalculatable> handleToICalculatableMap)
        {
            base.SetSynchronizationStatus((SynchronizationStatus)3);
        }
    }

    private partial class HandlersFactory
    {
        public static IEngineCallbackHandler CreateEngineCallbackHandler(ICalculatable calculatable, Dispatcher dispatcher, Func<ICalculatable, bool> isCalcInContext, EngineOptions engineOptions)
        {
            if (calculatable is ICalculatableAggregate)
            {
                return new DispatchedEngineCallbackHandlerAggregateWithCache(calculatable, dispatcher, isCalcInContext, engineOptions);
            }
            if (calculatable is ICalculationHideableChild)
            {
                return new DispatchedEngineCallbackHandlerHideableChildWithCache(calculatable, dispatcher, isCalcInContext, engineOptions);
            }
            if (calculatable is ISimpleCalculatable)
            {
                if (calculatable.Rl == EngineBuilder.RL_NIL)
                {
                    return new EngineCallbackHandlerNoOP(calculatable, dispatcher, isCalcInContext, engineOptions);
                }
                return new DispatchedEngineCallbackHandlerChildWithCache(calculatable, dispatcher, isCalcInContext, engineOptions);
            }
            return new DispatchedEngineCallbackHandler(calculatable, dispatcher, isCalcInContext, engineOptions);
        }

        public static IDataExchangeHandler CreateDataExchangeHandler(ICalculatableWithDataExchange calculatable, Dispatcher dispatcher, Action handleEngineNeedsUIThreadMessagesProcessed, ref EngineOptions engineOptions)
        {
            if (calculatable.DataExchangeModel is IDataExchangeIValueModel model)
            {
                return new DataExchangeIValueHandler(model, dispatcher, handleEngineNeedsUIThreadMessagesProcessed, engineOptions);
            }
            return new DataExchangeScriptingHandler(calculatable, dispatcher, handleEngineNeedsUIThreadMessagesProcessed, engineOptions);
        }

        public HandlersFactory() { }
    }
}
