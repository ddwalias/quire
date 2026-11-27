using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.ExcelComponent.View;

namespace Quire.Controls.ExcelComponent.Model;

// Compile-order device: all ChartComponentModel members live in this file, in Mathcad member order, so each
// closure is numbered as in Release IL (lambda ordinals count the members before its owner).
public partial class ChartComponentModel
{
    private readonly Dictionary<ChartWrapperType, SimpleChartCalculatable> _defaultLimits;

    private readonly ChartMinMaxCalculatable _chartMinMaxCalc;

    private readonly ChartMinMaxPreCalculatable _chartMinMaxPreCalc;

    private readonly SortedDictionary<uint, ChartTraceCalculatable> _traceCalculatables;

    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("ChartComponentModel constructor order")]
    public ChartComponentModel() { }

    internal void CommitPre()
    {
        ConnectedDefaultLimits.Each(limit => limit.Commit());
    }

    internal void CommitPost()
    {
        _chartMinMaxCalc.Commit();
    }

    public override void Commit()
    {
        CalculatablesConnectedToTheEngine.Each(calc => calc.Commit());
        base.Commit();
    }

        public override string Rl
        {
            get
            {
                ComponentNodeHandle = 1u;
                return string.Format("(eval (named-component:0x{0:x8} {1} {2} %@ 0))", ComponentNodeHandle, ChartTraceCalculatable.ChartVariableWithLabel, ComponentId);
            }
        }

        protected override void FireCalculatableAdded(ICalculatable calculatable)
        {
            IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = calculatable as IChartComponentCalculatableWrapper;
            if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Failed)
            {
                return;
            }
            if (DebugTools.DevAssert(chartComponentCalculatableWrapper.ChartWrapperType.IsUnknownType(), null).Failed)
            {
                chartComponentCalculatableWrapper.ResetChartWrapperType();
            }
        }

        protected override void FireCalculatableRemoved(ICalculatable calculatable)
        {
            IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = calculatable as IChartComponentCalculatableWrapper;
            if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Failed)
            {
                return;
            }
            if (!IsDisposed)
            {
                ChartWrapperType value = chartComponentCalculatableWrapper.ResetChartWrapperType();
                HandleChildWrapperRemovedOrChartTypeChanged(chartComponentCalculatableWrapper, value);
            }
        }

        protected override void FireCalculatableMoved(ICalculatable calculatable)
        {
            IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = calculatable as IChartComponentCalculatableWrapper;
            if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Failed)
            {
                return;
            }
            if (chartComponentCalculatableWrapper.IsLimitExpression())
            {
                UpdateChartMinMaxCalculatable();
            }
            if (chartComponentCalculatableWrapper.IsTraceOrCommonExpression())
            {
                UpdateAllTraces(null);
            }
            base.FireCalculatableMoved(calculatable);
        }

    protected override IEnumerable<string> ComponentErrors { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

        protected override void SetErrorByNodeHandle(EngineError error)
        {
            DebugTools.FailAssertion(error.NodeHandle.ToString());
            SetErrorFallback(error);
        }

        protected override void SetChildError(EngineError error)
        {
            if (error != null && error.ErrorSource is IChartMinMaxCommon)
            {
                _chartMinMaxCalc.ProcessComponentError(error);
                error = null;
            }
            base.SetChildError(error);
        }

        protected override void UpdateCustomErrorBasedOnPlacement(IWhiteboardItem item, bool placedAfterOleContainer)
        {
            ICalculatable calculatable = item as ICalculatable;
            if (DebugTools.AssertNotNull(calculatable, null).Failed)
            {
                return;
            }
            DebugTools.DevAssert(!GetWrapper(calculatable).IsOutputEquation && !placedAfterOleContainer, null);
        }

    protected override IEnumerable<ICalculatable> CalculatablesConnectedToTheEngine
    {
        get
        {
            foreach (SimpleChartCalculatable connectedDefaultLimit in ConnectedDefaultLimits)
            {
                yield return connectedDefaultLimit;
            }
            foreach (ICalculatable item in from item in WhiteboardItems.OfType<ICalculatable>()
                where GetWrapper(item).IsConnectedToTheEngine
                select item)
            {
                yield return item;
            }
            yield return _chartMinMaxPreCalc;
            foreach (ChartTraceCalculatable value in _traceCalculatables.Values)
            {
                yield return value;
            }
            yield return _chartMinMaxCalc;
        }
    }

        protected override ICalculatable CreateCalculatableWrapper(ICalculatable calc, bool arg2)
        {
            if (calc is IChartCalculatable)
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
                return calc;
            }
            if (CalculatableWrapperMap.ContainsKey(calc))
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
                return CalculatableWrapperMap[calc];
            }
            ChartComponentCalculatableWrapper chartComponentCalculatableWrapper = new ChartComponentCalculatableWrapper(calc);
            CalculatableWrapperMap[calc] = chartComponentCalculatableWrapper;
            chartComponentCalculatableWrapper.ChartTypeChanged += HandleChildWrapperChartTypeChanged;
            chartComponentCalculatableWrapper.ErrorChanged += HandleChildWrapperErrorChanged;
            return chartComponentCalculatableWrapper;
        }

        protected override ICalculatable LookupCalculatableWrapper(ICalculatable calc)
        {
            if (calc is IChartCalculatable)
            {
                return calc;
            }
            return base.LookupCalculatableWrapper(calc);
        }

        protected override void RemoveCalculatableWrapper(ICalculatable calc)
        {
            if (calc is IChartCalculatable)
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
                return;
            }
            IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = GetWrapper(calc) as IChartComponentCalculatableWrapper;
            if (DebugTools.AssertNotNull(chartComponentCalculatableWrapper, null).Passed)
            {
                chartComponentCalculatableWrapper.ChartTypeChanged -= HandleChildWrapperChartTypeChanged;
                chartComponentCalculatableWrapper.ErrorChanged -= HandleChildWrapperErrorChanged;
                chartComponentCalculatableWrapper.Dispose();
            }
            CalculatableWrapperMap.Remove(calc);
            UpdateErrorAccordingToChildren();
        }

    internal override IOleContainerItem OleContainerItem
    {
        get
        {
            return base.OleContainerItem;
        }
        set
        {
            DebugTools.DevAssert(base.OleContainerItem == null && value != null, null);
            base.OleContainerItem = value;
            if (ChartContainerItem != null)
            {
                ChartContainerItem.PublicModel.CurrentTraceIds = _traceCalculatables.Keys;
                ChartContainerItem.PublicModel.OnProcessMinMax += (IResultNode result, double[,] dataLimits, StringBuilder errorBuilder) => _chartMinMaxCalc.ProcessResult(result, dataLimits, errorBuilder);
            }
        }
    }

        public override object Clone()
        {
            ChartComponentModel chartComponentModel = new ChartComponentModel();
            CopyStateTo(chartComponentModel);
            return chartComponentModel;
        }

    private IEnumerable<SimpleChartCalculatable> ConnectedDefaultLimits => _defaultLimits.Values.Where(calc => calc.IsConnectedToTheEngine);

        private ChartContainer ChartContainerItem => (ChartContainer)OleContainerItem;

    internal event Action RequestUpdateIdentifierLabels;

        private void HandleChildWrapperChartTypeChanged(IChartComponentCalculatableWrapper wrapper)
        {
            HandleChildWrapperRemovedOrChartTypeChanged(wrapper, null);
        }

        private void HandleChildWrapperRemovedOrChartTypeChanged(IChartComponentCalculatableWrapper wrapper, ChartWrapperType? prevTypeWhenRemoving)
        {
            ChartWrapperType type = prevTypeWhenRemoving ?? wrapper.PreviousChartType;
            ChartWrapperType chartWrapperType = wrapper.ChartWrapperType;
            if (chartWrapperType.IsUnknownType() && !prevTypeWhenRemoving.HasValue)
            {
                return;
            }
            DebugTools.DevAssert(!prevTypeWhenRemoving.HasValue || (wrapper.ChartWrapperType == ChartWrapperType.Unknown && wrapper.PreviousChartType == ChartWrapperType.Unknown), null);
            if (type.IsLimitType() || chartWrapperType.IsLimitType())
            {
                if (wrapper.IsConnectedToTheEngine && !chartWrapperType.IsLimitType())
                {
                    base.FireCalculatableRemoved(wrapper);
                }
                UpdateChartMinMaxCalculatable();
            }
            if (type.IsTraceOrCommonType() || chartWrapperType.IsTraceOrCommonType())
            {
                UpdateAllTraces(wrapper);
            }
            if (RequestUpdateIdentifierLabels != null)
            {
                RequestUpdateIdentifierLabels();
            }
        }

    private void UpdateAllTraces(IChartComponentCalculatableWrapper wrapperToCommit)
    {
        SortedDictionary<uint, ChartTraceCalculatable> sortedDictionary = new SortedDictionary<uint, ChartTraceCalculatable>();
        IEnumerable<IChartComponentCalculatableWrapper> source = from IChartComponentCalculatableWrapper wrapper in WhiteboardItems.OfType<ICalculatable>().Select(GetWrapper)
            where !wrapper.HideFromCalculationContext && wrapper.IsTraceOrCommonExpression()
            select wrapper;
        IDictionary<IChartComponentCalculatableWrapper, bool> primaryDict = source.ToDictionary((IChartComponentCalculatableWrapper wrapper) => wrapper, (IChartComponentCalculatableWrapper wrapper) => false);
        primaryDict.Keys.Each((IChartComponentCalculatableWrapper wrapper) =>
        {
            if (wrapper.CustomErrorType == OleWrapperCustomErrorType.Placement)
            {
                wrapper.SetCustomError(null);
            }
        });
        Action<ChartTraceCalculatable, IChartComponentCalculatableWrapper> action = (ChartTraceCalculatable traceCalc, IChartComponentCalculatableWrapper wrapper) =>
        {
            if (wrapper == null)
            {
                return;
            }
            if (wrapper.ChartWrapperType == ChartWrapperType.XTrace || wrapper.ChartWrapperType == ChartWrapperType.XCommon)
            {
                if (traceCalc.XCalculatable == null)
                {
                    traceCalc.XCalculatable = wrapper;
                    primaryDict[wrapper] = true;
                }
            }
            else if (traceCalc.YCalculatable == null)
            {
                traceCalc.YCalculatable = wrapper;
                primaryDict[wrapper] = true;
            }
        };
        foreach (IChartComponentCalculatableWrapper item in source.Where((IChartComponentCalculatableWrapper wrapper) => wrapper.ChartWrapperType.IsIndexedTraceType()))
        {
            if (!DebugTools.AssertNotNull(item.TraceId, null).Failed)
            {
                uint value = item.TraceId.Value;
                if (!sortedDictionary.TryGetValue(value, out var traceCalculatable))
                {
                    // Chained so the new trace is stored in traceCalculatable before the indexer setter, as Release IL does.
                    sortedDictionary[value] = traceCalculatable = new ChartTraceCalculatable(_chartMinMaxCalc);
                }
                action(traceCalculatable, item);
            }
        }
        IChartComponentCalculatableWrapper arg = source.FirstOrDefault((IChartComponentCalculatableWrapper wrapper) => wrapper.ChartWrapperType == ChartWrapperType.XCommon);
        IChartComponentCalculatableWrapper yCommonWrapper = source.FirstOrDefault((IChartComponentCalculatableWrapper wrapper) => wrapper.ChartWrapperType == ChartWrapperType.YCommon);
        if (!sortedDictionary.ContainsKey(1u))
        {
            sortedDictionary.Add(1u, new ChartTraceCalculatable(_chartMinMaxCalc));
        }
        foreach (ChartTraceCalculatable traceCalculatable in sortedDictionary.Values)
        {
            action(traceCalculatable, arg);
            action(traceCalculatable, yCommonWrapper);
        }
        List<uint> list = new List<uint>();
        foreach (KeyValuePair<uint, ChartTraceCalculatable> traceCalculatable in _traceCalculatables)
        {
            uint key = traceCalculatable.Key;
            if (!sortedDictionary.ContainsKey(key) || sortedDictionary[key].XCalculatable == null || sortedDictionary[key].YCalculatable == null)
            {
                list.Add(key);
            }
        }
        foreach (uint traceId in list)
        {
            ChartTraceCalculatable traceToRemove = _traceCalculatables[traceId];
            _traceCalculatables.Remove(traceId);
            if (DebugTools.AssertNotNull(ChartContainerItem, null).Passed)
            {
                Exception ex = ChartContainerItem.PublicModel.RemoveTrace(traceId);
                if (ex != null)
                {
                    DebugTools.FailAssertion(ex);
                }
            }
            base.FireCalculatableRemoved(traceToRemove);
            traceToRemove.Dispose();
        }
        List<ChartTraceCalculatable> list2 = new List<ChartTraceCalculatable>();
        foreach (KeyValuePair<uint, ChartTraceCalculatable> tracePair in sortedDictionary)
        {
            uint traceId = tracePair.Key;
            ChartTraceCalculatable traceCalculatable = tracePair.Value;
            if (traceCalculatable.XCalculatable != null && traceCalculatable.YCalculatable != null)
            {
                if (!_traceCalculatables.ContainsKey(traceId))
                {
                    _traceCalculatables.Add(traceId, traceCalculatable);
                    base.FireCalculatableAdded(traceCalculatable);
                    list2.Add(traceCalculatable);
                    continue;
                }
                ChartTraceCalculatable existingTrace = _traceCalculatables[traceId];
                IChartComponentCalculatableWrapper xCalculatable = traceCalculatable.XCalculatable;
                IChartComponentCalculatableWrapper yCalculatable = traceCalculatable.YCalculatable;
                traceCalculatable.Dispose();
                if (existingTrace.XCalculatable != xCalculatable || existingTrace.YCalculatable != yCalculatable)
                {
                    existingTrace.XCalculatable = xCalculatable;
                    existingTrace.YCalculatable = yCalculatable;
                    list2.Add(existingTrace);
                }
                else if (existingTrace.XCalculatable == wrapperToCommit || existingTrace.YCalculatable == wrapperToCommit)
                {
                    list2.Add(existingTrace);
                }
            }
            else
            {
                traceCalculatable.Dispose();
            }
        }
        sortedDictionary.Clear();
        bool flag = false;
        if (ChartContainerItem != null)
        {
            flag = _traceCalculatables.IsNotEmpty() && ChartContainerItem.PublicModel.CurrentTraceIds.IsNullOrEmpty();
            ChartContainerItem.PublicModel.CurrentTraceIds = _traceCalculatables.Keys;
        }
        primaryDict.Each((KeyValuePair<IChartComponentCalculatableWrapper, bool> pair) =>
        {
            IChartComponentCalculatableWrapper wrapper = pair.Key;
            if (pair.Value)
            {
                DebugTools.DevAssert(wrapper.CustomErrorType != OleWrapperCustomErrorType.Placement, null);
            }
            else
            {
                wrapper.SetCustomError("chart_duplicate_trace_expression");
            }
        });
        if (flag)
        {
            Commit();
        }
        else
        {
            list2.Each((ChartTraceCalculatable calc) =>
            {
                calc.Commit();
            });
        }
        if (DebugTools.IsDebug)
        {
            source.Where((IChartComponentCalculatableWrapper wrapper) => wrapper.IsTraceOrCommonExpression()).Each((IChartComponentCalculatableWrapper currWrapper) =>
            {
                DebugTools.DevAssert(_traceCalculatables.Values.Any((ChartTraceCalculatable traceCalc) => traceCalc.XCalculatable == currWrapper || traceCalc.YCalculatable == currWrapper) == currWrapper.HasAssociatedTraces(), null);
            });
        }
    }

    private void UpdateChartMinMaxCalculatable()
    {
        IEnumerable<IChartComponentCalculatableWrapper> source = from IChartComponentCalculatableWrapper wrapper in WhiteboardItems.OfType<ICalculatable>().Select(GetWrapper)
            where !wrapper.HideFromCalculationContext && wrapper.IsLimitExpression()
            select wrapper;
        IDictionary<IChartComponentCalculatableWrapper, bool> dictionary = source.ToDictionary((IChartComponentCalculatableWrapper wrapper) => wrapper, (IChartComponentCalculatableWrapper wrapper) => false);
        dictionary.Keys.Each((IChartComponentCalculatableWrapper wrapper) =>
        {
            if (wrapper.CustomErrorType == OleWrapperCustomErrorType.Placement)
            {
                wrapper.SetCustomError(null);
            }
        });
        bool flag = false;
        ChartWrapperType[] array = new ChartWrapperType[6]
        {
            ChartWrapperType.XMin,
            ChartWrapperType.XMax,
            ChartWrapperType.YMin,
            ChartWrapperType.YMax,
            ChartWrapperType.Y2Min,
            ChartWrapperType.Y2Max
        };
        foreach (ChartWrapperType chartWrapperType in array)
        {
            ChartWrapperType currType = chartWrapperType;
            IChartComponentCalculatableWrapper chartComponentCalculatableWrapper = source.FirstOrDefault((IChartComponentCalculatableWrapper wrapper) => wrapper.ChartWrapperType == currType);
            if (chartComponentCalculatableWrapper != null)
            {
                dictionary[chartComponentCalculatableWrapper] = true;
            }
            if (_chartMinMaxCalc.SetLimit(currType, chartComponentCalculatableWrapper))
            {
                flag = true;
            }
        }
        foreach (KeyValuePair<IChartComponentCalculatableWrapper, bool> item in dictionary)
        {
            IChartComponentCalculatableWrapper key = item.Key;
            if (item.Value)
            {
                DebugTools.DevAssert(key.CustomErrorType != OleWrapperCustomErrorType.Placement, null);
                if (!key.IsConnectedToTheEngine)
                {
                    InternalAddCalculatableAndEvaluate(key);
                }
            }
            else
            {
                if (key.IsConnectedToTheEngine)
                {
                    base.FireCalculatableRemoved(key);
                }
                key.SetCustomError("chart_duplicate_limit_expression");
            }
        }
        foreach (KeyValuePair<ChartWrapperType, SimpleChartCalculatable> defaultLimit in _defaultLimits)
        {
            ChartWrapperType wrapperType = defaultLimit.Key;
            SimpleChartCalculatable value = defaultLimit.Value;
            if (_chartMinMaxCalc.GetLimit(wrapperType) != null)
            {
                if (value.IsConnectedToTheEngine)
                {
                    base.FireCalculatableRemoved(value);
                }
            }
            else if (!value.IsConnectedToTheEngine)
            {
                InternalAddCalculatableAndEvaluate(value);
            }
        }
        if (flag)
        {
            _chartMinMaxCalc.Commit();
        }
    }

        private void InternalAddCalculatableAndEvaluate(IOleBaseComponentCalculatableChild calc)
        {
            base.FireCalculatableAdded(calc);
            if (!(calc is IChartComponentCalculatableWrapper chartComponentCalculatableWrapper))
            {
                calc.Commit();
            }
            else
            {
                chartComponentCalculatableWrapper.HandleRequestEvaluationAux();
            }
        }
}
