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
                DispatcherPriority? priority = _encapsulatedCalculatable is ICalcWithPriority ? (_encapsulatedCalculatable as ICalcWithPriority).ResultsProcessingPriority : (DispatcherPriority?)null;
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
    }
}
